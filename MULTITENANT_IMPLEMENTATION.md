# Multi-tenant — Paso 2: Usuarios atados a una empresa

## Documentación técnica de la implementación

> Esta es documentación **técnica/interna** (arquitectura, decisiones, cómo extenderlo). Para consumir los endpoints de `ManagementCompany` desde el front, ver [`COMPANY_API_DOCUMENTATION.md`](./COMPANY_API_DOCUMENTATION.md). Los cambios de `login`/`register` descritos acá todavía **no** están documentados para consumo de front — ese es un paso aparte, pendiente de pedir.

---

## 1. Objetivo

El sistema se renta a varias empresas administradoras (multi-tenant SaaS). El **Paso 1** ya dejó el agregado `ManagementCompanyAgg` (Bounded Context `Tenancy`) funcionando en back y front. Este **Paso 2** ata cada usuario (`ApplicationUser`) a una empresa, de modo que:

- Un usuario normal solo puede operar dentro de la empresa a la que pertenece.
- Un rol especial de plataforma (`Administrator`) no pertenece a ninguna empresa y puede operar sobre todas.
- Queda lista la plomería genérica para que, cuando un aggregate del dominio (`PhysicalStructure`, `Owner`, etc.) gane su propio `CompanyId` en un paso futuro, el aislamiento por empresa funcione automáticamente sin tocar cada Command Handler uno por uno.

**Importante:** este paso *no* filtra todavía ningún dato de negocio — no hay ningún aggregate del dominio con `CompanyId` aún. Es la base de Identity sobre la que se construirá el filtrado real.

---

## 2. Piezas nuevas de arquitectura

### 2.1 `ApplicationUser.CompanyId`

`Infraestructure/Identity/ApplicationUser.cs` — nueva propiedad `Guid? CompanyId`. Vive en Infraestructura (no en Domain) porque `ApplicationUser` ya es una extensión de `IdentityUser` de ASP.NET Core Identity, fuera de las reglas tácticas de DDD del dominio.

```csharp
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public Guid? CompanyId { get; set; } // null = usuario de plataforma (Administrator)
}
```

### 2.2 Claim `CompanyId` en el JWT

`IJwtTokenService.GenerateTokenAsync` gana un parámetro opcional `Guid? companyId`. Si tiene valor, se agrega como claim plano (mismo patrón que los roles):

```csharp
// Infraestructure/Identity/JwtTokenService.cs
if (companyId.HasValue)
{
    claims.Add(new Claim("CompanyId", companyId.Value.ToString()));
}
```

Un usuario `Administrator` nunca lleva este claim (su `CompanyId` es siempre `null`).

### 2.3 Nuevo puerto `ICurrentUserService`

`Domain/Ports/Identity/ICurrentUserService.cs` — el dominio/aplicación necesita saber "¿quién soy, de qué empresa soy, soy de plataforma?" en cualquier punto del pipeline (Command Handlers, `ApplicationService`) sin acoplarse a `HttpContext` directamente:

```csharp
public interface ICurrentUserService
{
    bool IsAuthenticated { get; }
    Guid? CompanyId { get; }
    bool IsPlatformAdministrator { get; }
}
```

Implementado en `Infraestructure/Identity/CurrentUserService.cs`, leyendo el `ClaimsPrincipal` ya validado por el middleware de JWT vía `IHttpContextAccessor` — no hace ninguna consulta a base de datos, todo sale de los claims del token:

```csharp
public Guid? CompanyId =>
    Guid.TryParse(User?.FindFirstValue("CompanyId"), out var id) ? id : null;

public bool IsPlatformAdministrator =>
    User != null && User.IsInRole(RolePermissionsPolicy.AdministratorRoleName);
```

Registrado como `Scoped` (vive por request) en `Infraestructure/Entity/DependencyInjection.cs`. Requiere `builder.Services.AddHttpContextAccessor();` en `Api/Program.cs`.

---

## 3. Reglas de negocio del registro (`RegisterCommandHandler`)

Toda la lógica de "quién puede registrar a quién, y con qué empresa" vive en `Application/Auth/Cqrs/Commands/RegisterCommand.cs`, método privado `ResolveTargetCompanyIdAsync`. El endpoint HTTP (`POST /auth/register`) sigue siendo `[AllowAnonymous]` — la seguridad real está en el handler, no en el atributo.

| # | Condición | Resultado |
|---|---|---|
| 1 | Rol pedido `Administrator` **y** no existe ningún Administrator todavía en el sistema | **Bootstrap** — permitido sin sesión, `CompanyId = null` |
| 2 | Rol pedido `Administrator` **y** ya existe al menos uno | Requiere que quien llama ya sea `Administrator` (`ICurrentUserService.IsPlatformAdministrator`); si no, `DomainException` |
| 3 | Quien llama es `Administrator` (y el rol pedido no es Administrator) | Usa `dto.CompanyId` (obligatorio); valida que la empresa exista vía `IManagementCompanyReadOnlyRepository.GetByIdAsync` |
| 4 | Quien llama está autenticado y pertenece a una empresa | `CompanyId` se toma de `ICurrentUserService.CompanyId` — **cualquier valor que venga en el DTO se ignora** |
| 5 | Cualquier otro caso (anónimo fuera del caso 1, o usuario sin empresa) | `DomainException` |

La regla clave de seguridad es la del caso 4: un usuario normal **nunca** puede elegir su propia empresa ni la de otro al invitar — siempre hereda la suya. Solo un `Administrator` de plataforma puede asignar una empresa explícita, porque él mismo no tiene ninguna.

`AuthService.RegisterAsync` (Infraestructura) aplica una segunda capa de la misma regla, por si se le llama sin pasar por el handler: `CompanyId = RolePermissionsPolicy.IsAdministrator(role) ? null : companyId` — un Administrator jamás queda atado a una empresa aunque se la pasen.

`IAuthService.AnyAdministratorExistsAsync()` (nuevo método) es lo que habilita el caso 1 — consulta si ya existe algún usuario con el rol `Administrator`.

---

## 4. Tenant-stamping genérico (`ApplicationService<ENT,DTO>`)

Este es el mecanismo que hará el "filtrado real" transparente cuando los aggregates del dominio ganen `CompanyId`. Reemplaza los comentarios `::TODO multi tenant` que ya estaban en el código (`Application/Base/Service/Implementation/ApplicationService.cs`), dejados como semilla por el equipo original.

```csharp
private static readonly PropertyInfo? CompanyIdProperty =
    typeof(DTO).GetProperty("CompanyId", typeof(Guid?));

private void StampCompany(DTO dto)
{
    if (CompanyIdProperty is null) return; // el DTO no es tenant-scoped, no hace nada

    if (CurrentUser.CompanyId is null)
        throw new DomainException("Tu usuario no pertenece a ninguna empresa; no puedes crear ni modificar este recurso.");

    CompanyIdProperty.SetValue(dto, CurrentUser.CompanyId); // sobrescribe, nunca confía en el cliente
}
```

Se invoca al inicio de `CreateAsync`, `UpdateAsync` y `CreateListAsync` — **antes** de mapear el DTO al agregado. Por reflexión, busca una propiedad `CompanyId` de tipo `Guid?` en el DTO:

- **Si no existe** (caso de hoy para `PhysicalStructureDto`, `OwnerDto`, `GuestDto`, `DocumentDto`, `ManagementCompanyDto`) → no hace nada. Cero efecto funcional todavía.
- **Si existe** → sobrescribe con la empresa del usuario autenticado, ignorando cualquier valor que haya mandado el cliente (cierra el hueco de que alguien intente crear un recurso "a nombre de" otra empresa).

El constructor de `ApplicationService<ENT,DTO>` ahora exige `ICurrentUserService`, y los 5 servicios de escritura existentes (`PhysicalStructureService`, `OwnerService`, `DocumentService`, `GuestService`, `ManagementCompanyService`) lo reciben y lo pasan al `base(...)` — cambio mecánico de una línea cada uno, sin lógica adicional.

### Cómo se activa para un aggregate nuevo (guía para el próximo paso)

Cuando llegue el momento de hacer tenant-scoped un aggregate (ej. `PhysicalStructure`), los pasos son:

1. Agregar `CompanyId` como parámetro del constructor de negocio del aggregate + invariante (`CompanyId == Guid.Empty` → `DomainException`).
2. Agregar `public Guid? CompanyId { get; set; }` al DTO correspondiente.
3. En el Mapper: pasar `src.CompanyId!.Value` al `ConstructUsing` e ignorar el campo en la dirección DTO→Agg (mismo patrón que cualquier otro campo con `private set`).
4. En el `EntityTypeConfiguration`: `builder.Property(p => p.CompanyId).IsRequired();`.
5. (Opcional, para que `GetAll`/`GetById`/`GetPaginated` también respeten la empresa) Agregar un `HasQueryFilter` en `EntityDBSets.OnModelCreating`, inyectando `ICurrentUserService` al `DbContext`.

Con solo los pasos 1-4, `StampCompany` ya empieza a actuar automáticamente en `Create`/`Update` — no hace falta tocar ningún Command Handler genérico.

---

## 5. Archivos nuevos / modificados

| Archivo | Cambio |
|---|---|
| `Infraestructure/Identity/ApplicationUser.cs` | + `CompanyId` |
| `Domain/Ports/Identity/IJwtTokenService.cs` / `Infraestructure/Identity/JwtTokenService.cs` | `GenerateTokenAsync` + `companyId`, claim `CompanyId` |
| `Domain/DomainShared/AuthResult.cs` | + `CompanyId` |
| `Domain/Ports/Identity/IAuthService.cs` / `Infraestructure/Identity/AuthService.cs` | `RegisterAsync` + `companyId`, + `AnyAdministratorExistsAsync()` |
| `Domain/Ports/Identity/ICurrentUserService.cs` (nuevo) | Puerto |
| `Infraestructure/Identity/CurrentUserService.cs` (nuevo) | Adaptador (`IHttpContextAccessor`) |
| `Api/Program.cs` | `AddHttpContextAccessor()` |
| `Infraestructure/Entity/DependencyInjection.cs` | registro de `ICurrentUserService` |
| `Application/Auth/Dtos/AuthRegisterDto.cs` / `AuthResponseDto.cs` | + `CompanyId` |
| `Application/Auth/Cqrs/Commands/RegisterCommand.cs` | reescrito — `ResolveTargetCompanyIdAsync` |
| `Application/Auth/Cqrs/Commands/LoginCommand.cs` | mapea `CompanyId` en la respuesta |
| `Api/Controllers/v1/AuthController.cs` | comentario XML de `Register` actualizado |
| `Application/Base/Service/Implementation/ApplicationService.cs` | `StampCompany`, constructor + `ICurrentUserService` |
| `PhysicalStructureService.cs`, `OwnerService.cs`, `DocumentService.cs`, `GuestService.cs`, `ManagementCompanyService.cs` | constructor + `ICurrentUserService` |
| `Infraestructure/Migrations/IdentityAppDb/..._AddCompanyIdToApplicationUser.cs` (nuevo) | columna `CompanyId` (nullable) en `AspNetUsers` |

---

## 6. Migración

Solo cambia el modelo de `IdentityAppDbContext` (ningún aggregate del dominio cambió):

```bash
dotnet ef migrations add AddCompanyIdToApplicationUser --project Infraestructure --startup-project Api --context IdentityAppDbContext -o Migrations/IdentityAppDb
dotnet ef database update --project Infraestructure --startup-project Api --context IdentityAppDbContext
```

---

## 7. Qué NO incluye este paso

- Ningún aggregate del dominio (`PhysicalStructure`, `Owner`, `Guest`, `Document`, `ManagementCompany`) tiene `CompanyId` todavía — `StampCompany` está cableado pero inactivo en la práctica.
- No hay `HasQueryFilter` de EF Core — no hay nada que filtrar aún en `GetAll`/`GetById`.
- `ManagementCompanyController` sigue sin restricción de rol (cualquier usuario autenticado puede administrar empresas) — se mantuvo así a propósito porque el front ya lo consume de esa forma.
- No hay documentación de cara al front para `login`/`register` con estos cambios — pendiente si/cuando el front vaya a integrarlos.

---

## 8. Verificación

- `dotnet build` → 0 errores.
- `dotnet test` → 185/185 (sin tests nuevos en este paso; Identity no tiene tests de dominio propios, la lógica de `RegisterCommandHandler` es de aplicación/orquestación).
- Migración generada y revisada (columna nullable, sin romper datos existentes).
- Pendiente de correr en un entorno con SQL Server real: smoke test de bootstrap → crear empresa → registrar usuario con `companyId` → login → verificar que el token trae el claim.

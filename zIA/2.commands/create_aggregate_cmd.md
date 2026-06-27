# Comando: Crear Nuevo Agregado DDD

## Rol del Agente
Eres un arquitecto de software especializado en C# / .NET 10 y Domain-Driven Design. Tu misión es generar **todos los archivos necesarios** para incorporar un nuevo Agregado al proyecto `NET.10.backend.seed`, respetando estrictamente los patrones, namespaces, convenciones y estructura de capas ya existentes en el repositorio.

> **Referencia obligatoria:** Antes de generar cualquier código, consulta los templates y reglas en `./zIA/3.skills/architecture_ddd_dotnet_skill.md` y las reglas de revisión en `./zIA/3.skills/instrucciones_code_review_ddd_skill.md`.

---

## PASO 1 — Recopilar Información (OBLIGATORIO, no saltar)

Antes de generar ningún archivo, **pregunta al usuario** lo siguiente y espera su respuesta completa:

### 1.1 — Identificación del Agregado
```
1. ¿Cuál es el nombre del Bounded Context al que pertenece este agregado?
   (Ejemplo: Properties, Tenants, Payments, Maintenance)

2. ¿Cuál es el nombre del Agregado en Lenguaje Ubicuo del negocio?
   (Ejemplo: ResidentialUnit, MaintenanceRequest, RentContract)
   ⚠️ Usa sustantivos del negocio, nunca términos técnicos como "Entity", "Manager", "Handler".
```

### 1.2 — Campos del Agregado
```
3. Lista los campos propios del Agregado con su tipo y si son requeridos:
   Formato: NombreCampo : Tipo : Requerido(si/no)
   Ejemplo:
     - Name        : string  : si
     - Description : string  : no
     - UnitCount   : int     : si
     - Price       : decimal : si

   ℹ️ No incluyas: Id, Status, CreatedAt, UpdateAt (se heredan de Entity automáticamente).
```

### 1.3 — Value Objects
```
4. ¿El agregado contiene Value Objects?
   Para cada uno indica:
     a) Nombre del VO (en Lenguaje Ubicuo)
     b) Relación: OwnsOne (uno solo) u OwnsMany (colección)
     c) Campos del VO con su tipo y si son requeridos
   
   Ejemplo:
     - LocationValueObject [OwnsOne]
         · Street   : string : si
         · City     : string : si
         · Country  : string : si
     - MaintenanceItemValueObject [OwnsMany]
         · Description : string  : si
         · Cost        : decimal : si
```

### 1.3b — Entidades Hijas (Child Entities)
```
4b. ¿El agregado contiene Entidades hijas?
    (A diferencia de los Value Objects, las Entidades hijas heredan de Entity
    y tienen identidad propia — Id, Status, CreatedAt, UpdateAt)
    
    Para cada una indica:
      a) Nombre de la Entidad (en Lenguaje Ubicuo), con sufijo Entity
      b) Relación: siempre OwnsMany (colección)
      c) Campos de la Entidad con su tipo y si son requeridos
      d) Reglas de validación específicas para cada campo
    
    Ejemplo:
      - TowerEntity [OwnsMany]
          · Number : string : si (max 20 chars)
          · Floors : int    : si (mayor a 0)
      - CommonAreaEntity [OwnsMany]
          · Name        : string : si (max 150 chars)
          · Description  : string : si (max 500 chars)
```

### 1.4 — Reglas de Invarianza
```
5. Define las reglas de invarianza del agregado (validaciones en ExcecuteDomainInvariants):
   Formato: Campo | Condición | Mensaje de error en español
   
   Ejemplo:
     - Name      | IsNullOrWhiteSpace    | "El nombre es obligatorio."
     - Name      | Length > 150          | "El nombre no puede exceder 150 caracteres."
     - UnitCount | valor <= 0            | "La cantidad de unidades debe ser mayor a cero."
     - Price     | valor < 0             | "El precio no puede ser negativo."
```

### 1.5 — Nombre de la Tabla en BD
```
6. ¿Cómo se llamará la tabla principal en la base de datos?
   (Ejemplo: ResidentialUnits, MaintenanceRequests)
   
   Si el agregado tiene Value Objects con OwnsOne u OwnsMany,
   indica el nombre de tabla para cada uno:
   Ejemplo:
     - LocationValueObject → tabla: "UnitLocations"
     - MaintenanceItemValueObject → tabla: "MaintenanceItems"
```

---

## PASO 2 — Confirmación antes de generar

Una vez que el usuario haya respondido todo, presenta un resumen así:

```
📋 RESUMEN DEL AGREGADO A GENERAR:

Bounded Context  : {BoundedContext}
Agregado (Root)  : {NombreAgg}
Tabla BD         : {NombreTabla}

Campos propios:
  {lista de campos con tipo y requerido}

Value Objects:
  {lista de VOs con sus campos y relación}

Invariantes:
  {lista de reglas}

Archivos que se crearán:
  ✅ Domain/BoundedContext/{BC}/Aggregates/{Nombre}Agg.cs
  ✅ Domain/BoundedContext/{BC}/Aggregates/{VO}ValueObject.cs  (por cada VO)
  ✅ Domain/BoundedContext/{BC}/Aggregates/{Entity}Entity.cs    (por cada Entidad hija)
  ✅ Domain/BoundedContext/{BC}/Events/DomainEvents.cs
  ✅ Domain/Ports/Repository/I{Nombre}Repository.cs
  ✅ Domain/Ports/Repository/I{Nombre}ReadOnlyRepository.cs
  ✅ Application/{Nombre}/Dtos/{Nombre}Dto.cs
  ✅ Application/{Nombre}/Dtos/{VO}Dto.cs                       (por cada VO con OwnsMany — archivo separado)
  ✅ Application/{Nombre}/Mapper/{Nombre}Mapper.cs
  ✅ Application/{Nombre}/Service/I{Nombre}Service.cs
  ✅ Application/{Nombre}/Service/{Nombre}Service.cs
  ✅ Application/{Nombre}/Service/I{Nombre}ReadOnlyService.cs
  ✅ Application/{Nombre}/Service/{Nombre}ReadOnlyService.cs
  ✅ Application/{Nombre}/Validator/{Nombre}Validator.cs
  ✅ Application/{Nombre}/Cqrs/Commands/Create{Nombre}Command.cs
  ✅ Infraestructure/Entity/Context/EntityConfigurations/{Nombre}Config.cs
  ✅ Infraestructure/Entity/Repository/{BC}/{Nombre}Repository.cs
  ✅ Infraestructure/Entity/Repository/{BC}/{Nombre}ReadOnlyRepository.cs
  ✅ Api/Controllers/v1/{Nombre}Controller.cs

Archivos a MODIFICAR:
  ⚠️  Infraestructure/Entity/Context/EntityDBSets.cs            (DbSet<> + ApplyConfiguration)
  ⚠️  Infraestructure/Entity/DependencyInjection.cs             (AddScoped repositorio escritura + lectura)
  ⚠️  Application/DependencyInyection.cs                        (RegisterMediatrAbstractService + ReadOnly + Validator)

¿Confirmas la generación? (sí/no)
```

Espera confirmación explícita del usuario antes de continuar.

---

## PASO 3 — Generación de Archivos

Genera **todos** los archivos en el orden indicado abajo. Usa los **templates del skill** (`./zIA/3.skills/architecture_ddd_dotnet_skill.md`) para cada artefacto. Por cada archivo muestra la ruta completa y el código completo.

### Orden de generación:

| # | Capa | Archivo a crear | Sección del skill |
|---|---|---|---|
| 1 | Domain | `Domain/BoundedContext/{BC}/Aggregates/{Nombre}Agg.cs` | Aggregate Roots |
| 2 | Domain | `Domain/BoundedContext/{BC}/Aggregates/{VO}ValueObject.cs` _(por cada VO)_ | Value Objects |
| 2b | Domain | `Domain/BoundedContext/{BC}/Aggregates/{Entity}Entity.cs` _(por cada Entidad hija)_ | Entidades Hijas |
| 3 | Domain | `Domain/BoundedContext/{BC}/Events/DomainEvents.cs` | Domain Events |
| 4 | Domain/Ports | `Domain/Ports/Repository/I{Nombre}Repository.cs` | Repositorios de Escritura |
| 5 | Domain/Ports | `Domain/Ports/Repository/I{Nombre}ReadOnlyRepository.cs` | Repositorios de Solo Lectura |
| 6 | Application | `Application/{Nombre}/Dtos/{Nombre}Dto.cs` | DTOs |
| 7 | Application | `Application/{Nombre}/Dtos/{VO}Dto.cs` _(por cada VO OwnsMany)_ | DTOs |
| 8 | Application | `Application/{Nombre}/Mapper/{Nombre}Mapper.cs` | Mapper (AutoMapper) |
| 9 | Application | `Application/{Nombre}/Service/I{Nombre}Service.cs` | Application Services — Command Side |
| 10 | Application | `Application/{Nombre}/Service/{Nombre}Service.cs` | Application Services — Command Side |
| 11 | Application | `Application/{Nombre}/Service/I{Nombre}ReadOnlyService.cs` | Application Services — Query Side |
| 12 | Application | `Application/{Nombre}/Service/{Nombre}ReadOnlyService.cs` | Application Services — Query Side |
| 13 | Application | `Application/{Nombre}/Validator/{Nombre}Validator.cs` | Validator (FluentValidation) |
| 14 | Application | `Application/{Nombre}/Cqrs/Commands/Create{Nombre}Command.cs` | CQRS Commands (MediatR) |
| 15 | Infraestructure | `Infraestructure/Entity/Context/EntityConfigurations/{Nombre}Config.cs` | Entity Configuration (Fluent API) |
| 16 | Infraestructure | `Infraestructure/Entity/Repository/{BC}/{Nombre}Repository.cs` | Repositorios de Escritura |
| 17 | Infraestructure | `Infraestructure/Entity/Repository/{BC}/{Nombre}ReadOnlyRepository.cs` | Repositorios de Solo Lectura |
| 18 | Api | `Api/Controllers/v1/{Nombre}Controller.cs` | Controllers |

---

## PASO 4 — Modificaciones a Archivos Existentes

Muestra los **diffs exactos** que deben aplicarse a los archivos existentes. Consulta la sección correspondiente del skill para el formato correcto de cada registro.

### 4.1 — `Infraestructure/Entity/Context/EntityDBSets.cs`
Consultar sección **Entity Configuration (Fluent API) → Registro en EntityDBSets.cs** del skill:
- Agregar `using Domain.BoundedContext.{BoundedContext};`
- Agregar `DbSet<{Nombre}Agg>`
- Agregar `ApplyConfiguration(new {Nombre}Config())` en `OnModelCreating`

### 4.2 — `Infraestructure/Entity/DependencyInjection.cs`
Consultar secciones **Repositorios de Escritura → Registro DI** y **Repositorios de Solo Lectura → Registro DI** del skill:
- Agregar `AddScoped<I{Nombre}Repository, {Nombre}Repository>()`
- Agregar `AddScoped<I{Nombre}ReadOnlyRepository, {Nombre}ReadOnlyRepository>()`
- Agregar `using Infraestructure.Repository.{BoundedContext};`

### 4.3 — `Application/DependencyInyection.cs`
Consultar secciones **Application Services Command → Registro DI**, **Application Services Query → Registro DI** y **Validator → Registro DI** del skill:
- Agregar `RegisterMediatrAbstractService<{Nombre}Service, {Nombre}Dto, {Nombre}Agg, I{Nombre}Service>()`
- Agregar `RegisterMediatrAbstractReadOnlyService<{Nombre}ReadOnlyService, {Nombre}Dto, {Nombre}Agg, I{Nombre}ReadOnlyService>()`
- Agregar `AddScoped<IValidator<{Nombre}Dto>, {Nombre}Validator>()`

---

## PASO 5 — Recordatorio de Migración

Al finalizar la generación de todos los archivos, recuerda al usuario ejecutar:

```bash
# Crear la migración
dotnet ef migrations add Add{Nombre}Aggregate --project Infraestructure --startup-project Api

# Aplicar a la base de datos
dotnet ef database update --project Infraestructure --startup-project Api
```

---

## Checklist de Calidad — Verificación Interna del Agente

Antes de entregar el código generado, el agente debe verificar internamente:

**Dominio:**
- [ ] El agregado hereda de `AggregateRoot` (no de `Entity` directamente)
- [ ] El constructor vacío existe para EF (sin lógica de negocio)
- [ ] `ExcecuteDomainInvariants()` se llama al final del constructor con parámetros
- [ ] Todas las propiedades del dominio tienen `get; private set;` (ningún setter público)
- [ ] Todos los VOs son `record : ValueObject` con Guard Clauses en el constructor
- [ ] `I{Nombre}Repository` e `I{Nombre}ReadOnlyRepository` están en `Domain/Ports/Repository/`
- [ ] Cada Entidad hija hereda de `Entity` (NO de `AggregateRoot`) y tiene sufijo `Entity`
- [ ] Cada Entidad hija tiene constructor vacío para ORM, constructor de negocio y constructor de reconstrucción (con `Guid id`)
- [ ] Cada Entidad hija tiene métodos de validación privados estáticos y un método `Update(...)`
- [ ] El Aggregate Root tiene un método `Update{Colección}(IEnumerable<{Entity}>)` por cada colección de entidades hijas

**Infraestructura:**
- [ ] El Fluent API mapea `Status`, `CreatedAt`, `UpdateAt` (campos de `Entity`)
- [ ] Cada VO tiene `ToTable`, `WithOwner().HasForeignKey(...)`, `HasKey("Id")`
- [ ] Cada Entidad hija en OwnsMany tiene `ToTable`, `WithOwner().HasForeignKey(...)`, `Property(t => t.Id).ValueGeneratedNever()`, `HasKey(t => t.Id)`, y mapeo de `Status`, `CreatedAt`, `UpdateAt`
- [ ] El DbSet y `ApplyConfiguration` se agregan en `EntityDBSets.cs` (NO en EntityDbContext.cs)
- [ ] El repositorio de escritura implementa `BaseRepositiry<TAgg>` e `I{Nombre}Repository`
- [ ] El repositorio de lectura implementa `BaseReadOnlyRepository<TAgg>` e `I{Nombre}ReadOnlyRepository`
- [ ] Ambos repositorios registrados en `DependencyInjection.cs` de Infraestructura

**Application:**
- [ ] El DTO de VO con OwnsMany está en **archivo separado** (`{VO}Dto.cs`)
- [ ] El Mapper usa `ConstructUsing` para DTO → Agg (respetando el constructor de negocio)
- [ ] El Mapper usa `ForMember` + `Select()` inline para Agg → DTO (incluyendo colecciones OwnsMany)
- [ ] `{Nombre}ReadOnlyService` reutiliza `{Nombre}Mapper.Expresion(cnf)` (mismo mapper)
- [ ] `RegisterMediatrAbstractService` Y `RegisterMediatrAbstractReadOnlyService` registrados en Application DI
- [ ] Validator registrado en `RegisterValidators`

**API:**
- [ ] El Controller hereda de `BaseController<{Nombre}Agg, {Nombre}Dto>`

**Migración:**
- [ ] Se indica el comando de migración EF al usuario

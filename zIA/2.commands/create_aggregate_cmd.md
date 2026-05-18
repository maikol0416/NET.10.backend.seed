# Skill: Crear Nuevo Agregado DDD

## Rol del Agente
Eres un arquitecto de software especializado en C# / .NET 10 y Domain-Driven Design. Tu misión es generar **todos los archivos necesarios** para incorporar un nuevo Agregado al proyecto `NET.10.backend.seed`, respetando estrictamente los patrones, namespaces, convenciones y estructura de capas ya existentes en el repositorio.

> **Referencia obligatoria:** Antes de generar cualquier código, verifica la estructura real del proyecto y los ejemplos de implementación en `./zIA/skills/architecture_ddd_dotnet_skill.md` y las reglas de revisión en `./zIA/skills/instrucciones_code_review_ddd.md`.

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
  ✅ Domain/BoundedContext/{BC}/Events/DomainEvents.cs
  ✅ Domain/Ports/Repository/I{Nombre}Repository.cs
  ✅ Application/{Nombre}/Dtos/{Nombre}Dto.cs
  ✅ Application/{Nombre}/Service/I{Nombre}Service.cs
  ✅ Application/{Nombre}/Service/{Nombre}Service.cs
  ✅ Application/{Nombre}/Mapper/{Nombre}Mapper.cs
  ✅ Application/{Nombre}/Validator/{Nombre}Validator.cs
  ✅ Application/{Nombre}/Cqrs/Commands/Create{Nombre}Command.cs
  ✅ Infraestructure/Entity/Context/EntityConfigurations/{Nombre}Config.cs
  ✅ Infraestructure/Entity/Repository/{BC}/{Nombre}Repository.cs

Archivos a MODIFICAR:
  ⚠️  Infraestructure/Entity/Context/EntityDbContext.cs         (ApplyConfiguration)
  ⚠️  Infraestructure/Entity/Context/EntityDbContext.dbsets.cs  (DbSet<>)
  ⚠️  Infraestructure/Entity/DependencyInjection.cs             (AddScoped repositorio)
  ⚠️  Application/DependencyInyection.cs                        (RegisterMediatrAbstractService + Validator)

¿Confirmas la generación? (sí/no)
```

Espera confirmación explícita del usuario antes de continuar.

---

## PASO 3 — Generación de Archivos

Genera **todos** los archivos en el orden indicado. Por cada archivo muestra la ruta completa y el código completo.

### Convenciones obligatorias:
- Nombre del agregado: `{Nombre}Agg` (ej. `ResidentialUnitAgg`)
- Value Objects: `{Nombre}ValueObject` (ej. `LocationValueObject`)
- Namespace Domain BC: `Domain.BoundedContext.{BoundedContext}`
- Namespace Application: `Application.{Nombre}` / `Application.Dto` / `Application.Service` / `Application.Validator`
- Namespace Infraestructure: `Infraestructure.Repository.{BoundedContext}` / `Infraestructure.Entity`
- Los agregados siempre deben heredar de `AggregateRoot` (que ya hereda `Entity`)
- Value Objects siempre `record` heredando de `ValueObject`
- DomainException para todas las validaciones
- `ExcecuteDomainInvariants()` siempre llamado al final del constructor con parámetros

---

### 3.1 — Aggregate Root
**Ruta:** `Domain/BoundedContext/{BC}/Aggregates/{Nombre}Agg.cs`

```csharp
using Domain.DomainShared;

namespace Domain.BoundedContext.{BoundedContext};

public class {Nombre}Agg : AggregateRoot
{
    // Constructor para ORM (Entity Framework)
    public {Nombre}Agg() { }

    // Constructor de negocio
    public {Nombre}Agg(
        {parametros_campos_propios},
        {parametros_value_objects}
        ) : base()
    {
        // Asignar campos propios
        {Campo} = {parametro};
        // Asignar value objects
        {VO} = {parametroVO};

        ExcecuteDomainInvariants();
    }

    // Campos propios (get; private set;)
    public {Tipo} {Campo} { get; private set; }

    // Value Objects
    public {VO}ValueObject {VO} { get; private set; }
    public List<{VOMany}ValueObject> {VOs} { get; private set; }

    protected override void ExcecuteDomainInvariants()
    {
        // Reglas de invarianza definidas por el usuario
        if (string.IsNullOrWhiteSpace({Campo}))
            throw new DomainException("{Mensaje}");

        if ({Campo}.Length > {max})
            throw new DomainException("{Mensaje}");

        if ({NumericCampo} <= 0)
            throw new DomainException("{Mensaje}");

        if ({VO} == null)
            throw new DomainException("{Mensaje}");
    }
}
```

---

### 3.2 — Value Object(s)
**Ruta:** `Domain/BoundedContext/{BC}/Aggregates/{VO}ValueObject.cs`  
_(Un archivo por cada Value Object declarado)_

```csharp
using Domain.DomainShared;

namespace Domain.BoundedContext.{BoundedContext};

public record {VO}ValueObject : ValueObject
{
    public {VO}ValueObject(
        {parametros_campos_vo}
        )
    {
        // Guard Clauses para cada campo requerido
        if (string.IsNullOrEmpty({campo}))
            throw new DomainException("{Campo} es obligatorio.");

        {Campo} = {campo};
        // ... resto de campos
    }

    public {Tipo} {Campo} { get; private set; }
    // ... resto de propiedades
}
```

---

### 3.3 — Domain Events
**Ruta:** `Domain/BoundedContext/{BC}/Events/DomainEvents.cs`

```csharp
using Domain.DomainShared;
using Domain.Ports.Events.Properties;

namespace Domain.{BoundedContext}.Events;

// Evento: {Nombre} creado (registro pasado)
public record {Nombre}CreatedDomainEvent(Guid {Nombre}Id, string {CampoRepresentativo}) 
    : DomainEvent, IDomainEvent
{
}

// Agrega más eventos según las acciones de negocio relevantes
// public record {Nombre}UpdatedDomainEvent(Guid {Nombre}Id) : DomainEvent, IDomainEvent { }
```

---

### 3.4 — Interfaz de Repositorio (Puerto del Dominio)
**Ruta:** `Domain/Ports/Repository/I{Nombre}Repository.cs`

```csharp
using Domain.Ports.Repository.Base;
using Domain.BoundedContext.{BoundedContext};

namespace Domain.Ports;

public interface I{Nombre}Repository : IBaseRepository<{Nombre}Agg>
{
}
```

---

### 3.5 — DTO
**Ruta:** `Application/{Nombre}/Dtos/{Nombre}Dto.cs`

```csharp
namespace Application.Dto;

public class {Nombre}Dto
{
    // Campos planos del agregado
    public {Tipo} {Campo} { get; set; }

    // Campos aplanados de los Value Objects (sin anidar)
    // OwnsOne: aplana los campos del VO directamente en el DTO
    public {Tipo} {VoCampo} { get; set; }

    // OwnsMany: lista separada o simplificada según necesidad
}
```

---

### 3.6 — Interfaz del Application Service
**Ruta:** `Application/{Nombre}/Service/I{Nombre}Service.cs`

```csharp
using Application.Base;
using Application.Dto;
using Domain.BoundedContext.{BoundedContext};

namespace Application.Service;

public interface I{Nombre}Service : IApplicationService<{Nombre}Agg, {Nombre}Dto>
{
}
```

---

### 3.7 — Mapper (AutoMapper estático)
**Ruta:** `Application/{Nombre}/Mapper/{Nombre}Mapper.cs`

```csharp
using Application.Dto;
using AutoMapper;
using Domain.BoundedContext.{BoundedContext};

namespace Application.Service;

public static class {Nombre}Mapper
{
    public static void Expresion(IMapperConfigurationExpression cnf)
    {
        // DTO → Aggregate (usa el constructor de negocio del agregado)
        cnf.CreateMap<{Nombre}Dto, {Nombre}Agg>()
            .ConstructUsing(src => new {Nombre}Agg(
                src.{Campo},
                // Value Objects construidos dentro del mapper
                new {VO}ValueObject(
                    src.{VoCampo},
                    src.{VoCampo2}
                )
                // OwnsMany: new List<{VOMany}ValueObject> { ... }
            ));

        // Aggregate → DTO (proyección plana)
        cnf.CreateMap<{Nombre}Agg, {Nombre}Dto>()
            .ForMember(dest => dest.{Campo}, opt => opt.MapFrom(src => src.{Campo}))
            // Value Object owned:
            .ForMember(dest => dest.{VoCampo}, opt => opt.MapFrom(src => src.{VO}.{VoCampo}));
    }
}
```

---

### 3.8 — Application Service
**Ruta:** `Application/{Nombre}/Service/{Nombre}Service.cs`

```csharp
using Application.Base;
using Application.Dto;
using Domain.BoundedContext.{BoundedContext};
using Domain.Ports;

namespace Application.Service;

public class {Nombre}Service 
    : ApplicationService<{Nombre}Agg, {Nombre}Dto>, I{Nombre}Service
{
    public {Nombre}Service(I{Nombre}Repository repository) : base(repository)
    {
        CreateMapperExpresion<{Nombre}Agg, {Nombre}Dto>(cnf =>
        {
            {Nombre}Mapper.Expresion(cnf);
        });
    }
}
```

---

### 3.9 — Validator (FluentValidation)
**Ruta:** `Application/{Nombre}/Validator/{Nombre}Validator.cs`

```csharp
using Application.Dto;
using Domain.Ports;
using FluentValidation;

namespace Application.Validator;

public class {Nombre}Validator : AbstractValidator<{Nombre}Dto>
{
    private readonly I{Nombre}Repository _{nombre}Repository;

    public {Nombre}Validator(I{Nombre}Repository {nombre}Repository)
    {
        _{nombre}Repository = {nombre}Repository;

        // Regla de ejemplo: campo requerido
        RuleFor(x => x.{Campo})
            .NotEmpty()
            .WithErrorCode("{Campo}Empty")
            .WithMessage("El campo {Campo} es obligatorio.")
            .WithName(nameof({Nombre}Dto.{Campo}));

        // Agrega más reglas según los campos del DTO
    }
}
```

---

### 3.10 — CQRS Command
**Ruta:** `Application/{Nombre}/Cqrs/Commands/Create{Nombre}Command.cs`

```csharp
using Application.Dto;
using Domain.BoundedContext.{BoundedContext};
using Domain.Ports;
using MediatR;

namespace Application.{Nombre}.Commands;

public record Create{Nombre}Command({Nombre}Dto {Nombre}Dto) : IRequest<Guid>;

public class Create{Nombre}CommandHandler
    : IRequestHandler<Create{Nombre}Command, Guid>
{
    private readonly I{Nombre}Repository _repository;

    public Create{Nombre}CommandHandler(I{Nombre}Repository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(
        Create{Nombre}Command request,
        CancellationToken cancellationToken)
    {
        // Construir el agregado desde el DTO usando el constructor de negocio
        var agg = new {Nombre}Agg(
            request.{Nombre}Dto.{Campo},
            new {VO}ValueObject(
                request.{Nombre}Dto.{VoCampo}
            )
        );

        await _repository.CreateAsync(agg);
        return agg.Id;
    }
}
```

---

### 3.11 — Fluent API Configuration
**Ruta:** `Infraestructure/Entity/Context/EntityConfigurations/{Nombre}Config.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.BoundedContext.{BoundedContext};

namespace Infraestructure.Entity;

public class {Nombre}Config : IEntityTypeConfiguration<{Nombre}Agg>
{
    public void Configure(EntityTypeBuilder<{Nombre}Agg> builder)
    {
        builder.ToTable("{NombreTabla}");
        builder.HasKey(p => p.Id);

        // ✅ Campos heredados de Entity (SIEMPRE incluir)
        builder.Property(p => p.Status)
            .IsRequired()
            .HasMaxLength(1);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdateAt)
            .IsRequired(false);

        // Campos propios del agregado
        builder.Property(p => p.{Campo})
            .IsRequired()          // si el campo es requerido
            .HasMaxLength({max});   // si es string

        // OwnsOne: Value Object de 1 a 1
        builder.OwnsOne(p => p.{VO}, voBuilder =>
        {
            voBuilder.ToTable("{NombreTablaVO}");
            voBuilder.WithOwner().HasForeignKey("{Nombre}Id");
            voBuilder.Property<int>("Id");
            voBuilder.HasKey("Id");

            voBuilder.Property(v => v.{VoCampo})
                .HasColumnName("{VoCampo}")
                .IsRequired()
                .HasMaxLength({max});
        });

        // OwnsMany: Value Object de 1 a muchos
        builder.OwnsMany(p => p.{VOs}, voBuilder =>
        {
            voBuilder.ToTable("{NombreTablaVOMany}");
            voBuilder.WithOwner().HasForeignKey("{Nombre}Id");
            voBuilder.Property<int>("Id");
            voBuilder.HasKey("Id");

            voBuilder.Property(v => v.{VoCampo})
                .HasColumnName("{VoCampo}")
                .IsRequired()
                .HasMaxLength({max});
        });
    }
}
```

---

### 3.12 — Repository Implementation
**Ruta:** `Infraestructure/Entity/Repository/{BC}/{Nombre}Repository.cs`

```csharp
using Domain.Ports;
using Domain.BoundedContext.{BoundedContext};
using Infraestructure.Repository.Shared;

namespace Infraestructure.Repository.{BoundedContext};

public class {Nombre}Repository : BaseRepositiry<{Nombre}Agg>, I{Nombre}Repository
{
    public {Nombre}Repository(IEntityDbContext entityDbContext)
        : base(entityDbContext)
    {
    }
}
```

---

### 3.13 — Controller
**Ruta:** `Api/Controllers/v1/{Nombre}Controller.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Dto;
using Domain.BoundedContext.{BoundedContext};
using FluentValidation;

namespace Api.Controllers;

[Route("api/[controller]")]
public class {Nombre}Controller 
    : BaseController<{Nombre}Agg, {Nombre}Dto>
{
    public {Nombre}Controller(
        IValidator<{Nombre}Dto> validator, 
        IMediator mediator)
        : base(validator, mediator)
    {
    }
}
```

---

## PASO 4 — Modificaciones a Archivos Existentes

Muestra los **diffs exactos** que deben aplicarse a los archivos existentes:

### 4.1 — `Infraestructure/Entity/Context/EntityDbContext.cs`
Agregar en `OnModelCreating`:
```csharp
modelBuilder.ApplyConfiguration(new {Nombre}Config());
```

### 4.2 — `Infraestructure/Entity/Context/EntityDbContext.dbsets.cs`
Agregar:
```csharp
public DbSet<{Nombre}Agg> {Nombre} { get; set; }
```

### 4.3 — `Infraestructure/Entity/DependencyInjection.cs`
Agregar en el método `AddDependencyInjectionInfrastructureEf`:
```csharp
services.AddScoped<I{Nombre}Repository, {Nombre}Repository>();
```
Agregar el using correspondiente:
```csharp
using Infraestructure.Repository.{BoundedContext};
```

### 4.4 — `Application/DependencyInyection.cs`
Agregar en `AddDependencyInjectionApplication`:
```csharp
services.RegisterMediatrAbstractService<{Nombre}Service, {Nombre}Dto, {Nombre}Agg, I{Nombre}Service>();
```
Agregar en `RegisterValidators`:
```csharp
services.AddScoped<IValidator<{Nombre}Dto>, {Nombre}Validator>();
```

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

## Reglas de Calidad — Checklist Interno del Agente

Antes de entregar el código generado, el agente debe verificar internamente:

- [ ] El agregado hereda de `AggregateRoot` (no de `Entity` directamente)
- [ ] El constructor vacío existe para EF (sin lógica de negocio)
- [ ] `ExcecuteDomainInvariants()` se llama al final del constructor con parámetros
- [ ] Todas las propiedades del dominio tienen `get; private set;` (ningún setter público)
- [ ] Todos los VOs son `record : ValueObject` con Guard Clauses en el constructor
- [ ] El Fluent API mapea `Status`, `CreatedAt`, `UpdateAt` (campos de `Entity`)
- [ ] Cada VO tiene `ToTable`, `WithOwner().HasForeignKey(...)`, `HasKey("Id")` 
- [ ] El repositorio implementa `BaseRepositiry<TAgg>` e `I{Nombre}Repository`
- [ ] El contrato `I{Nombre}Repository` está en `Domain/Ports/Repository/`
- [ ] El Mapper usa `ConstructUsing` para DTO → Agg (respetando el constructor de negocio)
- [ ] El Controller hereda de `BaseController<{Nombre}Agg, {Nombre}Dto>`
- [ ] Los 4 archivos existentes tienen sus modificaciones indicadas con diff claro
- [ ] Se indica el comando de migración EF al usuario

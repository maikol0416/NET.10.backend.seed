1. LO QUE DEBES HACER (Arquitectura y Tácticas DDD)
Objetivo del Agente: Eres un arquitecto de software especializado en C# / .NET (o el lenguaje aplicable) y Domain-Driven Design (DDD). Tu misión es generar código donde el Dominio sea rico, agnóstico de la infraestructura y refleje fielmente el Lenguaje Ubicuo.

Reglas de Diseño Estratégico:

Bounded Contexts: Antes de proponer una solución, asegúrate de identificar a qué Bounded Context pertenece. No mezcles conceptos de Ventas con conceptos de Envíos en la misma clase.

Lenguaje Ubicuo: Usa nombres de clases y métodos que los expertos del negocio entiendan. Nunca uses términos puramente técnicos en el dominio (ej. usa PromoteToManager() en lugar de UpdateStatus(2)).

C# ejemplo arquitectura de solution (estructura real del proyecto — incluyendo CQRS ReadOnly)
```
NET.10.backend.seed.slnx
├── Domain/                                    # Capa de Dominio
│   ├── DomainShared/                          # Clases base compartidas por todos los BC
│   │   ├── Entity.cs                          # Base: Id, Status, CreatedAt, UpdateAt
│   │   ├── AggregateRoot.cs                   # Base: hereda Entity + gestión de DomainEvents
│   │   ├── ValueObject.cs                     # Base para Value Objects (record)
│   │   ├── DomainEvent.cs                     # Clase base de eventos de dominio
│   │   ├── DomainException.cs                 # Excepción de dominio
│   │   └── StatusEnum.cs                      # Enum de estados
│   ├── BoundedContext/                        # Un subdirectorio por cada Bounded Context
│   │   └── Properties/                        # BC: Propiedades / Inmuebles
│   │       ├── Aggregates/
│   │       │   ├── PhysicalStructureAgg.cs    # Aggregate Root
│   │       │   ├── LocationValueObject.cs     # Value Object (owned by PhysicalStructure)
│   │       │   └── CommonAreaValueObject.cs   # Value Object (owned many)
│   │       └── Events/
│   │           └── DomainEvents.cs            # Eventos de dominio del BC Properties
│   └── Ports/                                 # Contratos (interfaces) del dominio
│       ├── Repository/
│       │   ├── Base/
│       │   │   ├── IBaseRepository.cs         # Contrato genérico de repositorio (escritura)
│       │   │   ├── IBaseReadOnlyRepository.cs # Contrato genérico repositorio solo lectura
│       │   │   ├── IEntityDbContext.cs        # Contrato DbContext de escritura
│       │   │   └── IEntityReadOnlyDbContext.cs# Contrato DbContext solo lectura (NoTracking)
│       │   ├── IPhysicalStructureRepository.cs
│       │   └── IPhysicalStructureReadOnlyRepository.cs
│       └── Events/
│           └── Properties/
│               └── IDomainEvent.cs
│
├── Application/                               # Capa de Aplicación (Orquestación)
│   ├── DependencyInyection.cs
│   ├── Base/
│   │   ├── Cqrs/
│   │   │   ├── Command/
│   │   │   │   ├── CreateCommand.cs
│   │   │   │   ├── UpdateCommand.cs
│   │   │   │   └── DeleteCommand.cs
│   │   │   └── Query/
│   │   │       ├── GetByIdQuery.cs            # Query + Handler (Query side CQRS)
│   │   │       └── GetAllQuery.cs             # Query + Handler (Query side CQRS)
│   │   └── Service/
│   │       ├── IApplicationService.cs         # Contrato base Command side
│   │       ├── IApplicationReadOnlyService.cs # Contrato base Query side
│   │       └── Implementation/
│   │           ├── ApplicationService.cs      # Implementación base RUD
│   │           ├── ApplicationServiceMapper.cs# Mapper compartido (AutoMapper)
│   │           └── ApplicationReadOnlyService.cs # Implementación base solo lectura
│   └── PhysicalStructure/
│       ├── Cqrs/Commands/
│       │   └── CreatePhysicalStructureCommand.cs
│       ├── Dtos/
│       │   ├── PhysicalStructureDto.cs
│       │   └── CommonAreaDto.cs               # DTO por VO con OwnsMany (archivo separado)
│       ├── Mapper/
│       │   └── PhysicalStructureMapper.cs
│       ├── Service/
│       │   ├── IPhysicalStructureService.cs
│       │   ├── PhysicalStructureService.cs
│       │   ├── IPhysicalStructureReadOnlyService.cs  # Contrato Query side
│       │   └── PhysicalStructureReadOnlyService.cs   # Implementación Query side
│       └── Validator/
│           └── PhysicalStructureValidator.cs
│
├── Infraestructure/
│   ├── Entity/
│   │   ├── Context/
│   │   │   ├── EntityDBSets.cs                # Clase base abstracta: DbSets + OnModelCreating
│   │   │   │                                  # Compartida por ambos DbContexts
│   │   │   ├── EntityDbContext.cs             # DbContext escritura (hereda EntityDBSets)
│   │   │   ├── EntityReadOnlyDbContext.cs     # DbContext solo lectura NoTracking
│   │   │   │                                  # (hereda EntityDBSets, IEntityReadOnlyDbContext)
│   │   │   └── EntityConfigurations/
│   │   │       └── PhysicalStructureConfig.cs
│   │   ├── Repository/
│   │   │   ├── Base/
│   │   │   │   ├── BaseRepositiry.cs          # Implementación genérica escritura
│   │   │   │   └── BaseReadOnlyRepository.cs  # Implementación genérica solo lectura
│   │   │   └── Properties/
│   │   │       ├── PhysicalStructureRepository.cs
│   │   │       └── PhysicalStructureReadOnlyRepository.cs
│   │   └── DependencyInjection.cs
│   └── Migrations/
│
└── Api/
    ├── Program.cs
    └── Controllers/
        ├── Base/
        │   ├── BaseController.cs
        │   └── ResponseApi.cs
        └── v1/
            └── PhysicalStructureController.cs
```



# Reglas de Diseño Táctico (Implementación línea a línea):

## Raíces de Agregado (Aggregate Roots) y Entidades:

Encapsulamiento estricto: Propiedades con get; private set; o init;. NUNCA expongas setters públicos.

Invariantes: Toda entidad debe nacer en un estado válido. Exige los datos necesarios en el constructor. Lanza excepciones de dominio (DomainException) si se violan reglas.

Constructores para ORMs: Siempre proporciona un constructor sin parámetros private o protected para que Entity Framework (u otros ORMs) puedan hidratar la entidad sin saltarse las reglas de negocio.

Mapa general del Sln teniendo en cuenta las reglas mencionadas.


## Ejemplo de implementación exigida:

### C# ejemplo aggregate root
```
    using Domain.DomainShared;
    namespace Domain.BoundedContext.Properties;

    public class PhysicalStructureAgg : AggregateRoot
    {
        public PhysicalStructureAgg()
        {
            
        }
        public PhysicalStructureAgg(string name,
                                    string nit,
                                    int unitCount,
                                    LocationValueObject location,
                                    List<CommonAreaValueObject> commonAreas
                                    ):base()
        {
            Name = name;
            Nit = nit;
            UnitCount = unitCount;
            CommonsAreas = commonAreas;
            Location = location;
            ExcecuteDomainInvariants();

        }
        public  string Name { get; private set; }
        public string Nit { get; set; }
        public int UnitCount { get; set; }
        public List<CommonAreaValueObject> CommonsAreas { get; private set; }
        public LocationValueObject Location { get; private set; }

        protected override void ExcecuteDomainInvariants()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new DomainException("La estructura física debe tener un nombre.");

            if (Name.Length > 150)
                throw new DomainException("El nombre no puede exceder los 150 caracteres.");

            if (Location == null)
                throw new DomainException("La ubicación es obligatoria.");
        }
    }
```

## Objetos de Valor (Value Objects):

Deben ser inmutables. Si su estado cambia, se crea uno nuevo.

En C#, impleméntalos preferiblemente como record para obtener igualdad estructural automática, o como clases que sobreescriban Equals y GetHashCode.

Ejemplo: public record Money(decimal Amount, string Currency);

### C# ejemplo value object
```
    using Domain.DomainShared;

    namespace Domain.BoundedContext.Properties;

    public record LocationValueObject : ValueObject
    {
        public LocationValueObject(string number,
                                string detail,
                                string country,
                                string city,
                                string neighborhood)
        {
            if(string.IsNullOrEmpty(number))
                throw new DomainException("Number cannot be null");
            
            if(string.IsNullOrEmpty(detail))
                throw new DomainException("Detail cannot be null");
            
            if(string.IsNullOrEmpty(country))
                throw new DomainException("Country cannot be null");
            
            if(string.IsNullOrEmpty(city))
                throw new DomainException("City cannot be null");
            
            if(string.IsNullOrEmpty(neighborhood))
                throw new DomainException("Neighborhood cannot be null");

            Number = number;
            Detail = detail;
            Country = country;
            City = city;
            Neighborhood = neighborhood;
        }
        
        public string Number { get; private set; }
        public string Detail { get; private set; }
        public string Country { get; private set; }
        public string City { get; private set; }
        public string Neighborhood { get; private set; }
    }
```

## Eventos de Dominio (Domain Events):

El agregado es el único responsable de registrar sus eventos (ej. AddDomainEvent()).

Los eventos deben representar algo que ya ocurrió en el pasado (ej. OrderShippedDomainEvent).

### C# ejemplo
```
    // ejemplo de eventos pendiente de implementar
```

## Repositorios de Escritura — CQRS (Command Side):

### Teoría
Solo debe haber repositorios para las **Raíces de Agregado** (Aggregate Roots). Nunca para entidades internas ni Value Objects.

Reglas clave:
- El contrato (`I{Nombre}Repository`) pertenece al **Dominio** (`Domain/Ports/Repository/`) — el dominio dicta el contrato.
- La implementación (`{Nombre}Repository`) pertenece a la **Infraestructura** y hereda de `BaseRepositiry<TAgg>`.
- Usa `IEntityDbContext` (con tracking) para operar en el Command side.
- Prohibido inyectar repositorios en Entidades o Value Objects. Solo los Application Services (o Command Handlers) los usan.

### Artefactos que genera este patrón por cada Agregado

| Capa | Archivo | Descripción |
|---|---|---|
| Domain/Ports | `I{Nombre}Repository.cs` | Puerto de escritura del dominio |
| Infraestructure | `{Nombre}Repository.cs` | Implementación: hereda `BaseRepositiry` |

### C# ejemplo — Interfaz de repositorio de escritura (Domain/Ports)
```csharp
using Domain.Ports.Repository.Base;
using Domain.BoundedContext.{BoundedContext};

namespace Domain.Ports;

public interface I{Nombre}Repository : IBaseRepository<{Nombre}Agg>
{
}
```

### C# ejemplo — Repositorio de escritura (Infraestructure)
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

### Registro en DI — Infrastructure (`DependencyInjection.cs`)
```csharp
services.AddScoped<I{Nombre}Repository, {Nombre}Repository>();
```

## Application Services — Command Side:

### Teoría
Los Application Services de escritura orquestan el flujo de un **comando** (Create, Update, Delete). Su responsabilidad exclusiva es:
1. Obtener el Agregado del repositorio.
2. Invocar el método de negocio en el Agregado.
3. Guardar el Agregado.

Nunca contienen lógica de negocio (condicionales `if` evaluando estado del dominio).

> Domain Service: Úsalo solo cuando una regla de negocio involucre múltiples Agregados y no pertenezca lógicamente a ninguno de ellos.

### Artefactos por Agregado

| Archivo | Descripción |
|---|---|
| `I{Nombre}Service.cs` | Contrato del servicio de escritura |
| `{Nombre}Service.cs` | Implementación: hereda `ApplicationService<TAgg, TDto>` |

### C# ejemplo — Interfaz del servicio de escritura
```csharp
using Application.Base;
using Application.Dto;
using Domain.BoundedContext.{BoundedContext};

namespace Application.Service;

public interface I{Nombre}Service : IApplicationService<{Nombre}Agg, {Nombre}Dto>
{
}
```

### C# ejemplo — Implementación del servicio de escritura
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

### Registro en DI — Application (`DependencyInyection.cs`)
```csharp
services.RegisterMediatrAbstractService<
    {Nombre}Service, {Nombre}Dto, {Nombre}Agg, I{Nombre}Service>();
```

---

## Application Services — Query Side (ReadOnly):

### Teoría
Los Application Services de solo lectura orquestan las **queries** (`GetByIdAsync`, `GetAllAsync`, `FindAsync`, `ExistsAsync`). Usan el `EntityReadOnlyDbContext` (NoTracking) y el mismo Mapper que el servicio de escritura.

### Artefactos por Agregado

| Archivo | Descripción |
|---|---|
| `I{Nombre}ReadOnlyService.cs` | Contrato del servicio de lectura |
| `{Nombre}ReadOnlyService.cs` | Implementación: hereda `ApplicationReadOnlyService<TAgg, TDto>` |

### C# ejemplo — Interfaz del servicio de lectura
```csharp
using Application.Base;
using Application.Dto;
using Domain.BoundedContext.{BoundedContext};

namespace Application.Service;

public interface I{Nombre}ReadOnlyService
    : IApplicationReadOnlyService<{Nombre}Agg, {Nombre}Dto>
{
}
```

### C# ejemplo — Implementación del servicio de lectura
```csharp
using Application.Base;
using Application.Dto;
using Domain.BoundedContext.{BoundedContext};
using Domain.Ports;

namespace Application.Service;

public class {Nombre}ReadOnlyService
    : ApplicationReadOnlyService<{Nombre}Agg, {Nombre}Dto>,
      I{Nombre}ReadOnlyService
{
    public {Nombre}ReadOnlyService(I{Nombre}ReadOnlyRepository repository)
        : base(repository)
    {
        // Reutiliza el mismo mapper del Service de escritura
        CreateMapperExpresion<{Nombre}Agg, {Nombre}Dto>(cnf =>
        {
            {Nombre}Mapper.Expresion(cnf);
        });
    }
}
```

### Registro en DI — Application (`DependencyInyection.cs`)
```csharp
services.RegisterMediatrAbstractReadOnlyService<
    {Nombre}ReadOnlyService, {Nombre}Dto, {Nombre}Agg, I{Nombre}ReadOnlyService>();
```

---

## Mappers (AutoMapper):

### Teoría
El Mapper es la capa de traducción entre el **Dominio** (Aggregate Roots, Value Objects) y la **Aplicación** (DTOs). Su responsabilidad es única: convertir datos en una dirección u otra sin contener lógica de negocio.

Reglas clave:
- El Mapper **nunca** instancia el Agregado directamente asignando propiedades. Siempre usa el **constructor de negocio** del Agregado (vía `ConstructUsing`) para respetar las invariantes.
- La dirección **DTO → Agregado** usa `ConstructUsing`, pasando los argumentos al constructor del Agregado e instanciando los Value Objects dentro del mapper.
- La dirección **Agregado → DTO** usa `ForMember` para proyectar cada campo, incluyendo los campos anidados de los Value Objects (`src.VO.Campo`).
- El Mapper es una clase `static` con un método `Expresion(IMapperConfigurationExpression cnf)` que se registra en el constructor del Service correspondiente.

### Ubicación en la arquitectura
```
Application/
└── {NombreAgregado}/
    └── Mapper/
        └── {NombreAgregado}Mapper.cs   ← clase static, método Expresion()
```
El Mapper se registra en el `{NombreAgregado}Service` mediante:
```csharp
CreateMapperExpresion<{Agg}, {Dto}>(cnf => {NombreAgregado}Mapper.Expresion(cnf));
```

### C# ejemplo mapper
```csharp
using Application.Dto;
using AutoMapper;
using Domain.BoundedContext.Properties;

namespace Application.Service;

public static class PhysicalStructureMapper
{
    public static void Expresion(IMapperConfigurationExpression cnf)
    {
        // DTO → Aggregate Root
        // Usa ConstructUsing para invocar el constructor de negocio del Agregado.
        // Los Value Objects se construyen aquí respetando sus Guard Clauses.
        cnf.CreateMap<PhysicalStructureDto, PhysicalStructureAgg>()
            .ConstructUsing(src => new PhysicalStructureAgg(
                src.Name,
                src.Nit,
                src.UnitCount,
                new LocationValueObject(         // OwnsOne: construido directamente
                    src.Number,
                    src.DetailLocation,
                    src.Country,
                    src.City,
                    src.Neighborhood
                ),
                src.CommonAreas               // OwnsMany: proyectado desde lista del DTO
                    .Select(ca => new CommonAreaValueObject(ca.Name, ca.Description))
                    .ToList()
            ));

        // Aggregate Root → DTO
        // Proyección plana: los campos de los Value Objects se mapean
        // accediendo a la propiedad del VO dentro del Agregado (src.VO.Campo).
        cnf.CreateMap<PhysicalStructureAgg, PhysicalStructureDto>()
            .ForMember(dest => dest.Name,           opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Nit,            opt => opt.MapFrom(src => src.Nit))
            .ForMember(dest => dest.UnitCount,      opt => opt.MapFrom(src => src.UnitCount))
            .ForMember(dest => dest.Number,         opt => opt.MapFrom(src => src.Location.Number))
            .ForMember(dest => dest.DetailLocation, opt => opt.MapFrom(src => src.Location.Detail))
            .ForMember(dest => dest.Country,        opt => opt.MapFrom(src => src.Location.Country))
            .ForMember(dest => dest.City,           opt => opt.MapFrom(src => src.Location.City))
            .ForMember(dest => dest.Neighborhood,   opt => opt.MapFrom(src => src.Location.Neighborhood));
    }
}
```

---

## Repositorios de Solo Lectura — CQRS (Query Side):

### Teoría
El proyecto implementa **CQRS con dos DbContexts separados**:
- **Command side (`EntityDbContext`)**: Con tracking, para operaciones de escritura (Create, Update, Delete).
- **Query side (`EntityReadOnlyDbContext`)**: Sin tracking (`NoTrackingWithIdentityResolution`), optimizado para consultas de lectura. Hereda de `EntityDBSets` (clase base abstracta que centraliza `DbSets` y `OnModelCreating`).

Reglas clave:
- Cada Aggregate Root tiene **dos repositorios**: `I{Nombre}Repository` (escritura) e `I{Nombre}ReadOnlyRepository` (lectura). Ambos contratos pertenecen al Dominio (Ports).
- `IEntityReadOnlyDbContext` **no hereda** de `IEntityDbContext` — garantiza que no se puede persistir ningún cambio a través del contrato de solo lectura.
- El `ReadOnlyService` **reutiliza el mismo Mapper** que el `Service` de escritura para mantener proyecciones consistentes.
- Los Queries genéricos (`GetByIdQuery`, `GetAllQuery`) son despachados por MediatR usando el `IApplicationReadOnlyService<ENT, DTO>`.

### Artefactos por capa (Dominio e Infraestructura)

| Capa | Archivo | Descripción |
|---|---|---|
| Domain/Ports | `I{Nombre}ReadOnlyRepository.cs` | Puerto de lectura del dominio |
| Infraestructure | `{Nombre}ReadOnlyRepository.cs` | Implementación: hereda `BaseReadOnlyRepository` |

> Los artefactos de Application (servicio e interfaz) están en la sección **Application Services — Query Side**.

### C# ejemplo — Interfaz de repositorio de solo lectura (Domain/Ports)
```csharp
using Domain.Ports.Repository.Base;
using Domain.BoundedContext.{BoundedContext};

namespace Domain.Ports;

public interface I{Nombre}ReadOnlyRepository : IBaseReadOnlyRepository<{Nombre}Agg>
{
}
```

### C# ejemplo — Repositorio de solo lectura (Infraestructure)
```csharp
using Domain.Ports;
using Domain.BoundedContext.{BoundedContext};
using Infraestructure.Repository.Shared;

namespace Infraestructure.Repository.{BoundedContext};

public class {Nombre}ReadOnlyRepository
    : BaseReadOnlyRepository<{Nombre}Agg>, I{Nombre}ReadOnlyRepository
{
    public {Nombre}ReadOnlyRepository(IEntityReadOnlyDbContext readOnlyContext)
        : base(readOnlyContext)
    {
    }
}
```

### Registro en DI — Infrastructure (`DependencyInjection.cs`)
```csharp
services.AddScoped<I{Nombre}ReadOnlyRepository, {Nombre}ReadOnlyRepository>();
```
Y el `EntityReadOnlyDbContext` ya está registrado una sola vez en la infraestructura — no se repite por agregado.

---

## Controllers (Capa API):

### Teoría
Los Controllers son la **puerta de entrada HTTP** de la aplicación. Su responsabilidad es mínima: recibir la solicitud, delegar la validación al Validator y delegar la ejecución al Mediator (CQRS). No contienen lógica de negocio ni de mapeo.

Reglas clave:
- Todo Controller hereda de `BaseController<ENT, DTO>`, que ya provee los endpoints `POST /create`, `PUT /update` y `DELETE /delete` con validación y respuesta estandarizada.
- El Controller concreto **solo necesita declarar su herencia** y recibir por inyección `IValidator<DTO>` e `IMediator`.
- El `BaseController` se encarga de: validar el DTO con FluentValidation → enviar el comando por MediatR → envolver la respuesta en `ResponseApi<T>`.
- Todos los endpoints concretos van en `Api/Controllers/v1/` para versionado explícito.
- Prohibido inyectar repositorios, Application Services ni DbContext directamente en un Controller.

### Flujo interno de BaseController
```
HTTP Request → Controller.Create(DTO)
    → IValidator<DTO>.ValidateAsync()    ← FluentValidation (Application/Validator)
    → IMediator.Send(CreateCommand<ENT,DTO>)  ← MediatR despacha al Handler
    → HandlerResponse(result)            ← envuelve en ResponseApi<T>
→ HTTP Response 200 OK
```

### Ubicación en la arquitectura
```
Api/
└── Controllers/
    ├── Base/
    │   ├── BaseController.cs    ← abstract, genérico: BaseController<ENT, DTO>
    │   │                          provee: POST /create, PUT /update, DELETE /delete
    │   └── ResponseApi.cs       ← wrapper de respuesta: { Data, Status, Message }
    └── v1/
        └── {Nombre}Controller.cs  ← hereda BaseController, sin lógica adicional
```

### C# ejemplo controller concreto
```csharp
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Dto;
using Domain.BoundedContext.Properties;
using FluentValidation;

namespace Api.Controllers;

// La ruta se infiere del nombre de la clase: api/PhysicalStructure
[Route("api/[controller]")]
public class PhysicalStructureController 
    : BaseController<PhysicalStructureAgg, PhysicalStructureDto>
{
    // Recibe IValidator y IMediator, los pasa al BaseController.
    // No necesita ninguna lógica adicional: el BaseController provee
    // los endpoints create, update y delete automáticamente.
    public PhysicalStructureController(
        IValidator<PhysicalStructureDto> validator,
        IMediator mediator)
        : base(validator, mediator)
    {
    }
}
```

### C# referencia BaseController (no modificar)
```csharp
// Api/Controllers/Base/BaseController.cs
// Clase base genérica que todos los controllers concretos heredan.
// Provee los 3 endpoints estándar con validación y respuesta unificada.

[AllowAnonymous]
[ApiController]
public abstract partial class BaseController<ENT, DTO> : ControllerBase
    where ENT : class, new()
    where DTO : class, new()
{
    // POST api/{Nombre}/create  → valida con FluentValidation → envía CreateCommand
    // PUT  api/{Nombre}/update  → valida con FluentValidation → envía UpdateCommand
    // DELETE api/{Nombre}/delete → envía DeleteCommand
    // Toda respuesta exitosa se envuelve en: { Data: T, Status: true, Message: "..." }
}
```

---

## 2. LO QUE NO DEBES HACER (Anti-patrones y Violaciones de Reglas)
### Prohibiciones Arquitectónicas:

#### NO crees Modelos de Dominio Anémicos:

Prohibido: Clases que solo son sacos de datos (Data Bags) con getters y setters públicos y sin métodos de comportamiento.

Prohibido: Validar el estado del Agregado en el Application Service. Ejemplo malo: if (order.Status == Draft) { order.Status = Shipped; }. Esto debe ser order.Ship(); dentro del agregado.

NO acoples el Dominio a la Infraestructura:

Prohibido: Añadir dependencias de Entity Framework Core, SQL Client, Dapper, ASP.NET Core, o cualquier framework técnico dentro de los proyectos de la capa de Dominio.

Prohibido: Usar atributos de Entity Framework (Data Annotations como [Table], [Key], [Required]) en las Entidades del dominio. Usa IEntityTypeConfiguration<T> en la capa de infraestructura (Fluent API).

NO inyectes Repositorios en Entidades:

Prohibido: Pasar un repositorio por el constructor de una Entidad. Las entidades no deben guardar cosas en la base de datos por sí mismas. Eso lo orquesta el Servicio de Aplicación (o Command Handler).

NO crees Repositorios Genéricos expuestos directamente:

Prohibido: Inyectar un IRepository<T> genérico en la capa de aplicación y hacer consultas LINQ complejas fuera del dominio. El repositorio debe expresar la intención del dominio (ej. IOrderRepository.GetPendingOrdersAsync()).

NO uses Entity Framework como Unit of Work directamente en la lógica de negocio:

Prohibido: Llamar a dbContext.SaveChanges() dentro de un Servicio de Dominio o una Entidad. El SaveChanges debe ser invocado por la capa de Aplicación (idealmente a través de una abstracción de UnitOfWork) o mediante un middleware/pipeline behavior.
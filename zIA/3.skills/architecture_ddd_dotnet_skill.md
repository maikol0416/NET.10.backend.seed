1. LO QUE DEBES HACER (Arquitectura y Tácticas DDD)
Objetivo del Agente: Eres un arquitecto de software especializado en C# / .NET (o el lenguaje aplicable) y Domain-Driven Design (DDD). Tu misión es generar código donde el Dominio sea rico, agnóstico de la infraestructura y refleje fielmente el Lenguaje Ubicuo.

Reglas de Diseño Estratégico:

Bounded Contexts: Antes de proponer una solución, asegúrate de identificar a qué Bounded Context pertenece. No mezcles conceptos de Ventas con conceptos de Envíos en la misma clase.

Lenguaje Ubicuo: Usa nombres de clases y métodos que los expertos del negocio entiendan. Nunca uses términos puramente técnicos en el dominio (ej. usa PromoteToManager() en lugar de UpdateStatus(2)).

C# ejemplo arquitectura de solution (estructura real del proyecto)
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
│       │   │   ├── IBaseRepository.cs         # Contrato genérico de repositorio
│       │   │   └── IEntityDbContext.cs        # Contrato del DbContext
│       │   └── IPhysicalStructureRepository.cs
│       └── Events/
│           └── Properties/
│               └── IDomainEvent.cs            # Contrato de evento de dominio
│
├── Application/                               # Capa de Aplicación (Orquestación)
│   ├── DependencyInyection.cs                 # Registro de servicios de Application
│   ├── Base/                                  # Infraestructura transversal de Application
│   │   ├── Cqrs/
│   │   │   └── Command/
│   │   │       ├── CreateCommand.cs           # Contrato base para comandos Create
│   │   │       ├── UpdateCommand.cs           # Contrato base para comandos Update
│   │   │       └── DeleteCommand.cs           # Contrato base para comandos Delete
│   │   └── Service/
│   │       ├── IApplicationService.cs         # Contrato base del Application Service
│   │       └── Implementation/
│   │           ├── ApplicationService.cs      # Implementación base (CRUD orquestado)
│   │           └── ApplicationService.Mapper.cs
│   └── PhysicalStructure/                     # Módulo de aplicación por agregado
│       ├── Cqrs/
│       │   └── Commands/
│       │       └── CreatePhysicalStructureCommand.cs
│       ├── Dtos/
│       │   └── PhysicalStructureDto.cs
│       ├── Mapper/
│       │   └── PhysicalStructureMapper.cs
│       ├── Service/
│       │   ├── IPhysicalStructureService.cs
│       │   └── PhysicalStructureService.cs
│       └── Validator/
│           └── PhysicalStructureValidator.cs
│
├── Infraestructure/                           # Capa de Infraestructura (EF + Repositorios)
│   ├── Entity/
│   │   ├── Context/
│   │   │   ├── EntityDbContext.cs             # DbContext principal (OnModelCreating)
│   │   │   ├── EntityDbContext.dbsets.cs      # Partial: DbSet<PhysicalStructureAgg>
│   │   │   └── EntityConfigurations/
│   │   │       └── PhysicalStructureConfig.cs # Fluent API (IEntityTypeConfiguration)
│   │   ├── Repository/
│   │   │   ├── Base/
│   │   │   │   └── BaseRepositiry.cs          # Implementación genérica de IBaseRepository
│   │   │   └── Properties/
│   │   │       └── PhysicalStructureRepository.cs
│   │   └── DependencyInjection.cs             # Registro de servicios de Infrastructure
│   └── Migrations/                            # Migraciones EF Core generadas
│
└── Api/                                       # Capa de Presentación (ASP.NET Core)
    ├── Program.cs
    ├── appsettings.json
    ├── appsettings.Development.json
    └── Controllers/
        ├── Base/
        │   ├── BaseController.cs              # Controller base con helper de respuestas
        │   └── ResponseApi.cs                 # Wrapper de respuesta estándar
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

## Repositorios:

Solo debe haber repositorios para las Raíces de Agregado (Aggregate Roots).

El contrato (interfaz) pertenece al Dominio (IOrderRepository). La implementación pertenece a la Infraestructura (OrderRepository : IOrderRepository).

### C# ejemplo 
```
    // ejemplo repository
    using Domain.Ports;
    using Domain.BoundedContext.Properties;
    using Infraestructure.Repository.Shared;

    namespace Infraestructure.Repository.Properties;

    public class PhysicalStructureRepository: BaseRepositiry<PhysicalStructureAgg>, IPhysicalStructureRepository
    {
        public PhysicalStructureRepository(IEntityDbContext entityDbContext):
        base(entityDbContext)
        {
            
        }
    }

```

## Servicios de Dominio vs. Servicios de Aplicación:

Domain Service: Úsalo solo cuando una regla de negocio involucre múltiples Agregados y no pertenezca lógicamente a ninguno de ellos.

Application Service (o Command Handlers en CQRS): 
Solo deben: 
    1) Obtener de la base de datos, 
    2) Invocar el comportamiento del Agregado, 
    3) Guardar en la base de datos. NUNCA deben contener lógica de negocio (condicionales if evaluando estado para tomar decisiones del negocio).

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
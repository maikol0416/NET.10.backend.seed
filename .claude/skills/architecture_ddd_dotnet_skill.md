# Skill: Arquitectura DDD .NET 10 — Referencia Completa

## Rol del Agente

Eres un arquitecto de software especializado en C# / .NET 10 y Domain-Driven Design (DDD). Tu misión es generar código donde el Dominio sea rico, agnóstico de la infraestructura y refleje fielmente el Lenguaje Ubicuo.

---

## Reglas de Diseño Estratégico

- **Bounded Contexts:** Antes de proponer una solución, asegúrate de identificar a qué Bounded Context pertenece. No mezcles conceptos de Ventas con conceptos de Envíos en la misma clase.
- **Lenguaje Ubicuo:** Usa nombres de clases y métodos que los expertos del negocio entiendan. Nunca uses términos puramente técnicos en el dominio (ej. usa `PromoteToManager()` en lugar de `UpdateStatus(2)`).

---

## Estructura del Solution

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
├── Api/
│   ├── Program.cs
│   └── Controllers/
│       ├── Base/
│       │   ├── BaseController.cs
│       │   └── ResponseApi.cs
│       └── v1/
│           └── PhysicalStructureController.cs
│
└── Test/                                       # Tests unitarios de Dominio (xUnit + FluentAssertions)
    └── Properties/                             # Un subdirectorio por Bounded Context (espejo de Domain/BoundedContext)
        ├── PhysicalStructureAggTests.cs
        ├── LocationValueObjectTests.cs
        └── ApartmentEntityTests.cs
```

---

## Convenciones de Nomenclatura

| Concepto | Convención | Ejemplo |
|---|---|---|
| Aggregate Root | `{Nombre}Agg` | `ResidentialUnitAgg` |
| Value Object | `{Nombre}ValueObject` | `LocationValueObject` |
| Namespace Domain BC | `Domain.BoundedContext.{BoundedContext}` | `Domain.BoundedContext.Properties` |
| Namespace Application | `Application.{Nombre}` | `Application.PhysicalStructure` |
| Namespace Infraestructure | `Infraestructure.Repository.{BoundedContext}` | `Infraestructure.Repository.Properties` |
| Herencia Aggregate | `: AggregateRoot` (nunca `Entity` directamente) | — |
| Herencia Value Object | `record : ValueObject` | — |
| Excepciones de dominio | `DomainException` | — |
| Invariantes | `ExcecuteDomainInvariants()` al final del constructor con parámetros | — |
| Propiedades de dominio | `get; private set;` (nunca setter público) | — |

---

## Reglas de Diseño Táctico

### Raíces de Agregado (Aggregate Roots) y Entidades

- **Encapsulamiento estricto:** Propiedades con `get; private set;` o `init;`. NUNCA expongas setters públicos.
- **Invariantes:** Toda entidad debe nacer en un estado válido. Exige los datos necesarios en el constructor. Lanza `DomainException` si se violan reglas.
- **Constructores para ORMs:** Siempre proporciona un constructor sin parámetros `public` o `protected` para que Entity Framework pueda hidratar la entidad sin saltarse las reglas de negocio.

### Objetos de Valor (Value Objects)

- Deben ser **inmutables**. Si su estado cambia, se crea uno nuevo.
- En C#, impleméntalos como `record` para obtener igualdad estructural automática.
- Heredan de `ValueObject` (clase base del proyecto).
- Incluyen Guard Clauses en el constructor para campos requeridos.

### Eventos de Dominio (Domain Events)

- El agregado es el único responsable de registrar sus eventos (`AddDomainEvent()`).
- Los eventos deben representar algo que **ya ocurrió** en el pasado (ej. `OrderShippedDomainEvent`).

---

## Aggregate Roots

### Teoría

El Aggregate Root es la raíz del agregado. Es la única entidad accesible desde fuera del agregado. Toda operación que modifique el estado del agregado debe pasar por el Aggregate Root.

### Artefactos

| Capa | Ruta | Descripción |
|---|---|---|
| Domain | `Domain/BoundedContext/{BC}/Aggregates/{Nombre}Agg.cs` | Aggregate Root |

### C# template genérico

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

### C# ejemplo concreto — PhysicalStructureAgg

```csharp
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

### Métodos de actualización de colecciones en el Agregado

Cuando un Aggregate Root tiene colecciones de Entidades hijas o Value Objects (`OwnsMany`), la responsabilidad de sincronizar esas colecciones debe residir en el **Agregado**, NO en el repositorio. El repositorio solo invoca los métodos del agregado.

Patrón:
- El agregado expone un método `Update{Colección}(IEnumerable<{Entity}> incoming)`.
- El método limpia la colección actual y recrea las entidades/VOs a partir de los datos entrantes.
- Se valida null para evitar excepciones.

### C# template — Método de actualización de colección

```csharp
public void Update{Colección}(IEnumerable<{Entity}> incoming{Colección})
{
    {Colección}.Clear();
    if (incoming{Colección} != null)
    {
        foreach (var item in incoming{Colección})
        {
            {Colección}.Add(new {Entity}(item.{Campo1}, item.{Campo2}));
        }
    }
}
```

### C# ejemplo concreto — UpdateTowers y UpdateCommonsAreas

```csharp
public void UpdateTowers(IEnumerable<TowerEntity> incomingTowers)
{
    Towers.Clear();
    if (incomingTowers != null)
    {
        foreach (var incomingTower in incomingTowers)
        {
            Towers.Add(new TowerEntity(incomingTower.Number, incomingTower.Floors));
        }
    }
}

public void UpdateCommonsAreas(IEnumerable<CommonAreaEntity> incomingCommonAreas)
{
    CommonsAreas.Clear();
    if (incomingCommonAreas != null)
    {
        foreach (var incomingCommonArea in incomingCommonAreas)
        {
            CommonsAreas.Add(new CommonAreaEntity(incomingCommonArea.Name, incomingCommonArea.Description));
        }
    }
}
```

---

## Value Objects

### Teoría

Los Value Objects representan conceptos del dominio que se definen por sus atributos (no por identidad). Son inmutables: si cambian, se crean nuevos.

### Artefactos

| Capa | Ruta | Descripción |
|---|---|---|
| Domain | `Domain/BoundedContext/{BC}/Aggregates/{VO}ValueObject.cs` | Un archivo por cada Value Object |

### C# template genérico

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

### C# ejemplo concreto — LocationValueObject

```csharp
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

---

## Entidades Hijas (Child Entities)

### Teoría

Las Entidades hijas representan conceptos del dominio que tienen **identidad propia** (`Id`, `Status`, `CreatedAt`, `UpdateAt`) pero que pertenecen a un Agregado y solo se acceden a través de la Raíz del Agregado. A diferencia de los Value Objects, las entidades tienen ciclo de vida propio y se identifican por su `Id`, no por sus atributos.

Reglas clave:
- Heredan de `Entity` (NO de `AggregateRoot`).
- Tienen `get; private set;` en todas sus propiedades.
- Incluyen un constructor vacío `public` para EF Core y un constructor de negocio con validaciones.
- Pueden tener un constructor de reconstrucción (con `Guid id`) para hidratar entidades existentes desde BD.
- Incluyen métodos de validación privados estáticos (`ValidateXxx`) y un método `Update(...)` para mutación controlada.
- Se nombran con el sufijo `Entity` (ej. `TowerEntity`, `CommonAreaEntity`).

### Artefactos

| Capa | Ruta | Descripción |
|---|---|---|
| Domain | `Domain/BoundedContext/{BC}/Aggregates/{Nombre}Entity.cs` | Entidad hija del agregado |

### C# template genérico

```csharp
using Domain.DomainShared;

namespace Domain.BoundedContext.{BoundedContext};

/// <summary>
/// Entidad {Nombre}Entity — representa {descripción} dentro de {Agregado}.
/// Pertenece al agregado {Agregado}Agg. Solo se accede a través del Aggregate Root.
/// </summary>
public class {Nombre}Entity : Entity
{
    /// <summary>Constructor para ORM (Entity Framework).</summary>
    public {Nombre}Entity() { }

    /// <summary>Constructor de negocio (nueva entidad).</summary>
    public {Nombre}Entity({parametros_campos}) : base()
    {
        Validate{Campo1}({campo1});
        // ... validar resto de campos
        {Campo1} = {campo1};
        // ... asignar resto de campos
    }

    /// <summary>Constructor para reconstrucción (entidad existente con Id conocido).</summary>
    public {Nombre}Entity(Guid id, {parametros_campos}) : base()
    {
        Validate{Campo1}({campo1});
        // ... validar resto de campos
        Id = id;
        {Campo1} = {campo1};
        // ... asignar resto de campos
    }

    private static void Validate{Campo1}({Tipo} {campo1})
    {
        if (string.IsNullOrWhiteSpace({campo1}))
            throw new DomainException("{Mensaje de error}");
    }

    // Campos propios (get; private set;)
    public {Tipo} {Campo1} { get; private set; }

    /// <summary>
    /// Actualiza la entidad con validación de negocio.
    /// </summary>
    public void Update({parametros_campos})
    {
        Validate{Campo1}({campo1});
        // ... validar resto de campos
        {Campo1} = {campo1};
        // ... asignar resto de campos
    }
}
```

### C# ejemplo concreto — TowerEntity

```csharp
using Domain.DomainShared;

namespace Domain.BoundedContext.Properties;

/// <summary>
/// Entidad TowerEntity — representa una torre dentro de una estructura física.
/// Pertenece al agregado PhysicalStructureAgg. Solo se accede a través del Aggregate Root.
/// </summary>
public class TowerEntity : Entity
{
    /// <summary>Constructor para ORM (Entity Framework).</summary>
    public TowerEntity() { }

    /// <summary>Constructor de negocio (nueva torre).</summary>
    public TowerEntity(string number, int floors) : base()
    {
        ValidateNumber(number);
        ValidateFloors(floors);
        Number = number;
        Floors = floors;
    }

    /// <summary>Constructor para reconstrucción (torre existente con Id conocido).</summary>
    public TowerEntity(Guid id, string number, int floors) : base()
    {
        ValidateNumber(number);
        ValidateFloors(floors);
        Id = id;
        Number = number;
        Floors = floors;
    }

    private static void ValidateNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new DomainException("El número de la torre es obligatorio.");

        if (number.Length > 20)
            throw new DomainException("El número de la torre no puede exceder los 20 caracteres.");
    }

    private static void ValidateFloors(int floors)
    {
        if (floors <= 0)
            throw new DomainException("El número de pisos debe ser mayor a 0.");
    }

    public string Number { get; private set; }
    public int Floors { get; private set; }

    /// <summary>
    /// Actualiza la torre con validación de negocio.
    /// </summary>
    public void Update(string number, int floors)
    {
        ValidateNumber(number);
        ValidateFloors(floors);

        Number = number;
        Floors = floors;
    }
}
```

---

## Entidades Anidadas (Entity dentro de otra Entity)

### Teoría

En agregados más grandes, una Entidad hija puede a su vez tener su propia colección de Entidades hijas (3 niveles: `Agg → Entity → Entity`). Ejemplo real del proyecto: `PhysicalStructureAgg → TowerEntity → ApartmentEntity` (un apartamento pertenece a una torre, una torre pertenece a la estructura física).

Reglas clave (extienden las de Entidades Hijas):
- La Entidad de nivel intermedio (`TowerEntity`) sigue las mismas reglas que cualquier Entidad hija, **más** un método `Update{ColecciónNieta}(IEnumerable<{Nieta}Entity>)` propio, análogo a `Update{Colección}` del Aggregate Root.
- El Aggregate Root **no** gestiona directamente la colección nieta. Su método `Update{Colección}` (ver sección Aggregate Roots) es responsable de invocar el `Update{ColecciónNieta}` de cada entidad intermedia recién creada.
- El repositorio de escritura debe usar `.Include().ThenInclude()` para cargar el grafo completo antes de delegar la actualización al agregado.
- El Fluent API anida un `OwnsMany` dentro de otro `OwnsMany` (ver Entity Configuration).
- El Mapper anida el `.Select()` de la colección nieta dentro del `.Select()` de la colección intermedia, tanto en dirección DTO → Agregado como Agregado → DTO.

### Artefactos

| Capa | Ruta | Descripción |
|---|---|---|
| Domain | `Domain/BoundedContext/{BC}/Aggregates/{Nieta}Entity.cs` | Entidad de nivel más profundo, sigue el template de Entidades Hijas |

### C# ejemplo concreto — TowerEntity con colección de ApartmentEntity

```csharp
public class TowerEntity : Entity
{
    public TowerEntity() { }

    public TowerEntity(string number, int floors, List<ApartmentEntity> apartments = null) : base()
    {
        ValidateNumber(number);
        ValidateFloors(floors);
        Number = number;
        Floors = floors;
        Apartments = apartments ?? new List<ApartmentEntity>();
    }

    public TowerEntity(Guid id, string number, int floors, List<ApartmentEntity> apartments = null) : base()
    {
        ValidateNumber(number);
        ValidateFloors(floors);
        Id = id;
        Number = number;
        Floors = floors;
        Apartments = apartments ?? new List<ApartmentEntity>();
    }

    public string Number { get; private set; }
    public int Floors { get; private set; }
    public List<ApartmentEntity> Apartments { get; private set; }

    public void Update(string number, int floors)
    {
        ValidateNumber(number);
        ValidateFloors(floors);
        Number = number;
        Floors = floors;
    }

    // Método propio de sincronización de la colección nieta (mismo patrón que Update{Colección} del Agg Root)
    public void UpdateApartments(IEnumerable<ApartmentEntity> incomingApartments)
    {
        Apartments.Clear();
        if (incomingApartments != null)
        {
            foreach (var inc in incomingApartments)
            {
                Apartments.Add(new ApartmentEntity(inc.Number, inc.Size, inc.IdOwner));
            }
        }
    }
}
```

### El Aggregate Root delega en la Entidad intermedia

```csharp
public void UpdateTowers(IEnumerable<TowerEntity> incomingTowers)
{
    Towers.Clear();
    if (incomingTowers != null)
    {
        foreach (var incomingTower in incomingTowers)
        {
            var newTower = new TowerEntity(incomingTower.Number, incomingTower.Floors);
            newTower.UpdateApartments(incomingTower.Apartments); // delega la colección nieta
            Towers.Add(newTower);
        }
    }
}
```

### Repositorio — `ThenInclude` para cargar el grafo completo

```csharp
public override async Task<PhysicalStructureAgg> UpdateAsync(PhysicalStructureAgg ent)
{
    var tracked = await entity
        .Include(p => p.CommonsAreas)
        .Include(p => p.Towers)
            .ThenInclude(t => t.Apartments)   // ⚠️ obligatorio para colecciones nietas
        .FirstOrDefaultAsync(p => p.Id == ent.Id)
        ?? throw new Exception($"No se encontró la estructura física con Id {ent.Id} para actualizar.");

    tracked.Update(ent.Name, ent.Nit, ent.UnitCount);
    tracked.UpdateTowers(ent.Towers);
    tracked.UpdateCommonsAreas(ent.CommonsAreas);

    await MainContext.SaveChangesAsync();
    return tracked;
}
```

### Fluent API — `OwnsMany` anidado dentro de `OwnsMany`

```csharp
builder.OwnsMany(p => p.Towers, towerBuilder =>
{
    towerBuilder.ToTable("Tower");
    towerBuilder.WithOwner().HasForeignKey("PhysicalStructureId");
    towerBuilder.Property(t => t.Id).ValueGeneratedNever();
    towerBuilder.HasKey(t => t.Id);

    // ... Status, CreatedAt, UpdateAt, Number, Floors ...

    // Colección nieta anidada dentro de la colección de Towers
    towerBuilder.OwnsMany(t => t.Apartments, apartmentBuilder =>
    {
        apartmentBuilder.ToTable("Apartment");
        apartmentBuilder.WithOwner().HasForeignKey("TowerId");
        apartmentBuilder.Property(a => a.Id).ValueGeneratedNever();
        apartmentBuilder.HasKey(a => a.Id);

        apartmentBuilder.Property(a => a.Status).IsRequired().HasMaxLength(10);
        apartmentBuilder.Property(a => a.CreatedAt).IsRequired();
        apartmentBuilder.Property(a => a.UpdateAt).IsRequired(false);

        apartmentBuilder.Property(a => a.Number).HasColumnName("Number").IsRequired().HasMaxLength(20);
        apartmentBuilder.Property(a => a.Size).HasColumnName("Size").IsRequired().HasMaxLength(50);
        apartmentBuilder.Property(a => a.IdOwner).HasColumnName("IdOwner").IsRequired();
    });
});
```

### DTO y Mapper anidados

El DTO intermedio (`TowerDto`) incluye la lista de DTOs nietos, y el Mapper anida el `.Select()`:

```csharp
public class TowerDto
{
    public Guid? Id { get; set; }
    public string Number { get; set; }
    public int Floors { get; set; }
    public List<ApartmentDto> Apartments { get; set; }
}
```

```csharp
// DTO → Aggregate: dentro del .Select() de Towers, otro .Select() para Apartments
src.Towers.Select(t => t.Id.HasValue && t.Id.Value != Guid.Empty
    ? new TowerEntity(t.Id.Value, t.Number, t.Floors,
        t.Apartments?.Select(a => a.Id.HasValue && a.Id.Value != Guid.Empty
            ? new ApartmentEntity(a.Id.Value, a.Number, a.Size, a.IdOwner)
            : new ApartmentEntity(a.Number, a.Size, a.IdOwner)).ToList() ?? new List<ApartmentEntity>())
    : new TowerEntity(t.Number, t.Floors, /* ... mismo patrón ... */))
    .ToList()

// Aggregate → DTO: mismo anidamiento en sentido inverso
.ForMember(dest => dest.Towers, opt => opt.MapFrom(src => src.Towers.Select(t => new TowerDto
{
    Id = t.Id, Number = t.Number, Floors = t.Floors,
    Apartments = t.Apartments != null
        ? t.Apartments.Select(a => new ApartmentDto { Id = a.Id, Number = a.Number, Size = a.Size, IdOwner = a.IdOwner }).ToList()
        : new List<ApartmentDto>()
}).ToList()));
```

---

## Domain Events

### Teoría

Los eventos de dominio representan algo que **ya ocurrió** en el pasado. El Aggregate Root es el único responsable de registrarlos mediante `AddDomainEvent()`.

### Artefactos

| Capa | Ruta | Descripción |
|---|---|---|
| Domain | `Domain/BoundedContext/{BC}/Events/DomainEvents.cs` | Eventos de dominio del Bounded Context |

### C# template genérico

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

## Repositorios de Escritura — CQRS (Command Side)

### Teoría

Solo debe haber repositorios para las **Raíces de Agregado** (Aggregate Roots). Nunca para entidades internas ni Value Objects.

Reglas clave:
- El contrato (`I{Nombre}Repository`) pertenece al **Dominio** (`Domain/Ports/Repository/`) — el dominio dicta el contrato.
- La implementación (`{Nombre}Repository`) pertenece a la **Infraestructura** y hereda de `BaseRepositiry<TAgg>`.
- Usa `IEntityDbContext` (con tracking) para operar en el Command side.
- Prohibido inyectar repositorios en Entidades o Value Objects. Solo los Application Services (o Command Handlers) los usan.

### Artefactos

| Capa | Ruta | Descripción |
|---|---|---|
| Domain/Ports | `Domain/Ports/Repository/I{Nombre}Repository.cs` | Puerto de escritura del dominio |
| Infraestructure | `Infraestructure/Entity/Repository/{BC}/{Nombre}Repository.cs` | Implementación: hereda `BaseRepositiry` |

### C# template — Interfaz de repositorio (Domain/Ports)

```csharp
using Domain.Ports.Repository.Base;
using Domain.BoundedContext.{BoundedContext};

namespace Domain.Ports;

public interface I{Nombre}Repository : IBaseRepository<{Nombre}Agg>
{
}
```

### C# template — Repositorio de escritura (Infraestructure)

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

### Registro DI — `Infraestructure/Entity/DependencyInjection.cs`

Agregar en el método `AddDependencyInjectionInfrastructureEf`:
```csharp
services.AddScoped<I{Nombre}Repository, {Nombre}Repository>();
```
Agregar el using:
```csharp
using Infraestructure.Repository.{BoundedContext};
```

### Override UpdateAsync (agregados con colecciones OwnsMany)

Cuando el agregado contiene colecciones de Entidades hijas o Value Objects (`OwnsMany`), el `UpdateAsync` base del repositorio genérico no gestiona correctamente la sincronización de las colecciones. Se debe sobrecargar para:
1. Cargar el agregado completo desde BD con `.Include()` de cada colección.
2. Invocar los métodos de actualización del agregado (`Update`, `Update{Colección}`).
3. Persistir los cambios.

### C# template — Override UpdateAsync

```csharp
public override async Task<{Nombre}Agg> UpdateAsync({Nombre}Agg ent)
{
    // Cargar el agregado completo con sus owned entities
    var tracked = await entity
        .Include(p => p.{Colección1})
        .Include(p => p.{Colección2})
        .FirstOrDefaultAsync(p => p.Id == ent.Id)
        ?? throw new Exception($"No se encontró con Id {ent.Id} para actualizar.");

    // Delegar la actualización al agregado (lógica de negocio en el dominio)
    tracked.Update(ent.{Campo1}, ent.{Campo2}, ent.{Campo3});
    tracked.Update{Colección1}(ent.{Colección1});
    tracked.Update{Colección2}(ent.{Colección2});

    await MainContext.SaveChangesAsync();
    return tracked;
}
```

### C# ejemplo concreto — PhysicalStructureRepository

```csharp
public override async Task<PhysicalStructureAgg> UpdateAsync(PhysicalStructureAgg ent)
{
    var tracked = await entity
        .Include(p => p.CommonsAreas)
        .Include(p => p.Towers)
            .ThenInclude(t => t.Apartments)   // colección nieta: ver sección "Entidades Anidadas"
        .FirstOrDefaultAsync(p => p.Id == ent.Id)
        ?? throw new Exception($"No se encontró la estructura física con Id {ent.Id} para actualizar.");

    tracked.Update(ent.Name, ent.Nit, ent.UnitCount);
    tracked.UpdateTowers(ent.Towers);
    tracked.UpdateCommonsAreas(ent.CommonsAreas);

    await MainContext.SaveChangesAsync();
    return tracked;
}
```

> Si una colección tiene a su vez colecciones propias (Entity dentro de Entity), agrega un `.ThenInclude()` por cada nivel — de lo contrario EF Core no cargará el grafo completo y `Update{ColecciónNieta}` operará sobre una colección vacía, borrando los datos existentes al guardar. Ver **Entidades Anidadas (Entity dentro de otra Entity)**.

---

## Repositorios de Solo Lectura — CQRS (Query Side)

### Teoría

El proyecto implementa **CQRS con dos DbContexts separados**:
- **Command side (`EntityDbContext`)**: Con tracking, para operaciones de escritura (Create, Update, Delete).
- **Query side (`EntityReadOnlyDbContext`)**: Sin tracking (`NoTrackingWithIdentityResolution`), optimizado para consultas de lectura. Hereda de `EntityDBSets` (clase base abstracta que centraliza `DbSets` y `OnModelCreating`).

Reglas clave:
- Cada Aggregate Root tiene **dos repositorios**: `I{Nombre}Repository` (escritura) e `I{Nombre}ReadOnlyRepository` (lectura). Ambos contratos pertenecen al Dominio (Ports).
- `IEntityReadOnlyDbContext` **no hereda** de `IEntityDbContext` — garantiza que no se puede persistir ningún cambio a través del contrato de solo lectura.
- Los Queries genéricos (`GetByIdQuery`, `GetAllQuery`) son despachados por MediatR usando el `IApplicationReadOnlyService<ENT, DTO>`.

### Artefactos

| Capa | Ruta | Descripción |
|---|---|---|
| Domain/Ports | `Domain/Ports/Repository/I{Nombre}ReadOnlyRepository.cs` | Puerto de lectura del dominio |
| Infraestructure | `Infraestructure/Entity/Repository/{BC}/{Nombre}ReadOnlyRepository.cs` | Implementación: hereda `BaseReadOnlyRepository` |

### C# template — Interfaz de repositorio solo lectura (Domain/Ports)

```csharp
using Domain.Ports.Repository.Base;
using Domain.BoundedContext.{BoundedContext};

namespace Domain.Ports;

public interface I{Nombre}ReadOnlyRepository : IBaseReadOnlyRepository<{Nombre}Agg>
{
}
```

### C# template — Repositorio solo lectura (Infraestructure)

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

### Registro DI — `Infraestructure/Entity/DependencyInjection.cs`

Agregar en el método `AddDependencyInjectionInfrastructureEf`:
```csharp
services.AddScoped<I{Nombre}ReadOnlyRepository, {Nombre}ReadOnlyRepository>();
```

> El `EntityReadOnlyDbContext` ya está registrado una sola vez en la infraestructura — no se repite por agregado.

---

## DTOs (Data Transfer Objects)

### Teoría

Los DTOs son objetos planos que representan la forma en que los datos se exponen a la capa de presentación (API). No contienen lógica de negocio.

Reglas clave:
- Los campos de Value Objects con relación **OwnsOne** se **aplanan** directamente en el DTO del agregado (sin anidar).
- Los Value Objects con relación **OwnsMany** tienen un **DTO separado** en su propio archivo (`{VO}Dto.cs`), y se referencian como `List<{VO}Dto>` en el DTO del agregado.
- Los DTOs usan `{ get; set; }` (setters públicos) — son objetos de transporte, no de dominio.

### Artefactos

| Capa | Ruta | Descripción |
|---|---|---|
| Application | `Application/{Nombre}/Dtos/{Nombre}Dto.cs` | DTO del agregado |
| Application | `Application/{Nombre}/Dtos/{VO}Dto.cs` | DTO por cada VO con OwnsMany (archivo separado) |

### C# template — DTO del Agregado

```csharp
namespace Application.Dto;

public class {Nombre}Dto
{
    // Campos planos del agregado
    public {Tipo} {Campo} { get; set; }

    // Campos aplanados de los Value Objects OwnsOne (sin anidar)
    public {Tipo} {VoCampo} { get; set; }

    // OwnsMany: lista de DTOs separados
    public List<{VO}Dto> {VOs} { get; set; }
}
```

### C# template — DTO de Value Object OwnsMany (archivo separado)

```csharp
namespace Application.Dto;

public class {VO}Dto
{
    public {Tipo} {Campo} { get; set; }
    // ... resto de campos del VO
}
```

---

## Mapper (AutoMapper)

### Teoría

El Mapper es la capa de traducción entre el **Dominio** (Aggregate Roots, Value Objects) y la **Aplicación** (DTOs). Su responsabilidad es única: convertir datos en una dirección u otra sin contener lógica de negocio.

Reglas clave:
- El Mapper **nunca** instancia el Agregado directamente asignando propiedades. Siempre usa el **constructor de negocio** del Agregado (vía `ConstructUsing`) para respetar las invariantes.
- La dirección **DTO → Agregado** usa `ConstructUsing`, pasando los argumentos al constructor del Agregado e instanciando los Value Objects dentro del mapper.
- La dirección **Agregado → DTO** usa `ForMember` para proyectar cada campo, incluyendo los campos anidados de los Value Objects (`src.VO.Campo`).
- Para **OwnsMany**, la dirección Agregado → DTO usa `ForMember` + `Select()` inline para proyectar la colección.
- El Mapper es una clase `static` con un método `Expresion(IMapperConfigurationExpression cnf)` que se registra en el constructor del Service correspondiente.

### Artefactos

| Capa | Ruta | Descripción |
|---|---|---|
| Application | `Application/{Nombre}/Mapper/{Nombre}Mapper.cs` | Clase static, método `Expresion()` |

### Ubicación y registro

El Mapper se registra en el constructor del `{Nombre}Service` y del `{Nombre}ReadOnlyService` mediante:
```csharp
CreateMapperExpresion<{Agg}, {Dto}>(cnf => {NombreAgregado}Mapper.Expresion(cnf));
```

### C# template genérico

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
            // Value Object OwnsOne:
            .ForMember(dest => dest.{VoCampo}, opt => opt.MapFrom(src => src.{VO}.{VoCampo}))
            // Value Object OwnsMany:
            .ForMember(dest => dest.{VOs}, opt => opt.MapFrom(src => src.{VOs}
                .Select(vo => new {VO}Dto { {Campo} = vo.{Campo} }).ToList()));
    }
}
```

### C# ejemplo concreto — PhysicalStructureMapper

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

## Application Services — Command Side

### Teoría

Los Application Services de escritura orquestan el flujo de un **comando** (Create, Update, Delete). Su responsabilidad exclusiva es:
1. Obtener el Agregado del repositorio.
2. Invocar el método de negocio en el Agregado.
3. Guardar el Agregado.

Nunca contienen lógica de negocio (condicionales `if` evaluando estado del dominio).

> Domain Service: Úsalo solo cuando una regla de negocio involucre múltiples Agregados y no pertenezca lógicamente a ninguno de ellos.

### Artefactos

| Capa | Ruta | Descripción |
|---|---|---|
| Application | `Application/{Nombre}/Service/I{Nombre}Service.cs` | Contrato del servicio de escritura |
| Application | `Application/{Nombre}/Service/{Nombre}Service.cs` | Implementación: hereda `ApplicationService<TAgg, TDto>` |

### C# template — Interfaz del servicio de escritura

```csharp
using Application.Base;
using Application.Dto;
using Domain.BoundedContext.{BoundedContext};

namespace Application.Service;

public interface I{Nombre}Service : IApplicationService<{Nombre}Agg, {Nombre}Dto>
{
}
```

### C# template — Implementación del servicio de escritura

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

### Registro DI — `Application/DependencyInyection.cs`

Agregar en `AddDependencyInjectionApplication`:
```csharp
services.RegisterMediatrAbstractService<{Nombre}Service, {Nombre}Dto, {Nombre}Agg, I{Nombre}Service>();
```

---

## Application Services — Query Side (ReadOnly)

### Teoría

Los Application Services de solo lectura orquestan las **queries** (`GetByIdAsync`, `GetAllAsync`, `FindAsync`, `ExistsAsync`). Usan el `EntityReadOnlyDbContext` (NoTracking) y el **mismo Mapper** que el servicio de escritura.

### Artefactos

| Capa | Ruta | Descripción |
|---|---|---|
| Application | `Application/{Nombre}/Service/I{Nombre}ReadOnlyService.cs` | Contrato del servicio de lectura |
| Application | `Application/{Nombre}/Service/{Nombre}ReadOnlyService.cs` | Implementación: hereda `ApplicationReadOnlyService<TAgg, TDto>` |

### C# template — Interfaz del servicio de lectura

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

### C# template — Implementación del servicio de lectura

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

### Registro DI — `Application/DependencyInyection.cs`

Agregar en `AddDependencyInjectionApplication`:
```csharp
services.RegisterMediatrAbstractReadOnlyService<{Nombre}ReadOnlyService, {Nombre}Dto, {Nombre}Agg, I{Nombre}ReadOnlyService>();
```

---

## Validator (FluentValidation)

### Teoría

Los Validators validan los DTOs **antes** de que lleguen al dominio. Usan FluentValidation y se ejecutan automáticamente desde el `BaseController` antes de enviar el comando por MediatR.

Reglas clave:
- El Validator opera sobre el **DTO**, no sobre el Agregado. Las invariantes del Agregado están en `ExcecuteDomainInvariants()`.
- Puede inyectar repositorios para validaciones de unicidad u otras consultas.
- Cada regla debe tener `WithErrorCode`, `WithMessage` y `WithName`.

### Artefactos

| Capa | Ruta | Descripción |
|---|---|---|
| Application | `Application/{Nombre}/Validator/{Nombre}Validator.cs` | Validator del DTO del agregado |

### C# template

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

### Reglas condicionales — Create vs. Update

El mismo `{Nombre}Validator` se ejecuta tanto en `POST create` como en `PUT update` (el `BaseController` reutiliza `IValidator<DTO>` para ambos). Si una regla solo aplica a uno de los dos casos (ej. `Id` es obligatorio en Update pero no debe enviarse en Create), usa `When`/`Unless` en vez de crear un Validator separado:

```csharp
// El Id solo es obligatorio cuando el DTO ya trae un valor distinto de default
// (heurística simple: si el caller no puede distinguir Create de Update por otro medio,
// evalúa exponer el contexto explícitamente en el DTO o usar RuleSet).
RuleFor(x => x.Id)
    .NotEmpty()
    .WithErrorCode("IdRequired")
    .WithMessage("El Id es obligatorio para actualizar.")
    .When(x => x.Id.HasValue && x.Id != Guid.Empty);
```

### Registro DI — `Application/DependencyInyection.cs`

Agregar en `RegisterValidators`:
```csharp
services.AddScoped<IValidator<{Nombre}Dto>, {Nombre}Validator>();
```

---

## CQRS Commands (MediatR)

### Teoría

Los Commands encapsulan la intención de un caso de uso de escritura. Son despachados por MediatR hacia un Handler que orquesta la operación.

Reglas clave:
- El Command es un `record` inmutable que implementa `IRequest<T>`.
- El Handler recibe el Command, construye el Agregado usando su **constructor de negocio** (no setters), y persiste a través del repositorio.
- Los Commands genéricos base (`CreateCommand`, `UpdateCommand`, `DeleteCommand`) ya existen en `Application/Base/Cqrs/Command/`. Solo se crean Commands específicos cuando se necesita lógica particular.

### Artefactos

| Capa | Ruta | Descripción |
|---|---|---|
| Application | `Application/{Nombre}/Cqrs/Commands/Create{Nombre}Command.cs` | Command + Handler para crear el agregado |

### C# template — CreateCommand

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

## Entity Configuration (Fluent API)

### Teoría

Las configuraciones de entidad definen el mapeo entre el modelo de dominio y la base de datos usando Fluent API. Prohibido usar Data Annotations (`[Table]`, `[Key]`, `[Required]`) en las entidades del dominio.

Reglas clave:
- Cada Aggregate Root tiene su propio archivo de configuración `{Nombre}Config.cs`.
- Siempre mapear los campos heredados de `Entity`: `Status`, `CreatedAt`, `UpdateAt`.
- Value Objects con **OwnsOne**: tabla separada con `ToTable`, `WithOwner().HasForeignKey(...)`, `HasKey("Id")`.
- Value Objects con **OwnsMany**: misma estructura que OwnsOne pero con `builder.OwnsMany(...)`.

### Artefactos

| Capa | Ruta | Descripción |
|---|---|---|
| Infraestructure | `Infraestructure/Entity/Context/EntityConfigurations/{Nombre}Config.cs` | Configuración Fluent API del agregado |

### C# template

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

        // OwnsMany: Entidad hija (hereda de Entity — tiene Id, Status, CreatedAt, UpdateAt)
        builder.OwnsMany(p => p.{Entidades}, entityBuilder =>
        {
            entityBuilder.ToTable("{NombreTablaEntity}");
            entityBuilder.WithOwner().HasForeignKey("{Nombre}Id");
            entityBuilder.Property(t => t.Id).ValueGeneratedNever();
            entityBuilder.HasKey(t => t.Id);

            // ✅ Campos heredados de Entity (SIEMPRE incluir para entidades hijas)
            entityBuilder.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(10);

            entityBuilder.Property(t => t.CreatedAt)
                .IsRequired();

            entityBuilder.Property(t => t.UpdateAt)
                .IsRequired(false);

            // Campos propios de la entidad
            entityBuilder.Property(t => t.{Campo})
                .HasColumnName("{Campo}")
                .IsRequired()
                .HasMaxLength({max});
        });
    }
}
```

> **Diferencia clave Entity vs Value Object en OwnsMany:**
> - **Entidad hija**: usa `t => t.Id` (propiedad existente heredada de `Entity`) con `ValueGeneratedNever()`.
> - **Value Object**: usa `Property<int>("Id")` (shadow property creada por EF Core) con `HasKey("Id")`.

### Registro en `Infraestructure/Entity/Context/EntityDBSets.cs`

⚠️ Este archivo centraliza **DbSets y OnModelCreating** — NO modificar `EntityDbContext.cs` ni `EntityReadOnlyDbContext.cs` directamente.

Agregar el **using** en la cabecera:
```csharp
using Domain.BoundedContext.{BoundedContext};
```

Agregar el **DbSet**:
```csharp
public DbSet<{Nombre}Agg> {Nombre} { get; set; }
```

Agregar en **OnModelCreating**:
```csharp
modelBuilder.ApplyConfiguration(new {Nombre}Config());
```

---

## Controllers (Capa API)

### Teoría

Los Controllers son la **puerta de entrada HTTP** de la aplicación. Su responsabilidad es mínima: recibir la solicitud, delegar la validación al Validator y delegar la ejecución al Mediator (CQRS). No contienen lógica de negocio ni de mapeo.

Reglas clave:
- Todo Controller hereda de `BaseController<ENT, DTO>`, que ya provee los endpoints `POST /create`, `PUT /update` y `DELETE /delete` con validación y respuesta estandarizada.
- El Controller concreto **solo necesita declarar su herencia** y recibir por inyección `IValidator<DTO>` e `IMediator`.
- El `BaseController` se encarga de: validar el DTO con FluentValidation → enviar el comando por MediatR → envolver la respuesta en `ResponseApi<T>`.
- Todos los endpoints concretos van en `Api/Controllers/v1/` para versionado explícito.
- Prohibido inyectar repositorios, Application Services ni DbContext directamente en un Controller.

### Artefactos

| Capa | Ruta | Descripción |
|---|---|---|
| Api | `Api/Controllers/v1/{Nombre}Controller.cs` | Controller concreto |

### Flujo interno de BaseController

```
HTTP Request → Controller.Create(DTO)
    → IValidator<DTO>.ValidateAsync()    ← FluentValidation (Application/Validator)
    → IMediator.Send(CreateCommand<ENT,DTO>)  ← MediatR despacha al Handler
    → HandlerResponse(result)            ← envuelve en ResponseApi<T>
→ HTTP Response 200 OK
```

### C# template — Controller concreto

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

### C# ejemplo concreto — PhysicalStructureController

```csharp
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Dto;
using Domain.BoundedContext.Properties;
using FluentValidation;

namespace Api.Controllers;

[Route("api/[controller]")]
public class PhysicalStructureController 
    : BaseController<PhysicalStructureAgg, PhysicalStructureDto>
{
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
// Provee 6 endpoints estándar con validación y respuesta unificada.
// Requiere autenticación (no es anónimo).

[Authorize]
[ApiController]
public abstract partial class BaseController<ENT, DTO> : ControllerBase
    where ENT : class, new()
    where DTO : class, new()
{
    // POST   api/{Nombre}/create        → valida con FluentValidation → envía CreateCommand
    // PUT    api/{Nombre}/update        → valida con FluentValidation → envía UpdateCommand
    // DELETE api/{Nombre}/delete        → envía DeleteCommand
    // GET    api/{Nombre}/getAll        → envía GetAllQuery
    // GET    api/{Nombre}/getById       → envía GetByIdQuery
    // GET    api/{Nombre}/getPaginated  → envía GetPaginatedQuery (pageNumber, pageSize)
    // Toda respuesta exitosa se envuelve en: { Data: T, Status: true, Message: "..." }
}
```

> ⚠️ El Controller concreto **nunca** necesita declarar estos 6 endpoints — vienen heredados. Solo se agregan endpoints propios cuando el caso de uso no encaja en Create/Update/Delete/GetAll/GetById/GetPaginated genéricos (poco común; si ocurre, evalúa primero si es un Command/Query específico antes de romper el patrón del Controller).

---

## Tests Unitarios de Dominio

### Teoría

Cada artefacto de Dominio nuevo (Aggregate Root, Value Object, Entidad hija) debe tener su clase de test correspondiente en el proyecto `Test/`, verificando invariantes/guard clauses y métodos de negocio — sin mocks, sin infraestructura, solo construyendo el objeto directamente y aseverando su estado.

Reglas clave:
- Un archivo de test por artefacto de dominio, ubicado en `Test/{BoundedContext}/{Nombre}Tests.cs`, namespace `Test.{BoundedContext}`.
- Cubre como mínimo: construcción válida (happy path), cada Guard Clause / invariante con `[Theory]`/`[InlineData]` cuando aplique, y el método `Update(...)` si existe.
- Para Value Objects, cubre también la igualdad estructural (dos instancias con los mismos valores son iguales, gracias a `record`).
- Usa `FluentAssertions` (`.Should()...`) y `Xunit` (`[Fact]`, `[Theory]`).
- No se testea el Mapper, el Repositorio ni el Controller a este nivel — esos se cubren con tests de integración si el proyecto los requiere.

### Artefactos

| Capa | Ruta | Descripción |
|---|---|---|
| Test | `Test/{BC}/{Nombre}AggTests.cs` | Tests del Aggregate Root |
| Test | `Test/{BC}/{VO}ValueObjectTests.cs` | Tests de cada Value Object |
| Test | `Test/{BC}/{Entity}EntityTests.cs` | Tests de cada Entidad hija (incluye Entidades anidadas) |

### C# ejemplo concreto — ApartmentEntityTests

```csharp
using Domain.BoundedContext.Properties;
using Domain.DomainShared;
using FluentAssertions;
using Xunit;

namespace Test.Properties;

public class ApartmentEntityTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateSuccessfully()
    {
        var idOwner = Guid.NewGuid();
        var apartment = new ApartmentEntity("101", "50m2", idOwner);

        apartment.Number.Should().Be("101");
        apartment.Size.Should().Be("50m2");
        apartment.IdOwner.Should().Be(idOwner);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidNumber_ShouldThrowDomainException(string invalidNumber)
    {
        var idOwner = Guid.NewGuid();
        var action = () => new ApartmentEntity(invalidNumber, "50m2", idOwner);
        action.Should().ThrowExactly<DomainException>().WithMessage("*número*");
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateFieldsSuccessfully()
    {
        var idOwner1 = Guid.NewGuid();
        var idOwner2 = Guid.NewGuid();
        var apartment = new ApartmentEntity("101", "50m2", idOwner1);

        apartment.Update("102", "60m2", idOwner2);

        apartment.Number.Should().Be("102");
        apartment.Size.Should().Be("60m2");
        apartment.IdOwner.Should().Be(idOwner2);
    }
}
```

---

## Anti-patrones — LO QUE NO DEBES HACER

### Prohibiciones Arquitectónicas

#### NO crees Modelos de Dominio Anémicos

- **Prohibido:** Clases que solo son sacos de datos (Data Bags) con getters y setters públicos y sin métodos de comportamiento.
- **Prohibido:** Validar el estado del Agregado en el Application Service. Ejemplo malo: `if (order.Status == Draft) { order.Status = Shipped; }`. Esto debe ser `order.Ship();` dentro del agregado.

#### NO acoples el Dominio a la Infraestructura

- **Prohibido:** Añadir dependencias de Entity Framework Core, SQL Client, Dapper, ASP.NET Core, o cualquier framework técnico dentro de los proyectos de la capa de Dominio.
- **Prohibido:** Usar atributos de Entity Framework (Data Annotations como `[Table]`, `[Key]`, `[Required]`) en las Entidades del dominio. Usa `IEntityTypeConfiguration<T>` en la capa de infraestructura (Fluent API).

#### NO inyectes Repositorios en Entidades

- **Prohibido:** Pasar un repositorio por el constructor de una Entidad. Las entidades no deben guardar cosas en la base de datos por sí mismas. Eso lo orquesta el Servicio de Aplicación (o Command Handler).

#### NO crees Repositorios Genéricos expuestos directamente

- **Prohibido:** Inyectar un `IRepository<T>` genérico en la capa de aplicación y hacer consultas LINQ complejas fuera del dominio. El repositorio debe expresar la intención del dominio (ej. `IOrderRepository.GetPendingOrdersAsync()`).

#### NO uses Entity Framework como Unit of Work directamente en la lógica de negocio

- **Prohibido:** Llamar a `dbContext.SaveChanges()` dentro de un Servicio de Dominio o una Entidad. El `SaveChanges` debe ser invocado por la capa de Aplicación (idealmente a través de una abstracción de UnitOfWork) o mediante un middleware/pipeline behavior.
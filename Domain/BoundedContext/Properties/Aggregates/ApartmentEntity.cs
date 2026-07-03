using Domain.DomainShared;

namespace Domain.BoundedContext.Properties;

/// <summary>
/// Entidad ApartmentEntity — representa un apartamento dentro de una torre.
/// Pertenece al agregado PhysicalStructureAgg a través de TowerEntity.
/// </summary>
public class ApartmentEntity : Entity
{
    /// <summary>Constructor para ORM (Entity Framework).</summary>
    public ApartmentEntity() { }

    /// <summary>
    /// Constructor de negocio (nuevo apartamento).
    /// Size e IdOwner son opcionales: un apartamento puede nacer sin tamaño ni propietario
    /// asignado (ej. generación masiva por piso) y completarse después mediante Update().
    /// </summary>
    public ApartmentEntity(string number, string? size = null, Guid? idOwner = null) : base()
    {
        ValidateNumber(number);
        ValidateSize(size);
        ValidateIdOwner(idOwner);

        Number = number;
        Size = size;
        IdOwner = idOwner;
    }

    /// <summary>Constructor para reconstrucción (apartamento existente con Id conocido).</summary>
    public ApartmentEntity(Guid id, string number, string? size = null, Guid? idOwner = null) : base()
    {
        ValidateNumber(number);
        ValidateSize(size);
        ValidateIdOwner(idOwner);

        Id = id;
        Number = number;
        Size = size;
        IdOwner = idOwner;
    }

    private static void ValidateNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new DomainException("El número del apartamento no puede ser vacío o nulo.");
    }

    private static void ValidateSize(string? size)
    {
        if (size != null && string.IsNullOrWhiteSpace(size))
            throw new DomainException("El tamaño del apartamento no puede ser vacío.");
    }

    private static void ValidateIdOwner(Guid? idOwner)
    {
        if (idOwner.HasValue && idOwner.Value == Guid.Empty)
            throw new DomainException("El propietario del apartamento no puede ser un identificador vacío.");
    }

    public string Number { get; private set; }
    public string? Size { get; private set; }
    public Guid? IdOwner { get; private set; }

    /// <summary>
    /// Actualiza el apartamento con validación de negocio.
    /// </summary>
    public void Update(string number, string? size, Guid? idOwner)
    {
        ValidateNumber(number);
        ValidateSize(size);
        ValidateIdOwner(idOwner);

        Number = number;
        Size = size;
        IdOwner = idOwner;
    }
}

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
    public TowerEntity(string number) : base()
    {
        ValidateNumber(number);
        Number = number;
    }

    /// <summary>Constructor para reconstrucción (torre existente con Id conocido).</summary>
    public TowerEntity(Guid id, string number) : base()
    {
        ValidateNumber(number);
        Id = id;
        Number = number;
    }

    private static void ValidateNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new DomainException("El número de la torre es obligatorio.");

        if (number.Length > 20)
            throw new DomainException("El número de la torre no puede exceder los 20 caracteres.");
    }

    public string Number { get; private set; }

    /// <summary>
    /// Actualiza el número de la torre con validación de negocio.
    /// </summary>
    public void UpdateNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new DomainException("El número de la torre es obligatorio.");

        if (number.Length > 20)
            throw new DomainException("El número de la torre no puede exceder los 20 caracteres.");

        Number = number;
    }
}

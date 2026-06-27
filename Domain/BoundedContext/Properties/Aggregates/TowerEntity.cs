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

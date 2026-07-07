using Domain.DomainShared;

namespace Domain.BoundedContext.People;

/// <summary>
/// Entidad GuestPermissionEntity — representa un permiso de acceso otorgado a un huésped.
/// Pertenece al agregado GuestAgg. Solo se accede a través del Aggregate Root.
/// </summary>
public class GuestPermissionEntity : Entity
{
    /// <summary>Constructor para ORM (Entity Framework).</summary>
    public GuestPermissionEntity() { }

    /// <summary>Constructor de negocio (nuevo permiso).</summary>
    public GuestPermissionEntity(DateTime startDate, DateTime endDate) : base()
    {
        ValidateDateRange(startDate, endDate);
        StartDate = startDate;
        EndDate = endDate;
    }

    /// <summary>Constructor para reconstrucción (permiso existente con Id conocido).</summary>
    public GuestPermissionEntity(Guid id, DateTime startDate, DateTime endDate) : base()
    {
        ValidateDateRange(startDate, endDate);
        Id = id;
        StartDate = startDate;
        EndDate = endDate;
    }

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate >= endDate)
            throw new DomainException("La fecha de inicio debe ser menor a la fecha de fin.");
    }

    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    /// <summary>
    /// Actualiza el permiso con validación de negocio.
    /// </summary>
    public void Update(DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);
        StartDate = startDate;
        EndDate = endDate;
    }
}

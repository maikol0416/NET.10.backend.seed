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

    /// <summary>
    /// Constructor de negocio (nuevo permiso).
    /// PhysicalStructureId es obligatorio: el permiso siempre aplica a una propiedad horizontal
    /// concreta. ApartmentId es opcional: permite acotar el permiso a un apartamento específico
    /// dentro de esa propiedad (si es null, el permiso aplica a la propiedad en general).
    /// </summary>
    public GuestPermissionEntity(DateTime startDate, DateTime endDate, Guid physicalStructureId, Guid? apartmentId = null) : base()
    {
        ValidateDateRange(startDate, endDate);
        ValidatePhysicalStructureId(physicalStructureId);
        ValidateApartmentId(apartmentId);
        StartDate = startDate;
        EndDate = endDate;
        PhysicalStructureId = physicalStructureId;
        ApartmentId = apartmentId;
    }

    /// <summary>Constructor para reconstrucción (permiso existente con Id conocido).</summary>
    public GuestPermissionEntity(Guid id, DateTime startDate, DateTime endDate, Guid physicalStructureId, Guid? apartmentId = null) : base()
    {
        ValidateDateRange(startDate, endDate);
        ValidatePhysicalStructureId(physicalStructureId);
        ValidateApartmentId(apartmentId);
        Id = id;
        StartDate = startDate;
        EndDate = endDate;
        PhysicalStructureId = physicalStructureId;
        ApartmentId = apartmentId;
    }

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate >= endDate)
            throw new DomainException("La fecha de inicio debe ser menor a la fecha de fin.");
    }

    private static void ValidatePhysicalStructureId(Guid physicalStructureId)
    {
        if (physicalStructureId == Guid.Empty)
            throw new DomainException("La propiedad horizontal del permiso no puede ser un identificador vacío.");
    }

    private static void ValidateApartmentId(Guid? apartmentId)
    {
        if (apartmentId.HasValue && apartmentId.Value == Guid.Empty)
            throw new DomainException("El apartamento del permiso no puede ser un identificador vacío.");
    }

    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public Guid PhysicalStructureId { get; private set; }
    public Guid? ApartmentId { get; private set; }

    /// <summary>
    /// Actualiza el permiso con validación de negocio.
    /// </summary>
    public void Update(DateTime startDate, DateTime endDate, Guid physicalStructureId, Guid? apartmentId)
    {
        ValidateDateRange(startDate, endDate);
        ValidatePhysicalStructureId(physicalStructureId);
        ValidateApartmentId(apartmentId);
        StartDate = startDate;
        EndDate = endDate;
        PhysicalStructureId = physicalStructureId;
        ApartmentId = apartmentId;
    }
}

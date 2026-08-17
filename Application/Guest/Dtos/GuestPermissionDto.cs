namespace Application.Dto;

public class GuestPermissionDto
{
    public Guid? Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid PhysicalStructureId { get; set; }
    public Guid? ApartmentId { get; set; }

    /// <summary>
    /// Nombre de la estructura física a la que pertenece el permiso. Solo lectura: se
    /// resuelve en la consulta a partir de PhysicalStructureId — enviarlo en create/update
    /// no tiene efecto.
    /// </summary>
    public string? PhysicalStructureName { get; set; }

    /// <summary>
    /// Número del apartamento al que pertenece el permiso (si aplica). Solo lectura: se
    /// resuelve en la consulta a partir de ApartmentId — enviarlo en create/update no
    /// tiene efecto.
    /// </summary>
    public string? ApartmentNumber { get; set; }
}

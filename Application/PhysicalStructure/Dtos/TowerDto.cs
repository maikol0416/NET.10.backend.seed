namespace Application.Dto;

public class TowerDto
{
    public Guid? Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public int Floors { get; set; }
    public List<ApartmentDto> Apartments { get; set; }

    /// <summary>
    /// Cantidad de apartamentos por piso a generar automáticamente.
    /// Si viene con valor, reemplaza cualquier apartamento en <see cref="Apartments"/>
    /// con la generación masiva (ver PhysicalStructureAgg.AddMasiveApartment).
    /// Si es null, se guarda tal cual lo que venga en <see cref="Apartments"/>.
    /// </summary>
    public int? ApartmentForFloors { get; set; }
}

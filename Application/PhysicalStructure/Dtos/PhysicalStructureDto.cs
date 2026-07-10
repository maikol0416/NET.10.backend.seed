namespace Application.Dto;

public class PhysicalStructureDto
{
    public Guid? Id { get; set; }

    /// <summary>
    /// Empresa dueña de la estructura. El backend siempre lo sobrescribe con la
    /// empresa del usuario autenticado (StampCompany) — no confíes en lo que
    /// mande el cliente en Create/Update, es solo informativo en las lecturas.
    /// </summary>
    public Guid? CompanyId { get; set; }
    public string Name { get; set; }
    public string Nit { get; set; }
    public int UnitCount { get; set; }
    public string Number { get;  set; }
    public string DetailLocation { get;  set; }
    public string Country { get;  set; }
    public string City { get;  set; }
    public string Neighborhood { get;  set; }
    public List<CommonAreaDto> CommonAreas { get; set; }
    public List<TowerDto> Towers { get; set; }
}

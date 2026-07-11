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

    /// <summary>
    /// Bytes de la imagen a guardar (solo entrada, en create/update). El backend la
    /// persiste en disco y descarta el array — nunca se devuelve en las respuestas.
    /// Si no se envía en un update, se conserva la imagen que ya tenía la estructura.
    /// </summary>
    public byte[]? ImageBytes { get; set; }

    /// <summary>
    /// Ruta de la imagen ya guardada. Es de solo lectura desde la perspectiva del
    /// cliente: el backend siempre la calcula a partir de ImageBytes — cualquier
    /// valor que el cliente mande acá se ignora.
    /// </summary>
    public string? PathImg { get; set; }
}

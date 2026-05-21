namespace Application.Dto;

public class PhysicalStructureDto
{
    public string Name { get; set; }
    public string Nit { get; set; }
    public int UnitCount { get; set; }
    public string Number { get;  set; }
    public string DetailLocation { get;  set; }
    public string Country { get;  set; }
    public string City { get;  set; }
    public string Neighborhood { get;  set; }
    public List<CommonAreaDto> CommonAreas { get; set; }
}

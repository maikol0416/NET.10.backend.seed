namespace Application.Dto;

public class TowerDto
{
    public Guid? Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public int Floors { get; set; }
    public List<ApartmentDto> Apartments { get; set; }
}

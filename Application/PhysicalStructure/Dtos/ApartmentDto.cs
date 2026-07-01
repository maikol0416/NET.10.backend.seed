namespace Application.Dto;

public class ApartmentDto
{
    public Guid? Id { get; set; }
    public string Number { get; set; }
    public string Size { get; set; }
    public Guid IdOwner { get; set; }
}

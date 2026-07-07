using Domain.BoundedContext.People.Aggregates;

namespace Application.Dto;

public class GuestDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public DocumentTypeEnum DocumentType { get; set; }
    public string DocumentNumber { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string TermsAndCondition { get; set; }
    public string ResponseTermsAndCondition { get; set; }
    public string MediaId { get; set; }
    public List<GuestPermissionDto> GuestPermissions { get; set; }
}

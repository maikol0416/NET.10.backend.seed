using System.Text.RegularExpressions;
using Domain.BoundedContext.People.Aggregates;
using Domain.DomainShared;

namespace Domain.BoundedContext.People;

public class GuestAgg : AggregateRoot
{
    public GuestAgg() { }

    public GuestAgg(string name, string lastName, DocumentTypeEnum documentType,
                    string documentNumber, string phoneNumber, string email,
                    string termsAndCondition, string responseTermsAndCondition,
                    string mediaId, List<GuestPermissionEntity> guestPermissions) : base()
    {
        Name = name;
        LastName = lastName;
        DocumentType = documentType;
        DocumentNumber = documentNumber;
        PhoneNumber = phoneNumber;
        Email = email;
        TermsAndCondition = termsAndCondition;
        ResponseTermsAndCondition = responseTermsAndCondition;
        MediaId = mediaId;
        GuestPermissions = guestPermissions ?? new List<GuestPermissionEntity>();
        ExcecuteDomainInvariants();
    }

    public string Name { get; private set; }
    public string LastName { get; private set; }
    public DocumentTypeEnum DocumentType { get; private set; }
    public string DocumentNumber { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Email { get; private set; }
    public string TermsAndCondition { get; private set; }
    public string ResponseTermsAndCondition { get; private set; }
    public string MediaId { get; private set; }
    public List<GuestPermissionEntity> GuestPermissions { get; private set; }

    public void Update(string name, string lastName, DocumentTypeEnum documentType,
                       string documentNumber, string phoneNumber, string email,
                       string termsAndCondition, string responseTermsAndCondition,
                       string mediaId)
    {
        Name = name;
        LastName = lastName;
        DocumentType = documentType;
        DocumentNumber = documentNumber;
        PhoneNumber = phoneNumber;
        Email = email;
        TermsAndCondition = termsAndCondition;
        ResponseTermsAndCondition = responseTermsAndCondition;
        MediaId = mediaId;
        ExcecuteDomainInvariants();
    }

    public void UpdateGuestPermissions(IEnumerable<GuestPermissionEntity> incomingGuestPermissions)
    {
        GuestPermissions.Clear();
        if (incomingGuestPermissions != null)
        {
            foreach (var incomingGuestPermission in incomingGuestPermissions)
            {
                GuestPermissions.Add(new GuestPermissionEntity(incomingGuestPermission.StartDate, incomingGuestPermission.EndDate,
                    incomingGuestPermission.PhysicalStructureId, incomingGuestPermission.ApartmentId));
            }
        }
    }

    protected override void ExcecuteDomainInvariants()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new DomainException("El nombre no puede ser vacío o nulo.");

        if (string.IsNullOrWhiteSpace(LastName))
            throw new DomainException("El apellido no puede ser vacío o nulo.");

        if (!Enum.IsDefined(typeof(DocumentTypeEnum), DocumentType))
            throw new DomainException("El tipo de documento no es válido.");

        if (string.IsNullOrWhiteSpace(DocumentNumber))
            throw new DomainException("El número de documento no puede ser vacío o nulo.");

        if (string.IsNullOrWhiteSpace(PhoneNumber))
            throw new DomainException("El número de teléfono no puede ser vacío o nulo.");

        if (string.IsNullOrWhiteSpace(Email))
            throw new DomainException("El correo no puede ser vacío o nulo.");

        if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new DomainException("El correo no cuenta con el formato correcto.");

        if (string.IsNullOrWhiteSpace(TermsAndCondition))
            throw new DomainException("Los términos y condiciones no pueden ser vacíos o nulos.");

        if (string.IsNullOrWhiteSpace(ResponseTermsAndCondition))
            throw new DomainException("La respuesta de términos y condiciones no puede ser vacía o nula.");

        if (string.IsNullOrWhiteSpace(MediaId))
            throw new DomainException("El MediaId no puede ser vacío o nulo.");
    }
}

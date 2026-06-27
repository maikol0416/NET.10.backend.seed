using System.Text.RegularExpressions;
using Domain.BoundedContext.People.Aggregates;
using Domain.DomainShared;

namespace Domain.BoundedContext.People;

public class OwnerAgg : AggregateRoot
{
    public OwnerAgg() { }

    public OwnerAgg(string name, string lastName, DocumentTypeEnum documentType,
                    string documentNumber, string phoneNumber, string email,
                    int? idTermsAndCondition, string responseTermsAndCondition,
                    string mediaId) : base()
    {
        Name = name;
        LastName = lastName;
        DocumentType = documentType;
        DocumentNumber = documentNumber;
        PhoneNumber = phoneNumber;
        Email = email;
        IdTermsAndCondition = idTermsAndCondition;
        ResponseTermsAndCondition = responseTermsAndCondition;
        MediaId = mediaId;
        ExcecuteDomainInvariants();
    }

    public string Name { get; private set; }
    public string LastName { get; private set; }
    public DocumentTypeEnum DocumentType { get; private set; }
    public string DocumentNumber { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Email { get; private set; }
    public int? IdTermsAndCondition { get; private set; }
    public string ResponseTermsAndCondition { get; private set; }
    public string MediaId { get; private set; }

    public void Update(string name, string lastName, DocumentTypeEnum documentType,
                       string documentNumber, string phoneNumber, string email,
                       int? idTermsAndCondition, string responseTermsAndCondition,
                       string mediaId)
    {
        Name = name;
        LastName = lastName;
        DocumentType = documentType;
        DocumentNumber = documentNumber;
        PhoneNumber = phoneNumber;
        Email = email;
        IdTermsAndCondition = idTermsAndCondition;
        ResponseTermsAndCondition = responseTermsAndCondition;
        MediaId = mediaId;
        ExcecuteDomainInvariants();
    }

    protected override void ExcecuteDomainInvariants()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new DomainException("El nombre no puede ser vacío o nulo.");

        if (string.IsNullOrWhiteSpace(LastName))
            throw new DomainException("El apellido no puede ser vacío o nulo.");

        if (string.IsNullOrWhiteSpace(DocumentNumber))
            throw new DomainException("El número de documento no puede ser vacío o nulo.");

        if (string.IsNullOrWhiteSpace(Email))
            throw new DomainException("El correo no puede ser vacío o nulo.");

        if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new DomainException("El correo no cuenta con el formato correcto.");
    }
}

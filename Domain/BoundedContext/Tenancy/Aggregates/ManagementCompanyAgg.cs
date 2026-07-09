using Domain.DomainShared;

namespace Domain.BoundedContext.Tenancy;

public class ManagementCompanyAgg : AggregateRoot
{
    public ManagementCompanyAgg() { }

    public ManagementCompanyAgg(string name,
                                string nit,
                                string contactEmail,
                                string contactPhone
                                ) : base()
    {
        Name = name;
        Nit = nit;
        ContactEmail = contactEmail;
        ContactPhone = contactPhone;
        ExcecuteDomainInvariants();
    }

    public string Name { get; private set; }
    public string Nit { get; private set; }
    public string ContactEmail { get; private set; }
    public string ContactPhone { get; private set; }

    public void Update(string name, string nit, string contactEmail, string contactPhone)
    {
        Name = name;
        Nit = nit;
        ContactEmail = contactEmail;
        ContactPhone = contactPhone;
        ExcecuteDomainInvariants();
    }

    protected override void ExcecuteDomainInvariants()
    {
        if (string.IsNullOrEmpty(Name))
            throw new DomainException("El nombre es obligatorio.");

        if (string.IsNullOrEmpty(Nit))
            throw new DomainException("El Nit es obligatorio.");

        if (string.IsNullOrEmpty(ContactEmail))
            throw new DomainException("El ContactEmail es obligatorio.");

        if (string.IsNullOrEmpty(ContactPhone))
            throw new DomainException("El ContactPhone es obligatorio.");
    }
}

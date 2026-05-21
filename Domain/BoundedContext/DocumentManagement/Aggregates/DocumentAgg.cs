using Domain.DomainShared;

namespace Domain.BoundedContext.DocumentManagement;

public class DocumentAgg : AggregateRoot
{
    // Constructor para ORM (Entity Framework)
    public DocumentAgg() { }

    // Constructor de negocio
    public DocumentAgg(
        string name,
        string? description,
        string path,
        List<SignatureValueObject> signatures
        ) : base()
    {
        Name = name;
        Description = description;
        Path = path;
        Signatures = signatures;
        ExcecuteDomainInvariants();
    }

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string Path { get; private set; }
    public List<SignatureValueObject> Signatures { get; private set; }

    protected override void ExcecuteDomainInvariants()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new DomainException("El nombre es obligatorio.");

        if (Name.Length > 150)
            throw new DomainException("El nombre no puede exceder 150 caracteres.");

        if (string.IsNullOrWhiteSpace(Path))
            throw new DomainException("La ruta es obligatoria.");
    }
}

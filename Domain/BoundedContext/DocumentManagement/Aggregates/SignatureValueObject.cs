using Domain.DomainShared;

namespace Domain.BoundedContext.DocumentManagement;

public record SignatureValueObject : ValueObject
{
    public SignatureValueObject(
        string name,
        string rol
        )
    {
        if (string.IsNullOrEmpty(name))
            throw new DomainException("El nombre del firmante es obligatorio.");

        if (string.IsNullOrEmpty(rol))
            throw new DomainException("El rol del firmante es obligatorio.");

        Name = name;
        Rol = rol;
    }

    public string Name { get; private set; }
    public string Rol { get; private set; }
}

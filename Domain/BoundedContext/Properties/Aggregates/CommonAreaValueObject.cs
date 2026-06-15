using Domain.DomainShared;

namespace Domain.BoundedContext.Properties;

public record CommonAreaValueObject : ValueObject
{
    public CommonAreaValueObject(string name, string description)
    {
        if(string.IsNullOrEmpty(name))
            throw new DomainException("el nombre del área común es obligatorio.");
        
        if(string.IsNullOrEmpty(description))
            throw new DomainException("la descripción del área común es obligatoria.");
        
        Name = name;
        Description = description;
    }
    public string Name { get; private set; }
    public string Description { get; private set; }
}

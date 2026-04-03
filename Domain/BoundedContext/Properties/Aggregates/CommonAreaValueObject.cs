using Domain.DomainShared;

namespace Domain.BoundedContext.Properties;

public record CommonAreaValueObject: ValueObject
{
    public CommonAreaValueObject(string name,string description)
    {
        if(string.IsNullOrEmpty(Name))
            throw new DomainException("Number cannot be null");
        
        if(string.IsNullOrEmpty(Description))
            throw new DomainException("Detail cannot be null");
        
        Name = name;
        Description = description;
    }
    public string Name { get; private set; }
    public string Description { get; private set; }
}

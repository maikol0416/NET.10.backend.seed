namespace Domain.BoundedContext.Properties;

public record LocationValueObject : ValueObject
{
    public LocationValueObject(string street,string number)
    {
        Street = street;
        Number = number;
    }
    public string Street { get; private set; }
    public string Number { get; private set; }
}

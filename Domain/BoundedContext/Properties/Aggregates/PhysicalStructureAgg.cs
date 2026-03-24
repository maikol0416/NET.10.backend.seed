namespace Domain.BoundedContext.Properties;

public class PhysicalStructureAgg : AggregateRoot
{
    public PhysicalStructureAgg()
    {
        
    }
    public PhysicalStructureAgg(string name,LocationValueObject location)
    {
        Location = location;
        Name = name;
    }
    public  string Name { get; private set; }
    public LocationValueObject Location { get; private set; }
}

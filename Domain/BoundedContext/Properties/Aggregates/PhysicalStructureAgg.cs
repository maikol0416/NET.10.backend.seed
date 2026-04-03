using Domain.DomainShared;

namespace Domain.BoundedContext.Properties;

public class PhysicalStructureAgg : AggregateRoot
{
    public PhysicalStructureAgg()
    {
        
    }
    public PhysicalStructureAgg(string name,
                                string nit,
                                int unitCount,
                                LocationValueObject location,
                                List<CommonAreaValueObject> commonAreas
                                )
    {
        Name = name;
        Nit = nit;
        UnitCount = unitCount;
        CommonsAreas = commonAreas;
        Location = location;
        ExcecuteDomainInvariants();

    }
    public  string Name { get; private set; }
    public string Nit { get; set; }
    public int UnitCount { get; set; }
    public List<CommonAreaValueObject> CommonsAreas { get; private set; }
    public LocationValueObject Location { get; private set; }

    protected override void ExcecuteDomainInvariants()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new DomainException("La estructura física debe tener un nombre.");

        if (Name.Length > 150)
            throw new DomainException("El nombre no puede exceder los 150 caracteres.");

        if (Location == null)
            throw new DomainException("La ubicación es obligatoria.");
    }
}

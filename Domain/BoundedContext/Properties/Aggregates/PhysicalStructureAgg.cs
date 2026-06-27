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
                                List<CommonAreaEntity> commonAreas,
                                List<TowerEntity> towers
                                ):base()
    {
        Name = name;
        Nit = nit;
        UnitCount = unitCount;
        CommonsAreas = commonAreas;
        Location = location;
        Towers = towers ?? new List<TowerEntity>();
        ExcecuteDomainInvariants();

    }
    public  string Name { get; private set; }
    public string Nit { get; set; }
    public int UnitCount { get; set; }
    public List<CommonAreaEntity> CommonsAreas { get; private set; }
    public LocationValueObject Location { get; private set; }
    public List<TowerEntity> Towers { get; private set; }

    /// <summary>
    /// Actualiza los campos básicos de la estructura física.
    /// Las áreas comunes y torres se sincronizan a través de sus respectivos métodos.
    /// </summary>
    public void Update(string name, string nit, int unitCount)
    {
        Name = name;
        Nit = nit;
        UnitCount = unitCount;
        ExcecuteDomainInvariants();
    }

    public void UpdateTowers(IEnumerable<TowerEntity> incomingTowers)
    {
        Towers.Clear();
        if (incomingTowers != null)
        {
            foreach (var incomingTower in incomingTowers)
            {
                Towers.Add(new TowerEntity(incomingTower.Number, incomingTower.Floors));
            }
        }
    }

    public void UpdateCommonsAreas(IEnumerable<CommonAreaEntity> incomingCommonAreas)
    {
        CommonsAreas.Clear();
        if (incomingCommonAreas != null)
        {
            foreach (var incomingCommonArea in incomingCommonAreas)
            {
                CommonsAreas.Add(new CommonAreaEntity(incomingCommonArea.Name, incomingCommonArea.Description));
            }
        }
    }

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

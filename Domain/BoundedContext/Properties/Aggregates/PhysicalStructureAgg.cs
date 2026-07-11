using Domain.DomainShared;

namespace Domain.BoundedContext.Properties;

public class PhysicalStructureAgg : AggregateRoot
{
    public PhysicalStructureAgg()
    {
        
    }
    public PhysicalStructureAgg(Guid companyId,
                                string name,
                                string nit,
                                int unitCount,
                                LocationValueObject location,
                                List<CommonAreaEntity> commonAreas,
                                List<TowerEntity> towers,
                                string? pathImg = null
                                ):base()
    {
        CompanyId = companyId;
        Name = name;
        Nit = nit;
        UnitCount = unitCount;
        CommonsAreas = commonAreas;
        Location = location;
        Towers = towers ?? new List<TowerEntity>();
        PathImg = pathImg;
        ExcecuteDomainInvariants();

    }

    /// <summary>
    /// Empresa administradora (tenant) dueña de esta estructura física.
    /// Una empresa puede tener muchas estructuras; una estructura pertenece a una sola
    /// empresa, y esa pertenencia es inmutable una vez creada (no se "traspasa").
    /// </summary>
    public Guid CompanyId { get; private set; }
    public  string Name { get; private set; }
    public string Nit { get; set; }
    public int UnitCount { get; set; }
    public List<CommonAreaEntity> CommonsAreas { get; private set; }
    public LocationValueObject Location { get; private set; }
    public List<TowerEntity> Towers { get; private set; }

    /// <summary>Ruta local (relativa) de la imagen de la estructura física. Opcional.</summary>
    public string? PathImg { get; private set; }

    /// <summary>
    /// Actualiza los campos básicos de la estructura física.
    /// Las áreas comunes y torres se sincronizan a través de sus respectivos métodos.
    /// </summary>
    public void Update(string name, string nit, int unitCount, string? pathImg)
    {
        Name = name;
        Nit = nit;
        UnitCount = unitCount;
        PathImg = pathImg;
        ExcecuteDomainInvariants();
    }

    public void UpdateTowers(IEnumerable<TowerEntity> incomingTowers)
    {
        Towers.Clear();
        if (incomingTowers != null)
        {
            foreach (var incomingTower in incomingTowers)
            {
                var newTower = new TowerEntity(incomingTower.Number, incomingTower.Floors);
                newTower.UpdateApartments(incomingTower.Apartments);
                Towers.Add(newTower);
            }
        }
    }

    /// <summary>
    /// Genera masivamente los apartamentos de una torre, distribuidos por piso.
    /// El número de cada apartamento se compone del piso + un consecutivo por piso
    /// (ej. piso 1 con 4 aptos por piso: 101, 102, 103, 104; piso 2: 201, 202, 203, 204).
    /// Reemplaza cualquier apartamento existente en la torre — Size e IdOwner nacen
    /// sin asignar y se completan después mediante ApartmentEntity.Update(...).
    /// </summary>
    public void AddMasiveApartment(string towerNumber, int apartmentsPerFloor)
    {
        if (apartmentsPerFloor <= 0)
            throw new DomainException("La cantidad de apartamentos por piso debe ser mayor a cero.");

        var tower = Towers.FirstOrDefault(t => t.Number == towerNumber)
            ?? throw new DomainException($"No se encontró la torre '{towerNumber}' para generar apartamentos.");

        var generatedApartments = new List<ApartmentEntity>();
        for (var floor = 1; floor <= tower.Floors; floor++)
        {
            for (var sequence = 1; sequence <= apartmentsPerFloor; sequence++)
            {
                var apartmentNumber = (floor * 100 + sequence).ToString();
                generatedApartments.Add(new ApartmentEntity(apartmentNumber));
            }
        }

        tower.UpdateApartments(generatedApartments);
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
        if (CompanyId == Guid.Empty)
            throw new DomainException("La estructura física debe pertenecer a una empresa.");

        if (string.IsNullOrWhiteSpace(Name))
            throw new DomainException("La estructura física debe tener un nombre.");

        if (Name.Length > 150)
            throw new DomainException("El nombre no puede exceder los 150 caracteres.");

        if (Location == null)
            throw new DomainException("La ubicación es obligatoria.");
    }
}

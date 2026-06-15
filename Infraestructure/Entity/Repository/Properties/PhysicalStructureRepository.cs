using Domain.Ports;
using Domain.BoundedContext.Properties;
using Infraestructure.Repository.Shared;
using Microsoft.EntityFrameworkCore;
using Domain.Ports.Repository.Base;

namespace Infraestructure.Repository.Properties;

public class PhysicalStructureRepository: BaseRepositiry<PhysicalStructureAgg>, IPhysicalStructureRepository
{
    public PhysicalStructureRepository(IEntityDbContext entityDbContext):
    base(entityDbContext)
    {
        
    }

    /// <summary>
    /// Actualiza la estructura física excluyendo las áreas comunes (CommonAreas),
    /// ya que son Value Objects inmutables. EF Core no puede rastrear nuevas instancias
    /// de CommonArea sin su shadow key "Id".
    /// </summary>
    public override async Task<PhysicalStructureAgg> UpdateAsync(PhysicalStructureAgg ent)
    {
        // Buscamos el agregado ya trackeado por EF para preservar las CommonAreas originales
        var tracked = await entity
            .Include(p => p.CommonsAreas)
            .FirstOrDefaultAsync(p => p.Id == ent.Id);

        if (tracked is null)
            throw new InvalidOperationException($"No se encontró PhysicalStructure con Id={ent.Id}.");

        // Aplicamos solo los campos de negocio mutables, sin tocar CommonsAreas
        tracked.UpdateBasicInfo(ent.Name, ent.Nit, ent.UnitCount, ent.Location);

        await MainContext.SaveChangesAsync();
        return tracked;
    }
}

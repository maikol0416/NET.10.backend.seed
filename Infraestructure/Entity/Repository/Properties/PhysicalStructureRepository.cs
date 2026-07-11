using Domain.Ports;
using Domain.BoundedContext.Properties;
using Infraestructure.Repository.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repository.Properties;

public class PhysicalStructureRepository: BaseRepositiry<PhysicalStructureAgg>, IPhysicalStructureRepository
{
    public PhysicalStructureRepository(IEntityDbContext entityDbContext):
    base(entityDbContext)
    {
        
    }

    /// <summary>
    /// Actualiza la estructura física.
    /// Carga el agregado completo y reemplaza las torres y zonas comunes.
    /// Location no se modifica en el update.
    /// </summary>
    public override async Task<PhysicalStructureAgg> UpdateAsync(PhysicalStructureAgg ent)
    {
        // Cargar el agregado completo con sus owned entities
        var tracked = await entity
            .Include(p => p.CommonsAreas)
            .Include(p => p.Towers)
                .ThenInclude(t => t.Apartments)
            .FirstOrDefaultAsync(p => p.Id == ent.Id)
            ?? throw new Exception($"No se encontró la estructura física con Id {ent.Id} para actualizar.");

        tracked.Update(ent.Name, ent.Nit, ent.UnitCount, ent.PathImg, ent.AdministratorUserId);
        tracked.UpdateTowers(ent.Towers);
        tracked.UpdateCommonsAreas(ent.CommonsAreas);

        await MainContext.SaveChangesAsync();
        return tracked;
    }
}

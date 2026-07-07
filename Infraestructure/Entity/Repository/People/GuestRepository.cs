using Domain.Ports;
using Domain.BoundedContext.People;
using Infraestructure.Repository.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repository.People;

public class GuestRepository : BaseRepositiry<GuestAgg>, IGuestRepository
{
    public GuestRepository(IEntityDbContext entityDbContext) : base(entityDbContext)
    {
    }

    /// <summary>
    /// Actualiza el huésped. Carga el agregado completo y reemplaza los permisos de acceso.
    /// </summary>
    public override async Task<GuestAgg> UpdateAsync(GuestAgg ent)
    {
        var tracked = await entity
            .Include(p => p.GuestPermissions)
            .FirstOrDefaultAsync(p => p.Id == ent.Id)
            ?? throw new Exception($"No se encontró el huésped con Id {ent.Id} para actualizar.");

        tracked.Update(ent.Name, ent.LastName, ent.DocumentType, ent.DocumentNumber,
            ent.PhoneNumber, ent.Email, ent.TermsAndCondition, ent.ResponseTermsAndCondition, ent.MediaId);
        tracked.UpdateGuestPermissions(ent.GuestPermissions);

        await MainContext.SaveChangesAsync();
        return tracked;
    }
}

using System.Linq.Expressions;
using Application.Base;
using Application.Dto;
using Domain.BoundedContext.People;
using Domain.DomainShared;
using Domain.Ports;

namespace Application.Service;

public class GuestReadOnlyService : ApplicationReadOnlyService<GuestAgg, GuestDto>, IGuestReadOnlyService
{
    private readonly IPhysicalStructureReadOnlyRepository _physicalStructureReadOnlyRepository;

    public GuestReadOnlyService(
        IGuestReadOnlyRepository repository,
        IPhysicalStructureReadOnlyRepository physicalStructureReadOnlyRepository) : base(repository)
    {
        _physicalStructureReadOnlyRepository = physicalStructureReadOnlyRepository;

        CreateMapperExpresion<GuestAgg, GuestDto>(cnf =>
        {
            GuestMapper.Expresion(cnf);
        });
    }

    public override async Task<GuestDto?> GetByIdAsync(Guid id)
    {
        var dto = await base.GetByIdAsync(id);
        if (dto is null)
            return null;

        await EnrichPermissionsAsync(new[] { dto });
        return dto;
    }

    public override async Task<IEnumerable<GuestDto>> GetAllAsync()
    {
        var dtos = (await base.GetAllAsync()).ToList();
        await EnrichPermissionsAsync(dtos);
        return dtos;
    }

    public override async Task<IEnumerable<GuestDto>> FindAsync(Expression<Func<GuestAgg, bool>> predicate)
    {
        var dtos = (await base.FindAsync(predicate)).ToList();
        await EnrichPermissionsAsync(dtos);
        return dtos;
    }

    public override async Task<PaginatedList<GuestDto>> GetPaginatedAsync(int pageNumber, int pageSize)
    {
        var result = await base.GetPaginatedAsync(pageNumber, pageSize);
        var items = result.Items.ToList();
        await EnrichPermissionsAsync(items);
        return new PaginatedList<GuestDto>(items, result.TotalCount, result.PageNumber, result.PageSize);
    }

    public override async Task<PaginatedList<GuestDto>> GetPaginatedAsync(int pageNumber, int pageSize, Expression<Func<GuestAgg, bool>> predicate)
    {
        var result = await base.GetPaginatedAsync(pageNumber, pageSize, predicate);
        var items = result.Items.ToList();
        await EnrichPermissionsAsync(items);
        return new PaginatedList<GuestDto>(items, result.TotalCount, result.PageNumber, result.PageSize);
    }

    /// <summary>
    /// Resuelve, para cada GuestPermission de los guests dados, el nombre de la estructura
    /// física y el número del apartamento (cuando aplica). Cruza contra BC Properties en una
    /// sola consulta batched por Id para evitar N+1.
    /// </summary>
    private async Task EnrichPermissionsAsync(IEnumerable<GuestDto> guests)
    {
        var physicalStructureIds = guests
            .SelectMany(g => g.GuestPermissions ?? new List<GuestPermissionDto>())
            .Select(p => p.PhysicalStructureId)
            .Distinct()
            .ToList();

        if (physicalStructureIds.Count == 0)
            return;

        var physicalStructures = await _physicalStructureReadOnlyRepository
            .FindAsync(ps => physicalStructureIds.Contains(ps.Id));
        var structuresById = physicalStructures.ToDictionary(ps => ps.Id);

        foreach (var permission in guests.SelectMany(g => g.GuestPermissions ?? new List<GuestPermissionDto>()))
        {
            if (!structuresById.TryGetValue(permission.PhysicalStructureId, out var structure))
                continue;

            permission.PhysicalStructureName = structure.Name;

            if (permission.ApartmentId.HasValue)
            {
                var apartment = structure.Towers
                    .SelectMany(t => t.Apartments)
                    .FirstOrDefault(a => a.Id == permission.ApartmentId.Value);
                permission.ApartmentNumber = apartment?.Number;
            }
        }
    }
}

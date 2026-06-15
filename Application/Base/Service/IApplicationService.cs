namespace Application.Base;

public interface IApplicationService<ENT, DTO>
    where ENT : class, new()
    where DTO : class, new()
{
    Task<DTO> CreateAsync(DTO dto);
    Task<bool> CreateListAsync(List<DTO> dtos);
    Task<bool> DeleteEntity(Guid id);
    Task<DTO> UpdateAsync(DTO dto);
}


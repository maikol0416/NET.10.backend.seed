namespace Application.Base;

 public interface IApplicationService<ENT, DTO>
    where ENT : class, new()
    where DTO : class, new()
{
    Task<DTO> CreateAsync(DTO dto);
    Task<bool> CreateListAsync(List<DTO> dtos);
    Task<bool> DeleteEntity(int id);
    Task<DTO> UpdateAsync(DTO dto);
    DTO MapToDTO(ENT entity);
    ENT MapToENT(DTO dto);
    List<DTO> MapLstToDTO(List<ENT> entity);
    List<ENT> MapLstToENT(List<DTO> dto);

}

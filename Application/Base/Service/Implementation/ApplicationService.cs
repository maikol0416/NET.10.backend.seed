using AutoMapper;
using Domain.Ports.Repository.Base;

namespace Application.Base;

public abstract partial class ApplicationService<ENT, DTO> : IApplicationService<ENT, DTO>
        where ENT : class, new()
        where DTO : class, new()
    {
        protected IBaseRepository<ENT> RepositoryBase { get; set; }

        public ApplicationService(IBaseRepository<ENT> repositoryBase)
        {
            RepositoryBase = repositoryBase;
        }
        public ApplicationService()
        {

        }

        public virtual async Task<DTO> CreateAsync(DTO dto)
        {
            var entity = MapToENT(dto);
            // var client = _util.GetHeaderRequest(EHeaders.CodeClient); ::TODO multi tenant
            // SetPropertyValue("CodeClient", entity, client); ::TODO multi tenant
            return MapToDTO(await RepositoryBase.CreateAsync(entity));
        }

        public virtual async Task<bool> CreateListAsync(List<DTO> dtos)
        {
            // var client = _util.GetHeaderRequest(EHeaders.CodeClient);
            // entities.ForEach(f => SetPropertyValue("CodeClient", f, client));
            var entities = MapLstToENT(dtos);
            await RepositoryBase.CreateListAsync(entities);
            return await Task.FromResult(true);
        }
      
        public virtual async Task<bool> DeleteEntity(int id)
        {
            return await RepositoryBase.DeleteEntity(id);
        }

        public virtual async Task<DTO> UpdateAsync(DTO dto)
        {
            // var client = _util.GetHeaderRequest(EHeaders.CodeClient);  ::TODO for multi tenant
            // SetPropertyValue("CodeClient", dto, client); ::TODO for multi tenant
            return MapToDTO(await RepositoryBase.UpdateAsync(MapToENT(dto)));
        }
    }

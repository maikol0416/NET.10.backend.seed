using AutoMapper;
using Domain.Ports.Repository.Base;

namespace Application.Base;

public abstract partial class ApplicationService<ENT, DTO> : IApplicationService<ENT, DTO>
        where ENT : class, new()
        where DTO : class, new()
    {

        private IMapper Mapper;
        
        public DTO MapToDTO(ENT entity)
        {
            return Mapper.Map<DTO>(entity);
        }

        public ENT MapToENT(DTO dto)
        {
            return Mapper.Map<ENT>(dto);
        }

        public List<DTO> MapLstToDTO(List<ENT> entity)
        {
            return Mapper.Map<List<DTO>>(entity);
        }

        public List<ENT> MapLstToENT(List<DTO> dto)
        {
            return Mapper.Map<List<ENT>>(dto);
        }
    }

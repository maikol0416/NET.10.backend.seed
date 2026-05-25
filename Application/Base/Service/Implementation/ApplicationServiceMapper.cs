using AutoMapper;

namespace Application.Base;

public abstract partial class ApplicationServiceMapper<ENT, DTO>
        where ENT : class, new()
        where DTO : class, new()
    {
        
        private IMapper Mapper;
        private MapperConfigurationExpression configurationmapper;
        protected void CreateMapper()
        {
            MapperConfiguration cnfMapper = new(configurationmapper);
            Mapper = cnfMapper.CreateMapper();
        }
        public void CreateMapperExpresion<ENT, DTO>(Action<IMapperConfigurationExpression> configure) where ENT : class, new() where DTO : class, new()
        {
            configurationmapper = CreateConfiguration<ENT, DTO>();
            CreateMapperExpresion(configure);
        }
        public void CreateMapperExpresion(Action<IMapperConfigurationExpression> configure)
        {
            configure(configurationmapper);
            CreateMapper();
        }

        protected void CreateMapper<ENT, DTO>() where ENT : class, new() where DTO : class, new()
        {
            configurationmapper = CreateConfiguration<ENT, DTO>();
            CreateMapper();
        }

        private static MapperConfigurationExpression CreateConfiguration<ENT, DTO>() where ENT : class, new() where DTO : class, new()
        {
            var cnf = new MapperConfigurationExpression
            {
                AllowNullCollections = true
            };
            cnf.CreateMap<ENT, DTO>();
            cnf.CreateMap<DTO, ENT>();
            cnf.CreateMap<DateTimeOffset, DateTime>().ConvertUsing(n => n.UtcDateTime);
            cnf.CreateMap<DateTime, DateTimeOffset>().ConvertUsing(n => DateTime.SpecifyKind(n, DateTimeKind.Utc));
            return cnf;
        }
        

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

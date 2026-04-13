using Application.Dto;
using AutoMapper;
using Domain.BoundedContext.Properties;

namespace Application.Service;

public static class PhysicalStructureMapper
    {
        public static void Expresion(IMapperConfigurationExpression cnf)
        {
            cnf.CreateMap<PhysicalStructureDto, PhysicalStructureAgg>()
                .ConstructUsing(src => new PhysicalStructureAgg(
                    src.Name,
                    src.Nit,
                    src.UnitCount,
                    new LocationValueObject(
                        src.Number,
                        src.DetailLocation,
                        src.Country,
                        src.City,
                        src.Neighborhood
                    ),
                    new List<CommonAreaValueObject>()
                    {
                        new CommonAreaValueObject("Salon solical","Descripción salon solical")
                    }
                ));
            
            cnf.CreateMap<PhysicalStructureAgg, PhysicalStructureDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Nit, opt => opt.MapFrom(src => src.Nit))
                .ForMember(dest => dest.UnitCount, opt => opt.MapFrom(src => src.UnitCount))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Location.Number))
                .ForMember(dest => dest.DetailLocation, opt => opt.MapFrom(src => src.Location.Detail))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Location.Country))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Location.City))
                .ForMember(dest => dest.Neighborhood, opt => opt.MapFrom(src => src.Location.Neighborhood)
            );
        }
    }

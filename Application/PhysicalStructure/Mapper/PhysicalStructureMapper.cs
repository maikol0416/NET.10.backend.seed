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
                src.CommonAreas                                 
                    .Select(ca => new CommonAreaEntity(ca.Name, ca.Description))
                    .ToList(),
                src.Towers
                    .Select(t => t.Id.HasValue && t.Id.Value != Guid.Empty
                        ? new TowerEntity(t.Id.Value, t.Number)
                        : new TowerEntity(t.Number))
                    .ToList()
            ))
            .ForMember(dest => dest.Name,        opt => opt.Ignore())
            .ForMember(dest => dest.CommonsAreas, opt => opt.Ignore())  
            .ForMember(dest => dest.Location,    opt => opt.Ignore())
            .ForMember(dest => dest.Towers,      opt => opt.Ignore()); 

        cnf.CreateMap<PhysicalStructureAgg, PhysicalStructureDto>()
            .ForMember(dest => dest.Id,             opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name,           opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Nit,            opt => opt.MapFrom(src => src.Nit))
            .ForMember(dest => dest.UnitCount,      opt => opt.MapFrom(src => src.UnitCount))
            .ForMember(dest => dest.Number,         opt => opt.MapFrom(src => src.Location.Number))
            .ForMember(dest => dest.DetailLocation, opt => opt.MapFrom(src => src.Location.Detail))
            .ForMember(dest => dest.Country,        opt => opt.MapFrom(src => src.Location.Country))
            .ForMember(dest => dest.City,           opt => opt.MapFrom(src => src.Location.City))
            .ForMember(dest => dest.Neighborhood,   opt => opt.MapFrom(src => src.Location.Neighborhood))
            .ForMember(dest => dest.CommonAreas,    opt => opt.MapFrom(src =>
                src.CommonsAreas
                    .Select(ca => new CommonAreaDto { Name = ca.Name, Description = ca.Description })
                    .ToList()
            ))
            .ForMember(dest => dest.Towers,         opt => opt.MapFrom(src =>
                src.Towers
                    .Select(t => new TowerDto { Id = t.Id, Number = t.Number })
                    .ToList()
            ));
    }
}

using Application.Dto;
using AutoMapper;
using Domain.BoundedContext.Tenancy;

namespace Application.Service;

public static class ManagementCompanyMapper
{
    public static void Expresion(IMapperConfigurationExpression cnf)
    {
        cnf.CreateMap<ManagementCompanyDto, ManagementCompanyAgg>()
            .ConstructUsing(src => new ManagementCompanyAgg(
                src.Name,
                src.Nit,
                src.ContactEmail,
                src.ContactPhone
            ))
            .ForMember(dest => dest.Name,          opt => opt.Ignore())
            .ForMember(dest => dest.Nit,            opt => opt.Ignore())
            .ForMember(dest => dest.ContactEmail,   opt => opt.Ignore())
            .ForMember(dest => dest.ContactPhone,   opt => opt.Ignore());

        cnf.CreateMap<ManagementCompanyAgg, ManagementCompanyDto>()
            .ForMember(dest => dest.Id,             opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name,           opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Nit,            opt => opt.MapFrom(src => src.Nit))
            .ForMember(dest => dest.ContactEmail,   opt => opt.MapFrom(src => src.ContactEmail))
            .ForMember(dest => dest.ContactPhone,   opt => opt.MapFrom(src => src.ContactPhone));
    }
}

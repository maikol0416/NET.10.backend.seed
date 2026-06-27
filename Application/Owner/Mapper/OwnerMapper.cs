using Application.Dto;
using AutoMapper;
using Domain.BoundedContext.People;

namespace Application.Service;

public static class OwnerMapper
{
    public static void Expresion(IMapperConfigurationExpression cnf)
    {
        cnf.CreateMap<OwnerDto, OwnerAgg>()
            .ConstructUsing(src => new OwnerAgg(
                src.Name,
                src.LastName,
                src.DocumentType,
                src.DocumentNumber,
                src.PhoneNumber,
                src.Email,
                src.IdTermsAndCondition,
                src.ResponseTermsAndCondition,
                src.MediaId
            ))
            .ForMember(dest => dest.Name, opt => opt.Ignore())
            .ForMember(dest => dest.LastName, opt => opt.Ignore())
            .ForMember(dest => dest.DocumentType, opt => opt.Ignore())
            .ForMember(dest => dest.DocumentNumber, opt => opt.Ignore())
            .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.IdTermsAndCondition, opt => opt.Ignore())
            .ForMember(dest => dest.ResponseTermsAndCondition, opt => opt.Ignore())
            .ForMember(dest => dest.MediaId, opt => opt.Ignore());

        cnf.CreateMap<OwnerAgg, OwnerDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.DocumentType))
            .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.DocumentNumber))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.IdTermsAndCondition, opt => opt.MapFrom(src => src.IdTermsAndCondition))
            .ForMember(dest => dest.ResponseTermsAndCondition, opt => opt.MapFrom(src => src.ResponseTermsAndCondition))
            .ForMember(dest => dest.MediaId, opt => opt.MapFrom(src => src.MediaId));
    }
}

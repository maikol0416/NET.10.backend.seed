using Application.Dto;
using AutoMapper;
using Domain.BoundedContext.DocumentManagement;

namespace Application.Service;

public static class DocumentMapper
{
    public static void Expresion(IMapperConfigurationExpression cnf)
    {
        cnf.CreateMap<DocumentDto, DocumentAgg>()
            .ConstructUsing(src => new DocumentAgg(
                src.Name,
                src.Description,
                src.Path,
                src.Signatures                                          
                    .Select(s => new SignatureValueObject(s.Name, s.Rol))
                    .ToList()
            ))
            .ForMember(dest => dest.Name,        opt => opt.Ignore())  
            .ForMember(dest => dest.Description, opt => opt.Ignore()) 
            .ForMember(dest => dest.Path,        opt => opt.Ignore()) 
            .ForMember(dest => dest.Signatures,  opt => opt.Ignore()); 

        cnf.CreateMap<DocumentAgg, DocumentDto>()
            .ForMember(dest => dest.Name,        opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Path,        opt => opt.MapFrom(src => src.Path))
            .ForMember(dest => dest.Signatures,  opt => opt.MapFrom(src =>
                src.Signatures
                    .Select(s => new SignatureDto { Name = s.Name, Rol = s.Rol })
                    .ToList()
            ));
    }
}


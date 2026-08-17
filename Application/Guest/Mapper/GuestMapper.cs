using Application.Dto;
using AutoMapper;
using Domain.BoundedContext.People;

namespace Application.Service;

public static class GuestMapper
{
    public static void Expresion(IMapperConfigurationExpression cnf)
    {
        cnf.CreateMap<GuestDto, GuestAgg>()
            .ConstructUsing(src => new GuestAgg(
                src.Name,
                src.LastName,
                src.DocumentType,
                src.DocumentNumber,
                src.PhoneNumber,
                src.Email,
                src.TermsAndCondition,
                src.ResponseTermsAndCondition,
                src.MediaId,
                src.GuestPermissions != null
                    ? src.GuestPermissions.Select(gp => gp.Id.HasValue && gp.Id.Value != Guid.Empty
                        ? new GuestPermissionEntity(gp.Id.Value, gp.StartDate, gp.EndDate, gp.PhysicalStructureId, gp.ApartmentId)
                        : new GuestPermissionEntity(gp.StartDate, gp.EndDate, gp.PhysicalStructureId, gp.ApartmentId)).ToList()
                    : new List<GuestPermissionEntity>()
            ))
            .ForMember(dest => dest.Name, opt => opt.Ignore())
            .ForMember(dest => dest.LastName, opt => opt.Ignore())
            .ForMember(dest => dest.DocumentType, opt => opt.Ignore())
            .ForMember(dest => dest.DocumentNumber, opt => opt.Ignore())
            .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.TermsAndCondition, opt => opt.Ignore())
            .ForMember(dest => dest.ResponseTermsAndCondition, opt => opt.Ignore())
            .ForMember(dest => dest.MediaId, opt => opt.Ignore())
            .ForMember(dest => dest.GuestPermissions, opt => opt.Ignore());

        cnf.CreateMap<GuestAgg, GuestDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.DocumentType))
            .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.DocumentNumber))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.TermsAndCondition, opt => opt.MapFrom(src => src.TermsAndCondition))
            .ForMember(dest => dest.ResponseTermsAndCondition, opt => opt.MapFrom(src => src.ResponseTermsAndCondition))
            .ForMember(dest => dest.MediaId, opt => opt.MapFrom(src => src.MediaId))
            .ForMember(dest => dest.GuestPermissions, opt => opt.MapFrom(src => src.GuestPermissions
                .Select(gp => new GuestPermissionDto { Id = gp.Id, StartDate = gp.StartDate, EndDate = gp.EndDate, PhysicalStructureId = gp.PhysicalStructureId, ApartmentId = gp.ApartmentId })
                .ToList()));
    }
}

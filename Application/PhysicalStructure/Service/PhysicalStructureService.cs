using Application.Base;
using Application.Dto;
using Domain.BoundedContext.Properties;
using Domain.Ports;
using Domain.Ports.Identity;

namespace Application.Service;

public class PhysicalStructureService:ApplicationService<PhysicalStructureAgg,PhysicalStructureDto> ,IPhysicalStructureService
{
    private readonly IImageStorageService _imageStorageService;

    public PhysicalStructureService(
        IPhysicalStructureRepository physicalStructureRepository,
        ICurrentUserService currentUser,
        IImageStorageService imageStorageService) : base(physicalStructureRepository, currentUser)
    {
        _imageStorageService = imageStorageService;
        CreateMapperExpresion<PhysicalStructureAgg, PhysicalStructureDto>(cnf =>
            {
                PhysicalStructureMapper.Expresion(cnf);
            });
    }

    public override async Task<PhysicalStructureDto> CreateAsync(PhysicalStructureDto dto)
    {
        if (dto.ImageBytes is { Length: > 0 })
        {
            dto.PathImg = await _imageStorageService.SaveAsync(dto.ImageBytes);
        }

        return await base.CreateAsync(dto);
    }

    public override async Task<PhysicalStructureDto> UpdateAsync(PhysicalStructureDto dto)
    {
        if (dto.ImageBytes is { Length: > 0 })
        {
            dto.PathImg = await _imageStorageService.SaveAsync(dto.ImageBytes);
        }
        else if (dto.Id.HasValue)
        {
            var existing = await RepositoryBase.GetById(dto.Id.Value);
            dto.PathImg = existing?.PathImg;
        }

        return await base.UpdateAsync(dto);
    }
}

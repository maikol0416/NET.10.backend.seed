using Application.Base;
using Application.Dto;
using Domain.BoundedContext.Properties;
using Domain.Ports;
using Domain.Ports.Identity;

namespace Application.Service;

public class PhysicalStructureService:ApplicationService<PhysicalStructureAgg,PhysicalStructureDto> ,IPhysicalStructureService
{
    public PhysicalStructureService(IPhysicalStructureRepository physicalStructureRepository, ICurrentUserService currentUser) : base(physicalStructureRepository, currentUser)
    {
        CreateMapperExpresion<PhysicalStructureAgg, PhysicalStructureDto>(cnf =>
            {
                PhysicalStructureMapper.Expresion(cnf);
            });
    }
}

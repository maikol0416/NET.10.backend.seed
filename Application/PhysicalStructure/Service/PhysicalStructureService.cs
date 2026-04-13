using Application.Base;
using Application.Dto;
using Domain.BoundedContext.Properties;
using Domain.Ports;

namespace Application.Service;

public class PhysicalStructureService:ApplicationService<PhysicalStructureAgg,PhysicalStructureDto> ,IPhysicalStructureService
{
    public PhysicalStructureService(IPhysicalStructureRepository physicalStructureRepository) : base(physicalStructureRepository)
    {
        CreateMapperExpresion<PhysicalStructureAgg, PhysicalStructureDto>(cnf =>
            {
                PhysicalStructureMapper.Expresion(cnf);
            });
    }
}

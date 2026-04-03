using Application.Base;
using Application.Dto;
using Domain.BoundedContext.Properties;

namespace Application.Service;

public class PhysicalStructureService:ApplicationService<PhysicalStructureAgg,PhysicalStructureDto> ,IPhysicalStructureService
{

}

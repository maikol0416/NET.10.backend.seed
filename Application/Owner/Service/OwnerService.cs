using Application.Base;
using Application.Dto;
using Domain.BoundedContext.People;
using Domain.Ports;

namespace Application.Service;

public class OwnerService : ApplicationService<OwnerAgg, OwnerDto>, IOwnerService
{
    public OwnerService(IOwnerRepository ownerRepository) : base(ownerRepository)
    {
        CreateMapperExpresion<OwnerAgg, OwnerDto>(cnf =>
        {
            OwnerMapper.Expresion(cnf);
        });
    }
}

using Application.Base;
using Application.Dto;
using Domain.BoundedContext.People;
using Domain.Ports;
using Domain.Ports.Identity;

namespace Application.Service;

public class OwnerService : ApplicationService<OwnerAgg, OwnerDto>, IOwnerService
{
    public OwnerService(IOwnerRepository ownerRepository, ICurrentUserService currentUser) : base(ownerRepository, currentUser)
    {
        CreateMapperExpresion<OwnerAgg, OwnerDto>(cnf =>
        {
            OwnerMapper.Expresion(cnf);
        });
    }
}

using Application.Base;
using Application.Dto;
using Domain.BoundedContext.People;
using Domain.Ports;
using Domain.Ports.Identity;

namespace Application.Service;

public class GuestService : ApplicationService<GuestAgg, GuestDto>, IGuestService
{
    public GuestService(IGuestRepository guestRepository, ICurrentUserService currentUser) : base(guestRepository, currentUser)
    {
        CreateMapperExpresion<GuestAgg, GuestDto>(cnf =>
        {
            GuestMapper.Expresion(cnf);
        });
    }
}

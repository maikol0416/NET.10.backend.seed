using Application.Base;
using Application.Dto;
using Domain.BoundedContext.People;
using Domain.Ports;

namespace Application.Service;

public class GuestService : ApplicationService<GuestAgg, GuestDto>, IGuestService
{
    public GuestService(IGuestRepository guestRepository) : base(guestRepository)
    {
        CreateMapperExpresion<GuestAgg, GuestDto>(cnf =>
        {
            GuestMapper.Expresion(cnf);
        });
    }
}

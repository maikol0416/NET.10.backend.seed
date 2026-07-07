using Application.Base;
using Application.Dto;
using Domain.BoundedContext.People;
using Domain.Ports;

namespace Application.Service;

public class GuestReadOnlyService : ApplicationReadOnlyService<GuestAgg, GuestDto>, IGuestReadOnlyService
{
    public GuestReadOnlyService(IGuestReadOnlyRepository repository) : base(repository)
    {
        CreateMapperExpresion<GuestAgg, GuestDto>(cnf =>
        {
            GuestMapper.Expresion(cnf);
        });
    }
}

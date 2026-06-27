using Application.Base;
using Application.Dto;
using Domain.BoundedContext.People;
using Domain.Ports;

namespace Application.Service;

public class OwnerReadOnlyService : ApplicationReadOnlyService<OwnerAgg, OwnerDto>, IOwnerReadOnlyService
{
    public OwnerReadOnlyService(IOwnerReadOnlyRepository repository) : base(repository)
    {
        CreateMapperExpresion<OwnerAgg, OwnerDto>(cnf =>
        {
            OwnerMapper.Expresion(cnf);
        });
    }
}

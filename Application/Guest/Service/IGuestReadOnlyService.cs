using Application.Base;
using Application.Dto;
using Domain.BoundedContext.People;

namespace Application.Service;

public interface IGuestReadOnlyService : IApplicationReadOnlyService<GuestAgg, GuestDto>
{
}

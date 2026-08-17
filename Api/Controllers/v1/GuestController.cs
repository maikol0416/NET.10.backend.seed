using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Base;
using Application.Dto;
using Domain.BoundedContext.People;
using FluentValidation;

namespace Api.Controllers;

[Route("api/[controller]")]
public class GuestController : BaseController<GuestAgg, GuestDto>
{
    public GuestController(IValidator<GuestDto> validator, IMediator mediator)
        : base(validator, mediator)
    {
    }

    /// <summary>
    /// Listado paginado de guests filtrado por la propiedad horizontal (y, opcionalmente,
    /// el apartamento) a la que aplica alguno de sus GuestPermissions.
    /// </summary>
    [HttpGet("getPaginatedByProperty")]
    public async Task<IActionResult> GetPaginatedByProperty(
        [FromQuery] Guid physicalStructureId,
        [FromQuery] Guid? apartmentId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        Expression<Func<GuestAgg, bool>> predicate = g => g.GuestPermissions.Any(p =>
            p.PhysicalStructureId == physicalStructureId &&
            (!apartmentId.HasValue || p.ApartmentId == apartmentId));

        return this.HandlerResponse(await _mediator.Send(
            new GetPaginatedFilteredQuery<GuestAgg, GuestDto>(pageNumber, pageSize, predicate)));
    }
}

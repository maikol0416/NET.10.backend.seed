using Microsoft.AspNetCore.Mvc;
using MediatR;
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
}

using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Dto;
using Domain.BoundedContext.People;
using FluentValidation;

namespace Api.Controllers;

[Route("api/[controller]")]
public class OwnerController : BaseController<OwnerAgg, OwnerDto>
{
    public OwnerController(IValidator<OwnerDto> validator, IMediator mediator)
        : base(validator, mediator)
    {
    }
}

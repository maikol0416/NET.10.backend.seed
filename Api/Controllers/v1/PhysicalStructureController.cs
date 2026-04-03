using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.PhysicalStructure.Commands;
using Application.Dto;
using Domain.BoundedContext.Properties;
using FluentValidation;

namespace Api.Controllers;

[Route("api/[controller]")]
public class PhysicalStructureController : BaseController<PhysicalStructureAgg, PhysicalStructureDto>
{
    public PhysicalStructureController(IValidator<PhysicalStructureDto> validator,IMediator mediator)
    :base(validator,mediator)
    {
        
    }

}

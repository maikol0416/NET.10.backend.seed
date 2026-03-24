using Microsoft.AspNetCore.Mvc;
using Application.PhysicalStructure;
using MediatR;
using Application.PhysicalStructure.Commands;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PhysicalStructureController : ControllerBase
{
    private IMediator _mediator;
    public PhysicalStructureController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(PhysicalStructureDto dto)
    {
        var command = new CreatePhysicalStructureCommand(dto);
        var structures = await _mediator.Send(command);
        return Ok(structures);
    }


}

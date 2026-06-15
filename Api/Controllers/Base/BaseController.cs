using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using MediatR;
using System.Text.Json;
using Application.Base;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

[Authorize]
[ApiController]
public abstract partial class BaseController<ENT, DTO> : ControllerBase
    where ENT : class, new()
    where DTO : class, new()
{
    protected readonly IValidator<DTO> _validator;
    protected readonly IMediator _mediator;

    public BaseController(IValidator<DTO> validator, IMediator mediator)
    {
        _mediator = mediator;
        _validator = validator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(DTO dto)
    {
        var validate = await _validator.ValidateAsync(dto);
        if (validate.Errors.Count > 0)
        {
            throw new Util.Ex.DomainException( JsonSerializer.Serialize( validate.Errors));
        }
        return this.HandlerResponse(await _mediator.Send(new CreateCommand<ENT, DTO>(dto)));
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(DTO dto)
    {
        var validate = await _validator.ValidateAsync(dto);
        if (validate.Errors.Count > 0)
        {
            throw new Util.Ex.DomainException(JsonSerializer.Serialize(validate.Errors));
        }
        return this.HandlerResponse(await _mediator.Send(new UpdateCommand<ENT, DTO>(dto)));
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(Guid code)
    {
        return this.HandlerResponse(await _mediator.Send(new DeleteCommand<ENT, DTO>(code)));
    }

    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll()
    {
        return this.HandlerResponse(await _mediator.Send(new GetAllQuery<ENT, DTO>()));
    }

    [HttpGet("getById")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return this.HandlerResponse( await _mediator.Send(new GetByIdQuery<ENT, DTO>(id)));
    }

    [HttpGet("getPaginated")]
    public async Task<IActionResult> GetPaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        return this.HandlerResponse(await _mediator.Send(new GetPaginatedQuery<ENT, DTO>(pageNumber, pageSize)));
    }

    protected IActionResult HandlerResponse<DTO>(DTO dato)
    {
        return Ok(new ResponseApi<DTO> { Data = dato, Status = true, Message = "Operation carried out successfully." });
    }
}
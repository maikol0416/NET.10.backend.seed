using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Dto;
using Domain.BoundedContext.DocumentManagement;
using FluentValidation;

namespace Api.Controllers;

[Route("api/[controller]")]
public class DocumentController : BaseController<DocumentAgg, DocumentDto>
{
    public DocumentController(
        IValidator<DocumentDto> validator,
        IMediator mediator)
        : base(validator, mediator)
    {
    }
}

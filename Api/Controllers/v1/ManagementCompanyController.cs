using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Dto;
using Domain.BoundedContext.Tenancy;
using FluentValidation;

namespace Api.Controllers;

[Route("api/[controller]")]
public class ManagementCompanyController
    : BaseController<ManagementCompanyAgg, ManagementCompanyDto>
{
    public ManagementCompanyController(
        IValidator<ManagementCompanyDto> validator,
        IMediator mediator)
        : base(validator, mediator)
    {
    }
}

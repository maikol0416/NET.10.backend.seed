using Application.Dto;
using Domain.Ports;
using FluentValidation;

namespace Application.Validator;

public class ManagementCompanyValidator : AbstractValidator<ManagementCompanyDto>
{
    private readonly IManagementCompanyRepository _managementCompanyRepository;

    public ManagementCompanyValidator(IManagementCompanyRepository managementCompanyRepository)
    {
        _managementCompanyRepository = managementCompanyRepository;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode("NameEmpty")
            .WithMessage("El nombre es obligatorio.")
            .WithName(nameof(ManagementCompanyDto.Name));

        RuleFor(x => x.Nit)
            .NotEmpty()
            .WithErrorCode("NitEmpty")
            .WithMessage("El Nit es obligatorio.")
            .WithName(nameof(ManagementCompanyDto.Nit));

        RuleFor(x => x.ContactEmail)
            .NotEmpty()
            .WithErrorCode("ContactEmailEmpty")
            .WithMessage("El ContactEmail es obligatorio.")
            .WithName(nameof(ManagementCompanyDto.ContactEmail));

        RuleFor(x => x.ContactPhone)
            .NotEmpty()
            .WithErrorCode("ContactPhoneEmpty")
            .WithMessage("El ContactPhone es obligatorio.")
            .WithName(nameof(ManagementCompanyDto.ContactPhone));
    }
}

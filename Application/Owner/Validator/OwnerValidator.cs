using Application.Dto;
using Domain.Ports;
using FluentValidation;

namespace Application.Validator;

public class OwnerValidator : AbstractValidator<OwnerDto>
{
    private readonly IOwnerRepository _ownerRepository;

    public OwnerValidator(IOwnerRepository ownerRepository)
    {
        _ownerRepository = ownerRepository;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode("NameEmpty")
            .WithMessage("El nombre no puede ser vacío o nulo.")
            .WithName(nameof(OwnerDto.Name));

        RuleFor(x => x.Name)
            .MaximumLength(150)
            .WithErrorCode("NameMaxLength")
            .WithMessage("El nombre no puede exceder 150 caracteres.")
            .WithName(nameof(OwnerDto.Name));

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithErrorCode("LastNameEmpty")
            .WithMessage("El apellido no puede ser vacío o nulo.")
            .WithName(nameof(OwnerDto.LastName));

        RuleFor(x => x.LastName)
            .MaximumLength(150)
            .WithErrorCode("LastNameMaxLength")
            .WithMessage("El apellido no puede exceder 150 caracteres.")
            .WithName(nameof(OwnerDto.LastName));

        RuleFor(x => x.DocumentNumber)
            .NotEmpty()
            .WithErrorCode("DocumentNumberEmpty")
            .WithMessage("El número de documento no puede ser vacío o nulo.")
            .WithName(nameof(OwnerDto.DocumentNumber));

        RuleFor(x => x.DocumentNumber)
            .MaximumLength(20)
            .WithErrorCode("DocumentNumberMaxLength")
            .WithMessage("El número de documento no puede exceder 20 caracteres.")
            .WithName(nameof(OwnerDto.DocumentNumber));

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithErrorCode("EmailEmpty")
            .WithMessage("El correo no puede ser vacío o nulo.")
            .WithName(nameof(OwnerDto.Email));

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithErrorCode("EmailInvalidFormat")
            .WithMessage("El correo no cuenta con el formato correcto.")
            .WithName(nameof(OwnerDto.Email));

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(15)
            .WithErrorCode("PhoneNumberMaxLength")
            .WithMessage("El número de teléfono no puede exceder 15 caracteres.")
            .WithName(nameof(OwnerDto.PhoneNumber));

        RuleFor(x => x.ResponseTermsAndCondition)
            .MaximumLength(500)
            .WithErrorCode("ResponseTermsAndConditionMaxLength")
            .WithMessage("La respuesta de términos y condiciones no puede exceder 500 caracteres.")
            .WithName(nameof(OwnerDto.ResponseTermsAndCondition));

        RuleFor(x => x.MediaId)
            .MaximumLength(100)
            .WithErrorCode("MediaIdMaxLength")
            .WithMessage("El MediaId no puede exceder 100 caracteres.")
            .WithName(nameof(OwnerDto.MediaId));
    }
}

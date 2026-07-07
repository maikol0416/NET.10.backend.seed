using Application.Dto;
using Domain.Ports;
using FluentValidation;

namespace Application.Validator;

public class GuestValidator : AbstractValidator<GuestDto>
{
    private readonly IGuestRepository _guestRepository;

    public GuestValidator(IGuestRepository guestRepository)
    {
        _guestRepository = guestRepository;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode("NameEmpty")
            .WithMessage("El nombre no puede ser vacío o nulo.")
            .WithName(nameof(GuestDto.Name));

        RuleFor(x => x.Name)
            .MaximumLength(150)
            .WithErrorCode("NameMaxLength")
            .WithMessage("El nombre no puede exceder 150 caracteres.")
            .WithName(nameof(GuestDto.Name));

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithErrorCode("LastNameEmpty")
            .WithMessage("El apellido no puede ser vacío o nulo.")
            .WithName(nameof(GuestDto.LastName));

        RuleFor(x => x.LastName)
            .MaximumLength(150)
            .WithErrorCode("LastNameMaxLength")
            .WithMessage("El apellido no puede exceder 150 caracteres.")
            .WithName(nameof(GuestDto.LastName));

        RuleFor(x => x.DocumentNumber)
            .NotEmpty()
            .WithErrorCode("DocumentNumberEmpty")
            .WithMessage("El número de documento no puede ser vacío o nulo.")
            .WithName(nameof(GuestDto.DocumentNumber));

        RuleFor(x => x.DocumentNumber)
            .MaximumLength(20)
            .WithErrorCode("DocumentNumberMaxLength")
            .WithMessage("El número de documento no puede exceder 20 caracteres.")
            .WithName(nameof(GuestDto.DocumentNumber));

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithErrorCode("PhoneNumberEmpty")
            .WithMessage("El número de teléfono no puede ser vacío o nulo.")
            .WithName(nameof(GuestDto.PhoneNumber));

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(15)
            .WithErrorCode("PhoneNumberMaxLength")
            .WithMessage("El número de teléfono no puede exceder 15 caracteres.")
            .WithName(nameof(GuestDto.PhoneNumber));

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithErrorCode("EmailEmpty")
            .WithMessage("El correo no puede ser vacío o nulo.")
            .WithName(nameof(GuestDto.Email));

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithErrorCode("EmailInvalidFormat")
            .WithMessage("El correo no cuenta con el formato correcto.")
            .WithName(nameof(GuestDto.Email));

        RuleFor(x => x.TermsAndCondition)
            .NotEmpty()
            .WithErrorCode("TermsAndConditionEmpty")
            .WithMessage("Los términos y condiciones no pueden ser vacíos o nulos.")
            .WithName(nameof(GuestDto.TermsAndCondition));

        RuleFor(x => x.TermsAndCondition)
            .MaximumLength(500)
            .WithErrorCode("TermsAndConditionMaxLength")
            .WithMessage("Los términos y condiciones no pueden exceder 500 caracteres.")
            .WithName(nameof(GuestDto.TermsAndCondition));

        RuleFor(x => x.ResponseTermsAndCondition)
            .NotEmpty()
            .WithErrorCode("ResponseTermsAndConditionEmpty")
            .WithMessage("La respuesta de términos y condiciones no puede ser vacía o nula.")
            .WithName(nameof(GuestDto.ResponseTermsAndCondition));

        RuleFor(x => x.ResponseTermsAndCondition)
            .MaximumLength(500)
            .WithErrorCode("ResponseTermsAndConditionMaxLength")
            .WithMessage("La respuesta de términos y condiciones no puede exceder 500 caracteres.")
            .WithName(nameof(GuestDto.ResponseTermsAndCondition));

        RuleFor(x => x.MediaId)
            .NotEmpty()
            .WithErrorCode("MediaIdEmpty")
            .WithMessage("El MediaId no puede ser vacío o nulo.")
            .WithName(nameof(GuestDto.MediaId));

        RuleFor(x => x.MediaId)
            .MaximumLength(100)
            .WithErrorCode("MediaIdMaxLength")
            .WithMessage("El MediaId no puede exceder 100 caracteres.")
            .WithName(nameof(GuestDto.MediaId));

        RuleForEach(x => x.GuestPermissions)
            .ChildRules(permission =>
            {
                permission.RuleFor(p => p.EndDate)
                    .GreaterThan(p => p.StartDate)
                    .WithErrorCode("GuestPermissionDateRangeInvalid")
                    .WithMessage("La fecha de inicio debe ser menor a la fecha de fin.")
                    .WithName(nameof(GuestPermissionDto.EndDate));
            });
    }
}

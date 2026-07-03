using Application.Auth.Dtos;
using FluentValidation;

namespace Application.Auth.Validator;

/// <summary>
/// Validator para el DTO de actualización de usuario.
/// Valida Id, formato del email y nombre completo.
/// </summary>
public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithErrorCode("IdEmpty")
            .WithMessage("El Id del usuario es obligatorio.")
            .WithName(nameof(UpdateUserDto.Id));

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithErrorCode("EmailEmpty")
            .WithMessage("El campo Email es obligatorio.")
            .WithName(nameof(UpdateUserDto.Email))
            .EmailAddress()
            .WithErrorCode("EmailInvalid")
            .WithMessage("El formato del email no es válido.")
            .WithName(nameof(UpdateUserDto.Email));

        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithErrorCode("FullNameEmpty")
            .WithMessage("El campo Nombre Completo es obligatorio.")
            .WithName(nameof(UpdateUserDto.FullName))
            .MaximumLength(200)
            .WithErrorCode("FullNameTooLong")
            .WithMessage("El Nombre Completo no puede exceder los 200 caracteres.")
            .WithName(nameof(UpdateUserDto.FullName));
    }
}

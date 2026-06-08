using Application.Auth.Dtos;
using FluentValidation;

namespace Application.Auth.Validator;

/// <summary>
/// Validator para el DTO de login.
/// Valida formato del email y que la contraseña no esté vacía.
/// </summary>
public class LoginValidator : AbstractValidator<AuthLoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithErrorCode("EmailEmpty")
            .WithMessage("El campo Email es obligatorio.")
            .WithName(nameof(AuthLoginDto.Email))
            .EmailAddress()
            .WithErrorCode("EmailInvalid")
            .WithMessage("El formato del email no es válido.")
            .WithName(nameof(AuthLoginDto.Email));

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithErrorCode("PasswordEmpty")
            .WithMessage("El campo Password es obligatorio.")
            .WithName(nameof(AuthLoginDto.Password));
    }
}

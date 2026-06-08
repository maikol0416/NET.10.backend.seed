using Application.Auth.Dtos;
using FluentValidation;

namespace Application.Auth.Validator;

/// <summary>
/// Validator para el DTO de registro.
/// Valida email, contraseña con longitud mínima, y nombre completo.
/// </summary>
public class RegisterValidator : AbstractValidator<AuthRegisterDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithErrorCode("EmailEmpty")
            .WithMessage("El campo Email es obligatorio.")
            .WithName(nameof(AuthRegisterDto.Email))
            .EmailAddress()
            .WithErrorCode("EmailInvalid")
            .WithMessage("El formato del email no es válido.")
            .WithName(nameof(AuthRegisterDto.Email));

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithErrorCode("PasswordEmpty")
            .WithMessage("El campo Password es obligatorio.")
            .WithName(nameof(AuthRegisterDto.Password))
            .MinimumLength(6)
            .WithErrorCode("PasswordTooShort")
            .WithMessage("La contraseña debe tener al menos 6 caracteres.")
            .WithName(nameof(AuthRegisterDto.Password));

        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithErrorCode("FullNameEmpty")
            .WithMessage("El campo Nombre Completo es obligatorio.")
            .WithName(nameof(AuthRegisterDto.FullName))
            .MaximumLength(200)
            .WithErrorCode("FullNameTooLong")
            .WithMessage("El Nombre Completo no puede exceder los 200 caracteres.")
            .WithName(nameof(AuthRegisterDto.FullName));

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithErrorCode("RoleEmpty")
            .WithMessage("El campo Rol es obligatorio.")
            .WithName(nameof(AuthRegisterDto.Role))
            .MaximumLength(50)
            .WithErrorCode("RoleTooLong")
            .WithMessage("El nombre del rol no puede exceder los 50 caracteres.")
            .WithName(nameof(AuthRegisterDto.Role));
    }
}

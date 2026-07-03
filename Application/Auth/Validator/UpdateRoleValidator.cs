using Application.Auth.Dtos;
using FluentValidation;

namespace Application.Auth.Validator;

/// <summary>
/// Validator para el DTO de actualización (renombrado) de rol.
/// </summary>
public class UpdateRoleValidator : AbstractValidator<UpdateRoleDto>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithErrorCode("IdEmpty")
            .WithMessage("El Id del rol es obligatorio.")
            .WithName(nameof(UpdateRoleDto.Id));

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode("NameEmpty")
            .WithMessage("El nombre del rol es obligatorio.")
            .WithName(nameof(UpdateRoleDto.Name))
            .MaximumLength(50)
            .WithErrorCode("NameTooLong")
            .WithMessage("El nombre del rol no puede exceder los 50 caracteres.")
            .WithName(nameof(UpdateRoleDto.Name));
    }
}

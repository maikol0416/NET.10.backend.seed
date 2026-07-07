using Application.Auth.Dtos;
using FluentValidation;

namespace Application.Auth.Validator;

/// <summary>
/// Validator para el DTO de creación de roles.
/// Valida que el nombre del rol no esté vacío y tenga longitud razonable.
/// </summary>
public class CreateRoleValidator : AbstractValidator<CreateRoleDto>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.RoleName)
            .NotEmpty()
            .WithErrorCode("RoleNameEmpty")
            .WithMessage("El nombre del rol es obligatorio.")
            .WithName(nameof(CreateRoleDto.RoleName))
            .MaximumLength(50)
            .WithErrorCode("RoleNameTooLong")
            .WithMessage("El nombre del rol no puede exceder los 50 caracteres.")
            .WithName(nameof(CreateRoleDto.RoleName));

        RuleForEach(x => x.Permissions)
            .Must(ModulePermissionsValidation.IsValidModuleName)
            .WithErrorCode("InvalidModuleName")
            .WithMessage("El módulo indicado no es válido.")
            .When(x => x.Permissions != null);
    }
}

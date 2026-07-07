using Application.Auth.Dtos;
using FluentValidation;

namespace Application.Auth.Validator;

/// <summary>
/// Validator para el DTO de asignación (reemplazo) de permisos de un rol.
/// </summary>
public class AssignRolePermissionsValidator : AbstractValidator<AssignRolePermissionsDto>
{
    public AssignRolePermissionsValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithErrorCode("RoleIdEmpty")
            .WithMessage("El Id del rol es obligatorio.")
            .WithName(nameof(AssignRolePermissionsDto.RoleId));

        RuleForEach(x => x.Permissions)
            .Must(ModulePermissionsValidation.IsValidModuleName)
            .WithErrorCode("InvalidModuleName")
            .WithMessage("El módulo indicado no es válido.");
    }
}

using Domain.DomainShared;

namespace Application.Auth.Validator;

/// <summary>
/// Regla de validación compartida entre CreateRoleValidator y
/// AssignRolePermissionsValidator: un nombre de módulo debe existir en ModuleEnum.
/// </summary>
internal static class ModulePermissionsValidation
{
    public static bool IsValidModuleName(string name) =>
        Enum.TryParse<ModuleEnum>(name, ignoreCase: false, out _);
}

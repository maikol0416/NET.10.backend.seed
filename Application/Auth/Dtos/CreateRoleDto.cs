namespace Application.Auth.Dtos;

/// <summary>
/// DTO de entrada para el endpoint de creación de roles.
/// </summary>
public class CreateRoleDto
{
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// Lista opcional de módulos a asignar al rol en el mismo momento de crearlo.
    /// Si viene vacía o null, el rol se crea sin permisos (se pueden asignar después).
    /// </summary>
    public List<string>? Permissions { get; set; }
}

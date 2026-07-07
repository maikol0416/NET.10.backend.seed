namespace Application.Auth.Dtos;

/// <summary>
/// DTO de entrada para reemplazar la lista completa de permisos (módulos) de un rol.
/// </summary>
public class AssignRolePermissionsDto
{
    public string RoleId { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = [];
}

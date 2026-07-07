namespace Application.Auth.Dtos;

/// <summary>
/// Desglose de los módulos permitidos para un rol puntual del usuario.
/// Se usa dentro de UserDto y AuthResponseDto para que el front deje
/// elegir con cuál de sus roles quiere trabajar.
/// </summary>
public class RolePermissionsDto
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = [];
}

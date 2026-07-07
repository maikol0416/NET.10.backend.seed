namespace Application.Auth.Dtos;

/// <summary>
/// DTO de salida para el listado paginado de usuarios. Nunca expone
/// el hash de contraseña ni otros detalles internos de Identity.
/// </summary>
public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];

    /// <summary>
    /// Desglose de módulos permitidos por cada rol del usuario (un rol puede tener
    /// varios). El front usa esto para dejar elegir con cuál rol quiere trabajar.
    /// </summary>
    public List<RolePermissionsDto> RolePermissions { get; set; } = [];
}

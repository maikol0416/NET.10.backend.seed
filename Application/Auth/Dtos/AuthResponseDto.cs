namespace Application.Auth.Dtos;

/// <summary>
/// DTO de respuesta para los endpoints de autenticación.
/// Contiene el token JWT y datos básicos del usuario.
/// </summary>
public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
    public string Role => Roles.FirstOrDefault() ?? string.Empty;

    /// <summary>Empresa administradora del usuario autenticado. Null para usuarios de plataforma.</summary>
    public Guid? CompanyId { get; set; }

    /// <summary>
    /// Desglose de módulos permitidos por cada rol del usuario. El front lo usa
    /// justo después del login/registro para dejar elegir con cuál rol trabajar,
    /// sin necesidad de una llamada adicional.
    /// </summary>
    public List<RolePermissionsDto> RolePermissions { get; set; } = [];
}

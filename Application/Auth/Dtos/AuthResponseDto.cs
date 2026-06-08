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
}

namespace Infraestructure.Identity;

/// <summary>
/// POCO para binding de la sección "Jwt" de appsettings.json.
/// Contiene la configuración necesaria para generar y validar tokens JWT.
/// </summary>
public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
}

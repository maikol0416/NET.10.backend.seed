using Microsoft.AspNetCore.Identity;

namespace Infraestructure.Identity;

/// <summary>
/// Extensión de IdentityUser con propiedades adicionales del negocio.
/// Vive en Infraestructura porque depende directamente de ASP.NET Core Identity.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}

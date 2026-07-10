using Microsoft.AspNetCore.Identity;

namespace Infraestructure.Identity;

/// <summary>
/// Extensión de IdentityUser con propiedades adicionales del negocio.
/// Vive en Infraestructura porque depende directamente de ASP.NET Core Identity.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Empresa administradora (tenant) a la que pertenece el usuario.
    /// Null para los usuarios de plataforma (rol Administrator), que no pertenecen
    /// a ninguna empresa y pueden operar sobre todas.
    /// </summary>
    public Guid? CompanyId { get; set; }
}

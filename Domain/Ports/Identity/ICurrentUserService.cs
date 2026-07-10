namespace Domain.Ports.Identity;

/// <summary>
/// Puerto del dominio para conocer al usuario autenticado de la petición actual:
/// si está autenticado, a qué empresa administradora (tenant) pertenece y si es
/// un usuario de plataforma (rol Administrator, sin empresa, ve todas).
/// La infraestructura lo implementa leyendo los claims del JWT vía IHttpContextAccessor.
/// </summary>
public interface ICurrentUserService
{
    bool IsAuthenticated { get; }
    Guid? CompanyId { get; }
    bool IsPlatformAdministrator { get; }
}

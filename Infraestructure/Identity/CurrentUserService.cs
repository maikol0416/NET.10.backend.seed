using System.Security.Claims;
using Domain.DomainShared;
using Domain.Ports.Identity;
using Microsoft.AspNetCore.Http;

namespace Infraestructure.Identity;

/// <summary>
/// Implementación del puerto ICurrentUserService usando IHttpContextAccessor.
/// Lee el CompanyId y el rol Administrator directamente de los claims del JWT
/// ya validado por el middleware de autenticación — sin ir a base de datos.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private const string CompanyIdClaimType = "CompanyId";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public Guid? CompanyId =>
        Guid.TryParse(User?.FindFirstValue(CompanyIdClaimType), out var companyId) ? companyId : null;

    public bool IsPlatformAdministrator =>
        User != null && User.IsInRole(RolePermissionsPolicy.AdministratorRoleName);

    public bool IsCompanyAdministrator =>
        User != null && User.IsInRole(RolePermissionsPolicy.CompanyAdministratorRoleName);
}

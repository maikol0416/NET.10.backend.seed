using Application.Auth.Dtos;
using Domain.DomainShared;
using Domain.Ports.Identity;
using MediatR;

namespace Application.Auth.Cqrs.Queries;

/// <summary>
/// Query para listar TODOS los usuarios (sin paginar) que tienen un rol puntual —
/// ej. "los Property Administrator de mi empresa". Distinto de GetUsersPaginatedQuery,
/// que lista el catálogo completo de usuarios página por página.
/// Despachado por MediatR hacia el GetUsersByRoleQueryHandler.
/// </summary>
public record GetUsersByRoleQuery(string Role, Guid? CompanyId) : IRequest<List<UserDto>>;

public class GetUsersByRoleQueryHandler : IRequestHandler<GetUsersByRoleQuery, List<UserDto>>
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUser;

    public GetUsersByRoleQueryHandler(IAuthService authService, ICurrentUserService currentUser)
    {
        _authService = authService;
        _currentUser = currentUser;
    }

    public async Task<List<UserDto>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Role))
        {
            throw new DomainException("Debes indicar el rol a consultar.");
        }

        // Administrator (plataforma) puede indicar cualquier companyId (o ninguno, para ver todas).
        // Company Administrator queda siempre acotado a su propia empresa — se ignora
        // cualquier companyId que mande en la query.
        var effectiveCompanyId = request.CompanyId;
        if (!_currentUser.IsPlatformAdministrator)
        {
            effectiveCompanyId = _currentUser.CompanyId
                ?? throw new DomainException("Tu usuario no pertenece a ninguna empresa; no puedes consultar usuarios.");
        }

        var users = await _authService.GetUsersByRoleAsync(request.Role, effectiveCompanyId);

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            FullName = u.FullName,
            Roles = u.Roles.ToList(),
            RolePermissions = u.RolePermissions.Select(rp => new RolePermissionsDto
            {
                RoleId = rp.RoleId,
                RoleName = rp.RoleName,
                Permissions = rp.Permissions.Select(p => p.ToString()).ToList()
            }).ToList(),
            CompanyId = u.CompanyId
        }).ToList();
    }
}

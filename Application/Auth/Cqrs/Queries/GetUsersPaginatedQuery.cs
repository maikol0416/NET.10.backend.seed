using Application.Auth.Dtos;
using Domain.DomainShared;
using Domain.Ports.Identity;
using MediatR;

namespace Application.Auth.Cqrs.Queries;

/// <summary>
/// Query para listar usuarios de forma paginada.
/// Despachado por MediatR hacia el GetUsersPaginatedQueryHandler.
/// </summary>
public record GetUsersPaginatedQuery(int PageNumber, int PageSize) : IRequest<PaginatedList<UserDto>>;

public class GetUsersPaginatedQueryHandler : IRequestHandler<GetUsersPaginatedQuery, PaginatedList<UserDto>>
{
    private readonly IAuthService _authService;

    public GetUsersPaginatedQueryHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<PaginatedList<UserDto>> Handle(GetUsersPaginatedQuery request, CancellationToken cancellationToken)
    {
        var result = await _authService.GetUsersPaginatedAsync(request.PageNumber, request.PageSize);

        var dtos = result.Items.Select(u => new UserDto
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
            }).ToList()
        }).ToList();

        return new PaginatedList<UserDto>(dtos, result.TotalCount, result.PageNumber, result.PageSize);
    }
}

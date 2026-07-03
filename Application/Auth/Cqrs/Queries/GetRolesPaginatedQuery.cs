using Application.Auth.Dtos;
using Domain.DomainShared;
using Domain.Ports.Identity;
using MediatR;

namespace Application.Auth.Cqrs.Queries;

/// <summary>
/// Query para listar roles de forma paginada.
/// Despachado por MediatR hacia el GetRolesPaginatedQueryHandler.
/// </summary>
public record GetRolesPaginatedQuery(int PageNumber, int PageSize) : IRequest<PaginatedList<RoleDto>>;

public class GetRolesPaginatedQueryHandler : IRequestHandler<GetRolesPaginatedQuery, PaginatedList<RoleDto>>
{
    private readonly IAuthService _authService;

    public GetRolesPaginatedQueryHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<PaginatedList<RoleDto>> Handle(GetRolesPaginatedQuery request, CancellationToken cancellationToken)
    {
        var result = await _authService.GetRolesPaginatedAsync(request.PageNumber, request.PageSize);

        var dtos = result.Items.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name
        }).ToList();

        return new PaginatedList<RoleDto>(dtos, result.TotalCount, result.PageNumber, result.PageSize);
    }
}

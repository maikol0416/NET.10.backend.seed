using Application.Auth.Dtos;
using Domain.DomainShared;
using Domain.Ports.Identity;
using MediatR;

namespace Application.Auth.Cqrs.Commands;

/// <summary>
/// Comando para reemplazar la lista completa de permisos (módulos) de un rol.
/// Despachado por MediatR hacia el AssignRolePermissionsCommandHandler.
/// </summary>
public record AssignRolePermissionsCommand(AssignRolePermissionsDto PermissionsDto) : IRequest<bool>;

public class AssignRolePermissionsCommandHandler : IRequestHandler<AssignRolePermissionsCommand, bool>
{
    private readonly IAuthService _authService;

    public AssignRolePermissionsCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<bool> Handle(AssignRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        var permissions = request.PermissionsDto.Permissions
            .Select(Enum.Parse<ModuleEnum>)
            .ToList();

        var result = await _authService.AssignPermissionsToRoleAsync(request.PermissionsDto.RoleId, permissions);

        if (!result.Success)
        {
            throw new DomainException(
                string.Join(" | ", result.Errors ?? ["Error al asignar permisos al rol."])
            );
        }

        return true;
    }
}

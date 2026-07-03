using Application.Auth.Dtos;
using Domain.DomainShared;
using Domain.Ports.Identity;
using MediatR;

namespace Application.Auth.Cqrs.Commands;

/// <summary>
/// Comando para renombrar un rol existente.
/// Despachado por MediatR hacia el UpdateRoleCommandHandler.
/// </summary>
public record UpdateRoleCommand(UpdateRoleDto RoleDto) : IRequest<bool>;

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, bool>
{
    private readonly IAuthService _authService;

    public UpdateRoleCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<bool> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.UpdateRoleAsync(request.RoleDto.Id, request.RoleDto.Name);

        if (!result.Success)
        {
            throw new DomainException(
                string.Join(" | ", result.Errors ?? ["Error al actualizar el rol."])
            );
        }

        return true;
    }
}

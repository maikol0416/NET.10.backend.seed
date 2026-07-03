using Domain.DomainShared;
using Domain.Ports.Identity;
using MediatR;

namespace Application.Auth.Cqrs.Commands;

/// <summary>
/// Comando para eliminar un rol existente.
/// Despachado por MediatR hacia el DeleteRoleCommandHandler.
/// </summary>
public record DeleteRoleCommand(string RoleId) : IRequest<bool>;

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, bool>
{
    private readonly IAuthService _authService;

    public DeleteRoleCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.DeleteRoleAsync(request.RoleId);

        if (!result.Success)
        {
            throw new DomainException(
                string.Join(" | ", result.Errors ?? ["Error al eliminar el rol."])
            );
        }

        return true;
    }
}

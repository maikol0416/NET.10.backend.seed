using Domain.DomainShared;
using Domain.Ports.Identity;
using MediatR;

namespace Application.Auth.Cqrs.Commands;

/// <summary>
/// Comando para eliminar un usuario existente.
/// RequestingUserId es el usuario autenticado que ejecuta la operación —
/// se usa para bloquear la auto-eliminación.
/// Despachado por MediatR hacia el DeleteUserCommandHandler.
/// </summary>
public record DeleteUserCommand(string UserId, string RequestingUserId) : IRequest<bool>;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IAuthService _authService;

    public DeleteUserCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.DeleteUserAsync(request.UserId, request.RequestingUserId);

        if (!result.Success)
        {
            throw new DomainException(
                string.Join(" | ", result.Errors ?? ["Error al eliminar el usuario."])
            );
        }

        return true;
    }
}

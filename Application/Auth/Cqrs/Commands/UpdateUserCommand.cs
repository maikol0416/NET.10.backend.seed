using Application.Auth.Dtos;
using Domain.DomainShared;
using Domain.Ports.Identity;
using MediatR;

namespace Application.Auth.Cqrs.Commands;

/// <summary>
/// Comando para actualizar un usuario existente (email, nombre y roles).
/// Despachado por MediatR hacia el UpdateUserCommandHandler.
/// </summary>
public record UpdateUserCommand(UpdateUserDto UserDto) : IRequest<bool>;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
{
    private readonly IAuthService _authService;

    public UpdateUserCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.UpdateUserAsync(
            request.UserDto.Id,
            request.UserDto.Email,
            request.UserDto.FullName,
            request.UserDto.Roles);

        if (!result.Success)
        {
            throw new DomainException(
                string.Join(" | ", result.Errors ?? ["Error al actualizar el usuario."])
            );
        }

        return true;
    }
}

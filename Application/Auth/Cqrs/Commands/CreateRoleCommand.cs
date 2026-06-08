using Application.Auth.Dtos;
using Domain.DomainShared;
using Domain.Ports.Identity;
using MediatR;

namespace Application.Auth.Cqrs.Commands;

/// <summary>
/// Comando para crear un nuevo rol en el sistema.
/// Despachado por MediatR hacia el CreateRoleCommandHandler.
/// </summary>
public record CreateRoleCommand(CreateRoleDto RoleDto) : IRequest<bool>;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, bool>
{
    private readonly IAuthService _authService;

    public CreateRoleCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<bool> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.CreateRoleAsync(request.RoleDto.RoleName);

        if (!result.Success)
        {
            throw new DomainException(
                string.Join(" | ", result.Errors ?? ["Error al crear el rol."])
            );
        }

        return true;
    }
}

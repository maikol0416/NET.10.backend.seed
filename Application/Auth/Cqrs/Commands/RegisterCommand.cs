using Application.Auth.Dtos;
using Domain.DomainShared;
using Domain.Ports.Identity;
using MediatR;

namespace Application.Auth.Cqrs.Commands;

/// <summary>
/// Comando para registrar un nuevo usuario.
/// Despachado por MediatR hacia el RegisterCommandHandler.
/// </summary>
public record RegisterCommand(AuthRegisterDto RegisterDto) : IRequest<AuthResponseDto>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;

    public RegisterCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(
            request.RegisterDto.Email,
            request.RegisterDto.Password,
            request.RegisterDto.FullName,
            request.RegisterDto.Role
        );

        if (!result.Success)
        {
            throw new DomainException(
                string.Join(" | ", result.Errors ?? ["Error al registrar el usuario."])
            );
        }

        return new AuthResponseDto
        {
            Token = result.Token!,
            Email = result.Email!,
            FullName = result.FullName!,
            Expiration = result.Expiration!.Value
        };
    }
}

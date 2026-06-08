using Application.Auth.Dtos;
using Domain.DomainShared;
using Domain.Ports.Identity;
using MediatR;

namespace Application.Auth.Cqrs.Commands;

/// <summary>
/// Comando para iniciar sesión.
/// Despachado por MediatR hacia el LoginCommandHandler.
/// </summary>
public record LoginCommand(AuthLoginDto LoginDto) : IRequest<AuthResponseDto>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(
            request.LoginDto.Email,
            request.LoginDto.Password
        );

        if (!result.Success)
        {
            throw new DomainException(
                string.Join(" | ", result.Errors ?? ["Error de autenticación."])
            );
        }

        return new AuthResponseDto
        {
            Token = result.Token!,
            Email = result.Email!,
            FullName = result.FullName!,
            Expiration = result.Expiration!.Value,
            Roles = result.Roles?.ToList() ?? new List<string>()
        };
    }
}

using Application.Auth.Dtos;
using Domain.DomainShared;
using Domain.Ports;
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
    private readonly ICurrentUserService _currentUser;
    private readonly IManagementCompanyReadOnlyRepository _managementCompanyReadOnlyRepository;

    public RegisterCommandHandler(
        IAuthService authService,
        ICurrentUserService currentUser,
        IManagementCompanyReadOnlyRepository managementCompanyReadOnlyRepository)
    {
        _authService = authService;
        _currentUser = currentUser;
        _managementCompanyReadOnlyRepository = managementCompanyReadOnlyRepository;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var dto = request.RegisterDto;
        var targetCompanyId = await ResolveTargetCompanyIdAsync(dto);

        var result = await _authService.RegisterAsync(
            dto.Email,
            dto.Password,
            dto.FullName,
            dto.Role,
            targetCompanyId
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
            Expiration = result.Expiration!.Value,
            Roles = result.Roles?.ToList() ?? new List<string>(),
            RolePermissions = (result.RolePermissions ?? []).Select(rp => new RolePermissionsDto
            {
                RoleId = rp.RoleId,
                RoleName = rp.RoleName,
                Permissions = rp.Permissions.Select(p => p.ToString()).ToList()
            }).ToList(),
            CompanyId = result.CompanyId
        };
    }

    /// <summary>
    /// Resuelve a qué empresa (CompanyId) queda atado el usuario a registrar.
    /// Nunca confía en el CompanyId del DTO salvo cuando quien registra es un
    /// Administrator de plataforma — cualquier otro usuario hereda su propia empresa.
    /// </summary>
    private async Task<Guid?> ResolveTargetCompanyIdAsync(AuthRegisterDto dto)
    {
        var requestsAdministratorRole = RolePermissionsPolicy.IsAdministrator(dto.Role);

        // Bootstrap: la plataforma no tiene todavía ningún Administrator — se permite
        // el registro anónimo del primero, sin empresa.
        if (requestsAdministratorRole && !await _authService.AnyAdministratorExistsAsync())
        {
            return null;
        }

        if (requestsAdministratorRole)
        {
            if (!_currentUser.IsPlatformAdministrator)
            {
                throw new DomainException("Solo un administrador de la plataforma puede crear otro administrador.");
            }

            return null;
        }

        if (_currentUser.IsPlatformAdministrator)
        {
            var companyId = dto.CompanyId
                ?? throw new DomainException("Debes indicar la empresa (CompanyId) para el nuevo usuario.");

            if (await _managementCompanyReadOnlyRepository.GetByIdAsync(companyId) is null)
            {
                throw new DomainException("La empresa indicada no existe.");
            }

            return companyId;
        }

        if (_currentUser.IsAuthenticated)
        {
            return _currentUser.CompanyId
                ?? throw new DomainException("Tu usuario no pertenece a ninguna empresa; no puedes registrar usuarios.");
        }

        throw new DomainException("Debes iniciar sesión para registrar usuarios.");
    }
}

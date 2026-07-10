using Application.Auth.Dtos;
using Domain.DomainShared;
using Domain.Ports;
using Domain.Ports.Identity;
using MediatR;

namespace Application.Auth.Cqrs.Commands;

/// <summary>
/// Comando para actualizar un usuario existente (email, nombre, roles y empresa).
/// Despachado por MediatR hacia el UpdateUserCommandHandler.
/// </summary>
public record UpdateUserCommand(UpdateUserDto UserDto) : IRequest<bool>;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
{
    private readonly IAuthService _authService;
    private readonly IManagementCompanyReadOnlyRepository _managementCompanyReadOnlyRepository;

    public UpdateUserCommandHandler(
        IAuthService authService,
        IManagementCompanyReadOnlyRepository managementCompanyReadOnlyRepository)
    {
        _authService = authService;
        _managementCompanyReadOnlyRepository = managementCompanyReadOnlyRepository;
    }

    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var dto = request.UserDto;

        if (dto.CompanyId.HasValue &&
            await _managementCompanyReadOnlyRepository.GetByIdAsync(dto.CompanyId.Value) is null)
        {
            throw new DomainException("La empresa indicada no existe.");
        }

        var result = await _authService.UpdateUserAsync(
            dto.Id,
            dto.Email,
            dto.FullName,
            dto.Roles,
            dto.CompanyId);

        if (!result.Success)
        {
            throw new DomainException(
                string.Join(" | ", result.Errors ?? ["Error al actualizar el usuario."])
            );
        }

        return true;
    }
}

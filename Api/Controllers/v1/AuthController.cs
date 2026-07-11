using System.Security.Claims;
using System.Text.Json;
using Application.Auth.Cqrs.Commands;
using Application.Auth.Cqrs.Queries;
using Application.Auth.Dtos;
using Domain.DomainShared;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller de autenticación. NO hereda de BaseController porque Auth
/// no es un CRUD de agregado del dominio — tiene sus propios endpoints.
/// Login y Register son públicos; el resto requiere rol Administrator.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<AuthLoginDto> _loginValidator;
    private readonly IValidator<AuthRegisterDto> _registerValidator;
    private readonly IValidator<CreateRoleDto> _createRoleValidator;
    private readonly IValidator<UpdateUserDto> _updateUserValidator;
    private readonly IValidator<UpdateRoleDto> _updateRoleValidator;
    private readonly IValidator<AssignRolePermissionsDto> _assignRolePermissionsValidator;

    public AuthController(
        IMediator mediator,
        IValidator<AuthLoginDto> loginValidator,
        IValidator<AuthRegisterDto> registerValidator,
        IValidator<CreateRoleDto> createRoleValidator,
        IValidator<UpdateUserDto> updateUserValidator,
        IValidator<UpdateRoleDto> updateRoleValidator,
        IValidator<AssignRolePermissionsDto> assignRolePermissionsValidator)
    {
        _mediator = mediator;
        _loginValidator = loginValidator;
        _registerValidator = registerValidator;
        _createRoleValidator = createRoleValidator;
        _updateUserValidator = updateUserValidator;
        _updateRoleValidator = updateRoleValidator;
        _assignRolePermissionsValidator = assignRolePermissionsValidator;
    }

    /// <summary>
    /// Inicia sesión con email y contraseña. Retorna un JWT válido.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthLoginDto loginDto)
    {
        var validation = await _loginValidator.ValidateAsync(loginDto);
        if (validation.Errors.Count > 0)
        {
            throw new Util.Ex.DomainException(JsonSerializer.Serialize(validation.Errors));
        }

        var result = await _mediator.Send(new LoginCommand(loginDto));
        return Ok(new ResponseApi<AuthResponseDto>
        {
            Data = result,
            Status = true,
            Message = "Inicio de sesión exitoso."
        });
    }

    /// <summary>
    /// Registra un nuevo usuario y le asigna el rol indicado. Retorna un JWT válido.
    /// El endpoint queda [AllowAnonymous] únicamente para permitir el bootstrap del primer
    /// Administrator de la plataforma en una base vacía — el propio RegisterCommandHandler
    /// cierra esa puerta en cuanto existe al menos un Administrator, y desde ahí exige que
    /// quien registre esté autenticado (hereda su empresa, o la indica si es Administrator).
    /// </summary>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthRegisterDto registerDto)
    {
        var validation = await _registerValidator.ValidateAsync(registerDto);
        if (validation.Errors.Count > 0)
        {
            throw new Util.Ex.DomainException(JsonSerializer.Serialize(validation.Errors));
        }

        var result = await _mediator.Send(new RegisterCommand(registerDto));
        return Ok(new ResponseApi<AuthResponseDto>
        {
            Data = result,
            Status = true,
            Message = "Registro exitoso."
        });
    }

    /// <summary>
    /// Crea un nuevo rol en el sistema. Requiere autenticación.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("create-role")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto createRoleDto)
    {
        var validation = await _createRoleValidator.ValidateAsync(createRoleDto);
        if (validation.Errors.Count > 0)
        {
            throw new Util.Ex.DomainException(JsonSerializer.Serialize(validation.Errors));
        }

        var result = await _mediator.Send(new CreateRoleCommand(createRoleDto));
        return Ok(new ResponseApi<bool>
        {
            Data = result,
            Status = true,
            Message = "Rol creado exitosamente."
        });
    }

    /// <summary>
    /// Lista los usuarios del sistema de forma paginada. Solo administradores.
    /// </summary>
    [Authorize(Roles = "Administrator")]
    [HttpGet("users/paginated")]
    public async Task<IActionResult> GetUsersPaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetUsersPaginatedQuery(pageNumber, pageSize));
        return Ok(new ResponseApi<PaginatedList<UserDto>>
        {
            Data = result,
            Status = true,
            Message = "Operation carried out successfully."
        });
    }

    /// <summary>
    /// Lista TODOS los usuarios (sin paginar) que tienen el rol indicado — ej. los
    /// "Property Administrator" de una empresa. Solo <c>Administrator</c> (plataforma,
    /// puede indicar cualquier <paramref name="companyId"/> o ninguno para ver todas)
    /// o <c>Company Administrator</c> (siempre acotado a su propia empresa — cualquier
    /// <paramref name="companyId"/> que mande se ignora).
    /// </summary>
    [Authorize(Roles = "Administrator,Company Administrator")]
    [HttpGet("users/by-role")]
    public async Task<IActionResult> GetUsersByRole([FromQuery] string role, [FromQuery] Guid? companyId = null)
    {
        var result = await _mediator.Send(new GetUsersByRoleQuery(role, companyId));
        return Ok(new ResponseApi<List<UserDto>>
        {
            Data = result,
            Status = true,
            Message = "Operation carried out successfully."
        });
    }

    /// <summary>
    /// Lista los roles del sistema de forma paginada. Solo administradores.
    /// </summary>
    [Authorize(Roles = "Administrator")]
    [HttpGet("roles/paginated")]
    public async Task<IActionResult> GetRolesPaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetRolesPaginatedQuery(pageNumber, pageSize));
        return Ok(new ResponseApi<PaginatedList<RoleDto>>
        {
            Data = result,
            Status = true,
            Message = "Operation carried out successfully."
        });
    }

    /// <summary>
    /// Actualiza email, nombre y roles de un usuario existente. Solo administradores.
    /// No permite quitar el rol Administrator al último administrador del sistema.
    /// </summary>
    [Authorize(Roles = "Administrator")]
    [HttpPut("users/update")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
    {
        var validation = await _updateUserValidator.ValidateAsync(updateUserDto);
        if (validation.Errors.Count > 0)
        {
            throw new Util.Ex.DomainException(JsonSerializer.Serialize(validation.Errors));
        }

        var result = await _mediator.Send(new UpdateUserCommand(updateUserDto));
        return Ok(new ResponseApi<bool>
        {
            Data = result,
            Status = true,
            Message = "Usuario actualizado exitosamente."
        });
    }

    /// <summary>
    /// Elimina un usuario existente. Solo administradores.
    /// Bloquea la auto-eliminación y la eliminación del último administrador del sistema.
    /// </summary>
    [Authorize(Roles = "Administrator")]
    [HttpDelete("users/delete")]
    public async Task<IActionResult> DeleteUser([FromQuery] string userId)
    {
        var requestingUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new Util.Ex.DomainException("No se pudo identificar al usuario autenticado.");

        var result = await _mediator.Send(new DeleteUserCommand(userId, requestingUserId));
        return Ok(new ResponseApi<bool>
        {
            Data = result,
            Status = true,
            Message = "Usuario eliminado exitosamente."
        });
    }

    /// <summary>
    /// Renombra un rol existente. Solo administradores.
    /// </summary>
    [Authorize(Roles = "Administrator")]
    [HttpPut("roles/update")]
    public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleDto updateRoleDto)
    {
        var validation = await _updateRoleValidator.ValidateAsync(updateRoleDto);
        if (validation.Errors.Count > 0)
        {
            throw new Util.Ex.DomainException(JsonSerializer.Serialize(validation.Errors));
        }

        var result = await _mediator.Send(new UpdateRoleCommand(updateRoleDto));
        return Ok(new ResponseApi<bool>
        {
            Data = result,
            Status = true,
            Message = "Rol actualizado exitosamente."
        });
    }

    /// <summary>
    /// Elimina un rol existente. Solo administradores.
    /// Bloquea la eliminación si el rol tiene usuarios asignados.
    /// </summary>
    [Authorize(Roles = "Administrator")]
    [HttpDelete("roles/delete")]
    public async Task<IActionResult> DeleteRole([FromQuery] string roleId)
    {
        var result = await _mediator.Send(new DeleteRoleCommand(roleId));
        return Ok(new ResponseApi<bool>
        {
            Data = result,
            Status = true,
            Message = "Rol eliminado exitosamente."
        });
    }

    /// <summary>
    /// Reemplaza la lista completa de permisos (módulos) de un rol. Solo administradores.
    /// </summary>
    [Authorize(Roles = "Administrator")]
    [HttpPut("roles/permissions")]
    public async Task<IActionResult> AssignRolePermissions([FromBody] AssignRolePermissionsDto assignRolePermissionsDto)
    {
        var validation = await _assignRolePermissionsValidator.ValidateAsync(assignRolePermissionsDto);
        if (validation.Errors.Count > 0)
        {
            throw new Util.Ex.DomainException(JsonSerializer.Serialize(validation.Errors));
        }

        var result = await _mediator.Send(new AssignRolePermissionsCommand(assignRolePermissionsDto));
        return Ok(new ResponseApi<bool>
        {
            Data = result,
            Status = true,
            Message = "Permisos actualizados exitosamente."
        });
    }
}

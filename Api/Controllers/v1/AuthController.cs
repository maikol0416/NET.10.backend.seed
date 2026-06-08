using System.Text.Json;
using Application.Auth.Cqrs.Commands;
using Application.Auth.Dtos;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller de autenticación. NO hereda de BaseController porque Auth
/// no es un CRUD de agregado del dominio — tiene sus propios endpoints.
/// Login y Register son públicos; Create-Role requiere autenticación.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<AuthLoginDto> _loginValidator;
    private readonly IValidator<AuthRegisterDto> _registerValidator;
    private readonly IValidator<CreateRoleDto> _createRoleValidator;

    public AuthController(
        IMediator mediator,
        IValidator<AuthLoginDto> loginValidator,
        IValidator<AuthRegisterDto> registerValidator,
        IValidator<CreateRoleDto> createRoleValidator)
    {
        _mediator = mediator;
        _loginValidator = loginValidator;
        _registerValidator = registerValidator;
        _createRoleValidator = createRoleValidator;
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
}

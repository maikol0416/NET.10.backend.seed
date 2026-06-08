using Domain.DomainShared;
using Domain.Ports.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infraestructure.Identity;

/// <summary>
/// Implementación del puerto IAuthService usando ASP.NET Core Identity.
/// Encapsula toda la lógica técnica de Identity (UserManager, SignInManager, RoleManager)
/// detrás de la interfaz agnóstica del dominio.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return new AuthResult(
                Success: false,
                Token: null,
                Email: null,
                FullName: null,
                Expiration: null,
                Errors: ["Credenciales inválidas."]
            );
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return new AuthResult(
                Success: false,
                Token: null,
                Email: null,
                FullName: null,
                Expiration: null,
                Errors: ["Credenciales inválidas."]
            );
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = await _jwtTokenService.GenerateTokenAsync(user.Id, user.Email!, roles);

        return new AuthResult(
            Success: true,
            Token: token,
            Email: user.Email,
            FullName: user.FullName,
            Expiration: DateTime.UtcNow.AddMinutes(60),
            Errors: null
        );
    }

    public async Task<AuthResult> RegisterAsync(string email, string password, string fullName, string role)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return new AuthResult(
                Success: false,
                Token: null,
                Email: null,
                FullName: null,
                Expiration: null,
                Errors: ["Ya existe un usuario registrado con este email."]
            );
        }

        // Validar que el rol exista antes de crear el usuario
        if (!string.IsNullOrWhiteSpace(role))
        {
            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                return new AuthResult(
                    Success: false,
                    Token: null,
                    Email: null,
                    FullName: null,
                    Expiration: null,
                    Errors: [$"El rol '{role}' no existe. Debe crearlo primero."]
                );
            }
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return new AuthResult(
                Success: false,
                Token: null,
                Email: null,
                FullName: null,
                Expiration: null,
                Errors: result.Errors.Select(e => e.Description)
            );
        }

        // Asignar rol al usuario recién creado
        if (!string.IsNullOrWhiteSpace(role))
        {
            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                return new AuthResult(
                    Success: false,
                    Token: null,
                    Email: null,
                    FullName: null,
                    Expiration: null,
                    Errors: roleResult.Errors.Select(e => e.Description)
                );
            }
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = await _jwtTokenService.GenerateTokenAsync(user.Id, user.Email!, roles);

        return new AuthResult(
            Success: true,
            Token: token,
            Email: user.Email,
            FullName: user.FullName,
            Expiration: DateTime.UtcNow.AddMinutes(60),
            Errors: null
        );
    }

    public async Task<AuthResult> CreateRoleAsync(string roleName)
    {
        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (roleExists)
        {
            return new AuthResult(
                Success: false,
                Token: null,
                Email: null,
                FullName: null,
                Expiration: null,
                Errors: [$"El rol '{roleName}' ya existe."]
            );
        }

        var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
        if (!result.Succeeded)
        {
            return new AuthResult(
                Success: false,
                Token: null,
                Email: null,
                FullName: null,
                Expiration: null,
                Errors: result.Errors.Select(e => e.Description)
            );
        }

        return new AuthResult(
            Success: true,
            Token: null,
            Email: null,
            FullName: null,
            Expiration: null,
            Errors: null
        );
    }
}

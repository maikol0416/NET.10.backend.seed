using Domain.DomainShared;
using Domain.Ports.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Identity;

/// <summary>
/// Implementación del puerto IAuthService usando ASP.NET Core Identity.
/// Encapsula toda la lógica técnica de Identity (UserManager, SignInManager, RoleManager)
/// detrás de la interfaz agnóstica del dominio.
/// </summary>
public class AuthService : IAuthService
{
    private const string AdminRoleName = "Administrator";

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
            Errors: null,
            Roles: roles
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
            Errors: null,
            Roles: roles
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

    public async Task<PaginatedList<UserSummary>> GetUsersPaginatedAsync(int pageNumber, int pageSize)
    {
        var totalCount = await _userManager.Users.CountAsync();
        var users = await _userManager.Users
            .OrderBy(u => u.Email)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var summaries = new List<UserSummary>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            summaries.Add(new UserSummary(user.Id, user.Email!, user.FullName, roles));
        }

        return new PaginatedList<UserSummary>(summaries, totalCount, pageNumber, pageSize);
    }

    public async Task<PaginatedList<RoleSummary>> GetRolesPaginatedAsync(int pageNumber, int pageSize)
    {
        var totalCount = await _roleManager.Roles.CountAsync();
        var roles = await _roleManager.Roles
            .OrderBy(r => r.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var summaries = roles.Select(r => new RoleSummary(r.Id, r.Name!)).ToList();
        return new PaginatedList<RoleSummary>(summaries, totalCount, pageNumber, pageSize);
    }

    public async Task<AuthResult> UpdateUserAsync(string userId, string email, string fullName, IEnumerable<string> roles)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Failure("Usuario no encontrado.");
        }

        if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
        {
            var ownerOfEmail = await _userManager.FindByEmailAsync(email);
            if (ownerOfEmail != null && ownerOfEmail.Id != userId)
            {
                return Failure($"Ya existe otro usuario registrado con el email '{email}'.");
            }
        }

        var incomingRoles = roles?.ToList() ?? [];
        var currentRoles = await _userManager.GetRolesAsync(user);
        var rolesToRemove = currentRoles.Except(incomingRoles).ToList();
        var rolesToAdd = incomingRoles.Except(currentRoles).ToList();

        if (rolesToRemove.Contains(AdminRoleName))
        {
            var adminGuardError = await ValidateNotLastAdminAsync(user);
            if (adminGuardError != null)
            {
                return Failure(adminGuardError);
            }
        }

        user.Email = email;
        user.UserName = email;
        user.FullName = fullName;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return Failure(updateResult.Errors.Select(e => e.Description));
        }

        if (rolesToRemove.Count > 0)
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded)
            {
                return Failure(removeResult.Errors.Select(e => e.Description));
            }
        }

        if (rolesToAdd.Count > 0)
        {
            foreach (var role in rolesToAdd)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    return Failure($"El rol '{role}' no existe. Debe crearlo primero.");
                }
            }

            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!addResult.Succeeded)
            {
                return Failure(addResult.Errors.Select(e => e.Description));
            }
        }

        return Success();
    }

    public async Task<AuthResult> DeleteUserAsync(string userId, string requestingUserId)
    {
        if (string.Equals(userId, requestingUserId, StringComparison.OrdinalIgnoreCase))
        {
            return Failure("No puedes eliminar tu propio usuario.");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Failure("Usuario no encontrado.");
        }

        var adminGuardError = await ValidateNotLastAdminAsync(user);
        if (adminGuardError != null)
        {
            return Failure(adminGuardError);
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return Failure(result.Errors.Select(e => e.Description));
        }

        return Success();
    }

    public async Task<AuthResult> UpdateRoleAsync(string roleId, string name)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return Failure("Rol no encontrado.");
        }

        if (!string.Equals(role.Name, name, StringComparison.OrdinalIgnoreCase) &&
            await _roleManager.RoleExistsAsync(name))
        {
            return Failure($"Ya existe otro rol con el nombre '{name}'.");
        }

        role.Name = name;
        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            return Failure(result.Errors.Select(e => e.Description));
        }

        return Success();
    }

    public async Task<AuthResult> DeleteRoleAsync(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return Failure("Rol no encontrado.");
        }

        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
        if (usersInRole.Count > 0)
        {
            return Failure($"No se puede eliminar el rol '{role.Name}' porque tiene {usersInRole.Count} usuario(s) asignado(s). Reasígnalos antes de eliminarlo.");
        }

        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded)
        {
            return Failure(result.Errors.Select(e => e.Description));
        }

        return Success();
    }

    /// <summary>
    /// Bloquea la operación si el usuario es Administrator y es el último con ese rol —
    /// evita que el sistema se quede sin administradores.
    /// </summary>
    private async Task<string?> ValidateNotLastAdminAsync(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);
        if (!userRoles.Contains(AdminRoleName))
        {
            return null;
        }

        var admins = await _userManager.GetUsersInRoleAsync(AdminRoleName);
        if (admins.Count <= 1)
        {
            return "No puedes eliminar ni quitar el rol Administrator al último administrador del sistema.";
        }

        return null;
    }

    private static AuthResult Success() => new(
        Success: true,
        Token: null,
        Email: null,
        FullName: null,
        Expiration: null,
        Errors: null
    );

    private static AuthResult Failure(string error) => Failure([error]);

    private static AuthResult Failure(IEnumerable<string> errors) => new(
        Success: false,
        Token: null,
        Email: null,
        FullName: null,
        Expiration: null,
        Errors: errors
    );
}

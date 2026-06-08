namespace Application.Auth.Dtos;

/// <summary>
/// DTO de entrada para el endpoint de creación de roles.
/// </summary>
public class CreateRoleDto
{
    public string RoleName { get; set; } = string.Empty;
}

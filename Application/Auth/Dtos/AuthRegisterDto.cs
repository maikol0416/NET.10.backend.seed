namespace Application.Auth.Dtos;

/// <summary>
/// DTO de entrada para el endpoint de registro.
/// </summary>
public class AuthRegisterDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

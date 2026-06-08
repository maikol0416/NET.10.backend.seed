namespace Application.Auth.Dtos;

/// <summary>
/// DTO de entrada para el endpoint de login.
/// </summary>
public class AuthLoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

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

    /// <summary>
    /// Empresa administradora destino del nuevo usuario. Solo la usa un Administrator
    /// (usuario de plataforma) para indicar a qué empresa pertenece el usuario que crea.
    /// Cualquier otro usuario la ignora: hereda automáticamente su propia empresa.
    /// </summary>
    public Guid? CompanyId { get; set; }
}

namespace Application.Auth.Dtos;

/// <summary>
/// DTO de entrada para actualizar un usuario existente (email, nombre y roles).
/// No permite cambiar la contraseña — eso queda fuera de alcance de este endpoint.
/// </summary>
public class UpdateUserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];

    /// <summary>
    /// Empresa administradora a la que queda atado el usuario. Se ignora (queda null)
    /// si Roles incluye Administrator — un usuario de plataforma nunca pertenece a una empresa.
    /// </summary>
    public Guid? CompanyId { get; set; }
}

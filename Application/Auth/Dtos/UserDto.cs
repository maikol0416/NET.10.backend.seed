namespace Application.Auth.Dtos;

/// <summary>
/// DTO de salida para el listado paginado de usuarios. Nunca expone
/// el hash de contraseña ni otros detalles internos de Identity.
/// </summary>
public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
}

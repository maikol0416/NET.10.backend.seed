namespace Application.Auth.Dtos;

/// <summary>
/// DTO de salida para el listado paginado de roles.
/// </summary>
public class RoleDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

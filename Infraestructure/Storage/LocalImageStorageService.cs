using Domain.Ports;
using Microsoft.AspNetCore.Http;

namespace Infraestructure.Storage;

/// <summary>
/// Implementación local (disco) del puerto IImageStorageService. Guarda el archivo
/// bajo wwwroot/uploads/physical-structures del proyecto Api (content root del
/// proceso en ejecución) para que quede servible como archivo estático, y retorna
/// la URL absoluta (esquema + host de la petición actual) para que el front pueda
/// usarla directamente sin tener que armarla.
/// </summary>
public class LocalImageStorageService : IImageStorageService
{
    private const string RelativeFolder = "uploads/physical-structures";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public LocalImageStorageService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> SaveAsync(byte[] imageBytes)
    {
        var extension = DetectExtension(imageBytes);
        var fileName = $"{Guid.NewGuid()}{extension}";

        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", RelativeFolder);
        Directory.CreateDirectory(folderPath);

        var fullPath = Path.Combine(folderPath, fileName);
        await File.WriteAllBytesAsync(fullPath, imageBytes);

        var relativePath = $"/{RelativeFolder}/{fileName}";
        var request = _httpContextAccessor.HttpContext?.Request;

        return request is null
            ? relativePath
            : $"{request.Scheme}://{request.Host}{relativePath}";
    }

    /// <summary>Detecta el formato por la firma de bytes (magic numbers). Si no la reconoce, usa .png.</summary>
    private static string DetectExtension(byte[] bytes)
    {
        if (bytes.Length >= 8 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
            return ".png";

        if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
            return ".jpg";

        if (bytes.Length >= 6 && bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46)
            return ".gif";

        if (bytes.Length >= 12 && bytes[8] == 0x57 && bytes[9] == 0x45 && bytes[10] == 0x42 && bytes[11] == 0x50)
            return ".webp";

        return ".png";
    }
}

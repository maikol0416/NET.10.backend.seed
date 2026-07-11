namespace Domain.Ports;

/// <summary>
/// Puerto del dominio/aplicación para persistir archivos de imagen. El dominio no
/// sabe ni le importa dónde ni cómo se guarda físicamente — hoy la Infraestructura
/// lo implementa escribiendo en disco local; podría cambiar a un storage externo
/// (S3, Blob Storage) sin tocar Domain ni Application.
/// </summary>
public interface IImageStorageService
{
    /// <summary>Guarda la imagen y retorna la ruta relativa para acceder a ella.</summary>
    Task<string> SaveAsync(byte[] imageBytes);
}

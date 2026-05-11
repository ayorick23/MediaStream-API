namespace Proyecto.Application.Interfaces;

public interface IMediaUploadService
{
    /// <summary>
    /// Procesa y almacena un archivo multimedia en el servidor.
    /// </summary>
    /// <param name="fileStream">Flujo de datos del archivo recibido.</param>
    /// <param name="fileName">Nombre original o deseado del archivo.</param>
    /// <returns>El nombre final con el que se guardó el archivo.</returns>
    Task<(Guid Id, string FileName)> UploadMediaAsync(Stream fileStream, string fileName, string title, string userId, Guid categoryId);
}
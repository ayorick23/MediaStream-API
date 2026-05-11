using Microsoft.Extensions.Options;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.Configuration;

namespace Proyecto.Infrastructure.Services;

public class MediaUploadService : IMediaUploadService
{
    private readonly MediaSettings _settings;

    public MediaUploadService(IOptions<MediaSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<string> UploadMediaAsync(Stream fileStream, string fileName)
    {
        // 1. Asegurarnos de que la ruta de almacenamiento existe
        if (!Directory.Exists(_settings.StoredFilesPath))
        {
            Directory.CreateDirectory(_settings.StoredFilesPath);
        }

        // 2. Construir la ruta completa combinando la carpeta con el nombre del archivo
        string fullPath = Path.Combine(_settings.StoredFilesPath, fileName);

        // 3. Crear el archivo en el servidor. 
        // 'using' asegura que el archivo se cierre y libere correctamente al terminar.
        using (var destinationStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            // 4. Copiar el contenido del stream de la API al stream del disco de forma asíncrona
            await fileStream.CopyToAsync(destinationStream);
        }

        return fileName;
    }
}
using Microsoft.Extensions.Options;
using Proyecto.Domain.Configuration;
using Proyecto.Application.Interfaces;

namespace Proyecto.Infrastructure.Services;

public class VideoStreamingService : IVideoStreamingService
{
    private readonly MediaSettings _settings;

    public VideoStreamingService(IOptions<MediaSettings> settings)
    {
        _settings = settings.Value;
    }

    public Stream GetVideoStream(string fileName)
    {
        string fullPath = Path.Combine(_settings.StoredFilesPath, fileName);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException("Archivo no encontrado en el servidor local.");

        // Abrimos el stream sin bloquear el archivo (FileShare.Read)
        return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
    }
}
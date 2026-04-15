using Microsoft.AspNetCore.Mvc;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.Configuration;
using Microsoft.Extensions.Options;

namespace Proyecto.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VideosController : ControllerBase
{
    private readonly IVideoStreamingService _streamingService;
    private readonly MediaSettings _settings;

    public VideosController(
        IVideoStreamingService streamingService,
        IOptions<MediaSettings> settings)
    {
        _streamingService = streamingService;
        _settings = settings.Value;
    }

    [HttpGet("stream/{nombreArchivo}")]
    public IActionResult StreamVideo(string nombreArchivo)
    {
        try
        {
            // 1. Validar extensión (Seguridad básica)
            var extension = Path.GetExtension(nombreArchivo).ToLower();
            if (!_settings.AllowedExtensions.Contains(extension))
            {
                return BadRequest("Formato de archivo no permitido.");
            }

            // 2. Obtener el Stream desde la capa de Infraestructura
            var stream = _streamingService.GetVideoStream(nombreArchivo);

            // 3. Determinar el tipo MIME
            // Si es .mp4 -> video/mp4 | Si es .mp3 -> audio/mpeg
            string contentType = extension == ".mp3" ? "audio/mpeg" : "video/mp4";

            // 4. RETORNO MAESTRO
            // 'enableRangeProcessing: true' es lo que permite el streaming real (HTTP 206)
            return File(stream, contentType, enableRangeProcessing: true);
        }
        catch (FileNotFoundException)
        {
            return NotFound($"El archivo '{nombreArchivo}' no existe en el servidor.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}
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
    private readonly IMediaUploadService _uploadService; // Nuevo servicio
    private readonly MediaSettings _settings;

    public VideosController(
        IVideoStreamingService streamingService,
        IMediaUploadService uploadService, // nuevo servicio subida
        IOptions<MediaSettings> settings)
    {
        _streamingService = streamingService;
        _uploadService = uploadService;
        _settings = settings.Value;
    }

    // iniciamos el método de subida de archivos multimedia (video/audio)
    [HttpPost("upload")]
    [DisableRequestSizeLimit] // Importante para permitir archivos de video grandes
    public async Task<IActionResult> Upload(IFormFile file)
    {
        // 1. Validaciones básicas
        if (file == null || file.Length == 0)
            return BadRequest("No se ha seleccionado ningún archivo.");

        try
        {
            // 2. Validar extensión contra nuestra lista permitida en appsettings.json
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_settings.AllowedExtensions.Contains(extension))
            {
                return BadRequest($"La extensión {extension} no está permitida.");
            }

            // 3. Procesar la subida
            // Usamos 'OpenReadStream' para obtener el flujo sin cargar el archivo en RAM
            using (var stream = file.OpenReadStream())
            {
                // Llamamos al método de la infraestructura que definimos en MediaUploadService
                var resultado = await _uploadService.UploadMediaAsync(stream, file.FileName);

                return Ok(new
                {
                    Mensaje = "Archivo subido correctamente",
                    Archivo = resultado
                });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno al subir el archivo: {ex.Message}");
        }
    }
    // terminamos el método de subida y ahora implementamos el método de streaming

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
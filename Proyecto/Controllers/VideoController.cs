using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.Configuration;
using Proyecto.Infrastructure.Persistence;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Proyecto.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VideosController : ControllerBase
{
    private readonly IVideoStreamingService _streamingService;
    private readonly IMediaUploadService _uploadService;
    private readonly MediaSettings _settings;
    private readonly ApplicationDbContext _context;

    public VideosController(
        IVideoStreamingService streamingService,
        IMediaUploadService uploadService,
        IOptions<MediaSettings> settings,
        ApplicationDbContext context)
    {
        _streamingService = streamingService;
        _uploadService = uploadService;
        _settings = settings.Value;
        _context = context;
    }

    [HttpPost("upload")]
    [Authorize(Roles = "Admin")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> Upload(IFormFile file, [FromForm] string? title, [FromForm] Guid categoryId)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No se ha seleccionado ningún archivo.");

        try
        {
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_settings.AllowedExtensions.Contains(extension))
            {
                return BadRequest($"La extensión {extension} no está permitida.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var stream = file.OpenReadStream())
            {
                var (id, archivo) = await _uploadService.UploadMediaAsync(
                    stream,
                    file.FileName,
                    title ?? file.FileName,
                    userId,
                    categoryId
                );

                return Ok(new
                {
                    Mensaje = "Archivo subido correctamente",
                    Archivo = archivo,
                    Id = id
                });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno al subir el archivo: {ex.Message}");
        }
    }

    [HttpGet("media/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var media = await _context.Media
            .Include(m => m.Category)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (media == null) return NotFound();

        return Ok(new
        {
            media.Id,
            media.Title,
            media.Description,
            Category = new { media.Category.Id, media.Category.Name },
            media.MimeType,
            media.FileSizeBytes,
            MediaType = media.MediaType.ToString(),
            media.Views,
            media.CreatedAt,
        });
    }

    [HttpPost("media/{id}/view")]
    public async Task<IActionResult> RegisterView(Guid id)
    {
        var media = await _context.Media.FirstOrDefaultAsync(m => m.Id == id);

        if (media == null) return NotFound();

        media.Views++;
        await _context.SaveChangesAsync();

        return Ok(new { views = media.Views });
    }

    [HttpGet("stream/{nombreArchivo}")]
    public IActionResult StreamVideo(string nombreArchivo)
    {
        try
        {
            var extension = Path.GetExtension(nombreArchivo).ToLower();
            if (!_settings.AllowedExtensions.Contains(extension))
            {
                return BadRequest("Formato de archivo no permitido.");
            }

            var stream = _streamingService.GetVideoStream(nombreArchivo);

            string contentType = extension == ".mp3" ? "audio/mpeg" : "video/mp4";

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

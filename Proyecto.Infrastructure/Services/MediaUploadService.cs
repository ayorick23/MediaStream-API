using Microsoft.Extensions.Options;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.Configuration;
using Proyecto.Domain.Entities.Enums;
using Proyecto.Domain.Entities.Media;
using Proyecto.Infrastructure.Persistence;

namespace Proyecto.Infrastructure.Services;

public class MediaUploadService : IMediaUploadService
{
    private readonly MediaSettings _settings;
    private readonly ApplicationDbContext _context;

    public MediaUploadService(IOptions<MediaSettings> settings, ApplicationDbContext context)
    {
        _settings = settings.Value;
        _context = context;
    }

    public async Task<(Guid Id, string FileName)> UploadMediaAsync(Stream fileStream, string fileName, string title, string userId, Guid categoryId)
    {
        if (!Directory.Exists(_settings.StoredFilesPath))
        {
            Directory.CreateDirectory(_settings.StoredFilesPath);
        }

        string fullPath = Path.Combine(_settings.StoredFilesPath, fileName);

        using (var destinationStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await fileStream.CopyToAsync(destinationStream);
        }

        var extension = Path.GetExtension(fileName).ToLower();

        string mimeType = extension switch
        {
            ".mp4" => "video/mp4",
            ".mkv" => "video/x-matroska",
            ".mp3" => "audio/mpeg",
            ".wav" => "audio/wav",
            _ => "application/octet-stream"
        };

        bool isVideo = extension == ".mp4" || extension == ".mkv";
        long fileSize = new FileInfo(fullPath).Length;
        var entityId = Guid.NewGuid();

        if (isVideo)
        {
            var video = new Video
            {
                Id = entityId,
                Title = title,
                FileName = fileName,
                StoredFileName = fileName,
                FilePath = fullPath,
                MimeType = mimeType,
                MediaType = MediaType.Video,
                UploadedByUserId = userId,
                CategoryId = categoryId,
                FileSizeBytes = fileSize
            };
            _context.Videos.Add(video);
        }
        else
        {
            var audio = new Audio
            {
                Id = entityId,
                Title = title,
                FileName = fileName,
                StoredFileName = fileName,
                FilePath = fullPath,
                MimeType = mimeType,
                MediaType = MediaType.Audio,
                UploadedByUserId = userId,
                CategoryId = categoryId,
                FileSizeBytes = fileSize
            };
            _context.Audios.Add(audio);
        }

        await _context.SaveChangesAsync();

        return (entityId, fileName);
    }
}

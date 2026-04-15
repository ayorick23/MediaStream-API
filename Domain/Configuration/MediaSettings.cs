namespace Proyecto.Domain.Configuration;

public class MediaSettings
{
    public const string SectionName = "MediaSettings"; // Nombre en appsettings.json
    public string StoredFilesPath { get; set; } = string.Empty;
    public List<string> AllowedExtensions { get; set; } = new();
}
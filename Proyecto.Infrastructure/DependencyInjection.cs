using Microsoft.Extensions.DependencyInjection;
using Proyecto.Application.Interfaces;
using Proyecto.Infrastructure.Services;

namespace Proyecto.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IVideoStreamingService, VideoStreamingService>();
        // Nuevo servicio para subir archivos multimedia
        services.AddScoped<IMediaUploadService, MediaUploadService>();
        return services;
    }
}
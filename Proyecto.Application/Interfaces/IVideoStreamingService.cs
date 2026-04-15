namespace Proyecto.Application.Interfaces;

public interface IVideoStreamingService
{
    // Recibe el nombre del archivo y retorna el flujo de datos
    Stream GetVideoStream(string fileName);
}
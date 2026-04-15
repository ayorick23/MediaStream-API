# Multimedia Stream API

Esta es una API robusta y escalable desarrollada en C# para la gestión y reproducción de contenido multimedia. El proyecto ha sido diseñado siguiendo los principios de Clean Architecture, lo que permite una separación de responsabilidades clara y facilita su crecimiento para ser consumido por aplicaciones web modernas.

# 🏗️ Arquitectura

El proyecto se divide en las siguientes capas para asegurar la mantenibilidad y desacoplamiento:

- **Domain:** Contiene las entidades, interfaces de repositorios y lógica de negocio central (reglas de dominio). No tiene dependencias externas.
- **Application:** Implementa los casos de uso del sistema. Orquestador de la lógica de negocio y definiciones de DTOs.
- **Infrastructure:** Implementa el acceso a datos, servicios externos (como almacenamiento de archivos multimedia) y otras herramientas de terceros.
- **API / Presentation:** Punto de entrada de la aplicación que expone los endpoints REST para ser consumidos por el frontend.

# 🚀 Tecnologías Principales

- **Lenguaje:** C#
- **Framework:** .NET 10.0
- **Persistencia:** Entity Framework Core (SQL Server)
- **Documentación:** OpenAPI

# 🛠️ Instalación y Configuración

1. **Clonar el repositorio:**

```bash
git clone https://github.com/ayorick23/MediaStream-API
```

2. **Configurar el Connection String:**

   Antes de ejecutar el proyecto, debes configurar la cadena de conexión a tu base de datos:

   - Abre el archivo `appsettings.json` ubicado en la raíz del proyecto
   - Localiza la sección `ConnectionStrings` y agrega tu cadena de conexión:
   
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=MediaStreamDB;Trusted_Connection=True;TrustServerCertificate=True"
   }
   ```
   
   O si usas autenticación SQL Server:
   
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=MediaStreamDB;User Id=tu_usuario;Password=tu_contraseña;TrustServerCertificate=True"
   }
   ```

3. **Configurar la ruta de almacenamiento de medios:**

   En el archivo `appsettings.json`, configura la ruta donde se almacenarán los archivos multimedia:
   
   ```json
   "MediaSettings": {
     "StoredFilesPath": "C:\\Ruta\\A\\Tu\\Carpeta",
     "AllowedExtensions": [ ".mp4", ".mkv", ".mp3", ".wav" ]
   }
   ```

4. **Ejecutar migraciones:**

```bash
dotnet ef database update --project Proyecto.Infrastructure --startup-project Proyecto
```

5. **Iniciar la aplicación:**

```bash
dotnet run --project Proyecto
```

# 🛣️ Roadmap / Próximos Pasos

- [x] Implementar autenticación y autorización con JWT.
- [ ] Integración con proveedores de almacenamiento en la nube (AWS S3 / Azure Blob Storage).
- [ ] Implementar transcodificación de video en segundo plano.
- [ ] Desarrollo de cliente web para consumo de la API.

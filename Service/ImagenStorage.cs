using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;


namespace ProyectoFinal4.Service
{
    public class ImagenStorage
    {
        private readonly IWebHostEnvironment _env; //inyección para obtener wwwroot

        // formatos permitidos, se puede ampliar
        private static readonly HashSet<string> _allowed = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/png",
            "image/jpeg",
            "image/jpg",
            "image/webp"
        };

        //constructor, recibe la ruta wwwroot del servidor para guardar las imágenes
        public ImagenStorage(IWebHostEnvironment env)
        {
            _env = env;
        }


        //Método para guardar la imagen
        public async Task<string> SaveAsync(string userId, IFormFile file, CancellationToken ct = default)
        {
            if (file is null || file.Length == 0)
                throw new InvalidOperationException("Archivo vacío.");

            if (file.Length > 2 * 1024 * 1024)
                throw new InvalidOperationException("Supera el límite de 2 MB.");

            // 1) Validar Content-Type declarado (los formatos que declaramos arriba) 
            if (!_allowed.Contains(file.ContentType))
                throw new InvalidOperationException("Formato no permitido.");

            // 2) Validar firma real (cargar con ImageSharp). Esto evita spoofing (extensión incorrecta jpeg, png etc.).
            //Puede lanzar excepciones si no es imagen o está corrupta.
            //hay que aplicar manejo de excepciones aquí y luego en el controlador
            //en el controlador se puede capturar y guardar en el modelstate
            using var image = await Image.LoadAsync(file.OpenReadStream(), ct);

            // 3) Normalizar Imagen: recortar cuadrado y redimensionar (p.ej. 512x512)
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(512, 512),
                Mode = ResizeMode.Crop
            }));

            //CREACION DE RUTAS Y NOMBRES DE LA CARPETA
            // 4) Elegir extensión de salida (recomiendo webp o jpg)
            var ext = ".webp"; //webp es más eficiente y utilizada, ya que es menos pesada y carga mas rapido 
            var folderRel = $"/uploads/avatars/{userId}"; // ruta relativa para guardar las imagenes
            var folderAbs = Path.Combine(_env.WebRootPath, "uploads", "avatars", userId); // ruta absoluta concatenada en el servidor

            //CREACION DE CARPETA SI NO EXISTE
            Directory.CreateDirectory(folderAbs); // crear la carpeta si no existe

            //NOMBRE Y RUTA DEL ARCHIVO A GUARDAR (IMAGEN)
            var fileName = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}_{Guid.NewGuid():N}{ext}"; // nombre único para evitar colisiones
            var absPath = Path.Combine(folderAbs, fileName); // ruta absoluta completa
            var relPath = $"{folderRel}/{fileName}".Replace("\\", "/"); // ruta relativa completa para almacenar en BD

            await image.SaveAsWebpAsync(absPath, ct); // guardar la imagen en formato webp // necesita SixLabors.ImageSharp.Formats.Webp

            return relPath; // devolver la ruta relativa para almacenar en BD
        }


        //Metodo para eliminar una imagen 
        public Task DeleteAsync(string? relativePath, CancellationToken ct = default)
        {
            //si la ruta es nula o vacia no hace nada
            if (string.IsNullOrWhiteSpace(relativePath)) return Task.CompletedTask;

            //construir la ruta absoluta
            var abs = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

            //eliminar el archivo si existe
            if (File.Exists(abs)) File.Delete(abs);

            //tarea completada
            return Task.CompletedTask;
        }

    }


}


namespace ProyectoFinal4.Models
{
    public class Plataforma
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string  Url { get; set; }
        public string LogoUrl { get; set; }
        public List<Pelicula>? PeliculasPlataformas { get; set; }
    }
}

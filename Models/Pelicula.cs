using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinal4.Models
{
    public class Pelicula
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título de la película es obligatorio")]
        [StringLength(100, ErrorMessage = "El título no puede superar los 100 caracteres")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "La fecha de lanzamiento es obligatoria")]
        [DataType(DataType.Date, ErrorMessage = "Ingresá una fecha válida")]
        public DateTime FechaLanzamiento { get; set; }

        [Required(ErrorMessage = "La duración es obligatoria")]
        [Range(1, 500, ErrorMessage = "La duración debe estar entre 1 y 500 minutos")]
        public int? MinutosDuracion { get; set; }

        [Required(ErrorMessage = "La sinopsis es obligatoria")]
        [StringLength(1000, ErrorMessage = "La sinopsis no puede superar los 1000 caracteres")]
        public string Sinopsis { get; set; }

        [Required(ErrorMessage = "La URL del póster es obligatoria")]
        [Url(ErrorMessage = "Ingresá una URL válida para el póster")]
        public string PosterUrlPortada { get; set; }
        public int GeneroId { get; set; }
        public Genero? Genero { get; set; }
        public int PlataformaId { get; set; }
        public Plataforma? Plataforma { get; set; }
        [NotMapped] //sirve para que no se cree en la base de datos
        public int?  PromedioRating { get; set; }
        public List<Review>? Listareviews { get; set; }
        public List<Favorito>? UsuariosFavorito { get; set; }

    }
}

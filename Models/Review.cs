using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal4.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int PeliculaId { get; set; }
        public Pelicula? Pelicula { get; set; }
        public string UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        [Required]
        [StringLength(500)]
        public string  Comentario { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaReview { get; set; }

        //row version for concurrency control
        [Timestamp]
        public byte[] RowVersion { get; set; } //sirve para que 2 usuarios o mas no puedan editar el mismo registro al mismo tiempo
    }
}

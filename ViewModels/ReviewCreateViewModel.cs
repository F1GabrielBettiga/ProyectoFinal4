using Microsoft.AspNetCore.Mvc.Routing;
using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal4.ViewModels
{
    public class ReviewCreateViewModel
    {
        public int? Id { get; set; }
        public int PeliculaId { get; set; }
        public string? PeliculaTitulo  { get; set; }
        public string UsuarioId { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "La calificacion debe estar entre 1 y 5 estrellas")]
        [Required(ErrorMessage = "La calificacion es obligatoria")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "El comentario no puede exceder los 500 caracteres")]
        [Required(ErrorMessage = "El comentario es obligatorio")]
        public string Comentario { get; set; } = string.Empty;



    }
}

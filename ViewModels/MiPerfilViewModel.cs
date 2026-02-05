using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal4.ViewModels
{
    public class MiPerfilViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres.")]
        public string Apellido { get; set; }

        // NO requerido

        public string? Email { get; set; }

        // NO requerido
        public IFormFile? ImagenPerfil { get; set; } // para subir nueva imagen

        public string? ImagenUrlPerfil { get; set; }


    }
}

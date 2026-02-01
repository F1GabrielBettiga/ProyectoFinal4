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

        // ⬇️ NO requerido por ahora

        public string? Email { get; set; }

        // ⬇️ NO requerido
        public string? ImagenUrlPerfil { get; set; }

        // ⬇️ NO requerido
  
        public string? ClaveActual { get; set; }

        // ⬇️ NO requerido

        public string? ClaveNueva { get; set; }

        // ⬇️ NO requerido y CORREGIDO el Compare

        public string? ConfirmarClave { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal4.ViewModels
{
    public class MiPerfilClaveViewModel
    {

        [Required(ErrorMessage = "Debes ingresar tu contraseña actual")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        public string ClaveActual { get; set; }

        [Required(ErrorMessage = "Debes ingresar una nueva contraseña")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string ClaveNueva { get; set; }

        [Required(ErrorMessage = "Debes confirmar la nueva contraseña")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nueva contraseña")]
        [Compare("ClaveNueva", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarClave { get; set; }

    }
}

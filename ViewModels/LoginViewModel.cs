using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal4.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El campo Email es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El campo Clave es obligatorio")]
        public string Clave { get; set; }



    }
}

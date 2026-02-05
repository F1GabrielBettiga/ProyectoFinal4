using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinal4.Models;
using ProyectoFinal4.Service;
using ProyectoFinal4.ViewModels;

namespace ProyectoFinal4.Controllers
{
    
    public class UsuarioController : Controller

    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly ImagenStorage _imagenStorage; 
        public UsuarioController(UserManager<Usuario> userManager, SignInManager<Usuario> singnInManager, ImagenStorage imagenStorage)
        {
            _userManager = userManager; //nos sirve para la parte del registro
            _signInManager = singnInManager;//nos siirve para la parte del login
            _imagenStorage = imagenStorage; // servicio para manejo de imagenes
        }



        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //Esto es para evitar ataques CSRF
        public async Task <IActionResult> Login(LoginViewModel usuario)
        {
            //Validamos si el modelo es valido
            if (ModelState.IsValid)
            {
                var resultado = await _signInManager.PasswordSignInAsync(usuario.Email, usuario.Clave, isPersistent: false, lockoutOnFailure: false); //Aqui se valida el login del usuario
                //chequeamos si el login fue exitoso
                if (resultado.Succeeded)
                {
                    //Redirigimos al usuario a la pagina principal
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    //Si el login no fue exitoso, mostramos un mensaje de error
                    ModelState.AddModelError(string.Empty, "Error. Revisá los datos ingresados.");
                }
            }
            return View(usuario);
        }

        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //Esto es para evitar ataques CSRF
        public async Task<IActionResult> Registro(UsuarioViewModel usuario)
        {
            if (ModelState.IsValid)
            {
                var NuevoUsuario = new Usuario
                {
                    UserName = usuario.Email,
                    Email = usuario.Email,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    ImagenUrlPerfil = "/images/defaults/default-profile.jpg",
                };
                var resultado = await _userManager.CreateAsync(NuevoUsuario, usuario.Clave); //Aqui se crea el usuario en la base de datos

                if (resultado.Succeeded)
                {
                    await _signInManager.SignInAsync(NuevoUsuario, isPersistent: false); //Aqui se loguea automaticamente al usuario despues de registrarse
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in resultado.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description); //Aqui se agregan los errores al ModelState para mostrarlos en la vista
                    }
                }

            }

            return View(usuario);
        }
        [HttpGet]
        public async Task<IActionResult> MiPerfil()
        {
            var usuarioActual = await _userManager.GetUserAsync(User);

            if (usuarioActual == null)
                return RedirectToAction("Login", "Usuario");


            var vm = new MiPerfilViewModel
            {
                Nombre = usuarioActual.Nombre,
                Apellido = usuarioActual.Apellido,
                Email = usuarioActual.Email,
                ImagenUrlPerfil = usuarioActual.ImagenUrlPerfil
            };

            return View(vm); // esto abre Views/Usuario/MiPerfil.cshtml
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarPerfil(MiPerfilViewModel usuarioVM)
        {
            if (!ModelState.IsValid)
                return View("MiPerfil", usuarioVM);

            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null)
                return RedirectToAction("Login", "Usuario");

            // Actualiza la imagen de perfil solo si el usuario selecciona una nueva; si queda el placeholder, no se guarda nada y elimina la anterior si existe
            try
            {
                if (usuarioVM.ImagenPerfil is not null && usuarioVM.ImagenPerfil.Length > 0)
                {
                    // opcional: borrar la anterior (si no es placeholder)
                    if (!string.IsNullOrWhiteSpace(usuarioActual.ImagenUrlPerfil))
                        await _imagenStorage.DeleteAsync(usuarioActual.ImagenUrlPerfil);

                    var nuevaRuta = await _imagenStorage.SaveAsync(usuarioActual.Id, usuarioVM.ImagenPerfil);
                    usuarioActual.ImagenUrlPerfil = nuevaRuta;
                    usuarioVM.ImagenUrlPerfil = nuevaRuta;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(usuarioVM);
            }

            usuarioActual.Nombre = usuarioVM.Nombre;
            usuarioActual.Apellido = usuarioVM.Apellido;

            var resultado = await _userManager.UpdateAsync(usuarioActual);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View("MiPerfil", usuarioVM);
            }

            TempData["MensajePerfil"] = "Perfil actualizado correctamente.";
            return RedirectToAction(nameof(MiPerfil));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarClave(MiPerfilClaveViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Guardamos los errores para mostrarlos
                var errores = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                TempData["ErrorClave"] = string.Join(" ", errores);
                return RedirectToAction(nameof(MiPerfil));
            }

            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
                return RedirectToAction("Login", "Usuario");

            var result = await _userManager.ChangePasswordAsync(usuario, model.ClaveActual, model.ClaveNueva); //le enviamos el usuario, la clave actual y la nueva clave

            if (!result.Succeeded)
            {
                TempData["ErrorClave"] = "Error, no se pudo actualizar la contraseña. Revisá los datos ingresados.";
                return RedirectToAction(nameof(MiPerfil));
            }

            await _signInManager.RefreshSignInAsync(usuario);

            TempData["MensajeClave"] = "Contraseña actualizada correctamente.";
            return RedirectToAction(nameof(MiPerfil));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); //Aqui se desloguea al usuario
            return RedirectToAction("Index", "Home"); //Redirigimos al usuario a la pagina principal despues de desloguearse
        }

        public IActionResult AccessDenied()
        {
            return View();
        }



    }
}

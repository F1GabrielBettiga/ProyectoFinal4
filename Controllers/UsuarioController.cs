using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinal4.Models;
using ProyectoFinal4.ViewModels;

namespace ProyectoFinal4.Controllers
{
    
    public class UsuarioController : Controller

    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        public UsuarioController(UserManager<Usuario> userManager, SignInManager<Usuario> singnInManager)
        {
            _userManager = userManager; //nos sirve para la parte del registro
            _signInManager = singnInManager;//nos siirve para la parte del login
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
                    ModelState.AddModelError(string.Empty, resultado.ToString());
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
                    ImagenUrlPerfil = "default-profile.jpg",
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
            var usuarioActual =  await _userManager.GetUserAsync(User); //la variable User es global y representa el usuario logeado y solo trae los datos basicos por esa razon vamos a tener que buscar el usuario en la base de datos

            if (usuarioActual == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            var usuarioLogeado = new MiPerfilViewModel
            {
                Nombre = usuarioActual.Nombre,
                Apellido = usuarioActual.Apellido,
                Email = usuarioActual.Email,
                ImagenUrlPerfil = usuarioActual.ImagenUrlPerfil
            };
            //esto nos permite mostrar los datos del usuario en la vista


            return View(usuarioLogeado);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MiPerfil(MiPerfilViewModel usuarioVM)
        {
            if (ModelState.IsValid)
            {
                var usuarioActual = await _userManager.GetUserAsync(User); //la variable User es global y representa el usuario logeado y solo trae los datos basicos por esa razon vamos a tener que buscar el usuario en la base de datos

                usuarioActual.Nombre = usuarioVM.Nombre;
                usuarioActual.Apellido = usuarioVM.Apellido;

                var resultado = await _userManager.UpdateAsync(usuarioActual);
                if (resultado.Succeeded)
                {
                    ViewBag.Mensaje = "Perfil actualizado correctamente";
                    return View(usuarioVM);//Redirigimos al usuario a la misma vista para que vea los cambios realizados
                }
                else
                {
                    foreach (var error in resultado.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description); //Aqui se agregan los errores al ModelState para mostrarlos en la vista
                    }
                }

            }

            return View(usuarioVM);
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

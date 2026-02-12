using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal4.Data;
using ProyectoFinal4.Models;
using ProyectoFinal4.ViewModels;

namespace ProyectoFinal4.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly MovieDbContext _context;

        public ReviewController(UserManager<Usuario> userManager, MovieDbContext context)
        {
            _userManager = userManager;
            _context = context;

        }

        // GET: ReviewController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ReviewController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ReviewController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReviewController/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReviewCreateViewModel review)
        {
            try
            {
                review.UsuarioId = _userManager.GetUserId(User); // Obtener el ID del usuario actual

                // Verificar si el usuario ya ha realizado una reseña para la película
                var reviewExistente = _context.Reviews
                    .FirstOrDefault(r => r.PeliculaId == review.PeliculaId && r.UsuarioId == review.UsuarioId);
                // Si ya existe una reseña, mostrar un mensaje de error
                if (reviewExistente != null)
                {
                    TempData["ReviewExiste"] = "Ya has realizado una reseña para esta pelicula";
                    return RedirectToAction("Details", "Home", new { id = review.PeliculaId });
                }

                if (ModelState.IsValid)
                {
                    var nuevaReview = new Review
                    {
                        PeliculaId = review.PeliculaId,
                        UsuarioId = review.UsuarioId,
                        Rating = review.Rating,
                        Comentario = review.Comentario,
                        FechaReview = DateTime.Now
                    };
                    _context.Reviews.Add(nuevaReview);
                    _context.SaveChanges();
                    return RedirectToAction("Details", "Home", new { id = review.PeliculaId});
                }

                return RedirectToAction("Details", "Home", new { id = review.PeliculaId });
            }
            catch
            {
                return View(review);
            }
        }

        // GET: ReviewController/Edit/5
        public IActionResult Edit(int idReview)
        {
            // Validar que el ID sea válido
            if (idReview <= 0)
            {
                return NotFound();
            }
            // Obtener el usuario actual
            var usuarioActual = _userManager.GetUserAsync(User).Result;


            // Buscar la reseña por ID y verificar que pertenezca al usuario actual tambien incluir la pelicula para mostrar su titulo en la vista
            var review = _context.Reviews
                            .Include(r => r.Pelicula)
                            .FirstOrDefault(r => r.Id == idReview && r.UsuarioId == usuarioActual.Id);

            // Si no se encuentra la reseña o no pertenece al usuario actual, retornar NotFound
            if (review == null)
            {
                return NotFound();
            }


            // Crear un ViewModel para pasar los datos a la vista de edición
            ReviewCreateViewModel reviewEditar = new ReviewCreateViewModel
            {
                Id = review.Id,
                PeliculaId = review.PeliculaId,
                UsuarioId = review.UsuarioId,
                Rating = review.Rating,
                Comentario = review.Comentario,
                UrlImagenPelicula = review.Pelicula?.PosterUrlPortada,               
                PeliculaTitulo = review.Pelicula.Titulo
            };


            return View(reviewEditar);
        }

        // POST: ReviewController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ReviewCreateViewModel reviewEditar)
        {
            if (!ModelState.IsValid)
            {
                return View(reviewEditar);
            }

            if (!reviewEditar.Id.HasValue || reviewEditar.Id.Value <= 0)
            {
                return NotFound();
            }

            // Usuario logueado
            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null)
                return RedirectToAction("Login", "Usuario");

            // Traigo la review SOLO si es del usuario
            var reviewDb = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == reviewEditar.Id.Value && r.UsuarioId == usuarioActual.Id);

            if (reviewDb == null)
            {
                return NotFound();
            }

            // Actualizo solo lo que corresponde
            reviewDb.Rating = reviewEditar.Rating;
            reviewDb.Comentario = reviewEditar.Comentario;

            await _context.SaveChangesAsync();

            return RedirectToAction("MisReseñas", "Usuario"); // o "Review" según dónde esté tu acción
        }


        // GET: ReviewController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ReviewController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

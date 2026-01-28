using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal4.Data;
using ProyectoFinal4.Models;
using System.Diagnostics;

namespace ProyectoFinal4.Controllers
{
    public class HomeController : Controller
    {
        private readonly MovieDbContext _context;

        public HomeController(MovieDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {

            ViewBag.GeneroId = new SelectList(_context.Generos, "Id", "Descripcion");
            ViewBag.PlataformaId = new SelectList(_context.Plataformas, "Id", "Nombre");

            //traer las pelicuas con su genero y plataforma
            var peliculas = await _context.Peliculas
                .Include(p => p.Genero)
                .Include(p => p.Plataforma)
                .ToListAsync();

            return View(peliculas);
        }

        // GET: Pelicula/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pelicula = await _context.Peliculas
                .Include(p => p.Genero)
                .Include(p => p.Plataforma)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pelicula == null)
            {
                return NotFound();
            }

            return View(pelicula);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

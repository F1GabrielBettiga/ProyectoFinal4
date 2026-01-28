using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal4.Data;
using ProyectoFinal4.Models;
using System;
using System.Diagnostics;
using System.Linq;

namespace ProyectoFinal4.Controllers
{
    public class HomeController : Controller
    {
        private readonly MovieDbContext _context;

        public HomeController(MovieDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10; //numero de items por pagina

            ViewBag.GeneroId = new SelectList(_context.Generos, "Id", "Descripcion");
            ViewBag.PlataformaId = new SelectList(_context.Plataformas, "Id", "Nombre");

            //aca vamos a chequear si pagina es menor a 1 por si alguien pone un numero negativo
            if (page < 1)
            {
                page = 1;
            }


            // total items
            var totalItems = await _context.Peliculas.CountAsync(); // contar todas las peliculas

            //aca vamos a chequer si la page es mayor al total de paginas y si lo es la ponemos en la ultima pagina
            if (page > totalItems / pageSize + 1)
            {
                page = (totalItems / pageSize) + 1;
            }


            // traer las pelicuas con su genero y plataforma usando y incluimos un paginacion con skip y take
            var peliculas = await _context.Peliculas
                .Include(p => p.Genero)
                .Include(p => p.Plataforma)
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page; // pagina actual
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize); // total de paginas 
            ViewBag.PageSize = pageSize; // numero de items por pagina 

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

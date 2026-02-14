using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal4.Data;
using ProyectoFinal4.Models;

namespace ProyectoFinal4.Controllers
{
    [Authorize (Roles = "Admin")]
    public class PeliculaController : Controller
    {
        private readonly MovieDbContext _context;

        public PeliculaController(MovieDbContext context)
        {
            _context = context;
        }

        // GET: Pelicula
        public async Task<IActionResult> Index(int page = 1, string txtBusqueda = "", int generoId = 0, int plataformaId = 0)
        {
            const int pageSize = 10; // número de items por página

            /* ================================
             * CARGA DE SELECTS (FILTROS)
             * ================================ */

            // Cargamos los Plataformas ordenados
            var listaDePlataformas = await _context.Plataformas
                .OrderBy(g => g.Nombre)
                .ToListAsync();

            // Agregamos la opción "Plataformas" con Id = 0
            // Esto nos sirve para saber que NO hay filtro aplicado
            listaDePlataformas.Insert(0, new Plataforma
            {
                Id = 0,
                Nombre = "Plataformas"
            });

            // Creamos el SelectList y marcamos el Plataformas seleccionado
            ViewBag.PlataformaId = new SelectList(
                listaDePlataformas,
                "Id",
                "Nombre",
                plataformaId
            );

            // Cargamos los géneros ordenados
            var listaDeGeneros = await _context.Generos
                .OrderBy(g => g.Descripcion)
                .ToListAsync();

            // Agregamos la opción "Generos" con Id = 0
            // Esto nos sirve para saber que NO hay filtro aplicado
            listaDeGeneros.Insert(0, new Genero
            {
                Id = 0,
                Descripcion = "Generos"
            });

            // Creamos el SelectList y marcamos el género seleccionado
            ViewBag.GeneroId = new SelectList(
                listaDeGeneros,
                "Id",
                "Descripcion",
                generoId
            );

            /* ================================
             * VALIDACIONES DE PAGINADO
             * ================================ */

            // Chequeamos que la página no sea menor a 1
            if (page < 1)
            {
                page = 1;
            }

            /* ================================
             * ARMADO DE LA CONSULTA
             * ================================ */

            // Creamos una consulta base (todavía no se ejecuta)
            var consulta = _context.Peliculas.AsQueryable();

            // Filtro por título (búsqueda)
            if (!string.IsNullOrEmpty(txtBusqueda))
            {
                consulta = consulta.Where(p => p.Titulo.Contains(txtBusqueda));
            }

            // Filtro por género (si no es 0)
            if (generoId > 0)
            {
                consulta = consulta.Where(p => p.GeneroId == generoId);
            }

            // Filtro por plataforma (si no es 0)
            if (plataformaId > 0)
            {
                consulta = consulta.Where(p => p.PlataformaId == plataformaId);
            }

            /* ================================
             * PAGINADO
             * ================================ */

            // Contamos el total de registros filtrados
            var totalItems = await consulta.CountAsync();

            // Calculamos el total de páginas
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Si la página es mayor al total, la ajustamos
            if (page > totalPages)
            {
                page = totalPages == 0 ? 1 : totalPages;
            }

            /* ================================
             * OBTENER PELÍCULAS
             * ================================ */

            // Traemos las películas con género y plataforma
            // Aplicamos paginado con Skip y Take
            var peliculas = await consulta
                .Include(p => p.Genero)
                .Include(p => p.Plataforma)
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            /* ================================
             * VIEWBAGS PARA LA VISTA
             * ================================ */

            ViewBag.CurrentPage = page; // página actual
            ViewBag.TotalPages = totalPages; // total de páginas
            ViewBag.PageSize = pageSize; // items por página
            ViewBag.TxtBusqueda = txtBusqueda; // texto de búsqueda actual

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

        // GET: Pelicula/Create
        public IActionResult Create()
        {
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Descripcion");
            ViewData["PlataformaId"] = new SelectList(_context.Plataformas, "Id", "Nombre");
            return View();
        }

        // POST: Pelicula/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,FechaLanzamiento,MinutosDuracion,Sinopsis,PosterUrlPortada,GeneroId,PlataformaId")] Pelicula pelicula)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pelicula);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Descripcion", pelicula.GeneroId);
            ViewData["PlataformaId"] = new SelectList(_context.Plataformas, "Id", "Nombre", pelicula.PlataformaId);
            return View(pelicula);
        }

        // GET: Pelicula/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pelicula = await _context.Peliculas.FindAsync(id);
            if (pelicula == null)
            {
                return NotFound();
            }
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Descripcion", pelicula.GeneroId);
            ViewData["PlataformaId"] = new SelectList(_context.Plataformas, "Id", "Nombre", pelicula.PlataformaId);
            return View(pelicula);
        }

        // POST: Pelicula/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,FechaLanzamiento,MinutosDuracion,Sinopsis,PosterUrlPortada,GeneroId,PlataformaId")] Pelicula pelicula)
        {
            if (id != pelicula.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pelicula);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PeliculaExists(pelicula.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Descripcion", pelicula.GeneroId);
            ViewData["PlataformaId"] = new SelectList(_context.Plataformas, "Id", "Nombre", pelicula.PlataformaId);
            return View(pelicula);
        }

        // GET: Pelicula/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Pelicula/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pelicula = await _context.Peliculas.FindAsync(id);
            if (pelicula != null)
            {
                _context.Peliculas.Remove(pelicula);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PeliculaExists(int id)
        {
            return _context.Peliculas.Any(e => e.Id == id);
        }


    }
}

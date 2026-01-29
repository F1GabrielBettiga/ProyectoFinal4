using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal4.Data;
using ProyectoFinal4.Models;

namespace ProyectoFinal4.Controllers
{
    public class GeneroController : Controller
    {
        private readonly MovieDbContext _context;

        public GeneroController(MovieDbContext context)
        {
            _context = context;
        }

        // GET: Genero
        public async Task<IActionResult> Index(int page = 1, string txtBusqueda = "")
        {
            const int pageSize = 3; // número de items por página

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
            var consulta = _context.Generos.AsQueryable();

            // Filtro por título (búsqueda)
            if (!string.IsNullOrEmpty(txtBusqueda))
            {
                consulta = consulta.Where(g => g.Descripcion.Contains(txtBusqueda));
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
            var generos = await consulta
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

            return View(generos);
        }

        // GET: Genero/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genero = await _context.Generos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genero == null)
            {
                return NotFound();
            }

            return View(genero);
        }

        // GET: Genero/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Genero/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion")] Genero genero)
        {
            if (ModelState.IsValid)
            {
                _context.Add(genero);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(genero);
        }

        // GET: Genero/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genero = await _context.Generos.FindAsync(id);
            if (genero == null)
            {
                return NotFound();
            }
            return View(genero);
        }

        // POST: Genero/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion")] Genero genero)
        {
            if (id != genero.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genero);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GeneroExists(genero.Id))
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
            return View(genero);
        }

        // GET: Genero/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genero = await _context.Generos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genero == null)
            {
                return NotFound();
            }

            return View(genero);
        }

        // POST: Genero/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genero = await _context.Generos.FindAsync(id);
            if (genero != null)
            {
                _context.Generos.Remove(genero);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GeneroExists(int id)
        {
            return _context.Generos.Any(e => e.Id == id);
        }
    }
}

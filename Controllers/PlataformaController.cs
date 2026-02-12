using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal4.Data;
using ProyectoFinal4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoFinal4.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PlataformaController : Controller
    {
        private readonly MovieDbContext _context;

        public PlataformaController(MovieDbContext context)
        {
            _context = context;
        }

        // GET: Plataforma
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
            var consulta = _context.Plataformas.AsQueryable();

            // Filtro por título (búsqueda)
            if (!string.IsNullOrEmpty(txtBusqueda))
            {
                consulta = consulta.Where(p => p.Nombre.Contains(txtBusqueda));
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

        // GET: Plataforma/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plataforma = await _context.Plataformas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plataforma == null)
            {
                return NotFound();
            }

            return View(plataforma);
        }

        // GET: Plataforma/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Plataforma/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Url,LogoUrl")] Plataforma plataforma)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plataforma);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(plataforma);
        }

        // GET: Plataforma/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plataforma = await _context.Plataformas.FindAsync(id);
            if (plataforma == null)
            {
                return NotFound();
            }
            return View(plataforma);
        }

        // POST: Plataforma/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Url,LogoUrl")] Plataforma plataforma)
        {
            if (id != plataforma.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plataforma);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlataformaExists(plataforma.Id))
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
            return View(plataforma);
        }

        // GET: Plataforma/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plataforma = await _context.Plataformas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plataforma == null)
            {
                return NotFound();
            }

            return View(plataforma);
        }

        // POST: Plataforma/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plataforma = await _context.Plataformas.FindAsync(id);
            if (plataforma != null)
            {
                _context.Plataformas.Remove(plataforma);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlataformaExists(int id)
        {
            return _context.Plataformas.Any(e => e.Id == id);
        }
    }
}

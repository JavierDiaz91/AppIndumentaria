using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppIndumentaria.Models;
using X.PagedList.Extensions;

namespace AppIndumentaria.Controllers
{
    public class CapacitacionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CapacitacionesController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Capacitaciones
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            ViewData["CurrentFilter"] = searchString; // Guardar la búsqueda para mantenerla en la vista

            var capacitaciones = from c in _context.Capacitaciones
                                 select c;

            // Filtrar capacitaciones según el término de búsqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                capacitaciones = capacitaciones.Where(c => c.Titulo.Contains(searchString) || c.Descripcion.Contains(searchString));
            }

            capacitaciones = capacitaciones.OrderBy(c => c.Titulo);

            int pageSize = 5;
            int pageNumber = page ?? 1;

            // Convertir a lista y paginar
            var pagedList = await capacitaciones.ToListAsync();
            var pagedResult = pagedList.ToPagedList(pageNumber, pageSize);

            return View(pagedResult);
        }


        // GET: Capacitaciones/Create
        public IActionResult Create()
        {
            // Puedes inicializar un modelo vacío si es necesario
            var model = new Capacitacion();

            return View(model);
        }


        // GET: Capacitaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var capacitacion = await _context.Capacitaciones
                .FirstOrDefaultAsync(m => m.CapacitacionID == id);
            if (capacitacion == null)
            {
                return NotFound();
            }

            return View(capacitacion);
        }
       
        // POST: Capacitaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CapacitacionID,Titulo,Descripcion,Fecha")] Capacitacion capacitacion)
        {
            // Eliminar la validación para la propiedad Participaciones
            ModelState.Remove("Participaciones");

            if (ModelState.IsValid)
            {
                _context.Add(capacitacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Para imprimir los errores, si persisten
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View(capacitacion);
        }

        // GET: Capacitaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var capacitacion = await _context.Capacitaciones.FindAsync(id);
            if (capacitacion == null)
            {
                return NotFound();
            }

            return View(capacitacion);
        }

        // POST: Capacitaciones/Edit/5      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CapacitacionID,Titulo,Descripcion,Fecha")] Capacitacion capacitacion)
        {
            if (id != capacitacion.CapacitacionID)
            {
                return NotFound();
            }

            ModelState.Remove("Participaciones"); // Ignora la propiedad de navegación

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(capacitacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Capacitaciones.Any(e => e.CapacitacionID == id))
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

            return View(capacitacion);
        }

        // GET: Capacitaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var capacitacion = await _context.Capacitaciones
                .FirstOrDefaultAsync(m => m.CapacitacionID == id);
            if (capacitacion == null)
            {
                return NotFound();
            }

            return View(capacitacion);
        }

        // POST: Capacitaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var capacitacion = await _context.Capacitaciones.FindAsync(id);
            if (capacitacion != null)
            {
                _context.Capacitaciones.Remove(capacitacion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CapacitacionExists(int id)
        {
            return _context.Capacitaciones.Any(e => e.CapacitacionID == id);
        }
    }
}

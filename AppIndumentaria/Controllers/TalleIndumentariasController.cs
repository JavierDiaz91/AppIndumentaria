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
    public class TalleIndumentariasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TalleIndumentariasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TalleIndumentarias
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 3;  
            int pageNumber = page ?? 1;  

            // Incluir la relación con Talle para obtener el nombre del Talle
            var tallesIndumentarias = _context.TalleIndumentarias
                .Include(ti => ti.Indumentaria)  
                .Include(ti => ti.Talle)        
                .OrderBy(ti => ti.Indumentaria.Nombre); 

            // Convertir a una lista paginada
            var pagedList = await tallesIndumentarias.ToListAsync();
            var pagedResult = pagedList.ToPagedList(pageNumber, pageSize);

            return View(pagedResult);
        }

        // GET: TalleIndumentarias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var talleIndumentaria = await _context.TalleIndumentarias
                .Include(t => t.Indumentaria)
                .Include(t => t.Talle)
                .FirstOrDefaultAsync(m => m.TalleIndumentariaID == id);

            if (talleIndumentaria == null)
            {
                return NotFound();
            }

            return View(talleIndumentaria);
        }


        public IActionResult Create()
        {
            // Cargar la lista de indumentarias disponibles
            ViewData["IndumentariaID"] = new SelectList(_context.Indumentarias, "IndumentariaID", "Nombre");

            // Cargar la lista de talles disponibles
            ViewData["TalleID"] = new SelectList(_context.Talles, "TalleID", "Nombre");

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IndumentariaID,TalleID,CantidadDisponible")] TalleIndumentaria talleIndumentaria)
        {
            ModelState.Remove("Indumentaria");
            ModelState.Remove("Talle");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(talleIndumentaria);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Hubo un error al intentar guardar los datos. Verifica que las relaciones sean correctas.");
                }
            }

            // Volver a cargar las listas necesarias en caso de error
            ViewData["IndumentariaID"] = new SelectList(_context.Indumentarias, "IndumentariaID", "Nombre", talleIndumentaria.IndumentariaID);
            ViewData["TalleID"] = new SelectList(_context.Talles, "TalleID", "Nombre", talleIndumentaria.TalleID);
            return View(talleIndumentaria);
        }

        // GET: TalleIndumentarias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var talleIndumentaria = await _context.TalleIndumentarias
                .Include(ti => ti.Indumentaria) // Incluimos la información de la Indumentaria
                .FirstOrDefaultAsync(ti => ti.TalleIndumentariaID == id);

            if (talleIndumentaria == null)
            {
                return NotFound();
            }

            // Asegurarse de cargar la lista de Indumentarias y Talles para la vista
            ViewData["IndumentariaID"] = new SelectList(_context.Indumentarias, "IndumentariaID", "Nombre", talleIndumentaria.IndumentariaID);
            ViewData["TalleID"] = new SelectList(_context.Talles, "TalleID", "Nombre", talleIndumentaria.TalleID);
            return View(talleIndumentaria);
        }

        // POST: TalleIndumentarias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TalleIndumentariaID,IndumentariaID,TalleID,CantidadDisponible")] TalleIndumentaria talleIndumentaria)
        {
            if (id != talleIndumentaria.TalleIndumentariaID)
            {
                return NotFound();
            }

            ModelState.Remove("Indumentaria");
            ModelState.Remove("Talle");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(talleIndumentaria);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TalleIndumentariaExists(talleIndumentaria.TalleIndumentariaID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Ocurrió un error inesperado al actualizar los datos: {ex.Message}");
                }
            }

            // Si el modelo no es válido, recargamos las listas necesarias
            ViewData["IndumentariaID"] = new SelectList(_context.Indumentarias, "IndumentariaID", "Nombre", talleIndumentaria.IndumentariaID);
            ViewData["TalleID"] = new SelectList(_context.Talles, "TalleID", "Nombre", talleIndumentaria.TalleID);
            return View(talleIndumentaria);
        }

        // GET: TalleIndumentarias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var talleIndumentaria = await _context.TalleIndumentarias
                .Include(t => t.Indumentaria)
                .Include(t => t.Talle) // Incluir información del talle para mayor contexto en la vista de eliminación.
                .FirstOrDefaultAsync(m => m.TalleIndumentariaID == id);

            if (talleIndumentaria == null)
            {
                return NotFound();
            }

            return View(talleIndumentaria);
        }

        // POST: TalleIndumentarias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var talleIndumentaria = await _context.TalleIndumentarias.FindAsync(id);
            if (talleIndumentaria == null)
            {
                return NotFound();
            }

            try
            {
                _context.TalleIndumentarias.Remove(talleIndumentaria);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Si hay un error, se muestra la página con el error
                ModelState.AddModelError("", $"Ocurrió un error inesperado al eliminar los datos: {ex.Message}");
                return View(talleIndumentaria); // Devuelve a la vista de eliminación si falla.
            }

            return RedirectToAction(nameof(Index));
        }

        // Método auxiliar para verificar si TalleIndumentaria existe
        private bool TalleIndumentariaExists(int id)
        {
            return _context.TalleIndumentarias.Any(e => e.TalleIndumentariaID == id);
        }
    }
}

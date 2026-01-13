using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppIndumentaria.Models;

namespace AppIndumentaria.Controllers
{
    public class ParticipacionCapacitacionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ParticipacionCapacitacionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ParticipacionCapacitaciones
        public async Task<IActionResult> Index(string searchString)
        {
            // Preparar la consulta de participaciones
            var participacionesQuery = _context.ParticipacionesCapacitaciones
                .Include(p => p.Capacitacion)
                .Include(p => p.Empleado)
                .AsQueryable();

            // Si hay una búsqueda, aplicamos el filtro correspondiente
            if (!string.IsNullOrEmpty(searchString))
            {
                participacionesQuery = participacionesQuery.Where(p =>
                    p.Empleado.Nombre.Contains(searchString) ||
                    p.Empleado.Apellido.Contains(searchString) ||
                    p.Capacitacion.Titulo.Contains(searchString));
            }

            // Convertir la consulta en una lista para pasarla a la vista
            var participacionesCapacitacion = await participacionesQuery.ToListAsync();

            return View(participacionesCapacitacion);
        }


        // GET: ParticipacionCapacitaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var participacionCapacitacion = await _context.ParticipacionesCapacitaciones
                .Include(p => p.Capacitacion)
                .Include(p => p.Empleado)
                .FirstOrDefaultAsync(m => m.ParticipacionCapacitacionID == id);
            if (participacionCapacitacion == null)
            {
                return NotFound();
            }

            return View(participacionCapacitacion);
        }

        // GET: ParticipacionCapacitaciones/Create
        public IActionResult Create()
        {
            var empleados = _context.Empleados.ToList();
            var capacitaciones = _context.Capacitaciones.ToList();

            if (empleados == null || capacitaciones == null)
            {
                return View("Error", new ErrorViewModel { RequestId = "Datos no disponibles" });
            }

            ViewBag.EmpleadoID = new SelectList(empleados, "EmpleadoID", "Nombre");
            ViewBag.CapacitacionID = new SelectList(capacitaciones, "CapacitacionID", "Titulo");

            // Inicializa la fecha de participación con la fecha actual
            var model = new ParticipacionCapacitacion
            {
                FechaParticipacion = DateTime.Now
            };

            return View(model);
        }


        // POST: ParticipacionCapacitaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ParticipacionCapacitacionID,EmpleadoID,CapacitacionID,FechaParticipacion,Estado")] ParticipacionCapacitacion participacionCapacitacion)
        {
            ModelState.Remove("Empleado");
            ModelState.Remove("Capacitacion");

            if (ModelState.IsValid)
            {
                _context.Add(participacionCapacitacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Volver a cargar las listas en caso de error en la validación
            ViewData["EmpleadoID"] = new SelectList(_context.Empleados, "EmpleadoID", "Nombre", participacionCapacitacion.EmpleadoID);
            ViewData["CapacitacionID"] = new SelectList(_context.Capacitaciones, "CapacitacionID", "Titulo", participacionCapacitacion.CapacitacionID);

            return View(participacionCapacitacion);
        }

        // GET: ParticipacionCapacitaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var participacionCapacitacion = await _context.ParticipacionesCapacitaciones
                .FirstOrDefaultAsync(p => p.ParticipacionCapacitacionID == id);

            if (participacionCapacitacion == null)
            {
                return NotFound();
            }

            // Asegúrate de que los ViewBag se llenan con los valores correctos para las listas desplegables
            ViewData["EmpleadoID"] = new SelectList(_context.Empleados.ToList(), "EmpleadoID", "Nombre", participacionCapacitacion.EmpleadoID);
            ViewData["CapacitacionID"] = new SelectList(_context.Capacitaciones.ToList(), "CapacitacionID", "Titulo", participacionCapacitacion.CapacitacionID);

            return View(participacionCapacitacion);
        }


        // POST: ParticipacionCapacitaciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ParticipacionCapacitacionID,EmpleadoID,CapacitacionID,FechaParticipacion,Estado")] ParticipacionCapacitacion participacionCapacitacion)
        {
            if (id != participacionCapacitacion.ParticipacionCapacitacionID)
            {
                return NotFound();
            }

            // Anular las propiedades de navegación del ModelState para evitar errores de validación
            ModelState.Remove("Empleado");
            ModelState.Remove("Capacitacion");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(participacionCapacitacion);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ParticipacionesCapacitaciones.Any(e => e.ParticipacionCapacitacionID == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Volver a cargar las listas desplegables en caso de error en la validación
            ViewData["EmpleadoID"] = new SelectList(_context.Empleados.ToList(), "EmpleadoID", "Nombre", participacionCapacitacion.EmpleadoID);
            ViewData["CapacitacionID"] = new SelectList(_context.Capacitaciones.ToList(), "CapacitacionID", "Titulo", participacionCapacitacion.CapacitacionID);

            return View(participacionCapacitacion);
        }


        // GET: ParticipacionCapacitaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var participacionCapacitacion = await _context.ParticipacionesCapacitaciones
                .Include(p => p.Capacitacion)
                .Include(p => p.Empleado)
                .FirstOrDefaultAsync(m => m.ParticipacionCapacitacionID == id);
            if (participacionCapacitacion == null)
            {
                return NotFound();
            }

            return View(participacionCapacitacion);
        }

        // POST: ParticipacionCapacitaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var participacionCapacitacion = await _context.ParticipacionesCapacitaciones.FindAsync(id);
            if (participacionCapacitacion != null)
            {
                _context.ParticipacionesCapacitaciones.Remove(participacionCapacitacion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ParticipacionCapacitacionExists(int id)
        {
            return _context.ParticipacionesCapacitaciones.Any(e => e.ParticipacionCapacitacionID == id);
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppIndumentaria.Models;
using X.PagedList.Extensions;
using X.PagedList;

namespace AppIndumentaria.Controllers
{
    public class IndumentariasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IndumentariasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Indumentarias
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            ViewData["CurrentFilter"] = searchString; // Guardar la búsqueda para mantenerla en la vista

            var indumentarias = from i in _context.Indumentarias
                                select i;

            // Filtrar indumentarias según el término de búsqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                indumentarias = indumentarias.Where(i => i.Nombre.Contains(searchString));
            }

            indumentarias = indumentarias.OrderBy(i => i.Nombre);

            int pageSize = 5;
            int pageNumber = page ?? 1;

            // Convertir a lista y paginar
            var pagedList = await indumentarias.ToListAsync();
            var pagedResult = pagedList.ToPagedList(pageNumber, pageSize);

            return View(pagedResult);
        }



        public IActionResult Details(int id)
        {
            var indumentaria = _context.Indumentarias
                .Include(i => i.TallesIndumentaria) 
                .ThenInclude(ti => ti.Talle) 
                .FirstOrDefault(i => i.IndumentariaID == id);

            if (indumentaria == null)
            {
                return NotFound();
            }

            return View(indumentaria);
        }


        // GET: Indumentarias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Indumentarias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IndumentariaID,Nombre,")] Indumentaria indumentaria)
        {
           
            ModelState.Remove("TallesIndumentaria");

            if (ModelState.IsValid)
            {
                _context.Add(indumentaria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(indumentaria);
        }


        // GET: Indumentarias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var indumentaria = await _context.Indumentarias.FindAsync(id);
            if (indumentaria == null)
            {
                return NotFound();
            }
            return View(indumentaria);
        }

        // POST: Indumentarias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IndumentariaID,Nombre,Talla,CantidadDisponible")] Indumentaria indumentaria)
        {
            if (id != indumentaria.IndumentariaID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(indumentaria);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IndumentariaExists(indumentaria.IndumentariaID))
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
            return View(indumentaria);
        }

        // GET: Indumentarias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var indumentaria = await _context.Indumentarias
                .FirstOrDefaultAsync(m => m.IndumentariaID == id);
            if (indumentaria == null)
            {
                return NotFound();
            }

            return View(indumentaria);
        }

        // POST: Indumentarias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var indumentaria = await _context.Indumentarias.FindAsync(id);
            if (indumentaria != null)
            {
                _context.Indumentarias.Remove(indumentaria);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IndumentariaExists(int id)
        {
            return _context.Indumentarias.Any(e => e.IndumentariaID == id);
        }
    }
}

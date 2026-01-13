using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppIndumentaria.Models;

public class TallesController : Controller
{
    private readonly ApplicationDbContext _context;

    public TallesController(ApplicationDbContext context)
    {
        _context = context;
    }
    // GET: Talles
    public async Task<IActionResult> Index()
    {
        return View(await _context.Talles.ToListAsync());
    }

    // GET: Talles/Details/5
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var talle = await _context.Talles
            .FirstOrDefaultAsync(m => m.TalleID == id); 

        if (talle == null)
        {
            return NotFound();
        }

        return View(talle);
    }

    // GET: Talles/Create
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TalleID,Nombre,Descripcion")] Talle talle)
    {
        // Genera un ID único si no se ha proporcionado uno
        if (string.IsNullOrEmpty(talle.TalleID))
        {
            // Puedes personalizar este patrón para generar el ID, por ejemplo usando un GUID
            talle.TalleID = Guid.NewGuid().ToString("N"); // Genera un identificador único sin guiones
        }
        ModelState.Remove("TalleID");
        ModelState.Remove("TallesIndumentaria");

        if (ModelState.IsValid)
        {
            try
            {
                _context.Add(talle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear el Talle: {ex.Message}");
            }
        }
        return View(talle);
    }



    // GET: Talles/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var talle = await _context.Talles.FindAsync(id);
        if (talle == null)
        {
            return NotFound();
        }

        return View(talle);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("TalleID,Nombre,Descripcion")] Talle talle) // Incluido 'Descripcion' si existe en el modelo.
    {
        if (id != talle.TalleID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(talle);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TalleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    // Añadir manejo de error de concurrencia si es necesario.
                    throw;
                }
            }
            catch (Exception ex)
            {
                // Añadir manejo de error general para identificar problemas.
                ModelState.AddModelError("", $"Ocurrió un error inesperado al actualizar el talle: {ex.Message}");
            }

            return RedirectToAction(nameof(Index));
        }

        return View(talle);
    }

    // GET: Talles/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var talle = await _context.Talles
            .FirstOrDefaultAsync(m => m.TalleID == id); // Cambié `talleId` a `id`, ya que ambos deben ser `string`.

        if (talle == null)
        {
            return NotFound();
        }

        return View(talle);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var talle = await _context.Talles.FindAsync(id);
        if (talle == null)
        {
            return NotFound();
        }

        try
        {
            _context.Talles.Remove(talle);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Añadir manejo de error para identificar problemas al eliminar.
            ModelState.AddModelError("", $"Ocurrió un error inesperado al eliminar el talle: {ex.Message}");
            return View(talle); // Retornar la vista si hubo un error.
        }

        return RedirectToAction(nameof(Index));
    }

    // Método auxiliar para verificar si el Talle existe.
    private bool TalleExists(string id)
    {
        return _context.Talles.Any(e => e.TalleID == id);
    }

}



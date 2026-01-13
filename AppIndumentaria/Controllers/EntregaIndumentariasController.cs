using AppIndumentaria.Models;
using AppIndumentaria.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using X.PagedList.Extensions;
using PdfSharp.Drawing;


namespace AppIndumentaria.Controllers
{
    public class EntregaIndumentariasController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public EntregaIndumentariasController(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        [HttpGet]
        public JsonResult GetTallesByIndumentaria(int indumentariaId)
        {
            var talles = _context.TalleIndumentarias
                .Where(ti => ti.IndumentariaID == indumentariaId)
                .Select(ti => new
                {
                    talleId = ti.TalleID,
                    nombre = ti.Talle.Nombre
                })
                .ToList();

            return Json(talles);
        }


        // GET: EntregaIndumentarias
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            // Almacenar el filtro de búsqueda actual para la vista
            ViewData["CurrentFilter"] = searchString;

            int pageSize = 5; // Número de elementos por página
            int pageNumber = (page ?? 1);

            // Consulta inicial con las relaciones necesarias
            var entregasIndumentaria = _context.EntregasIndumentaria
                .Include(e => e.Empleado)
                .Include(e => e.Indumentaria)
                .Include(e => e.Talle)
                .AsQueryable(); // Permitir modificaciones a la consulta

            // Si el término de búsqueda no está vacío, filtrar los resultados
            if (!string.IsNullOrEmpty(searchString))
            {
                entregasIndumentaria = entregasIndumentaria.Where(e =>
                    e.Empleado.Nombre.Contains(searchString) ||
                    e.Empleado.Apellido.Contains(searchString) ||
                    e.Indumentaria.Nombre.Contains(searchString) ||
                    e.Talle.Nombre.Contains(searchString));
            }

            // Ordenar los resultados y convertirlos a una lista para la paginación
            entregasIndumentaria = entregasIndumentaria.OrderBy(e => e.FechaEntrega);

            var pagedList = await entregasIndumentaria.ToListAsync();
            var pagedResult = pagedList.ToPagedList(pageNumber, pageSize);

            return View(pagedResult);
        }


        // GET: EntregaIndumentarias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entregaIndumentaria = await _context.EntregasIndumentaria
                .Include(e => e.Empleado)
                .Include(e => e.Indumentaria)
                .Include(e => e.Talle)  // Asegúrate de incluir el Talle
                .FirstOrDefaultAsync(m => m.EntregaIndumentariaID == id);

            if (entregaIndumentaria == null)
            {
                return NotFound();
            }

            return View(entregaIndumentaria);
        }


        public IActionResult Create()
        {
            ViewData["EmpleadoID"] = new SelectList(_context.Empleados, "EmpleadoID", "NombreCompleto");
            ViewData["IndumentariaID"] = new SelectList(_context.Indumentarias, "IndumentariaID", "Nombre");
            ViewData["TalleID"] = new SelectList(_context.Talles, "TalleID", "Nombre");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EntregaIndumentariaID,EmpleadoID,IndumentariaID,TalleID,FechaEntrega,CantidadEntregada")] EntregaIndumentaria entregaIndumentaria)
        {
            ModelState.Remove("Talle");

            if (ModelState.IsValid)
            {
                // Verificar si el talle existe y si tiene suficiente stock
                var indumentariaTalle = await _context.TalleIndumentarias
                    .Include(ti => ti.Talle)  // Incluimos la relación con Talle
                    .FirstOrDefaultAsync(ti => ti.IndumentariaID == entregaIndumentaria.IndumentariaID && ti.TalleID == entregaIndumentaria.TalleID);

                if (indumentariaTalle == null)
                {
                    ModelState.AddModelError("TalleID", "Debe seleccionar un talle válido para la indumentaria seleccionada.");
                    await CargarListasParaVistaCrear(entregaIndumentaria);
                    return View(entregaIndumentaria);
                }

                // Verificar el stock disponible
                if (entregaIndumentaria.CantidadEntregada <= 0)
                {
                    ModelState.AddModelError("CantidadEntregada", "La cantidad entregada debe ser mayor a cero.");
                    await CargarListasParaVistaCrear(entregaIndumentaria);
                    return View(entregaIndumentaria);
                }

                if (entregaIndumentaria.CantidadEntregada > indumentariaTalle.CantidadDisponible)
                {
                    ModelState.AddModelError("CantidadEntregada", $"No hay suficiente stock disponible. Solo quedan {indumentariaTalle.CantidadDisponible} unidades de este talle.");
                    await CargarListasParaVistaCrear(entregaIndumentaria);
                    return View(entregaIndumentaria);
                }

                // Actualizar el stock disponible
                indumentariaTalle.CantidadDisponible -= entregaIndumentaria.CantidadEntregada;

                // Guardar la entrega de indumentaria
                _context.Add(entregaIndumentaria);
                _context.Update(indumentariaTalle);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // En caso de que la validación falle, recargar las listas para la vista
            await CargarListasParaVistaCrear(entregaIndumentaria);
            return View(entregaIndumentaria);
        }



        // GET: EntregaIndumentarias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entregaIndumentaria = await _context.EntregasIndumentaria
                .Include(e => e.Empleado)
                .Include(e => e.Indumentaria)
                .FirstOrDefaultAsync(e => e.EntregaIndumentariaID == id);

            if (entregaIndumentaria == null)
            {
                return NotFound();
            }

            // Cargar las listas necesarias para la vista
            ViewData["EmpleadoID"] = new SelectList(_context.Empleados, "EmpleadoID", "NombreCompleto", entregaIndumentaria.EmpleadoID);
            ViewData["IndumentariaID"] = new SelectList(_context.Indumentarias, "IndumentariaID", "Nombre", entregaIndumentaria.IndumentariaID);

            // Cargar los talles correspondientes a la indumentaria seleccionada
            var talles = _context.TalleIndumentarias
                .Where(t => t.IndumentariaID == entregaIndumentaria.IndumentariaID)
                .Select(t => t.Talle)
                .ToList();

            ViewData["TalleID"] = new SelectList(talles, "TalleID", "Nombre", entregaIndumentaria.TalleID);

            return View(entregaIndumentaria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EntregaIndumentariaID,EmpleadoID,IndumentariaID,TalleID,FechaEntrega,CantidadEntregada")] EntregaIndumentaria entregaIndumentaria)
        {
            if (id != entregaIndumentaria.EntregaIndumentariaID)
            {
                return NotFound();
            }

            ModelState.Remove("Talle");

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener la entrega original antes de los cambios
                    var entregaOriginal = await _context.EntregasIndumentaria
                        .AsNoTracking()
                        .FirstOrDefaultAsync(e => e.EntregaIndumentariaID == id);

                    if (entregaOriginal == null)
                    {
                        return NotFound();
                    }

                    // Si el Talle ha cambiado, devolver el stock del talle original antes de validar el nuevo
                    if (entregaOriginal.TalleID != entregaIndumentaria.TalleID)
                    {
                        // Obtener el talle original y devolver la cantidad al stock
                        var talleOriginal = await _context.TalleIndumentarias
                            .FirstOrDefaultAsync(ti => ti.IndumentariaID == entregaOriginal.IndumentariaID && ti.TalleID == entregaOriginal.TalleID);

                        if (talleOriginal != null)
                        {
                            talleOriginal.CantidadDisponible += entregaOriginal.CantidadEntregada;
                        }

                        // Validar stock del nuevo talle seleccionado
                        var nuevoTalle = await _context.TalleIndumentarias
                            .FirstOrDefaultAsync(ti => ti.IndumentariaID == entregaIndumentaria.IndumentariaID && ti.TalleID == entregaIndumentaria.TalleID);

                        if (nuevoTalle == null)
                        {
                            ModelState.AddModelError("TalleID", "Debe seleccionar un talle válido para la indumentaria seleccionada.");
                            await CargarListasParaVistaEditar(entregaIndumentaria);
                            return View(entregaIndumentaria);
                        }

                        // Verificar si el stock es suficiente en el nuevo talle
                        if (entregaIndumentaria.CantidadEntregada > nuevoTalle.CantidadDisponible)
                        {
                            ModelState.AddModelError("CantidadEntregada", $"No hay suficiente stock disponible para este cambio. Solo quedan {nuevoTalle.CantidadDisponible} unidades de este talle.");
                            await CargarListasParaVistaEditar(entregaIndumentaria);
                            return View(entregaIndumentaria);
                        }

                        // Reducir el stock del nuevo talle seleccionado
                        nuevoTalle.CantidadDisponible -= entregaIndumentaria.CantidadEntregada;

                        // Actualizar en el contexto ambos talles (original y nuevo)
                        _context.Update(talleOriginal);
                        _context.Update(nuevoTalle);
                    }
                    else
                    {
                        // Si no cambió el talle, validar si se está incrementando la cantidad entregada
                        var talleActual = await _context.TalleIndumentarias
                            .FirstOrDefaultAsync(ti => ti.IndumentariaID == entregaIndumentaria.IndumentariaID && ti.TalleID == entregaIndumentaria.TalleID);

                        if (talleActual == null)
                        {
                            ModelState.AddModelError("TalleID", "Debe seleccionar un talle válido para la indumentaria seleccionada.");
                            await CargarListasParaVistaEditar(entregaIndumentaria);
                            return View(entregaIndumentaria);
                        }

                        // Calcular la diferencia de cantidad entregada
                        int diferenciaCantidad = entregaIndumentaria.CantidadEntregada - entregaOriginal.CantidadEntregada;

                        if (diferenciaCantidad > 0 && diferenciaCantidad > talleActual.CantidadDisponible)
                        {
                            ModelState.AddModelError("CantidadEntregada", $"No hay suficiente stock disponible para realizar este cambio. Solo quedan {talleActual.CantidadDisponible} unidades de este talle.");
                            await CargarListasParaVistaEditar(entregaIndumentaria);
                            return View(entregaIndumentaria);
                        }

                        // Actualizar el stock del talle actual
                        talleActual.CantidadDisponible -= diferenciaCantidad;

                        _context.Update(talleActual);
                    }

                    // Actualizar la entrega de indumentaria
                    _context.Update(entregaIndumentaria);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntregaIndumentariaExists(entregaIndumentaria.EntregaIndumentariaID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // En caso de que la validación falle, recargar las listas para la vista
            await CargarListasParaVistaEditar(entregaIndumentaria);
            return View(entregaIndumentaria);
        }

        // GET: EntregaIndumentarias/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entregaIndumentaria = _context.EntregasIndumentaria
                .Include(e => e.Empleado)
                .Include(e => e.Indumentaria)
                .Include(e => e.Talle)  // Asegurarse de incluir Talle para mostrarlo en la vista
                .FirstOrDefault(m => m.EntregaIndumentariaID == id);

            if (entregaIndumentaria == null)
            {
                return NotFound();
            }

            return View(entregaIndumentaria);
        }

        // POST: EntregaIndumentarias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entregaIndumentaria = await _context.EntregasIndumentaria
                .Include(e => e.Indumentaria)
                .Include(e => e.Talle)  // Incluir Talle para devolver la cantidad al stock correcto
                .FirstOrDefaultAsync(e => e.EntregaIndumentariaID == id);

            if (entregaIndumentaria != null)
            {
                // Buscar el registro de TalleIndumentaria correspondiente
                var indumentariaTalle = await _context.TalleIndumentarias
                    .FirstOrDefaultAsync(ti => ti.IndumentariaID == entregaIndumentaria.IndumentariaID && ti.TalleID == entregaIndumentaria.TalleID);

                if (indumentariaTalle == null)
                {
                    ModelState.AddModelError("", "No se encontró la indumentaria con el talle especificado.");
                    await CargarListasParaVistaEditar(entregaIndumentaria);
                    return View(entregaIndumentaria);
                }

                // Devolver la cantidad entregada al stock disponible del Talle correspondiente
                indumentariaTalle.CantidadDisponible += entregaIndumentaria.CantidadEntregada;

                // Eliminar la entrega de indumentaria
                _context.EntregasIndumentaria.Remove(entregaIndumentaria);
                // Actualizar el talle correspondiente
                _context.Update(indumentariaTalle);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        // Método para cargar las listas de selección en el ViewData
        private async Task CargarListasParaVistaCrear(EntregaIndumentaria entregaIndumentaria)
        {
            entregaIndumentaria ??= new EntregaIndumentaria(); // Proveer una instancia predeterminada si es null

            ViewData["EmpleadoID"] = new SelectList(await _context.Empleados.ToListAsync(), "EmpleadoID", "NombreCompleto", entregaIndumentaria?.EmpleadoID);
            ViewData["IndumentariaID"] = new SelectList(await _context.Indumentarias.ToListAsync(), "IndumentariaID", "Nombre", entregaIndumentaria?.IndumentariaID);
            ViewData["TalleID"] = new SelectList(
                await _context.TalleIndumentarias
                    .Where(t => t.IndumentariaID == entregaIndumentaria.IndumentariaID)
                    .Select(t => t.Talle)
                    .ToListAsync(), "TalleID", "Nombre", entregaIndumentaria?.TalleID);
        }


        private async Task CargarListasParaVistaEditar(EntregaIndumentaria entregaIndumentaria)
        {
            ViewData["EmpleadoID"] = new SelectList(await _context.Empleados.ToListAsync(), "EmpleadoID", "NombreCompleto", entregaIndumentaria?.EmpleadoID);
            ViewData["IndumentariaID"] = new SelectList(await _context.Indumentarias.ToListAsync(), "IndumentariaID", "Nombre", entregaIndumentaria?.IndumentariaID);

            // Cargar talles relacionados a la indumentaria seleccionada
            if (entregaIndumentaria?.IndumentariaID != null)
            {
                var tallesDisponibles = await _context.TalleIndumentarias
                    .Where(ti => ti.IndumentariaID == entregaIndumentaria.IndumentariaID)
                    .Select(ti => ti.Talle)
                    .ToListAsync();

                ViewData["TalleID"] = new SelectList(tallesDisponibles, "TalleID", "Nombre", entregaIndumentaria?.TalleID);
            }
            else
            {
                ViewData["TalleID"] = new SelectList(Enumerable.Empty<Talle>(), "TalleID", "Nombre");
            }
        }
        // Acción para seleccionar un empleado antes de generar el PDF
        public IActionResult SeleccionarEmpleadoParaPDF()
        {
            // Obtener la lista de empleados y mapearlos a un tipo más simple (EmpleadoSimpleViewModel)
            var empleados = _context.Empleados
                .Select(e => new EmpleadoSimpleViewModel
                {
                    EmpleadoID = e.EmpleadoID,
                    NombreCompleto = $"{e.Nombre} {e.Apellido}"
                }).ToList();

            // Crear el ViewModel con la lista de empleados
            var viewModel = new SeleccionarEmpleadoViewModel
            {
                Empleados = empleados
            };

            return View(viewModel);
        }
        public IActionResult GenerarEntregaIndumentariaPDF(int empleadoId)
        {
            var empleado = _context.Empleados
                .Where(e => e.EmpleadoID == empleadoId)
                .Select(e => new
                {
                    e.NombreCompleto,
                    e.Puesto,
                    Entregas = e.EntregasIndumentaria.Select(entrega => new
                    {
                        entrega.FechaEntrega,
                        entrega.CantidadEntregada,
                        IndumentariaNombre = entrega.Indumentaria.Nombre,
                        TalleDescripcion = entrega.Talle.Nombre + " / " + entrega.Talle.Descripcion, // Concatenar Nombre y Descripción
                        Certificacion = "NO"
                    }).ToList()
                }).FirstOrDefault();

            PdfDocument documento = new PdfDocument();
            PdfPage pagina = documento.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(pagina);
            XFont fontTitulo = new XFont("Verdana", 9);
            XFont fontTexto = new XFont("Verdana", 7);
            XFont fontTabla = new XFont("Verdana", 6.5);
            XPen pen = new XPen(XColors.Black, 0.5);

            int yPoint = 30;

            // Logo y título
            gfx.DrawRectangle(XPens.Gray, 20, yPoint, pagina.Width - 40, 60);
            gfx.DrawString("CONSTANCIA DE ENTREGA DE ROPA DE TRABAJO Y ELEMENTOS DE PROTECCIÓN PERSONAL", fontTitulo, XBrushes.Black, new XRect(20, yPoint, pagina.Width - 40, 20), XStringFormats.TopCenter);
            gfx.DrawString("Res.299/11", fontTexto, XBrushes.Black, new XRect(20, yPoint + 15, pagina.Width - 40, 20), XStringFormats.TopCenter);

            // Cargar el logo
            string logoPath = Path.Combine(_webHostEnvironment.WebRootPath, "image", "Iso_COLOR.png"); // Cambia el nombre y la ruta según la ubicación de tu logo
            if (System.IO.File.Exists(logoPath))
            {
                XImage logo = XImage.FromFile(logoPath);

                // Ajusta el tamaño del logo
                double logoWidth = 50;
                double logoHeight = 50;
                gfx.DrawImage(logo, 25, yPoint + 5, logoWidth, logoHeight);
            }
            else
            {
                // Si no se encuentra el logo, se puede optar por mostrar un espacio en blanco o un mensaje de error
                gfx.DrawString("Logo no encontrado", fontTexto, XBrushes.Red, 25, yPoint + 25);
            }
            yPoint += 70;

            // Información de la empresa
            string razonSocial = "Conectar S.R.L.";
            string cuit = "30-71775141-4";
            string direccion = "Av. Independencia 135";
            string localidad = "Sunchales";
            string cp = "2322";
            string provincia = "Santa Fe";

            // Información general
            gfx.DrawString("RAZÓN SOCIAL:", fontTexto, XBrushes.Black, 20, yPoint);
            gfx.DrawString(razonSocial, fontTexto, XBrushes.Black, 110, yPoint); 
            gfx.DrawString("C.U.I.T:", fontTexto, XBrushes.Black, pagina.Width - 120, yPoint); 
            gfx.DrawString(cuit, fontTexto, XBrushes.Black, pagina.Width - 60 - 30, yPoint);

            yPoint += 20;

            gfx.DrawString("DIRECCIÓN:", fontTexto, XBrushes.Black, 20, yPoint);
            gfx.DrawString(direccion, fontTexto, XBrushes.Black, 110, yPoint); // Agregar dirección
            gfx.DrawString("LOCALIDAD:", fontTexto, XBrushes.Black, 210, yPoint);
            gfx.DrawString(localidad, fontTexto, XBrushes.Black, 290, yPoint); // Agregar localidad
            gfx.DrawString("CP:", fontTexto, XBrushes.Black, 380, yPoint);
            gfx.DrawString(cp, fontTexto, XBrushes.Black, 400, yPoint); // Agregar CP
            gfx.DrawString("PROVINCIA:", fontTexto, XBrushes.Black, 460, yPoint);
            gfx.DrawString(provincia, fontTexto, XBrushes.Black, 540, yPoint); // Agregar provincia

            yPoint += 20;

            // Línea para "NOMBRE Y APELLIDO DEL TRABAJADOR" y "DNI"
            gfx.DrawString("NOMBRE Y APELLIDO DEL TRABAJADOR:", fontTexto, XBrushes.Black, 20, yPoint);
            gfx.DrawLine(pen, 200, yPoint + 10, 400, yPoint + 10); // Línea para el campo del nombre
            gfx.DrawString("D.N.I.:", fontTexto, XBrushes.Black, 410, yPoint);
            gfx.DrawLine(pen, 450, yPoint + 10, pagina.Width - 20, yPoint + 10); // Línea para el campo del DNI
            yPoint += 20;


            // Definir dimensiones para las celdas
            int halfWidth = (int)((pagina.Width - 40) / 2); // Dividir el ancho en dos columnas
            int cellHeight = 40; // Altura de las celdas

            // Primera celda: Puesto que desempeña el trabajador
            gfx.DrawRectangle(pen, 20, yPoint, halfWidth, cellHeight); // Dibujar rectángulo
            gfx.DrawString("Puesto que desempeña el trabajador:",
                fontTexto, XBrushes.Black,
                new XRect(25, yPoint + 10, halfWidth - 10, cellHeight), // Centrar el texto verticalmente
                XStringFormats.TopLeft);

            // Segunda celda: Elementos de protección personal
            gfx.DrawRectangle(pen, 20 + halfWidth, yPoint, halfWidth, cellHeight);
            gfx.DrawString("Elementos de protección personal necesarios para el trabajador, ",
                fontTexto, XBrushes.Black,
                new XRect(25 + halfWidth, yPoint + 5, halfWidth - 10, cellHeight / 2),
                XStringFormats.TopLeft);
            gfx.DrawString("según el puesto de trabajo:",
                fontTexto, XBrushes.Black,
                new XRect(25 + halfWidth, yPoint + 20, halfWidth - 10, cellHeight / 2),
                XStringFormats.TopLeft);


            yPoint += cellHeight;


            int paddingColumnas = 5;

            // Definir el ancho total disponible para la tabla
            double anchoTabla = pagina.Width - 40;

            // Ajustar anchos de las columnas proporcionalmente al ancho disponible
            int colProducto = (int)(anchoTabla * 0.2) - paddingColumnas;
            int colModelo = (int)(anchoTabla * 0.15) - paddingColumnas;
            int colMarca = (int)(anchoTabla * 0.15) - paddingColumnas;
            int colCertificacion = (int)(anchoTabla * 0.1) - paddingColumnas;
            int colCantidad = (int)(anchoTabla * 0.1) - paddingColumnas;
            int colFecha = (int)(anchoTabla * 0.15) - paddingColumnas;
            int colFirma = (int)(anchoTabla * 0.15);
            int rowHeight = 20;
            int headerPadding = 5;

            int maxFilas = 15;
            int fila = 0;

            // Dibujar encabezado de la tabla con separación adecuada
            gfx.DrawRectangle(pen, 20, yPoint, anchoTabla, rowHeight);
            gfx.DrawString("PRODUCTO", fontTabla, XBrushes.Black, new XRect(20 + paddingColumnas / 2, yPoint + headerPadding, colProducto, rowHeight), XStringFormats.Center);
            gfx.DrawString("TALLE / TIPO", fontTabla, XBrushes.Black, new XRect(20 + colProducto + paddingColumnas, yPoint + headerPadding, colModelo, rowHeight), XStringFormats.Center);
            gfx.DrawString("MARCA", fontTabla, XBrushes.Black, new XRect(20 + colProducto + colModelo + paddingColumnas, yPoint + headerPadding, colMarca, rowHeight), XStringFormats.Center);
            gfx.DrawString("CERTIFICACIÓN", fontTabla, XBrushes.Black, new XRect(20 + colProducto + colModelo + colMarca + paddingColumnas, yPoint + headerPadding, colCertificacion, rowHeight), XStringFormats.Center);
            gfx.DrawString("CANTIDAD", fontTabla, XBrushes.Black, new XRect(20 + colProducto + colModelo + colMarca + colCertificacion + paddingColumnas, yPoint + headerPadding, colCantidad, rowHeight), XStringFormats.Center);
            gfx.DrawString("FECHA DE ENTREGA", fontTabla, XBrushes.Black, new XRect(20 + colProducto + colModelo + colMarca + colCertificacion + colCantidad + paddingColumnas, yPoint + headerPadding, colFecha, rowHeight), XStringFormats.Center);
            gfx.DrawString("FIRMA DEL TRABAJADOR", fontTabla, XBrushes.Black, new XRect(20 + colProducto + colModelo + colMarca + colCertificacion + colCantidad + colFecha + paddingColumnas, yPoint + headerPadding, colFirma, rowHeight), XStringFormats.Center);

            yPoint += rowHeight;

            // Dibujar las filas con datos reales
            foreach (var entrega in empleado.Entregas)
            {
                if (fila >= maxFilas) break; // Limitar las filas visibles a maxFilas

                gfx.DrawRectangle(pen, 20, yPoint, anchoTabla, rowHeight);

                // Líneas divisorias verticales
                gfx.DrawLine(pen, 20 + colProducto, yPoint, 20 + colProducto, yPoint + rowHeight);
                gfx.DrawLine(pen, 20 + colProducto + colModelo, yPoint, 20 + colProducto + colModelo, yPoint + rowHeight);
                gfx.DrawLine(pen, 20 + colProducto + colModelo + colMarca, yPoint, 20 + colProducto + colModelo + colMarca, yPoint + rowHeight);
                gfx.DrawLine(pen, 20 + colProducto + colModelo + colMarca + colCertificacion, yPoint, 20 + colProducto + colModelo + colMarca + colCertificacion, yPoint + rowHeight);
                gfx.DrawLine(pen, 20 + colProducto + colModelo + colMarca + colCertificacion + colCantidad, yPoint, 20 + colProducto + colModelo + colMarca + colCertificacion + colCantidad, yPoint + rowHeight);
                gfx.DrawLine(pen, 20 + colProducto + colModelo + colMarca + colCertificacion + colCantidad + colFecha, yPoint, 20 + colProducto + colModelo + colMarca + colCertificacion + colCantidad + colFecha, yPoint + rowHeight);

                // Colocar datos en cada columna
                gfx.DrawString(entrega.IndumentariaNombre, fontTabla, XBrushes.Black, new XRect(20 + paddingColumnas / 2, yPoint + 5, colProducto, rowHeight), XStringFormats.CenterLeft);
                gfx.DrawString(entrega.TalleDescripcion, fontTabla, XBrushes.Black, new XRect(20 + colProducto + paddingColumnas, yPoint + 5, colModelo, rowHeight), XStringFormats.CenterLeft);
                gfx.DrawString("Pampero / SP", fontTabla, XBrushes.Black, new XRect(20 + colProducto + colModelo + paddingColumnas, yPoint + 5, colMarca, rowHeight), XStringFormats.CenterLeft);
                gfx.DrawString("SI / NO", fontTabla, XBrushes.Black, new XRect(20 + colProducto + colModelo + colMarca + paddingColumnas, yPoint + 5, colCertificacion, rowHeight), XStringFormats.Center);
                gfx.DrawString(entrega.CantidadEntregada.ToString(), fontTabla, XBrushes.Black, new XRect(20 + colProducto + colModelo + colMarca + colCertificacion + paddingColumnas, yPoint + 5, colCantidad, rowHeight), XStringFormats.Center);
                gfx.DrawString(entrega.FechaEntrega.ToShortDateString(), fontTabla, XBrushes.Black, new XRect(20 + colProducto + colModelo + colMarca + colCertificacion + colCantidad + paddingColumnas, yPoint + 5, colFecha, rowHeight), XStringFormats.Center);
                gfx.DrawString("", fontTabla, XBrushes.Black, new XRect(20 + colProducto + colModelo + colMarca + colCertificacion + colCantidad + colFecha + paddingColumnas, yPoint + 5, colFirma, rowHeight), XStringFormats.Center); // Espacio para la firma

                yPoint += rowHeight;
                fila++;
            }

            // Rellenar filas vacías hasta completar maxFilas
            while (fila < maxFilas)
            {
                gfx.DrawRectangle(pen, 20, yPoint, anchoTabla, rowHeight);

                // Líneas divisorias verticales
                gfx.DrawLine(pen, 20 + colProducto, yPoint, 20 + colProducto, yPoint + rowHeight);
                gfx.DrawLine(pen, 20 + colProducto + colModelo, yPoint, 20 + colProducto + colModelo, yPoint + rowHeight);
                gfx.DrawLine(pen, 20 + colProducto + colModelo + colMarca, yPoint, 20 + colProducto + colModelo + colMarca, yPoint + rowHeight);
                gfx.DrawLine(pen, 20 + colProducto + colModelo + colMarca + colCertificacion, yPoint, 20 + colProducto + colModelo + colMarca + colCertificacion, yPoint + rowHeight);
                gfx.DrawLine(pen, 20 + colProducto + colModelo + colMarca + colCertificacion + colCantidad, yPoint, 20 + colProducto + colModelo + colMarca + colCertificacion + colCantidad, yPoint + rowHeight);
                gfx.DrawLine(pen, 20 + colProducto + colModelo + colMarca + colCertificacion + colCantidad + colFecha, yPoint, 20 + colProducto + colModelo + colMarca + colCertificacion + colCantidad + colFecha, yPoint + rowHeight);

                // Dejar las celdas vacías
                yPoint += rowHeight;
                fila++;
            }

            int infoAdicionalHeight = 60; // Altura de la sección
            gfx.DrawRectangle(pen, 20, yPoint, pagina.Width - 40, infoAdicionalHeight);


            gfx.DrawString("Información adicional:", fontTabla, XBrushes.Black, new XRect(25, yPoint + 5, pagina.Width - 40, infoAdicionalHeight), XStringFormats.TopLeft);


            // Guardar y devolver el PDF
            using (var stream = new System.IO.MemoryStream())
            {
                documento.Save(stream, false);
                return File(stream.ToArray(), "application/pdf", $"Entrega_Indumentaria_{empleado.NombreCompleto}.pdf");
            }
        }

        // Método para verificar si la entrega existe
        private bool EntregaIndumentariaExists(int id)
        {
            return _context.EntregasIndumentaria.Any(e => e.EntregaIndumentariaID == id);
        }
    }
}

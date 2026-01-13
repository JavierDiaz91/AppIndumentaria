using System.Collections.Generic;
using AppIndumentaria.Models;

namespace AppIndumentaria.ViewModels
{
    public class SeleccionarEmpleadoViewModel
    {
        public int EmpleadoId { get; set; }
        public IEnumerable<EmpleadoSimpleViewModel> Empleados { get; set; }

        // Nueva propiedad para seleccionar la opción de impresión
        public string OpcionSeleccionada { get; set; }
        public IEnumerable<string> Opciones { get; set; } = new List<string> { "Indumentaria", "Cursos Tomados" };

        public DateTime FechaInicio { get; set; } = DateTime.Today.AddMonths(-1); // Último mes como predeterminado
        public DateTime FechaFin { get; set; } = DateTime.Today;
    }

    public class EmpleadoSimpleViewModel
    {
        public int EmpleadoID { get; set; }
        public string NombreCompleto { get; set; }
    }
}

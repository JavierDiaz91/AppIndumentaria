using System.ComponentModel.DataAnnotations.Schema;

namespace AppIndumentaria.Models
{
    public class Empleado
    {
        public int EmpleadoID { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Puesto { get; set; }
        public DateTime FechaIngreso { get; set; }
        // Propiedad calculada para el nombre completo
        public string NombreCompleto => $"{Nombre} {Apellido}";
        // Relaciones
        public ICollection<EntregaIndumentaria> EntregasIndumentaria { get; set; } = new List<EntregaIndumentaria>();
        public ICollection<ParticipacionCapacitacion> ParticipacionesCapacitaciones { get; set; } = new List<ParticipacionCapacitacion>();
    }

}

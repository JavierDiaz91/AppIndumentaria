namespace AppIndumentaria.Models
{
    public class ParticipacionCapacitacion
    {
        public int ParticipacionCapacitacionID { get; set; }
        public int EmpleadoID { get; set; }
        public int CapacitacionID { get; set; }
        public DateTime FechaParticipacion { get; set; }
        public string Estado { get; set; } 

        // Relaciones
        public Empleado Empleado { get; set; }
        public Capacitacion Capacitacion { get; set; }
    }
}

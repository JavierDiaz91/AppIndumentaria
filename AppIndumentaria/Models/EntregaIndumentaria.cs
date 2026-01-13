namespace AppIndumentaria.Models
{
    public class EntregaIndumentaria
    {
        public int EntregaIndumentariaID { get; set; }
        public int EmpleadoID { get; set; }
        public int IndumentariaID { get; set; }
        public string TalleID { get; set; } // Cambiado a string
        public DateTime FechaEntrega { get; set; }
        public int CantidadEntregada { get; set; }
        public bool Certificacion { get; set; }

        // Relaciones
        public Empleado? Empleado { get; set; }
        public Indumentaria? Indumentaria { get; set; }
        public Talle Talle { get; set; }
    }



}

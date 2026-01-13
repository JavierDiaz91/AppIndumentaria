namespace AppIndumentaria.Models
{
    public class TalleIndumentaria
    {
        public int TalleIndumentariaID { get; set; } // Clave primaria
        public int IndumentariaID { get; set; } // Clave foránea a Indumentaria
        public string TalleID { get; set; } // Cambiado a string

        public int CantidadDisponible { get; set; } // Para indicar cuántas prendas hay disponibles para este talle

        // Relaciones
        public Indumentaria Indumentaria { get; set; } // Relación con la entidad Indumentaria
        public Talle Talle { get; set; } // Relación con la entidad Talle
    }


}

namespace AppIndumentaria.Models
{
    public class TalleIndumentaria
    {
        public int TalleIndumentariaID { get; set; } 
        public int IndumentariaID { get; set; } 
        public string TalleID { get; set; } 

        public int CantidadDisponible { get; set; } 

        // Relaciones
        public Indumentaria Indumentaria { get; set; } 
        public Talle Talle { get; set; } 
    }


}

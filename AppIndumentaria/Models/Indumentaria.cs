namespace AppIndumentaria.Models
{
    public class Indumentaria
    {
        public int IndumentariaID { get; set; }
        public string Nombre { get; set; }

        public ICollection<TalleIndumentaria> TallesIndumentaria { get; set; } = new List<TalleIndumentaria>();
    }
}

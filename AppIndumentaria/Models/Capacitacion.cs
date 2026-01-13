using System.ComponentModel.DataAnnotations.Schema;

namespace AppIndumentaria.Models
{
    public class Capacitacion
    {
        public int CapacitacionID { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;

        [NotMapped]
        public ICollection<ParticipacionCapacitacion> Participaciones { get; set; } = new List<ParticipacionCapacitacion>();
    }

}

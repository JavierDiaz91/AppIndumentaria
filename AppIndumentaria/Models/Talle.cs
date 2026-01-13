using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppIndumentaria.Models
{
    public class Talle
    {
        [Key]
        [Required]
        public string TalleID { get; set; } // Ejemplo: "M", "G", "2MG", etc.

        [Required]
        public string Nombre { get; set; } // Ejemplo: "M"

        public string Descripcion { get; set; } // Ejemplo: "M chomba manga corta"

        // Relación con TalleIndumentaria
        public virtual ICollection<TalleIndumentaria> TallesIndumentaria { get; set; }
    }

}

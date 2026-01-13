using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; 

namespace AppIndumentaria.Models
{
    // 2. Cambiar la clase base de DbContext a IdentityDbContext
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Capacitacion> Capacitaciones { get; set; }
        public DbSet<EntregaIndumentaria> EntregasIndumentaria { get; set; }
        public DbSet<Indumentaria> Indumentarias { get; set; }
        public DbSet<ParticipacionCapacitacion> ParticipacionesCapacitaciones { get; set; }
        public DbSet<Talle> Talles { get; set; }
        public DbSet<TalleIndumentaria> TalleIndumentarias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TalleIndumentaria>()
                .HasOne(ti => ti.Talle)  // Relación con Talle.
                .WithMany(t => t.TallesIndumentaria)  // Un talle puede estar en múltiples TalleIndumentarias.
                .HasForeignKey(ti => ti.TalleID)  // TalleID como clave foránea.
                .OnDelete(DeleteBehavior.Cascade);

            // Configura la clave primaria si es necesario, por ejemplo:
            modelBuilder.Entity<Talle>()
                .Property(t => t.TalleID)
                .ValueGeneratedNever(); // Si quieres usar valores específicos como "M", "G", etc., para TalleID.
        }

    }
}
    

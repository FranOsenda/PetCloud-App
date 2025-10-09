using Microsoft.EntityFrameworkCore;
using PetCloud.Api.Models;

namespace PetCloud.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Dueno> Duenos { get; set; }
    public DbSet<Veterinario> Veterinarios { get; set; }
    public DbSet<Mascota> Mascotas { get; set; }
    public DbSet<Vacuna> Vacunas { get; set; }
    public DbSet<HistorialMedico> HistorialesMedicos { get; set; }
    public DbSet<Turno> Turnos { get; set; }
    public DbSet<Recordatorio> Recordatorios { get; set; }
    public DbSet<Gasto> Gastos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().HasIndex(u => u.CorreoElectronico).IsUnique();

            modelBuilder.Entity<Dueno>()
                .HasOne(o => o.Usuario)
                .WithOne()
                .HasForeignKey<Dueno>(o => o.UsuarioId);

            modelBuilder.Entity<Veterinario>()
                .HasOne(v => v.Usuario)
                .WithOne()
                .HasForeignKey<Veterinario>(v => v.UsuarioId);

            modelBuilder.Entity<Mascota>()
                .HasOne(p => p.Dueno)
                .WithMany(o => o.Mascotas)
                .HasForeignKey(p => p.DuenoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vacuna>()
                .HasOne(v => v.Mascota)
                .WithMany(p => p.Vacunas)
                .HasForeignKey(v => v.MascotaId);

            modelBuilder.Entity<HistorialMedico>()
                .HasOne(h => h.Mascota)
                .WithMany(p => p.HistorialesMedicos)
                .HasForeignKey(h => h.MascotaId);

            modelBuilder.Entity<Turno>()
                .HasOne(a => a.Mascota)
                .WithMany(p => p.Turnos)
                .HasForeignKey(a => a.MascotaId);

            modelBuilder.Entity<Recordatorio>()
                .HasOne(r => r.Mascota)
                .WithMany(p => p.Recordatorios)
                .HasForeignKey(r => r.MascotaId);

            modelBuilder.Entity<Gasto>()
                .HasOne(e => e.Mascota)
                .WithMany(p => p.Gastos)
                .HasForeignKey(e => e.MascotaId);
            // Especificar precisión para evitar truncamiento silencioso de decimal
            modelBuilder.Entity<Gasto>()
                .Property(e => e.Monto)
                .HasPrecision(18, 2);
        }
    }
}
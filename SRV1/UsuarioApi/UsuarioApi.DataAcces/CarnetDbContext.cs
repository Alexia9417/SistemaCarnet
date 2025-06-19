using Microsoft.EntityFrameworkCore;
using UsuarioApi.Models;

namespace UsuarioApi.DataAccess
{
    public class CarnetDbContext : DbContext
    {
        public CarnetDbContext(DbContextOptions<CarnetDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioTelefono> UsuarioTelefonos { get; set; }
        public DbSet<UsuarioCarrera> UsuarioCarreras { get; set; }
        public DbSet<UsuarioArea> UsuarioAreas { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Carrera> Carreras { get; set; }
        public DbSet<TipoUsuario> TipoUsuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USUARIO
            modelBuilder.Entity<Usuario>().ToTable("usuario");
            modelBuilder.Entity<Usuario>().HasKey(u => u.Email);

            // TELEFONOS
            modelBuilder.Entity<UsuarioTelefono>().ToTable("usuario_telefonos");
            modelBuilder.Entity<UsuarioTelefono>().HasKey(t => t.Numero);
            modelBuilder.Entity<UsuarioTelefono>().Property(t => t.Numero).HasColumnName("numero");
            modelBuilder.Entity<UsuarioTelefono>().Property(t => t.UsuarioEmail).HasColumnName("usuario_email");
            modelBuilder.Entity<UsuarioTelefono>()
                .HasOne(t => t.Usuario)
                .WithMany(u => u.Telefonos)
                .HasForeignKey(t => t.UsuarioEmail);

            // USUARIO-CARRERA
            modelBuilder.Entity<UsuarioCarrera>().ToTable("usuario_carreras");
            modelBuilder.Entity<UsuarioCarrera>().HasKey(uc => new { uc.UsuarioEmail, uc.CarreraId });
            modelBuilder.Entity<UsuarioCarrera>().Property(uc => uc.UsuarioEmail).HasColumnName("usuario_email");
            modelBuilder.Entity<UsuarioCarrera>().Property(uc => uc.CarreraId).HasColumnName("carrera_id");
            modelBuilder.Entity<UsuarioCarrera>()
                .HasOne(uc => uc.Usuario)
                .WithMany(u => u.Carreras)
                .HasForeignKey(uc => uc.UsuarioEmail);

            // USUARIO-AREA
            modelBuilder.Entity<UsuarioArea>().ToTable("usuario_areas");
            modelBuilder.Entity<UsuarioArea>().HasKey(ua => new { ua.UsuarioEmail, ua.AreaId });
            modelBuilder.Entity<UsuarioArea>().Property(ua => ua.UsuarioEmail).HasColumnName("usuario_email");
            modelBuilder.Entity<UsuarioArea>().Property(ua => ua.AreaId).HasColumnName("area_id");
            modelBuilder.Entity<UsuarioArea>()
                .HasOne(ua => ua.Usuario)
                .WithMany(u => u.Areas)
                .HasForeignKey(ua => ua.UsuarioEmail);

            // AREAS
            modelBuilder.Entity<Area>().ToTable("areas");
            modelBuilder.Entity<Area>().HasKey(a => a.Id);
            modelBuilder.Entity<Area>().Property(a => a.Id).HasColumnName("id");
            modelBuilder.Entity<Area>().Property(a => a.Nombre).HasColumnName("nombre");

            // CARRERAS
            modelBuilder.Entity<Carrera>().ToTable("carreras");
            modelBuilder.Entity<Carrera>().HasKey(c => c.Id);
            modelBuilder.Entity<Carrera>().Property(c => c.Id).HasColumnName("id");
            modelBuilder.Entity<Carrera>().Property(c => c.Nombre).HasColumnName("nombre");

            // TIPOS DE USUARIO
            modelBuilder.Entity<TipoUsuario>().ToTable("tipos_usuario");
            modelBuilder.Entity<TipoUsuario>().HasKey(t => t.Id);
            modelBuilder.Entity<TipoUsuario>().Property(t => t.Id).HasColumnName("id");
            modelBuilder.Entity<TipoUsuario>().Property(t => t.Nombre).HasColumnName("nombre");
        }
    }
}

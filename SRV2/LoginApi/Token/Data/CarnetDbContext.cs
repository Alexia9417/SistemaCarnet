using Microsoft.EntityFrameworkCore;
using loginapi.Models;

namespace loginapi.Data
{
    public class CarnetDbContext : DbContext
    {
        public CarnetDbContext(DbContextOptions<CarnetDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Token> Tokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().ToTable("usuario");
            modelBuilder.Entity<Token>().ToTable("tokens");

            modelBuilder.Entity<Usuario>().HasKey(u => u.Email);

            modelBuilder.Entity<Token>().HasKey(t => t.Id);
            modelBuilder.Entity<Token>().HasOne(t => t.Usuario).WithMany(u => u.Tokens).HasForeignKey(t => t.UsuarioEmail);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using ReservasColegio.Models;

namespace ReservasColegio.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Diretiva> Diretivas { get; set; }
        public DbSet<DiretivaPermissao> DiretivasPermissao { get; set; }
        public DbSet<Equipamento> Equipamentos { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<Permissao> Permissoes { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}

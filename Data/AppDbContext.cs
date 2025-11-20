using Microsoft.EntityFrameworkCore;
using VacinaApi.Models;

namespace VacinaApi.Data
{
  public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
  {
    public DbSet<Pessoa> Pessoas { get; set; }
    public DbSet<Vacina> Vacinas { get; set; }
    public DbSet<RegistroVacina> Registros { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      // Garante unicidade do CPF
      modelBuilder.Entity<Pessoa>().HasIndex(p => p.Cpf).IsUnique();
    }
  }
}

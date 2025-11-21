using Microsoft.EntityFrameworkCore;
using VacinaApi.Models;

namespace VacinaApi.Data
{
  public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
  {
    public DbSet<Person> Persons { get; set; }
    public DbSet<Vaccine> Vaccines { get; set; }
    public DbSet<VaccineRecord> Records { get; set; }
    public DbSet<VaccineCard> VaccineCards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      // Garante unicidade do CPF
      modelBuilder.Entity<Person>().HasIndex(p => p.Cpf).IsUnique();

      // Garante unicidade dos cartões de vacina
      modelBuilder.Entity<VaccineCard>().HasIndex(p => p.Name).IsUnique();
    }
  }
}

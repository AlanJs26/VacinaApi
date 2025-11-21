using System.Text.Json.Serialization;

namespace VacinaApi.Models
{
  public class Person
  {
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Cpf { get; set; } // Identificador único
  }

  public class Vaccine
  {
    public int Id { get; set; }
    public required string Name { get; set; } // Ex: BCG, Hepatite B
    public required string Manufacturer { get; set; }
  }

  public class VaccineRecord
  {
    public int Id { get; set; }
    public required int PersonId { get; set; }
    public required int VaccineId { get; set; }
    public required int VaccineCardId { get; set; }

    public required int Dose { get; set; } // 1 (1ª Dose), 2 (2ª Dose), 3 (3ª Dose), 4 (1º Reforço), 5 (2º Reforço)
    public required DateTime ApplicationDate { get; set; } = DateTime.Now;

    // Navegação
    public Person? Person { get; set; }
    public Vaccine? Vaccine { get; set; }
    public VaccineCard? VaccineCard { get; set; }
  }

  public class VaccineCard
  {
    public int Id { get; set; }
    public required string Name { get; set; }
  }
}

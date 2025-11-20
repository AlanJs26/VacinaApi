using System.Text.Json.Serialization;

namespace VacinaApi.Models
{
  public class Pessoa
  {
    public int Id { get; set; }
    public required string Nome { get; set; }
    public required string Cpf { get; set; } // Identificador único

    [JsonIgnore] // Evita ciclo no JSON
    public List<RegistroVacina> Historico { get; set; } = [];
  }

  public class Vacina
  {
    public int Id { get; set; }
    public required string Nome { get; set; } // Ex: BCG, Hepatite B
    public required string Fabricante { get; set; }
  }

  public class RegistroVacina
  {
    public int Id { get; set; }
    public int PessoaId { get; set; }
    public int VacinaId { get; set; }
    public required string Dose { get; set; } // 1ª Dose, 2ª Dose, Reforço
    public DateTime DataAplicacao { get; set; } = DateTime.Now;

    // Navegação
    public Pessoa? Pessoa { get; set; }
    public Vacina? Vacina { get; set; }
  }
}

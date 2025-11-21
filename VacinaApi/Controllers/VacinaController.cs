namespace VacinaApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VacinaApi.Data;
using VacinaApi.Models;
using VacinaApi.Utils;

[ApiController]
[Route("api")]
public class SistemaController(AppDbContext context) : ControllerBase
{
  private readonly AppDbContext _context = context;

  [HttpGet("cartoes")]
  public async Task<IActionResult> GetVaccineCard() => Ok(await _context.VaccineCards.ToListAsync());

  // 1. Cadastrar Cartao
  [HttpPost("cartoes")]
  public async Task<IActionResult> CreateVaccineCard([FromBody] VaccineCard vaccineCard)
  {
    if (vaccineCard.Name.Length == 0)
    {
      return BadRequest("Invalid name");
    }

    if (_context.VaccineCards.Where(p => p.Name == vaccineCard.Name).Any())
    {
      return BadRequest("Already exists another vaccine card with this name");
    }

    _context.VaccineCards.Add(vaccineCard);
    await _context.SaveChangesAsync();
    return CreatedAtAction(nameof(GetVaccineCard), new { id = vaccineCard.Id }, vaccineCard);
  }

  // 2. Remover cartão de vacina (Cascata)
  [HttpDelete("cartoes/{id}")]
  public async Task<IActionResult> DeleteVaccineCard(int id)
  {
    var vaccineCard = await _context.VaccineCards.FindAsync(id);
    if (vaccineCard == null) return NotFound();

    // O EF Core remove os registros associados se configurado, ou removemos manualmente
    var registros = _context.Records.Where(r => r.VaccineCardId == id);
    _context.Records.RemoveRange(registros);
    _context.VaccineCards.Remove(vaccineCard);

    await _context.SaveChangesAsync();
    return NoContent();
  }

  [HttpGet("pessoas")]
  public ActionResult<IEnumerable<Person>> GetPersons(
          [FromQuery] string? nome,
          [FromQuery] string? cpf)
  {
    // 1. Comece com a coleção completa de pessoas (ou com a query base)
    IQueryable<Person> query = _context.Persons.AsQueryable();

    // 2. Aplica o filtro de Nome se o parâmetro foi fornecido
    if (!string.IsNullOrWhiteSpace(nome))
    {
      // Usamos .Where() para aplicar a condição de filtro
      // .Contains() geralmente é usado para busca parcial (LIKE '%nome%')
      // Se quiser busca exata, use .Equals()
#pragma warning disable CA1862 // Use as sobrecargas do método 'StringComparison' para realizar comparações de strings que não diferenciam maiúsculas de minúsculas
#pragma warning disable RCS1155 // Use StringComparison when comparing strings
      // SQLite não suporta StringComparison com Contains
      query = query.Where(p => p.Name.ToLower().Contains(nome.ToLower()));
#pragma warning restore RCS1155 // Use StringComparison when comparing strings
#pragma warning restore CA1862 // Use as sobrecargas do método 'StringComparison' para realizar comparações de strings que não diferenciam maiúsculas de minúsculas
    }

    // 3. Aplica o filtro de CPF se o parâmetro foi fornecido
    if (!string.IsNullOrWhiteSpace(cpf))
    {
      // Busca exata por CPF
      query = query.Where(p => p.Cpf.Equals(cpf));
    }

    // 4. Executa a query e retorna o resultado
    // Se nenhum filtro foi fornecido, o resultado será a lista completa.
    var persons = query.ToList();

    if (persons.Count == 0)
    {
      return NotFound("Nenhuma pessoa encontrada com os critérios fornecidos.");
    }

    return Ok(persons);
  }

  // 1. Cadastrar Pessoa
  [HttpPost("pessoas")]
  public async Task<IActionResult> CreatePerson([FromBody] Person person)
  {

    if (!Utils.IsCPFValid(person.Cpf, out var error_message))
    {
      return BadRequest(error_message);
    }

    if (person.Name.Length == 0)
    {
      return BadRequest("Invalid name");
    }

    if (_context.Persons.Where(p => p.Cpf == person.Cpf).Any())
    {
      return BadRequest("A person with this cpf already exists");
    }

    _context.Persons.Add(person);
    await _context.SaveChangesAsync();
    return CreatedAtAction(nameof(GetPersonVaccineCard), new { id = person.Id }, person);
  }

  // 2. Remover Pessoa (Cascata)
  [HttpDelete("pessoas/{id}")]
  public async Task<IActionResult> DeletarPessoa(int id)
  {
    var pessoa = await _context.Persons.FindAsync(id);
    if (pessoa == null) return NotFound();

    // O EF Core remove os registros associados se configurado, ou removemos manualmente
    var registros = _context.Records.Where(r => r.PersonId == id);
    _context.Records.RemoveRange(registros);
    _context.Persons.Remove(pessoa);

    await _context.SaveChangesAsync();
    return NoContent();
  }

  [HttpGet("vacinas")]
  public async Task<IActionResult> ListarVacinas() => Ok(await _context.Vaccines.ToListAsync());

  // 3. Cadastrar Vacina (Metadata)
  [HttpPost("vacinas")]
  public async Task<IActionResult> CreateVaccine([FromBody] Vaccine vaccine)
  {
    if (vaccine.Name.Length == 0)
    {
      return BadRequest("Invalid name");
    }
    if (vaccine.Manufacturer.Length == 0)
    {
      return BadRequest("Invalid Manufacturer");
    }

    if (_context.Vaccines.Where(v => v.Manufacturer == vaccine.Manufacturer && v.Name == vaccine.Name).Any())
    {
      return BadRequest("Already exists a vaccine with the same name and manufacturer");
    }

    _context.Vaccines.Add(vaccine);
    await _context.SaveChangesAsync();
    return Ok(vaccine);
  }

  [HttpDelete("vacinas/{id}")]
  public async Task<IActionResult> DeletarVacina(int id)
  {
    var vacina = await _context.Vaccines.FindAsync(id);
    if (vacina == null) return NotFound();


    // O EF Core remove os registros associados se configurado
    var registros = _context.Records.Where(r => r.VaccineId == id);
    _context.Records.RemoveRange(registros);
    _context.Vaccines.Remove(vacina);

    await _context.SaveChangesAsync();
    return NoContent();
  }

  // 5. Consultar Cartões de uma pessoa
  [HttpGet("pessoas/{id}/cartoes")]
  public async Task<IActionResult> GetPersonVaccineCard(int id)
  {
#pragma warning disable CS8602 // Desreferência de uma referência possivelmente nula.
    var cartaoList = await _context.Records
    .Include(r => r.Vaccine)
    .Include(r => r.VaccineCard)
    .Where(r => r.PersonId == id)
    .ToListAsync();


    var cartao = cartaoList.GroupBy(r => r.VaccineCard,
        (vaccineCard, records) => new
        {
          VaccineCardId = vaccineCard.Id,
          vaccineCard.Name,
          Records = records.Select(r => new
          {
            VaccineRecordId = r.Id,
            r.Vaccine,
            r.Dose,
            Date = r.ApplicationDate.ToShortDateString()
          }
        )
        }
        );
#pragma warning restore CS8602 // Desreferência de uma referência possivelmente nula.

    return Ok(cartao);
  }

  // 4. Registrar Vacinação
  [HttpPost("vacinacao")]
  public async Task<IActionResult> RegistrarVacinacao([FromBody] VaccineRecord registro)
  {
    // Validação de Dose Simples
    int[] dosesValidas = [1, 2, 3, 4, 5];
    if (!dosesValidas.Contains(registro.Dose))
      return BadRequest("Invalid dose. Use: 1, 2, 3, 4 or 5");

    var vacina = await _context.Vaccines.FindAsync(registro.VaccineId);
    if (vacina == null) return NotFound("Provided vaccine does not exist");

    var pessoa = await _context.Persons.FindAsync(registro.PersonId);
    if (pessoa == null) return NotFound("Provided person does not exist");

    var vaccineCard = await _context.VaccineCards.FindAsync(registro.VaccineCardId);
    if (vaccineCard == null) return NotFound("Provided vaccine card does not exist");

    registro.Person = null;
    registro.Vaccine = null;
    registro.VaccineCard = null;

    _context.Records.Add(registro);
    await _context.SaveChangesAsync();
    return Ok(registro);
  }

  // 6. Excluir Registro de Vacina
  [HttpDelete("vacinacao/{id}")]
  public async Task<IActionResult> DeletarRegistro(int id)
  {
    var reg = await _context.Records.FindAsync(id);
    if (reg == null) return NotFound();

    _context.Records.Remove(reg);
    await _context.SaveChangesAsync();
    return NoContent();
  }
}

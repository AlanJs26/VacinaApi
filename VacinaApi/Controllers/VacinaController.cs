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
    return CreatedAtAction(nameof(GetCartao), new { id = person.Id }, person);
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

  [HttpGet("vacinas")]
  public async Task<IActionResult> ListarVacinas() => Ok(await _context.Vaccines.ToListAsync());

  [HttpDelete("vacinas/{id}")]
  public async Task<IActionResult> DeletarVacina(int id)
  {
    var vacina = await _context.Vaccines.FindAsync(id);
    if (vacina == null) return NotFound();

    _context.Vaccines.Remove(vacina);

    await _context.SaveChangesAsync();
    return NoContent();
  }

  [HttpGet("pessoas")]
  public async Task<IActionResult> ListarPessoas() => Ok(await _context.Persons.ToListAsync());

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

    _context.Records.Add(registro);
    await _context.SaveChangesAsync();
    return Ok(registro);
  }

  // 5. Consultar Cartão
  [HttpGet("pessoas/{id}/cartao")]
  public async Task<IActionResult> GetCartao(int id)
  {
#pragma warning disable CS8602 // Desreferência de uma referência possivelmente nula.
    var cartao = await _context.Records
    .Include(r => r.Vaccine)
    .Where(r => r.PersonId == id)
    .Select(r => new
    {
      r.Id,
      Vaccine = r.Vaccine.Name,
      r.Dose,
      Data = r.ApplicationDate.ToShortDateString()
    })
    .ToListAsync();
#pragma warning restore CS8602 // Desreferência de uma referência possivelmente nula.

    return Ok(cartao);
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

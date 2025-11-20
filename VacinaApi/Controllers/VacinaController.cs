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
  public async Task<IActionResult> CriarPessoa([FromBody] Pessoa pessoa)
  {

    if (!Utils.IsCPFValid(pessoa.Cpf, out var error_message))
    {
      return BadRequest(error_message);
    }

    if (pessoa.Nome.Length == 0)
    {
      return BadRequest("Nome inválido");
    }

    if (_context.Pessoas.Where(p => p.Cpf == pessoa.Cpf).Any())
    {
      return BadRequest($"Já existe uma pessoa com o cpf {pessoa.Cpf}");
    }

    _context.Pessoas.Add(pessoa);
    await _context.SaveChangesAsync();
    return CreatedAtAction(nameof(GetCartao), new { id = pessoa.Id }, pessoa);
  }

  // 2. Remover Pessoa (Cascata)
  [HttpDelete("pessoas/{id}")]
  public async Task<IActionResult> DeletarPessoa(int id)
  {
    var pessoa = await _context.Pessoas.FindAsync(id);
    if (pessoa == null) return NotFound();

    // O EF Core remove os registros associados se configurado, ou removemos manualmente
    var registros = _context.Registros.Where(r => r.PessoaId == id);
    _context.Registros.RemoveRange(registros);
    _context.Pessoas.Remove(pessoa);

    await _context.SaveChangesAsync();
    return NoContent();
  }

  // 3. Cadastrar Vacina (Metadata)
  [HttpPost("vacinas")]
  public async Task<IActionResult> CriarVacina([FromBody] Vacina vacina)
  {
    if (vacina.Nome.Length == 0)
    {
      return BadRequest("Nome inválido");
    }
    if (vacina.Fabricante.Length == 0)
    {
      return BadRequest("Fabricante inválido");
    }

    if (_context.Vacinas.Where(v => v.Fabricante == vacina.Fabricante && v.Nome == vacina.Nome).Any())
    {
      return BadRequest("Já existe uma vacina com mesmo nome e fabricante");
    }

    _context.Vacinas.Add(vacina);
    await _context.SaveChangesAsync();
    return Ok(vacina);
  }

  [HttpGet("vacinas")]
  public async Task<IActionResult> ListarVacinas() => Ok(await _context.Vacinas.ToListAsync());

  [HttpDelete("vacinas/{id}")]
  public async Task<IActionResult> DeletarVacina(int id)
  {
    var vacina = await _context.Vacinas.FindAsync(id);
    if (vacina == null) return NotFound();

    _context.Vacinas.Remove(vacina);

    await _context.SaveChangesAsync();
    return NoContent();
  }

  [HttpGet("pessoas")]
  public async Task<IActionResult> ListarPessoas() => Ok(await _context.Pessoas.ToListAsync());

  // 4. Registrar Vacinação
  [HttpPost("vacinacao")]
  public async Task<IActionResult> RegistrarVacinacao([FromBody] RegistroVacina registro)
  {
    // Validação de Dose Simples
    string[] dosesValidas = ["1ª Dose", "2ª Dose", "3ª Dose", "Reforço", "Dose Única"];
    if (!dosesValidas.Contains(registro.Dose))
      return BadRequest("Dose inválida. Use: 1ª Dose, 2ª Dose, etc.");

    var vacina = await _context.Vacinas.FindAsync(registro.VacinaId);
    if (vacina == null) return NotFound("Vacina informada não existe");

    var pessoa = await _context.Pessoas.FindAsync(registro.PessoaId);
    if (pessoa == null) return NotFound("Pessoa informada não existe");

    _context.Registros.Add(registro);
    await _context.SaveChangesAsync();
    return Ok(registro);
  }

  // 5. Consultar Cartão
  [HttpGet("pessoas/{id}/cartao")]
  public async Task<IActionResult> GetCartao(int id)
  {
#pragma warning disable CS8602 // Desreferência de uma referência possivelmente nula.
    var cartao = await _context.Registros
    .Include(r => r.Vacina)
    .Where(r => r.PessoaId == id)
    .Select(r => new
    {
      r.Id,
      Vacina = r.Vacina.Nome,
      r.Dose,
      Data = r.DataAplicacao.ToShortDateString()
    })
    .ToListAsync();
#pragma warning restore CS8602 // Desreferência de uma referência possivelmente nula.

    return Ok(cartao);
  }

  // 6. Excluir Registro de Vacina
  [HttpDelete("vacinacao/{id}")]
  public async Task<IActionResult> DeletarRegistro(int id)
  {
    var reg = await _context.Registros.FindAsync(id);
    if (reg == null) return NotFound();

    _context.Registros.Remove(reg);
    await _context.SaveChangesAsync();
    return NoContent();
  }
}

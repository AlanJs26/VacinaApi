using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VacinaApi.Data;
using VacinaApi.Models;

namespace VacinaApi.Controllers
{
  [ApiController]
  [Route("api")]
  public class SistemaController(AppDbContext context) : ControllerBase
  {
    private readonly AppDbContext _context = context;

    // 1. Cadastrar Pessoa
    [HttpPost("pessoas")]
    public async Task<IActionResult> CriarPessoa([FromBody] Pessoa pessoa)
    {
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

      _context.Registros.Add(registro);
      await _context.SaveChangesAsync();
      return Ok(registro);
    }

    // 5. Consultar Cartão
    [HttpGet("pessoas/{id}/cartao")]
    public async Task<IActionResult> GetCartao(int id)
    {
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
}

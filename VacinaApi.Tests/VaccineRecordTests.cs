using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VacinaApi.Controllers;
using VacinaApi.Data;
using VacinaApi.Models;
using Xunit;

namespace VacinaApi.Tests;

public class VaccineRecordTests
{
    private readonly AppDbContext _context;
    private readonly SistemaController _controller;

    public VaccineRecordTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _controller = new SistemaController(_context);

        // Seed database with necessary entities
        _context.Persons.Add(new Person { Id = 1, Name = "Test Person", Cpf = "12345678901" });
        _context.Vaccines.Add(new Vaccine { Id = 1, Name = "Test Vaccine", Manufacturer = "Test" });
        _context.VaccineCards.Add(new VaccineCard { Id = 1, Name = "Test Card" });
        _context.SaveChanges();
    }

    [Fact]
    public async Task RegistrarVacinacao_WithValidData_ReturnsOk()
    {
        // Arrange
        var newRecord = new VaccineRecord
        {
            PersonId = 1,
            VaccineId = 1,
            VaccineCardId = 1,
            Dose = 1,
            ApplicationDate = DateTime.Now
        };

        // Act
        var result = await _controller.RegistrarVacinacao(newRecord);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var record = Assert.IsType<VaccineRecord>(okResult.Value);
        Assert.Equal(1, record.Dose);
        Assert.True(await _context.Records.AnyAsync(r => r.Id == record.Id));
    }

    [Fact]
    public async Task RegistrarVacinacao_WithInvalidDose_ReturnsBadRequest()
    {
        // Arrange
        var newRecord = new VaccineRecord
        {
            Dose = 99,
            PersonId = 1,
            VaccineId = 1,
            VaccineCardId = 1,
            ApplicationDate = DateTime.Now
        };

        // Act
        var result = await _controller.RegistrarVacinacao(newRecord);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid dose. Use: 1, 2, 3, 4 or 5", badRequestResult.Value);
    }

    [Fact]
    public async Task RegistrarVacinacao_WithNonExistingVaccine_ReturnsNotFound()
    {
        // Arrange
        var newRecord = new VaccineRecord { Dose = 1, VaccineId = 99, PersonId = 1, VaccineCardId = 1, ApplicationDate = DateTime.Now };

        // Act
        var result = await _controller.RegistrarVacinacao(newRecord);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Provided vaccine does not exist", notFoundResult.Value);
    }

    [Fact]
    public async Task ListarVacinacao_ReturnsAllRecords()
    {
        // Arrange
        _context.Records.Add(new VaccineRecord { PersonId = 1, VaccineId = 1, VaccineCardId = 1, Dose = 1, ApplicationDate = DateTime.Now });
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.ListarVacinacao();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var records = Assert.IsAssignableFrom<System.Collections.IEnumerable>(okResult.Value);
        Assert.Single(records.Cast<object>());
    }

    [Fact]
    public async Task DeletarRegistro_WithExistingId_ReturnsNoContent()
    {
        // Arrange
        var record = new VaccineRecord { Id = 1, PersonId = 1, VaccineId = 1, VaccineCardId = 1, Dose = 1, ApplicationDate = DateTime.Now };
        _context.Records.Add(record);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DeletarRegistro(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.False(await _context.Records.AnyAsync(r => r.Id == 1));
    }

    [Fact]
    public async Task DeletarRegistro_WithNonExistingId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.DeletarRegistro(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}

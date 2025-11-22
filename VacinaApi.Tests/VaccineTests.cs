using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VacinaApi.Controllers;
using VacinaApi.Data;
using VacinaApi.Models;
using Xunit;

namespace VacinaApi.Tests;

public class VaccineTests
{
    private readonly AppDbContext _context;
    private readonly SistemaController _controller;

    public VaccineTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _controller = new SistemaController(_context);
    }

    [Fact]
    public async Task CreateVaccine_WithValidData_ReturnsOk()
    {
        // Arrange
        var newVaccine = new Vaccine { Name = "COVID-19", Manufacturer = "Pfizer" };

        // Act
        var result = await _controller.CreateVaccine(newVaccine);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var vaccine = Assert.IsType<Vaccine>(okResult.Value);
        Assert.Equal("COVID-19", vaccine.Name);
        Assert.True(await _context.Vaccines.AnyAsync(v => v.Name == "COVID-19"));
    }

    [Fact]
    public async Task CreateVaccine_WithEmptyName_ReturnsBadRequest()
    {
        // Arrange
        var newVaccine = new Vaccine { Name = "", Manufacturer = "Pfizer" };

        // Act
        var result = await _controller.CreateVaccine(newVaccine);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid name", badRequestResult.Value);
    }

    [Fact]
    public async Task CreateVaccine_WithExistingData_ReturnsBadRequest()
    {
        // Arrange
        _context.Vaccines.Add(new Vaccine { Name = "COVID-19", Manufacturer = "Pfizer" });
        await _context.SaveChangesAsync();
        var newVaccine = new Vaccine { Name = "COVID-19", Manufacturer = "Pfizer" };

        // Act
        var result = await _controller.CreateVaccine(newVaccine);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Already exists a vaccine with the same name and manufacturer", badRequestResult.Value);
    }

    [Fact]
    public async Task ListarVacinas_ReturnsAllVaccines()
    {
        // Arrange
        _context.Vaccines.AddRange(
            new Vaccine { Name = "Flu", Manufacturer = "GSK" },
            new Vaccine { Name = "Tetanus", Manufacturer = "Sanofi" }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.ListarVacinas();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var vaccines = Assert.IsAssignableFrom<IEnumerable<Vaccine>>(okResult.Value);
        Assert.Equal(2, vaccines.Count());
    }

    [Fact]
    public async Task DeletarVacina_WithExistingId_ReturnsNoContent()
    {
        // Arrange
        var vaccine = new Vaccine { Id = 1, Name = "To be deleted", Manufacturer = "Test" };
        _context.Vaccines.Add(vaccine);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DeletarVacina(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.False(await _context.Vaccines.AnyAsync(v => v.Id == 1));
    }

    [Fact]
    public async Task DeletarVacina_WithNonExistingId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.DeletarVacina(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}

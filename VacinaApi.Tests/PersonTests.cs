using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VacinaApi.Controllers;
using VacinaApi.Data;
using VacinaApi.Models;
using Xunit;

namespace VacinaApi.Tests;

public class PersonTests
{
    private AppDbContext _context;
    private SistemaController _controller;

    public PersonTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _controller = new SistemaController(_context);
    }

    [Fact]
    public async Task CreatePerson_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var newPerson = new Person { Name = "John Doe", Cpf = "52998224725" };

        // Act
        var result = await _controller.CreatePerson(newPerson);

        // Assert
        var actionResult = Assert.IsType<CreatedAtActionResult>(result);
        var person = Assert.IsType<Person>(actionResult.Value);
        Assert.Equal("John Doe", person.Name);
        Assert.True(await _context.Persons.AnyAsync(p => p.Cpf == "52998224725"));
    }

    [Fact]
    public async Task CreatePerson_WithInvalidCPF_ReturnsBadRequest()
    {
        // Arrange
        var newPerson = new Person { Name = "John Doe", Cpf = "111" };

        // Act
        var result = await _controller.CreatePerson(newPerson);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("CPF must have 11 digits", badRequestResult.Value);
    }

    [Fact]
    public async Task CreatePerson_WithExistingCPF_ReturnsBadRequest()
    {
        // Arrange
        var existingPerson = new Person { Name = "Jane Doe", Cpf = "52998224725" };
        _context.Persons.Add(existingPerson);
        await _context.SaveChangesAsync();

        var newPerson = new Person { Name = "John Doe", Cpf = "52998224725" };

        // Act
        var result = await _controller.CreatePerson(newPerson);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("A person with this cpf already exists", badRequestResult.Value);
    }

    [Fact]
    public async Task GetPersons_ReturnsAllPersons()
    {
        // Arrange
        _context.Persons.AddRange(
            new Person { Name = "Alice", Cpf = "11111111111" },
            new Person { Name = "Bob", Cpf = "22222222222" }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = _controller.GetPersons(null, null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var persons = Assert.IsAssignableFrom<IEnumerable<Person>>(okResult.Value);
        Assert.Equal(2, persons.Count());
    }

    [Fact]
    public async Task GetPersons_WithCpfFilter_ReturnsMatchingPerson()
    {
        // Arrange
        _context.Persons.AddRange(
            new Person { Name = "Alice", Cpf = "11111111111" },
            new Person { Name = "Bob", Cpf = "22222222222" }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = _controller.GetPersons(null, "11111111111");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var persons = Assert.IsAssignableFrom<IEnumerable<Person>>(okResult.Value);
        Assert.Single(persons);
        Assert.Equal("Alice", persons.First().Name);
    }

    [Fact]
    public async Task DeletePerson_WithExistingId_ReturnsNoContent()
    {
        // Arrange
        var person = new Person { Id = 1, Name = "Test Person", Cpf = "33333333333" };
        _context.Persons.Add(person);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DeletarPessoa(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.False(await _context.Persons.AnyAsync(p => p.Id == 1));
    }

    [Fact]
    public async Task DeletePerson_WithNonExistingId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.DeletarPessoa(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VacinaApi.Controllers;
using VacinaApi.Data;
using VacinaApi.Models;
using Xunit;

namespace VacinaApi.Tests;

public class VaccineCardTests
{
    private readonly AppDbContext _context;
    private readonly SistemaController _controller;

    public VaccineCardTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _controller = new SistemaController(_context);
    }

    [Fact]
    public async Task CreateVaccineCard_WithValidName_ReturnsCreatedAtAction()
    {
        // Arrange
        var newCard = new VaccineCard { Name = "Adult" };

        // Act
        var result = await _controller.CreateVaccineCard(newCard);

        // Assert
        var actionResult = Assert.IsType<CreatedAtActionResult>(result);
        var card = Assert.IsType<VaccineCard>(actionResult.Value);
        Assert.Equal("Adult", card.Name);
        Assert.True(await _context.VaccineCards.AnyAsync(vc => vc.Name == "Adult"));
    }

    [Fact]
    public async Task CreateVaccineCard_WithEmptyName_ReturnsBadRequest()
    {
        // Arrange
        var newCard = new VaccineCard { Name = "" };

        // Act
        var result = await _controller.CreateVaccineCard(newCard);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid name", badRequestResult.Value);
    }

    [Fact]
    public async Task CreateVaccineCard_WithExistingName_ReturnsBadRequest()
    {
        // Arrange
        _context.VaccineCards.Add(new VaccineCard { Name = "Adult" });
        await _context.SaveChangesAsync();
        var newCard = new VaccineCard { Name = "Adult" };

        // Act
        var result = await _controller.CreateVaccineCard(newCard);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Already exists another vaccine card with this name", badRequestResult.Value);
    }

    [Fact]
    public async Task GetVaccineCard_ReturnsAllCards()
    {
        // Arrange
        _context.VaccineCards.AddRange(
            new VaccineCard { Name = "Adult" },
            new VaccineCard { Name = "Child" }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetVaccineCard();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var cards = Assert.IsAssignableFrom<IEnumerable<VaccineCard>>(okResult.Value);
        Assert.Equal(2, cards.Count());
    }

    [Fact]
    public async Task DeleteVaccineCard_WithExistingId_ReturnsNoContent()
    {
        // Arrange
        var card = new VaccineCard { Id = 1, Name = "To be deleted" };
        _context.VaccineCards.Add(card);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DeleteVaccineCard(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.False(await _context.VaccineCards.AnyAsync(vc => vc.Id == 1));
    }

    [Fact]
    public async Task DeleteVaccineCard_WithNonExistingId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.DeleteVaccineCard(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}

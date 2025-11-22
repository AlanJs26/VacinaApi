namespace VacinaApi.Tests;

using VacinaApi.Utils;
using Xunit;

public class CpfValidationTests
{
    [Theory]
    [InlineData("11111111111")]
    [InlineData("22222222222")]
    [InlineData("33333333333")]
    public void IsCPFValid_ShouldReturnFalse_WhenAllDigitsAreTheSame(string cpf)
    {
        var status = Utils.IsCPFValid(cpf, out var errorMessage);
        Assert.False(status);
        Assert.Equal("Invalid CPF", errorMessage);
    }

    [Theory]
    [InlineData("52998224725")]
    [InlineData("43813879100")]
    public void IsCPFValid_ShouldReturnTrue_ForValidCpfs(string cpf)
    {
        var status = Utils.IsCPFValid(cpf, out var errorMessage);
        Assert.True(status);
        Assert.Empty(errorMessage);
    }

    [Fact]
    public void IsCPFValid_ShouldReturnFalse_ForInvalidCpf()
    {
        var status = Utils.IsCPFValid("43813879101", out var errorMessage);
        Assert.False(status);
        Assert.Equal("Invalid CPF", errorMessage);
    }

    [Fact]
    public void IsCPFValid_ShouldReturnFalse_WhenCpfContainsLetters()
    {
        var status = Utils.IsCPFValid("43a13879101", out var errorMessage);
        Assert.False(status);
        Assert.Equal("CPF must contain only numbers", errorMessage);
    }

    [Fact]
    public void IsCPFValid_ShouldReturnFalse_WhenCpfHasIncorrectLength()
    {
        var status = Utils.IsCPFValid("413879101", out var errorMessage);
        Assert.False(status);
        Assert.Equal("CPF must have 11 digits", errorMessage);
    }
}

using Xunit;
using EFinanceira.Messages.Builders.Eventos;
using EFinanceira.Messages.Builders.Lotes;

namespace EFinanceira.Tests;

/// <summary>
/// Tests for message builders functionality.
/// </summary>
public class BuildersTests
{
    [Fact]
    public void LeiauteAberturaBuilder_Constructor_ShouldCreateInstance()
    {
        // Arrange & Act
        var builder = new LeiauteAberturaBuilder();

        // Assert
        Assert.NotNull(builder);
    }

    [Fact]
    public void LeiauteAberturaBuilder_IsValid_ShouldReturnTrue()
    {
        // Arrange
        var builder = new LeiauteAberturaBuilder();

        // Act
        var isValid = builder.IsValid();

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void LeiauteAberturaBuilder_Build_ShouldReturnMessage()
    {
        // Arrange
        var builder = new LeiauteAberturaBuilder();

        // Act
        var message = builder.Build();

        // Assert
        Assert.NotNull(message);
    }

    [Fact]
    public void LeiauteAberturaBuilder_GetValidationErrors_ShouldReturnEmpty()
    {
        // Arrange
        var builder = new LeiauteAberturaBuilder();

        // Act
        var errors = builder.GetValidationErrors();

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public void EnvioLoteEventosV120Builder_Constructor_ShouldCreateInstance()
    {
        // Arrange & Act
        var builder = new EnvioLoteEventosV120Builder();

        // Assert
        Assert.NotNull(builder);
    }

    [Fact]
    public void EnvioLoteEventosV120Builder_IsValid_ShouldReturnTrue()
    {
        // Arrange
        var builder = new EnvioLoteEventosV120Builder();

        // Act
        var isValid = builder.IsValid();

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void EnvioLoteEventosV120Builder_Build_ShouldReturnMessage()
    {
        // Arrange
        var builder = new EnvioLoteEventosV120Builder();

        // Act
        var message = builder.Build();

        // Assert
        Assert.NotNull(message);
    }

    [Fact]
    public void LeiauteInfoEmpresaDeclaranteBuilder_Constructor_ShouldCreateInstance()
    {
        // Arrange & Act
        var builder = new LeiauteInfoEmpresaDeclaranteBuilder();

        // Assert
        Assert.NotNull(builder);
    }

    [Fact]
    public void LeiauteInfoEmpresaDeclaranteBuilder_Build_ShouldReturnMessage()
    {
        // Arrange
        var builder = new LeiauteInfoEmpresaDeclaranteBuilder();

        // Act
        var message = builder.Build();

        // Assert
        Assert.NotNull(message);
    }
}
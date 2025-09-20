using Xunit;
using EFinanceira.Core.Validation;

namespace EFinanceira.Tests;

/// <summary>
/// Tests for XML validation functionality.
/// </summary>
public class ValidationTests
{
    [Fact]
    public void XmlValidator_Constructor_ShouldCreateInstance()
    {
        // Arrange & Act
        var validator = new XmlValidator();

        // Assert
        Assert.NotNull(validator);
    }

    [Fact]
    public void ValidateXml_WithNullXml_ShouldReturnFalse()
    {
        // Arrange
        var validator = new XmlValidator();

        // Act
        var result = validator.ValidateXml(null!, "schema.xsd");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateXml_WithEmptyXml_ShouldReturnFalse()
    {
        // Arrange
        var validator = new XmlValidator();

        // Act
        var result = validator.ValidateXml("", "schema.xsd");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateXml_WithNonExistentSchema_ShouldReturnFalse()
    {
        // Arrange
        var validator = new XmlValidator();
        var xml = "<root>test</root>";

        // Act
        var result = validator.ValidateXml(xml, "nonexistent.xsd");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateXmlWithErrors_WithNullXml_ShouldReturnErrors()
    {
        // Arrange
        var validator = new XmlValidator();

        // Act
        var errors = validator.ValidateXmlWithErrors(null!, "schema.xsd");

        // Assert
        Assert.NotEmpty(errors);
        Assert.Contains("XML content cannot be null or empty", errors);
    }

    [Fact]
    public void ValidateXmlWithErrors_WithNonExistentSchema_ShouldReturnErrors()
    {
        // Arrange
        var validator = new XmlValidator();
        var xml = "<root>test</root>";

        // Act
        var errors = validator.ValidateXmlWithErrors(xml, "nonexistent.xsd");

        // Assert
        Assert.NotEmpty(errors);
        Assert.True(errors.Any(e => e.Contains("Schema file not found")));
    }

    [Fact]
    public void ValidateMessage_WithNullMessage_ShouldReturnFalse()
    {
        // Arrange
        var validator = new XmlValidator();

        // Act
        var result = validator.ValidateMessage(null!);

        // Assert
        Assert.False(result);
    }
}
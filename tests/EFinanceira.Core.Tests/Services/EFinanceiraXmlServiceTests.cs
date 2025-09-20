using EFinanceira.Core.Exceptions;
using EFinanceira.Core.Models;
using EFinanceira.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace EFinanceira.Core.Tests.Services;

public class EFinanceiraXmlServiceTests
{
    private readonly Mock<ILogger<EFinanceiraXmlService>> _loggerMock;
    private readonly EFinanceiraXmlService _xmlService;

    public EFinanceiraXmlServiceTests()
    {
        _loggerMock = new Mock<ILogger<EFinanceiraXmlService>>();
        _xmlService = new EFinanceiraXmlService(_loggerMock.Object);
    }

    [Fact]
    public async Task SerializeAsync_WithValidEnvelope_ShouldReturnXml()
    {
        // Arrange
        var envelope = CreateValidEnvelope();

        // Act
        var result = await _xmlService.SerializeAsync(envelope);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        result.Should().Contain("envEvtEFinanceira");
        result.Should().Contain("ideEvento");
        result.Should().Contain("ideRespons");
    }

    [Fact]
    public async Task SerializeAsync_WithNullEnvelope_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = async () => await _xmlService.SerializeAsync(null!);
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task DeserializeAsync_WithValidXml_ShouldReturnEnvelope()
    {
        // Arrange
        var originalEnvelope = CreateValidEnvelope();
        var xml = await _xmlService.SerializeAsync(originalEnvelope);

        // Act
        var result = await _xmlService.DeserializeAsync(xml);

        // Assert
        result.Should().NotBeNull();
        result.IdeEvento.Should().NotBeNull();
        result.IdeRespons.Should().NotBeNull();
        result.IdeEvento.CnpjRespons.Should().Be(originalEnvelope.IdeEvento.CnpjRespons);
        result.IdeRespons.CnpjRespons.Should().Be(originalEnvelope.IdeRespons.CnpjRespons);
    }

    [Fact]
    public async Task DeserializeAsync_WithInvalidXml_ShouldThrowEFinanceiraSerializationException()
    {
        // Arrange
        var invalidXml = "<invalid>xml</invalid>";

        // Act & Assert
        var action = async () => await _xmlService.DeserializeAsync(invalidXml);
        await action.Should().ThrowAsync<EFinanceiraSerializationException>();
    }

    [Fact]
    public async Task DeserializeAsync_WithNullOrEmptyXml_ShouldThrowArgumentException()
    {
        // Act & Assert
        var actionNull = async () => await _xmlService.DeserializeAsync(null!);
        var actionEmpty = async () => await _xmlService.DeserializeAsync("");
        var actionWhitespace = async () => await _xmlService.DeserializeAsync("   ");

        await actionNull.Should().ThrowAsync<ArgumentException>();
        await actionEmpty.Should().ThrowAsync<ArgumentException>();
        await actionWhitespace.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ValidateXmlAsync_WithValidXml_ShouldReturnNoErrors()
    {
        // Arrange
        var envelope = CreateValidEnvelope();
        var xml = await _xmlService.SerializeAsync(envelope);

        // Act
        var errors = await _xmlService.ValidateXmlAsync(xml);

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateXmlAsync_WithInvalidXml_ShouldReturnErrors()
    {
        // Arrange
        var invalidXml = "<invalid>xml<invalid>";

        // Act
        var errors = await _xmlService.ValidateXmlAsync(invalidXml);

        // Assert
        errors.Should().NotBeEmpty();
        errors.Should().Contain(e => e.Contains("XML mal formado"));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new EFinanceiraXmlService(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    private static EFinanceiraEnvelope CreateValidEnvelope()
    {
        return new EFinanceiraEnvelope
        {
            IdeEvento = new EventoIdentificacao
            {
                CnpjRespons = "11222333000181",
                DhEvento = DateTime.Now,
                VersaoEvento = "1.0.0"
            },
            IdeRespons = new ResponsavelIdentificacao
            {
                CnpjRespons = "11222333000181",
                NmRespons = "Empresa Teste Ltda",
                Email = "contato@empresa.com.br"
            },
            Eventos = new List<EventoEFinanceira>()
        };
    }
}
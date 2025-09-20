using EFinanceira.Core.Models;
using FluentAssertions;

namespace EFinanceira.Core.Tests.Models;

public class EFinanceiraEnvelopeTests
{
    [Fact]
    public void EFinanceiraEnvelope_ShouldInitializeWithEmptyEventsList()
    {
        // Arrange & Act
        var envelope = new EFinanceiraEnvelope();

        // Assert
        envelope.Eventos.Should().NotBeNull();
        envelope.Eventos.Should().BeEmpty();
    }

    [Fact]
    public void EventoIdentificacao_ShouldHaveDefaultVersaoEvento()
    {
        // Arrange & Act
        var evento = new EventoIdentificacao();

        // Assert
        evento.VersaoEvento.Should().Be("1.0.0");
    }

    [Fact]
    public void MovimentacaoFinanceira_ShouldInitializeWithEmptyMovimentacoesList()
    {
        // Arrange & Act
        var movimentacao = new MovimentacaoFinanceira();

        // Assert
        movimentacao.Movimentacoes.Should().NotBeNull();
        movimentacao.Movimentacoes.Should().BeEmpty();
    }

    [Theory]
    [InlineData(TipoConta.ContaCorrente, "01")]
    [InlineData(TipoConta.ContaPoupanca, "02")]
    [InlineData(TipoConta.ContaDepositoPrazo, "03")]
    [InlineData(TipoConta.ContaInvestimento, "04")]
    public void TipoConta_ShouldHaveCorrectXmlEnumValues(TipoConta tipoConta, string expectedXmlValue)
    {
        // Arrange & Act
        var xmlValue = GetXmlEnumValue(tipoConta);

        // Assert
        xmlValue.Should().Be(expectedXmlValue);
    }

    [Theory]
    [InlineData(SubtipoConta.Individual, "01")]
    [InlineData(SubtipoConta.Conjunta, "02")]
    [InlineData(SubtipoConta.Corporativa, "03")]
    public void SubtipoConta_ShouldHaveCorrectXmlEnumValues(SubtipoConta subtipoConta, string expectedXmlValue)
    {
        // Arrange & Act
        var xmlValue = GetXmlEnumValue(subtipoConta);

        // Assert
        xmlValue.Should().Be(expectedXmlValue);
    }

    [Theory]
    [InlineData(TipoMovimento.Credito, "C")]
    [InlineData(TipoMovimento.Debito, "D")]
    public void TipoMovimento_ShouldHaveCorrectXmlEnumValues(TipoMovimento tipoMovimento, string expectedXmlValue)
    {
        // Arrange & Act
        var xmlValue = GetXmlEnumValue(tipoMovimento);

        // Assert
        xmlValue.Should().Be(expectedXmlValue);
    }

    private static string GetXmlEnumValue<T>(T enumValue) where T : Enum
    {
        var field = typeof(T).GetField(enumValue.ToString());
        var attribute = field?.GetCustomAttributes(typeof(System.Xml.Serialization.XmlEnumAttribute), false)
            .FirstOrDefault() as System.Xml.Serialization.XmlEnumAttribute;
        
        return attribute?.Name ?? enumValue.ToString();
    }
}
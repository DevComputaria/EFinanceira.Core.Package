using System;
using Xunit;
using EFinanceira.Messages.Builders.Eventos.EvtRERCT;

namespace EFinanceira.Tests.Messages.Builders.Eventos.EvtRERCT;

/// <summary>
/// Testes unitários para o EvtRERCTBuilder
/// </summary>
public class EvtRERCTBuilderTests
{
    [Fact]
    public void Create_ShouldCreateBuilderWithDefaultVersion()
    {
        // Act
        var builder = EvtRERCTBuilder.Create();

        // Assert
        Assert.NotNull(builder);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithMinimalConfiguration()
    {
        // Arrange & Act
        var message = EvtRERCTBuilder
            .Create()
            .WithIdeEvento(ideEvento => ideEvento
                .WithIdeEventoRERCT(111222333))
            .WithIdeDeclarante(ideDeclarante => ideDeclarante
                .WithCnpjDeclarante("00000000000100"))
            .WithIdeDeclarado(ideDeclarado => ideDeclarado
                .WithCpfCnpjDeclarado(cpfCnpj => cpfCnpj
                    .WithTipoInscricao(1)
                    .WithNumeroInscricao("00000000001")))
            .AddRERCT(rerct => rerct
                .WithNomeBancoOrigem("Test Bank")
                .WithPaisOrigem("US")
                .AddInfoContaExterior(conta => conta
                    .WithNumeroContaExterior("TEST123")
                    .WithValorUltimoDia("1000.00")
                    .WithMoeda("USD")
                    .AddTitular(titular => titular
                        .WithNomeTitular("Test Titular")
                        .WithCpfCnpjTitular(cpfCnpj => cpfCnpj
                            .WithTipoInscricao(1)
                            .WithNumeroInscricao("00000000001")))))
            .Build();

        // Assert
        Assert.NotNull(message);
        Assert.Equal("v1_2_0", message.Version);
        Assert.Equal("evtRERCT", message.RootElementName);
        Assert.Equal("id", message.IdAttributeName);
        Assert.NotNull(message.IdValue);
        Assert.NotNull(message.Evento);
        Assert.NotNull(message.Payload);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenRERCTIsMissing()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            EvtRERCTBuilder
                .Create()
                .WithIdeEvento(ideEvento => ideEvento
                    .WithIdeEventoRERCT(111222333))
                .WithIdeDeclarante(ideDeclarante => ideDeclarante
                    .WithCnpjDeclarante("00000000000100"))
                .WithIdeDeclarado(ideDeclarado => ideDeclarado
                    .WithCpfCnpjDeclarado(cpfCnpj => cpfCnpj
                        .WithTipoInscricao(1)
                        .WithNumeroInscricao("00000000001")))
                .Build());

        Assert.Equal("Pelo menos um registro RERCT é obrigatório", exception.Message);
    }

    [Fact]
    public void AddRERCTs_ShouldAddMultipleRERCTs()
    {
        // Arrange & Act
        var message = EvtRERCTBuilder
            .Create()
            .WithIdeEvento(ideEvento => ideEvento
                .WithIdeEventoRERCT(111222333))
            .WithIdeDeclarante(ideDeclarante => ideDeclarante
                .WithCnpjDeclarante("00000000000100"))
            .WithIdeDeclarado(ideDeclarado => ideDeclarado
                .WithCpfCnpjDeclarado(cpfCnpj => cpfCnpj
                    .WithTipoInscricao(1)
                    .WithNumeroInscricao("00000000001")))
            .AddRERCTs(
                rerct => rerct
                    .WithNomeBancoOrigem("Bank 1")
                    .WithPaisOrigem("US")
                    .AddInfoContaExterior(conta => conta
                        .WithNumeroContaExterior("US123")
                        .WithValorUltimoDia("1000.00")
                        .WithMoeda("USD")
                        .AddTitular(titular => titular
                            .WithNomeTitular("Titular 1")
                            .WithCpfCnpjTitular(cpfCnpj => cpfCnpj
                                .WithTipoInscricao(1)
                                .WithNumeroInscricao("00000000001")))),
                rerct => rerct
                    .WithNomeBancoOrigem("Bank 2")
                    .WithPaisOrigem("UK")
                    .AddInfoContaExterior(conta => conta
                        .WithNumeroContaExterior("UK456")
                        .WithValorUltimoDia("2000.00")
                        .WithMoeda("GBP")
                        .AddTitular(titular => titular
                            .WithNomeTitular("Titular 2")
                            .WithCpfCnpjTitular(cpfCnpj => cpfCnpj
                                .WithTipoInscricao(1)
                                .WithNumeroInscricao("00000000001")))))
            .Build();

        // Assert
        Assert.NotNull(message.Evento.RERCT);
        Assert.Equal(2, message.Evento.RERCT.Length);

        Assert.Equal("Bank 1", message.Evento.RERCT[0].nomeBancoOrigem);
        Assert.Equal("US", message.Evento.RERCT[0].paisOrigem);

        Assert.Equal("Bank 2", message.Evento.RERCT[1].nomeBancoOrigem);
        Assert.Equal("UK", message.Evento.RERCT[1].paisOrigem);
    }
}
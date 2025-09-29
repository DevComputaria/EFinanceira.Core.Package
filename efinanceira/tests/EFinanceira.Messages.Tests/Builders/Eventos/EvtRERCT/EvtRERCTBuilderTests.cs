using System;
using Xunit;
using EFinanceira.Messages.Builders.Eventos.EvtRERCT;

namespace EFinanceira.Messages.Tests.Builders.Eventos.EvtRERCT;

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
    public void Create_ShouldCreateBuilderWithSpecificVersion()
    {
        // Arrange
        const string version = "v1_2_0";

        // Act
        var builder = EvtRERCTBuilder.Create(version);

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
    public void Build_ShouldCreateValidMessage_WithCompleteConfiguration()
    {
        // Arrange & Act
        var message = EvtRERCTBuilder
            .Create("v1_2_0")
            .WithId("ID_TEST_RERCT")
            .WithIdeEvento(ideEvento => ideEvento
                .WithIdeEventoRERCT(123456789)
                .WithIndRetificacao(0)
                .WithAmbiente(2)
                .WithAplicativoEmi(1)
                .WithVersaoAplic("1.0.0"))
            .WithIdeDeclarante(ideDeclarante => ideDeclarante
                .WithCnpjDeclarante("12345678000123"))
            .WithIdeDeclarado(ideDeclarado => ideDeclarado
                .WithCpfCnpjDeclarado(cpfCnpj => cpfCnpj
                    .WithTipoInscricao(1)
                    .WithNumeroInscricao("12345678901")))
            .AddRERCT(rerct => rerct
                .WithNomeBancoOrigem("Bank of International")
                .WithPaisOrigem("US")
                .WithBICBancoOrigem("BOFAUS3N")
                .AddInfoContaExterior(conta => conta
                    .WithTipoContaExterior(1)
                    .WithNumeroContaExterior("US123456789")
                    .WithValorUltimoDia("150000.00")
                    .WithMoeda("USD")
                    .AddTitular(titular => titular
                        .WithNomeTitular("João Silva")
                        .WithCpfCnpjTitular(cpfCnpj => cpfCnpj
                            .WithTipoInscricao(1)
                            .WithNumeroInscricao("12345678901"))
                        .WithNIFTitular("123456789US"))
                    .AddBeneficiarioFinal(beneficiario => beneficiario
                        .WithNomeBeneficiarioFinal("Maria Silva")
                        .WithCpfBeneficiarioFinal("98765432100")
                        .WithNIFBeneficiarioFinal("987654321US"))))
            .Build();

        // Assert
        Assert.NotNull(message);
        Assert.Equal("v1_2_0", message.Version);
        Assert.Equal("ID_TEST_RERCT", message.IdValue);

        // Verificar estrutura do evento
        var evento = message.Evento;
        Assert.NotNull(evento);
        Assert.Equal("ID_TEST_RERCT", evento.id);

        // Verificar IdeEvento
        Assert.NotNull(evento.ideEvento);
        Assert.Equal(123456789u, evento.ideEvento.ideEventoRERCT);
        Assert.Equal(0u, evento.ideEvento.indRetificacao);
        Assert.Equal(2u, evento.ideEvento.tpAmb);
        Assert.Equal(1u, evento.ideEvento.aplicEmi);
        Assert.Equal("1.0.0", evento.ideEvento.verAplic);

        // Verificar IdeDeclarante
        Assert.NotNull(evento.ideDeclarante);
        Assert.Equal("12345678000123", evento.ideDeclarante.cnpjDeclarante);

        // Verificar IdeDeclarado
        Assert.NotNull(evento.ideDeclarado);
        Assert.NotNull(evento.ideDeclarado.cpfCnpjDeclarado);
        Assert.Equal(1u, evento.ideDeclarado.cpfCnpjDeclarado.tpInscr);
        Assert.Equal("12345678901", evento.ideDeclarado.cpfCnpjDeclarado.nrInscr);

        // Verificar RERCT
        Assert.NotNull(evento.RERCT);
        Assert.Single(evento.RERCT);

        var rerct = evento.RERCT[0];
        Assert.Equal("Bank of International", rerct.nomeBancoOrigem);
        Assert.Equal("US", rerct.paisOrigem);
        Assert.Equal("BOFAUS3N", rerct.BICBancoOrigem);

        // Verificar InfoContaExterior
        Assert.NotNull(rerct.infoContaExterior);
        Assert.Single(rerct.infoContaExterior);

        var conta = rerct.infoContaExterior[0];
        Assert.Equal(1u, conta.tpContaExterior);
        Assert.True(conta.tpContaExteriorSpecified);
        Assert.Equal("US123456789", conta.numContaExterior);
        Assert.Equal("150000.00", conta.vlrUltDia);
        Assert.Equal("USD", conta.moeda);

        // Verificar Titular
        Assert.NotNull(conta.titular);
        Assert.Single(conta.titular);

        var titular = conta.titular[0];
        Assert.Equal("João Silva", titular.nomeTitular);
        Assert.Equal("123456789US", titular.NIFTitular);
        Assert.NotNull(titular.cpfCnpjTitular);
        Assert.Equal(1u, titular.cpfCnpjTitular.tpInscr);
        Assert.Equal("12345678901", titular.cpfCnpjTitular.nrInscr);

        // Verificar BeneficiarioFinal
        Assert.NotNull(conta.beneficiarioFinal);
        Assert.Single(conta.beneficiarioFinal);

        var beneficiario = conta.beneficiarioFinal[0];
        Assert.Equal("Maria Silva", beneficiario.nomeBeneficiarioFinal);
        Assert.Equal("98765432100", beneficiario.cpfBeneficiarioFinal);
        Assert.Equal("987654321US", beneficiario.NIFBeneficiarioFinal);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenIdeEventoIsMissing()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            EvtRERCTBuilder
                .Create()
                .WithIdeDeclarante(ideDeclarante => ideDeclarante
                    .WithCnpjDeclarante("00000000000100"))
                .Build());

        Assert.Equal("IdeEvento é obrigatório", exception.Message);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenIdeDeclaranteIsMissing()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            EvtRERCTBuilder
                .Create()
                .WithIdeEvento(ideEvento => ideEvento
                    .WithIdeEventoRERCT(111222333))
                .Build());

        Assert.Equal("IdeDeclarante é obrigatório", exception.Message);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenIdeDeclaradoIsMissing()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            EvtRERCTBuilder
                .Create()
                .WithIdeEvento(ideEvento => ideEvento
                    .WithIdeEventoRERCT(111222333))
                .WithIdeDeclarante(ideDeclarante => ideDeclarante
                    .WithCnpjDeclarante("00000000000100"))
                .Build());

        Assert.Equal("IdeDeclarado é obrigatório", exception.Message);
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
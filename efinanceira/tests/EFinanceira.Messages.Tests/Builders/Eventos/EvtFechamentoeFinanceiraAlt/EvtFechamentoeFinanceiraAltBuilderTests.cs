using Xunit;
using EFinanceira.Messages.Builders.Eventos.EvtFechamentoeFinanceiraAlt;

namespace EFinanceira.Messages.Tests.Builders.Eventos.EvtFechamentoeFinanceiraAlt;

public class EvtFechamentoeFinanceiraAltBuilderTests
{
    [Fact]
    public void Create_ShouldCreateBuilderWithDefaultVersion()
    {
        var builder = EvtFechamentoeFinanceiraAltBuilder.Create();
        Assert.NotNull(builder);
    }

    [Fact]
    public void Create_ShouldCreateBuilderWithSpecificVersion()
    {
        const string version = "v1_2_0";
        var builder = EvtFechamentoeFinanceiraAltBuilder.Create(version);
        Assert.NotNull(builder);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithMinimalConfiguration()
    {
        var message = EvtFechamentoeFinanceiraAltBuilder
            .Create()
            .WithIdeEvento(ideEvento => ideEvento
                .WithIndicadorRetificacao(1))
            .WithIdeDeclarante(ideDeclarante => ideDeclarante
                .WithCnpj("00000000000100"))
            .WithInfoFechamento(info => info
                .WithDataInicio(System.DateTime.UtcNow)
                .WithDataFim(System.DateTime.UtcNow))
            .Build();

        Assert.NotNull(message);
        Assert.Equal("v1_2_2_alt", message.Version);
        Assert.Equal("evtFechamentoeFinanceira", message.RootElementName);
        Assert.NotNull(message.Evento);
        Assert.NotNull(message.Payload);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenIdeDeclaranteIsMissing()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            EvtFechamentoeFinanceiraAltBuilder
                .Create()
                .WithIdeEvento(ideEvento => ideEvento
                    .WithIndicadorRetificacao(1))
                .Build());

        Assert.Equal("IdeDeclarante é obrigatório", exception.Message);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenInfoFechamentoIsMissing()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            EvtFechamentoeFinanceiraAltBuilder
                .Create()
                .WithIdeEvento(ideEvento => ideEvento
                    .WithIndicadorRetificacao(1))
                .WithIdeDeclarante(ideDeclarante => ideDeclarante
                    .WithCnpj("00000000000100"))
                .Build());

        Assert.Equal("InfoFechamento é obrigatório", exception.Message);
    }

    [Fact]
    public void Build_ShouldSupportFechamentoPP()
    {
        var message = EvtFechamentoeFinanceiraAltBuilder
            .Create()
            .WithIdeEvento(ideEvento => ideEvento
                .WithIndicadorRetificacao(1))
            .WithIdeDeclarante(ideDeclarante => ideDeclarante
                .WithCnpj("00000000000100"))
            .WithInfoFechamento(info => info
                .WithDataInicio(System.DateTime.UtcNow)
                .WithDataFim(System.DateTime.UtcNow))
            .WithFechamentoPP(fechamento => fechamento
                .AdicionarFechamentoMes("202401", 5))
            .Build();

        Assert.NotNull(message);
    }

    [Fact]
    public void Build_ShouldSupportFechamentoMovOpFin()
    {
        var message = EvtFechamentoeFinanceiraAltBuilder
            .Create()
            .WithIdeEvento(ideEvento => ideEvento
                .WithIndicadorRetificacao(1))
            .WithIdeDeclarante(ideDeclarante => ideDeclarante
                .WithCnpj("00000000000100"))
            .WithInfoFechamento(info => info
                .WithDataInicio(System.DateTime.UtcNow)
                .WithDataFim(System.DateTime.UtcNow))
            .WithFechamentoMovOpFin(fechamento => fechamento
                .AdicionarFechamentoMes(mes => mes
                    .WithAnoMesCaixa("202401")
                    .WithQuantidadeArquivos(3)))
            .Build();

        Assert.NotNull(message);
    }

    [Fact]
    public void Build_ShouldSupportFechamentoMovOpFinAnual()
    {
        var message = EvtFechamentoeFinanceiraAltBuilder
            .Create()
            .WithIdeEvento(ideEvento => ideEvento
                .WithIndicadorRetificacao(1))
            .WithIdeDeclarante(ideDeclarante => ideDeclarante
                .WithCnpj("00000000000100"))
            .WithInfoFechamento(info => info
                .WithDataInicio(System.DateTime.UtcNow)
                .WithDataFim(System.DateTime.UtcNow))
            .WithFechamentoMovOpFinAnual(fechamento => fechamento
                .WithFechamentoAno(ano => ano
                    .WithAnoCaixa("2024")
                    .WithQuantidadeArquivos(12)))
            .Build();

        Assert.NotNull(message);
    }

    [Fact]
    public void Build_ShouldSupportWithId()
    {
        var message = EvtFechamentoeFinanceiraAltBuilder
            .Create()
            .WithId("ID_CUSTOM123")
            .WithIdeEvento(ideEvento => ideEvento
                .WithIndicadorRetificacao(1))
            .WithIdeDeclarante(ideDeclarante => ideDeclarante
                .WithCnpj("00000000000100"))
            .WithInfoFechamento(info => info
                .WithDataInicio(System.DateTime.UtcNow)
                .WithDataFim(System.DateTime.UtcNow))
            .Build();

        Assert.NotNull(message);
        Assert.Equal("ID_CUSTOM123", message.IdValue);
    }
}
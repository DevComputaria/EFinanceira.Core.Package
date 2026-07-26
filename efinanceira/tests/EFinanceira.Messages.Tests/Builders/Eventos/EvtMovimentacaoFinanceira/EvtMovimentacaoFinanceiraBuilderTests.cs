using Xunit;
using EFinanceira.Messages.Builders.Eventos.EvtMovimentacaoFinanceira;

namespace EFinanceira.Messages.Tests.Builders.Eventos.EvtMovimentacaoFinanceira;

public class EvtMovimentacaoFinanceiraBuilderTests
{
    [Fact]
    public void Constructor_ShouldCreateBuilderWithDefaultVersion()
    {
        var builder = new EvtMovimentacaoFinanceiraBuilder();
        Assert.NotNull(builder);
    }

    [Fact]
    public void Constructor_ShouldCreateBuilderWithSpecificVersion()
    {
        const string version = "v1_2_1";
        var builder = new EvtMovimentacaoFinanceiraBuilder(version);
        Assert.NotNull(builder);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithMinimalConfiguration()
    {
        var message = new EvtMovimentacaoFinanceiraBuilder()
            .ComId("ID001")
            .ComIdeEvento(ideEvento => ideEvento
                .ComAmbiente(2))
            .ComIdeDeclarante(ideDeclarante => ideDeclarante
                .ComCnpj("00000000000100"))
            .ComIdeDeclarado(ideDeclarado => ideDeclarado
                .ComCpfCnpj("00000000000100"))
            .ComMesCaixa(mesCaixa => mesCaixa
                .ComAnoMes("202501"))
            .Build();

        Assert.NotNull(message);
        Assert.Equal("v1_2_1", message.Version);
        Assert.Equal("evtMovOpFin", message.RootElementName);
        Assert.NotNull(message.Evento);
        Assert.NotNull(message.Payload);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithAllConfiguration()
    {
        var message = new EvtMovimentacaoFinanceiraBuilder("v1_2_1")
            .ComId("ID002")
            .ComIdeEvento(ideEvento => ideEvento
                .ComAmbiente(2)
                .ComAplicacaoEmissor("TestApp"))
            .ComIdeDeclarante(ideDeclarante => ideDeclarante
                .ComCnpj("00000000000100"))
            .ComIdeDeclarado(ideDeclarado => ideDeclarado
                .ComCpfCnpj("00000000000100"))
            .ComMesCaixa(mesCaixa => mesCaixa
                .ComAnoMes("202501"))
            .Build();

        Assert.NotNull(message);
        Assert.Equal("v1_2_1", message.Version);
        Assert.Equal("evtMovOpFin", message.RootElementName);
    }
}

using Xunit;
using EFinanceira.Messages.Builders.Eventos.EvtMovimentacaoFinanceiraAnual;

namespace EFinanceira.Messages.Tests.Builders.Eventos.EvtMovimentacaoFinanceiraAnual;

public class EvtMovimentacaoFinanceiraAnualBuilderTests
{
    [Fact]
    public void Constructor_ShouldCreateBuilderWithDefaultVersion()
    {
        var builder = new EvtMovimentacaoFinanceiraAnualBuilder();
        Assert.NotNull(builder);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithMinimalConfiguration()
    {
        var message = new EvtMovimentacaoFinanceiraAnualBuilder()
            .ComId("ID001")
            .ComIdeEvento(ideEvento => ideEvento
                .ComAmbiente(2))
            .ComIdeDeclarante(ideDeclarante => ideDeclarante
                .ComCnpj("00000000000100"))
            .ComIdeDeclarado(ideDeclarado => ideDeclarado
                .ComCpfCnpj("00000000000100"))
            .ComCaixa(caixa => caixa
                .ComAnoBase(2025))
            .Build();

        Assert.NotNull(message);
        Assert.Equal("v1_2_2", message.Version);
        Assert.Equal("evtMovOpFinAnual", message.RootElementName);
        Assert.NotNull(message.Evento);
        Assert.NotNull(message.Payload);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithAllConfiguration()
    {
        var message = new EvtMovimentacaoFinanceiraAnualBuilder()
            .ComId("ID002")
            .ComIdeEvento(ideEvento => ideEvento
                .ComAmbiente(2)
                .ComAplicacaoEmissor("TestApp"))
            .ComIdeDeclarante(ideDeclarante => ideDeclarante
                .ComCnpj("00000000000100"))
            .ComIdeDeclarado(ideDeclarado => ideDeclarado
                .ComCpfCnpj("00000000000100"))
            .ComCaixa(caixa => caixa
                .ComAnoBase(2025))
            .Build();

        Assert.NotNull(message);
        Assert.Equal("v1_2_2", message.Version);
        Assert.Equal("evtMovOpFinAnual", message.RootElementName);
    }
}

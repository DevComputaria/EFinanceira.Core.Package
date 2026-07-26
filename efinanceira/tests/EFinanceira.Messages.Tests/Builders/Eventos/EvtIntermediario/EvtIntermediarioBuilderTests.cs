using Xunit;
using EFinanceira.Messages.Builders.Eventos.EvtIntermediario;

namespace EFinanceira.Messages.Tests.Builders.Eventos.EvtIntermediario;

public class EvtIntermediarioBuilderTests
{
    [Fact]
    public void Constructor_ShouldCreateBuilderWithDefaultVersion()
    {
        var builder = new EvtIntermediarioBuilder();
        Assert.NotNull(builder);
    }

    [Fact]
    public void Constructor_ShouldCreateBuilderWithSpecificVersion()
    {
        const string version = "v1_2_0";
        var builder = new EvtIntermediarioBuilder(version);
        Assert.NotNull(builder);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithMinimalConfiguration()
    {
        var message = new EvtIntermediarioBuilder()
            .ComId("ID001")
            .ComIdeEvento(ideEvento => ideEvento
                .ComAmbiente(2))
            .ComIdeDeclarante(ideDeclarante => ideDeclarante
                .ComCnpj("00000000000100"))
            .ComInfoIntermediario(info => info
                .ComNome("Intermediario Teste"))
            .Build();

        Assert.NotNull(message);
        Assert.Equal("v1_2_0", message.Version);
        Assert.Equal("evtCadIntermediario", message.RootElementName);
        Assert.NotNull(message.Evento);
        Assert.NotNull(message.Payload);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithAllConfiguration()
    {
        var message = new EvtIntermediarioBuilder("v1_2_0")
            .ComId("ID002")
            .ComIdeEvento(ideEvento => ideEvento
                .ComAmbiente(2)
                .ComAplicacaoEmissor("TestApp"))
            .ComIdeDeclarante(ideDeclarante => ideDeclarante
                .ComCnpj("00000000000100"))
            .ComInfoIntermediario(info => info
                .ComNome("Intermediario Teste")
                .ComCnpj("00000000000200"))
            .Build();

        Assert.NotNull(message);
        Assert.Equal("v1_2_0", message.Version);
        Assert.Equal("evtCadIntermediario", message.RootElementName);
    }
}

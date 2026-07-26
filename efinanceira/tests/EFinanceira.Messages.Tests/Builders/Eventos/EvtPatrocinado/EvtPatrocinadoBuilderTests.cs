using Xunit;
using EFinanceira.Messages.Builders.Eventos.EvtPatrocinado;

namespace EFinanceira.Messages.Tests.Builders.Eventos.EvtPatrocinado;

public class EvtPatrocinadoBuilderTests
{
    [Fact]
    public void Constructor_ShouldCreateBuilderWithDefaultVersion()
    {
        var builder = new EvtPatrocinadoBuilder();
        Assert.NotNull(builder);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithMinimalConfiguration()
    {
        var message = new EvtPatrocinadoBuilder()
            .ComId("ID001")
            .ComIdeEvento(ideEvento => ideEvento
                .ComAmbiente(2))
            .ComIdeDeclarante(ideDeclarante => ideDeclarante
                .ComCnpj("00000000000100"))
            .ComInfoPatrocinado(info => info
                .ComNome("Patrocinado Teste"))
            .Build();

        Assert.NotNull(message);
        Assert.Equal("v1_2_0", message.Version);
        Assert.Equal("evtCadPatrocinado", message.RootElementName);
        Assert.NotNull(message.Evento);
        Assert.NotNull(message.Payload);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithAllConfiguration()
    {
        var message = new EvtPatrocinadoBuilder()
            .ComId("ID002")
            .ComIdeEvento(ideEvento => ideEvento
                .ComAmbiente(2)
                .ComAplicacaoEmissor("TestApp"))
            .ComIdeDeclarante(ideDeclarante => ideDeclarante
                .ComCnpj("00000000000100"))
            .ComInfoPatrocinado(info => info
                .ComNome("Patrocinado Teste")
                .ComCpf("00000000100"))
            .Build();

        Assert.NotNull(message);
        Assert.Equal("v1_2_0", message.Version);
        Assert.Equal("evtCadPatrocinado", message.RootElementName);
    }
}

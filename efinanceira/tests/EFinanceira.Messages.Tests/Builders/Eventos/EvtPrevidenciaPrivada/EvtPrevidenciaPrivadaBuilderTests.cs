using Xunit;
using EFinanceira.Messages.Builders.Eventos.EvtPrevidenciaPrivada;

namespace EFinanceira.Messages.Tests.Builders.Eventos.EvtPrevidenciaPrivada;

public class EvtPrevidenciaPrivadaBuilderTests
{
    [Fact]
    public void Constructor_ShouldCreateBuilderWithDefaultVersion()
    {
        var builder = new EvtPrevidenciaPrivadaBuilder();
        Assert.NotNull(builder);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithMinimalConfiguration()
    {
        var message = new EvtPrevidenciaPrivadaBuilder()
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
        Assert.Equal("v1_2_5", message.Version);
        Assert.Equal("evtMovPP", message.RootElementName);
        Assert.NotNull(message.Evento);
        Assert.NotNull(message.Payload);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithAllConfiguration()
    {
        var message = new EvtPrevidenciaPrivadaBuilder()
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
        Assert.Equal("v1_2_5", message.Version);
        Assert.Equal("evtMovPP", message.RootElementName);
    }
}

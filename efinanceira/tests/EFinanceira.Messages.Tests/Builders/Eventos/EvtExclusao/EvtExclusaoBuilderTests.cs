using Xunit;
using EFinanceira.Messages.Builders.Eventos.EvtExclusao;

namespace EFinanceira.Messages.Tests.Builders.Eventos.EvtExclusao;

public class EvtExclusaoBuilderTests
{
    [Fact]
    public void Create_ShouldCreateBuilderWithDefaultVersion()
    {
        var builder = EvtExclusaoBuilder.Create();
        Assert.NotNull(builder);
    }

    [Fact]
    public void Create_ShouldCreateBuilderWithSpecificVersion()
    {
        const string version = "v1_2_0";
        var builder = EvtExclusaoBuilder.Create(version);
        Assert.NotNull(builder);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithMinimalConfiguration()
    {
        var message = EvtExclusaoBuilder
            .Create()
            .WithIdeEvento(ideEvento => ideEvento
                .WithAmbiente(2))
            .WithIdeDeclarante(ideDeclarante => ideDeclarante
                .WithCnpj("00000000000100"))
            .WithInfoExclusao(info => info
                .WithNumeroReciboEvento("REC-001"))
            .Build();

        Assert.NotNull(message);
        Assert.Equal("v1_2_0", message.Version);
        Assert.Equal("evtExclusao", message.RootElementName);
        Assert.NotNull(message.Evento);
        Assert.NotNull(message.Payload);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenIdeDeclaranteIsMissing()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            EvtExclusaoBuilder
                .Create()
                .WithIdeEvento(ideEvento => ideEvento
                    .WithAmbiente(2))
                .Build());

        Assert.Equal("IdeDeclarante é obrigatório", exception.Message);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenInfoExclusaoIsMissing()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            EvtExclusaoBuilder
                .Create()
                .WithIdeEvento(ideEvento => ideEvento
                    .WithAmbiente(2))
                .WithIdeDeclarante(ideDeclarante => ideDeclarante
                    .WithCnpj("00000000000100"))
                .Build());

        Assert.Equal("InfoExclusao é obrigatório", exception.Message);
    }
}
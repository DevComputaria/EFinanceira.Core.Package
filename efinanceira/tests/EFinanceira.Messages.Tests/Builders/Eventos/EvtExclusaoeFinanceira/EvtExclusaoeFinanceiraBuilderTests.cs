using Xunit;
using EFinanceira.Messages.Builders.Eventos.EvtExclusaoeFinanceira;

namespace EFinanceira.Messages.Tests.Builders.Eventos.EvtExclusaoeFinanceira;

public class EvtExclusaoeFinanceiraBuilderTests
{
    [Fact]
    public void Create_ShouldCreateBuilderWithDefaultVersion()
    {
        var builder = EvtExclusaoeFinanceiraBuilder.Create();
        Assert.NotNull(builder);
    }

    [Fact]
    public void Create_ShouldCreateBuilderWithSpecificVersion()
    {
        const string version = "v1_2_0";
        var builder = EvtExclusaoeFinanceiraBuilder.Create(version);
        Assert.NotNull(builder);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithMinimalConfiguration()
    {
        var message = EvtExclusaoeFinanceiraBuilder
            .Create()
            .WithIdeEvento(ideEvento => ideEvento
                .WithAmbiente(2))
            .WithIdeDeclarante(ideDeclarante => ideDeclarante
                .WithCnpj("00000000000100"))
            .WithInfoExclusaoeFinanceira(info => info
                .WithNumeroReciboEvento("REC-001"))
            .Build();

        Assert.NotNull(message);
        Assert.Equal("v1_2_0", message.Version);
        Assert.Equal("evtExclusaoeFinanceira", message.RootElementName);
        Assert.NotNull(message.Evento);
        Assert.NotNull(message.Payload);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenIdeDeclaranteIsMissing()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            EvtExclusaoeFinanceiraBuilder
                .Create()
                .WithIdeEvento(ideEvento => ideEvento
                    .WithAmbiente(2))
                .Build());

        Assert.Equal("IdeDeclarante é obrigatório", exception.Message);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenInfoExclusaoeFinanceiraIsMissing()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            EvtExclusaoeFinanceiraBuilder
                .Create()
                .WithIdeEvento(ideEvento => ideEvento
                    .WithAmbiente(2))
                .WithIdeDeclarante(ideDeclarante => ideDeclarante
                    .WithCnpj("00000000000100"))
                .Build());

        Assert.Equal("InfoExclusaoeFinanceira é obrigatório", exception.Message);
    }
}
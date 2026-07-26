using Xunit;
using EFinanceira.Messages.Builders.Eventos.EvtAberturaeFinanceira;

namespace EFinanceira.Messages.Tests.Builders.Eventos.EvtAberturaeFinanceira;

public class EvtAberturaeFinanceiraBuilderTests
{
    [Fact]
    public void Create_ShouldCreateBuilderWithDefaultVersion()
    {
        var builder = EvtAberturaeFinanceiraBuilder.Create();
        Assert.NotNull(builder);
    }

    [Fact]
    public void Create_ShouldCreateBuilderWithSpecificVersion()
    {
        const string version = "v1_2_1";
        var builder = EvtAberturaeFinanceiraBuilder.Create(version);
        Assert.NotNull(builder);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithMinimalConfiguration()
    {
        var message = EvtAberturaeFinanceiraBuilder
            .Create()
            .WithIdeEvento(ideEvento => ideEvento
                .WithIndRetificacao(1))
            .WithIdeDeclarante(ideDeclarante => ideDeclarante
                .WithCnpjDeclarante("00000000000100"))
            .WithInfoAbertura(info => info
                .WithDataInicio(System.DateTime.UtcNow)
                .WithDataFim(System.DateTime.UtcNow))
            .Build();

        Assert.NotNull(message);
        Assert.Equal("v1_2_1", message.Version);
        Assert.Equal("evtAberturaeFinanceira", message.RootElementName);
        Assert.NotNull(message.Evento);
        Assert.NotNull(message.Payload);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenIdeDeclaranteIsMissing()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            EvtAberturaeFinanceiraBuilder
                .Create()
                .WithIdeEvento(ideEvento => ideEvento
                    .WithIndRetificacao(1))
                .Build());

        Assert.Equal("IdeDeclarante é obrigatório", exception.Message);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenInfoAberturaIsMissing()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            EvtAberturaeFinanceiraBuilder
                .Create()
                .WithIdeEvento(ideEvento => ideEvento
                    .WithIndRetificacao(1))
                .WithIdeDeclarante(ideDeclarante => ideDeclarante
                    .WithCnpjDeclarante("00000000000100"))
                .Build());

        Assert.Equal("InfoAbertura é obrigatório", exception.Message);
    }
}
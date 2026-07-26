using Xunit;
using EFinanceira.Messages.Builders.Eventos.EvtFechamentoeFinanceira;

namespace EFinanceira.Messages.Tests.Builders.Eventos.EvtFechamentoeFinanceira;

public class EvtFechamentoeFinanceiraBuilderTests
{
    [Fact]
    public void Create_ShouldCreateBuilderWithDefaultVersion()
    {
        var builder = EvtFechamentoeFinanceiraBuilder.Create();
        Assert.NotNull(builder);
    }

    [Fact]
    public void Create_ShouldCreateBuilderWithSpecificVersion()
    {
        const string version = "v1_2_0";
        var builder = EvtFechamentoeFinanceiraBuilder.Create(version);
        Assert.NotNull(builder);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithMinimalConfiguration()
    {
        var message = EvtFechamentoeFinanceiraBuilder
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
        Assert.Equal("v1_2_2", message.Version);
        Assert.Equal("evtFechamentoeFinanceira", message.RootElementName);
        Assert.NotNull(message.Evento);
        Assert.NotNull(message.Payload);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenIdeDeclaranteIsMissing()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            EvtFechamentoeFinanceiraBuilder
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
            EvtFechamentoeFinanceiraBuilder
                .Create()
                .WithIdeEvento(ideEvento => ideEvento
                    .WithIndicadorRetificacao(1))
                .WithIdeDeclarante(ideDeclarante => ideDeclarante
                    .WithCnpj("00000000000100"))
                .Build());

        Assert.Equal("InfoFechamento é obrigatório", exception.Message);
    }
}
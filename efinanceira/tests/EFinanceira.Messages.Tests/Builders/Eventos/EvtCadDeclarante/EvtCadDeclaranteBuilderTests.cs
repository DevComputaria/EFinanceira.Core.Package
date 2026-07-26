using Xunit;
using EFinanceira.Messages.Builders.Eventos.EvtCadDeclarante;

namespace EFinanceira.Messages.Tests.Builders.Eventos.EvtCadDeclarante;

public class EvtCadDeclaranteBuilderTests
{
    [Fact]
    public void Create_ShouldCreateBuilderWithDefaultVersion()
    {
        var builder = EvtCadDeclaranteBuilder.Create();
        Assert.NotNull(builder);
    }

    [Fact]
    public void Create_ShouldCreateBuilderWithSpecificVersion()
    {
        const string version = "v1_2_0";
        var builder = EvtCadDeclaranteBuilder.Create(version);
        Assert.NotNull(builder);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithMinimalConfiguration()
    {
        var message = EvtCadDeclaranteBuilder
            .Create()
            .WithIdeEvento(ideEvento => ideEvento
                .WithIndRetificacao(1))
            .WithIdeDeclarante(ideDeclarante => ideDeclarante
                .WithCnpj("00000000000100"))
            .WithInfoCadastro(info => info
                .WithGIIN("ABCDEF.12345.SF.123")
                .WithNome("Declarante Teste"))
            .Build();

        Assert.NotNull(message);
        Assert.Equal("v1_2_0", message.Version);
        Assert.Equal("evtCadDeclarante", message.RootElementName);
        Assert.NotNull(message.Evento);
        Assert.NotNull(message.Payload);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenIdeDeclaranteIsMissing()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            EvtCadDeclaranteBuilder
                .Create()
                .WithIdeEvento(ideEvento => ideEvento
                    .WithIndRetificacao(1))
                .Build());

        Assert.Equal("IdeDeclarante é obrigatório", exception.Message);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenInfoCadastroIsMissing()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            EvtCadDeclaranteBuilder
                .Create()
                .WithIdeEvento(ideEvento => ideEvento
                    .WithIndRetificacao(1))
                .WithIdeDeclarante(ideDeclarante => ideDeclarante
                    .WithCnpj("00000000000100"))
                .Build());

        Assert.Equal("InfoCadastro é obrigatório", exception.Message);
    }
}
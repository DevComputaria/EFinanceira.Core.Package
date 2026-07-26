using Xunit;
using EFinanceira.Messages.Builders.Consultas.RetInfoMovimento;

namespace EFinanceira.Messages.Tests.Builders.Consultas.RetInfoMovimento;

public class RetInfoMovimentoBuilderTests
{
    [Fact]
    public void Constructor_ShouldCreateBuilderWithDefaultVersion()
    {
        var builder = new RetInfoMovimentoBuilder();
        Assert.NotNull(builder);
    }

    [Fact]
    public void Constructor_ShouldCreateBuilderWithSpecificVersion()
    {
        const string version = "v1_2_0";
        var builder = new RetInfoMovimentoBuilder(version);
        Assert.NotNull(builder);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithMinimalConfiguration()
    {
        var message = new RetInfoMovimentoBuilder()
            .WithStatus(status => status
                .WithCodigo("0"))
            .WithIdentificacaoEmpresaDeclarante(empresa => empresa
                .WithCnpj("00000000000100"))
            .WithInformacoesMovimento(info => info
                .WithCnpj("00000000000100"))
            .Build();

        Assert.NotNull(message);
        Assert.Equal("v1_2_0", message.Version);
        Assert.Equal("eFinanceira", message.RootElementName);
        Assert.NotNull(message.Consulta);
        Assert.NotNull(message.Payload);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenRetornoConsultaInformacoesMovimentoIsMissing()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            new RetInfoMovimentoBuilder()
                .Build());

        Assert.Equal("RetornoConsultaInformacoesMovimento é obrigatório", exception.Message);
    }
}

using Xunit;
using EFinanceira.Messages.Builders.Consultas.RetInfoIntermediario;

namespace EFinanceira.Messages.Tests.Builders.Consultas.RetInfoIntermediario;

public class RetInfoIntermediarioBuilderTests
{
    [Fact]
    public void Constructor_ShouldCreateBuilderWithDefaultVersion()
    {
        var builder = new RetInfoIntermediarioBuilder();
        Assert.NotNull(builder);
    }

    [Fact]
    public void Constructor_ShouldCreateBuilderWithSpecificVersion()
    {
        const string version = "v1_2_0";
        var builder = new RetInfoIntermediarioBuilder(version);
        Assert.NotNull(builder);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithMinimalConfiguration()
    {
        var message = new RetInfoIntermediarioBuilder()
            .WithStatus(status => status
                .WithCodigo("0"))
            .WithIdentificacaoEmpresaDeclarante(empresa => empresa
                .WithCnpj("00000000000100"))
            .WithIdentificacaoIntermediario(intermediario => intermediario
                .WithNome("Intermediario Teste"))
            .Build();

        Assert.NotNull(message);
        Assert.Equal("v1_2_0", message.Version);
        Assert.Equal("eFinanceira", message.RootElementName);
        Assert.NotNull(message.Consulta);
        Assert.NotNull(message.Payload);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenRetornoConsultaInformacoesIntermediarioIsMissing()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            new RetInfoIntermediarioBuilder()
                .Build());

        Assert.Equal("RetornoConsultaInformacoesIntermediario é obrigatório", exception.Message);
    }
}

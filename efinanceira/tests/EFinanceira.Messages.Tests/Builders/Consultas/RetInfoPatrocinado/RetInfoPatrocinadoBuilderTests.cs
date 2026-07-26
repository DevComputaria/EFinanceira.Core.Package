using Xunit;
using EFinanceira.Messages.Builders.Consultas.RetInfoPatrocinado;

namespace EFinanceira.Messages.Tests.Builders.Consultas.RetInfoPatrocinado;

public class RetInfoPatrocinadoBuilderTests
{
    [Fact]
    public void Constructor_ShouldCreateBuilderWithDefaultVersion()
    {
        var builder = new RetInfoPatrocinadoBuilder();
        Assert.NotNull(builder);
    }

    [Fact]
    public void Constructor_ShouldCreateBuilderWithSpecificVersion()
    {
        const string version = "v1_2_0";
        var builder = new RetInfoPatrocinadoBuilder(version);
        Assert.NotNull(builder);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithMinimalConfiguration()
    {
        var message = new RetInfoPatrocinadoBuilder()
            .WithStatus(status => status
                .WithCodigo("0"))
            .WithIdentificacaoEmpresaDeclarante(empresa => empresa
                .WithCnpj("00000000000100"))
            .WithIdentificacaoPatrocinado(patrocinado => patrocinado
                .WithNome("Patrocinado Teste"))
            .Build();

        Assert.NotNull(message);
        Assert.Equal("v1_2_0", message.Version);
        Assert.Equal("eFinanceira", message.RootElementName);
        Assert.NotNull(message.Consulta);
        Assert.NotNull(message.Payload);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenRetornoConsultaInformacoesPatrocinadoIsMissing()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            new RetInfoPatrocinadoBuilder()
                .Build());

        Assert.Equal("RetornoConsultaInformacoesPatrocinado é obrigatório", exception.Message);
    }
}

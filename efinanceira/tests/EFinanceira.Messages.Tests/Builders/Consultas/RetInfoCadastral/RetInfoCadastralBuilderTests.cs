using Xunit;
using EFinanceira.Messages.Builders.Consultas.RetInfoCadastral;

namespace EFinanceira.Messages.Tests.Builders.Consultas.RetInfoCadastral;

public class RetInfoCadastralBuilderTests
{
    [Fact]
    public void Constructor_ShouldCreateBuilderWithDefaultVersion()
    {
        var builder = new RetInfoCadastralBuilder();
        Assert.NotNull(builder);
    }

    [Fact]
    public void Constructor_ShouldCreateBuilderWithSpecificVersion()
    {
        const string version = "v1_2_0";
        var builder = new RetInfoCadastralBuilder(version);
        Assert.NotNull(builder);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithMinimalConfiguration()
    {
        var message = new RetInfoCadastralBuilder()
            .WithId("ID_TEST")
            .WithNumeroRecibo("REC-001")
            .WithStatus(status => status
                .WithCodigo("0"))
            .WithIdentificacaoEmpresaDeclarante(empresa => empresa
                .WithCnpj("00000000000100"))
            .WithInformacoesCadastrais(info => info
                .WithNome("Nome Teste"))
            .Build();

        Assert.NotNull(message);
        Assert.Equal("v1_2_0", message.Version);
        Assert.Equal("eFinanceira", message.RootElementName);
        Assert.NotNull(message.Consulta);
        Assert.NotNull(message.Payload);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenIdIsMissing()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            new RetInfoCadastralBuilder()
                .WithStatus(status => status
                    .WithCodigo("0"))
                .WithIdentificacaoEmpresaDeclarante(empresa => empresa
                    .WithCnpj("00000000000100"))
                .WithInformacoesCadastrais(info => info
                    .WithNome("Nome Teste"))
                .Build());

        Assert.Equal("Id é obrigatório", exception.Message);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenNumeroReciboIsMissing()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            new RetInfoCadastralBuilder()
                .WithId("ID_TEST")
                .WithStatus(status => status
                    .WithCodigo("0"))
                .WithIdentificacaoEmpresaDeclarante(empresa => empresa
                    .WithCnpj("00000000000100"))
                .WithInformacoesCadastrais(info => info
                    .WithNome("Nome Teste"))
                .Build());

        Assert.Equal("Número do recibo é obrigatório", exception.Message);
    }
}

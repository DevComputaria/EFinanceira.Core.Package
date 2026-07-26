using Xunit;
using EFinanceira.Messages.Builders.Consultas.RetListaeFinanceira;

namespace EFinanceira.Messages.Tests.Builders.Consultas.RetListaeFinanceira;

public class RetListaeFinanceiraBuilderTests
{
    [Fact]
    public void Constructor_ShouldCreateBuilderWithDefaultVersion()
    {
        var builder = new RetListaeFinanceiraBuilder();
        Assert.NotNull(builder);
    }

    [Fact]
    public void Constructor_ShouldCreateBuilderWithSpecificVersion()
    {
        const string version = "v1_2_0";
        var builder = new RetListaeFinanceiraBuilder(version);
        Assert.NotNull(builder);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithMinimalConfiguration()
    {
        var message = new RetListaeFinanceiraBuilder()
            .WithStatus("0")
            .WithEmpresaDeclarante("00000000000100")
            .Build();

        Assert.NotNull(message);
        Assert.Equal("v1_2_0", message.Version);
        Assert.Equal("eFinanceira", message.RootElementName);
        Assert.NotNull(message.Consulta);
        Assert.NotNull(message.Payload);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenStatusIsMissing()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            new RetListaeFinanceiraBuilder()
                .WithEmpresaDeclarante("00000000000100")
                .Build());

        Assert.Equal("Status é obrigatório. Use WithStatus() para defini-lo.", exception.Message);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenEmpresaDeclaranteIsMissing()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            new RetListaeFinanceiraBuilder()
                .WithStatus("0")
                .Build());

        Assert.Equal("Empresa declarante é obrigatória. Use WithEmpresaDeclarante() para defini-la.", exception.Message);
    }
}

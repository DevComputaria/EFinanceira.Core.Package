using Xunit;
using EFinanceira.Messages.Builders.Lotes.EnvioLoteCriptografado;

namespace EFinanceira.Messages.Tests.Builders.Lotes.EnvioLoteCriptografado;

public class EnvioLoteCriptografadoBuilderTests
{
    [Fact]
    public void Create_ShouldCreateBuilderWithDefaultVersion()
    {
        var builder = EnvioLoteCriptografadoBuilder.Create();
        Assert.NotNull(builder);
    }

    [Fact]
    public void Create_ShouldCreateBuilderWithSpecificVersion()
    {
        const string version = "v1_2_0";
        var builder = EnvioLoteCriptografadoBuilder.Create(version);
        Assert.NotNull(builder);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithMinimalConfiguration()
    {
        var message = EnvioLoteCriptografadoBuilder
            .Create()
            .ComIdCertificado("CERT-001")
            .ComChave("chave-criptografica-teste")
            .ComLote("lote-criptografado-teste")
            .Build();

        Assert.NotNull(message);
        Assert.Equal("v1_2_0", message.Version);
        Assert.Equal("eFinanceira", message.RootElementName);
        Assert.NotNull(message.EFinanceira);
        Assert.NotNull(message.Payload);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenIdCertificadoIsMissing()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            EnvioLoteCriptografadoBuilder
                .Create()
                .ComChave("chave-criptografica-teste")
                .ComLote("lote-criptografado-teste")
                .Build());

        Assert.Equal("IdCertificado é obrigatório", exception.Message);
    }

    [Fact]
    public void Build_ShouldThrowException_WhenChaveIsMissing()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            EnvioLoteCriptografadoBuilder
                .Create()
                .ComIdCertificado("CERT-001")
                .ComLote("lote-criptografado-teste")
                .Build());

        Assert.Equal("Chave criptográfica é obrigatória", exception.Message);
    }
}

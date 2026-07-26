using Xunit;
using EFinanceira.Messages.Builders.Consultas.RetRERCT;

namespace EFinanceira.Messages.Tests.Builders.Consultas.RetRERCT;

public class RetRERCTBuilderTests
{
    [Fact]
    public void Create_ShouldCreateBuilderWithDefaultVersion()
    {
        var builder = RetRERCTBuilder.Create();
        Assert.NotNull(builder);
    }

    [Fact]
    public void Constructor_ShouldCreateBuilderWithSpecificVersion()
    {
        const string version = "v1_2_0";
        var builder = new RetRERCTBuilder(version);
        Assert.NotNull(builder);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithMinimalConfiguration()
    {
        var message = RetRERCTBuilder
            .Create()
            .ComDadosProcessamento(dados => dados
                .ComDataHoraProcessamento(System.DateTime.UtcNow)
                .ComStatus("0", "Sucesso"))
            .ComDadosEvento(evento => evento
                .ComIdentificacaoEvento(identificacao => identificacao
                    .ComNumeroRecibo("REC-001")))
            .Build();

        Assert.NotNull(message);
        Assert.Equal("v1_2_0", message.Version);
        Assert.Equal("eFinanceira", message.RootElementName);
        Assert.NotNull(message.Consulta);
        Assert.NotNull(message.Payload);
    }

    [Fact]
    public void Build_ShouldCreateValidMessage_WithComplexConfiguration()
    {
        var message = RetRERCTBuilder
            .Create()
            .ComDadosProcessamento(dados => dados
                .ComDataHoraProcessamento(System.DateTime.UtcNow)
                .ComStatus("0", "Sucesso"))
            .ComDadosEvento(evento => evento
                .ComIdentificacaoEvento(identificacao => identificacao
                    .ComNumeroRecibo("REC-001"))
                .ComIdentificacaoDeclarado(declarado => declarado
                    .ComCpfCnpj("00000000000100")))
            .Build();

        Assert.NotNull(message);
        Assert.Equal("v1_2_0", message.Version);
        Assert.Equal("eFinanceira", message.RootElementName);
    }
}

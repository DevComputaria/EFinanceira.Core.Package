using System.Xml;
using EFinanceira.Messages.Builders.Lotes.RetornoLoteEventosAssincrono;
using EFinanceira.Messages.Generated.Lotes.RetornoLoteEventosAssincrono;
using Xunit;

namespace EFinanceira.Messages.Tests.Builders.Lotes.RetornoLoteEventosAssincrono;

/// <summary>
/// Testes unitários para RetornoLoteEventosAssincronoBuilder
/// </summary>
public class RetornoLoteEventosAssincronoBuilderTests
{
    [Fact]
    public void Constructor_ShouldCreateValidBuilder()
    {
        // Arrange & Act
        var builder = new RetornoLoteEventosAssincronoBuilder();

        // Assert
        Assert.NotNull(builder);
        var message = builder.Build();
        Assert.NotNull(message);
        Assert.Equal("v1_0_0", message.Version);
        Assert.NotNull(message.EFinanceira);
        Assert.NotNull(message.EFinanceira.retornoLoteEventosAssincrono);
    }

    [Fact]
    public void Constructor_WithCustomVersion_ShouldSetVersion()
    {
        // Arrange
        var customVersion = "v1_1_0";

        // Act
        var builder = new RetornoLoteEventosAssincronoBuilder(customVersion);
        var message = builder.Build();

        // Assert
        Assert.Equal(customVersion, message.Version);
    }

    [Fact]
    public void ComCnpjDeclarante_ShouldSetCnpjDeclarante()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();
        var cnpj = "12345678000195";

        // Act
        var result = builder.ComCnpjDeclarante(cnpj);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.Equal(cnpj, message.EFinanceira.retornoLoteEventosAssincrono.cnpjDeclarante);
        Assert.Equal(cnpj, message.GetCnpjDeclarante());
    }

    [Fact]
    public void ComCnpjDeclarante_WithNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder.ComCnpjDeclarante(null!));
    }

    [Fact]
    public void ComStatus_ShouldSetStatus()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();
        var codigo = 0;
        var descricao = "Processado com sucesso";

        // Act
        var result = builder.ComStatus(codigo, descricao);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.Equal(codigo, message.EFinanceira.retornoLoteEventosAssincrono.status.cdResposta);
        Assert.Equal(descricao, message.EFinanceira.retornoLoteEventosAssincrono.status.descResposta);
    }

    [Fact]
    public void ComDadosRecepcao_ShouldSetDadosRecepcaoLote()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();
        var dhRecepcao = new DateTime(2024, 1, 15, 10, 0, 0);
        var versaoApp = "1.0.0";
        var protocolo = "PROT001";

        // Act
        var result = builder.ComDadosRecepcao(dhRecepcao, versaoApp, protocolo);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        var dadosRecepcao = message.EFinanceira.retornoLoteEventosAssincrono.dadosRecepcaoLote;
        Assert.NotNull(dadosRecepcao);
        Assert.Equal(dhRecepcao, dadosRecepcao.dhRecepcao);
        Assert.Equal(versaoApp, dadosRecepcao.versaoAplicativoRecepcao);
        Assert.Equal(protocolo, dadosRecepcao.protocoloEnvio);
        Assert.Equal(dadosRecepcao, message.GetDadosRecepcao());
        Assert.Equal(protocolo, message.GetProtocoloEnvio());
    }

    [Fact]
    public void ComDadosRecepcao_WithNullVersaoApp_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            builder.ComDadosRecepcao(DateTime.Now, null!, "protocolo"));
    }

    [Fact]
    public void ComDadosProcessamento_ShouldSetDadosProcessamentoLote()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();
        var dhProcessamento = new DateTime(2024, 1, 15, 10, 30, 0);
        var versaoApp = "2.0.0";

        // Act
        var result = builder.ComDadosProcessamento(dhProcessamento, versaoApp);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        var dadosProcessamento = message.EFinanceira.retornoLoteEventosAssincrono.dadosProcessamentoLote;
        Assert.NotNull(dadosProcessamento);
        Assert.Equal(dhProcessamento, dadosProcessamento.dhProcessamento);
        Assert.Equal(versaoApp, dadosProcessamento.versaoAplicativoProcessamentoLote);
        Assert.Equal(dadosProcessamento, message.GetDadosProcessamento());
    }

    [Fact]
    public void AdicionarOcorrencia_ShouldAddOcorrencia()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();
        var codigo = "E001";
        var descricao = "Erro de validação";
        byte tipo = 1;
        var localizacao = "cnpjDeclarante";

        // Act
        var result = builder.AdicionarOcorrencia(codigo, descricao, tipo, localizacao);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        var ocorrencias = message.EFinanceira.retornoLoteEventosAssincrono.status.ocorrencias;
        Assert.NotNull(ocorrencias);
        Assert.Single(ocorrencias);
        Assert.Equal(codigo, ocorrencias[0].codigo);
        Assert.Equal(descricao, ocorrencias[0].descricao);
        Assert.Equal(tipo, ocorrencias[0].tipo);
        Assert.Equal(localizacao, ocorrencias[0].localizacao);
    }

    [Fact]
    public void AdicionarOcorrencia_MultipleOcorrencias_ShouldAddAll()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();

        // Act
        var result = builder
            .AdicionarOcorrencia("E001", "Erro 1", 1)
            .AdicionarOcorrencia("W001", "Aviso 1", 2)
            .AdicionarOcorrencia("E002", "Erro 2", 1);
        var message = result.Build();

        // Assert
        var ocorrencias = message.EFinanceira.retornoLoteEventosAssincrono.status.ocorrencias;
        Assert.NotNull(ocorrencias);
        Assert.Equal(3, ocorrencias.Length);
    }

    [Fact]
    public void AdicionarOcorrencias_ShouldAddMultipleOcorrencias()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();
        var ocorrencias = new List<TOcorrenciasOcorrencia>
        {
            new() { codigo = "E001", descricao = "Erro 1", tipo = 1 },
            new() { codigo = "W001", descricao = "Aviso 1", tipo = 2 }
        };

        // Act
        var result = builder.AdicionarOcorrencias(ocorrencias);
        var message = result.Build();

        // Assert
        var resultOcorrencias = message.EFinanceira.retornoLoteEventosAssincrono.status.ocorrencias;
        Assert.NotNull(resultOcorrencias);
        Assert.Equal(2, resultOcorrencias.Length);
    }

    [Fact]
    public void AdicionarEvento_WithXmlElement_ShouldAddEvento()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();
        var xmlDoc = new XmlDocument();
        var xmlElement = xmlDoc.CreateElement("evento");
        xmlElement.InnerText = "teste";
        var id = "EVT001";

        // Act
        var result = builder.AdicionarEvento(xmlElement, id);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        var eventos = message.EFinanceira.retornoLoteEventosAssincrono.retornoEventos.evento;
        Assert.NotNull(eventos);
        Assert.Single(eventos);
        Assert.Equal(id, eventos[0].id);
        Assert.Equal(xmlElement, eventos[0].Any);
    }

    [Fact]
    public void AdicionarEvento_WithXmlString_ShouldAddEvento()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();
        var xmlString = "<evento>teste</evento>";
        var id = "EVT001";

        // Act
        var result = builder.AdicionarEvento(xmlString, id);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        var eventos = message.EFinanceira.retornoLoteEventosAssincrono.retornoEventos.evento;
        Assert.NotNull(eventos);
        Assert.Single(eventos);
        Assert.Equal(id, eventos[0].id);
        Assert.NotNull(eventos[0].Any);
    }

    [Fact]
    public void ComId_ShouldSetId()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();
        var id = "LOTE001";

        // Act
        var result = builder.ComId(id);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.Equal(id, message.EFinanceira.retornoLoteEventosAssincrono.id);
        Assert.Equal(id, message.IdValue);
    }

    [Fact]
    public void ComAssinatura_ShouldSetSignature()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();
        var signature = new SignatureType();

        // Act
        var result = builder.ComAssinatura(signature);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.Same(signature, message.EFinanceira.Signature);
    }

    [Fact]
    public void LimparOcorrencias_ShouldRemoveAllOcorrencias()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();
        builder.AdicionarOcorrencia("E001", "Erro 1", 1);
        builder.AdicionarOcorrencia("W001", "Aviso 1", 2);

        // Act
        var result = builder.LimparOcorrencias();
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.Null(message.EFinanceira.retornoLoteEventosAssincrono.status.ocorrencias);
    }

    [Fact]
    public void LimparEventos_ShouldRemoveAllEventos()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();
        builder.AdicionarEvento("<evento>teste</evento>", "EVT001");

        // Act
        var result = builder.LimparEventos();
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.Null(message.EFinanceira.retornoLoteEventosAssincrono.retornoEventos.evento);
    }

    [Fact]
    public void ComSucesso_ShouldSetSuccessStatus()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();
        var descricao = "Processado com sucesso";

        // Act
        var result = builder.ComSucesso(descricao);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.Equal(0, message.EFinanceira.retornoLoteEventosAssincrono.status.cdResposta);
        Assert.Equal(descricao, message.EFinanceira.retornoLoteEventosAssincrono.status.descResposta);
        Assert.True(message.IsSuccessful());
    }

    [Fact]
    public void ComErro_ShouldAddErrorOcorrencia()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();
        var codigo = "E001";
        var descricao = "Erro de validação";
        var localizacao = "campo";

        // Act
        var result = builder.ComErro(codigo, descricao, localizacao);
        var message = result.Build();

        // Assert
        var erros = message.GetErros().ToList();
        Assert.Single(erros);
        Assert.Equal((byte)1, erros[0].tipo);
        Assert.Equal(codigo, erros[0].codigo);
        Assert.Equal(descricao, erros[0].descricao);
        Assert.Equal(localizacao, erros[0].localizacao);
    }

    [Fact]
    public void ComAviso_ShouldAddWarningOcorrencia()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();
        var codigo = "W001";
        var descricao = "Aviso de validação";

        // Act
        var result = builder.ComAviso(codigo, descricao);
        var message = result.Build();

        // Assert
        var avisos = message.GetAvisos().ToList();
        Assert.Single(avisos);
        Assert.Equal((byte)2, avisos[0].tipo);
        Assert.Equal(codigo, avisos[0].codigo);
        Assert.Equal(descricao, avisos[0].descricao);
    }

    [Fact]
    public void ComErroProcessamento_ShouldSetErrorStatus()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();
        var codigoErro = 1;
        var descricao = "Erro no processamento";

        // Act
        var result = builder.ComErroProcessamento(codigoErro, descricao);
        var message = result.Build();

        // Assert
        Assert.Equal(codigoErro, message.EFinanceira.retornoLoteEventosAssincrono.status.cdResposta);
        Assert.Equal(descricao, message.EFinanceira.retornoLoteEventosAssincrono.status.descResposta);
        Assert.False(message.IsSuccessful());
    }

    [Fact]
    public void GetErros_ShouldReturnOnlyErrors()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();
        builder.AdicionarOcorrencia("E001", "Erro 1", 1);
        builder.AdicionarOcorrencia("W001", "Aviso 1", 2);
        builder.AdicionarOcorrencia("E002", "Erro 2", 1);
        var message = builder.Build();

        // Act
        var erros = message.GetErros().ToList();

        // Assert
        Assert.Equal(2, erros.Count);
        Assert.All(erros, e => Assert.Equal((byte)1, e.tipo));
    }

    [Fact]
    public void GetAvisos_ShouldReturnOnlyWarnings()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();
        builder.AdicionarOcorrencia("E001", "Erro 1", 1);
        builder.AdicionarOcorrencia("W001", "Aviso 1", 2);
        builder.AdicionarOcorrencia("W002", "Aviso 2", 2);
        var message = builder.Build();

        // Act
        var avisos = message.GetAvisos().ToList();

        // Assert
        Assert.Equal(2, avisos.Count);
        Assert.All(avisos, a => Assert.Equal((byte)2, a.tipo));
    }

    [Fact]
    public void GetEventos_ShouldReturnAllEventos()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();
        builder.AdicionarEvento("<evento1>teste1</evento1>", "EVT001");
        builder.AdicionarEvento("<evento2>teste2</evento2>", "EVT002");
        var message = builder.Build();

        // Act
        var eventos = message.GetEventos().ToList();

        // Assert
        Assert.Equal(2, eventos.Count);
        Assert.Equal("EVT001", eventos[0].id);
        Assert.Equal("EVT002", eventos[1].id);
    }

    [Fact]
    public void IsSuccessful_WithSuccessCode_ShouldReturnTrue()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();
        var message = builder.ComStatus(0, "Sucesso").Build();

        // Act & Assert
        Assert.True(message.IsSuccessful());
    }

    [Fact]
    public void IsSuccessful_WithErrorCode_ShouldReturnFalse()
    {
        // Arrange
        var builder = new RetornoLoteEventosAssincronoBuilder();
        var message = builder.ComStatus(1, "Erro").Build();

        // Act & Assert
        Assert.False(message.IsSuccessful());
    }

    [Fact]
    public void FromXml_WithValidXml_ShouldCreateBuilder()
    {
        // Arrange
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<eFinanceira xmlns=""http://www.eFinanceira.gov.br/schemas/retornoLoteEventosAssincrono/v1_0_0"">
    <retornoLoteEventosAssincrono id=""LOTE001"">
        <cnpjDeclarante>12345678000195</cnpjDeclarante>
        <status>
            <cdResposta>0</cdResposta>
            <descResposta>Sucesso</descResposta>
        </status>
        <dadosRecepcaoLote>
            <dhRecepcao>2024-01-15T10:00:00</dhRecepcao>
            <versaoAplicativoRecepcao>1.0.0</versaoAplicativoRecepcao>
            <protocoloEnvio>PROT001</protocoloEnvio>
        </dadosRecepcaoLote>
    </retornoLoteEventosAssincrono>
</eFinanceira>";

        // Act
        var builder = RetornoLoteEventosAssincronoBuilder.FromXml(xml);
        var message = builder.Build();

        // Assert
        Assert.NotNull(builder);
        Assert.Equal("12345678000195", message.GetCnpjDeclarante());
        Assert.Equal(0, message.EFinanceira.retornoLoteEventosAssincrono.status.cdResposta);
        Assert.Equal("LOTE001", message.EFinanceira.retornoLoteEventosAssincrono.id);
        Assert.Equal("PROT001", message.GetProtocoloEnvio());
        Assert.True(message.IsSuccessful());
    }

    [Fact]
    public void FromXml_WithInvalidXml_ShouldThrowException()
    {
        // Arrange
        var invalidXml = "<invalid>xml</invalid>";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            RetornoLoteEventosAssincronoBuilder.FromXml(invalidXml));
    }

    [Fact]
    public void FromEFinanceira_ShouldCreateBuilder()
    {
        // Arrange
        var eFinanceira = new eFinanceira
        {
            retornoLoteEventosAssincrono = new eFinanceiraRetornoLoteEventosAssincrono
            {
                id = "LOTE001",
                cnpjDeclarante = "12345678000195",
                status = new TStatus
                {
                    cdResposta = 0,
                    descResposta = "Sucesso"
                }
            }
        };

        // Act
        var builder = RetornoLoteEventosAssincronoBuilder.FromEFinanceira(eFinanceira);
        var message = builder.Build();

        // Assert
        Assert.NotNull(builder);
        Assert.Equal("12345678000195", message.GetCnpjDeclarante());
        Assert.Equal(0, message.EFinanceira.retornoLoteEventosAssincrono.status.cdResposta);
        Assert.Equal("LOTE001", message.EFinanceira.retornoLoteEventosAssincrono.id);
    }

    [Fact]
    public void FluentInterface_CompleteExample_ShouldWork()
    {
        // Arrange & Act
        var message = new RetornoLoteEventosAssincronoBuilder()
            .ComCnpjDeclarante("12345678000195")
            .ComDadosRecepcao(
                new DateTime(2024, 1, 15, 10, 0, 0),
                "1.0.0",
                "PROT001"
            )
            .ComDadosProcessamento(
                new DateTime(2024, 1, 15, 10, 30, 0),
                "2.0.0"
            )
            .ComSucesso("Processado com sucesso")
            .AdicionarEvento("<evento>teste</evento>", "EVT001")
            .ComAviso("W001", "Campo opcional não preenchido")
            .ComId("LOTE001")
            .Build();

        // Assert
        Assert.Equal("12345678000195", message.GetCnpjDeclarante());
        Assert.Equal("PROT001", message.GetProtocoloEnvio());
        Assert.Equal("1.0.0", message.GetDadosRecepcao()?.versaoAplicativoRecepcao);
        Assert.Equal("2.0.0", message.GetDadosProcessamento()?.versaoAplicativoProcessamentoLote);
        Assert.True(message.IsSuccessful());
        Assert.Single(message.GetAvisos());
        Assert.Empty(message.GetErros());
        Assert.Single(message.GetEventos());
        Assert.Equal("LOTE001", message.EFinanceira.retornoLoteEventosAssincrono.id);
    }

    [Fact]
    public void Message_Properties_ShouldBeCorrect()
    {
        // Arrange
        var message = new RetornoLoteEventosAssincronoMessage();

        // Act & Assert
        Assert.Equal("v1_0_0", message.Version);
        Assert.Equal("eFinanceira", message.RootElementName);
        Assert.Equal("id", message.IdAttributeName);
        Assert.Same(message.EFinanceira, message.Payload);
    }

    [Fact]
    public void Message_Constructor_WithParameters_ShouldSetProperties()
    {
        // Arrange
        var version = "v1_1_0";
        var eFinanceira = new eFinanceira();

        // Act
        var message = new RetornoLoteEventosAssincronoMessage(version, eFinanceira);

        // Assert
        Assert.Equal(version, message.Version);
        Assert.Same(eFinanceira, message.EFinanceira);
        Assert.Same(eFinanceira, message.Payload);
    }

    [Fact]
    public void Message_IdValue_ShouldReturnCorrectId()
    {
        // Arrange
        var eFinanceira = new eFinanceira
        {
            retornoLoteEventosAssincrono = new eFinanceiraRetornoLoteEventosAssincrono
            {
                id = "LOTE123"
            }
        };
        var message = new RetornoLoteEventosAssincronoMessage("v1_0_0", eFinanceira);

        // Act & Assert
        Assert.Equal("LOTE123", message.IdValue);
    }
}
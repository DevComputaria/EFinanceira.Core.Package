using System.Xml;
using EFinanceira.Messages.Builders.Lotes.RetornoLoteEventos;
using EFinanceira.Messages.Generated.Lotes.RetornoLoteEventos_v1_2_0;
using Xunit;

namespace EFinanceira.Messages.Tests.Builders.Lotes.RetornoLoteEventos;

/// <summary>
/// Testes unitários para RetornoLoteEventosBuilder
/// </summary>
public class RetornoLoteEventosBuilderTests
{
    [Fact]
    public void Constructor_ShouldCreateValidBuilder()
    {
        // Arrange & Act
        var builder = new RetornoLoteEventosBuilder();

        // Assert
        Assert.NotNull(builder);
        var message = builder.Build();
        Assert.NotNull(message);
        Assert.Equal("v1_2_0", message.Version);
        Assert.NotNull(message.EFinanceira);
    }

    [Fact]
    public void Constructor_WithCustomVersion_ShouldSetVersion()
    {
        // Arrange
        var customVersion = "v2_0_0";

        // Act
        var builder = new RetornoLoteEventosBuilder(customVersion);
        var message = builder.Build();

        // Assert
        Assert.Equal(customVersion, message.Version);
    }

    [Fact]
    public void ComIdTransmissor_ShouldSetIdTransmissor()
    {
        // Arrange
        var builder = new RetornoLoteEventosBuilder();
        var idTransmissor = "12345678000195";

        // Act
        var result = builder.ComIdTransmissor(idTransmissor);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.Equal(idTransmissor, message.EFinanceira.retornoLoteEventos.ideTransmissor.IdTransmissor);
    }

    [Fact]
    public void ComIdTransmissor_WithNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new RetornoLoteEventosBuilder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder.ComIdTransmissor(null!));
    }

    [Fact]
    public void ComStatus_ShouldSetStatus()
    {
        // Arrange
        var builder = new RetornoLoteEventosBuilder();
        var codigo = "0";
        var descricao = "Processado com sucesso";

        // Act
        var result = builder.ComStatus(codigo, descricao);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.Equal(codigo, message.EFinanceira.retornoLoteEventos.status.cdStatus);
        Assert.Equal(descricao, message.EFinanceira.retornoLoteEventos.status.descRetorno);
    }

    [Fact]
    public void ComStatus_WithNullCodigo_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new RetornoLoteEventosBuilder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder.ComStatus(null!, "descricao"));
    }

    [Fact]
    public void AdicionarOcorrencia_ShouldAddOcorrencia()
    {
        // Arrange
        var builder = new RetornoLoteEventosBuilder();
        var tipo = "1";
        var codigo = "E001";
        var descricao = "Erro de validação";
        var localizacao = "evento[1]";

        // Act
        var result = builder.AdicionarOcorrencia(tipo, codigo, descricao, localizacao);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        var ocorrencias = message.EFinanceira.retornoLoteEventos.status.dadosRegistroOcorrenciaLote;
        Assert.NotNull(ocorrencias);
        Assert.Single(ocorrencias);
        Assert.Equal(tipo, ocorrencias[0].tipo);
        Assert.Equal(codigo, ocorrencias[0].codigo);
        Assert.Equal(descricao, ocorrencias[0].descricao);
        Assert.Equal(localizacao, ocorrencias[0].localizacaoErroAviso);
    }

    [Fact]
    public void AdicionarOcorrencia_MultipleOcorrencias_ShouldAddAll()
    {
        // Arrange
        var builder = new RetornoLoteEventosBuilder();

        // Act
        var result = builder
            .AdicionarOcorrencia("1", "E001", "Erro 1")
            .AdicionarOcorrencia("2", "W001", "Aviso 1")
            .AdicionarOcorrencia("1", "E002", "Erro 2");
        var message = result.Build();

        // Assert
        var ocorrencias = message.EFinanceira.retornoLoteEventos.status.dadosRegistroOcorrenciaLote;
        Assert.NotNull(ocorrencias);
        Assert.Equal(3, ocorrencias.Length);
    }

    [Fact]
    public void AdicionarOcorrencias_ShouldAddMultipleOcorrencias()
    {
        // Arrange
        var builder = new RetornoLoteEventosBuilder();
        var ocorrencias = new List<TRegistroOcorrenciasOcorrencias>
        {
            new() { tipo = "1", codigo = "E001", descricao = "Erro 1" },
            new() { tipo = "2", codigo = "W001", descricao = "Aviso 1" }
        };

        // Act
        var result = builder.AdicionarOcorrencias(ocorrencias);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        var resultOcorrencias = message.EFinanceira.retornoLoteEventos.status.dadosRegistroOcorrenciaLote;
        Assert.NotNull(resultOcorrencias);
        Assert.Equal(2, resultOcorrencias.Length);
    }

    [Fact]
    public void ComEventos_ShouldSetEventos()
    {
        // Arrange
        var builder = new RetornoLoteEventosBuilder();
        var eventos = new[]
        {
            new TArquivoeFinanceira { id = "evt1" },
            new TArquivoeFinanceira { id = "evt2" }
        };

        // Act
        var result = builder.ComEventos(eventos);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        var retornoEventos = message.EFinanceira.retornoLoteEventos.retornoEventos.evento;
        Assert.NotNull(retornoEventos);
        Assert.Equal(2, retornoEventos.Length);
        Assert.Equal("evt1", retornoEventos[0].id);
        Assert.Equal("evt2", retornoEventos[1].id);
    }

    [Fact]
    public void AdicionarEvento_WithXmlElement_ShouldAddEvento()
    {
        // Arrange
        var builder = new RetornoLoteEventosBuilder();
        var xmlDoc = new XmlDocument();
        var element = xmlDoc.CreateElement("evento");
        element.InnerText = "test content";
        var eventoId = "test-id";

        // Act
        var result = builder.AdicionarEvento(element, eventoId);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        var eventos = message.EFinanceira.retornoLoteEventos.retornoEventos.evento;
        Assert.NotNull(eventos);
        Assert.Single(eventos);
        Assert.Equal(eventoId, eventos[0].id);
        Assert.Equal(element, eventos[0].Any);
    }

    [Fact]
    public void AdicionarEvento_WithXmlString_ShouldAddEvento()
    {
        // Arrange
        var builder = new RetornoLoteEventosBuilder();
        var xmlString = "<evento><test>content</test></evento>";
        var eventoId = "test-id";

        // Act
        var result = builder.AdicionarEvento(xmlString, eventoId);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        var eventos = message.EFinanceira.retornoLoteEventos.retornoEventos.evento;
        Assert.NotNull(eventos);
        Assert.Single(eventos);
        Assert.Equal(eventoId, eventos[0].id);
        Assert.NotNull(eventos[0].Any);
    }

    [Fact]
    public void AdicionarEvento_WithInvalidXml_ShouldThrowArgumentException()
    {
        // Arrange
        var builder = new RetornoLoteEventosBuilder();
        var invalidXml = "<invalid>";

        // Act & Assert
        Assert.Throws<XmlException>(() => builder.AdicionarEvento(invalidXml));
    }

    [Fact]
    public void ComId_ShouldSetId()
    {
        // Arrange
        var builder = new RetornoLoteEventosBuilder();
        var id = "LOTE001";

        // Act
        var result = builder.ComId(id);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.Equal(id, message.EFinanceira.retornoLoteEventos.id);
    }

    [Fact]
    public void ComAssinatura_ShouldSetSignature()
    {
        // Arrange
        var builder = new RetornoLoteEventosBuilder();
        var signature = new SignatureType();

        // Act
        var result = builder.ComAssinatura(signature);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.Equal(signature, message.EFinanceira.Signature);
    }

    [Fact]
    public void LimparEventos_ShouldClearEventos()
    {
        // Arrange
        var builder = new RetornoLoteEventosBuilder();
        var xmlDoc = new XmlDocument();
        var element = xmlDoc.CreateElement("evento");

        builder.AdicionarEvento(element, "test");

        // Act
        var result = builder.LimparEventos();
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.Null(message.EFinanceira.retornoLoteEventos.retornoEventos?.evento);
    }

    [Fact]
    public void LimparOcorrencias_ShouldClearOcorrencias()
    {
        // Arrange
        var builder = new RetornoLoteEventosBuilder();
        builder.AdicionarOcorrencia("1", "E001", "Erro");

        // Act
        var result = builder.LimparOcorrencias();
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.Null(message.EFinanceira.retornoLoteEventos.status?.dadosRegistroOcorrenciaLote);
    }

    [Fact]
    public void FromXml_WithValidXml_ShouldCreateBuilder()
    {
        // Arrange
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<eFinanceira xmlns=""http://www.eFinanceira.gov.br/schemas/retornoLoteEventos/v1_2_0"">
    <retornoLoteEventos id=""LOTE001"">
        <ideTransmissor>
            <IdTransmissor>12345678000195</IdTransmissor>
        </ideTransmissor>
        <status>
            <cdStatus>0</cdStatus>
            <descRetorno>Processado com sucesso</descRetorno>
        </status>
    </retornoLoteEventos>
</eFinanceira>";

        // Act
        var builder = RetornoLoteEventosBuilder.FromXml(xml);
        var message = builder.Build();

        // Assert
        Assert.NotNull(builder);
        Assert.Equal("12345678000195", message.EFinanceira.retornoLoteEventos.ideTransmissor.IdTransmissor);
        Assert.Equal("0", message.EFinanceira.retornoLoteEventos.status.cdStatus);
        Assert.Equal("LOTE001", message.EFinanceira.retornoLoteEventos.id);
    }

    [Fact]
    public void FromEFinanceira_WithValidObject_ShouldCreateBuilder()
    {
        // Arrange
        var eFinanceira = new eFinanceira
        {
            retornoLoteEventos = new eFinanceiraRetornoLoteEventos
            {
                id = "LOTE002",
                ideTransmissor = new TIdeTransmissor { IdTransmissor = "98765432000198" },
                status = new TStatus { cdStatus = "1", descRetorno = "Processado com avisos" }
            }
        };

        // Act
        var builder = RetornoLoteEventosBuilder.FromEFinanceira(eFinanceira);
        var message = builder.Build();

        // Assert
        Assert.NotNull(builder);
        Assert.Equal("98765432000198", message.EFinanceira.retornoLoteEventos.ideTransmissor.IdTransmissor);
        Assert.Equal("1", message.EFinanceira.retornoLoteEventos.status.cdStatus);
        Assert.Equal("LOTE002", message.EFinanceira.retornoLoteEventos.id);
    }
}

/// <summary>
/// Testes para as extensões do RetornoLoteEventosBuilder
/// </summary>
public class RetornoLoteEventosBuilderExtensionsTests
{
    [Fact]
    public void ComErro_ShouldAddErrorOcorrencia()
    {
        // Arrange
        var builder = new RetornoLoteEventosBuilder();
        var codigo = "E001";
        var descricao = "Erro crítico";
        var localizacao = "evento[1]";

        // Act
        var result = builder.ComErro(codigo, descricao, localizacao);
        var message = result.Build();

        // Assert
        var ocorrencias = message.EFinanceira.retornoLoteEventos.status.dadosRegistroOcorrenciaLote;
        Assert.NotNull(ocorrencias);
        Assert.Single(ocorrencias);
        Assert.Equal("1", ocorrencias[0].tipo);
        Assert.Equal(codigo, ocorrencias[0].codigo);
    }

    [Fact]
    public void ComAviso_ShouldAddWarningOcorrencia()
    {
        // Arrange
        var builder = new RetornoLoteEventosBuilder();
        var codigo = "W001";
        var descricao = "Aviso informativo";

        // Act
        var result = builder.ComAviso(codigo, descricao);
        var message = result.Build();

        // Assert
        var ocorrencias = message.EFinanceira.retornoLoteEventos.status.dadosRegistroOcorrenciaLote;
        Assert.NotNull(ocorrencias);
        Assert.Single(ocorrencias);
        Assert.Equal("2", ocorrencias[0].tipo);
        Assert.Equal(codigo, ocorrencias[0].codigo);
    }

    [Fact]
    public void ComSucesso_ShouldSetSuccessStatus()
    {
        // Arrange
        var builder = new RetornoLoteEventosBuilder();
        var descricao = "Processamento concluído";

        // Act
        var result = builder.ComSucesso(descricao);
        var message = result.Build();

        // Assert
        Assert.Equal("0", message.EFinanceira.retornoLoteEventos.status.cdStatus);
        Assert.Equal(descricao, message.EFinanceira.retornoLoteEventos.status.descRetorno);
    }

    [Fact]
    public void ComErroProcessamento_ShouldSetErrorStatus()
    {
        // Arrange
        var builder = new RetornoLoteEventosBuilder();
        var codigoErro = "999";
        var descricao = "Erro fatal";

        // Act
        var result = builder.ComErroProcessamento(codigoErro, descricao);
        var message = result.Build();

        // Assert
        Assert.Equal(codigoErro, message.EFinanceira.retornoLoteEventos.status.cdStatus);
        Assert.Equal(descricao, message.EFinanceira.retornoLoteEventos.status.descRetorno);
    }

    [Fact]
    public void IsSuccessful_WithSuccessStatus_ShouldReturnTrue()
    {
        // Arrange
        var message = new RetornoLoteEventosBuilder()
            .ComSucesso()
            .Build();

        // Act & Assert
        Assert.True(message.IsSuccessful());
    }

    [Fact]
    public void IsSuccessful_WithErrorStatus_ShouldReturnFalse()
    {
        // Arrange
        var message = new RetornoLoteEventosBuilder()
            .ComStatus("1", "Erro")
            .Build();

        // Act & Assert
        Assert.False(message.IsSuccessful());
    }

    [Fact]
    public void GetErros_ShouldReturnOnlyErrors()
    {
        // Arrange
        var message = new RetornoLoteEventosBuilder()
            .ComErro("E001", "Erro 1")
            .ComErro("E002", "Erro 2")
            .ComAviso("W001", "Aviso 1")
            .Build();

        // Act
        var erros = message.GetErros().ToList();

        // Assert
        Assert.Equal(2, erros.Count);
        Assert.All(erros, e => Assert.Equal("1", e.tipo));
    }

    [Fact]
    public void GetAvisos_ShouldReturnOnlyWarnings()
    {
        // Arrange
        var message = new RetornoLoteEventosBuilder()
            .ComErro("E001", "Erro 1")
            .ComAviso("W001", "Aviso 1")
            .ComAviso("W002", "Aviso 2")
            .Build();

        // Act
        var avisos = message.GetAvisos().ToList();

        // Assert
        Assert.Equal(2, avisos.Count);
        Assert.All(avisos, a => Assert.Equal("2", a.tipo));
    }

    [Fact]
    public void GetEventos_ShouldReturnAllEvents()
    {
        // Arrange
        var eventos = new[]
        {
            new TArquivoeFinanceira { id = "evt1" },
            new TArquivoeFinanceira { id = "evt2" },
            new TArquivoeFinanceira { id = "evt3" }
        };

        var message = new RetornoLoteEventosBuilder()
            .ComEventos(eventos)
            .Build();

        // Act
        var resultEventos = message.GetEventos().ToList();

        // Assert
        Assert.Equal(3, resultEventos.Count);
        Assert.Equal("evt1", resultEventos[0].id);
        Assert.Equal("evt2", resultEventos[1].id);
        Assert.Equal("evt3", resultEventos[2].id);
    }
}
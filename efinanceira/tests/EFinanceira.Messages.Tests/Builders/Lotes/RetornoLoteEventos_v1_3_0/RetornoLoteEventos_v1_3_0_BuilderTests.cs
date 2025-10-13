using System.Xml;
using EFinanceira.Messages.Builders.Lotes.RetornoLoteEventos_v1_3_0;
using EFinanceira.Messages.Generated.Lotes.RetornoLoteEventos_v1_3_0;
using Xunit;

namespace EFinanceira.Messages.Tests.Builders.Lotes.RetornoLoteEventos_v1_3_0;

/// <summary>
/// Testes unitários para RetornoLoteEventos_v1_3_0_Builder
/// </summary>
public class RetornoLoteEventos_v1_3_0_BuilderTests
{
    [Fact]
    public void Constructor_ShouldCreateValidBuilder()
    {
        // Arrange & Act
        var builder = new RetornoLoteEventos_v1_3_0_Builder();

        // Assert
        Assert.NotNull(builder);
        var message = builder.Build();
        Assert.NotNull(message);
        Assert.Equal("v1_3_0", message.Version);
        Assert.NotNull(message.EFinanceira);
        Assert.NotNull(message.EFinanceira.retornoEvento);
    }

    [Fact]
    public void Constructor_WithCustomVersion_ShouldSetVersion()
    {
        // Arrange
        var customVersion = "v1_3_1";

        // Act
        var builder = new RetornoLoteEventos_v1_3_0_Builder(customVersion);
        var message = builder.Build();

        // Assert
        Assert.Equal(customVersion, message.Version);
    }

    [Fact]
    public void ComEmpresaDeclarante_ShouldSetCnpjEmpresaDeclarante()
    {
        // Arrange
        var builder = new RetornoLoteEventos_v1_3_0_Builder();
        var cnpj = "12345678000195";

        // Act
        var result = builder.ComEmpresaDeclarante(cnpj);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.Equal(cnpj, message.EFinanceira.retornoEvento.identificacaoEmpresaDeclarante.cnpjEmpresaDeclarante);
        Assert.Equal(cnpj, message.GetCnpjEmpresaDeclarante());
    }

    [Fact]
    public void ComEmpresaDeclarante_WithNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new RetornoLoteEventos_v1_3_0_Builder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder.ComEmpresaDeclarante(null!));
    }

    [Fact]
    public void ComDadosRecepcao_ShouldSetDadosRecepcaoEvento()
    {
        // Arrange
        var builder = new RetornoLoteEventos_v1_3_0_Builder();
        var dhRecepcao = new DateTime(2024, 1, 15, 10, 0, 0);
        var dhProcessamento = new DateTime(2024, 1, 15, 10, 5, 0);
        var tipoEvento = "F200";
        var idEvento = "EVT001";
        var hash = "ABC123DEF456";
        var nrRecibo = "REC001";

        // Act
        var result = builder.ComDadosRecepcao(dhRecepcao, dhProcessamento, tipoEvento, idEvento, hash, nrRecibo);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        var dadosRecepcao = message.EFinanceira.retornoEvento.dadosRecepcaoEvento;
        Assert.NotNull(dadosRecepcao);
        Assert.Equal(dhRecepcao, dadosRecepcao.dhRecepcao);
        Assert.Equal(dhProcessamento, dadosRecepcao.dhProcessamento);
        Assert.Equal(tipoEvento, dadosRecepcao.tipoEvento);
        Assert.Equal(idEvento, dadosRecepcao.idEvento);
        Assert.Equal(hash, dadosRecepcao.hash);
        Assert.Equal(nrRecibo, dadosRecepcao.nrRecibo);
        Assert.Equal(dadosRecepcao, message.GetDadosRecepcao());
    }

    [Fact]
    public void ComDadosRecepcao_WithNullTipoEvento_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new RetornoLoteEventos_v1_3_0_Builder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            builder.ComDadosRecepcao(DateTime.Now, DateTime.Now, null!, "id", "hash", "recibo"));
    }

    [Fact]
    public void ComStatus_ShouldSetStatus()
    {
        // Arrange
        var builder = new RetornoLoteEventos_v1_3_0_Builder();
        var codigo = "0";
        var descricao = "Processado com sucesso";

        // Act
        var result = builder.ComStatus(codigo, descricao);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.Equal(codigo, message.EFinanceira.retornoEvento.status.cdRetorno);
        Assert.Equal(descricao, message.EFinanceira.retornoEvento.status.descRetorno);
    }

    [Fact]
    public void ComStatus_WithNullCodigo_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new RetornoLoteEventos_v1_3_0_Builder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder.ComStatus(null!, "descricao"));
    }

    [Fact]
    public void AdicionarOcorrencia_ShouldAddOcorrencia()
    {
        // Arrange
        var builder = new RetornoLoteEventos_v1_3_0_Builder();
        var tipo = "1";
        var codigo = "E001";
        var descricao = "Erro de validação";
        var localizacao = "identificacaoEmpresaDeclarante";

        // Act
        var result = builder.AdicionarOcorrencia(tipo, codigo, descricao, localizacao);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        var ocorrencias = message.EFinanceira.retornoEvento.status.dadosRegistroOcorrenciaEvento;
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
        var builder = new RetornoLoteEventos_v1_3_0_Builder();

        // Act
        var result = builder
            .AdicionarOcorrencia("1", "E001", "Erro 1")
            .AdicionarOcorrencia("2", "W001", "Aviso 1")
            .AdicionarOcorrencia("1", "E002", "Erro 2");
        var message = result.Build();

        // Assert
        var ocorrencias = message.EFinanceira.retornoEvento.status.dadosRegistroOcorrenciaEvento;
        Assert.NotNull(ocorrencias);
        Assert.Equal(3, ocorrencias.Length);
    }

    [Fact]
    public void AdicionarOcorrencias_ShouldAddMultipleOcorrencias()
    {
        // Arrange
        var builder = new RetornoLoteEventos_v1_3_0_Builder();
        var ocorrencias = new List<TRegistroOcorrenciasOcorrencias>
        {
            new() { tipo = "1", codigo = "E001", descricao = "Erro 1" },
            new() { tipo = "2", codigo = "W001", descricao = "Aviso 1" }
        };

        // Act
        var result = builder.AdicionarOcorrencias(ocorrencias);
        var message = result.Build();

        // Assert
        var resultOcorrencias = message.EFinanceira.retornoEvento.status.dadosRegistroOcorrenciaEvento;
        Assert.NotNull(resultOcorrencias);
        Assert.Equal(2, resultOcorrencias.Length);
    }

    [Fact]
    public void ComReciboEntrega_ShouldSetDadosReciboEntrega()
    {
        // Arrange
        var builder = new RetornoLoteEventos_v1_3_0_Builder();
        var nrRecibo = "REC001";
        var dhEntrega = new DateTime(2024, 1, 15, 10, 10, 0);

        // Act
        var result = builder.ComReciboEntrega(nrRecibo, dhEntrega);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        var reciboEntrega = message.EFinanceira.retornoEvento.dadosReciboEntrega;
        Assert.NotNull(reciboEntrega);
        Assert.Equal(nrRecibo, reciboEntrega.numeroRecibo);
        // Note: dhEntrega property doesn't exist in v1.3.0 schema
        Assert.Equal(reciboEntrega, message.GetReciboEntrega());
    }

    [Fact]
    public void ComReciboEntrega_WithoutDhEntrega_ShouldSetOnlyNrRecibo()
    {
        // Arrange
        var builder = new RetornoLoteEventos_v1_3_0_Builder();
        var nrRecibo = "REC001";

        // Act
        var result = builder.ComReciboEntrega(nrRecibo);
        var message = result.Build();

        // Assert
        var reciboEntrega = message.EFinanceira.retornoEvento.dadosReciboEntrega;
        Assert.NotNull(reciboEntrega);
        Assert.Equal(nrRecibo, reciboEntrega.numeroRecibo);
    }

    [Fact]
    public void ComId_ShouldSetId()
    {
        // Arrange
        var builder = new RetornoLoteEventos_v1_3_0_Builder();
        var id = "EVT001";

        // Act
        var result = builder.ComId(id);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.Equal(id, message.EFinanceira.retornoEvento.id);
    }

    [Fact]
    public void ComAssinatura_ShouldSetSignature()
    {
        // Arrange
        var builder = new RetornoLoteEventos_v1_3_0_Builder();
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
        var builder = new RetornoLoteEventos_v1_3_0_Builder();
        builder.AdicionarOcorrencia("1", "E001", "Erro 1");
        builder.AdicionarOcorrencia("2", "W001", "Aviso 1");

        // Act
        var result = builder.LimparOcorrencias();
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.Null(message.EFinanceira.retornoEvento.status.dadosRegistroOcorrenciaEvento);
    }

    [Fact]
    public void ComSucesso_ShouldSetSuccessStatus()
    {
        // Arrange
        var builder = new RetornoLoteEventos_v1_3_0_Builder();
        var descricao = "Processado com sucesso";

        // Act
        var result = builder.ComSucesso(descricao);
        var message = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.Equal("0", message.EFinanceira.retornoEvento.status.cdRetorno);
        Assert.Equal(descricao, message.EFinanceira.retornoEvento.status.descRetorno);
        Assert.True(message.IsSuccessful());
    }

    [Fact]
    public void ComErro_ShouldAddErrorOcorrencia()
    {
        // Arrange
        var builder = new RetornoLoteEventos_v1_3_0_Builder();
        var codigo = "E001";
        var descricao = "Erro de validação";
        var localizacao = "campo";

        // Act
        var result = builder.ComErro(codigo, descricao, localizacao);
        var message = result.Build();

        // Assert
        var erros = message.GetErros().ToList();
        Assert.Single(erros);
        Assert.Equal("1", erros[0].tipo);
        Assert.Equal(codigo, erros[0].codigo);
        Assert.Equal(descricao, erros[0].descricao);
        Assert.Equal(localizacao, erros[0].localizacaoErroAviso);
    }

    [Fact]
    public void ComAviso_ShouldAddWarningOcorrencia()
    {
        // Arrange
        var builder = new RetornoLoteEventos_v1_3_0_Builder();
        var codigo = "W001";
        var descricao = "Aviso de validação";

        // Act
        var result = builder.ComAviso(codigo, descricao);
        var message = result.Build();

        // Assert
        var avisos = message.GetAvisos().ToList();
        Assert.Single(avisos);
        Assert.Equal("2", avisos[0].tipo);
        Assert.Equal(codigo, avisos[0].codigo);
        Assert.Equal(descricao, avisos[0].descricao);
    }

    [Fact]
    public void ComErroProcessamento_ShouldSetErrorStatus()
    {
        // Arrange
        var builder = new RetornoLoteEventos_v1_3_0_Builder();
        var codigoErro = "001";
        var descricao = "Erro no processamento";

        // Act
        var result = builder.ComErroProcessamento(codigoErro, descricao);
        var message = result.Build();

        // Assert
        Assert.Equal(codigoErro, message.EFinanceira.retornoEvento.status.cdRetorno);
        Assert.Equal(descricao, message.EFinanceira.retornoEvento.status.descRetorno);
        Assert.False(message.IsSuccessful());
    }

    [Fact]
    public void GetErros_ShouldReturnOnlyErrors()
    {
        // Arrange
        var builder = new RetornoLoteEventos_v1_3_0_Builder();
        builder.AdicionarOcorrencia("1", "E001", "Erro 1");
        builder.AdicionarOcorrencia("2", "W001", "Aviso 1");
        builder.AdicionarOcorrencia("1", "E002", "Erro 2");
        var message = builder.Build();

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
        var builder = new RetornoLoteEventos_v1_3_0_Builder();
        builder.AdicionarOcorrencia("1", "E001", "Erro 1");
        builder.AdicionarOcorrencia("2", "W001", "Aviso 1");
        builder.AdicionarOcorrencia("2", "W002", "Aviso 2");
        var message = builder.Build();

        // Act
        var avisos = message.GetAvisos().ToList();

        // Assert
        Assert.Equal(2, avisos.Count);
        Assert.All(avisos, a => Assert.Equal("2", a.tipo));
    }

    [Fact]
    public void IsSuccessful_WithSuccessCode_ShouldReturnTrue()
    {
        // Arrange
        var builder = new RetornoLoteEventos_v1_3_0_Builder();
        var message = builder.ComStatus("0", "Sucesso").Build();

        // Act & Assert
        Assert.True(message.IsSuccessful());
    }

    [Fact]
    public void IsSuccessful_WithErrorCode_ShouldReturnFalse()
    {
        // Arrange
        var builder = new RetornoLoteEventos_v1_3_0_Builder();
        var message = builder.ComStatus("1", "Erro").Build();

        // Act & Assert
        Assert.False(message.IsSuccessful());
    }

    [Fact]
    public void FromXml_WithValidXml_ShouldCreateBuilder()
    {
        // Arrange
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<eFinanceira xmlns=""http://www.eFinanceira.gov.br/schemas/retornoEvento/v1_3_0"">
    <retornoEvento id=""EVT001"">
        <identificacaoEmpresaDeclarante>
            <cnpjEmpresaDeclarante>12345678000195</cnpjEmpresaDeclarante>
        </identificacaoEmpresaDeclarante>
        <dadosRecepcaoEvento>
            <dhRecepcao>2024-01-15T10:00:00</dhRecepcao>
            <dhProcessamento>2024-01-15T10:05:00</dhProcessamento>
            <tipoEvento>F200</tipoEvento>
            <idEvento>EVT001</idEvento>
            <hash>ABC123</hash>
            <nrRecibo>REC001</nrRecibo>
        </dadosRecepcaoEvento>
        <status>
            <cdRetorno>0</cdRetorno>
            <descRetorno>Sucesso</descRetorno>
        </status>
    </retornoEvento>
</eFinanceira>";

        // Act
        var builder = RetornoLoteEventos_v1_3_0_Builder.FromXml(xml);
        var message = builder.Build();

        // Assert
        Assert.NotNull(builder);
        Assert.Equal("12345678000195", message.GetCnpjEmpresaDeclarante());
        Assert.Equal("0", message.EFinanceira.retornoEvento.status.cdRetorno);
        Assert.Equal("EVT001", message.EFinanceira.retornoEvento.id);
        Assert.True(message.IsSuccessful());
    }

    [Fact]
    public void FromXml_WithInvalidXml_ShouldThrowException()
    {
        // Arrange
        var invalidXml = "<invalid>xml</invalid>";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            RetornoLoteEventos_v1_3_0_Builder.FromXml(invalidXml));
    }

    [Fact]
    public void FromEFinanceira_ShouldCreateBuilder()
    {
        // Arrange
        var eFinanceira = new eFinanceira
        {
            retornoEvento = new eFinanceiraRetornoEvento
            {
                id = "EVT001",
                identificacaoEmpresaDeclarante = new TIdeEmpresaDeclarante
                {
                    cnpjEmpresaDeclarante = "12345678000195"
                },
                status = new TStatus
                {
                    cdRetorno = "0",
                    descRetorno = "Sucesso"
                }
            }
        };

        // Act
        var builder = RetornoLoteEventos_v1_3_0_Builder.FromEFinanceira(eFinanceira);
        var message = builder.Build();

        // Assert
        Assert.NotNull(builder);
        Assert.Equal("12345678000195", message.GetCnpjEmpresaDeclarante());
        Assert.Equal("0", message.EFinanceira.retornoEvento.status.cdRetorno);
        Assert.Equal("EVT001", message.EFinanceira.retornoEvento.id);
    }

    [Fact]
    public void FluentInterface_CompleteExample_ShouldWork()
    {
        // Arrange & Act
        var message = new RetornoLoteEventos_v1_3_0_Builder()
            .ComEmpresaDeclarante("12345678000195")
            .ComDadosRecepcao(
                new DateTime(2024, 1, 15, 10, 0, 0),
                new DateTime(2024, 1, 15, 10, 5, 0),
                "F200",
                "EVT001",
                "ABC123DEF456",
                "REC001"
            )
            .ComReciboEntrega("REC001", new DateTime(2024, 1, 15, 10, 10, 0))
            .ComSucesso("Processado com sucesso")
            .ComAviso("W001", "Campo opcional não preenchido")
            .ComId("EVT001")
            .Build();

        // Assert
        Assert.Equal("12345678000195", message.GetCnpjEmpresaDeclarante());
        Assert.Equal("F200", message.GetDadosRecepcao()?.tipoEvento);
        Assert.Equal("REC001", message.GetReciboEntrega()?.numeroRecibo);
        Assert.True(message.IsSuccessful());
        Assert.Single(message.GetAvisos());
        Assert.Empty(message.GetErros());
        Assert.Equal("EVT001", message.EFinanceira.retornoEvento.id);
    }

    [Fact]
    public void Message_Properties_ShouldBeCorrect()
    {
        // Arrange
        var message = new RetornoLoteEventos_v1_3_0_Message();

        // Act & Assert
        Assert.Equal("v1_3_0", message.Version);
        Assert.Equal("eFinanceira", message.RootElementName);
        Assert.Null(message.IdAttributeName);
        Assert.Null(message.IdValue);
        Assert.Same(message.EFinanceira, message.Payload);
    }

    [Fact]
    public void Message_Constructor_WithParameters_ShouldSetProperties()
    {
        // Arrange
        var version = "v1_3_1";
        var eFinanceira = new eFinanceira();

        // Act
        var message = new RetornoLoteEventos_v1_3_0_Message(version, eFinanceira);

        // Assert
        Assert.Equal(version, message.Version);
        Assert.Same(eFinanceira, message.EFinanceira);
        Assert.Same(eFinanceira, message.Payload);
    }
}
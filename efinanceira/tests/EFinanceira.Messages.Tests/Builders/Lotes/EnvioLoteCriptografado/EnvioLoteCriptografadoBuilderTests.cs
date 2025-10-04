using System;
using System.Text;
using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Builders.Lotes.EnvioLoteCriptografado;
using EFinanceira.Messages.Builders.Lotes;
using Xunit;

namespace EFinanceira.Messages.Tests.Builders.Lotes.EnvioLoteCriptografado;

/// <summary>
/// Testes unitários para o EnvioLoteCriptografadoBuilder
/// </summary>
public class EnvioLoteCriptografadoBuilderTests
{
    /// <summary>
    /// Evento mock simples para testes
    /// </summary>
    private sealed class MockEvent : IEFinanceiraMessage
    {
        public string Version { get; } = "v1_2_0";
        public string RootElementName { get; } = "mockEvent";
        public string? IdAttributeName { get; } = "id";
        public string? IdValue { get; } = "MOCK_001";
        public object Payload { get; } = new { Id = "MOCK_001", Tipo = "Test" };
    }

    /// <summary>
    /// Cria um lote de eventos válido para testes
    /// </summary>
    /// <param name="loteId">ID do lote</param>
    /// <returns>Lote de eventos válido</returns>
    private static EnvioLoteEventosV120Message CriarLoteEventosValido(string loteId = "LOTE_TEST")
    {
        var mockEvent = new MockEvent();

        return new EnvioLoteEventosV120Builder()
            .WithId(loteId)
            .WithTransmissor("12345678000195")
            .AdicionarEvento(mockEvent)
            .Build();
    }
    [Fact]
    public void Create_DeveRetornarInstanciaValida()
    {
        // Act
        var builder = EnvioLoteCriptografadoBuilder.Create();

        // Assert
        Assert.NotNull(builder);
    }

    [Fact]
    public void Create_ComVersao_DeveRetornarInstanciaComVersaoCorreta()
    {
        // Arrange
        const string versaoEsperada = "v1_2_0";

        // Act
        var message = EnvioLoteCriptografadoBuilder
            .Create(versaoEsperada)
            .ComDadosManuais(
                idCertificado: "CERT_TEST",
                chave: "Q2hhdmVUZXN0ZQ==",
                lote: "TG90ZVRlc3Rl")
            .Build();

        // Assert
        Assert.Equal(versaoEsperada, message.Version);
    }

    [Fact]
    public void WithId_DeveDefinirIdCorretamente()
    {
        // Arrange
        const string idEsperado = "LOTE_TEST_001";

        // Act
        var message = EnvioLoteCriptografadoBuilder
            .Create()
            .WithId(idEsperado)
            .ComDadosManuais(
                idCertificado: "CERT_TEST",
                chave: "Q2hhdmVUZXN0ZQ==",
                lote: "TG90ZVRlc3Rl")
            .Build();

        // Assert
        Assert.Equal(idEsperado, message.EFinanceira.loteCriptografado.id);
        Assert.Equal(idEsperado, message.IdValue);
    }

    [Fact]
    public void ComDadosManuais_DeveConfigurarTodosOsCampos()
    {
        // Arrange
        const string idCertificado = "CERT_MANUAL";
        const string chave = "Q2hhdmVNYW51YWw=";
        const string lote = "TG90ZU1hbnVhbA==";

        // Act
        var message = EnvioLoteCriptografadoBuilder
            .Create()
            .ComDadosManuais(idCertificado, chave, lote)
            .Build();

        // Assert
        var loteCripto = message.EFinanceira.loteCriptografado;
        Assert.Equal(idCertificado, loteCripto.idCertificado);
        Assert.Equal(chave, loteCripto.chave);
        Assert.Equal(lote, loteCripto.lote);
    }

    [Fact]
    public void ComDadosBytes_DeveConverterParaBase64()
    {
        // Arrange
        const string idCertificado = "CERT_BYTES";
        var chaveBytes = Encoding.UTF8.GetBytes("ChaveEmBytes");
        var loteBytes = Encoding.UTF8.GetBytes("LoteEmBytes");

        var chaveBase64Esperada = Convert.ToBase64String(chaveBytes);
        var loteBase64Esperado = Convert.ToBase64String(loteBytes);

        // Act
        var message = EnvioLoteCriptografadoBuilder
            .Create()
            .ComDadosBytes(idCertificado, chaveBytes, loteBytes)
            .Build();

        // Assert
        var loteCripto = message.EFinanceira.loteCriptografado;
        Assert.Equal(idCertificado, loteCripto.idCertificado);
        Assert.Equal(chaveBase64Esperada, loteCripto.chave);
        Assert.Equal(loteBase64Esperado, loteCripto.lote);
    }

    [Fact]
    public void WithLoteCriptografado_DevePermitirConfiguracaoDetalhada()
    {
        // Arrange
        const string idCertificado = "CERT_DETALHADO";
        const string chave = "Q2hhdmVEZXRhbGhhZGE=";
        const string lote = "TG90ZURldGFsaGFkbw==";

        // Act
        var message = EnvioLoteCriptografadoBuilder
            .Create()
            .WithLoteCriptografado(loteCripto => loteCripto
                .WithIdCertificado(idCertificado)
                .WithChave(chave)
                .WithLote(lote))
            .Build();

        // Assert
        var loteCriptografado = message.EFinanceira.loteCriptografado;
        Assert.Equal(idCertificado, loteCriptografado.idCertificado);
        Assert.Equal(chave, loteCriptografado.chave);
        Assert.Equal(lote, loteCriptografado.lote);
    }

    [Fact]
    public void Build_SemIdCertificado_DeveLancarExcecao()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            EnvioLoteCriptografadoBuilder
                .Create()
                .WithLoteCriptografado(lote => lote
                    .WithChave("Q2hhdmU=")
                    .WithLote("TG90ZQ=="))
                .Build());

        Assert.Contains("IdCertificado é obrigatório", exception.Message);
    }

    [Fact]
    public void Build_SemChave_DeveLancarExcecao()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            EnvioLoteCriptografadoBuilder
                .Create()
                .WithLoteCriptografado(lote => lote
                    .WithIdCertificado("CERT_TEST")
                    .WithLote("TG90ZQ=="))
                .Build());

        Assert.Contains("Chave criptográfica é obrigatória", exception.Message);
    }

    [Fact]
    public void Build_SemLote_DeveLancarExcecao()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            EnvioLoteCriptografadoBuilder
                .Create()
                .WithLoteCriptografado(lote => lote
                    .WithIdCertificado("CERT_TEST")
                    .WithChave("Q2hhdmU="))
                .Build());

        Assert.Contains("Lote criptografado é obrigatório", exception.Message);
    }

    [Fact]
    public void Build_SemId_DeveLancarExcecao()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            EnvioLoteCriptografadoBuilder
                .Create()
                .WithId("") // ID vazio
                .ComDadosManuais(
                    idCertificado: "CERT_TEST",
                    chave: "Q2hhdmU=",
                    lote: "TG90ZQ==")
                .Build());

        Assert.Contains("Id do lote é obrigatório", exception.Message);
    }

    [Fact]
    public void ComLoteCompleto_DeveConfigurarAutomaticamente()
    {
        // Arrange
        const string idCertificado = "CERT_AUTO";
        var mockEvent = new MockEvent();
        var loteMessage = new EnvioLoteEventosV120Builder()
            .WithId("LOTE_TEST")
            .WithTransmissor("12345678000195") // CNPJ válido para o teste
            .AdicionarEvento(mockEvent)
            .Build();

        // Act
        var message = EnvioLoteCriptografadoBuilder
            .Create()
            .ComLoteCompleto(idCertificado, loteMessage)
            .Build();

        // Assert
        var loteCripto = message.EFinanceira.loteCriptografado;
        Assert.Equal(idCertificado, loteCripto.idCertificado);
        Assert.NotEmpty(loteCripto.chave);
        Assert.NotEmpty(loteCripto.lote);

        // Verificar se são Base64 válidos
        Assert.True(LoteCriptografiaUtils.IsValidBase64(loteCripto.chave));
        Assert.True(LoteCriptografiaUtils.IsValidBase64(loteCripto.lote));
    }

    [Theory]
    [InlineData("VGVzdGU=")] // Base64 válido: "Teste"
    [InlineData("UGxhY2Vob2xkZXI=")] // Base64 válido: "Placeholder"
    [InlineData("VGVzdGVEZURhZG9zRW1CYXNlNjQ=")] // Base64 válido: "TesteDesDadosEmBase64"
    public void IsValidBase64_ComStringValida_DeveRetornarTrue(string base64Valid)
    {
        // Act
        var result = LoteCriptografiaUtils.IsValidBase64(base64Valid);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("InvalidBase64!")] // Caracteres inválidos
    [InlineData("NotBase64String")] // Não é Base64
    [InlineData("")] // String vazia
    [InlineData("A")] // Muito curto
    public void IsValidBase64_ComStringInvalida_DeveRetornarFalse(string base64Invalid)
    {
        // Act
        var result = LoteCriptografiaUtils.IsValidBase64(base64Invalid);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GerarChaveAES_DeveRetornarChaveValida()
    {
        // Act
        var chave = LoteCriptografiaUtils.GerarChaveAES();

        // Assert
        Assert.NotNull(chave);
        Assert.Equal(32, chave.Length); // 256 bits = 32 bytes
    }

    [Fact]
    public void CriptografarLote_DeveRetornarDadosValidos()
    {
        // Arrange
        var mockEvent = new MockEvent();
        var loteMessage = new EnvioLoteEventosV120Builder()
            .WithId("LOTE_CRIPTO_TEST")
            .WithTransmissor("12345678000195")
            .AdicionarEvento(mockEvent)
            .Build();
        var chaveAES = LoteCriptografiaUtils.GerarChaveAES();

        // Act
        var (chaveBase64, loteBase64) = LoteCriptografiaUtils.CriptografarLote(loteMessage, chaveAES);

        // Assert
        Assert.NotEmpty(chaveBase64);
        Assert.NotEmpty(loteBase64);
        Assert.True(LoteCriptografiaUtils.IsValidBase64(chaveBase64));
        Assert.True(LoteCriptografiaUtils.IsValidBase64(loteBase64));
    }

    [Fact]
    public void LoteCriptografadoBuilder_WithChaveAESAleatoria_DeveGerarChave()
    {
        // Arrange
        var builder = new LoteCriptografadoBuilder();

        // Act
        builder.WithChaveAESAleatoria(out var chaveAES);
        var resultado = builder
            .WithIdCertificado("CERT_TEST")
            .WithLote("TG90ZVRlc3Rl")
            .Build();

        // Assert
        Assert.NotNull(chaveAES);
        Assert.Equal(32, chaveAES.Length); // 256 bits
        Assert.NotEmpty(resultado.chave);
        Assert.True(LoteCriptografiaUtils.IsValidBase64(resultado.chave));
    }

    [Fact]
    public void WithLoteCriptografadoCompleto_ComChaveNula_DeveGerarChaveAutomaticamente()
    {
        // Arrange
        var mockEvent = new MockEvent();
        var loteMessage = new EnvioLoteEventosV120Builder()
            .WithId("LOTE_AUTO_CHAVE")
            .WithTransmissor("12345678000195")
            .AdicionarEvento(mockEvent)
            .Build();
        var builder = new LoteCriptografadoBuilder();

        // Act
        var resultado = builder
            .WithIdCertificado("CERT_AUTO")
            .WithLoteCriptografadoCompleto(loteMessage, null) // Chave nula = gerar automaticamente
            .Build();

        // Assert
        Assert.NotEmpty(resultado.chave);
        Assert.NotEmpty(resultado.lote);
        Assert.True(LoteCriptografiaUtils.IsValidBase64(resultado.chave));
        Assert.True(LoteCriptografiaUtils.IsValidBase64(resultado.lote));
    }

    [Fact]
    public void EnvioLoteCriptografadoMessage_DeveImplementarIEFinanceiraMessage()
    {
        // Arrange & Act
        var message = EnvioLoteCriptografadoBuilder
            .Create()
            .ComDadosManuais(
                idCertificado: "CERT_INTERFACE",
                chave: "Q2hhdmVJbnRlcmZhY2U=",
                lote: "TG90ZUludGVyZmFjZQ==")
            .Build();

        // Assert
        Assert.Equal("v1_2_0", message.Version);
        Assert.Equal("eFinanceira", message.RootElementName);
        Assert.Null(message.IdAttributeName);
        Assert.NotNull(message.Payload);
        Assert.Equal(message.EFinanceira, message.Payload);
    }

    [Fact]
    public void WithLoteFromMessage_DeveProcessarMensagemCorretamente()
    {
        // Arrange
        var mockEvent = new MockEvent();
        var loteMessage = new EnvioLoteEventosV120Builder()
            .WithId("LOTE_FROM_MSG")
            .WithTransmissor("12345678000195")
            .AdicionarEvento(mockEvent)
            .Build();
        var chaveAES = LoteCriptografiaUtils.GerarChaveAES();
        var builder = new LoteCriptografadoBuilder();

        // Act
        var resultado = builder
            .WithIdCertificado("CERT_FROM_MESSAGE")
            .WithChave(chaveAES)
            .WithLoteFromMessage(loteMessage, chaveAES)
            .Build();

        // Assert
        Assert.Equal("CERT_FROM_MESSAGE", resultado.idCertificado);
        Assert.NotEmpty(resultado.chave);
        Assert.NotEmpty(resultado.lote);
        Assert.True(LoteCriptografiaUtils.IsValidBase64(resultado.lote));
    }

    [Fact]
    public void WithChave_ComBytes_DeveConverterParaBase64()
    {
        // Arrange
        var chaveBytes = Encoding.UTF8.GetBytes("MinhaChaveSecreta");
        var chaveBase64Esperada = Convert.ToBase64String(chaveBytes);
        var builder = new LoteCriptografadoBuilder();

        // Act
        var resultado = builder
            .WithChave(chaveBytes)
            .WithIdCertificado("CERT_BYTES")
            .WithLote("TG90ZQ==")
            .Build();

        // Assert
        Assert.Equal(chaveBase64Esperada, resultado.chave);
    }

    [Fact]
    public void WithLote_ComBytes_DeveConverterParaBase64()
    {
        // Arrange
        var loteBytes = Encoding.UTF8.GetBytes("ConteúdoDoLote");
        var loteBase64Esperado = Convert.ToBase64String(loteBytes);
        var builder = new LoteCriptografadoBuilder();

        // Act
        var resultado = builder
            .WithIdCertificado("CERT_LOTE_BYTES")
            .WithChave("Q2hhdmU=")
            .WithLote(loteBytes)
            .Build();

        // Assert
        Assert.Equal(loteBase64Esperado, resultado.lote);
    }
}
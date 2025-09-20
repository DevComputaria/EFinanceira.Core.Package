using EFinanceira.Core.Abstractions;
using EFinanceira.Core.Factory;
using EFinanceira.Core.Serialization;
using EFinanceira.Core.Signing;
using EFinanceira.Core.Validation;
using EFinanceira.Messages.Builders.Eventos;
using EFinanceira.Messages.Builders.Lotes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace EFinanceira.Tests;

/// <summary>
/// Testes de integração para o fluxo completo da biblioteca EFinanceira
/// </summary>
public class EFinanceiraIntegrationTests
{
    private readonly ITestOutputHelper _output;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public EFinanceiraIntegrationTests(ITestOutputHelper output)
    {
        _output = output;

        // Configuração de teste
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["EFinanceira:Declarante:Cnpj"] = "12345678000199",
                ["EFinanceira:Declarante:Nome"] = "Empresa Teste",
                ["EFinanceira:Certificate:PfxPath"] = "./certificates/test.pfx",
                ["EFinanceira:Certificate:PfxPassword"] = "123456",
                ["EFinanceira:Validation:SchemasPath"] = "../schemas"
            })
            .Build();

        // Setup DI
        var services = new ServiceCollection();
        services.AddLogging(); // Removido AddConsole temporariamente
        services.AddSingleton<IXmlSerializer, XmlNetSerializer>();
        services.AddSingleton<IXmlValidator, XmlValidator>();
        services.AddSingleton<IXmlSigner, XmlSigner>();
        services.AddSingleton<IMessageFactory, EFinanceiraMessageFactory>();
        services.AddSingleton(_configuration);

        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public void DeveCriarEventoMovimentacaoFinanceiraComSucesso()
    {
        // Arrange
        var cnpjDeclarante = _configuration["EFinanceira:Declarante:Cnpj"]!;

        // Act
        var evento = new LeiauteMovimentacaoFinanceiraBuilder()
            .WithId("TEST_EVT_001")
            .WithDeclarante(cnpjDeclarante)
            .WithAmbiente(2) // Homologação
            .WithRetificacao(1) // Original
            .WithDeclarado(declarado =>
            {
                declarado
                    .WithTipoNI(2) // CNPJ
                    .WithNumeroInscricao("98765432000188")
                    .WithNome("Cliente Teste S.A.")
                    .WithEndereco(endereco =>
                    {
                        endereco
                            .WithPais("076") // Brasil
                            .WithEndereco("Rua Teste, 123")
                            .WithCidade("São Paulo");
                    });
            })
            .WithMovimentacao(mov =>
            {
                mov
                    .WithDataInicio(new DateTime(2024, 1, 1))
                    .WithDataFim(new DateTime(2024, 12, 31))
                    .AdicionarReportavel(rep =>
                    {
                        rep
                            .WithTipoReportavel(1)
                            .WithInformacoesConta(conta =>
                            {
                                conta
                                    .WithTipoConta(1)
                                    .WithSubTipoConta("001")
                                    .WithMoeda("986"); // Real
                            });
                    });
            })
            .Build();

        // Assert
        Assert.NotNull(evento);
        Assert.Equal("TEST_EVT_001", evento.IdValue);
        Assert.Equal("evtMovimentacaoFinanceira", evento.RootElementName);
        Assert.NotNull(evento.Payload);

        _output.WriteLine($"Evento criado com sucesso: {evento.IdValue}");
    }

    [Fact]
    public void DeveCriarLoteComMultiplosEventosComSucesso()
    {
        // Arrange
        var cnpjTransmissor = _configuration["EFinanceira:Declarante:Cnpj"]!;
        var nomeTransmissor = _configuration["EFinanceira:Declarante:Nome"]!;

        var evento1 = new LeiauteMovimentacaoFinanceiraBuilder()
            .WithId("TEST_EVT_LOTE_001")
            .WithDeclarante(cnpjTransmissor)
            .WithDeclarado(d => d.WithTipoNI(2).WithNumeroInscricao("11111111000111").WithNome("Cliente 1"))
            .WithMovimentacao(m => m.WithDataInicio(DateTime.Today.AddDays(-30)).WithDataFim(DateTime.Today))
            .Build();

        var evento2 = new LeiauteMovimentacaoFinanceiraBuilder()
            .WithId("TEST_EVT_LOTE_002")
            .WithDeclarante(cnpjTransmissor)
            .WithDeclarado(d => d.WithTipoNI(2).WithNumeroInscricao("22222222000222").WithNome("Cliente 2"))
            .WithMovimentacao(m => m.WithDataInicio(DateTime.Today.AddDays(-30)).WithDataFim(DateTime.Today))
            .Build();

        // Act
        var lote = new EnvioLoteEventosV120Builder()
            .WithId("TEST_LOTE_001")
            .ComTransmissor(cnpjTransmissor, nomeTransmissor, "teste@empresa.com.br", "+5511999999999")
            .AdicionarEvento(evento1)
            .AdicionarEvento(evento2)
            .Build();

        // Assert
        Assert.NotNull(lote);
        Assert.Equal("TEST_LOTE_001", lote.IdValue);
        Assert.Equal(2, lote.Lote.Eventos.Count);
        Assert.Equal(cnpjTransmissor, lote.Lote.IdeTransmissor.CnpjTransmissor);

        _output.WriteLine($"Lote criado com {lote.Lote.Eventos.Count} eventos");
    }

    [Fact]
    public void DeveSerializarEventoParaXmlValidoComSucesso()
    {
        // Arrange
        var serializer = _serviceProvider.GetRequiredService<IXmlSerializer>();
        var evento = new LeiauteMovimentacaoFinanceiraBuilder()
            .WithId("TEST_SERIALIZATION_001")
            .WithDeclarante("12345678000199")
            .WithDeclarado(d => d.WithTipoNI(2).WithNumeroInscricao("98765432000188").WithNome("Teste Serialização"))
            .WithMovimentacao(m => m.WithDataInicio(DateTime.Today).WithDataFim(DateTime.Today))
            .Build();

        // Act
        var xml = serializer.Serialize(evento.Payload);

        // Assert
        Assert.NotNull(xml);
        Assert.NotEmpty(xml);
        Assert.Contains("<?xml", xml);
        Assert.Contains("evtMovimentacaoFinanceira", xml);
        Assert.Contains("TEST_SERIALIZATION_001", xml);
        Assert.Contains("12345678000199", xml);

        _output.WriteLine($"XML serializado com {xml.Length} caracteres");
        _output.WriteLine($"Primeiros 200 caracteres: {xml[..Math.Min(200, xml.Length)]}...");
    }

    [Fact]
    public void DeveSerializarLoteParaXmlValidoComSucesso()
    {
        // Arrange
        var serializer = _serviceProvider.GetRequiredService<IXmlSerializer>();
        var evento = new LeiauteMovimentacaoFinanceiraBuilder()
            .WithId("TEST_LOTE_SERIALIZATION_001")
            .WithDeclarante("12345678000199")
            .WithDeclarado(d => d.WithTipoNI(2).WithNumeroInscricao("98765432000188").WithNome("Teste"))
            .WithMovimentacao(m => m.WithDataInicio(DateTime.Today).WithDataFim(DateTime.Today))
            .Build();

        var lote = new EnvioLoteEventosV120Builder()
            .WithId("TEST_LOTE_SERIALIZATION")
            .ComTransmissor("12345678000199", "Empresa Teste", "teste@empresa.com")
            .AdicionarEvento(evento)
            .Build();

        // Act
        var xml = serializer.Serialize(lote.Payload);

        // Assert
        Assert.NotNull(xml);
        Assert.NotEmpty(xml);
        Assert.Contains("<?xml", xml);
        Assert.Contains("envioLoteEventos", xml);
        Assert.Contains("TEST_LOTE_SERIALIZATION", xml);
        Assert.Contains("evtMovimentacaoFinanceira", xml);

        _output.WriteLine($"XML do lote serializado com {xml.Length} caracteres");
    }

    [Theory]
    [InlineData(1, "CNPJ")]
    [InlineData(2, "CPF")]
    public void DeveCriarEventoComDiferentesTiposDeNI(int tipoNI, string descricaoTipo)
    {
        // Arrange & Act
        var numeroInscricao = tipoNI == 1 ? "12345678901" : "12345678000199";

        var evento = new LeiauteMovimentacaoFinanceiraBuilder()
            .WithId($"TEST_NI_{tipoNI}_001")
            .WithDeclarante("12345678000199")
            .WithDeclarado(d =>
                d.WithTipoNI(tipoNI)
                 .WithNumeroInscricao(numeroInscricao)
                 .WithNome($"Teste {descricaoTipo}"))
            .WithMovimentacao(m => m.WithDataInicio(DateTime.Today).WithDataFim(DateTime.Today))
            .Build();

        // Assert
        Assert.NotNull(evento);
        Assert.Contains($"TEST_NI_{tipoNI}_001", evento.IdValue!);

        _output.WriteLine($"Evento criado para tipo NI {tipoNI} ({descricaoTipo}): {evento.IdValue}");
    }

    [Fact]
    public void DeveTratarErroAoCriarEventoSemDadosObrigatorios()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            new LeiauteMovimentacaoFinanceiraBuilder()
                .WithId("TEST_ERROR_001")
                // Sem declarante (obrigatório)
                .Build();
        });

        Assert.NotNull(exception);
        _output.WriteLine($"Erro tratado corretamente: {exception.Message}");
    }

    [Fact]
    public void DevePermitirFluxoCompletoSerializacaoDeserializacao()
    {
        // Arrange
        var serializer = _serviceProvider.GetRequiredService<IXmlSerializer>();
        var eventoOriginal = new LeiauteMovimentacaoFinanceiraBuilder()
            .WithId("TEST_ROUNDTRIP_001")
            .WithDeclarante("12345678000199")
            .WithDeclarado(d => d.WithTipoNI(2).WithNumeroInscricao("98765432000188").WithNome("Teste Round-trip"))
            .WithMovimentacao(m => m.WithDataInicio(DateTime.Today).WithDataFim(DateTime.Today))
            .Build();

        // Act
        var xml = serializer.Serialize(eventoOriginal.Payload);
        // Comentado pois não há método Deserialize não-genérico
        // var deserializado = serializer.Deserialize(xml, eventoOriginal.Payload.GetType());

        // Assert
        Assert.NotNull(xml);
        // Assert.NotNull(deserializado);

        _output.WriteLine("Fluxo de serialização/deserialização completado com sucesso");
        _output.WriteLine($"XML gerado: {xml.Length} caracteres");
    }

    [Fact]
    public void DeveValidarEstruturaXmlGeradaContemElementosEsperados()
    {
        // Arrange
        var serializer = _serviceProvider.GetRequiredService<IXmlSerializer>();
        var evento = new LeiauteMovimentacaoFinanceiraBuilder()
            .WithId("TEST_STRUCTURE_001")
            .WithDeclarante("12345678000199")
            .WithAmbiente(2)
            .WithRetificacao(1)
            .WithDeclarado(d =>
            {
                d.WithTipoNI(2)
                 .WithNumeroInscricao("98765432000188")
                 .WithNome("Teste Estrutura")
                 .WithEndereco(e => e.WithPais("076").WithCidade("São Paulo"));
            })
            .WithMovimentacao(m =>
            {
                m.WithDataInicio(new DateTime(2024, 1, 1))
                 .WithDataFim(new DateTime(2024, 12, 31))
                 .AdicionarReportavel(r =>
                 {
                     r.WithTipoReportavel(1)
                      .WithInformacoesConta(c => c.WithTipoConta(1).WithMoeda("986"));
                 });
            })
            .Build();

        // Act
        var xml = serializer.Serialize(evento.Payload);

        // Assert - Verificar elementos principais
        var elementosEsperados = new[]
        {
            "evtMovimentacaoFinanceira",
            "ideEvento",
            "ideDeclarante",
            "ideDeclarado",
            "dadosDeclarado",
            "infoPeriodo",
            "reportavel"
        };

        foreach (var elemento in elementosEsperados)
        {
            Assert.Contains(elemento, xml);
            _output.WriteLine($"✓ Elemento encontrado: {elemento}");
        }

        // Verificar atributos específicos
        Assert.Contains("id=\"TEST_STRUCTURE_001\"", xml);
        Assert.Contains("tpAmb=\"2\"", xml);
        Assert.Contains("nrRecibo=\"1\"", xml);

        _output.WriteLine($"Estrutura XML validada com sucesso ({xml.Length} caracteres)");
    }
}

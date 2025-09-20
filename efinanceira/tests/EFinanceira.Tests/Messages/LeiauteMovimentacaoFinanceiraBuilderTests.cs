using EFinanceira.Messages.Builders.Eventos;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace EFinanceira.Tests.Messages;

/// <summary>
/// Testes unitários para LeiauteMovimentacaoFinanceiraBuilder
/// </summary>
public class LeiauteMovimentacaoFinanceiraBuilderTests
{
    private readonly ITestOutputHelper _output;

    public LeiauteMovimentacaoFinanceiraBuilderTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Build_ComDadosMinimos_DeveCriarEventoValido()
    {
        // Arrange
        var builder = new LeiauteMovimentacaoFinanceiraBuilder();

        // Act
        var evento = builder
            .WithId("TEST_MIN_001")
            .WithDeclarante("12345678000199")
            .WithDeclarado(d => d.WithTipoNI(2).WithNumeroInscricao("98765432000188").WithNome("Teste"))
            .WithMovimentacao(m => m.WithDataInicio(DateTime.Today).WithDataFim(DateTime.Today))
            .Build();

        // Assert
        evento.Should().NotBeNull();
        evento.IdValue.Should().Be("TEST_MIN_001");
        evento.RootElementName.Should().Be("evtMovimentacaoFinanceira");
        evento.Version.Should().Be("1.2.1");
        evento.Payload.Should().NotBeNull();

        _output.WriteLine($"Evento criado: {evento.IdValue}");
    }

    [Fact]
    public void Build_ComDadosCompletos_DeveCriarEventoCompleto()
    {
        // Arrange
        var builder = new LeiauteMovimentacaoFinanceiraBuilder();

        // Act
        var evento = builder
            .WithId("TEST_COMPLETO_001")
            .WithDeclarante("12345678000199")
            .WithAmbiente(2) // Homologação
            .WithRetificacao(1) // Original
            .WithVersaoAplicacao("1.0.0")
            .WithDeclarado(declarado =>
            {
                declarado
                    .WithTipoNI(2) // CNPJ
                    .WithNumeroInscricao("98765432000188")
                    .WithNome("Cliente Completo S.A.")
                    .WithEndereco(endereco =>
                    {
                        endereco
                            .WithPais("076") // Brasil
                            .WithEndereco("Rua Completa, 123, Centro")
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
        evento.Should().NotBeNull();
        evento.IdValue.Should().Be("TEST_COMPLETO_001");

        _output.WriteLine($"Evento completo criado: {evento.IdValue}");
    }

    [Theory]
    [InlineData(1, "CPF")]
    [InlineData(2, "CNPJ")]
    public void Build_ComDiferentesTiposNI_DeveCriarEventoCorreto(int tipoNI, string descricao)
    {
        // Arrange
        var numeroInscricao = tipoNI == 1 ? "12345678901" : "12345678000199";

        // Act
        var evento = new LeiauteMovimentacaoFinanceiraBuilder()
            .WithId($"TEST_NI_{tipoNI}")
            .WithDeclarante("12345678000199")
            .WithDeclarado(d =>
                d.WithTipoNI(tipoNI)
                 .WithNumeroInscricao(numeroInscricao)
                 .WithNome($"Teste {descricao}"))
            .WithMovimentacao(m => m.WithDataInicio(DateTime.Today).WithDataFim(DateTime.Today))
            .Build();

        // Assert
        evento.Should().NotBeNull();
        evento.IdValue.Should().Be($"TEST_NI_{tipoNI}");

        _output.WriteLine($"Evento {descricao} criado: {evento.IdValue}");
    }

    [Fact]
    public void Build_SemId_DeveLancarArgumentException()
    {
        // Arrange
        var builder = new LeiauteMovimentacaoFinanceiraBuilder();

        // Act & Assert
        var action = () => builder
            .WithDeclarante("12345678000199")
            .WithDeclarado(d => d.WithTipoNI(2).WithNumeroInscricao("98765432000188").WithNome("Teste"))
            .WithMovimentacao(m => m.WithDataInicio(DateTime.Today).WithDataFim(DateTime.Today))
            .Build();

        action.Should().Throw<ArgumentException>()
            .WithMessage("*Id*obrigatório*");
    }

    [Fact]
    public void Build_SemDeclarante_DeveLancarArgumentException()
    {
        // Arrange
        var builder = new LeiauteMovimentacaoFinanceiraBuilder();

        // Act & Assert
        var action = () => builder
            .WithId("TEST_ERROR")
            .WithDeclarado(d => d.WithTipoNI(2).WithNumeroInscricao("98765432000188").WithNome("Teste"))
            .WithMovimentacao(m => m.WithDataInicio(DateTime.Today).WithDataFim(DateTime.Today))
            .Build();

        action.Should().Throw<ArgumentException>()
            .WithMessage("*Declarante*obrigatório*");
    }

    [Fact]
    public void Build_SemDeclarado_DeveLancarArgumentException()
    {
        // Arrange
        var builder = new LeiauteMovimentacaoFinanceiraBuilder();

        // Act & Assert
        var action = () => builder
            .WithId("TEST_ERROR")
            .WithDeclarante("12345678000199")
            .WithMovimentacao(m => m.WithDataInicio(DateTime.Today).WithDataFim(DateTime.Today))
            .Build();

        action.Should().Throw<ArgumentException>()
            .WithMessage("*Declarado*obrigatório*");
    }

    [Fact]
    public void Build_ComMultiplosReportaveis_DeveCriarEventoComTodos()
    {
        // Act
        var evento = new LeiauteMovimentacaoFinanceiraBuilder()
            .WithId("TEST_MULTI_REP")
            .WithDeclarante("12345678000199")
            .WithDeclarado(d => d.WithTipoNI(2).WithNumeroInscricao("98765432000188").WithNome("Teste"))
            .WithMovimentacao(m =>
            {
                m.WithDataInicio(DateTime.Today).WithDataFim(DateTime.Today)
                 .AdicionarReportavel(r => r.WithTipoReportavel(1))
                 .AdicionarReportavel(r => r.WithTipoReportavel(2))
                 .AdicionarReportavel(r => r.WithTipoReportavel(3));
            })
            .Build();

        // Assert
        evento.Should().NotBeNull();
        _output.WriteLine($"Evento com múltiplos reportáveis criado: {evento.IdValue}");
    }

    [Fact]
    public void Build_ComFluentAPI_DevePermitirEncadeamento()
    {
        // Act & Assert (teste de sintaxe fluente)
        var action = () => new LeiauteMovimentacaoFinanceiraBuilder()
            .WithId("TEST_FLUENT")
            .WithDeclarante("12345678000199")
            .WithAmbiente(1)
            .WithRetificacao(1)
            .WithVersaoAplicacao("1.0.0")
            .WithDeclarado(d =>
                d.WithTipoNI(2)
                 .WithNumeroInscricao("98765432000188")
                 .WithNome("Teste Fluent")
                 .WithEndereco(e =>
                     e.WithPais("076")
                      .WithEndereco("Rua Teste")
                      .WithCidade("São Paulo")))
            .WithMovimentacao(m =>
                m.WithDataInicio(DateTime.Today)
                 .WithDataFim(DateTime.Today)
                 .AdicionarReportavel(r =>
                     r.WithTipoReportavel(1)
                      .WithInformacoesConta(c =>
                          c.WithTipoConta(1)
                           .WithSubTipoConta("001")
                           .WithMoeda("986"))))
            .Build();

        action.Should().NotThrow();

        var evento = action();
        evento.Should().NotBeNull();
        evento.IdValue.Should().Be("TEST_FLUENT");

        _output.WriteLine($"API fluente validada com evento: {evento.IdValue}");
    }

    [Fact]
    public void Build_ComDatasInvalidas_DeveLancarArgumentException()
    {
        // Arrange
        var dataInicio = DateTime.Today;
        var dataFim = DateTime.Today.AddDays(-1); // Data fim antes da data início

        // Act & Assert
        var action = () => new LeiauteMovimentacaoFinanceiraBuilder()
            .WithId("TEST_DATES_ERROR")
            .WithDeclarante("12345678000199")
            .WithDeclarado(d => d.WithTipoNI(2).WithNumeroInscricao("98765432000188").WithNome("Teste"))
            .WithMovimentacao(m => m.WithDataInicio(dataInicio).WithDataFim(dataFim))
            .Build();

        action.Should().Throw<ArgumentException>()
            .WithMessage("*data fim*antes*data início*");
    }

    [Fact]
    public void Build_ComReutilizacaoBuilder_DeveResetarEstado()
    {
        // Arrange
        var builder = new LeiauteMovimentacaoFinanceiraBuilder();

        // Act - Primeiro evento
        var evento1 = builder
            .WithId("TEST_REUSE_001")
            .WithDeclarante("12345678000199")
            .WithDeclarado(d => d.WithTipoNI(2).WithNumeroInscricao("11111111000111").WithNome("Primeiro"))
            .WithMovimentacao(m => m.WithDataInicio(DateTime.Today).WithDataFim(DateTime.Today))
            .Build();

        // Act - Segundo evento (criando novo builder)
        var builder2 = new LeiauteMovimentacaoFinanceiraBuilder();
        var evento2 = builder2
            .WithId("TEST_REUSE_002")
            .WithDeclarante("12345678000199")
            .WithDeclarado(d => d.WithTipoNI(2).WithNumeroInscricao("22222222000222").WithNome("Segundo"))
            .WithMovimentacao(m => m.WithDataInicio(DateTime.Today).WithDataFim(DateTime.Today))
            .Build();

        // Assert
        evento1.Should().NotBeNull();
        evento2.Should().NotBeNull();
        evento1.IdValue.Should().Be("TEST_REUSE_001");
        evento2.IdValue.Should().Be("TEST_REUSE_002");

        _output.WriteLine($"Builder reutilizado: {evento1.IdValue} -> {evento2.IdValue}");
    }
}

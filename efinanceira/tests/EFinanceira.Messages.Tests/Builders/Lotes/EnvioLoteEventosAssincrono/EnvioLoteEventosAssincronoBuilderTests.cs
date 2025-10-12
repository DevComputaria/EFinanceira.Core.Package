using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Builders.Lotes.EnvioLoteEventosAssincrono;
using Xunit;

namespace EFinanceira.Messages.Tests.Builders.Lotes.EnvioLoteEventosAssincrono;

/// <summary>
/// Testes unitários para EnvioLoteEventosAssincronoBuilder
/// </summary>
public class EnvioLoteEventosAssincronoBuilderTests
{
    /// <summary>
    /// Evento mock simples para testes
    /// </summary>
    private sealed class MockEvent : IEFinanceiraMessage
    {
        public string Version { get; } = "v1_0_0";
        public string RootElementName { get; } = "mockEvent";
        public string? IdAttributeName { get; } = "id";
        public string? IdValue { get; } = "MOCK_001";
        public object Payload { get; } = new MockPayload();
    }

    /// <summary>
    /// Payload mock para serialização XML
    /// </summary>
    public class MockPayload
    {
        public string Id { get; set; } = "MOCK_001";
        public string Tipo { get; set; } = "Test";
    }

    [Fact]
    public void Constructor_DeveInicializarCorretamente()
    {
        // Act
        var builder = new EnvioLoteEventosAssincronoBuilder();

        // Assert
        Assert.NotNull(builder);
        Assert.Equal(0, builder.ContarEventos());
        Assert.True(builder.EstaVazio());
    }

    [Fact]
    public void Constructor_ComVersaoPersonalizada_DeveDefinirVersao()
    {
        // Arrange
        const string versaoCustomizada = "v2_0_0";
        var builder = new EnvioLoteEventosAssincronoBuilder(versaoCustomizada);

        // Act
        builder.ComCnpjDeclarante("12345678000123")
               .AdicionarEvento(new MockEvent());
        var message = builder.Build();

        // Assert
        Assert.Equal(versaoCustomizada, message.Version);
        Assert.NotNull(message.EFinanceira);
        Assert.Equal("eFinanceira", message.RootElementName);
    }

    [Fact]
    public void ComCnpjDeclarante_DeveDefinirCnpj()
    {
        // Arrange
        const string cnpj = "12345678000123";
        var builder = new EnvioLoteEventosAssincronoBuilder();

        // Act
        builder.ComCnpjDeclarante(cnpj)
               .AdicionarEvento(new MockEvent());
        var message = builder.Build();

        // Assert
        Assert.Equal(cnpj, message.EFinanceira.loteEventosAssincrono.cnpjDeclarante);
    }

    [Fact]
    public void ComCnpjDeclarante_ComCnpjNulo_DeveLancarExcecao()
    {
        // Arrange
        var builder = new EnvioLoteEventosAssincronoBuilder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder.ComCnpjDeclarante(null!));
    }

    [Fact]
    public void AdicionarEvento_ComEventoValido_DeveAdicionarAoLote()
    {
        // Arrange
        var builder = new EnvioLoteEventosAssincronoBuilder();
        var mockEvent = new MockEvent();

        // Act
        builder.ComCnpjDeclarante("12345678000123")
               .AdicionarEvento(mockEvent);

        // Assert
        Assert.Equal(1, builder.ContarEventos());
        Assert.False(builder.EstaVazio());
    }

    [Fact]
    public void AdicionarEvento_ComEventoNulo_DeveLancarExcecao()
    {
        // Arrange
        var builder = new EnvioLoteEventosAssincronoBuilder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder.AdicionarEvento(null!));
    }

    [Fact]
    public void AdicionarEvento_ComIdPersonalizado_DeveUsarIdFornecido()
    {
        // Arrange
        var builder = new EnvioLoteEventosAssincronoBuilder();
        var mockEvent = new MockEvent();
        const string idPersonalizado = "CUSTOM_ID_001";

        // Act
        builder.ComCnpjDeclarante("12345678000123")
               .AdicionarEvento(mockEvent, idPersonalizado);
        var message = builder.Build();

        // Assert
        var eventos = message.EFinanceira.loteEventosAssincrono.eventos.evento;
        Assert.Single(eventos);
        Assert.Equal(idPersonalizado, eventos[0].id);
    }

    [Fact]
    public void AdicionarEvento_ComBuilder_DeveAdicionarEventoCorretamente()
    {
        // Arrange
        var builder = new EnvioLoteEventosAssincronoBuilder();
        var mockEventBuilder = new MockEventBuilder();

        // Act
        builder.ComCnpjDeclarante("12345678000123")
               .AdicionarEvento(mockEventBuilder);

        // Assert
        Assert.Equal(1, builder.ContarEventos());
    }

    [Fact]
    public void AdicionarEvento_ComBuilderNulo_DeveLancarExcecao()
    {
        // Arrange
        var builder = new EnvioLoteEventosAssincronoBuilder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder.AdicionarEvento<MockEvent>(null!));
    }

    [Fact]
    public void AdicionarEventos_ComColecao_DeveAdicionarTodosEventos()
    {
        // Arrange
        var builder = new EnvioLoteEventosAssincronoBuilder();
        var eventos = new[]
        {
            new MockEvent(),
            new MockEvent(),
            new MockEvent()
        };

        // Act
        builder.ComCnpjDeclarante("12345678000123")
               .AdicionarEventos(eventos);

        // Assert
        Assert.Equal(3, builder.ContarEventos());
    }

    [Fact]
    public void AdicionarEventos_ComColecaoNula_DeveLancarExcecao()
    {
        // Arrange
        var builder = new EnvioLoteEventosAssincronoBuilder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder.AdicionarEventos((IEnumerable<MockEvent>)null!));
    }

    [Fact]
    public void AdicionarEventos_ComBuildersColecao_DeveAdicionarTodosEventos()
    {
        // Arrange
        var builder = new EnvioLoteEventosAssincronoBuilder();
        var eventosBuilders = new[]
        {
            new MockEventBuilder(),
            new MockEventBuilder(),
            new MockEventBuilder()
        };

        // Act
        builder.ComCnpjDeclarante("12345678000123")
               .AdicionarEventos(eventosBuilders);

        // Assert
        Assert.Equal(3, builder.ContarEventos());
    }

    [Fact]
    public void AdicionarEventos_ComBuildersColecaoNula_DeveLancarExcecao()
    {
        // Arrange
        var builder = new EnvioLoteEventosAssincronoBuilder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder.AdicionarEventos((IEnumerable<MockEventBuilder>)null!));
    }

    [Fact]
    public void LimparEventos_DeveRemoverTodosEventos()
    {
        // Arrange
        var builder = new EnvioLoteEventosAssincronoBuilder();
        builder.ComCnpjDeclarante("12345678000123")
               .AdicionarEvento(new MockEvent())
               .AdicionarEvento(new MockEvent());

        // Act
        builder.LimparEventos();

        // Assert
        Assert.Equal(0, builder.ContarEventos());
        Assert.True(builder.EstaVazio());
    }

    [Fact]
    public void Build_ComEventos_DeveRetornarMensagemValida()
    {
        // Arrange
        var builder = new EnvioLoteEventosAssincronoBuilder();
        var mockEvent = new MockEvent();

        // Act
        builder.ComCnpjDeclarante("12345678000123")
               .AdicionarEvento(mockEvent);
        var message = builder.Build();

        // Assert
        Assert.NotNull(message);
        Assert.NotNull(message.EFinanceira);
        Assert.NotNull(message.EFinanceira.loteEventosAssincrono);
        Assert.Equal("12345678000123", message.EFinanceira.loteEventosAssincrono.cnpjDeclarante);
        Assert.Single(message.EFinanceira.loteEventosAssincrono.eventos.evento);
    }

    [Fact]
    public void Build_SemCnpjDeclarante_DeveLancarExcecao()
    {
        // Arrange
        var builder = new EnvioLoteEventosAssincronoBuilder();
        builder.AdicionarEvento(new MockEvent());

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Contains("CNPJ do declarante é obrigatório", exception.Message);
    }

    [Fact]
    public void Build_SemEventos_DeveLancarExcecao()
    {
        // Arrange
        var builder = new EnvioLoteEventosAssincronoBuilder();
        builder.ComCnpjDeclarante("12345678000123");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Contains("Pelo menos um evento deve ser adicionado ao lote", exception.Message);
    }

    [Fact]
    public void Build_ComMaisDe100Eventos_DeveLancarExcecao()
    {
        // Arrange
        var builder = new EnvioLoteEventosAssincronoBuilder();
        builder.ComCnpjDeclarante("12345678000123");

        // Adicionar 101 eventos
        for (int i = 0; i <= EnvioLoteEventosAssincronoBuilder.MaxEventosPorLote; i++)
        {
            if (i < EnvioLoteEventosAssincronoBuilder.MaxEventosPorLote)
            {
                builder.AdicionarEvento(new MockEvent());
            }
            else
            {
                // O 101º evento deve lançar exceção
                Assert.Throws<InvalidOperationException>(() => builder.AdicionarEvento(new MockEvent()));
            }
        }
    }

    [Fact]
    public void ContarEventos_DeveRetornarNumeroCorreto()
    {
        // Arrange
        var builder = new EnvioLoteEventosAssincronoBuilder();
        builder.ComCnpjDeclarante("12345678000123");

        // Act & Assert
        Assert.Equal(0, builder.ContarEventos());

        builder.AdicionarEvento(new MockEvent());
        Assert.Equal(1, builder.ContarEventos());

        builder.AdicionarEvento(new MockEvent());
        Assert.Equal(2, builder.ContarEventos());
    }

    [Fact]
    public void EstaVazio_DeveRetornarStatusCorreto()
    {
        // Arrange
        var builder = new EnvioLoteEventosAssincronoBuilder();

        // Act & Assert
        Assert.True(builder.EstaVazio());

        builder.ComCnpjDeclarante("12345678000123")
               .AdicionarEvento(new MockEvent());
        Assert.False(builder.EstaVazio());

        builder.LimparEventos();
        Assert.True(builder.EstaVazio());
    }

    [Fact]
    public void SerializeEventoToXmlElement_DeveGerarXmlValido()
    {
        // Arrange
        var builder = new EnvioLoteEventosAssincronoBuilder();
        var mockEvent = new MockEvent();

        // Act
        builder.ComCnpjDeclarante("12345678000123")
               .AdicionarEvento(mockEvent);
        var message = builder.Build();

        // Assert
        var eventos = message.EFinanceira.loteEventosAssincrono.eventos.evento;
        Assert.Single(eventos);
        Assert.NotNull(eventos[0].Any);
        Assert.Equal("EVT_001", eventos[0].id);
    }

    [Fact]
    public void ExtensaoCreate_DeveRetornarNovaInstancia()
    {
        // Act
        var builder = EnvioLoteEventosAssincronoBuilderExtensions.Create();

        // Assert
        Assert.NotNull(builder);
        Assert.IsType<EnvioLoteEventosAssincronoBuilder>(builder);
    }

    [Fact]
    public void ExtensaoCreate_ComVersao_DeveDefinirVersaoCorreta()
    {
        // Arrange
        const string versao = "v2_0_0";

        // Act
        var builder = EnvioLoteEventosAssincronoBuilderExtensions.Create(versao);
        builder.ComCnpjDeclarante("12345678000123")
               .AdicionarEvento(new MockEvent());
        var message = builder.Build();

        // Assert
        Assert.Equal(versao, message.Version);
    }

    [Fact]
    public void ExtensaoComEventos_DeveAdicionarEventos()
    {
        // Arrange
        var builder = new EnvioLoteEventosAssincronoBuilder();
        var eventos = new[]
        {
            new MockEvent(),
            new MockEvent()
        };

        // Act
        builder.ComCnpjDeclarante("12345678000123")
               .ComEventos(eventos);

        // Assert
        Assert.Equal(2, builder.ContarEventos());
    }

    [Fact]
    public void ExtensaoComEventosBuilder_DeveAdicionarEventosDoBuilders()
    {
        // Arrange
        var builder = new EnvioLoteEventosAssincronoBuilder();
        var eventosBuilders = new[]
        {
            new MockEventBuilder(),
            new MockEventBuilder()
        };

        // Act
        builder.ComCnpjDeclarante("12345678000123")
               .ComEventosBuilder(eventosBuilders);

        // Assert
        Assert.Equal(2, builder.ContarEventos());
    }

    [Fact]
    public void MessageProperties_DeveRetornarValoresCorretos()
    {
        // Arrange
        var builder = new EnvioLoteEventosAssincronoBuilder();
        builder.ComCnpjDeclarante("12345678000123")
               .AdicionarEvento(new MockEvent());

        // Act
        var message = builder.Build();

        // Assert
        Assert.Equal("v1_0_0", message.Version);
        Assert.NotNull(message.EFinanceira);
        Assert.Equal("eFinanceira", message.RootElementName);
        Assert.Null(message.IdAttributeName);
        Assert.Null(message.IdValue);
        Assert.Same(message.EFinanceira, message.Payload);
    }

    [Fact]
    public void EnvioLoteEventosAssincronoMessage_ConstructorVazio_DeveInicializarCorretamente()
    {
        // Act
        var message = new EnvioLoteEventosAssincronoMessage();

        // Assert
        Assert.Equal("v1_0_0", message.Version);
        Assert.NotNull(message.EFinanceira);
        Assert.Equal("eFinanceira", message.RootElementName);
    }

    /// <summary>
    /// Builder mock para testes
    /// </summary>
    private class MockEventBuilder : IMessageBuilder<MockEvent>
    {
        public MockEvent Build() => new MockEvent();
    }
}
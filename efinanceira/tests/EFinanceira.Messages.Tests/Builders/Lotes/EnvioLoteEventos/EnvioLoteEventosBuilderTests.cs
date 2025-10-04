using System.Xml;
using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Builders.Lotes.EnvioLoteEventos;
using EFinanceira.Messages.Generated.Lotes.EnvioLoteEventos;
using Xunit;

namespace EFinanceira.Messages.Tests.Builders.Lotes.EnvioLoteEventos;

/// <summary>
/// Testes unitários para o EnvioLoteEventosBuilder baseado nas classes geradas do XSD
/// </summary>
public class EnvioLoteEventosBuilderTests
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
        var builder = new EnvioLoteEventosBuilder();

        // Assert
        Assert.True(builder.EstaVazio());
        Assert.Equal(0, builder.ContarEventos());
    }

    [Fact]
    public void Constructor_ComVersaoPersonalizada_DeveDefinirVersao()
    {
        // Arrange
        const string versaoCustomizada = "v1_1_0";

        // Act
        var builder = new EnvioLoteEventosBuilder(versaoCustomizada);
        var message = builder.AdicionarEvento(new MockEvent()).Build();

        // Assert
        Assert.Equal(versaoCustomizada, message.Version);
    }

    [Fact]
    public void AdicionarEvento_ComEventoValido_DeveAdicionarAoLote()
    {
        // Arrange
        var builder = new EnvioLoteEventosBuilder();
        var mockEvent = new MockEvent();

        // Act
        builder.AdicionarEvento(mockEvent);

        // Assert
        Assert.False(builder.EstaVazio());
        Assert.Equal(1, builder.ContarEventos());
    }

    [Fact]
    public void AdicionarEvento_ComIdPersonalizado_DeveUsarIdFornecido()
    {
        // Arrange
        var builder = new EnvioLoteEventosBuilder();
        var mockEvent = new MockEvent();
        const string idPersonalizado = "EVENTO_CUSTOM_001";

        // Act
        var message = builder.AdicionarEvento(mockEvent, idPersonalizado).Build();

        // Assert
        Assert.Single(message.EFinanceira.loteEventos.evento);
        Assert.Equal(idPersonalizado, message.EFinanceira.loteEventos.evento[0].id);
    }

    [Fact]
    public void AdicionarEvento_ComBuilder_DeveAdicionarEventoCorretamente()
    {
        // Arrange
        var builder = new EnvioLoteEventosBuilder();
        var mockEventBuilder = new MockEventBuilder();

        // Act
        builder.AdicionarEvento(mockEventBuilder);

        // Assert
        Assert.Equal(1, builder.ContarEventos());
    }

    [Fact]
    public void AdicionarEventos_ComColecao_DeveAdicionarTodosEventos()
    {
        // Arrange
        var builder = new EnvioLoteEventosBuilder();
        var eventos = new[]
        {
            new MockEvent(),
            new MockEvent(),
            new MockEvent()
        };

        // Act
        builder.AdicionarEventos(eventos);

        // Assert
        Assert.Equal(3, builder.ContarEventos());
    }

    [Fact]
    public void AdicionarEventos_ComBuildersColecao_DeveAdicionarTodosEventos()
    {
        // Arrange
        var builder = new EnvioLoteEventosBuilder();
        var builders = new[]
        {
            new MockEventBuilder(),
            new MockEventBuilder(),
            new MockEventBuilder()
        };

        // Act
        builder.AdicionarEventos(builders);

        // Assert
        Assert.Equal(3, builder.ContarEventos());
    }

    [Fact]
    public void LimparEventos_DeveRemoverTodosEventos()
    {
        // Arrange
        var builder = new EnvioLoteEventosBuilder();
        builder.AdicionarEvento(new MockEvent())
               .AdicionarEvento(new MockEvent());

        // Act
        builder.LimparEventos();

        // Assert
        Assert.True(builder.EstaVazio());
        Assert.Equal(0, builder.ContarEventos());
    }

    [Fact]
    public void Build_ComEventos_DeveRetornarMensagemValida()
    {
        // Arrange
        var builder = new EnvioLoteEventosBuilder();
        var mockEvent = new MockEvent();

        // Act
        var message = builder.AdicionarEvento(mockEvent).Build();

        // Assert
        Assert.NotNull(message);
        Assert.Equal("eFinanceira", message.RootElementName);
        Assert.Equal("v1_2_0", message.Version);
        Assert.NotNull(message.EFinanceira);
        Assert.NotNull(message.EFinanceira.loteEventos);
        Assert.Single(message.EFinanceira.loteEventos.evento);
    }

    [Fact]
    public void Build_SemEventos_DeveLancarExcecao()
    {
        // Arrange
        var builder = new EnvioLoteEventosBuilder();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Contains("Lote deve conter pelo menos um evento", exception.Message);
    }

    [Fact]
    public void Build_ComMaisDe100Eventos_DeveLancarExcecao()
    {
        // Arrange
        var builder = new EnvioLoteEventosBuilder();

        // Adicionar 101 eventos
        for (int i = 0; i < 101; i++)
        {
            builder.AdicionarEvento(new MockEvent());
        }

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Contains("Lote não pode conter mais de 100 eventos", exception.Message);
    }

    [Fact]
    public void ContarEventos_DeveRetornarNumeroCorreto()
    {
        // Arrange
        var builder = new EnvioLoteEventosBuilder();

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
        var builder = new EnvioLoteEventosBuilder();

        // Act & Assert
        Assert.True(builder.EstaVazio());

        builder.AdicionarEvento(new MockEvent());
        Assert.False(builder.EstaVazio());

        builder.LimparEventos();
        Assert.True(builder.EstaVazio());
    }

    [Fact]
    public void SerializeEventoToXmlElement_DeveGerarXmlValido()
    {
        // Arrange
        var builder = new EnvioLoteEventosBuilder();
        var mockEvent = new MockEvent();

        // Act
        var message = builder.AdicionarEvento(mockEvent).Build();
        var eventoXml = message.EFinanceira.loteEventos.evento[0].Any;

        // Assert
        Assert.NotNull(eventoXml);
        Assert.IsType<XmlElement>(eventoXml);
    }

    [Fact]
    public void ExtensaoCreate_DeveRetornarNovoBuilder()
    {
        // Act
        var builder = EnvioLoteEventosBuilderExtensions.Create();

        // Assert
        Assert.NotNull(builder);
        Assert.IsType<EnvioLoteEventosBuilder>(builder);
        Assert.True(builder.EstaVazio());
    }

    [Fact]
    public void ExtensaoCreate_ComVersao_DeveDefinirVersaoCorreta()
    {
        // Arrange
        const string versaoCustomizada = "v1_1_0";

        // Act
        var builder = EnvioLoteEventosBuilderExtensions.Create(versaoCustomizada);
        var message = builder.AdicionarEvento(new MockEvent()).Build();

        // Assert
        Assert.Equal(versaoCustomizada, message.Version);
    }

    [Fact]
    public void ExtensaoComEventos_DeveAdicionarEventos()
    {
        // Arrange
        var builder = new EnvioLoteEventosBuilder();
        var eventos = new IEFinanceiraMessage[]
        {
            new MockEvent(),
            new MockEvent()
        };

        // Act
        var resultado = builder.ComEventos(eventos);

        // Assert
        Assert.Same(builder, resultado); // Deve retornar o mesmo builder
        Assert.Equal(2, builder.ContarEventos());
    }

    [Fact]
    public void ExtensaoComEventosBuilder_DeveAdicionarEventosDoBuilders()
    {
        // Arrange
        var builder = new EnvioLoteEventosBuilder();
        var builders = new[]
        {
            new MockEventBuilder(),
            new MockEventBuilder()
        };

        // Act
        var resultado = builder.ComEventosBuilder(builders);

        // Assert
        Assert.Same(builder, resultado); // Deve retornar o mesmo builder
        Assert.Equal(2, builder.ContarEventos());
    }

    [Fact]
    public void MessageProperties_DeveRetornarValoresCorretos()
    {
        // Arrange
        var builder = new EnvioLoteEventosBuilder();
        var mockEvent = new MockEvent();

        // Act
        var message = builder.AdicionarEvento(mockEvent).Build();

        // Assert
        Assert.Equal("eFinanceira", message.RootElementName);
        Assert.Null(message.IdAttributeName);
        Assert.Null(message.IdValue);
        Assert.Equal(message.EFinanceira, message.Payload);
    }

    [Fact]
    public void ConstructorSemParametros_MessageCompleta_DeveInicializarCorretamente()
    {
        // Act
        var message = new EnvioLoteEventosMessage();

        // Assert
        Assert.Equal("v1_2_0", message.Version);
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
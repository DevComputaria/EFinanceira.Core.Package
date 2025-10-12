using System.Xml;
using System.Xml.Serialization;
using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Generated.Lotes.EnvioLoteEventosAssincrono;

namespace EFinanceira.Messages.Builders.Lotes.EnvioLoteEventosAssincrono;

/// <summary>
/// Mensagem para envio de lote de eventos assíncrono usando classes geradas do XSD
/// </summary>
public sealed class EnvioLoteEventosAssincronoMessage : IEFinanceiraMessage
{
    public string Version { get; }
    public string RootElementName => "eFinanceira";
    public string? IdAttributeName => null;
    public string? IdValue => null;
    public object Payload => EFinanceira;

    /// <summary>
    /// Objeto raiz gerado do XSD
    /// </summary>
    public eFinanceira EFinanceira { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnvioLoteEventosAssincronoMessage"/> class for serialization.
    /// </summary>
    public EnvioLoteEventosAssincronoMessage()
    {
        Version = "v1_0_0";
        EFinanceira = new eFinanceira();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnvioLoteEventosAssincronoMessage"/> class.
    /// </summary>
    /// <param name="version">Versão do esquema</param>
    /// <param name="eFinanceira">Objeto raiz do XSD</param>
    public EnvioLoteEventosAssincronoMessage(string version, eFinanceira eFinanceira)
    {
        Version = version;
        EFinanceira = eFinanceira;
    }
}

/// <summary>
/// Builder para construção de lote de eventos assíncrono e-Financeira
/// </summary>
public sealed class EnvioLoteEventosAssincronoBuilder : IMessageBuilder<EnvioLoteEventosAssincronoMessage>
{
    /// <summary>
    /// Limite máximo de eventos por lote conforme especificação
    /// </summary>
    public const int MaxEventosPorLote = 100;

    private readonly string _version;
    private readonly eFinanceira _eFinanceira;
    private readonly List<TArquivoeFinanceira> _eventos;
    private string? _cnpjDeclarante;
    private int _eventoCounter;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnvioLoteEventosAssincronoBuilder"/> class.
    /// </summary>
    /// <param name="version">Versão do esquema (padrão: v1_0_0)</param>
    public EnvioLoteEventosAssincronoBuilder(string version = "v1_0_0")
    {
        _version = version;
        _eFinanceira = new eFinanceira();
        _eventos = new List<TArquivoeFinanceira>();
    }

    /// <summary>
    /// Define o CNPJ do declarante para o lote
    /// </summary>
    /// <param name="cnpjDeclarante">CNPJ do declarante</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public EnvioLoteEventosAssincronoBuilder ComCnpjDeclarante(string cnpjDeclarante)
    {
        ArgumentNullException.ThrowIfNull(cnpjDeclarante);
        _cnpjDeclarante = cnpjDeclarante;
        return this;
    }

    /// <summary>
    /// Adiciona um evento ao lote assíncrono
    /// </summary>
    /// <param name="eventoMessage">Mensagem do evento a ser adicionada</param>
    /// <param name="eventoId">ID do evento (opcional, será gerado automaticamente se não fornecido)</param>
    /// <returns>Builder para encadeamento fluente</returns>
    /// <exception cref="InvalidOperationException">Quando o limite de eventos é excedido</exception>
    public EnvioLoteEventosAssincronoBuilder AdicionarEvento(IEFinanceiraMessage eventoMessage, string? eventoId = null)
    {
        ArgumentNullException.ThrowIfNull(eventoMessage);

        if (_eventos.Count >= MaxEventosPorLote)
            throw new InvalidOperationException($"Limite máximo de {MaxEventosPorLote} eventos por lote excedido.");

        // Gerar ID automaticamente se não fornecido
        var id = eventoId ?? $"EVT_{++_eventoCounter:D3}";

        // Serializar o evento para XmlElement
        var xmlElement = SerializeEventoToXmlElement(eventoMessage);

        // Criar TArquivoeFinanceira
        var arquivo = new TArquivoeFinanceira
        {
            id = id,
            Any = xmlElement
        };

        _eventos.Add(arquivo);
        return this;
    }

    /// <summary>
    /// Adiciona um evento ao lote usando um builder
    /// </summary>
    /// <typeparam name="T">Tipo da mensagem do evento</typeparam>
    /// <param name="eventoBuilder">Builder do evento</param>
    /// <param name="eventoId">ID do evento (opcional)</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public EnvioLoteEventosAssincronoBuilder AdicionarEvento<T>(IMessageBuilder<T> eventoBuilder, string? eventoId = null)
        where T : IEFinanceiraMessage
    {
        ArgumentNullException.ThrowIfNull(eventoBuilder);

        var eventoMessage = eventoBuilder.Build();
        return AdicionarEvento(eventoMessage, eventoId);
    }

    /// <summary>
    /// Adiciona múltiplos eventos ao lote
    /// </summary>
    /// <param name="eventosMessages">Coleção de mensagens de eventos</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public EnvioLoteEventosAssincronoBuilder AdicionarEventos(IEnumerable<IEFinanceiraMessage> eventosMessages)
    {
        ArgumentNullException.ThrowIfNull(eventosMessages);

        foreach (var eventoMessage in eventosMessages)
        {
            AdicionarEvento(eventoMessage);
        }

        return this;
    }

    /// <summary>
    /// Adiciona múltiplos eventos usando builders
    /// </summary>
    /// <typeparam name="T">Tipo da mensagem do evento</typeparam>
    /// <param name="eventosBuilders">Coleção de builders de eventos</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public EnvioLoteEventosAssincronoBuilder AdicionarEventos<T>(IEnumerable<IMessageBuilder<T>> eventosBuilders)
        where T : IEFinanceiraMessage
    {
        ArgumentNullException.ThrowIfNull(eventosBuilders);

        foreach (var eventoBuilder in eventosBuilders)
        {
            AdicionarEvento(eventoBuilder);
        }

        return this;
    }

    /// <summary>
    /// Remove todos os eventos do lote
    /// </summary>
    /// <returns>Builder para encadeamento fluente</returns>
    public EnvioLoteEventosAssincronoBuilder LimparEventos()
    {
        _eventos.Clear();
        _eventoCounter = 0;
        return this;
    }

    /// <summary>
    /// Conta o número de eventos no lote
    /// </summary>
    /// <returns>Número de eventos</returns>
    public int ContarEventos() => _eventos.Count;

    /// <summary>
    /// Verifica se o lote está vazio
    /// </summary>
    /// <returns>True se o lote não possui eventos</returns>
    public bool EstaVazio() => _eventos.Count == 0;

    /// <summary>
    /// Constrói a mensagem final do lote de eventos assíncrono
    /// </summary>
    /// <returns>Mensagem do lote de eventos assíncrono</returns>
    /// <exception cref="InvalidOperationException">Quando dados obrigatórios não foram fornecidos</exception>
    public EnvioLoteEventosAssincronoMessage Build()
    {
        if (string.IsNullOrWhiteSpace(_cnpjDeclarante))
            throw new InvalidOperationException("CNPJ do declarante é obrigatório.");

        if (_eventos.Count == 0)
            throw new InvalidOperationException("Pelo menos um evento deve ser adicionado ao lote.");

        // Configurar lote de eventos assíncrono
        var loteEventos = new eFinanceiraLoteEventosAssincrono
        {
            cnpjDeclarante = _cnpjDeclarante,
            eventos = new eFinanceiraLoteEventosAssincronoEventos
            {
                evento = _eventos.ToArray()
            }
        };

        _eFinanceira.loteEventosAssincrono = loteEventos;

        return new EnvioLoteEventosAssincronoMessage(_version, _eFinanceira);
    }

    /// <summary>
    /// Serializa um evento para XmlElement
    /// </summary>
    /// <param name="eventoMessage">Mensagem do evento</param>
    /// <returns>XmlElement representando o evento</returns>
    private static XmlElement SerializeEventoToXmlElement(IEFinanceiraMessage eventoMessage)
    {
        var serializer = new XmlSerializer(eventoMessage.Payload.GetType());
        var document = new XmlDocument();

        using var stream = new MemoryStream();
        serializer.Serialize(stream, eventoMessage.Payload);
        stream.Position = 0;
        document.Load(stream);

        return document.DocumentElement ?? throw new InvalidOperationException("Falha na serialização do evento.");
    }
}

/// <summary>
/// Extensões para facilitar o uso do EnvioLoteEventosAssincronoBuilder
/// </summary>
public static class EnvioLoteEventosAssincronoBuilderExtensions
{
    /// <summary>
    /// Cria um novo builder de lote de eventos assíncrono
    /// </summary>
    /// <param name="version">Versão do esquema</param>
    /// <returns>Nova instância do builder</returns>
    public static EnvioLoteEventosAssincronoBuilder Create(string version = "v1_0_0")
    {
        return new EnvioLoteEventosAssincronoBuilder(version);
    }

    /// <summary>
    /// Adiciona múltiplos eventos usando sintaxe fluente
    /// </summary>
    /// <param name="builder">Builder</param>
    /// <param name="eventos">Array de eventos</param>
    /// <returns>Builder para encadeamento</returns>
    public static EnvioLoteEventosAssincronoBuilder ComEventos(this EnvioLoteEventosAssincronoBuilder builder, params IEFinanceiraMessage[] eventos)
    {
        return builder.AdicionarEventos(eventos);
    }

    /// <summary>
    /// Adiciona múltiplos eventos usando builders com sintaxe fluente
    /// </summary>
    /// <typeparam name="T">Tipo da mensagem</typeparam>
    /// <param name="builder">Builder</param>
    /// <param name="eventosBuilders">Array de builders de eventos</param>
    /// <returns>Builder para encadeamento</returns>
    public static EnvioLoteEventosAssincronoBuilder ComEventosBuilder<T>(this EnvioLoteEventosAssincronoBuilder builder, params IMessageBuilder<T>[] eventosBuilders)
        where T : IEFinanceiraMessage
    {
        return builder.AdicionarEventos(eventosBuilders);
    }
}
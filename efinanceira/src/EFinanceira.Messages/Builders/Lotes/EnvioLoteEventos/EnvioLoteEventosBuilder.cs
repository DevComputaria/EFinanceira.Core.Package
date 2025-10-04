using System.Xml;
using System.Xml.Serialization;
using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Generated.Lotes.EnvioLoteEventos;

namespace EFinanceira.Messages.Builders.Lotes.EnvioLoteEventos;

/// <summary>
/// Mensagem para envio de lote de eventos usando classes geradas do XSD
/// </summary>
public sealed class EnvioLoteEventosMessage : IEFinanceiraMessage
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
    /// Initializes a new instance of the <see cref="EnvioLoteEventosMessage"/> class for serialization.
    /// </summary>
    public EnvioLoteEventosMessage()
    {
        Version = "v1_2_0";
        EFinanceira = new eFinanceira();
    }

    internal EnvioLoteEventosMessage(eFinanceira eFinanceira, string version)
    {
        EFinanceira = eFinanceira;
        Version = version;
    }
}

/// <summary>
/// Builder fluente para envio de lote de eventos usando classes geradas do XSD
/// </summary>
public sealed class EnvioLoteEventosBuilder : IMessageBuilder<EnvioLoteEventosMessage>
{
    private readonly eFinanceira _eFinanceira;
    private readonly string _version;
    private readonly List<TArquivoeFinanceira> _eventos;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnvioLoteEventosBuilder"/> class.
    /// </summary>
    /// <param name="version">Versão do schema (padrão: v1_2_0)</param>
    public EnvioLoteEventosBuilder(string version = "v1_2_0")
    {
        _version = version;
        _eFinanceira = new eFinanceira
        {
            loteEventos = new eFinanceiraLoteEventos()
        };
        _eventos = new List<TArquivoeFinanceira>();
    }

    /// <summary>
    /// Adiciona um evento ao lote a partir de uma mensagem IEFinanceiraMessage
    /// </summary>
    /// <param name="eventoMessage">Mensagem do evento</param>
    /// <param name="eventoId">ID do evento (opcional, será gerado automaticamente se não fornecido)</param>
    /// <returns>O próprio builder para fluent interface</returns>
    public EnvioLoteEventosBuilder AdicionarEvento(IEFinanceiraMessage eventoMessage, string? eventoId = null)
    {
        var evento = new TArquivoeFinanceira
        {
            id = eventoId ?? GenerateEventoId(),
            Any = SerializeEventoToXmlElement(eventoMessage)
        };

        _eventos.Add(evento);
        return this;
    }

    /// <summary>
    /// Adiciona um evento ao lote usando um builder
    /// </summary>
    /// <typeparam name="T">Tipo da mensagem do evento</typeparam>
    /// <param name="eventoBuilder">Builder do evento</param>
    /// <param name="eventoId">ID do evento (opcional, será gerado automaticamente se não fornecido)</param>
    /// <returns>O próprio builder para fluent interface</returns>
    public EnvioLoteEventosBuilder AdicionarEvento<T>(IMessageBuilder<T> eventoBuilder, string? eventoId = null)
        where T : IEFinanceiraMessage
    {
        var eventoMessage = eventoBuilder.Build();
        return AdicionarEvento(eventoMessage, eventoId);
    }

    /// <summary>
    /// Adiciona múltiplos eventos ao lote
    /// </summary>
    /// <param name="eventosMessages">Coleção de mensagens de eventos</param>
    /// <returns>O próprio builder para fluent interface</returns>
    public EnvioLoteEventosBuilder AdicionarEventos(IEnumerable<IEFinanceiraMessage> eventosMessages)
    {
        foreach (var evento in eventosMessages)
        {
            AdicionarEvento(evento);
        }
        return this;
    }

    /// <summary>
    /// Adiciona múltiplos eventos usando builders
    /// </summary>
    /// <typeparam name="T">Tipo das mensagens dos eventos</typeparam>
    /// <param name="eventosBuilders">Coleção de builders de eventos</param>
    /// <returns>O próprio builder para fluent interface</returns>
    public EnvioLoteEventosBuilder AdicionarEventos<T>(IEnumerable<IMessageBuilder<T>> eventosBuilders)
        where T : IEFinanceiraMessage
    {
        foreach (var builder in eventosBuilders)
        {
            AdicionarEvento(builder);
        }
        return this;
    }

    /// <summary>
    /// Remove todos os eventos do lote
    /// </summary>
    /// <returns>O próprio builder para fluent interface</returns>
    public EnvioLoteEventosBuilder LimparEventos()
    {
        _eventos.Clear();
        return this;
    }

    /// <summary>
    /// Obtém o número de eventos no lote
    /// </summary>
    /// <returns>Número de eventos</returns>
    public int ContarEventos() => _eventos.Count;

    /// <summary>
    /// Verifica se o lote está vazio
    /// </summary>
    /// <returns>True se não há eventos, false caso contrário</returns>
    public bool EstaVazio() => _eventos.Count == 0;

    /// <summary>
    /// Constrói a mensagem final
    /// </summary>
    /// <returns>Mensagem de envio de lote de eventos</returns>
    public EnvioLoteEventosMessage Build()
    {
        ValidateRequiredFields();

        // Atribuir eventos ao lote
        _eFinanceira.loteEventos.evento = _eventos.ToArray();

        return new EnvioLoteEventosMessage(_eFinanceira, _version);
    }

    #region Métodos privados

    /// <summary>
    /// Valida os campos obrigatórios
    /// </summary>
    private void ValidateRequiredFields()
    {
        if (_eventos.Count == 0)
            throw new InvalidOperationException("Lote deve conter pelo menos um evento");

        if (_eventos.Count > 100)
            throw new InvalidOperationException("Lote não pode conter mais de 100 eventos");
    }

    /// <summary>
    /// Gera um ID único para o evento
    /// </summary>
    /// <returns>ID do evento</returns>
    private static string GenerateEventoId()
    {
        return $"EVT_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}"[..30];
    }

    /// <summary>
    /// Serializa uma mensagem de evento para XmlElement
    /// </summary>
    /// <param name="eventoMessage">Mensagem do evento</param>
    /// <returns>XmlElement representando o evento</returns>
    private static XmlElement SerializeEventoToXmlElement(IEFinanceiraMessage eventoMessage)
    {
        var doc = new XmlDocument();

        // Criar serializer para o tipo do payload
        var serializer = new XmlSerializer(eventoMessage.Payload.GetType());

        // Serializar para string primeiro
        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            Indent = false
        });

        serializer.Serialize(xmlWriter, eventoMessage.Payload);
        var xmlString = stringWriter.ToString();

        // Carregar como XmlDocument e retornar o DocumentElement
        doc.LoadXml(xmlString);
        return doc.DocumentElement!;
    }

    #endregion
}

/// <summary>
/// Extensões para facilitar o uso do builder de envio de lote de eventos
/// </summary>
public static class EnvioLoteEventosBuilderExtensions
{
    /// <summary>
    /// Cria um lote com múltiplos eventos de uma vez
    /// </summary>
    /// <param name="builder">Builder do lote</param>
    /// <param name="eventos">Eventos a serem adicionados</param>
    /// <returns>O builder configurado</returns>
    public static EnvioLoteEventosBuilder ComEventos(
        this EnvioLoteEventosBuilder builder,
        params IEFinanceiraMessage[] eventos)
    {
        return builder.AdicionarEventos(eventos);
    }

    /// <summary>
    /// Cria um lote usando builders de eventos
    /// </summary>
    /// <typeparam name="T">Tipo das mensagens dos eventos</typeparam>
    /// <param name="builder">Builder do lote</param>
    /// <param name="eventosBuilders">Builders dos eventos</param>
    /// <returns>O builder configurado</returns>
    public static EnvioLoteEventosBuilder ComEventosBuilder<T>(
        this EnvioLoteEventosBuilder builder,
        params IMessageBuilder<T>[] eventosBuilders)
        where T : IEFinanceiraMessage
    {
        return builder.AdicionarEventos(eventosBuilders);
    }

    /// <summary>
    /// Cria um novo builder de envio de lote de eventos
    /// </summary>
    /// <param name="version">Versão do schema</param>
    /// <returns>Novo builder</returns>
    public static EnvioLoteEventosBuilder Create(string version = "v1_2_0")
    {
        return new EnvioLoteEventosBuilder(version);
    }
}
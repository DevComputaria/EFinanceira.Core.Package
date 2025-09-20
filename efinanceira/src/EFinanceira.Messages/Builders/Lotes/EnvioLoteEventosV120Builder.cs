using EFinanceira.Core.Abstractions;

namespace EFinanceira.Messages.Builders.Lotes;

/// <summary>
/// Mensagem para envio de lote de eventos v1.2.0
/// </summary>
public sealed class EnvioLoteEventosV120Message : IEFinanceiraMessage
{
    public string Version { get; }
    public string RootElementName => "envioLoteEventos";
    public string? IdAttributeName => "Id";
    public string? IdValue { get; internal set; }
    public object Payload => Lote;

    /// <summary>
    /// Lote tipado (será substituído por POCO gerado do XSD)
    /// </summary>
    public EnvioLoteEventos Lote { get; }

    internal EnvioLoteEventosV120Message(EnvioLoteEventos lote, string version)
    {
        Lote = lote;
        Version = version;
        IdValue = lote.Id;
    }
}

/// <summary>
/// Builder fluente para lote de eventos v1.2.0
/// </summary>
public sealed class EnvioLoteEventosV120Builder : IMessageBuilder<EnvioLoteEventosV120Message>
{
    private readonly EnvioLoteEventos _lote;
    private readonly string _version;

    public EnvioLoteEventosV120Builder(string version = "v1_2_0")
    {
        _version = version;
        _lote = new EnvioLoteEventos
        {
            Id = GenerateId(),
            IdeTransmissor = new IdeTransmissor(),
            Eventos = new List<EventoLote>()
        };
    }

    /// <summary>
    /// Define o ID do lote
    /// </summary>
    public EnvioLoteEventosV120Builder WithId(string id)
    {
        _lote.Id = id;
        return this;
    }

    /// <summary>
    /// Define os dados do transmissor
    /// </summary>
    public EnvioLoteEventosV120Builder WithTransmissor(string cnpj)
    {
        _lote.IdeTransmissor.CnpjTransmissor = cnpj;
        return this;
    }

    /// <summary>
    /// Define dados completos do transmissor
    /// </summary>
    public EnvioLoteEventosV120Builder WithTransmissor(Action<IdeTransmissorBuilder> configurar)
    {
        var builder = new IdeTransmissorBuilder();
        configurar(builder);
        _lote.IdeTransmissor = builder.Build();
        return this;
    }

    /// <summary>
    /// Adiciona um evento ao lote
    /// </summary>
    public EnvioLoteEventosV120Builder AdicionarEvento(IEFinanceiraMessage eventoMessage)
    {
        var evento = new EventoLote
        {
            Id = eventoMessage.IdValue ?? GenerateEventoId(),
            XmlEvento = eventoMessage.Payload
        };

        _lote.Eventos.Add(evento);
        return this;
    }

    /// <summary>
    /// Adiciona um evento usando builder específico
    /// </summary>
    public EnvioLoteEventosV120Builder AdicionarEvento<T>(IMessageBuilder<T> eventoBuilder)
        where T : IEFinanceiraMessage
    {
        var eventoMessage = eventoBuilder.Build();
        return AdicionarEvento(eventoMessage);
    }

    /// <summary>
    /// Adiciona múltiplos eventos
    /// </summary>
    public EnvioLoteEventosV120Builder AdicionarEventos(IEnumerable<IEFinanceiraMessage> eventos)
    {
        foreach (var evento in eventos)
        {
            AdicionarEvento(evento);
        }
        return this;
    }

    /// <summary>
    /// Remove todos os eventos do lote
    /// </summary>
    public EnvioLoteEventosV120Builder LimparEventos()
    {
        _lote.Eventos.Clear();
        return this;
    }

    /// <inheritdoc />
    public EnvioLoteEventosV120Message Build()
    {
        ValidateRequiredFields();
        return new EnvioLoteEventosV120Message(_lote, _version);
    }

    private void ValidateRequiredFields()
    {
        if (string.IsNullOrEmpty(_lote.Id))
            throw new InvalidOperationException("Id do lote é obrigatório");

        if (string.IsNullOrEmpty(_lote.IdeTransmissor?.CnpjTransmissor))
            throw new InvalidOperationException("CNPJ do transmissor é obrigatório");

        if (_lote.Eventos.Count == 0)
            throw new InvalidOperationException("Lote deve conter pelo menos um evento");

        if (_lote.Eventos.Count > 100)
            throw new InvalidOperationException("Lote não pode conter mais de 100 eventos");
    }

    private static string GenerateId()
    {
        return $"LOTE{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }

    private static string GenerateEventoId()
    {
        return $"EVT{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
}

/// <summary>
/// Builder para dados do transmissor
/// </summary>
public sealed class IdeTransmissorBuilder
{
    private readonly IdeTransmissor _ideTransmissor = new();

    public IdeTransmissorBuilder WithCnpj(string cnpj)
    {
        _ideTransmissor.CnpjTransmissor = cnpj;
        return this;
    }

    public IdeTransmissorBuilder WithNome(string nome)
    {
        _ideTransmissor.NomeTransmissor = nome;
        return this;
    }

    public IdeTransmissorBuilder WithEmail(string email)
    {
        _ideTransmissor.EmailTransmissor = email;
        return this;
    }

    public IdeTransmissorBuilder WithTelefone(string telefone)
    {
        _ideTransmissor.TelefoneTransmissor = telefone;
        return this;
    }

    internal IdeTransmissor Build() => _ideTransmissor;
}

/// <summary>
/// Extensões para facilitar o uso do builder de lotes
/// </summary>
public static class EnvioLoteEventosBuilderExtensions
{
    /// <summary>
    /// Cria um lote com múltiplos eventos de uma vez
    /// </summary>
    public static EnvioLoteEventosV120Builder ComEventos(
        this EnvioLoteEventosV120Builder builder,
        params IEFinanceiraMessage[] eventos)
    {
        return builder.AdicionarEventos(eventos);
    }

    /// <summary>
    /// Cria um lote usando builders de eventos
    /// </summary>
    public static EnvioLoteEventosV120Builder ComEventosBuilder<T>(
        this EnvioLoteEventosV120Builder builder,
        params IMessageBuilder<T>[] eventosBuilders)
        where T : IEFinanceiraMessage
    {
        foreach (var eventoBuilder in eventosBuilders)
        {
            builder.AdicionarEvento(eventoBuilder);
        }
        return builder;
    }

    /// <summary>
    /// Configura transmissor de forma fluente
    /// </summary>
    public static EnvioLoteEventosV120Builder ComTransmissor(
        this EnvioLoteEventosV120Builder builder,
        string cnpj,
        string? nome = null,
        string? email = null,
        string? telefone = null)
    {
        return builder.WithTransmissor(t =>
        {
            t.WithCnpj(cnpj);
            if (!string.IsNullOrEmpty(nome)) t.WithNome(nome);
            if (!string.IsNullOrEmpty(email)) t.WithEmail(email);
            if (!string.IsNullOrEmpty(telefone)) t.WithTelefone(telefone);
        });
    }
}

// Classes de modelo temporárias (serão substituídas pelos POCOs gerados do XSD)
#region Modelos Temporários

public class EnvioLoteEventos
{
    public string Id { get; set; } = string.Empty;
    public IdeTransmissor IdeTransmissor { get; set; } = new();
    public List<EventoLote> Eventos { get; set; } = new();
}

public class IdeTransmissor
{
    public string CnpjTransmissor { get; set; } = string.Empty;
    public string? NomeTransmissor { get; set; }
    public string? EmailTransmissor { get; set; }
    public string? TelefoneTransmissor { get; set; }
}

public class EventoLote
{
    public string Id { get; set; } = string.Empty;
    public object XmlEvento { get; set; } = new();
}

#endregion

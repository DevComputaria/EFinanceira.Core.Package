using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Generated.Eventos.EvtExclusaoeFinanceira;

namespace EFinanceira.Messages.Builders.Eventos.EvtExclusaoeFinanceira;

/// <summary>
/// Mensagem de evento de Exclusão e-Financeira
/// </summary>
public sealed class EvtExclusaoeFinanceiraMessage : IEFinanceiraMessage
{
    public string Version { get; private set; }
    public string RootElementName => "evtExclusaoeFinanceira";
    public string? IdAttributeName => "id";
    public string? IdValue { get; internal set; }
    public object Payload => Evento;

    /// <summary>
    /// Evento tipado gerado do XSD
    /// </summary>
    public eFinanceiraEvtExclusaoeFinanceira Evento { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EvtExclusaoeFinanceiraMessage"/> class.
    /// Construtor sem parâmetros necessário para serialização XML.
    /// </summary>
    public EvtExclusaoeFinanceiraMessage()
    {
        Version = "v1_2_0";
        Evento = new eFinanceiraEvtExclusaoeFinanceira();
        IdValue = string.Empty;
    }

    internal EvtExclusaoeFinanceiraMessage(eFinanceiraEvtExclusaoeFinanceira evento, string version)
    {
        Evento = evento;
        Version = version;
        IdValue = evento.id;
    }
}

/// <summary>
/// Builder principal para construção de eventos de Exclusão e-Financeira.
/// Utiliza o padrão fluent interface para facilitar a criação de eventos estruturados.
/// </summary>
public sealed class EvtExclusaoeFinanceiraBuilder : IMessageBuilder<EvtExclusaoeFinanceiraMessage>
{
    private readonly eFinanceiraEvtExclusaoeFinanceira _evento;
    private readonly string _version;

    /// <summary>
    /// Initializes a new instance of the <see cref="EvtExclusaoeFinanceiraBuilder"/> class.
    /// </summary>
    /// <param name="version">Versão do schema (padrão: v1_2_0)</param>
    public EvtExclusaoeFinanceiraBuilder(string version = "v1_2_0")
    {
        _version = version;
        _evento = new eFinanceiraEvtExclusaoeFinanceira
        {
            id = GenerateId(),
            ideEvento = new eFinanceiraEvtExclusaoeFinanceiraIdeEvento
            {
                tpAmb = 2, // Homologação
                aplicEmi = 1, // Aplicativo do contribuinte
                verAplic = "1.0.0"
            }
        };
    }

    /// <summary>
    /// Cria uma nova instância do builder EvtExclusaoeFinanceira
    /// </summary>
    /// <param name="version">Versão do schema</param>
    /// <returns>Nova instância do builder</returns>
    public static EvtExclusaoeFinanceiraBuilder Create(string version = "v1_2_0") => new(version);

    /// <summary>
    /// Define o ID do evento
    /// </summary>
    /// <param name="id">ID único do evento</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtExclusaoeFinanceiraBuilder WithId(string id)
    {
        _evento.id = id;
        return this;
    }

    /// <summary>
    /// Configura os dados da identificação do evento
    /// </summary>
    /// <param name="configAction">Ação para configurar o IdeEvento</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtExclusaoeFinanceiraBuilder WithIdeEvento(Action<IdeEventoBuilder> configAction)
    {
        var builder = new IdeEventoBuilder();
        configAction(builder);
        _evento.ideEvento = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura os dados da identificação do declarante
    /// </summary>
    /// <param name="configAction">Ação para configurar o IdeDeclarante</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtExclusaoeFinanceiraBuilder WithIdeDeclarante(Action<IdeDeclaranteBuilder> configAction)
    {
        var builder = new IdeDeclaranteBuilder();
        configAction(builder);
        _evento.ideDeclarante = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura as informações de exclusão e-Financeira
    /// </summary>
    /// <param name="configAction">Ação para configurar o InfoExclusaoeFinanceira</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtExclusaoeFinanceiraBuilder WithInfoExclusaoeFinanceira(Action<InfoExclusaoeFinanceiraBuilder> configAction)
    {
        var builder = new InfoExclusaoeFinanceiraBuilder();
        configAction(builder);
        _evento.infoExclusaoeFinanceira = builder.Build();
        return this;
    }

    /// <summary>
    /// Constrói a instância final da mensagem de evento
    /// </summary>
    /// <returns>Instância configurada de EvtExclusaoeFinanceiraMessage</returns>
    public EvtExclusaoeFinanceiraMessage Build()
    {
        ValidateRequiredFields();
        return new EvtExclusaoeFinanceiraMessage(_evento, _version);
    }

    /// <summary>
    /// Valida se todos os campos obrigatórios foram preenchidos
    /// </summary>
    private void ValidateRequiredFields()
    {
        if (_evento.ideEvento == null)
            throw new InvalidOperationException("IdeEvento é obrigatório");
        
        if (_evento.ideDeclarante == null)
            throw new InvalidOperationException("IdeDeclarante é obrigatório");
        
        if (_evento.infoExclusaoeFinanceira == null)
            throw new InvalidOperationException("InfoExclusaoeFinanceira é obrigatório");
    }

    /// <summary>
    /// Gera um ID único para o evento
    /// </summary>
    /// <returns>ID único no formato ID_GUID</returns>
    private static string GenerateId() => $"ID_{Guid.NewGuid():N}";
}

#region Builders auxiliares

/// <summary>
/// Builder para configuração da identificação do evento
/// </summary>
public sealed class IdeEventoBuilder
{
    private readonly eFinanceiraEvtExclusaoeFinanceiraIdeEvento _ideEvento;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdeEventoBuilder"/> class.
    /// </summary>
    public IdeEventoBuilder()
    {
        _ideEvento = new eFinanceiraEvtExclusaoeFinanceiraIdeEvento
        {
            tpAmb = 2, // Homologação
            aplicEmi = 1, // Aplicativo do contribuinte
            verAplic = "1.0.0"
        };
    }

    /// <summary>
    /// Define o tipo de ambiente
    /// </summary>
    /// <param name="ambiente">1-Produção, 2-Homologação</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeEventoBuilder WithAmbiente(uint ambiente)
    {
        _ideEvento.tpAmb = ambiente;
        return this;
    }

    /// <summary>
    /// Define o aplicativo emissor
    /// </summary>
    /// <param name="aplicativo">1-Aplicativo do contribuinte</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeEventoBuilder WithAplicativoEmi(uint aplicativo)
    {
        _ideEvento.aplicEmi = aplicativo;
        return this;
    }

    /// <summary>
    /// Define a versão do aplicativo
    /// </summary>
    /// <param name="versao">Versão do aplicativo</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeEventoBuilder WithVersaoAplicativo(string versao)
    {
        _ideEvento.verAplic = versao;
        return this;
    }

    /// <summary>
    /// Constrói a instância final da identificação do evento
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtExclusaoeFinanceiraIdeEvento</returns>
    internal eFinanceiraEvtExclusaoeFinanceiraIdeEvento Build() => _ideEvento;
}

/// <summary>
/// Builder para configuração da identificação do declarante
/// </summary>
public sealed class IdeDeclaranteBuilder
{
    private readonly eFinanceiraEvtExclusaoeFinanceiraIdeDeclarante _ideDeclarante;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdeDeclaranteBuilder"/> class.
    /// </summary>
    public IdeDeclaranteBuilder()
    {
        _ideDeclarante = new eFinanceiraEvtExclusaoeFinanceiraIdeDeclarante();
    }

    /// <summary>
    /// Define o CNPJ do declarante
    /// </summary>
    /// <param name="cnpj">CNPJ do declarante</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeDeclaranteBuilder WithCnpj(string cnpj)
    {
        _ideDeclarante.cnpjDeclarante = cnpj;
        return this;
    }

    /// <summary>
    /// Constrói a instância final da identificação do declarante
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtExclusaoeFinanceiraIdeDeclarante</returns>
    internal eFinanceiraEvtExclusaoeFinanceiraIdeDeclarante Build() => _ideDeclarante;
}

/// <summary>
/// Builder para configuração das informações de exclusão e-Financeira
/// </summary>
public sealed class InfoExclusaoeFinanceiraBuilder
{
    private readonly eFinanceiraEvtExclusaoeFinanceiraInfoExclusaoeFinanceira _infoExclusao;

    /// <summary>
    /// Initializes a new instance of the <see cref="InfoExclusaoeFinanceiraBuilder"/> class.
    /// </summary>
    public InfoExclusaoeFinanceiraBuilder()
    {
        _infoExclusao = new eFinanceiraEvtExclusaoeFinanceiraInfoExclusaoeFinanceira();
    }

    /// <summary>
    /// Define o número do recibo do evento e-Financeira a ser excluído
    /// </summary>
    /// <param name="numeroRecibo">Número do recibo do evento e-Financeira</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoExclusaoeFinanceiraBuilder WithNumeroReciboEvento(string numeroRecibo)
    {
        _infoExclusao.nrReciboEvento = numeroRecibo;
        return this;
    }

    /// <summary>
    /// Constrói a instância final das informações de exclusão e-Financeira
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtExclusaoeFinanceiraInfoExclusaoeFinanceira</returns>
    internal eFinanceiraEvtExclusaoeFinanceiraInfoExclusaoeFinanceira Build() => _infoExclusao;
}

#endregion

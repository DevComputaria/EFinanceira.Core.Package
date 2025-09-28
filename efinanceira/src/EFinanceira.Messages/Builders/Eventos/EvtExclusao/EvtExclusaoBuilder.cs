using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Generated.Eventos.EvtExclusao;

namespace EFinanceira.Messages.Builders.Eventos.EvtExclusao;

/// <summary>
/// Mensagem de evento de Exclusão e-Financeira
/// </summary>
public sealed class EvtExclusaoMessage : IEFinanceiraMessage
{
    public string Version { get; private set; }
    public string RootElementName => "evtExclusao";
    public string? IdAttributeName => "id";
    public string? IdValue { get; internal set; }
    public object Payload => Evento;

    /// <summary>
    /// Evento tipado gerado do XSD
    /// </summary>
    public eFinanceiraEvtExclusao Evento { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EvtExclusaoMessage"/> class.
    /// Construtor sem parâmetros necessário para serialização XML.
    /// </summary>
    public EvtExclusaoMessage()
    {
        Version = "v1_2_0";
        Evento = new eFinanceiraEvtExclusao();
        IdValue = string.Empty;
    }

    internal EvtExclusaoMessage(eFinanceiraEvtExclusao evento, string version)
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
public sealed class EvtExclusaoBuilder : IMessageBuilder<EvtExclusaoMessage>
{
    private readonly eFinanceiraEvtExclusao _evento;
    private readonly string _version;

    /// <summary>
    /// Initializes a new instance of the <see cref="EvtExclusaoBuilder"/> class.
    /// </summary>
    /// <param name="version">Versão do schema (padrão: v1_2_0)</param>
    public EvtExclusaoBuilder(string version = "v1_2_0")
    {
        _version = version;
        _evento = new eFinanceiraEvtExclusao
        {
            id = GenerateId(),
            ideEvento = new eFinanceiraEvtExclusaoIdeEvento
            {
                tpAmb = 2, // Homologação
                aplicEmi = 1, // Aplicativo do contribuinte
                verAplic = "1.0.0"
            }
        };
    }

    /// <summary>
    /// Cria uma nova instância do builder EvtExclusao
    /// </summary>
    /// <param name="version">Versão do schema</param>
    /// <returns>Nova instância do builder</returns>
    public static EvtExclusaoBuilder Create(string version = "v1_2_0") => new(version);

    /// <summary>
    /// Define o ID do evento
    /// </summary>
    /// <param name="id">ID único do evento</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtExclusaoBuilder WithId(string id)
    {
        _evento.id = id;
        return this;
    }

    /// <summary>
    /// Configura os dados da identificação do evento
    /// </summary>
    /// <param name="configAction">Ação para configurar o IdeEvento</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtExclusaoBuilder WithIdeEvento(Action<IdeEventoBuilder> configAction)
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
    public EvtExclusaoBuilder WithIdeDeclarante(Action<IdeDeclaranteBuilder> configAction)
    {
        var builder = new IdeDeclaranteBuilder();
        configAction(builder);
        _evento.ideDeclarante = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura as informações de exclusão
    /// </summary>
    /// <param name="configAction">Ação para configurar o InfoExclusao</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtExclusaoBuilder WithInfoExclusao(Action<InfoExclusaoBuilder> configAction)
    {
        var builder = new InfoExclusaoBuilder();
        configAction(builder);
        _evento.infoExclusao = builder.Build();
        return this;
    }

    /// <summary>
    /// Constrói a instância final da mensagem de evento
    /// </summary>
    /// <returns>Instância configurada de EvtExclusaoMessage</returns>
    public EvtExclusaoMessage Build()
    {
        ValidateRequiredFields();
        return new EvtExclusaoMessage(_evento, _version);
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
        
        if (_evento.infoExclusao == null)
            throw new InvalidOperationException("InfoExclusao é obrigatório");
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
    private readonly eFinanceiraEvtExclusaoIdeEvento _ideEvento;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdeEventoBuilder"/> class.
    /// </summary>
    public IdeEventoBuilder()
    {
        _ideEvento = new eFinanceiraEvtExclusaoIdeEvento
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
    /// <returns>Instância configurada de eFinanceiraEvtExclusaoIdeEvento</returns>
    internal eFinanceiraEvtExclusaoIdeEvento Build() => _ideEvento;
}

/// <summary>
/// Builder para configuração da identificação do declarante
/// </summary>
public sealed class IdeDeclaranteBuilder
{
    private readonly eFinanceiraEvtExclusaoIdeDeclarante _ideDeclarante;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdeDeclaranteBuilder"/> class.
    /// </summary>
    public IdeDeclaranteBuilder()
    {
        _ideDeclarante = new eFinanceiraEvtExclusaoIdeDeclarante();
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
    /// <returns>Instância configurada de eFinanceiraEvtExclusaoIdeDeclarante</returns>
    internal eFinanceiraEvtExclusaoIdeDeclarante Build() => _ideDeclarante;
}

/// <summary>
/// Builder para configuração das informações de exclusão
/// </summary>
public sealed class InfoExclusaoBuilder
{
    private readonly eFinanceiraEvtExclusaoInfoExclusao _infoExclusao;

    /// <summary>
    /// Initializes a new instance of the <see cref="InfoExclusaoBuilder"/> class.
    /// </summary>
    public InfoExclusaoBuilder()
    {
        _infoExclusao = new eFinanceiraEvtExclusaoInfoExclusao();
    }

    /// <summary>
    /// Define o número do recibo do evento a ser excluído
    /// </summary>
    /// <param name="numeroRecibo">Número do recibo do evento</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoExclusaoBuilder WithNumeroReciboEvento(string numeroRecibo)
    {
        _infoExclusao.nrReciboEvento = numeroRecibo;
        return this;
    }

    /// <summary>
    /// Constrói a instância final das informações de exclusão
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtExclusaoInfoExclusao</returns>
    internal eFinanceiraEvtExclusaoInfoExclusao Build() => _infoExclusao;
}

#endregion

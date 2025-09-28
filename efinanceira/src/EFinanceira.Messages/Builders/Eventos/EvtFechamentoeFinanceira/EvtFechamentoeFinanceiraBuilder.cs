using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Generated.Eventos.EvtFechamentoeFinanceira;

namespace EFinanceira.Messages.Builders.Eventos.EvtFechamentoeFinanceira;

/// <summary>
/// Mensagem de evento de Fechamento e-Financeira
/// </summary>
public sealed class EvtFechamentoeFinanceiraMessage : IEFinanceiraMessage
{
    public string Version { get; private set; }
    public string RootElementName => "evtFechamentoeFinanceira";
    public string? IdAttributeName => "id";
    public string? IdValue { get; internal set; }
    public object Payload => Evento;

    /// <summary>
    /// Evento tipado gerado do XSD
    /// </summary>
    public eFinanceiraEvtFechamentoeFinanceira Evento { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EvtFechamentoeFinanceiraMessage"/> class.
    /// Construtor sem parâmetros necessário para serialização XML.
    /// </summary>
    public EvtFechamentoeFinanceiraMessage()
    {
        Version = "v1_2_2";
        Evento = new eFinanceiraEvtFechamentoeFinanceira();
        IdValue = string.Empty;
    }

    internal EvtFechamentoeFinanceiraMessage(eFinanceiraEvtFechamentoeFinanceira evento, string version)
    {
        Evento = evento;
        Version = version;
        IdValue = evento.id;
    }
}

/// <summary>
/// Builder principal para construção de eventos de Fechamento e-Financeira.
/// Utiliza o padrão fluent interface para facilitar a criação de eventos estruturados.
/// </summary>
public sealed class EvtFechamentoeFinanceiraBuilder : IMessageBuilder<EvtFechamentoeFinanceiraMessage>
{
    private readonly eFinanceiraEvtFechamentoeFinanceira _evento;
    private readonly string _version;

    /// <summary>
    /// Initializes a new instance of the <see cref="EvtFechamentoeFinanceiraBuilder"/> class.
    /// </summary>
    /// <param name="version">Versão do schema (padrão: v1_2_2)</param>
    public EvtFechamentoeFinanceiraBuilder(string version = "v1_2_2")
    {
        _version = version;
        _evento = new eFinanceiraEvtFechamentoeFinanceira
        {
            id = GenerateId(),
            ideEvento = new eFinanceiraEvtFechamentoeFinanceiraIdeEvento
            {
                indRetificacao = 1, // Original
                tpAmb = 2, // Homologação
                aplicEmi = 1, // Aplicativo do contribuinte
                verAplic = "1.0.0"
            }
        };
    }

    /// <summary>
    /// Cria uma nova instância do builder EvtFechamentoeFinanceira
    /// </summary>
    /// <param name="version">Versão do schema</param>
    /// <returns>Nova instância do builder</returns>
    public static EvtFechamentoeFinanceiraBuilder Create(string version = "v1_2_2") => new(version);

    /// <summary>
    /// Define o ID do evento
    /// </summary>
    /// <param name="id">ID único do evento</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtFechamentoeFinanceiraBuilder WithId(string id)
    {
        _evento.id = id;
        return this;
    }

    /// <summary>
    /// Configura os dados da identificação do evento
    /// </summary>
    /// <param name="configAction">Ação para configurar o IdeEvento</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtFechamentoeFinanceiraBuilder WithIdeEvento(Action<IdeEventoBuilder> configAction)
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
    public EvtFechamentoeFinanceiraBuilder WithIdeDeclarante(Action<IdeDeclaranteBuilder> configAction)
    {
        var builder = new IdeDeclaranteBuilder();
        configAction(builder);
        _evento.ideDeclarante = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura as informações de fechamento
    /// </summary>
    /// <param name="configAction">Ação para configurar o InfoFechamento</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtFechamentoeFinanceiraBuilder WithInfoFechamento(Action<InfoFechamentoBuilder> configAction)
    {
        var builder = new InfoFechamentoBuilder();
        configAction(builder);
        _evento.infoFechamento = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura os fechamentos PP (Pessoa Política) por mês
    /// </summary>
    /// <param name="configAction">Ação para configurar os fechamentos PP</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtFechamentoeFinanceiraBuilder WithFechamentoPP(Action<FechamentoPPBuilder> configAction)
    {
        var builder = new FechamentoPPBuilder();
        configAction(builder);
        _evento.FechamentoPP = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura o fechamento de movimento de operação financeira mensal
    /// </summary>
    /// <param name="configAction">Ação para configurar o FechamentoMovOpFin</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtFechamentoeFinanceiraBuilder WithFechamentoMovOpFin(Action<FechamentoMovOpFinBuilder> configAction)
    {
        var builder = new FechamentoMovOpFinBuilder();
        configAction(builder);
        _evento.FechamentoMovOpFin = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura o fechamento de movimento de operação financeira anual
    /// </summary>
    /// <param name="configAction">Ação para configurar o FechamentoMovOpFinAnual</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtFechamentoeFinanceiraBuilder WithFechamentoMovOpFinAnual(Action<FechamentoMovOpFinAnualBuilder> configAction)
    {
        var builder = new FechamentoMovOpFinAnualBuilder();
        configAction(builder);
        _evento.FechamentoMovOpFinAnual = builder.Build();
        return this;
    }

    /// <summary>
    /// Constrói a instância final da mensagem de evento
    /// </summary>
    /// <returns>Instância configurada de EvtFechamentoeFinanceiraMessage</returns>
    public EvtFechamentoeFinanceiraMessage Build()
    {
        ValidateRequiredFields();
        return new EvtFechamentoeFinanceiraMessage(_evento, _version);
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
        
        if (_evento.infoFechamento == null)
            throw new InvalidOperationException("InfoFechamento é obrigatório");
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
    private readonly eFinanceiraEvtFechamentoeFinanceiraIdeEvento _ideEvento;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdeEventoBuilder"/> class.
    /// </summary>
    public IdeEventoBuilder()
    {
        _ideEvento = new eFinanceiraEvtFechamentoeFinanceiraIdeEvento
        {
            indRetificacao = 1, // Original
            tpAmb = 2, // Homologação
            aplicEmi = 1, // Aplicativo do contribuinte
            verAplic = "1.0.0"
        };
    }

    /// <summary>
    /// Define o indicador de retificação
    /// </summary>
    /// <param name="indicador">1-Original, 2-Retificadora</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeEventoBuilder WithIndicadorRetificacao(uint indicador)
    {
        _ideEvento.indRetificacao = indicador;
        return this;
    }

    /// <summary>
    /// Define o número do recibo (obrigatório se for retificadora)
    /// </summary>
    /// <param name="numeroRecibo">Número do recibo da declaração original</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeEventoBuilder WithNumeroRecibo(string numeroRecibo)
    {
        _ideEvento.nrRecibo = numeroRecibo;
        return this;
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
    /// <returns>Instância configurada de eFinanceiraEvtFechamentoeFinanceiraIdeEvento</returns>
    internal eFinanceiraEvtFechamentoeFinanceiraIdeEvento Build() => _ideEvento;
}

/// <summary>
/// Builder para configuração da identificação do declarante
/// </summary>
public sealed class IdeDeclaranteBuilder
{
    private readonly eFinanceiraEvtFechamentoeFinanceiraIdeDeclarante _ideDeclarante;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdeDeclaranteBuilder"/> class.
    /// </summary>
    public IdeDeclaranteBuilder()
    {
        _ideDeclarante = new eFinanceiraEvtFechamentoeFinanceiraIdeDeclarante();
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
    /// <returns>Instância configurada de eFinanceiraEvtFechamentoeFinanceiraIdeDeclarante</returns>
    internal eFinanceiraEvtFechamentoeFinanceiraIdeDeclarante Build() => _ideDeclarante;
}

/// <summary>
/// Builder para configuração das informações de fechamento
/// </summary>
public sealed class InfoFechamentoBuilder
{
    private readonly eFinanceiraEvtFechamentoeFinanceiraInfoFechamento _infoFechamento;

    /// <summary>
    /// Initializes a new instance of the <see cref="InfoFechamentoBuilder"/> class.
    /// </summary>
    public InfoFechamentoBuilder()
    {
        _infoFechamento = new eFinanceiraEvtFechamentoeFinanceiraInfoFechamento();
    }

    /// <summary>
    /// Define a data de início do período de fechamento
    /// </summary>
    /// <param name="dataInicio">Data de início</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoFechamentoBuilder WithDataInicio(DateTime dataInicio)
    {
        _infoFechamento.dtInicio = dataInicio;
        return this;
    }

    /// <summary>
    /// Define a data de fim do período de fechamento
    /// </summary>
    /// <param name="dataFim">Data de fim</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoFechamentoBuilder WithDataFim(DateTime dataFim)
    {
        _infoFechamento.dtFim = dataFim;
        return this;
    }

    /// <summary>
    /// Define a situação especial
    /// </summary>
    /// <param name="situacao">1-Situação normal, 2-Evento de fechamento</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoFechamentoBuilder WithSituacaoEspecial(uint situacao)
    {
        _infoFechamento.sitEspecial = situacao;
        return this;
    }

    /// <summary>
    /// Constrói a instância final das informações de fechamento
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtFechamentoeFinanceiraInfoFechamento</returns>
    internal eFinanceiraEvtFechamentoeFinanceiraInfoFechamento Build() => _infoFechamento;
}

/// <summary>
/// Builder para configuração dos fechamentos PP (Pessoa Política)
/// </summary>
public sealed class FechamentoPPBuilder
{
    private readonly List<eFinanceiraEvtFechamentoeFinanceiraFechamentoMes> _fechamentos;

    /// <summary>
    /// Initializes a new instance of the <see cref="FechamentoPPBuilder"/> class.
    /// </summary>
    public FechamentoPPBuilder()
    {
        _fechamentos = new List<eFinanceiraEvtFechamentoeFinanceiraFechamentoMes>();
    }

    /// <summary>
    /// Adiciona um fechamento mensal PP
    /// </summary>
    /// <param name="anoMesCaixa">Ano e mês da caixa no formato AAAAMM</param>
    /// <param name="quantidadeArquivos">Quantidade de arquivos transmitidos</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public FechamentoPPBuilder AdicionarFechamentoMes(string anoMesCaixa, ulong quantidadeArquivos)
    {
        _fechamentos.Add(new eFinanceiraEvtFechamentoeFinanceiraFechamentoMes
        {
            anoMesCaixa = anoMesCaixa,
            quantArqTrans = quantidadeArquivos
        });
        return this;
    }

    /// <summary>
    /// Constrói o array final de fechamentos PP
    /// </summary>
    /// <returns>Array configurado de eFinanceiraEvtFechamentoeFinanceiraFechamentoMes</returns>
    internal eFinanceiraEvtFechamentoeFinanceiraFechamentoMes[] Build() => _fechamentos.ToArray();
}

/// <summary>
/// Builder para configuração do fechamento de movimento de operação financeira mensal
/// </summary>
public sealed class FechamentoMovOpFinBuilder
{
    private readonly eFinanceiraEvtFechamentoeFinanceiraFechamentoMovOpFin _fechamento;

    /// <summary>
    /// Initializes a new instance of the <see cref="FechamentoMovOpFinBuilder"/> class.
    /// </summary>
    public FechamentoMovOpFinBuilder()
    {
        _fechamento = new eFinanceiraEvtFechamentoeFinanceiraFechamentoMovOpFin();
    }

    /// <summary>
    /// Adiciona um fechamento mensal
    /// </summary>
    /// <param name="configAction">Ação para configurar o fechamento mensal</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public FechamentoMovOpFinBuilder AdicionarFechamentoMes(Action<FechamentoMovOpFinMensalBuilder> configAction)
    {
        var builder = new FechamentoMovOpFinMensalBuilder();
        configAction(builder);
        
        var fechamentos = _fechamento.FechamentoMes?.ToList() ?? new List<eFinanceiraEvtFechamentoeFinanceiraFechamentoMovOpFinFechamentoMes>();
        fechamentos.Add(builder.Build());
        _fechamento.FechamentoMes = fechamentos.ToArray();
        
        return this;
    }

    /// <summary>
    /// Constrói a instância final do fechamento MovOpFin
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtFechamentoeFinanceiraFechamentoMovOpFin</returns>
    internal eFinanceiraEvtFechamentoeFinanceiraFechamentoMovOpFin Build() => _fechamento;
}

/// <summary>
/// Builder para configuração de fechamento mensal MovOpFin
/// </summary>
public sealed class FechamentoMovOpFinMensalBuilder
{
    private readonly eFinanceiraEvtFechamentoeFinanceiraFechamentoMovOpFinFechamentoMes _fechamentoMes;

    /// <summary>
    /// Initializes a new instance of the <see cref="FechamentoMovOpFinMensalBuilder"/> class.
    /// </summary>
    public FechamentoMovOpFinMensalBuilder()
    {
        _fechamentoMes = new eFinanceiraEvtFechamentoeFinanceiraFechamentoMovOpFinFechamentoMes();
    }

    /// <summary>
    /// Define o ano e mês da caixa
    /// </summary>
    /// <param name="anoMesCaixa">Ano e mês no formato AAAAMM</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public FechamentoMovOpFinMensalBuilder WithAnoMesCaixa(string anoMesCaixa)
    {
        _fechamentoMes.anoMesCaixa = anoMesCaixa;
        return this;
    }

    /// <summary>
    /// Define a quantidade de arquivos transmitidos
    /// </summary>
    /// <param name="quantidade">Quantidade de arquivos</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public FechamentoMovOpFinMensalBuilder WithQuantidadeArquivos(ulong quantidade)
    {
        _fechamentoMes.quantArqTrans = quantidade;
        return this;
    }

    /// <summary>
    /// Constrói a instância final do fechamento mensal
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtFechamentoeFinanceiraFechamentoMovOpFinFechamentoMes</returns>
    internal eFinanceiraEvtFechamentoeFinanceiraFechamentoMovOpFinFechamentoMes Build() => _fechamentoMes;
}

/// <summary>
/// Builder para configuração do fechamento de movimento de operação financeira anual
/// </summary>
public sealed class FechamentoMovOpFinAnualBuilder
{
    private readonly eFinanceiraEvtFechamentoeFinanceiraFechamentoMovOpFinAnual _fechamentoAnual;

    /// <summary>
    /// Initializes a new instance of the <see cref="FechamentoMovOpFinAnualBuilder"/> class.
    /// </summary>
    public FechamentoMovOpFinAnualBuilder()
    {
        _fechamentoAnual = new eFinanceiraEvtFechamentoeFinanceiraFechamentoMovOpFinAnual();
    }

    /// <summary>
    /// Configura o fechamento anual
    /// </summary>
    /// <param name="configAction">Ação para configurar o fechamento anual</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public FechamentoMovOpFinAnualBuilder WithFechamentoAno(Action<FechamentoMovOpFinAnualItemBuilder> configAction)
    {
        var builder = new FechamentoMovOpFinAnualItemBuilder();
        configAction(builder);
        _fechamentoAnual.FechamentoAno = builder.Build();
        return this;
    }

    /// <summary>
    /// Constrói a instância final do fechamento anual
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtFechamentoeFinanceiraFechamentoMovOpFinAnual</returns>
    internal eFinanceiraEvtFechamentoeFinanceiraFechamentoMovOpFinAnual Build() => _fechamentoAnual;
}

/// <summary>
/// Builder para configuração de item de fechamento anual MovOpFin
/// </summary>
public sealed class FechamentoMovOpFinAnualItemBuilder
{
    private readonly eFinanceiraEvtFechamentoeFinanceiraFechamentoMovOpFinAnualFechamentoAno _fechamentoAno;

    /// <summary>
    /// Initializes a new instance of the <see cref="FechamentoMovOpFinAnualItemBuilder"/> class.
    /// </summary>
    public FechamentoMovOpFinAnualItemBuilder()
    {
        _fechamentoAno = new eFinanceiraEvtFechamentoeFinanceiraFechamentoMovOpFinAnualFechamentoAno();
    }

    /// <summary>
    /// Define o ano do fechamento
    /// </summary>
    /// <param name="anoCaixa">Ano no formato AAAA</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public FechamentoMovOpFinAnualItemBuilder WithAnoCaixa(string anoCaixa)
    {
        _fechamentoAno.anoCaixa = anoCaixa;
        return this;
    }

    /// <summary>
    /// Define a quantidade de arquivos transmitidos
    /// </summary>
    /// <param name="quantidade">Quantidade de arquivos</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public FechamentoMovOpFinAnualItemBuilder WithQuantidadeArquivos(ulong quantidade)
    {
        _fechamentoAno.quantArqTrans = quantidade;
        return this;
    }

    /// <summary>
    /// Constrói a instância final do fechamento anual
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtFechamentoeFinanceiraFechamentoMovOpFinAnualFechamentoAno</returns>
    internal eFinanceiraEvtFechamentoeFinanceiraFechamentoMovOpFinAnualFechamentoAno Build() => _fechamentoAno;
}

#endregion

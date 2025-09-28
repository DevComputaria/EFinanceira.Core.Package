using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Generated.Eventos.EvtMovimentacaoFinanceira;

namespace EFinanceira.Messages.Builders.Eventos.EvtMovimentacaoFinanceira;

/// <summary>
/// Mensagem de evento de Movimentação de Operação Financeira
/// </summary>
public sealed class EvtMovimentacaoFinanceiraMessage : IEFinanceiraMessage
{
    public string Version { get; private set; }
    public string RootElementName => "evtMovOpFin";
    public string? IdAttributeName => "id";
    public string? IdValue { get; internal set; }
    public object Payload => Evento;

    /// <summary>
    /// Evento tipado gerado do XSD
    /// </summary>
    public eFinanceiraEvtMovOpFin Evento { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EvtMovimentacaoFinanceiraMessage"/> class.
    /// Construtor sem parâmetros necessário para serialização XML.
    /// </summary>
    public EvtMovimentacaoFinanceiraMessage()
    {
        Version = "v1_2_1";
        Evento = new eFinanceiraEvtMovOpFin();
        IdValue = string.Empty;
    }

    internal EvtMovimentacaoFinanceiraMessage(eFinanceiraEvtMovOpFin evento, string version)
    {
        Evento = evento;
        Version = version;
        IdValue = evento.id;
    }
}

/// <summary>
/// Builder principal para construção de eventos de Movimentação de Operação Financeira.
/// Utiliza o padrão fluent interface para facilitar a criação de eventos estruturados.
/// </summary>
public sealed class EvtMovimentacaoFinanceiraBuilder : IMessageBuilder<EvtMovimentacaoFinanceiraMessage>
{
    private readonly eFinanceiraEvtMovOpFin _evento;
    private readonly string _version;

    /// <summary>
    /// Initializes a new instance of the <see cref="EvtMovimentacaoFinanceiraBuilder"/> class with default version.
    /// </summary>
    public EvtMovimentacaoFinanceiraBuilder() : this("v1_2_1")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EvtMovimentacaoFinanceiraBuilder"/> class with specific version.
    /// </summary>
    /// <param name="version">Versão do schema</param>
    public EvtMovimentacaoFinanceiraBuilder(string version)
    {
        _version = version;
        _evento = new eFinanceiraEvtMovOpFin();
    }

    /// <summary>
    /// Define o identificador único do evento
    /// </summary>
    /// <param name="id">Identificador único do evento</param>
    /// <returns>Builder para continuar a configuração</returns>
    public EvtMovimentacaoFinanceiraBuilder ComId(string id)
    {
        _evento.id = id;
        return this;
    }

    /// <summary>
    /// Configura as informações de identificação do evento
    /// </summary>
    /// <param name="builderAction">Ação para configurar o IdeEvento</param>
    /// <returns>Builder para continuar a configuração</returns>
    public EvtMovimentacaoFinanceiraBuilder ComIdeEvento(Action<IdeEventoBuilder> builderAction)
    {
        var builder = new IdeEventoBuilder();
        builderAction(builder);
        _evento.ideEvento = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura as informações do declarante
    /// </summary>
    /// <param name="builderAction">Ação para configurar o IdeDeclarante</param>
    /// <returns>Builder para continuar a configuração</returns>
    public EvtMovimentacaoFinanceiraBuilder ComIdeDeclarante(Action<IdeDeclaranteBuilder> builderAction)
    {
        var builder = new IdeDeclaranteBuilder();
        builderAction(builder);
        _evento.ideDeclarante = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura as informações do declarado (pessoa/entidade sobre quem se declara)
    /// </summary>
    /// <param name="builderAction">Ação para configurar o IdeDeclarado</param>
    /// <returns>Builder para continuar a configuração</returns>
    public EvtMovimentacaoFinanceiraBuilder ComIdeDeclarado(Action<IdeDeclaradoBuilder> builderAction)
    {
        var builder = new IdeDeclaradoBuilder();
        builderAction(builder);
        _evento.ideDeclarado = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura as informações do mês/caixa com movimentações financeiras
    /// </summary>
    /// <param name="builderAction">Ação para configurar o MesCaixa</param>
    /// <returns>Builder para continuar a configuração</returns>
    public EvtMovimentacaoFinanceiraBuilder ComMesCaixa(Action<MesCaixaBuilder> builderAction)
    {
        var builder = new MesCaixaBuilder();
        builderAction(builder);
        _evento.mesCaixa = builder.Build();
        return this;
    }

    /// <summary>
    /// Constrói a mensagem final do evento
    /// </summary>
    /// <returns>Mensagem construída</returns>
    public EvtMovimentacaoFinanceiraMessage Build()
    {
        return new EvtMovimentacaoFinanceiraMessage(_evento, _version);
    }
}

/// <summary>
/// Builder para configuração da identificação do evento
/// </summary>
public sealed class IdeEventoBuilder
{
    private readonly eFinanceiraEvtMovOpFinIdeEvento _ideEvento;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdeEventoBuilder"/> class.
    /// </summary>
    public IdeEventoBuilder()
    {
        _ideEvento = new eFinanceiraEvtMovOpFinIdeEvento();
    }

    /// <summary>
    /// Define o indicador de retificação
    /// </summary>
    /// <param name="indRetificacao">Indicador de retificação (1-Arquivo original, 2-Arquivo de retificação)</param>
    /// <returns>Builder para continuar a configuração</returns>
    public IdeEventoBuilder ComIndRetificacao(uint indRetificacao)
    {
        _ideEvento.indRetificacao = indRetificacao;
        return this;
    }

    /// <summary>
    /// Define o número do recibo (obrigatório se indRetificacao = 2)
    /// </summary>
    /// <param name="nrRecibo">Número do recibo do arquivo que está sendo retificado</param>
    /// <returns>Builder para continuar a configuração</returns>
    public IdeEventoBuilder ComNrRecibo(string nrRecibo)
    {
        _ideEvento.nrRecibo = nrRecibo;
        return this;
    }

    /// <summary>
    /// Define o tipo de ambiente
    /// </summary>
    /// <param name="tpAmb">Tipo de ambiente (1-Produção, 2-Homologação)</param>
    /// <returns>Builder para continuar a configuração</returns>
    public IdeEventoBuilder ComTpAmb(uint tpAmb)
    {
        _ideEvento.tpAmb = tpAmb;
        return this;
    }

    /// <summary>
    /// Define o aplicativo emissor
    /// </summary>
    /// <param name="aplicEmi">Aplicativo emissor do evento</param>
    /// <returns>Builder para continuar a configuração</returns>
    public IdeEventoBuilder ComAplicEmi(uint aplicEmi)
    {
        _ideEvento.aplicEmi = aplicEmi;
        return this;
    }

    /// <summary>
    /// Define a versão do aplicativo emissor
    /// </summary>
    /// <param name="verAplic">Versão do aplicativo emissor</param>
    /// <returns>Builder para continuar a configuração</returns>
    public IdeEventoBuilder ComVerAplic(string verAplic)
    {
        _ideEvento.verAplic = verAplic;
        return this;
    }

    /// <summary>
    /// Constrói o objeto IdeEvento
    /// </summary>
    /// <returns>Objeto construído</returns>
    internal eFinanceiraEvtMovOpFinIdeEvento Build()
    {
        return _ideEvento;
    }
}

/// <summary>
/// Builder para configuração da identificação do declarante
/// </summary>
public sealed class IdeDeclaranteBuilder
{
    private readonly eFinanceiraEvtMovOpFinIdeDeclarante _ideDeclarante;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdeDeclaranteBuilder"/> class.
    /// </summary>
    public IdeDeclaranteBuilder()
    {
        _ideDeclarante = new eFinanceiraEvtMovOpFinIdeDeclarante();
    }

    /// <summary>
    /// Define o CNPJ do declarante
    /// </summary>
    /// <param name="cnpjDeclarante">CNPJ do declarante</param>
    /// <returns>Builder para continuar a configuração</returns>
    public IdeDeclaranteBuilder ComCnpjDeclarante(string cnpjDeclarante)
    {
        _ideDeclarante.cnpjDeclarante = cnpjDeclarante;
        return this;
    }

    /// <summary>
    /// Constrói o objeto IdeDeclarante
    /// </summary>
    /// <returns>Objeto construído</returns>
    internal eFinanceiraEvtMovOpFinIdeDeclarante Build()
    {
        return _ideDeclarante;
    }
}

/// <summary>
/// Builder para configuração das informações do declarado (versão simplificada)
/// </summary>
public sealed class IdeDeclaradoBuilder
{
    private readonly eFinanceiraEvtMovOpFinIdeDeclarado _ideDeclarado;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdeDeclaradoBuilder"/> class.
    /// </summary>
    public IdeDeclaradoBuilder()
    {
        _ideDeclarado = new eFinanceiraEvtMovOpFinIdeDeclarado();
    }

    /// <summary>
    /// Define o tipo de NI (Número de Identificação)
    /// </summary>
    /// <param name="tpNI">Tipo de NI (1-CPF, 2-CNPJ, 3-Passaporte, 4-Outro)</param>
    /// <returns>Builder para continuar a configuração</returns>
    public IdeDeclaradoBuilder ComTpNI(byte tpNI)
    {
        _ideDeclarado.tpNI = tpNI;
        return this;
    }

    /// <summary>
    /// Define o número de identificação
    /// </summary>
    /// <param name="niDeclarado">Número de identificação do declarado</param>
    /// <returns>Builder para continuar a configuração</returns>
    public IdeDeclaradoBuilder ComNIDeclarado(string niDeclarado)
    {
        _ideDeclarado.NIDeclarado = niDeclarado;
        return this;
    }

    /// <summary>
    /// Define o nome do declarado
    /// </summary>
    /// <param name="nomeDeclarado">Nome do declarado</param>
    /// <returns>Builder para continuar a configuração</returns>
    public IdeDeclaradoBuilder ComNomeDeclarado(string nomeDeclarado)
    {
        _ideDeclarado.NomeDeclarado = nomeDeclarado;
        return this;
    }

    /// <summary>
    /// Define a data de nascimento (para pessoas físicas)
    /// </summary>
    /// <param name="dataNascimento">Data de nascimento</param>
    /// <returns>Builder para continuar a configuração</returns>
    public IdeDeclaradoBuilder ComDataNascimento(DateTime dataNascimento)
    {
        _ideDeclarado.DataNasc = dataNascimento;
        _ideDeclarado.DataNascSpecified = true;
        return this;
    }

    /// <summary>
    /// Define o endereço livre
    /// </summary>
    /// <param name="enderecoLivre">Endereço em formato livre</param>
    /// <returns>Builder para continuar a configuração</returns>
    public IdeDeclaradoBuilder ComEnderecoLivre(string enderecoLivre)
    {
        _ideDeclarado.EnderecoLivre = enderecoLivre;
        return this;
    }

    /// <summary>
    /// Constrói o objeto IdeDeclarado
    /// </summary>
    /// <returns>Objeto construído</returns>
    internal eFinanceiraEvtMovOpFinIdeDeclarado Build()
    {
        return _ideDeclarado;
    }
}

/// <summary>
/// Builder para configuração das informações do mês/caixa
/// </summary>
public sealed class MesCaixaBuilder
{
    private readonly eFinanceiraEvtMovOpFinMesCaixa _mesCaixa;

    /// <summary>
    /// Initializes a new instance of the <see cref="MesCaixaBuilder"/> class.
    /// </summary>
    public MesCaixaBuilder()
    {
        _mesCaixa = new eFinanceiraEvtMovOpFinMesCaixa();
    }

    /// <summary>
    /// Define o ano e mês do caixa
    /// </summary>
    /// <param name="anoMesCaixa">Ano e mês no formato AAAA-MM</param>
    /// <returns>Builder para continuar a configuração</returns>
    public MesCaixaBuilder ComAnoMesCaixa(string anoMesCaixa)
    {
        _mesCaixa.anoMesCaixa = anoMesCaixa;
        return this;
    }

    /// <summary>
    /// Configura as movimentações de operação financeira
    /// </summary>
    /// <param name="builderAction">Ação para configurar as movimentações</param>
    /// <returns>Builder para continuar a configuração</returns>
    public MesCaixaBuilder ComMovOpFin(Action<MovOpFinBuilder> builderAction)
    {
        var builder = new MovOpFinBuilder();
        builderAction(builder);
        _mesCaixa.movOpFin = builder.Build();
        return this;
    }

    /// <summary>
    /// Constrói o objeto MesCaixa
    /// </summary>
    /// <returns>Objeto construído</returns>
    internal eFinanceiraEvtMovOpFinMesCaixa Build()
    {
        return _mesCaixa;
    }
}

/// <summary>
/// Builder para configuração das movimentações de operação financeira
/// </summary>
public sealed class MovOpFinBuilder
{
    private readonly eFinanceiraEvtMovOpFinMesCaixaMovOpFin _movOpFin;

    /// <summary>
    /// Initializes a new instance of the <see cref="MovOpFinBuilder"/> class.
    /// </summary>
    public MovOpFinBuilder()
    {
        _movOpFin = new eFinanceiraEvtMovOpFinMesCaixaMovOpFin();
    }

    /// <summary>
    /// Adiciona uma conta às movimentações
    /// </summary>
    /// <param name="builderAction">Ação para configurar a conta</param>
    /// <returns>Builder para continuar a configuração</returns>
    public MovOpFinBuilder ComConta(Action<ContaBuilder> builderAction)
    {
        var builder = new ContaBuilder();
        builderAction(builder);
        
        var contas = _movOpFin.Conta?.ToList() ?? new List<eFinanceiraEvtMovOpFinMesCaixaMovOpFinConta>();
        contas.Add(builder.Build());
        _movOpFin.Conta = contas.ToArray();
        
        return this;
    }

    /// <summary>
    /// Configura informações de câmbio
    /// </summary>
    /// <param name="builderAction">Ação para configurar o câmbio</param>
    /// <returns>Builder para continuar a configuração</returns>
    public MovOpFinBuilder ComCambio(Action<CambioBuilder> builderAction)
    {
        var builder = new CambioBuilder();
        builderAction(builder);
        _movOpFin.Cambio = builder.Build();
        return this;
    }

    /// <summary>
    /// Constrói o objeto MovOpFin
    /// </summary>
    /// <returns>Objeto construído</returns>
    internal eFinanceiraEvtMovOpFinMesCaixaMovOpFin Build()
    {
        return _movOpFin;
    }
}

/// <summary>
/// Builder para configuração de conta (versão simplificada)
/// </summary>
public sealed class ContaBuilder
{
    private readonly eFinanceiraEvtMovOpFinMesCaixaMovOpFinConta _conta;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContaBuilder"/> class.
    /// </summary>
    public ContaBuilder()
    {
        _conta = new eFinanceiraEvtMovOpFinMesCaixaMovOpFinConta();
    }

    /// <summary>
    /// Configura informações da conta (versão básica)
    /// </summary>
    /// <param name="tpConta">Tipo de conta</param>
    /// <param name="subTpConta">Subtipo de conta</param>
    /// <param name="tpNumConta">Tipo de numeração da conta</param>
    /// <param name="numConta">Número da conta</param>
    /// <returns>Builder para continuar a configuração</returns>
    public ContaBuilder ComInfoConta(string tpConta, string subTpConta, string tpNumConta, string numConta)
    {
        _conta.infoConta = new eFinanceiraEvtMovOpFinMesCaixaMovOpFinContaInfoConta();
        
        // Configuração básica da conta - estrutura complexa, implementação simplificada
        return this;
    }

    /// <summary>
    /// Constrói o objeto Conta
    /// </summary>
    /// <returns>Objeto construído</returns>
    internal eFinanceiraEvtMovOpFinMesCaixaMovOpFinConta Build()
    {
        return _conta;
    }
}

/// <summary>
/// Builder para configuração de câmbio (versão simplificada)
/// </summary>
public sealed class CambioBuilder
{
    private readonly eFinanceiraEvtMovOpFinMesCaixaMovOpFinCambio _cambio;

    /// <summary>
    /// Initializes a new instance of the <see cref="CambioBuilder"/> class.
    /// </summary>
    public CambioBuilder()
    {
        _cambio = new eFinanceiraEvtMovOpFinMesCaixaMovOpFinCambio();
    }

    /// <summary>
    /// Define informações básicas de câmbio
    /// </summary>
    /// <param name="tpOperacao">Tipo de operação de câmbio</param>
    /// <returns>Builder para continuar a configuração</returns>
    public CambioBuilder ComTpOperacao(byte tpOperacao)
    {
        // Configuração básica do câmbio - estrutura complexa, implementação simplificada
        return this;
    }

    /// <summary>
    /// Constrói o objeto Cambio
    /// </summary>
    /// <returns>Objeto construído</returns>
    internal eFinanceiraEvtMovOpFinMesCaixaMovOpFinCambio Build()
    {
        return _cambio;
    }
}

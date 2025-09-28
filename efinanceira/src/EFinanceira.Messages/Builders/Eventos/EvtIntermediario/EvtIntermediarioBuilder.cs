using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Generated.Eventos.EvtIntermediario;

namespace EFinanceira.Messages.Builders.Eventos.EvtIntermediario;

/// <summary>
/// Mensagem de evento de Cadastro de Intermediário
/// </summary>
public sealed class EvtIntermediarioMessage : IEFinanceiraMessage
{
    public string Version { get; private set; }
    public string RootElementName => "evtCadIntermediario";
    public string? IdAttributeName => "id";
    public string? IdValue { get; internal set; }
    public object Payload => Evento;

    /// <summary>
    /// Evento tipado gerado do XSD
    /// </summary>
    public eFinanceiraEvtCadIntermediario Evento { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EvtIntermediarioMessage"/> class.
    /// Construtor sem parâmetros necessário para serialização XML.
    /// </summary>
    public EvtIntermediarioMessage()
    {
        Version = "v1_2_0";
        Evento = new eFinanceiraEvtCadIntermediario();
        IdValue = string.Empty;
    }

    internal EvtIntermediarioMessage(eFinanceiraEvtCadIntermediario evento, string version)
    {
        Evento = evento;
        Version = version;
        IdValue = evento.id;
    }
}

/// <summary>
/// Builder principal para construção de eventos de Cadastro de Intermediário.
/// Utiliza o padrão fluent interface para facilitar a criação de eventos estruturados.
/// </summary>
public sealed class EvtIntermediarioBuilder : IMessageBuilder<EvtIntermediarioMessage>
{
    private readonly eFinanceiraEvtCadIntermediario _evento;
    private readonly string _version;

    /// <summary>
    /// Initializes a new instance of the <see cref="EvtIntermediarioBuilder"/> class with default version.
    /// </summary>
    public EvtIntermediarioBuilder() : this("v1_2_0")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EvtIntermediarioBuilder"/> class with specific version.
    /// </summary>
    /// <param name="version">Versão do schema</param>
    public EvtIntermediarioBuilder(string version)
    {
        _version = version;
        _evento = new eFinanceiraEvtCadIntermediario();
    }

    /// <summary>
    /// Define o identificador único do evento
    /// </summary>
    /// <param name="id">Identificador único do evento</param>
    /// <returns>Builder para continuar a configuração</returns>
    public EvtIntermediarioBuilder ComId(string id)
    {
        _evento.id = id;
        return this;
    }

    /// <summary>
    /// Configura as informações de identificação do evento
    /// </summary>
    /// <param name="builderAction">Ação para configurar o IdeEvento</param>
    /// <returns>Builder para continuar a configuração</returns>
    public EvtIntermediarioBuilder ComIdeEvento(Action<IdeEventoBuilder> builderAction)
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
    public EvtIntermediarioBuilder ComIdeDeclarante(Action<IdeDeclaranteBuilder> builderAction)
    {
        var builder = new IdeDeclaranteBuilder();
        builderAction(builder);
        _evento.ideDeclarante = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura as informações do intermediário
    /// </summary>
    /// <param name="builderAction">Ação para configurar o InfoIntermediario</param>
    /// <returns>Builder para continuar a configuração</returns>
    public EvtIntermediarioBuilder ComInfoIntermediario(Action<InfoIntermediarioBuilder> builderAction)
    {
        var builder = new InfoIntermediarioBuilder();
        builderAction(builder);
        _evento.infoIntermediario = builder.Build();
        return this;
    }

    /// <summary>
    /// Constrói a mensagem final do evento
    /// </summary>
    /// <returns>Mensagem construída</returns>
    public EvtIntermediarioMessage Build()
    {
        return new EvtIntermediarioMessage(_evento, _version);
    }
}

/// <summary>
/// Builder para configuração da identificação do evento
/// </summary>
public sealed class IdeEventoBuilder
{
    private readonly eFinanceiraEvtCadIntermediarioIdeEvento _ideEvento;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdeEventoBuilder"/> class.
    /// </summary>
    public IdeEventoBuilder()
    {
        _ideEvento = new eFinanceiraEvtCadIntermediarioIdeEvento();
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
    internal eFinanceiraEvtCadIntermediarioIdeEvento Build()
    {
        return _ideEvento;
    }
}

/// <summary>
/// Builder para configuração da identificação do declarante
/// </summary>
public sealed class IdeDeclaranteBuilder
{
    private readonly eFinanceiraEvtCadIntermediarioIdeDeclarante _ideDeclarante;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdeDeclaranteBuilder"/> class.
    /// </summary>
    public IdeDeclaranteBuilder()
    {
        _ideDeclarante = new eFinanceiraEvtCadIntermediarioIdeDeclarante();
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
    internal eFinanceiraEvtCadIntermediarioIdeDeclarante Build()
    {
        return _ideDeclarante;
    }
}

/// <summary>
/// Builder para configuração das informações do intermediário
/// </summary>
public sealed class InfoIntermediarioBuilder
{
    private readonly eFinanceiraEvtCadIntermediarioInfoIntermediario _infoIntermediario;

    /// <summary>
    /// Initializes a new instance of the <see cref="InfoIntermediarioBuilder"/> class.
    /// </summary>
    public InfoIntermediarioBuilder()
    {
        _infoIntermediario = new eFinanceiraEvtCadIntermediarioInfoIntermediario();
    }

    /// <summary>
    /// Define o Global Intermediary Identification Number (GIIN)
    /// </summary>
    /// <param name="giin">GIIN do intermediário</param>
    /// <returns>Builder para continuar a configuração</returns>
    public InfoIntermediarioBuilder ComGIIN(string giin)
    {
        _infoIntermediario.GIIN = giin;
        return this;
    }

    /// <summary>
    /// Define o tipo de Número de Identificação
    /// </summary>
    /// <param name="tpNI">Tipo de NI (1-CPF, 2-CNPJ, 3-Passaporte, 4-Outro)</param>
    /// <returns>Builder para continuar a configuração</returns>
    public InfoIntermediarioBuilder ComTpNI(byte tpNI)
    {
        _infoIntermediario.tpNI = tpNI;
        _infoIntermediario.tpNISpecified = true;
        return this;
    }

    /// <summary>
    /// Define o número de identificação do intermediário
    /// </summary>
    /// <param name="niIntermediario">Número de identificação do intermediário</param>
    /// <returns>Builder para continuar a configuração</returns>
    public InfoIntermediarioBuilder ComNIIntermediario(string niIntermediario)
    {
        _infoIntermediario.NIIntermediario = niIntermediario;
        return this;
    }

    /// <summary>
    /// Define o nome do intermediário
    /// </summary>
    /// <param name="nomeIntermediario">Nome do intermediário</param>
    /// <returns>Builder para continuar a configuração</returns>
    public InfoIntermediarioBuilder ComNomeIntermediario(string nomeIntermediario)
    {
        _infoIntermediario.nomeIntermediario = nomeIntermediario;
        return this;
    }

    /// <summary>
    /// Configura o endereço do intermediário
    /// </summary>
    /// <param name="builderAction">Ação para configurar o endereço</param>
    /// <returns>Builder para continuar a configuração</returns>
    public InfoIntermediarioBuilder ComEndereco(Action<EnderecoBuilder> builderAction)
    {
        var builder = new EnderecoBuilder();
        builderAction(builder);
        _infoIntermediario.endereco = builder.Build();
        return this;
    }

    /// <summary>
    /// Define o país de residência
    /// </summary>
    /// <param name="paisResidencia">País de residência (código ISO 3166-1 alpha-2)</param>
    /// <returns>Builder para continuar a configuração</returns>
    public InfoIntermediarioBuilder ComPaisResidencia(string paisResidencia)
    {
        _infoIntermediario.paisResidencia = paisResidencia;
        return this;
    }

    /// <summary>
    /// Constrói o objeto InfoIntermediario
    /// </summary>
    /// <returns>Objeto construído</returns>
    internal eFinanceiraEvtCadIntermediarioInfoIntermediario Build()
    {
        return _infoIntermediario;
    }
}

/// <summary>
/// Builder para configuração do endereço do intermediário
/// </summary>
public sealed class EnderecoBuilder
{
    private readonly eFinanceiraEvtCadIntermediarioInfoIntermediarioEndereco _endereco;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnderecoBuilder"/> class.
    /// </summary>
    public EnderecoBuilder()
    {
        _endereco = new eFinanceiraEvtCadIntermediarioInfoIntermediarioEndereco();
    }

    /// <summary>
    /// Define o endereço livre
    /// </summary>
    /// <param name="enderecoLivre">Endereço em formato livre</param>
    /// <returns>Builder para continuar a configuração</returns>
    public EnderecoBuilder ComEnderecoLivre(string enderecoLivre)
    {
        _endereco.enderecoLivre = enderecoLivre;
        return this;
    }

    /// <summary>
    /// Define o município
    /// </summary>
    /// <param name="municipio">Município</param>
    /// <returns>Builder para continuar a configuração</returns>
    public EnderecoBuilder ComMunicipio(string municipio)
    {
        _endereco.municipio = municipio;
        return this;
    }

    /// <summary>
    /// Define o país
    /// </summary>
    /// <param name="pais">País (código ISO 3166-1 alpha-2)</param>
    /// <returns>Builder para continuar a configuração</returns>
    public EnderecoBuilder ComPais(string pais)
    {
        _endereco.pais = pais;
        return this;
    }

    /// <summary>
    /// Constrói o objeto Endereco
    /// </summary>
    /// <returns>Objeto construído</returns>
    internal eFinanceiraEvtCadIntermediarioInfoIntermediarioEndereco Build()
    {
        return _endereco;
    }
}

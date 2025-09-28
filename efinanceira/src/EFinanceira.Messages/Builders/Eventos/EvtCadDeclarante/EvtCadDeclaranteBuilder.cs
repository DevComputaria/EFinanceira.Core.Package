using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Generated.Eventos.EvtCadEmpresaDeclarante;

namespace EFinanceira.Messages.Builders.Eventos.EvtCadDeclarante;

/// <summary>
/// Mensagem de evento de Cadastro de Declarante
/// </summary>
public sealed class EvtCadDeclaranteMessage : IEFinanceiraMessage
{
    public string Version { get; private set; }
    public string RootElementName => "evtCadDeclarante";
    public string? IdAttributeName => "id";
    public string? IdValue { get; internal set; }
    public object Payload => Evento;

    /// <summary>
    /// Evento tipado gerado do XSD
    /// </summary>
    public eFinanceiraEvtCadDeclarante Evento { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EvtCadDeclaranteMessage"/> class.
    /// Construtor sem parâmetros necessário para serialização XML.
    /// </summary>
    public EvtCadDeclaranteMessage()
    {
        Version = "v1_2_0";
        Evento = new eFinanceiraEvtCadDeclarante();
        IdValue = string.Empty;
    }

    internal EvtCadDeclaranteMessage(eFinanceiraEvtCadDeclarante evento, string version)
    {
        Evento = evento;
        Version = version;
        IdValue = evento.id;
    }
}

/// <summary>
/// Builder principal para construção de eventos de Cadastro de Declarante.
/// Utiliza o padrão fluent interface para facilitar a criação de eventos estruturados.
/// </summary>
public sealed class EvtCadDeclaranteBuilder : IMessageBuilder<EvtCadDeclaranteMessage>
{
    private readonly eFinanceiraEvtCadDeclarante _evento;
    private readonly string _version;

    /// <summary>
    /// Initializes a new instance of the <see cref="EvtCadDeclaranteBuilder"/> class.
    /// </summary>
    /// <param name="version">Versão do schema (padrão: v1_2_0)</param>
    public EvtCadDeclaranteBuilder(string version = "v1_2_0")
    {
        _version = version;
        _evento = new eFinanceiraEvtCadDeclarante
        {
            id = GenerateId(),
            ideEvento = new eFinanceiraEvtCadDeclaranteIdeEvento
            {
                indRetificacao = 1, // Original
                tpAmb = 2, // Homologação
                aplicEmi = 1, // Aplicativo do contribuinte
                verAplic = "1.0.0"
            }
        };
    }

    /// <summary>
    /// Cria uma nova instância do builder EvtCadDeclarante
    /// </summary>
    /// <param name="version">Versão do schema</param>
    /// <returns>Nova instância do builder</returns>
    public static EvtCadDeclaranteBuilder Create(string version = "v1_2_0") => new(version);

    /// <summary>
    /// Define o ID do evento
    /// </summary>
    /// <param name="id">ID único do evento</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtCadDeclaranteBuilder WithId(string id)
    {
        _evento.id = id;
        return this;
    }

    /// <summary>
    /// Configura os dados da identificação do evento
    /// </summary>
    /// <param name="configAction">Ação para configurar o IdeEvento</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtCadDeclaranteBuilder WithIdeEvento(Action<IdeEventoBuilder> configAction)
    {
        var builder = new IdeEventoBuilder(_evento.ideEvento);
        configAction(builder);
        return this;
    }

    /// <summary>
    /// Configura os dados da identificação do declarante
    /// </summary>
    /// <param name="configAction">Ação para configurar o IdeDeclarante</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtCadDeclaranteBuilder WithIdeDeclarante(Action<IdeDeclaranteBuilder> configAction)
    {
        var builder = new IdeDeclaranteBuilder();
        configAction(builder);
        _evento.ideDeclarante = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura as informações de cadastro
    /// </summary>
    /// <param name="configAction">Ação para configurar o InfoCadastro</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtCadDeclaranteBuilder WithInfoCadastro(Action<InfoCadastroBuilder> configAction)
    {
        var builder = new InfoCadastroBuilder();
        configAction(builder);
        _evento.infoCadastro = builder.Build();
        return this;
    }

    /// <summary>
    /// Constrói a instância final da mensagem de evento
    /// </summary>
    /// <returns>Instância configurada de EvtCadDeclaranteMessage</returns>
    public EvtCadDeclaranteMessage Build()
    {
        ValidateRequiredFields();
        return new EvtCadDeclaranteMessage(_evento, _version);
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
        
        if (_evento.infoCadastro == null)
            throw new InvalidOperationException("InfoCadastro é obrigatório");
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
    private readonly eFinanceiraEvtCadDeclaranteIdeEvento _ideEvento;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdeEventoBuilder"/> class.
    /// </summary>
    internal IdeEventoBuilder(eFinanceiraEvtCadDeclaranteIdeEvento ideEvento)
    {
        _ideEvento = ideEvento;
    }

    /// <summary>
    /// Define o indicador de retificação
    /// </summary>
    /// <param name="indRetificacao">Indicador de retificação (1-Original, 2-Retificação)</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeEventoBuilder WithIndRetificacao(uint indRetificacao)
    {
        _ideEvento.indRetificacao = indRetificacao;
        return this;
    }

    /// <summary>
    /// Define o número do recibo (para retificações)
    /// </summary>
    /// <param name="nrRecibo">Número do recibo</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeEventoBuilder WithNrRecibo(string nrRecibo)
    {
        _ideEvento.nrRecibo = nrRecibo;
        return this;
    }

    /// <summary>
    /// Define o tipo de ambiente
    /// </summary>
    /// <param name="tpAmb">Tipo de ambiente (1-Produção, 2-Homologação)</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeEventoBuilder WithTpAmb(uint tpAmb)
    {
        _ideEvento.tpAmb = tpAmb;
        return this;
    }

    /// <summary>
    /// Define o aplicativo emissor
    /// </summary>
    /// <param name="aplicEmi">Aplicativo emissor (1-Contribuinte)</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeEventoBuilder WithAplicEmi(uint aplicEmi)
    {
        _ideEvento.aplicEmi = aplicEmi;
        return this;
    }

    /// <summary>
    /// Define a versão do aplicativo
    /// </summary>
    /// <param name="verAplic">Versão do aplicativo</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeEventoBuilder WithVerAplic(string verAplic)
    {
        _ideEvento.verAplic = verAplic;
        return this;
    }
}

/// <summary>
/// Builder para configuração da identificação do declarante
/// </summary>
public sealed class IdeDeclaranteBuilder
{
    private readonly eFinanceiraEvtCadDeclaranteIdeDeclarante _ideDeclarante;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdeDeclaranteBuilder"/> class.
    /// </summary>
    public IdeDeclaranteBuilder()
    {
        _ideDeclarante = new eFinanceiraEvtCadDeclaranteIdeDeclarante();
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
    /// <returns>Instância configurada de eFinanceiraEvtCadDeclaranteIdeDeclarante</returns>
    internal eFinanceiraEvtCadDeclaranteIdeDeclarante Build() => _ideDeclarante;
}

/// <summary>
/// Builder para configuração das informações de cadastro
/// </summary>
public sealed class InfoCadastroBuilder
{
    private readonly eFinanceiraEvtCadDeclaranteInfoCadastro _infoCadastro;

    /// <summary>
    /// Initializes a new instance of the <see cref="InfoCadastroBuilder"/> class.
    /// </summary>
    public InfoCadastroBuilder()
    {
        _infoCadastro = new eFinanceiraEvtCadDeclaranteInfoCadastro();
    }

    /// <summary>
    /// Define o GIIN (Global Intermediary Identification Number)
    /// </summary>
    /// <param name="giin">GIIN do declarante</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoCadastroBuilder WithGIIN(string giin)
    {
        _infoCadastro.GIIN = giin;
        return this;
    }

    /// <summary>
    /// Define a categoria do declarante
    /// </summary>
    /// <param name="categoria">Categoria do declarante</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoCadastroBuilder WithCategoriaDeclarante(string categoria)
    {
        _infoCadastro.CategoriaDeclarante = categoria;
        return this;
    }

    /// <summary>
    /// Configura os NIFs (National Identification Numbers)
    /// </summary>
    /// <param name="configAction">Ação para configurar os NIFs</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoCadastroBuilder WithNIFs(Action<NIFCollectionBuilder> configAction)
    {
        var builder = new NIFCollectionBuilder();
        configAction(builder);
        _infoCadastro.NIF = builder.Build();
        return this;
    }

    /// <summary>
    /// Define o nome do declarante
    /// </summary>
    /// <param name="nome">Nome do declarante</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoCadastroBuilder WithNome(string nome)
    {
        _infoCadastro.nome = nome;
        return this;
    }

    /// <summary>
    /// Define o tipo de nome
    /// </summary>
    /// <param name="tpNome">Tipo de nome</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoCadastroBuilder WithTipoNome(string tpNome)
    {
        _infoCadastro.tpNome = tpNome;
        return this;
    }

    /// <summary>
    /// Define o endereço livre
    /// </summary>
    /// <param name="endereco">Endereço em formato livre</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoCadastroBuilder WithEnderecoLivre(string endereco)
    {
        _infoCadastro.enderecoLivre = endereco;
        return this;
    }

    /// <summary>
    /// Define o tipo de endereço
    /// </summary>
    /// <param name="tpEndereco">Tipo de endereço</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoCadastroBuilder WithTipoEndereco(string tpEndereco)
    {
        _infoCadastro.tpEndereco = tpEndereco;
        return this;
    }

    /// <summary>
    /// Define o município
    /// </summary>
    /// <param name="municipio">Código do município</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoCadastroBuilder WithMunicipio(uint municipio)
    {
        _infoCadastro.municipio = municipio;
        return this;
    }

    /// <summary>
    /// Define a UF
    /// </summary>
    /// <param name="uf">Sigla da UF</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoCadastroBuilder WithUF(string uf)
    {
        _infoCadastro.UF = uf;
        return this;
    }

    /// <summary>
    /// Define o CEP
    /// </summary>
    /// <param name="cep">CEP</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoCadastroBuilder WithCEP(string cep)
    {
        _infoCadastro.CEP = cep;
        return this;
    }

    /// <summary>
    /// Define o país
    /// </summary>
    /// <param name="pais">Código do país</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoCadastroBuilder WithPais(string pais)
    {
        _infoCadastro.Pais = pais;
        return this;
    }

    /// <summary>
    /// Configura os países de residência
    /// </summary>
    /// <param name="configAction">Ação para configurar os países de residência</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoCadastroBuilder WithPaisesResidencia(Action<PaisResidCollectionBuilder> configAction)
    {
        var builder = new PaisResidCollectionBuilder();
        configAction(builder);
        _infoCadastro.paisResid = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura outros endereços
    /// </summary>
    /// <param name="configAction">Ação para configurar outros endereços</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoCadastroBuilder WithOutrosEnderecos(Action<EnderecoOutrosCollectionBuilder> configAction)
    {
        var builder = new EnderecoOutrosCollectionBuilder();
        configAction(builder);
        _infoCadastro.EnderecoOutros = builder.Build();
        return this;
    }

    /// <summary>
    /// Constrói a instância final das informações de cadastro
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtCadDeclaranteInfoCadastro</returns>
    internal eFinanceiraEvtCadDeclaranteInfoCadastro Build() => _infoCadastro;
}

/// <summary>
/// Builder para configuração de coleção de NIFs
/// </summary>
public sealed class NIFCollectionBuilder
{
    private readonly List<eFinanceiraEvtCadDeclaranteInfoCadastroNIF> _nifs;

    /// <summary>
    /// Initializes a new instance of the <see cref="NIFCollectionBuilder"/> class.
    /// </summary>
    public NIFCollectionBuilder()
    {
        _nifs = new List<eFinanceiraEvtCadDeclaranteInfoCadastroNIF>();
    }

    /// <summary>
    /// Adiciona um NIF
    /// </summary>
    /// <param name="configAction">Ação para configurar o NIF</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public NIFCollectionBuilder AddNIF(Action<NIFBuilder> configAction)
    {
        var builder = new NIFBuilder();
        configAction(builder);
        _nifs.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Constrói o array de NIFs
    /// </summary>
    /// <returns>Array de NIFs configurados</returns>
    internal eFinanceiraEvtCadDeclaranteInfoCadastroNIF[] Build() => _nifs.ToArray();
}

/// <summary>
/// Builder para configuração de NIF individual
/// </summary>
public sealed class NIFBuilder
{
    private readonly eFinanceiraEvtCadDeclaranteInfoCadastroNIF _nif;

    /// <summary>
    /// Initializes a new instance of the <see cref="NIFBuilder"/> class.
    /// </summary>
    public NIFBuilder()
    {
        _nif = new eFinanceiraEvtCadDeclaranteInfoCadastroNIF();
    }

    /// <summary>
    /// Define o número do NIF
    /// </summary>
    /// <param name="numeroNIF">Número do NIF</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public NIFBuilder WithNumeroNIF(string numeroNIF)
    {
        _nif.NumeroNIF = numeroNIF;
        return this;
    }

    /// <summary>
    /// Define o país emissor do NIF
    /// </summary>
    /// <param name="paisEmissao">Código do país emissor</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public NIFBuilder WithPaisEmissao(string paisEmissao)
    {
        _nif.PaisEmissao = paisEmissao;
        return this;
    }

    /// <summary>
    /// Define o tipo do NIF
    /// </summary>
    /// <param name="tpNIF">Tipo do NIF</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public NIFBuilder WithTipoNIF(string tpNIF)
    {
        _nif.tpNIF = tpNIF;
        return this;
    }

    /// <summary>
    /// Constrói a instância final do NIF
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtCadDeclaranteInfoCadastroNIF</returns>
    internal eFinanceiraEvtCadDeclaranteInfoCadastroNIF Build() => _nif;
}

/// <summary>
/// Builder para configuração de coleção de países de residência
/// </summary>
public sealed class PaisResidCollectionBuilder
{
    private readonly List<eFinanceiraEvtCadDeclaranteInfoCadastroPaisResid> _paisesResid;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaisResidCollectionBuilder"/> class.
    /// </summary>
    public PaisResidCollectionBuilder()
    {
        _paisesResid = new List<eFinanceiraEvtCadDeclaranteInfoCadastroPaisResid>();
    }

    /// <summary>
    /// Adiciona um país de residência
    /// </summary>
    /// <param name="configAction">Ação para configurar o país de residência</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public PaisResidCollectionBuilder AddPaisResidencia(Action<PaisResidBuilder> configAction)
    {
        var builder = new PaisResidBuilder();
        configAction(builder);
        _paisesResid.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Constrói o array de países de residência
    /// </summary>
    /// <returns>Array de países de residência configurados</returns>
    internal eFinanceiraEvtCadDeclaranteInfoCadastroPaisResid[] Build() => _paisesResid.ToArray();
}

/// <summary>
/// Builder para configuração de país de residência individual
/// </summary>
public sealed class PaisResidBuilder
{
    private readonly eFinanceiraEvtCadDeclaranteInfoCadastroPaisResid _paisResid;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaisResidBuilder"/> class.
    /// </summary>
    public PaisResidBuilder()
    {
        _paisResid = new eFinanceiraEvtCadDeclaranteInfoCadastroPaisResid();
    }

    /// <summary>
    /// Define o país
    /// </summary>
    /// <param name="pais">Código do país</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public PaisResidBuilder WithPais(string pais)
    {
        _paisResid.Pais = pais;
        return this;
    }

    /// <summary>
    /// Constrói a instância final do país de residência
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtCadDeclaranteInfoCadastroPaisResid</returns>
    internal eFinanceiraEvtCadDeclaranteInfoCadastroPaisResid Build() => _paisResid;
}

/// <summary>
/// Builder para configuração de coleção de outros endereços
/// </summary>
public sealed class EnderecoOutrosCollectionBuilder
{
    private readonly List<eFinanceiraEvtCadDeclaranteInfoCadastroEnderecoOutros> _enderecosOutros;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnderecoOutrosCollectionBuilder"/> class.
    /// </summary>
    public EnderecoOutrosCollectionBuilder()
    {
        _enderecosOutros = new List<eFinanceiraEvtCadDeclaranteInfoCadastroEnderecoOutros>();
    }

    /// <summary>
    /// Adiciona outro endereço
    /// </summary>
    /// <param name="configAction">Ação para configurar o endereço</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EnderecoOutrosCollectionBuilder AddEndereco(Action<EnderecoOutrosBuilder> configAction)
    {
        var builder = new EnderecoOutrosBuilder();
        configAction(builder);
        _enderecosOutros.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Constrói o array de outros endereços
    /// </summary>
    /// <returns>Array de outros endereços configurados</returns>
    internal eFinanceiraEvtCadDeclaranteInfoCadastroEnderecoOutros[] Build() => _enderecosOutros.ToArray();
}

/// <summary>
/// Builder para configuração de outro endereço individual
/// </summary>
public sealed class EnderecoOutrosBuilder
{
    private readonly eFinanceiraEvtCadDeclaranteInfoCadastroEnderecoOutros _enderecoOutros;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnderecoOutrosBuilder"/> class.
    /// </summary>
    public EnderecoOutrosBuilder()
    {
        _enderecoOutros = new eFinanceiraEvtCadDeclaranteInfoCadastroEnderecoOutros();
    }

    /// <summary>
    /// Define o tipo de endereço
    /// </summary>
    /// <param name="tpEndereco">Tipo de endereço</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EnderecoOutrosBuilder WithTipoEndereco(string tpEndereco)
    {
        _enderecoOutros.tpEndereco = tpEndereco;
        return this;
    }

    /// <summary>
    /// Define o país
    /// </summary>
    /// <param name="pais">Código do país</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EnderecoOutrosBuilder WithPais(string pais)
    {
        _enderecoOutros.Pais = pais;
        return this;
    }

    /// <summary>
    /// Define o endereço livre (adiciona aos Items como string)
    /// </summary>
    /// <param name="enderecoLivre">Endereço em formato livre</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EnderecoOutrosBuilder WithEnderecoLivre(string enderecoLivre)
    {
        _enderecoOutros.Items = new object[] { enderecoLivre };
        return this;
    }

    /// <summary>
    /// Constrói a instância final do outro endereço
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtCadDeclaranteInfoCadastroEnderecoOutros</returns>
    internal eFinanceiraEvtCadDeclaranteInfoCadastroEnderecoOutros Build() => _enderecoOutros;
}

#endregion

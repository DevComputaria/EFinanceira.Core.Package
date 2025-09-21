using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Generated.Eventos.EvtAberturaeFinanceira;

namespace EFinanceira.Messages.Builders.Eventos.EvtAberturaeFinanceira;

/// <summary>
/// Mensagem de evento de Abertura e-Financeira
/// </summary>
public sealed class EvtAberturaeFinanceiraMessage : IEFinanceiraMessage
{
    public string Version { get; private set; }
    public string RootElementName => "evtAberturaeFinanceira";
    public string? IdAttributeName => "id";
    public string? IdValue { get; internal set; }
    public object Payload => Evento;

    /// <summary>
    /// Evento tipado gerado do XSD
    /// </summary>
    public eFinanceiraEvtAberturaeFinanceira Evento { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EvtAberturaeFinanceiraMessage"/> class.
    /// Construtor sem parâmetros necessário para serialização XML.
    /// </summary>
    public EvtAberturaeFinanceiraMessage()
    {
        Version = "v1_2_1";
        Evento = new eFinanceiraEvtAberturaeFinanceira();
        IdValue = string.Empty;
    }

    internal EvtAberturaeFinanceiraMessage(eFinanceiraEvtAberturaeFinanceira evento, string version)
    {
        Evento = evento;
        Version = version;
        IdValue = evento.id;
    }
}

/// <summary>
/// Builder principal para construção de eventos de Abertura e-Financeira.
/// Utiliza o padrão fluent interface para facilitar a criação de eventos estruturados.
/// </summary>
public sealed class EvtAberturaeFinanceiraBuilder : IMessageBuilder<EvtAberturaeFinanceiraMessage>
{
    private readonly eFinanceiraEvtAberturaeFinanceira _evento;
    private readonly string _version;

    /// <summary>
    /// Initializes a new instance of the <see cref="EvtAberturaeFinanceiraBuilder"/> class.
    /// </summary>
    /// <param name="version">Versão do schema (padrão: v1_2_1)</param>
    public EvtAberturaeFinanceiraBuilder(string version = "v1_2_1")
    {
        _version = version;
        _evento = new eFinanceiraEvtAberturaeFinanceira
        {
            id = GenerateId(),
            ideEvento = new eFinanceiraEvtAberturaeFinanceiraIdeEvento
            {
                indRetificacao = 1, // Original
                tpAmb = 2, // Homologação
                aplicEmi = 1, // Aplicativo do contribuinte
                verAplic = "1.0.0"
            }
        };
    }

    /// <summary>
    /// Cria uma nova instância do builder EvtAberturaeFinanceira
    /// </summary>
    /// <param name="version">Versão do schema</param>
    /// <returns>Nova instância do builder</returns>
    public static EvtAberturaeFinanceiraBuilder Create(string version = "v1_2_1") => new(version);

    /// <summary>
    /// Define o ID do evento
    /// </summary>
    /// <param name="id">ID único do evento</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtAberturaeFinanceiraBuilder WithId(string id)
    {
        _evento.id = id;
        return this;
    }

    /// <summary>
    /// Configura os dados da identificação do evento
    /// </summary>
    /// <param name="configAction">Ação para configurar o IdeEvento</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtAberturaeFinanceiraBuilder WithIdeEvento(Action<IdeEventoBuilder> configAction)
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
    public EvtAberturaeFinanceiraBuilder WithIdeDeclarante(Action<IdeDeclaranteBuilder> configAction)
    {
        var builder = new IdeDeclaranteBuilder();
        configAction(builder);
        _evento.ideDeclarante = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura as informações de abertura
    /// </summary>
    /// <param name="configAction">Ação para configurar o InfoAbertura</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtAberturaeFinanceiraBuilder WithInfoAbertura(Action<InfoAberturaBuilder> configAction)
    {
        var builder = new InfoAberturaBuilder();
        configAction(builder);
        _evento.infoAbertura = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura os tipos de empresa para abertura de pessoa física
    /// </summary>
    /// <param name="configAction">Ação para configurar os tipos de empresa</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtAberturaeFinanceiraBuilder WithAberturaPP(Action<AberturaPPCollectionBuilder> configAction)
    {
        var builder = new AberturaPPCollectionBuilder();
        configAction(builder);
        _evento.AberturaPP = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura as informações de abertura de movimentação de operações financeiras
    /// </summary>
    /// <param name="configAction">Ação para configurar o AberturaMovOpFin</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtAberturaeFinanceiraBuilder WithAberturaMovOpFin(Action<AberturaMovOpFinBuilder> configAction)
    {
        var builder = new AberturaMovOpFinBuilder();
        configAction(builder);
        _evento.AberturaMovOpFin = builder.Build();
        return this;
    }

    /// <summary>
    /// Constrói a instância final da mensagem de evento
    /// </summary>
    /// <returns>Instância configurada de EvtAberturaeFinanceiraMessage</returns>
    public EvtAberturaeFinanceiraMessage Build()
    {
        ValidateRequiredFields();
        return new EvtAberturaeFinanceiraMessage(_evento, _version);
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
        
        if (_evento.infoAbertura == null)
            throw new InvalidOperationException("InfoAbertura é obrigatório");
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
    private readonly eFinanceiraEvtAberturaeFinanceiraIdeEvento _ideEvento;

    internal IdeEventoBuilder(eFinanceiraEvtAberturaeFinanceiraIdeEvento ideEvento)
    {
        _ideEvento = ideEvento;
    }

    /// <summary>
    /// Define o indicador de retificação (1=Original, 2=Retificação)
    /// </summary>
    /// <param name="indRetificacao">Indicador de retificação</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeEventoBuilder WithIndRetificacao(uint indRetificacao)
    {
        _ideEvento.indRetificacao = indRetificacao;
        return this;
    }

    /// <summary>
    /// Define o número do recibo (obrigatório se retificação)
    /// </summary>
    /// <param name="nrRecibo">Número do recibo</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeEventoBuilder WithNumeroRecibo(string nrRecibo)
    {
        _ideEvento.nrRecibo = nrRecibo;
        return this;
    }

    /// <summary>
    /// Define o tipo de ambiente (1=Produção, 2=Homologação)
    /// </summary>
    /// <param name="tpAmb">Tipo de ambiente</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeEventoBuilder WithTipoAmbiente(uint tpAmb)
    {
        _ideEvento.tpAmb = tpAmb;
        return this;
    }

    /// <summary>
    /// Define o aplicativo emissor (1=Aplicativo do contribuinte)
    /// </summary>
    /// <param name="aplicEmi">Aplicativo emissor</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeEventoBuilder WithAplicativoEmissor(uint aplicEmi)
    {
        _ideEvento.aplicEmi = aplicEmi;
        return this;
    }

    /// <summary>
    /// Define a versão da aplicação
    /// </summary>
    /// <param name="verAplic">Versão da aplicação</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeEventoBuilder WithVersaoAplicacao(string verAplic)
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
    private readonly eFinanceiraEvtAberturaeFinanceiraIdeDeclarante _ideDeclarante;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdeDeclaranteBuilder"/> class.
    /// </summary>
    public IdeDeclaranteBuilder()
    {
        _ideDeclarante = new eFinanceiraEvtAberturaeFinanceiraIdeDeclarante();
    }

    /// <summary>
    /// Define o CNPJ do declarante
    /// </summary>
    /// <param name="cnpjDeclarante">CNPJ do declarante</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeDeclaranteBuilder WithCnpjDeclarante(string cnpjDeclarante)
    {
        _ideDeclarante.cnpjDeclarante = cnpjDeclarante;
        return this;
    }

    /// <summary>
    /// Constrói a instância final da identificação do declarante
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtAberturaeFinanceiraIdeDeclarante</returns>
    internal eFinanceiraEvtAberturaeFinanceiraIdeDeclarante Build() => _ideDeclarante;
}

/// <summary>
/// Builder para configuração das informações de abertura
/// </summary>
public sealed class InfoAberturaBuilder
{
    private readonly eFinanceiraEvtAberturaeFinanceiraInfoAbertura _infoAbertura;

    /// <summary>
    /// Initializes a new instance of the <see cref="InfoAberturaBuilder"/> class.
    /// </summary>
    public InfoAberturaBuilder()
    {
        _infoAbertura = new eFinanceiraEvtAberturaeFinanceiraInfoAbertura();
    }

    /// <summary>
    /// Define a data de início do período
    /// </summary>
    /// <param name="dtInicio">Data de início</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoAberturaBuilder WithDataInicio(DateTime dtInicio)
    {
        _infoAbertura.dtInicio = dtInicio;
        return this;
    }

    /// <summary>
    /// Define a data de fim do período
    /// </summary>
    /// <param name="dtFim">Data de fim</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoAberturaBuilder WithDataFim(DateTime dtFim)
    {
        _infoAbertura.dtFim = dtFim;
        return this;
    }

    /// <summary>
    /// Constrói a instância final das informações de abertura
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtAberturaeFinanceiraInfoAbertura</returns>
    internal eFinanceiraEvtAberturaeFinanceiraInfoAbertura Build() => _infoAbertura;
}

#endregion

#region Builders para AberturaPP

/// <summary>
/// Builder para coleção de tipos de empresa para abertura de pessoa física
/// </summary>
public sealed class AberturaPPCollectionBuilder
{
    private readonly List<eFinanceiraEvtAberturaeFinanceiraTpEmpresa> _tiposEmpresa = new();

    /// <summary>
    /// Adiciona um tipo de empresa à coleção
    /// </summary>
    /// <param name="configAction">Ação para configurar o tipo de empresa</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public AberturaPPCollectionBuilder AddTipoEmpresa(Action<TipoEmpresaBuilder> configAction)
    {
        var builder = new TipoEmpresaBuilder();
        configAction(builder);
        _tiposEmpresa.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Constrói o array final de tipos de empresa
    /// </summary>
    /// <returns>Array configurado de eFinanceiraEvtAberturaeFinanceiraTpEmpresa</returns>
    internal eFinanceiraEvtAberturaeFinanceiraTpEmpresa[] Build() => _tiposEmpresa.ToArray();
}

/// <summary>
/// Builder para configuração de tipo de empresa
/// </summary>
public sealed class TipoEmpresaBuilder
{
    private readonly eFinanceiraEvtAberturaeFinanceiraTpEmpresa _tipoEmpresa;

    /// <summary>
    /// Initializes a new instance of the <see cref="TipoEmpresaBuilder"/> class.
    /// </summary>
    public TipoEmpresaBuilder()
    {
        _tipoEmpresa = new eFinanceiraEvtAberturaeFinanceiraTpEmpresa();
    }

    /// <summary>
    /// Define o tipo de previdência privada
    /// </summary>
    /// <param name="tpPrevPriv">Tipo de previdência privada</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public TipoEmpresaBuilder WithTipoPrevidenciaPrivada(string tpPrevPriv)
    {
        _tipoEmpresa.tpPrevPriv = tpPrevPriv;
        return this;
    }

    /// <summary>
    /// Constrói a instância final do tipo de empresa
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtAberturaeFinanceiraTpEmpresa</returns>
    internal eFinanceiraEvtAberturaeFinanceiraTpEmpresa Build() => _tipoEmpresa;
}

#endregion

#region Builders para AberturaMovOpFin

/// <summary>
/// Builder para configuração da abertura de movimentação de operações financeiras
/// </summary>
public sealed class AberturaMovOpFinBuilder
{
    private readonly eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFin _aberturaMovOpFin;

    /// <summary>
    /// Initializes a new instance of the <see cref="AberturaMovOpFinBuilder"/> class.
    /// </summary>
    public AberturaMovOpFinBuilder()
    {
        _aberturaMovOpFin = new eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFin();
    }

    /// <summary>
    /// Configura o responsável RMF (Responsável pela Movimentação Financeira)
    /// </summary>
    /// <param name="configAction">Ação para configurar o ResponsavelRMF</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public AberturaMovOpFinBuilder WithResponsavelRMF(Action<ResponsavelRMFBuilder> configAction)
    {
        var builder = new ResponsavelRMFBuilder();
        configAction(builder);
        _aberturaMovOpFin.ResponsavelRMF = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura os responsáveis financeiros
    /// </summary>
    /// <param name="configAction">Ação para configurar a coleção de ResponsáveisFinanceiros</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public AberturaMovOpFinBuilder WithResponsaveisFinanceiros(Action<ResponsaveisFinanceirosCollectionBuilder> configAction)
    {
        var builder = new ResponsaveisFinanceirosCollectionBuilder();
        configAction(builder);
        _aberturaMovOpFin.RespeFin = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura o representante legal
    /// </summary>
    /// <param name="configAction">Ação para configurar o RepresentanteLegal</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public AberturaMovOpFinBuilder WithRepresentanteLegal(Action<RepresentanteLegalBuilder> configAction)
    {
        var builder = new RepresentanteLegalBuilder();
        configAction(builder);
        _aberturaMovOpFin.RepresLegal = builder.Build();
        return this;
    }

    /// <summary>
    /// Constrói a instância final da abertura de movimentação de operações financeiras
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFin</returns>
    internal eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFin Build() => _aberturaMovOpFin;
}

#endregion

#region Builders para ResponsavelRMF

/// <summary>
/// Builder para configuração do responsável RMF
/// </summary>
public sealed class ResponsavelRMFBuilder
{
    private readonly eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinResponsavelRMF _responsavelRMF;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponsavelRMFBuilder"/> class.
    /// </summary>
    public ResponsavelRMFBuilder()
    {
        _responsavelRMF = new eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinResponsavelRMF();
    }

    /// <summary>
    /// Define o CNPJ do responsável (quando pessoa jurídica)
    /// </summary>
    /// <param name="cnpj">CNPJ do responsável</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public ResponsavelRMFBuilder WithCnpj(string cnpj)
    {
        _responsavelRMF.CNPJ = cnpj;
        return this;
    }

    /// <summary>
    /// Define o CPF do responsável (quando pessoa física)
    /// </summary>
    /// <param name="cpf">CPF do responsável</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public ResponsavelRMFBuilder WithCpf(string cpf)
    {
        _responsavelRMF.CPF = cpf;
        return this;
    }

    /// <summary>
    /// Define o nome do responsável
    /// </summary>
    /// <param name="nome">Nome do responsável</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public ResponsavelRMFBuilder WithNome(string nome)
    {
        _responsavelRMF.Nome = nome;
        return this;
    }

    /// <summary>
    /// Define o setor do responsável
    /// </summary>
    /// <param name="setor">Setor do responsável</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public ResponsavelRMFBuilder WithSetor(string setor)
    {
        _responsavelRMF.Setor = setor;
        return this;
    }

    /// <summary>
    /// Configura o telefone do responsável
    /// </summary>
    /// <param name="configAction">Ação para configurar o telefone</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public ResponsavelRMFBuilder WithTelefone(Action<TelefoneResponsavelRMFBuilder> configAction)
    {
        var builder = new TelefoneResponsavelRMFBuilder();
        configAction(builder);
        _responsavelRMF.Telefone = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura o endereço do responsável
    /// </summary>
    /// <param name="configAction">Ação para configurar o endereço</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public ResponsavelRMFBuilder WithEndereco(Action<EnderecoResponsavelRMFBuilder> configAction)
    {
        var builder = new EnderecoResponsavelRMFBuilder();
        configAction(builder);
        _responsavelRMF.Endereco = builder.Build();
        return this;
    }

    /// <summary>
    /// Constrói a instância final do responsável RMF
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinResponsavelRMF</returns>
    internal eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinResponsavelRMF Build() => _responsavelRMF;
}

/// <summary>
/// Builder para configuração do telefone do responsável RMF
/// </summary>
public sealed class TelefoneResponsavelRMFBuilder
{
    private readonly eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinResponsavelRMFTelefone _telefone;

    /// <summary>
    /// Initializes a new instance of the <see cref="TelefoneResponsavelRMFBuilder"/> class.
    /// </summary>
    public TelefoneResponsavelRMFBuilder()
    {
        _telefone = new eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinResponsavelRMFTelefone();
    }

    /// <summary>
    /// Define o DDD do telefone
    /// </summary>
    /// <param name="ddd">DDD do telefone</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public TelefoneResponsavelRMFBuilder WithDDD(string ddd)
    {
        _telefone.DDD = ddd;
        return this;
    }

    /// <summary>
    /// Define o número do telefone
    /// </summary>
    /// <param name="numero">Número do telefone</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public TelefoneResponsavelRMFBuilder WithNumero(string numero)
    {
        _telefone.Numero = numero;
        return this;
    }

    /// <summary>
    /// Define o ramal do telefone
    /// </summary>
    /// <param name="ramal">Ramal do telefone</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public TelefoneResponsavelRMFBuilder WithRamal(string ramal)
    {
        _telefone.Ramal = ramal;
        return this;
    }

    /// <summary>
    /// Constrói a instância final do telefone
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinResponsavelRMFTelefone</returns>
    internal eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinResponsavelRMFTelefone Build() => _telefone;
}

/// <summary>
/// Builder para configuração do endereço do responsável RMF
/// </summary>
public sealed class EnderecoResponsavelRMFBuilder
{
    private readonly eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinResponsavelRMFEndereco _endereco;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnderecoResponsavelRMFBuilder"/> class.
    /// </summary>
    public EnderecoResponsavelRMFBuilder()
    {
        _endereco = new eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinResponsavelRMFEndereco();
    }

    /// <summary>
    /// Define o logradouro
    /// </summary>
    /// <param name="logradouro">Logradouro do endereço</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EnderecoResponsavelRMFBuilder WithLogradouro(string logradouro)
    {
        _endereco.Logradouro = logradouro;
        return this;
    }

    /// <summary>
    /// Define o número do endereço
    /// </summary>
    /// <param name="numero">Número do endereço</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EnderecoResponsavelRMFBuilder WithNumero(string numero)
    {
        _endereco.Numero = numero;
        return this;
    }

    /// <summary>
    /// Define o complemento do endereço
    /// </summary>
    /// <param name="complemento">Complemento do endereço</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EnderecoResponsavelRMFBuilder WithComplemento(string complemento)
    {
        _endereco.Complemento = complemento;
        return this;
    }

    /// <summary>
    /// Define o bairro
    /// </summary>
    /// <param name="bairro">Nome do bairro</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EnderecoResponsavelRMFBuilder WithBairro(string bairro)
    {
        _endereco.Bairro = bairro;
        return this;
    }

    /// <summary>
    /// Define o CEP
    /// </summary>
    /// <param name="cep">CEP do endereço</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EnderecoResponsavelRMFBuilder WithCEP(string cep)
    {
        _endereco.CEP = cep;
        return this;
    }

    /// <summary>
    /// Define o município
    /// </summary>
    /// <param name="municipio">Nome do município</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EnderecoResponsavelRMFBuilder WithMunicipio(string municipio)
    {
        _endereco.Municipio = municipio;
        return this;
    }

    /// <summary>
    /// Define a UF
    /// </summary>
    /// <param name="uf">Unidade federativa</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EnderecoResponsavelRMFBuilder WithUF(string uf)
    {
        _endereco.UF = uf;
        return this;
    }

    /// <summary>
    /// Constrói a instância final do endereço
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinResponsavelRMFEndereco</returns>
    internal eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinResponsavelRMFEndereco Build() => _endereco;
}

#endregion

#region Builders para ResponsaveisFinanceiros

/// <summary>
/// Builder para coleção de responsáveis financeiros
/// </summary>
public sealed class ResponsaveisFinanceirosCollectionBuilder
{
    private readonly List<eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinRespeFin> _responsaveisFinanceiros = new();

    /// <summary>
    /// Adiciona um responsável financeiro à coleção
    /// </summary>
    /// <param name="configAction">Ação para configurar o responsável financeiro</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public ResponsaveisFinanceirosCollectionBuilder AddResponsavelFinanceiro(Action<ResponsavelFinanceiroBuilder> configAction)
    {
        var builder = new ResponsavelFinanceiroBuilder();
        configAction(builder);
        _responsaveisFinanceiros.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Constrói o array final de responsáveis financeiros
    /// </summary>
    /// <returns>Array configurado de eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinRespeFin</returns>
    internal eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinRespeFin[] Build() => _responsaveisFinanceiros.ToArray();
}

/// <summary>
/// Builder para configuração de responsável financeiro
/// </summary>
public sealed class ResponsavelFinanceiroBuilder
{
    private readonly eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinRespeFin _responsavelFinanceiro;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponsavelFinanceiroBuilder"/> class.
    /// </summary>
    public ResponsavelFinanceiroBuilder()
    {
        _responsavelFinanceiro = new eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinRespeFin();
    }

    /// <summary>
    /// Define o CPF do responsável financeiro
    /// </summary>
    /// <param name="cpf">CPF do responsável financeiro</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public ResponsavelFinanceiroBuilder WithCpf(string cpf)
    {
        _responsavelFinanceiro.CPF = cpf;
        return this;
    }

    /// <summary>
    /// Define o nome do responsável financeiro
    /// </summary>
    /// <param name="nome">Nome do responsável financeiro</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public ResponsavelFinanceiroBuilder WithNome(string nome)
    {
        _responsavelFinanceiro.Nome = nome;
        return this;
    }

    /// <summary>
    /// Define o setor do responsável financeiro
    /// </summary>
    /// <param name="setor">Setor do responsável financeiro</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public ResponsavelFinanceiroBuilder WithSetor(string setor)
    {
        _responsavelFinanceiro.Setor = setor;
        return this;
    }

    /// <summary>
    /// Define o email do responsável financeiro
    /// </summary>
    /// <param name="email">Email do responsável financeiro</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public ResponsavelFinanceiroBuilder WithEmail(string email)
    {
        _responsavelFinanceiro.Email = email;
        return this;
    }

    /// <summary>
    /// Configura o telefone do responsável financeiro
    /// </summary>
    /// <param name="configAction">Ação para configurar o telefone</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public ResponsavelFinanceiroBuilder WithTelefone(Action<TelefoneResponsavelFinanceiroBuilder> configAction)
    {
        var builder = new TelefoneResponsavelFinanceiroBuilder();
        configAction(builder);
        _responsavelFinanceiro.Telefone = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura o endereço do responsável financeiro
    /// </summary>
    /// <param name="configAction">Ação para configurar o endereço</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public ResponsavelFinanceiroBuilder WithEndereco(Action<EnderecoResponsavelFinanceiroBuilder> configAction)
    {
        var builder = new EnderecoResponsavelFinanceiroBuilder();
        configAction(builder);
        _responsavelFinanceiro.Endereco = builder.Build();
        return this;
    }

    /// <summary>
    /// Constrói a instância final do responsável financeiro
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinRespeFin</returns>
    internal eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinRespeFin Build() => _responsavelFinanceiro;
}

/// <summary>
/// Builder para configuração do telefone do responsável financeiro
/// </summary>
public sealed class TelefoneResponsavelFinanceiroBuilder
{
    private readonly eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinRespeFinTelefone _telefone;

    /// <summary>
    /// Initializes a new instance of the <see cref="TelefoneResponsavelFinanceiroBuilder"/> class.
    /// </summary>
    public TelefoneResponsavelFinanceiroBuilder()
    {
        _telefone = new eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinRespeFinTelefone();
    }

    /// <summary>
    /// Define o DDD do telefone
    /// </summary>
    /// <param name="ddd">DDD do telefone</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public TelefoneResponsavelFinanceiroBuilder WithDDD(string ddd)
    {
        _telefone.DDD = ddd;
        return this;
    }

    /// <summary>
    /// Define o número do telefone
    /// </summary>
    /// <param name="numero">Número do telefone</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public TelefoneResponsavelFinanceiroBuilder WithNumero(string numero)
    {
        _telefone.Numero = numero;
        return this;
    }

    /// <summary>
    /// Define o ramal do telefone
    /// </summary>
    /// <param name="ramal">Ramal do telefone</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public TelefoneResponsavelFinanceiroBuilder WithRamal(string ramal)
    {
        _telefone.Ramal = ramal;
        return this;
    }

    /// <summary>
    /// Constrói a instância final do telefone
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinRespeFinTelefone</returns>
    internal eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinRespeFinTelefone Build() => _telefone;
}

/// <summary>
/// Builder para configuração do endereço do responsável financeiro
/// </summary>
public sealed class EnderecoResponsavelFinanceiroBuilder
{
    private readonly eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinRespeFinEndereco _endereco;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnderecoResponsavelFinanceiroBuilder"/> class.
    /// </summary>
    public EnderecoResponsavelFinanceiroBuilder()
    {
        _endereco = new eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinRespeFinEndereco();
    }

    /// <summary>
    /// Define o logradouro
    /// </summary>
    /// <param name="logradouro">Logradouro do endereço</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EnderecoResponsavelFinanceiroBuilder WithLogradouro(string logradouro)
    {
        _endereco.Logradouro = logradouro;
        return this;
    }

    /// <summary>
    /// Define o número do endereço
    /// </summary>
    /// <param name="numero">Número do endereço</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EnderecoResponsavelFinanceiroBuilder WithNumero(string numero)
    {
        _endereco.Numero = numero;
        return this;
    }

    /// <summary>
    /// Define o complemento do endereço
    /// </summary>
    /// <param name="complemento">Complemento do endereço</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EnderecoResponsavelFinanceiroBuilder WithComplemento(string complemento)
    {
        _endereco.Complemento = complemento;
        return this;
    }

    /// <summary>
    /// Define o bairro
    /// </summary>
    /// <param name="bairro">Nome do bairro</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EnderecoResponsavelFinanceiroBuilder WithBairro(string bairro)
    {
        _endereco.Bairro = bairro;
        return this;
    }

    /// <summary>
    /// Define o CEP
    /// </summary>
    /// <param name="cep">CEP do endereço</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EnderecoResponsavelFinanceiroBuilder WithCEP(string cep)
    {
        _endereco.CEP = cep;
        return this;
    }

    /// <summary>
    /// Define o município
    /// </summary>
    /// <param name="municipio">Nome do município</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EnderecoResponsavelFinanceiroBuilder WithMunicipio(string municipio)
    {
        _endereco.Municipio = municipio;
        return this;
    }

    /// <summary>
    /// Define a UF
    /// </summary>
    /// <param name="uf">Unidade federativa</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EnderecoResponsavelFinanceiroBuilder WithUF(string uf)
    {
        _endereco.UF = uf;
        return this;
    }

    /// <summary>
    /// Constrói a instância final do endereço
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinRespeFinEndereco</returns>
    internal eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinRespeFinEndereco Build() => _endereco;
}

#endregion

#region Builders para RepresentanteLegal

/// <summary>
/// Builder para configuração do representante legal
/// </summary>
public sealed class RepresentanteLegalBuilder
{
    private readonly eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinRepresLegal _representanteLegal;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepresentanteLegalBuilder"/> class.
    /// </summary>
    public RepresentanteLegalBuilder()
    {
        _representanteLegal = new eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinRepresLegal();
    }

    /// <summary>
    /// Define o CPF do representante legal
    /// </summary>
    /// <param name="cpf">CPF do representante legal</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public RepresentanteLegalBuilder WithCpf(string cpf)
    {
        _representanteLegal.CPF = cpf;
        return this;
    }

    /// <summary>
    /// Define o setor do representante legal
    /// </summary>
    /// <param name="setor">Setor do representante legal</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public RepresentanteLegalBuilder WithSetor(string setor)
    {
        _representanteLegal.Setor = setor;
        return this;
    }

    /// <summary>
    /// Configura o telefone do representante legal
    /// </summary>
    /// <param name="configAction">Ação para configurar o telefone</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public RepresentanteLegalBuilder WithTelefone(Action<TelefoneRepresentanteLegalBuilder> configAction)
    {
        var builder = new TelefoneRepresentanteLegalBuilder();
        configAction(builder);
        _representanteLegal.Telefone = builder.Build();
        return this;
    }

    /// <summary>
    /// Constrói a instância final do representante legal
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinRepresLegal</returns>
    internal eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinRepresLegal Build() => _representanteLegal;
}

/// <summary>
/// Builder para configuração do telefone do representante legal
/// </summary>
public sealed class TelefoneRepresentanteLegalBuilder
{
    private readonly eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinRepresLegalTelefone _telefone;

    /// <summary>
    /// Initializes a new instance of the <see cref="TelefoneRepresentanteLegalBuilder"/> class.
    /// </summary>
    public TelefoneRepresentanteLegalBuilder()
    {
        _telefone = new eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinRepresLegalTelefone();
    }

    /// <summary>
    /// Define o DDD do telefone
    /// </summary>
    /// <param name="ddd">DDD do telefone</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public TelefoneRepresentanteLegalBuilder WithDDD(string ddd)
    {
        _telefone.DDD = ddd;
        return this;
    }

    /// <summary>
    /// Define o número do telefone
    /// </summary>
    /// <param name="numero">Número do telefone</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public TelefoneRepresentanteLegalBuilder WithNumero(string numero)
    {
        _telefone.Numero = numero;
        return this;
    }

    /// <summary>
    /// Define o ramal do telefone
    /// </summary>
    /// <param name="ramal">Ramal do telefone</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public TelefoneRepresentanteLegalBuilder WithRamal(string ramal)
    {
        _telefone.Ramal = ramal;
        return this;
    }

    /// <summary>
    /// Constrói a instância final do telefone
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinRepresLegalTelefone</returns>
    internal eFinanceiraEvtAberturaeFinanceiraAberturaMovOpFinRepresLegalTelefone Build() => _telefone;
}

#endregion

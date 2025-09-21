using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Generated.Consultas.RetRERCT;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor
#pragma warning disable SA1642 // Constructor summary documentation should begin with standard text

namespace EFinanceira.Messages.Builders.Consultas.RetRERCT;

/// <summary>
/// Mensagem de consulta RERCT
/// </summary>
public sealed class RetRERCTMessage : IEFinanceiraMessage
{
    public string Version { get; }
    public string RootElementName => "eFinanceira";
    public string? IdAttributeName => null;
    public string? IdValue => null;
    public object Payload => Consulta;

    /// <summary>
    /// Consulta tipada gerada do XSD
    /// </summary>
    public eFinanceira Consulta { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RetRERCTMessage"/> class.
    /// </summary>
    public RetRERCTMessage()
    {
        Consulta = new eFinanceira();
        Version = "v1_2_0";
    }

    internal RetRERCTMessage(eFinanceira consulta, string version)
    {
        Consulta = consulta;
        Version = version;
    }
}

/// <summary>
/// Builder principal para construção de mensagens RetRERCT (Retorno de Consulta RERCT).
/// Utiliza o padrão fluent interface para facilitar a criação de mensagens estruturadas.
/// </summary>
public class RetRERCTBuilder
{
    private readonly eFinanceira _message;
    private readonly string _version;

    /// <summary>
    /// Initializes a new instance of the <see cref="RetRERCTBuilder"/> class.
    /// </summary>
    /// <param name="version">Versão do schema.</param>
    public RetRERCTBuilder(string version)
    {
        _message = new eFinanceira();
        _version = version;
    }

    /// <summary>
    /// Cria uma nova instância do builder RetRERCT.
    /// </summary>
    /// <returns>Nova instância do builder.</returns>
    public static RetRERCTBuilder Create() => new("v1_2_0");

    /// <summary>
    /// Configura os dados de processamento da consulta.
    /// </summary>
    /// <param name="configAction">Ação para configurar os dados de processamento.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public RetRERCTBuilder ComDadosProcessamento(Action<DadosProcessamentoBuilder> configAction)
    {
        var builder = new DadosProcessamentoBuilder();
        configAction(builder);
        
        if (_message.retornoConsultaInformacoesRerct == null)
        {
            _message.retornoConsultaInformacoesRerct = new eFinanceiraRetornoConsultaInformacoesRerct();
        }
        
        _message.retornoConsultaInformacoesRerct.dadosProcessamento = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura os dados de eventos da consulta.
    /// </summary>
    /// <param name="configAction">Ação para configurar os dados de eventos.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public RetRERCTBuilder ComDadosEventos(Action<DadosEventosCollectionBuilder> configAction)
    {
        var builder = new DadosEventosCollectionBuilder();
        configAction(builder);
        
        if (_message.retornoConsultaInformacoesRerct == null)
        {
            _message.retornoConsultaInformacoesRerct = new eFinanceiraRetornoConsultaInformacoesRerct();
        }
        
        _message.retornoConsultaInformacoesRerct.dadosEvento = builder.Build();
        return this;
    }

    /// <summary>
    /// Constrói a instância final da mensagem RetRERCT.
    /// </summary>
    /// <returns>Instância configurada de RetRERCTMessage.</returns>
    public RetRERCTMessage Build() => new(_message, _version);
}

/// <summary>
/// Builder para configuração dos dados de processamento.
/// </summary>
public class DadosProcessamentoBuilder
{
    private readonly TDadosProcessamento _dadosProcessamento;

    /// <summary>
    /// Construtor do builder de dados de processamento.
    /// </summary>
    public DadosProcessamentoBuilder()
    {
        _dadosProcessamento = new TDadosProcessamento();
    }

    /// <summary>
    /// Define o código de status da resposta.
    /// </summary>
    /// <param name="codigoStatus">Código de status da resposta.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public DadosProcessamentoBuilder ComCodigoStatus(string codigoStatus)
    {
        _dadosProcessamento.cdStatusResposta = codigoStatus;
        return this;
    }

    /// <summary>
    /// Define a descrição da resposta.
    /// </summary>
    /// <param name="descricao">Descrição da resposta.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public DadosProcessamentoBuilder ComDescricaoResposta(string descricao)
    {
        _dadosProcessamento.descResposta = descricao;
        return this;
    }

    /// <summary>
    /// Configura os dados de registro de ocorrências de evento.
    /// </summary>
    /// <param name="configAction">Ação para configurar as ocorrências.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public DadosProcessamentoBuilder ComOcorrencias(Action<OcorrenciasCollectionBuilder> configAction)
    {
        var builder = new OcorrenciasCollectionBuilder();
        configAction(builder);
        _dadosProcessamento.dadosRegistroOcorrenciaEvento = builder.Build();
        return this;
    }

    /// <summary>
    /// Constrói a instância final dos dados de processamento.
    /// </summary>
    /// <returns>Instância configurada de TDadosProcessamento.</returns>
    public TDadosProcessamento Build() => _dadosProcessamento;
}

/// <summary>
/// Builder para coleção de ocorrências.
/// </summary>
public class OcorrenciasCollectionBuilder
{
    private readonly List<TRegistroOcorrenciasOcorrencias> _ocorrencias;

    /// <summary>
    /// Construtor do builder de coleção de ocorrências.
    /// </summary>
    public OcorrenciasCollectionBuilder()
    {
        _ocorrencias = new List<TRegistroOcorrenciasOcorrencias>();
    }

    /// <summary>
    /// Adiciona uma nova ocorrência à coleção.
    /// </summary>
    /// <param name="configAction">Ação para configurar a ocorrência.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public OcorrenciasCollectionBuilder AdicionarOcorrencia(Action<OcorrenciaBuilder> configAction)
    {
        var builder = new OcorrenciaBuilder();
        configAction(builder);
        _ocorrencias.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Constrói a array final de ocorrências.
    /// </summary>
    /// <returns>Array configurada de TRegistroOcorrenciasOcorrencias.</returns>
    public TRegistroOcorrenciasOcorrencias[] Build() => _ocorrencias.ToArray();
}

/// <summary>
/// Builder para configuração de uma ocorrência individual.
/// </summary>
public class OcorrenciaBuilder
{
    private readonly TRegistroOcorrenciasOcorrencias _ocorrencia;

    /// <summary>
    /// Construtor do builder de ocorrência.
    /// </summary>
    public OcorrenciaBuilder()
    {
        _ocorrencia = new TRegistroOcorrenciasOcorrencias();
    }

    /// <summary>
    /// Define o tipo da ocorrência.
    /// </summary>
    /// <param name="tipo">Tipo da ocorrência.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public OcorrenciaBuilder ComTipo(string tipo)
    {
        _ocorrencia.tipo = tipo;
        return this;
    }

    /// <summary>
    /// Define a localização do erro/aviso.
    /// </summary>
    /// <param name="localizacao">Localização do erro/aviso.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public OcorrenciaBuilder ComLocalizacaoErroAviso(string localizacao)
    {
        _ocorrencia.localizacaoErroAviso = localizacao;
        return this;
    }

    /// <summary>
    /// Define o código da ocorrência.
    /// </summary>
    /// <param name="codigo">Código da ocorrência.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public OcorrenciaBuilder ComCodigo(string codigo)
    {
        _ocorrencia.codigo = codigo;
        return this;
    }

    /// <summary>
    /// Define a descrição da ocorrência.
    /// </summary>
    /// <param name="descricao">Descrição da ocorrência.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public OcorrenciaBuilder ComDescricao(string descricao)
    {
        _ocorrencia.descricao = descricao;
        return this;
    }

    /// <summary>
    /// Constrói a instância final da ocorrência.
    /// </summary>
    /// <returns>Instância configurada de TRegistroOcorrenciasOcorrencias.</returns>
    public TRegistroOcorrenciasOcorrencias Build() => _ocorrencia;
}

/// <summary>
/// Builder para coleção de dados de eventos.
/// </summary>
public class DadosEventosCollectionBuilder
{
    private readonly List<TDadosEvento> _eventos;

    /// <summary>
    /// Construtor do builder de coleção de eventos.
    /// </summary>
    public DadosEventosCollectionBuilder()
    {
        _eventos = new List<TDadosEvento>();
    }

    /// <summary>
    /// Adiciona um novo evento à coleção.
    /// </summary>
    /// <param name="configAction">Ação para configurar o evento.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public DadosEventosCollectionBuilder AdicionarEvento(Action<DadosEventoBuilder> configAction)
    {
        var builder = new DadosEventoBuilder();
        configAction(builder);
        _eventos.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Constrói a array final de eventos.
    /// </summary>
    /// <returns>Array configurada de TDadosEvento.</returns>
    public TDadosEvento[] Build() => _eventos.ToArray();
}

/// <summary>
/// Builder para configuração de um evento individual.
/// </summary>
public class DadosEventoBuilder
{
    private readonly TDadosEvento _evento;

    /// <summary>
    /// Construtor do builder de evento.
    /// </summary>
    public DadosEventoBuilder()
    {
        _evento = new TDadosEvento();
    }

    /// <summary>
    /// Configura a identificação do evento.
    /// </summary>
    /// <param name="configAction">Ação para configurar a identificação do evento.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public DadosEventoBuilder ComIdentificacaoEvento(Action<IdentificacaoEventoBuilder> configAction)
    {
        var builder = new IdentificacaoEventoBuilder();
        configAction(builder);
        _evento.identificacaoEvento = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura a identificação do declarado.
    /// </summary>
    /// <param name="configAction">Ação para configurar a identificação do declarado.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public DadosEventoBuilder ComIdentificacaoDeclarado(Action<IdentificacaoDeclaradoBuilder> configAction)
    {
        var builder = new IdentificacaoDeclaradoBuilder();
        configAction(builder);
        _evento.identificacaoDeclarado = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura as identificações dos titulares.
    /// </summary>
    /// <param name="configAction">Ação para configurar as identificações dos titulares.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public DadosEventoBuilder ComIdentificacoesTitulares(Action<IdentificacoesTitularesCollectionBuilder> configAction)
    {
        var builder = new IdentificacoesTitularesCollectionBuilder();
        configAction(builder);
        _evento.identificacaoTitular = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura os beneficiários finais.
    /// </summary>
    /// <param name="configAction">Ação para configurar os beneficiários finais.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public DadosEventoBuilder ComBeneficiarios(Action<BeneficiariosCollectionBuilder> configAction)
    {
        var builder = new BeneficiariosCollectionBuilder();
        configAction(builder);
        _evento.beneficiarioFinal = builder.Build();
        return this;
    }

    /// <summary>
    /// Constrói a instância final do evento.
    /// </summary>
    /// <returns>Instância configurada de TDadosEvento.</returns>
    public TDadosEvento Build() => _evento;
}

/// <summary>
/// Builder para configuração da identificação do evento.
/// </summary>
public class IdentificacaoEventoBuilder
{
    private readonly TIdentificacaoEvento _identificacao;

    /// <summary>
    /// Construtor do builder de identificação do evento.
    /// </summary>
    public IdentificacaoEventoBuilder()
    {
        _identificacao = new TIdentificacaoEvento();
    }

    /// <summary>
    /// Define o ID do evento.
    /// </summary>
    /// <param name="idEvento">ID do evento.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public IdentificacaoEventoBuilder ComIdEvento(string idEvento)
    {
        _identificacao.idEvento = idEvento;
        return this;
    }

    /// <summary>
    /// Define o ID do evento RERCT.
    /// </summary>
    /// <param name="ideEventoRERCT">ID do evento RERCT.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public IdentificacaoEventoBuilder ComIdeEventoRERCT(string ideEventoRERCT)
    {
        _identificacao.ideEventoRERCT = ideEventoRERCT;
        return this;
    }

    /// <summary>
    /// Define a situação do evento.
    /// </summary>
    /// <param name="situacao">Situação do evento.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public IdentificacaoEventoBuilder ComSituacao(string situacao)
    {
        _identificacao.situacao = situacao;
        return this;
    }

    /// <summary>
    /// Define o número do recibo.
    /// </summary>
    /// <param name="numeroRecibo">Número do recibo.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public IdentificacaoEventoBuilder ComNumeroRecibo(string numeroRecibo)
    {
        _identificacao.numeroRecibo = numeroRecibo;
        return this;
    }

    /// <summary>
    /// Constrói a instância final da identificação do evento.
    /// </summary>
    /// <returns>Instância configurada de TIdentificacaoEvento.</returns>
    public TIdentificacaoEvento Build() => _identificacao;
}

/// <summary>
/// Builder para configuração da identificação do declarado.
/// </summary>
public class IdentificacaoDeclaradoBuilder
{
    private readonly TIdentificacaoDeclarado _identificacao;

    /// <summary>
    /// Construtor do builder de identificação do declarado.
    /// </summary>
    public IdentificacaoDeclaradoBuilder()
    {
        _identificacao = new TIdentificacaoDeclarado();
    }

    /// <summary>
    /// Define o tipo de inscrição do declarado.
    /// </summary>
    /// <param name="tipoInscricao">Tipo de inscrição do declarado.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public IdentificacaoDeclaradoBuilder ComTipoInscricao(string tipoInscricao)
    {
        _identificacao.tipoInscricaoDeclarado = tipoInscricao;
        return this;
    }

    /// <summary>
    /// Define a inscrição do declarado.
    /// </summary>
    /// <param name="inscricao">Inscrição do declarado.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public IdentificacaoDeclaradoBuilder ComInscricao(string inscricao)
    {
        _identificacao.inscricaoDeclarado = inscricao;
        return this;
    }

    /// <summary>
    /// Constrói a instância final da identificação do declarado.
    /// </summary>
    /// <returns>Instância configurada de TIdentificacaoDeclarado.</returns>
    public TIdentificacaoDeclarado Build() => _identificacao;
}

/// <summary>
/// Builder para coleção de identificações de titulares.
/// </summary>
public class IdentificacoesTitularesCollectionBuilder
{
    private readonly List<TIdentificacaoTitular> _titulares;

    /// <summary>
    /// Construtor do builder de coleção de titulares.
    /// </summary>
    public IdentificacoesTitularesCollectionBuilder()
    {
        _titulares = new List<TIdentificacaoTitular>();
    }

    /// <summary>
    /// Adiciona uma nova identificação de titular à coleção.
    /// </summary>
    /// <param name="configAction">Ação para configurar a identificação do titular.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public IdentificacoesTitularesCollectionBuilder AdicionarTitular(Action<IdentificacaoTitularBuilder> configAction)
    {
        var builder = new IdentificacaoTitularBuilder();
        configAction(builder);
        _titulares.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Constrói a array final de titulares.
    /// </summary>
    /// <returns>Array configurada de TIdentificacaoTitular.</returns>
    public TIdentificacaoTitular[] Build() => _titulares.ToArray();
}

/// <summary>
/// Builder para configuração da identificação do titular.
/// </summary>
public class IdentificacaoTitularBuilder
{
    private readonly TIdentificacaoTitular _identificacao;

    /// <summary>
    /// Construtor do builder de identificação do titular.
    /// </summary>
    public IdentificacaoTitularBuilder()
    {
        _identificacao = new TIdentificacaoTitular();
    }

    /// <summary>
    /// Define o tipo de inscrição do titular.
    /// </summary>
    /// <param name="tipoInscricao">Tipo de inscrição do titular.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public IdentificacaoTitularBuilder ComTipoInscricao(string tipoInscricao)
    {
        _identificacao.tipoInscricaoTitular = tipoInscricao;
        return this;
    }

    /// <summary>
    /// Define a inscrição do titular.
    /// </summary>
    /// <param name="inscricao">Inscrição do titular.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public IdentificacaoTitularBuilder ComInscricao(string inscricao)
    {
        _identificacao.inscricaoTitular = inscricao;
        return this;
    }

    /// <summary>
    /// Define o nome do titular.
    /// </summary>
    /// <param name="nome">Nome do titular.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public IdentificacaoTitularBuilder ComNome(string nome)
    {
        _identificacao.nomeTitular = nome;
        return this;
    }

    /// <summary>
    /// Define o NIF do titular.
    /// </summary>
    /// <param name="nif">NIF do titular.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public IdentificacaoTitularBuilder ComNif(string nif)
    {
        _identificacao.nifTitular = nif;
        return this;
    }

    /// <summary>
    /// Constrói a instância final da identificação do titular.
    /// </summary>
    /// <returns>Instância configurada de TIdentificacaoTitular.</returns>
    public TIdentificacaoTitular Build() => _identificacao;
}

/// <summary>
/// Builder para coleção de beneficiários finais.
/// </summary>
public class BeneficiariosCollectionBuilder
{
    private readonly List<TBeneficiarioFinal> _beneficiarios;

    /// <summary>
    /// Construtor do builder de coleção de beneficiários.
    /// </summary>
    public BeneficiariosCollectionBuilder()
    {
        _beneficiarios = new List<TBeneficiarioFinal>();
    }

    /// <summary>
    /// Adiciona um novo beneficiário final à coleção.
    /// </summary>
    /// <param name="configAction">Ação para configurar o beneficiário final.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public BeneficiariosCollectionBuilder AdicionarBeneficiario(Action<BeneficiarioFinalBuilder> configAction)
    {
        var builder = new BeneficiarioFinalBuilder();
        configAction(builder);
        _beneficiarios.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Constrói a array final de beneficiários.
    /// </summary>
    /// <returns>Array configurada de TBeneficiarioFinal.</returns>
    public TBeneficiarioFinal[] Build() => _beneficiarios.ToArray();
}

/// <summary>
/// Builder para configuração do beneficiário final.
/// </summary>
public class BeneficiarioFinalBuilder
{
    private readonly TBeneficiarioFinal _beneficiario;

    /// <summary>
    /// Construtor do builder de beneficiário final.
    /// </summary>
    public BeneficiarioFinalBuilder()
    {
        _beneficiario = new TBeneficiarioFinal();
    }

    /// <summary>
    /// Define a inscrição do beneficiário.
    /// </summary>
    /// <param name="inscricao">Inscrição do beneficiário.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public BeneficiarioFinalBuilder ComInscricao(string inscricao)
    {
        _beneficiario.inscricaoBeneficiario = inscricao;
        return this;
    }

    /// <summary>
    /// Define o nome do beneficiário.
    /// </summary>
    /// <param name="nome">Nome do beneficiário.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public BeneficiarioFinalBuilder ComNome(string nome)
    {
        _beneficiario.nomeBeneficiario = nome;
        return this;
    }

    /// <summary>
    /// Define o NIF do beneficiário.
    /// </summary>
    /// <param name="nif">NIF do beneficiário.</param>
    /// <returns>Instância atual do builder para encadeamento.</returns>
    public BeneficiarioFinalBuilder ComNif(string nif)
    {
        _beneficiario.nifBeneficiario = nif;
        return this;
    }

    /// <summary>
    /// Constrói a instância final do beneficiário.
    /// </summary>
    /// <returns>Instância configurada de TBeneficiarioFinal.</returns>
    public TBeneficiarioFinal Build() => _beneficiario;
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor

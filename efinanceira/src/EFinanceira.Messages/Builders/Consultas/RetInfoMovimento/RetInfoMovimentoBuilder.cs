using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Generated.Consultas.RetInfoMovimento;

namespace EFinanceira.Messages.Builders.Consultas.RetInfoMovimento;

/// <summary>
/// Mensagem de consulta de informações de movimento
/// </summary>
public sealed class RetInfoMovimentoMessage : IEFinanceiraMessage
{
    public string Version { get; }
    public string RootElementName => "eFinanceira";
    public string? IdAttributeName => null;
    public string? IdValue { get; internal set; }
    public object Payload => Consulta;

    /// <summary>
    /// Consulta tipada gerada do XSD
    /// </summary>
    public eFinanceira Consulta { get; }

    internal RetInfoMovimentoMessage(eFinanceira consulta, string version)
    {
        Consulta = consulta;
        Version = version;
    }
}

/// <summary>
/// Builder fluente para consulta de informações de movimento
/// </summary>
public sealed class RetInfoMovimentoBuilder : IMessageBuilder<RetInfoMovimentoMessage>
{
    private readonly eFinanceira _consulta;
    private readonly string _version;

    public RetInfoMovimentoBuilder(string version = "v1_2_0")
    {
        _version = version;
        _consulta = new eFinanceira
        {
            retornoConsultaInformacoesMovimento = new eFinanceiraRetornoConsultaInformacoesMovimento
            {
                dataHoraProcessamento = DateTime.UtcNow,
                status = new TStatus(),
                identificacaoEmpresaDeclarante = new TIdeEmpresaDeclarante(),
                informacoesMovimento = Array.Empty<TInformacoesMovimento>()
            }
        };
    }

    /// <summary>
    /// Define a data e hora de processamento
    /// </summary>
    public RetInfoMovimentoBuilder WithDataHoraProcessamento(DateTime dataHora)
    {
        if (_consulta.retornoConsultaInformacoesMovimento != null)
        {
            _consulta.retornoConsultaInformacoesMovimento.dataHoraProcessamento = dataHora;
        }
        return this;
    }

    /// <summary>
    /// Define o status da consulta
    /// </summary>
    public RetInfoMovimentoBuilder WithStatus(Action<StatusBuilder> configureStatus)
    {
        var builder = new StatusBuilder();
        configureStatus(builder);

        if (_consulta.retornoConsultaInformacoesMovimento != null)
        {
            _consulta.retornoConsultaInformacoesMovimento.status = builder.Build();
        }
        return this;
    }

    /// <summary>
    /// Define a identificação da empresa declarante
    /// </summary>
    public RetInfoMovimentoBuilder WithEmpresaDeclarante(Action<EmpresaDeclaranteBuilder> configureEmpresa)
    {
        var builder = new EmpresaDeclaranteBuilder();
        configureEmpresa(builder);

        if (_consulta.retornoConsultaInformacoesMovimento != null)
        {
            _consulta.retornoConsultaInformacoesMovimento.identificacaoEmpresaDeclarante = builder.Build();
        }
        return this;
    }

    /// <summary>
    /// Define as informações de movimento
    /// </summary>
    public RetInfoMovimentoBuilder WithInformacoesMovimento(Action<InformacoesMovimentoCollectionBuilder> configureMovimentos)
    {
        var builder = new InformacoesMovimentoCollectionBuilder();
        configureMovimentos(builder);

        if (_consulta.retornoConsultaInformacoesMovimento != null)
        {
            _consulta.retornoConsultaInformacoesMovimento.informacoesMovimento = builder.Build();
        }
        return this;
    }

    /// <inheritdoc />
    public RetInfoMovimentoMessage Build()
    {
        ValidateRequiredFields();
        return new RetInfoMovimentoMessage(_consulta, _version);
    }

    private void ValidateRequiredFields()
    {
        if (_consulta.retornoConsultaInformacoesMovimento == null)
            throw new InvalidOperationException("RetornoConsultaInformacoesMovimento é obrigatório");
    }
}

#region Builders auxiliares

/// <summary>
/// Builder para configuração de status
/// </summary>
public sealed class StatusBuilder
{
    private readonly TStatus _status = new();

    public StatusBuilder WithCodigo(string codigo)
    {
        _status.cdRetorno = codigo;
        return this;
    }

    public StatusBuilder WithDescricao(string descricao)
    {
        _status.descRetorno = descricao;
        return this;
    }

    public StatusBuilder WithOcorrencias(Action<OcorrenciasBuilder> configureOcorrencias)
    {
        var builder = new OcorrenciasBuilder();
        configureOcorrencias(builder);
        _status.dadosRegistroOcorrenciaEvento = builder.Build();
        return this;
    }

    internal TStatus Build() => _status;
}

/// <summary>
/// Builder para configuração de ocorrências
/// </summary>
public sealed class OcorrenciasBuilder
{
    private readonly List<TRegistroOcorrenciasOcorrencias> _ocorrencias = new();

    public OcorrenciasBuilder AddOcorrencia(string tipo, string codigo, string descricao, string localizacao)
    {
        _ocorrencias.Add(new TRegistroOcorrenciasOcorrencias
        {
            tipo = tipo,
            codigo = codigo,
            descricao = descricao,
            localizacaoErroAviso = localizacao
        });
        return this;
    }

    internal TRegistroOcorrenciasOcorrencias[] Build() => _ocorrencias.ToArray();
}

/// <summary>
/// Builder para configuração da empresa declarante
/// </summary>
public sealed class EmpresaDeclaranteBuilder
{
    private readonly TIdeEmpresaDeclarante _empresa = new();

    public EmpresaDeclaranteBuilder WithCnpj(string cnpj)
    {
        _empresa.cnpjEmpresaDeclarante = cnpj;
        return this;
    }

    internal TIdeEmpresaDeclarante Build() => _empresa;
}

/// <summary>
/// Builder para configuração de informações de movimento
/// </summary>
public sealed class InformacoesMovimentoBuilder
{
    private readonly TInformacoesMovimento _movimento = new();

    public InformacoesMovimentoBuilder WithTipoMovimento(string tipoMovimento)
    {
        _movimento.tipoMovimento = tipoMovimento;
        return this;
    }

    public InformacoesMovimentoBuilder WithTipoNI(string tipoNI)
    {
        _movimento.tipoNI = tipoNI;
        return this;
    }

    public InformacoesMovimentoBuilder WithNI(string ni)
    {
        _movimento.NI = ni;
        return this;
    }

    public InformacoesMovimentoBuilder WithAnoMesCaixa(string anoMesCaixa)
    {
        _movimento.anoMesCaixa = anoMesCaixa;
        return this;
    }

    public InformacoesMovimentoBuilder WithAnoCaixa(string anoCaixa)
    {
        _movimento.anoCaixa = anoCaixa;
        return this;
    }

    public InformacoesMovimentoBuilder WithSemestre(string semestre)
    {
        _movimento.semestre = semestre;
        return this;
    }

    public InformacoesMovimentoBuilder WithSituacao(string situacao)
    {
        _movimento.situacao = situacao;
        return this;
    }

    public InformacoesMovimentoBuilder WithNumeroRecibo(string numeroRecibo)
    {
        _movimento.numeroRecibo = numeroRecibo;
        return this;
    }

    public InformacoesMovimentoBuilder WithId(string id)
    {
        _movimento.id = id;
        return this;
    }

    internal TInformacoesMovimento Build() => _movimento;
}

/// <summary>
/// Builder para configuração da coleção de informações de movimento
/// </summary>
public sealed class InformacoesMovimentoCollectionBuilder
{
    private readonly List<TInformacoesMovimento> _movimentos = new();

    public InformacoesMovimentoCollectionBuilder AddMovimento(Action<InformacoesMovimentoBuilder> configureMovimento)
    {
        var builder = new InformacoesMovimentoBuilder();
        configureMovimento(builder);
        _movimentos.Add(builder.Build());
        return this;
    }

    internal TInformacoesMovimento[] Build() => _movimentos.ToArray();
}

#endregion

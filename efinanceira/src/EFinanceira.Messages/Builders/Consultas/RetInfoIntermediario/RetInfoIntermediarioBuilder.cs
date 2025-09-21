using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Generated.Consultas.RetInfoIntermediario;

namespace EFinanceira.Messages.Builders.Consultas.RetInfoIntermediario;

/// <summary>
/// Mensagem de consulta de informações de intermediário
/// </summary>
public sealed class RetInfoIntermediarioMessage : IEFinanceiraMessage
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

    internal RetInfoIntermediarioMessage(eFinanceira consulta, string version)
    {
        Consulta = consulta;
        Version = version;
    }
}

/// <summary>
/// Builder fluente para consulta de informações de intermediário
/// </summary>
public sealed class RetInfoIntermediarioBuilder : IMessageBuilder<RetInfoIntermediarioMessage>
{
    private readonly eFinanceira _consulta;
    private readonly string _version;

    public RetInfoIntermediarioBuilder(string version = "v1_2_0")
    {
        _version = version;
        _consulta = new eFinanceira
        {
            retornoConsultaInformacoesIntermediario = new eFinanceiraRetornoConsultaInformacoesIntermediario
            {
                dataHoraProcessamento = DateTime.UtcNow,
                status = new TStatus(),
                identificacaoEmpresaDeclarante = new TIdeEmpresaDeclarante(),
                identificacaoIntermediario = Array.Empty<TIdentificacaoIntermediario>()
            }
        };
    }

    /// <summary>
    /// Define a data e hora de processamento
    /// </summary>
    public RetInfoIntermediarioBuilder WithDataHoraProcessamento(DateTime dataHora)
    {
        if (_consulta.retornoConsultaInformacoesIntermediario != null)
        {
            _consulta.retornoConsultaInformacoesIntermediario.dataHoraProcessamento = dataHora;
        }
        return this;
    }

    /// <summary>
    /// Define o status da consulta
    /// </summary>
    public RetInfoIntermediarioBuilder WithStatus(Action<StatusBuilder> configureStatus)
    {
        var builder = new StatusBuilder();
        configureStatus(builder);

        if (_consulta.retornoConsultaInformacoesIntermediario != null)
        {
            _consulta.retornoConsultaInformacoesIntermediario.status = builder.Build();
        }
        return this;
    }

    /// <summary>
    /// Define a identificação da empresa declarante
    /// </summary>
    public RetInfoIntermediarioBuilder WithEmpresaDeclarante(Action<EmpresaDeclaranteBuilder> configureEmpresa)
    {
        var builder = new EmpresaDeclaranteBuilder();
        configureEmpresa(builder);

        if (_consulta.retornoConsultaInformacoesIntermediario != null)
        {
            _consulta.retornoConsultaInformacoesIntermediario.identificacaoEmpresaDeclarante = builder.Build();
        }
        return this;
    }

    /// <summary>
    /// Define as identificações de intermediários
    /// </summary>
    public RetInfoIntermediarioBuilder WithIdentificacoesIntermediarios(Action<IdentificacaoIntermediarioCollectionBuilder> configureIntermediarios)
    {
        var builder = new IdentificacaoIntermediarioCollectionBuilder();
        configureIntermediarios(builder);

        if (_consulta.retornoConsultaInformacoesIntermediario != null)
        {
            _consulta.retornoConsultaInformacoesIntermediario.identificacaoIntermediario = builder.Build();
        }
        return this;
    }

    /// <inheritdoc />
    public RetInfoIntermediarioMessage Build()
    {
        ValidateRequiredFields();
        return new RetInfoIntermediarioMessage(_consulta, _version);
    }

    private void ValidateRequiredFields()
    {
        if (_consulta.retornoConsultaInformacoesIntermediario == null)
            throw new InvalidOperationException("RetornoConsultaInformacoesIntermediario é obrigatório");
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
/// Builder para configuração de identificação de intermediário
/// </summary>
public sealed class IdentificacaoIntermediarioBuilder
{
    private readonly TIdentificacaoIntermediario _intermediario = new();

    public IdentificacaoIntermediarioBuilder WithGiin(string giin)
    {
        _intermediario.GIIN = giin;
        return this;
    }

    public IdentificacaoIntermediarioBuilder WithTipoNI(byte tipoNI)
    {
        _intermediario.tpNI = tipoNI;
        _intermediario.tpNISpecified = true;
        return this;
    }

    public IdentificacaoIntermediarioBuilder WithNIIntermediario(string niIntermediario)
    {
        _intermediario.NIIntermediario = niIntermediario;
        return this;
    }

    public IdentificacaoIntermediarioBuilder WithNumeroRecibo(string numeroRecibo)
    {
        _intermediario.numeroRecibo = numeroRecibo;
        return this;
    }

    public IdentificacaoIntermediarioBuilder WithId(string id)
    {
        _intermediario.id = id;
        return this;
    }

    internal TIdentificacaoIntermediario Build() => _intermediario;
}

/// <summary>
/// Builder para configuração da coleção de identificações de intermediários
/// </summary>
public sealed class IdentificacaoIntermediarioCollectionBuilder
{
    private readonly List<TIdentificacaoIntermediario> _intermediarios = new();

    public IdentificacaoIntermediarioCollectionBuilder AddIntermediario(Action<IdentificacaoIntermediarioBuilder> configureIntermediario)
    {
        var builder = new IdentificacaoIntermediarioBuilder();
        configureIntermediario(builder);
        _intermediarios.Add(builder.Build());
        return this;
    }

    internal TIdentificacaoIntermediario[] Build() => _intermediarios.ToArray();
}

#endregion

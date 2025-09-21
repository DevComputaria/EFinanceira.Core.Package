using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Generated.Consultas.RetInfoPatrocinado;

namespace EFinanceira.Messages.Builders.Consultas.RetInfoPatrocinado;

/// <summary>
/// Mensagem de consulta de informações de patrocinado
/// </summary>
public sealed class RetInfoPatrocinadoMessage : IEFinanceiraMessage
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

    internal RetInfoPatrocinadoMessage(eFinanceira consulta, string version)
    {
        Consulta = consulta;
        Version = version;
    }
}

/// <summary>
/// Builder fluente para consulta de informações de patrocinado
/// </summary>
public sealed class RetInfoPatrocinadoBuilder : IMessageBuilder<RetInfoPatrocinadoMessage>
{
    private readonly eFinanceira _consulta;
    private readonly string _version;

    public RetInfoPatrocinadoBuilder(string version = "v1_2_0")
    {
        _version = version;
        _consulta = new eFinanceira
        {
            retornoConsultaInformacoesPatrocinado = new eFinanceiraRetornoConsultaInformacoesPatrocinado
            {
                dataHoraProcessamento = DateTime.UtcNow,
                status = new TStatus(),
                identificacaoEmpresaDeclarante = new TIdeEmpresaDeclarante(),
                identificacaoPatrocinado = Array.Empty<TIdentificacaoPatrocinado>()
            }
        };
    }

    /// <summary>
    /// Define a data e hora de processamento
    /// </summary>
    public RetInfoPatrocinadoBuilder WithDataHoraProcessamento(DateTime dataHora)
    {
        if (_consulta.retornoConsultaInformacoesPatrocinado != null)
        {
            _consulta.retornoConsultaInformacoesPatrocinado.dataHoraProcessamento = dataHora;
        }
        return this;
    }

    /// <summary>
    /// Define o status da consulta
    /// </summary>
    public RetInfoPatrocinadoBuilder WithStatus(Action<StatusBuilder> configureStatus)
    {
        var builder = new StatusBuilder();
        configureStatus(builder);

        if (_consulta.retornoConsultaInformacoesPatrocinado != null)
        {
            _consulta.retornoConsultaInformacoesPatrocinado.status = builder.Build();
        }
        return this;
    }

    /// <summary>
    /// Define a identificação da empresa declarante
    /// </summary>
    public RetInfoPatrocinadoBuilder WithEmpresaDeclarante(Action<EmpresaDeclaranteBuilder> configureEmpresa)
    {
        var builder = new EmpresaDeclaranteBuilder();
        configureEmpresa(builder);

        if (_consulta.retornoConsultaInformacoesPatrocinado != null)
        {
            _consulta.retornoConsultaInformacoesPatrocinado.identificacaoEmpresaDeclarante = builder.Build();
        }
        return this;
    }

    /// <summary>
    /// Define as identificações de patrocinados
    /// </summary>
    public RetInfoPatrocinadoBuilder WithIdentificacoesPatrocinados(Action<IdentificacaoPatrocinadoCollectionBuilder> configurePatrocinados)
    {
        var builder = new IdentificacaoPatrocinadoCollectionBuilder();
        configurePatrocinados(builder);

        if (_consulta.retornoConsultaInformacoesPatrocinado != null)
        {
            _consulta.retornoConsultaInformacoesPatrocinado.identificacaoPatrocinado = builder.Build();
        }
        return this;
    }

    /// <inheritdoc />
    public RetInfoPatrocinadoMessage Build()
    {
        ValidateRequiredFields();
        return new RetInfoPatrocinadoMessage(_consulta, _version);
    }

    private void ValidateRequiredFields()
    {
        if (_consulta.retornoConsultaInformacoesPatrocinado == null)
            throw new InvalidOperationException("RetornoConsultaInformacoesPatrocinado é obrigatório");
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

    public EmpresaDeclaranteBuilder WithGiin(string giin)
    {
        _empresa.GIIN = giin;
        return this;
    }

    internal TIdeEmpresaDeclarante Build() => _empresa;
}

/// <summary>
/// Builder para configuração de identificação de patrocinado
/// </summary>
public sealed class IdentificacaoPatrocinadoBuilder
{
    private readonly TIdentificacaoPatrocinado _patrocinado = new();

    public IdentificacaoPatrocinadoBuilder WithGiin(string giin)
    {
        _patrocinado.GIIN = giin;
        return this;
    }

    public IdentificacaoPatrocinadoBuilder WithCnpj(string cnpj)
    {
        _patrocinado.CNPJ = cnpj;
        return this;
    }

    public IdentificacaoPatrocinadoBuilder WithNumeroRecibo(string numeroRecibo)
    {
        _patrocinado.numeroRecibo = numeroRecibo;
        return this;
    }

    public IdentificacaoPatrocinadoBuilder WithId(string id)
    {
        _patrocinado.id = id;
        return this;
    }

    internal TIdentificacaoPatrocinado Build() => _patrocinado;
}

/// <summary>
/// Builder para configuração da coleção de identificações de patrocinados
/// </summary>
public sealed class IdentificacaoPatrocinadoCollectionBuilder
{
    private readonly List<TIdentificacaoPatrocinado> _patrocinados = new();

    public IdentificacaoPatrocinadoCollectionBuilder AddPatrocinado(Action<IdentificacaoPatrocinadoBuilder> configurePatrocinado)
    {
        var builder = new IdentificacaoPatrocinadoBuilder();
        configurePatrocinado(builder);
        _patrocinados.Add(builder.Build());
        return this;
    }

    internal TIdentificacaoPatrocinado[] Build() => _patrocinados.ToArray();
}

#endregion

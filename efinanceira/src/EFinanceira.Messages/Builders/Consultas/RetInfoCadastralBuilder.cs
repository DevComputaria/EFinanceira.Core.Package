using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Generated.Consultas.RetInfoCadastral;

namespace EFinanceira.Messages.Builders.Consultas;

/// <summary>
/// Mensagem de consulta de informações cadastrais
/// </summary>
public sealed class RetInfoCadastralMessage : IEFinanceiraMessage
{
    public string Version { get; }
    public string RootElementName => "eFinanceira";
    public string? IdAttributeName => "id";
    public string? IdValue { get; internal set; }
    public object Payload => Consulta;

    /// <summary>
    /// Consulta tipada gerada do XSD
    /// </summary>
    public eFinanceira Consulta { get; }

    internal RetInfoCadastralMessage(eFinanceira consulta, string version)
    {
        Consulta = consulta;
        Version = version;
        IdValue = consulta.retornoConsultaInformacoesCadastrais?.id;
    }
}

/// <summary>
/// Builder fluente para consulta de informações cadastrais
/// </summary>
public sealed class RetInfoCadastralBuilder : IMessageBuilder<RetInfoCadastralMessage>
{
    private readonly eFinanceira _consulta;
    private readonly string _version;

    public RetInfoCadastralBuilder(string version = "v1_2_0")
    {
        _version = version;
        _consulta = new eFinanceira
        {
            retornoConsultaInformacoesCadastrais = new eFinanceiraRetornoConsultaInformacoesCadastrais
            {
                dataHoraProcessamento = DateTime.UtcNow,
                status = new TStatus(),
                identificacaoEmpresaDeclarante = new TIdeEmpresaDeclarante(),
                informacoesCadastrais = new TInformacoesCadastrais(),
                numeroRecibo = string.Empty,
                id = GenerateId()
            }
        };
    }

    /// <summary>
    /// Define o ID da consulta
    /// </summary>
    public RetInfoCadastralBuilder WithId(string id)
    {
        if (_consulta.retornoConsultaInformacoesCadastrais != null)
        {
            _consulta.retornoConsultaInformacoesCadastrais.id = id;
        }
        return this;
    }

    /// <summary>
    /// Define a data e hora de processamento
    /// </summary>
    public RetInfoCadastralBuilder WithDataHoraProcessamento(DateTime dataHora)
    {
        if (_consulta.retornoConsultaInformacoesCadastrais != null)
        {
            _consulta.retornoConsultaInformacoesCadastrais.dataHoraProcessamento = dataHora;
        }
        return this;
    }

    /// <summary>
    /// Define o número do recibo
    /// </summary>
    public RetInfoCadastralBuilder WithNumeroRecibo(string numeroRecibo)
    {
        if (_consulta.retornoConsultaInformacoesCadastrais != null)
        {
            _consulta.retornoConsultaInformacoesCadastrais.numeroRecibo = numeroRecibo;
        }
        return this;
    }

    /// <summary>
    /// Define o status da consulta
    /// </summary>
    public RetInfoCadastralBuilder WithStatus(Action<StatusBuilder> configureStatus)
    {
        var builder = new StatusBuilder();
        configureStatus(builder);

        if (_consulta.retornoConsultaInformacoesCadastrais != null)
        {
            _consulta.retornoConsultaInformacoesCadastrais.status = builder.Build();
        }
        return this;
    }

    /// <summary>
    /// Define a identificação da empresa declarante
    /// </summary>
    public RetInfoCadastralBuilder WithEmpresaDeclarante(Action<EmpresaDeclaranteBuilder> configureEmpresa)
    {
        var builder = new EmpresaDeclaranteBuilder();
        configureEmpresa(builder);

        if (_consulta.retornoConsultaInformacoesCadastrais != null)
        {
            _consulta.retornoConsultaInformacoesCadastrais.identificacaoEmpresaDeclarante = builder.Build();
        }
        return this;
    }

    /// <summary>
    /// Define as informações cadastrais
    /// </summary>
    public RetInfoCadastralBuilder WithInformacoesCadastrais(Action<InformacoesCadastraisBuilder> configureInfo)
    {
        var builder = new InformacoesCadastraisBuilder();
        configureInfo(builder);

        if (_consulta.retornoConsultaInformacoesCadastrais != null)
        {
            _consulta.retornoConsultaInformacoesCadastrais.informacoesCadastrais = builder.Build();
        }
        return this;
    }

    /// <inheritdoc />
    public RetInfoCadastralMessage Build()
    {
        ValidateRequiredFields();
        return new RetInfoCadastralMessage(_consulta, _version);
    }

    private void ValidateRequiredFields()
    {
        if (_consulta.retornoConsultaInformacoesCadastrais == null)
            throw new InvalidOperationException("RetornoConsultaInformacoesCadastrais é obrigatório");

        if (string.IsNullOrWhiteSpace(_consulta.retornoConsultaInformacoesCadastrais.id))
            throw new InvalidOperationException("Id é obrigatório");

        if (string.IsNullOrWhiteSpace(_consulta.retornoConsultaInformacoesCadastrais.numeroRecibo))
            throw new InvalidOperationException("Número do recibo é obrigatório");
    }

    private static string GenerateId() => $"ID_{Guid.NewGuid():N}";
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
/// Builder para configuração das informações cadastrais
/// </summary>
public sealed class InformacoesCadastraisBuilder
{
    private readonly TInformacoesCadastrais _info = new();

    public InformacoesCadastraisBuilder WithCnpj(string cnpj)
    {
        _info.cnpj = cnpj;
        return this;
    }

    public InformacoesCadastraisBuilder WithGiin(string giin)
    {
        _info.giin = giin;
        return this;
    }

    public InformacoesCadastraisBuilder WithNome(string nome)
    {
        _info.nome = nome;
        return this;
    }

    public InformacoesCadastraisBuilder WithEndereco(string endereco)
    {
        _info.endereco = endereco;
        return this;
    }

    public InformacoesCadastraisBuilder WithMunicipio(uint municipio)
    {
        _info.municipio = municipio;
        return this;
    }

    public InformacoesCadastraisBuilder WithUf(string uf)
    {
        _info.uf = uf;
        return this;
    }

    internal TInformacoesCadastrais Build() => _info;
}

#endregion

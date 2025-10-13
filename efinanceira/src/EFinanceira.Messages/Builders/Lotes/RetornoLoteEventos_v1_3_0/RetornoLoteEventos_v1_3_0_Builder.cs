using System.Xml;
using System.Xml.Serialization;
using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Generated.Lotes.RetornoLoteEventos_v1_3_0;

namespace EFinanceira.Messages.Builders.Lotes.RetornoLoteEventos_v1_3_0;

/// <summary>
/// Mensagem para retorno de evento e-Financeira v1.3.0
/// </summary>
public sealed class RetornoLoteEventos_v1_3_0_Message : IEFinanceiraMessage
{
    public string Version { get; }
    public string RootElementName => "eFinanceira";
    public string? IdAttributeName => null;
    public string? IdValue => null;
    public object Payload => EFinanceira;

    /// <summary>
    /// Objeto raiz gerado do XSD v1.3.0
    /// </summary>
    public eFinanceira EFinanceira { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RetornoLoteEventos_v1_3_0_Message"/> class for serialization.
    /// </summary>
    public RetornoLoteEventos_v1_3_0_Message()
    {
        Version = "v1_3_0";
        EFinanceira = new eFinanceira();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RetornoLoteEventos_v1_3_0_Message"/> class.
    /// </summary>
    /// <param name="version">Versão do esquema</param>
    /// <param name="eFinanceira">Objeto raiz do XSD</param>
    public RetornoLoteEventos_v1_3_0_Message(string version, eFinanceira eFinanceira)
    {
        Version = version;
        EFinanceira = eFinanceira;
    }
}

/// <summary>
/// Builder para construção e interpretação de retorno de evento e-Financeira v1.3.0
/// </summary>
public sealed class RetornoLoteEventos_v1_3_0_Builder : IMessageBuilder<RetornoLoteEventos_v1_3_0_Message>
{
    private readonly string _version;
    private readonly eFinanceira _eFinanceira;
    private readonly eFinanceiraRetornoEvento _retornoEvento;

    /// <summary>
    /// Initializes a new instance of the <see cref="RetornoLoteEventos_v1_3_0_Builder"/> class.
    /// </summary>
    /// <param name="version">Versão do esquema (padrão: v1_3_0)</param>
    public RetornoLoteEventos_v1_3_0_Builder(string version = "v1_3_0")
    {
        _version = version;
        _eFinanceira = new eFinanceira();
        _retornoEvento = new eFinanceiraRetornoEvento();
        _eFinanceira.retornoEvento = _retornoEvento;
    }

    /// <summary>
    /// Define a identificação da empresa declarante
    /// </summary>
    /// <param name="cnpjEmpresaDeclarante">CNPJ da empresa declarante</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventos_v1_3_0_Builder ComEmpresaDeclarante(string cnpjEmpresaDeclarante)
    {
        ArgumentNullException.ThrowIfNull(cnpjEmpresaDeclarante);

        _retornoEvento.identificacaoEmpresaDeclarante = new TIdeEmpresaDeclarante
        {
            cnpjEmpresaDeclarante = cnpjEmpresaDeclarante
        };
        return this;
    }

    /// <summary>
    /// Define os dados de recepção do evento
    /// </summary>
    /// <param name="dhRecepcao">Data/hora de recepção</param>
    /// <param name="dhProcessamento">Data/hora de processamento</param>
    /// <param name="tipoEvento">Tipo do evento</param>
    /// <param name="idEvento">ID do evento</param>
    /// <param name="hash">Hash do evento</param>
    /// <param name="nrRecibo">Número do recibo</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventos_v1_3_0_Builder ComDadosRecepcao(
        DateTime dhRecepcao, 
        DateTime dhProcessamento, 
        string tipoEvento, 
        string idEvento, 
        string hash, 
        string nrRecibo)
    {
        ArgumentNullException.ThrowIfNull(tipoEvento);
        ArgumentNullException.ThrowIfNull(idEvento);
        ArgumentNullException.ThrowIfNull(hash);
        ArgumentNullException.ThrowIfNull(nrRecibo);

        _retornoEvento.dadosRecepcaoEvento = new TDadosRecepcaoEvento
        {
            dhRecepcao = dhRecepcao,
            dhProcessamento = dhProcessamento,
            tipoEvento = tipoEvento,
            idEvento = idEvento,
            hash = hash,
            nrRecibo = nrRecibo
        };
        return this;
    }

    /// <summary>
    /// Define o status do processamento do evento
    /// </summary>
    /// <param name="codigoRetorno">Código do retorno</param>
    /// <param name="descricaoRetorno">Descrição do retorno</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventos_v1_3_0_Builder ComStatus(string codigoRetorno, string? descricaoRetorno = null)
    {
        ArgumentNullException.ThrowIfNull(codigoRetorno);

        _retornoEvento.status = new TStatus
        {
            cdRetorno = codigoRetorno,
            descRetorno = descricaoRetorno
        };
        return this;
    }

    /// <summary>
    /// Adiciona uma ocorrência/erro ao status
    /// </summary>
    /// <param name="tipo">Tipo da ocorrência (1=Erro, 2=Aviso)</param>
    /// <param name="codigo">Código da ocorrência</param>
    /// <param name="descricao">Descrição da ocorrência</param>
    /// <param name="localizacao">Localização do erro/aviso (opcional)</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventos_v1_3_0_Builder AdicionarOcorrencia(string tipo, string codigo, string descricao, string? localizacao = null)
    {
        ArgumentNullException.ThrowIfNull(tipo);
        ArgumentNullException.ThrowIfNull(codigo);
        ArgumentNullException.ThrowIfNull(descricao);

        // Garante que o status existe
        _retornoEvento.status ??= new TStatus();

        var ocorrencia = new TRegistroOcorrenciasOcorrencias
        {
            tipo = tipo,
            codigo = codigo,
            descricao = descricao,
            localizacaoErroAviso = localizacao
        };

        var ocorrenciasExistentes = _retornoEvento.status.dadosRegistroOcorrenciaEvento?.ToList() ?? new List<TRegistroOcorrenciasOcorrencias>();
        ocorrenciasExistentes.Add(ocorrencia);
        _retornoEvento.status.dadosRegistroOcorrenciaEvento = ocorrenciasExistentes.ToArray();

        return this;
    }

    /// <summary>
    /// Adiciona múltiplas ocorrências ao status
    /// </summary>
    /// <param name="ocorrencias">Lista de ocorrências</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventos_v1_3_0_Builder AdicionarOcorrencias(IEnumerable<TRegistroOcorrenciasOcorrencias> ocorrencias)
    {
        ArgumentNullException.ThrowIfNull(ocorrencias);

        foreach (var ocorrencia in ocorrencias)
        {
            AdicionarOcorrencia(ocorrencia.tipo, ocorrencia.codigo, ocorrencia.descricao, ocorrencia.localizacaoErroAviso);
        }

        return this;
    }

    /// <summary>
    /// Define os dados do recibo de entrega
    /// </summary>
    /// <param name="nrRecibo">Número do recibo</param>
    /// <param name="dhEntrega">Data/hora de entrega (opcional)</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventos_v1_3_0_Builder ComReciboEntrega(string nrRecibo, DateTime? dhEntrega = null)
    {
        ArgumentNullException.ThrowIfNull(nrRecibo);

        _retornoEvento.dadosReciboEntrega = new TDadosReciboEntrega
        {
            numeroRecibo = nrRecibo
        };

        // Note: dhEntrega property doesn't exist in v1.3.0 schema
        // This parameter is kept for API compatibility but not used
        return this;
    }

    /// <summary>
    /// Define o ID do evento
    /// </summary>
    /// <param name="id">ID do evento</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventos_v1_3_0_Builder ComId(string id)
    {
        ArgumentNullException.ThrowIfNull(id);
        _retornoEvento.id = id;
        return this;
    }

    /// <summary>
    /// Define a assinatura digital
    /// </summary>
    /// <param name="signature">Assinatura digital</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventos_v1_3_0_Builder ComAssinatura(SignatureType signature)
    {
        ArgumentNullException.ThrowIfNull(signature);
        _eFinanceira.Signature = signature;
        return this;
    }

    /// <summary>
    /// Remove todas as ocorrências do status
    /// </summary>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventos_v1_3_0_Builder LimparOcorrencias()
    {
        if (_retornoEvento.status != null)
        {
            _retornoEvento.status.dadosRegistroOcorrenciaEvento = null;
        }
        return this;
    }

    /// <summary>
    /// Constrói a mensagem final
    /// </summary>
    /// <returns>Mensagem RetornoLoteEventos v1.3.0 construída</returns>
    public RetornoLoteEventos_v1_3_0_Message Build()
    {
        return new RetornoLoteEventos_v1_3_0_Message(_version, _eFinanceira);
    }

    /// <summary>
    /// Cria um builder a partir de XML existente
    /// </summary>
    /// <param name="xmlContent">Conteúdo XML do retorno</param>
    /// <param name="version">Versão do esquema</param>
    /// <returns>Builder preenchido com os dados do XML</returns>
    public static RetornoLoteEventos_v1_3_0_Builder FromXml(string xmlContent, string version = "v1_3_0")
    {
        ArgumentNullException.ThrowIfNull(xmlContent);

        var serializer = new XmlSerializer(typeof(eFinanceira), "http://www.eFinanceira.gov.br/schemas/retornoEvento/v1_3_0");
        
        using var reader = new StringReader(xmlContent);
        using var xmlReader = XmlReader.Create(reader, new XmlReaderSettings 
        { 
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null
        });
        
        var deserializedObject = serializer.Deserialize(xmlReader);
        if (deserializedObject is not eFinanceira eFinanceira)
        {
            throw new InvalidOperationException("Falha ao deserializar o XML para eFinanceira v1.3.0");
        }

        var builder = new RetornoLoteEventos_v1_3_0_Builder(version);
        builder._eFinanceira.retornoEvento = eFinanceira.retornoEvento;
        builder._eFinanceira.Signature = eFinanceira.Signature;

        return builder;
    }

    /// <summary>
    /// Cria um builder a partir de um objeto eFinanceira existente
    /// </summary>
    /// <param name="eFinanceira">Objeto eFinanceira</param>
    /// <param name="version">Versão do esquema</param>
    /// <returns>Builder preenchido com os dados do objeto</returns>
    public static RetornoLoteEventos_v1_3_0_Builder FromEFinanceira(eFinanceira eFinanceira, string version = "v1_3_0")
    {
        ArgumentNullException.ThrowIfNull(eFinanceira);

        var builder = new RetornoLoteEventos_v1_3_0_Builder(version);
        builder._eFinanceira.retornoEvento = eFinanceira.retornoEvento;
        builder._eFinanceira.Signature = eFinanceira.Signature;

        return builder;
    }
}

/// <summary>
/// Extensões para facilitar o uso do RetornoLoteEventos_v1_3_0_Builder
/// </summary>
public static class RetornoLoteEventos_v1_3_0_BuilderExtensions
{
    /// <summary>
    /// Adiciona uma ocorrência de erro
    /// </summary>
    /// <param name="builder">Builder</param>
    /// <param name="codigo">Código do erro</param>
    /// <param name="descricao">Descrição do erro</param>
    /// <param name="localizacao">Localização do erro</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public static RetornoLoteEventos_v1_3_0_Builder ComErro(this RetornoLoteEventos_v1_3_0_Builder builder, string codigo, string descricao, string? localizacao = null)
    {
        return builder.AdicionarOcorrencia("1", codigo, descricao, localizacao);
    }

    /// <summary>
    /// Adiciona uma ocorrência de aviso
    /// </summary>
    /// <param name="builder">Builder</param>
    /// <param name="codigo">Código do aviso</param>
    /// <param name="descricao">Descrição do aviso</param>
    /// <param name="localizacao">Localização do aviso</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public static RetornoLoteEventos_v1_3_0_Builder ComAviso(this RetornoLoteEventos_v1_3_0_Builder builder, string codigo, string descricao, string? localizacao = null)
    {
        return builder.AdicionarOcorrencia("2", codigo, descricao, localizacao);
    }

    /// <summary>
    /// Define um status de sucesso
    /// </summary>
    /// <param name="builder">Builder</param>
    /// <param name="descricao">Descrição opcional</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public static RetornoLoteEventos_v1_3_0_Builder ComSucesso(this RetornoLoteEventos_v1_3_0_Builder builder, string? descricao = "Processado com sucesso")
    {
        return builder.ComStatus("0", descricao);
    }

    /// <summary>
    /// Define um status de erro
    /// </summary>
    /// <param name="builder">Builder</param>
    /// <param name="codigoErro">Código do erro</param>
    /// <param name="descricao">Descrição do erro</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public static RetornoLoteEventos_v1_3_0_Builder ComErroProcessamento(this RetornoLoteEventos_v1_3_0_Builder builder, string codigoErro, string descricao)
    {
        return builder.ComStatus(codigoErro, descricao);
    }

    /// <summary>
    /// Verifica se o retorno indica sucesso
    /// </summary>
    /// <param name="message">Mensagem de retorno</param>
    /// <returns>True se o processamento foi bem-sucedido</returns>
    public static bool IsSuccessful(this RetornoLoteEventos_v1_3_0_Message message)
    {
        return message.EFinanceira.retornoEvento?.status?.cdRetorno == "0";
    }

    /// <summary>
    /// Obtém todas as ocorrências de erro
    /// </summary>
    /// <param name="message">Mensagem de retorno</param>
    /// <returns>Lista de ocorrências de erro</returns>
    public static IEnumerable<TRegistroOcorrenciasOcorrencias> GetErros(this RetornoLoteEventos_v1_3_0_Message message)
    {
        var ocorrencias = message.EFinanceira.retornoEvento?.status?.dadosRegistroOcorrenciaEvento;
        return ocorrencias?.Where(o => o.tipo == "1") ?? Enumerable.Empty<TRegistroOcorrenciasOcorrencias>();
    }

    /// <summary>
    /// Obtém todas as ocorrências de aviso
    /// </summary>
    /// <param name="message">Mensagem de retorno</param>
    /// <returns>Lista de ocorrências de aviso</returns>
    public static IEnumerable<TRegistroOcorrenciasOcorrencias> GetAvisos(this RetornoLoteEventos_v1_3_0_Message message)
    {
        var ocorrencias = message.EFinanceira.retornoEvento?.status?.dadosRegistroOcorrenciaEvento;
        return ocorrencias?.Where(o => o.tipo == "2") ?? Enumerable.Empty<TRegistroOcorrenciasOcorrencias>();
    }

    /// <summary>
    /// Obtém os dados de recepção do evento
    /// </summary>
    /// <param name="message">Mensagem de retorno</param>
    /// <returns>Dados de recepção do evento</returns>
    public static TDadosRecepcaoEvento? GetDadosRecepcao(this RetornoLoteEventos_v1_3_0_Message message)
    {
        return message.EFinanceira.retornoEvento?.dadosRecepcaoEvento;
    }

    /// <summary>
    /// Obtém os dados do recibo de entrega
    /// </summary>
    /// <param name="message">Mensagem de retorno</param>
    /// <returns>Dados do recibo de entrega</returns>
    public static TDadosReciboEntrega? GetReciboEntrega(this RetornoLoteEventos_v1_3_0_Message message)
    {
        return message.EFinanceira.retornoEvento?.dadosReciboEntrega;
    }

    /// <summary>
    /// Obtém o CNPJ da empresa declarante
    /// </summary>
    /// <param name="message">Mensagem de retorno</param>
    /// <returns>CNPJ da empresa declarante</returns>
    public static string? GetCnpjEmpresaDeclarante(this RetornoLoteEventos_v1_3_0_Message message)
    {
        return message.EFinanceira.retornoEvento?.identificacaoEmpresaDeclarante?.cnpjEmpresaDeclarante;
    }
}
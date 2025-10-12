using System.Xml;
using System.Xml.Serialization;
using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Generated.Lotes.RetornoLoteEventos_v1_2_0;

namespace EFinanceira.Messages.Builders.Lotes.RetornoLoteEventos;

/// <summary>
/// Mensagem para retorno de lote de eventos e-Financeira
/// </summary>
public sealed class RetornoLoteEventosMessage : IEFinanceiraMessage
{
    public string Version { get; }
    public string RootElementName => "eFinanceira";
    public string? IdAttributeName => null;
    public string? IdValue => null;
    public object Payload => EFinanceira;

    /// <summary>
    /// Objeto raiz gerado do XSD
    /// </summary>
    public eFinanceira EFinanceira { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RetornoLoteEventosMessage"/> class for serialization.
    /// </summary>
    public RetornoLoteEventosMessage()
    {
        Version = "v1_2_0";
        EFinanceira = new eFinanceira();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RetornoLoteEventosMessage"/> class.
    /// </summary>
    /// <param name="version">Versão do esquema</param>
    /// <param name="eFinanceira">Objeto raiz do XSD</param>
    public RetornoLoteEventosMessage(string version, eFinanceira eFinanceira)
    {
        Version = version;
        EFinanceira = eFinanceira;
    }
}

/// <summary>
/// Builder para construção e interpretação de retorno de lote de eventos e-Financeira
/// </summary>
public sealed class RetornoLoteEventosBuilder : IMessageBuilder<RetornoLoteEventosMessage>
{
    private readonly string _version;
    private readonly eFinanceira _eFinanceira;
    private readonly eFinanceiraRetornoLoteEventos _retornoLoteEventos;

    /// <summary>
    /// Initializes a new instance of the <see cref="RetornoLoteEventosBuilder"/> class.
    /// </summary>
    /// <param name="version">Versão do esquema (padrão: v1_2_0)</param>
    public RetornoLoteEventosBuilder(string version = "v1_2_0")
    {
        _version = version;
        _eFinanceira = new eFinanceira();
        _retornoLoteEventos = new eFinanceiraRetornoLoteEventos();
        _eFinanceira.retornoLoteEventos = _retornoLoteEventos;
    }

    /// <summary>
    /// Define o identificador do transmissor
    /// </summary>
    /// <param name="idTransmissor">ID do transmissor</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventosBuilder ComIdTransmissor(string idTransmissor)
    {
        ArgumentNullException.ThrowIfNull(idTransmissor);

        _retornoLoteEventos.ideTransmissor = new TIdeTransmissor
        {
            IdTransmissor = idTransmissor
        };
        return this;
    }

    /// <summary>
    /// Define o status do processamento do lote
    /// </summary>
    /// <param name="codigoStatus">Código do status</param>
    /// <param name="descricaoRetorno">Descrição do retorno</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventosBuilder ComStatus(string codigoStatus, string? descricaoRetorno = null)
    {
        ArgumentNullException.ThrowIfNull(codigoStatus);

        _retornoLoteEventos.status = new TStatus
        {
            cdStatus = codigoStatus,
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
    public RetornoLoteEventosBuilder AdicionarOcorrencia(string tipo, string codigo, string descricao, string? localizacao = null)
    {
        ArgumentNullException.ThrowIfNull(tipo);
        ArgumentNullException.ThrowIfNull(codigo);
        ArgumentNullException.ThrowIfNull(descricao);

        // Garante que o status existe
        _retornoLoteEventos.status ??= new TStatus();

        var ocorrencia = new TRegistroOcorrenciasOcorrencias
        {
            tipo = tipo,
            codigo = codigo,
            descricao = descricao,
            localizacaoErroAviso = localizacao
        };

        var ocorrenciasExistentes = _retornoLoteEventos.status.dadosRegistroOcorrenciaLote?.ToList() ?? new List<TRegistroOcorrenciasOcorrencias>();
        ocorrenciasExistentes.Add(ocorrencia);
        _retornoLoteEventos.status.dadosRegistroOcorrenciaLote = ocorrenciasExistentes.ToArray();

        return this;
    }

    /// <summary>
    /// Adiciona múltiplas ocorrências ao status
    /// </summary>
    /// <param name="ocorrencias">Lista de ocorrências</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventosBuilder AdicionarOcorrencias(IEnumerable<TRegistroOcorrenciasOcorrencias> ocorrencias)
    {
        ArgumentNullException.ThrowIfNull(ocorrencias);

        foreach (var ocorrencia in ocorrencias)
        {
            AdicionarOcorrencia(ocorrencia.tipo, ocorrencia.codigo, ocorrencia.descricao, ocorrencia.localizacaoErroAviso);
        }

        return this;
    }

    /// <summary>
    /// Define os eventos retornados no lote
    /// </summary>
    /// <param name="eventos">Array de eventos</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventosBuilder ComEventos(params TArquivoeFinanceira[] eventos)
    {
        ArgumentNullException.ThrowIfNull(eventos);

        _retornoLoteEventos.retornoEventos = new eFinanceiraRetornoLoteEventosRetornoEventos
        {
            evento = eventos
        };
        return this;
    }

    /// <summary>
    /// Adiciona um evento ao retorno
    /// </summary>
    /// <param name="eventoXml">Conteúdo XML do evento</param>
    /// <param name="eventoId">ID do evento</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventosBuilder AdicionarEvento(XmlElement eventoXml, string? eventoId = null)
    {
        ArgumentNullException.ThrowIfNull(eventoXml);

        var evento = new TArquivoeFinanceira
        {
            Any = eventoXml,
            id = eventoId
        };

        // Garante que retornoEventos existe
        _retornoLoteEventos.retornoEventos ??= new eFinanceiraRetornoLoteEventosRetornoEventos();

        var eventosExistentes = _retornoLoteEventos.retornoEventos.evento?.ToList() ?? new List<TArquivoeFinanceira>();
        eventosExistentes.Add(evento);
        _retornoLoteEventos.retornoEventos.evento = eventosExistentes.ToArray();

        return this;
    }

    /// <summary>
    /// Adiciona um evento ao retorno usando XML string
    /// </summary>
    /// <param name="eventoXmlString">String XML do evento</param>
    /// <param name="eventoId">ID do evento</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventosBuilder AdicionarEvento(string eventoXmlString, string? eventoId = null)
    {
        ArgumentNullException.ThrowIfNull(eventoXmlString);

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(eventoXmlString);

        if (xmlDoc.DocumentElement == null)
            throw new ArgumentException("XML inválido fornecido", nameof(eventoXmlString));

        return AdicionarEvento(xmlDoc.DocumentElement, eventoId);
    }

    /// <summary>
    /// Define o ID do lote
    /// </summary>
    /// <param name="id">ID do lote</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventosBuilder ComId(string id)
    {
        ArgumentNullException.ThrowIfNull(id);
        _retornoLoteEventos.id = id;
        return this;
    }

    /// <summary>
    /// Define a assinatura digital
    /// </summary>
    /// <param name="signature">Assinatura digital</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventosBuilder ComAssinatura(SignatureType signature)
    {
        ArgumentNullException.ThrowIfNull(signature);
        _eFinanceira.Signature = signature;
        return this;
    }

    /// <summary>
    /// Remove todos os eventos do retorno
    /// </summary>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventosBuilder LimparEventos()
    {
        if (_retornoLoteEventos.retornoEventos != null)
        {
            _retornoLoteEventos.retornoEventos.evento = null;
        }
        return this;
    }

    /// <summary>
    /// Remove todas as ocorrências do status
    /// </summary>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventosBuilder LimparOcorrencias()
    {
        if (_retornoLoteEventos.status != null)
        {
            _retornoLoteEventos.status.dadosRegistroOcorrenciaLote = null;
        }
        return this;
    }

    /// <summary>
    /// Constrói a mensagem final
    /// </summary>
    /// <returns>Mensagem RetornoLoteEventos construída</returns>
    public RetornoLoteEventosMessage Build()
    {
        return new RetornoLoteEventosMessage(_version, _eFinanceira);
    }

    /// <summary>
    /// Cria um builder a partir de XML existente
    /// </summary>
    /// <param name="xmlContent">Conteúdo XML do retorno</param>
    /// <param name="version">Versão do esquema</param>
    /// <returns>Builder preenchido com os dados do XML</returns>
    public static RetornoLoteEventosBuilder FromXml(string xmlContent, string version = "v1_2_0")
    {
        ArgumentNullException.ThrowIfNull(xmlContent);

        var serializer = new XmlSerializer(typeof(eFinanceira), "http://www.eFinanceira.gov.br/schemas/retornoLoteEventos/v1_2_0");

        using var reader = new StringReader(xmlContent);
        using var xmlReader = XmlReader.Create(reader, new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null
        });

        var deserializedObject = serializer.Deserialize(xmlReader);
        if (deserializedObject is not eFinanceira eFinanceira)
        {
            throw new InvalidOperationException("Falha ao deserializar o XML para eFinanceira");
        }

        var builder = new RetornoLoteEventosBuilder(version);
        builder._eFinanceira.retornoLoteEventos = eFinanceira.retornoLoteEventos;
        builder._eFinanceira.Signature = eFinanceira.Signature;

        return builder;
    }

    /// <summary>
    /// Cria um builder a partir de um objeto eFinanceira existente
    /// </summary>
    /// <param name="eFinanceira">Objeto eFinanceira</param>
    /// <param name="version">Versão do esquema</param>
    /// <returns>Builder preenchido com os dados do objeto</returns>
    public static RetornoLoteEventosBuilder FromEFinanceira(eFinanceira eFinanceira, string version = "v1_2_0")
    {
        ArgumentNullException.ThrowIfNull(eFinanceira);

        var builder = new RetornoLoteEventosBuilder(version);
        builder._eFinanceira.retornoLoteEventos = eFinanceira.retornoLoteEventos;
        builder._eFinanceira.Signature = eFinanceira.Signature;

        return builder;
    }
}

/// <summary>
/// Extensões para facilitar o uso do RetornoLoteEventosBuilder
/// </summary>
public static class RetornoLoteEventosBuilderExtensions
{
    /// <summary>
    /// Adiciona uma ocorrência de erro
    /// </summary>
    /// <param name="builder">Builder</param>
    /// <param name="codigo">Código do erro</param>
    /// <param name="descricao">Descrição do erro</param>
    /// <param name="localizacao">Localização do erro</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public static RetornoLoteEventosBuilder ComErro(this RetornoLoteEventosBuilder builder, string codigo, string descricao, string? localizacao = null)
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
    public static RetornoLoteEventosBuilder ComAviso(this RetornoLoteEventosBuilder builder, string codigo, string descricao, string? localizacao = null)
    {
        return builder.AdicionarOcorrencia("2", codigo, descricao, localizacao);
    }

    /// <summary>
    /// Define um status de sucesso
    /// </summary>
    /// <param name="builder">Builder</param>
    /// <param name="descricao">Descrição opcional</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public static RetornoLoteEventosBuilder ComSucesso(this RetornoLoteEventosBuilder builder, string? descricao = "Processado com sucesso")
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
    public static RetornoLoteEventosBuilder ComErroProcessamento(this RetornoLoteEventosBuilder builder, string codigoErro, string descricao)
    {
        return builder.ComStatus(codigoErro, descricao);
    }

    /// <summary>
    /// Verifica se o retorno indica sucesso
    /// </summary>
    /// <param name="message">Mensagem de retorno</param>
    /// <returns>True se o processamento foi bem-sucedido</returns>
    public static bool IsSuccessful(this RetornoLoteEventosMessage message)
    {
        return message.EFinanceira.retornoLoteEventos?.status?.cdStatus == "0";
    }

    /// <summary>
    /// Obtém todas as ocorrências de erro
    /// </summary>
    /// <param name="message">Mensagem de retorno</param>
    /// <returns>Lista de ocorrências de erro</returns>
    public static IEnumerable<TRegistroOcorrenciasOcorrencias> GetErros(this RetornoLoteEventosMessage message)
    {
        var ocorrencias = message.EFinanceira.retornoLoteEventos?.status?.dadosRegistroOcorrenciaLote;
        return ocorrencias?.Where(o => o.tipo == "1") ?? Enumerable.Empty<TRegistroOcorrenciasOcorrencias>();
    }

    /// <summary>
    /// Obtém todas as ocorrências de aviso
    /// </summary>
    /// <param name="message">Mensagem de retorno</param>
    /// <returns>Lista de ocorrências de aviso</returns>
    public static IEnumerable<TRegistroOcorrenciasOcorrencias> GetAvisos(this RetornoLoteEventosMessage message)
    {
        var ocorrencias = message.EFinanceira.retornoLoteEventos?.status?.dadosRegistroOcorrenciaLote;
        return ocorrencias?.Where(o => o.tipo == "2") ?? Enumerable.Empty<TRegistroOcorrenciasOcorrencias>();
    }

    /// <summary>
    /// Obtém todos os eventos retornados
    /// </summary>
    /// <param name="message">Mensagem de retorno</param>
    /// <returns>Lista de eventos</returns>
    public static IEnumerable<TArquivoeFinanceira> GetEventos(this RetornoLoteEventosMessage message)
    {
        var eventos = message.EFinanceira.retornoLoteEventos?.retornoEventos?.evento;
        return eventos ?? Enumerable.Empty<TArquivoeFinanceira>();
    }
}
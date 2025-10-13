using System.Xml;
using System.Xml.Serialization;
using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Generated.Lotes.RetornoLoteEventosAssincrono;

namespace EFinanceira.Messages.Builders.Lotes.RetornoLoteEventosAssincrono;

/// <summary>
/// Mensagem para retorno de lote de eventos assíncrono e-Financeira v1.0.0
/// </summary>
public sealed class RetornoLoteEventosAssincronoMessage : IEFinanceiraMessage
{
    public string Version { get; }
    public string RootElementName => "eFinanceira";
    public string? IdAttributeName => "id";
    public string? IdValue => EFinanceira.retornoLoteEventosAssincrono?.id;
    public object Payload => EFinanceira;

    /// <summary>
    /// Objeto raiz gerado do XSD v1.0.0
    /// </summary>
    public eFinanceira EFinanceira { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RetornoLoteEventosAssincronoMessage"/> class for serialization.
    /// </summary>
    public RetornoLoteEventosAssincronoMessage()
    {
        Version = "v1_0_0";
        EFinanceira = new eFinanceira();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RetornoLoteEventosAssincronoMessage"/> class.
    /// </summary>
    /// <param name="version">Versão do esquema</param>
    /// <param name="eFinanceira">Objeto raiz do XSD</param>
    public RetornoLoteEventosAssincronoMessage(string version, eFinanceira eFinanceira)
    {
        Version = version;
        EFinanceira = eFinanceira;
    }
}

/// <summary>
/// Builder para construção e interpretação de retorno de lote de eventos assíncrono e-Financeira v1.0.0
/// </summary>
public sealed class RetornoLoteEventosAssincronoBuilder : IMessageBuilder<RetornoLoteEventosAssincronoMessage>
{
    private readonly string _version;
    private readonly eFinanceira _eFinanceira;
    private readonly eFinanceiraRetornoLoteEventosAssincrono _retornoLoteEventosAssincrono;

    /// <summary>
    /// Initializes a new instance of the <see cref="RetornoLoteEventosAssincronoBuilder"/> class.
    /// </summary>
    /// <param name="version">Versão do esquema (padrão: v1_0_0)</param>
    public RetornoLoteEventosAssincronoBuilder(string version = "v1_0_0")
    {
        _version = version;
        _eFinanceira = new eFinanceira();
        _retornoLoteEventosAssincrono = new eFinanceiraRetornoLoteEventosAssincrono();
        _eFinanceira.retornoLoteEventosAssincrono = _retornoLoteEventosAssincrono;
    }

    /// <summary>
    /// Define o CNPJ do declarante
    /// </summary>
    /// <param name="cnpjDeclarante">CNPJ do declarante</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventosAssincronoBuilder ComCnpjDeclarante(string cnpjDeclarante)
    {
        ArgumentNullException.ThrowIfNull(cnpjDeclarante);
        _retornoLoteEventosAssincrono.cnpjDeclarante = cnpjDeclarante;
        return this;
    }

    /// <summary>
    /// Define o status do processamento
    /// </summary>
    /// <param name="codigoResposta">Código da resposta</param>
    /// <param name="descricaoResposta">Descrição da resposta</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventosAssincronoBuilder ComStatus(int codigoResposta, string? descricaoResposta = null)
    {
        _retornoLoteEventosAssincrono.status = new TStatus
        {
            cdResposta = codigoResposta,
            descResposta = descricaoResposta
        };
        return this;
    }

    /// <summary>
    /// Define os dados de recepção do lote
    /// </summary>
    /// <param name="dhRecepcao">Data/hora de recepção</param>
    /// <param name="versaoAplicativoRecepcao">Versão do aplicativo de recepção</param>
    /// <param name="protocoloEnvio">Protocolo de envio</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventosAssincronoBuilder ComDadosRecepcao(
        DateTime dhRecepcao,
        string versaoAplicativoRecepcao,
        string protocoloEnvio)
    {
        ArgumentNullException.ThrowIfNull(versaoAplicativoRecepcao);
        ArgumentNullException.ThrowIfNull(protocoloEnvio);

        _retornoLoteEventosAssincrono.dadosRecepcaoLote = new TDadosRecepcao
        {
            dhRecepcao = dhRecepcao,
            versaoAplicativoRecepcao = versaoAplicativoRecepcao,
            protocoloEnvio = protocoloEnvio
        };
        return this;
    }

    /// <summary>
    /// Define os dados de processamento do lote
    /// </summary>
    /// <param name="dhProcessamento">Data/hora de processamento</param>
    /// <param name="versaoAplicativoProcessamento">Versão do aplicativo de processamento</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventosAssincronoBuilder ComDadosProcessamento(
        DateTime dhProcessamento,
        string versaoAplicativoProcessamento)
    {
        ArgumentNullException.ThrowIfNull(versaoAplicativoProcessamento);

        _retornoLoteEventosAssincrono.dadosProcessamentoLote = new TDadosProcessamento
        {
            dhProcessamento = dhProcessamento,
            versaoAplicativoProcessamentoLote = versaoAplicativoProcessamento
        };
        return this;
    }

    /// <summary>
    /// Adiciona uma ocorrência ao status
    /// </summary>
    /// <param name="codigo">Código da ocorrência</param>
    /// <param name="descricao">Descrição da ocorrência</param>
    /// <param name="tipo">Tipo da ocorrência (1=Erro, 2=Aviso)</param>
    /// <param name="localizacao">Localização do erro/aviso (opcional)</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventosAssincronoBuilder AdicionarOcorrencia(string codigo, string descricao, byte tipo, string? localizacao = null)
    {
        ArgumentNullException.ThrowIfNull(codigo);
        ArgumentNullException.ThrowIfNull(descricao);

        // Garante que o status existe
        _retornoLoteEventosAssincrono.status ??= new TStatus();

        var ocorrencia = new TOcorrenciasOcorrencia
        {
            codigo = codigo,
            descricao = descricao,
            tipo = tipo,
            localizacao = localizacao
        };

        var ocorrenciasExistentes = _retornoLoteEventosAssincrono.status.ocorrencias?.ToList() ?? new List<TOcorrenciasOcorrencia>();
        ocorrenciasExistentes.Add(ocorrencia);
        _retornoLoteEventosAssincrono.status.ocorrencias = ocorrenciasExistentes.ToArray();

        return this;
    }

    /// <summary>
    /// Adiciona múltiplas ocorrências ao status
    /// </summary>
    /// <param name="ocorrencias">Lista de ocorrências</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventosAssincronoBuilder AdicionarOcorrencias(IEnumerable<TOcorrenciasOcorrencia> ocorrencias)
    {
        ArgumentNullException.ThrowIfNull(ocorrencias);

        foreach (var ocorrencia in ocorrencias)
        {
            AdicionarOcorrencia(ocorrencia.codigo, ocorrencia.descricao, ocorrencia.tipo, ocorrencia.localizacao);
        }

        return this;
    }

    /// <summary>
    /// Adiciona um evento ao retorno
    /// </summary>
    /// <param name="eventoXml">XML do evento como XmlElement</param>
    /// <param name="id">ID do evento</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventosAssincronoBuilder AdicionarEvento(XmlElement eventoXml, string id)
    {
        ArgumentNullException.ThrowIfNull(eventoXml);
        ArgumentNullException.ThrowIfNull(id);

        // Inicializa retornoEventos se não existir
        _retornoLoteEventosAssincrono.retornoEventos ??= new eFinanceiraRetornoLoteEventosAssincronoRetornoEventos();

        var evento = new TArquivoeFinanceira
        {
            Any = eventoXml,
            id = id
        };

        var eventosExistentes = _retornoLoteEventosAssincrono.retornoEventos.evento?.ToList() ?? new List<TArquivoeFinanceira>();
        eventosExistentes.Add(evento);
        _retornoLoteEventosAssincrono.retornoEventos.evento = eventosExistentes.ToArray();

        return this;
    }

    /// <summary>
    /// Adiciona um evento ao retorno a partir de XML string
    /// </summary>
    /// <param name="eventoXmlString">XML do evento como string</param>
    /// <param name="id">ID do evento</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventosAssincronoBuilder AdicionarEvento(string eventoXmlString, string id)
    {
        ArgumentNullException.ThrowIfNull(eventoXmlString);
        ArgumentNullException.ThrowIfNull(id);

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(eventoXmlString);

        return AdicionarEvento(xmlDoc.DocumentElement!, id);
    }

    /// <summary>
    /// Define o ID do retorno
    /// </summary>
    /// <param name="id">ID do retorno</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventosAssincronoBuilder ComId(string id)
    {
        ArgumentNullException.ThrowIfNull(id);
        _retornoLoteEventosAssincrono.id = id;
        return this;
    }

    /// <summary>
    /// Define a assinatura digital
    /// </summary>
    /// <param name="signature">Assinatura digital</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventosAssincronoBuilder ComAssinatura(SignatureType signature)
    {
        ArgumentNullException.ThrowIfNull(signature);
        _eFinanceira.Signature = signature;
        return this;
    }

    /// <summary>
    /// Remove todas as ocorrências do status
    /// </summary>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventosAssincronoBuilder LimparOcorrencias()
    {
        if (_retornoLoteEventosAssincrono.status != null)
        {
            _retornoLoteEventosAssincrono.status.ocorrencias = null;
        }
        return this;
    }

    /// <summary>
    /// Remove todos os eventos do retorno
    /// </summary>
    /// <returns>Builder para encadeamento fluente</returns>
    public RetornoLoteEventosAssincronoBuilder LimparEventos()
    {
        if (_retornoLoteEventosAssincrono.retornoEventos != null)
        {
            _retornoLoteEventosAssincrono.retornoEventos.evento = null;
        }
        return this;
    }

    /// <summary>
    /// Constrói a mensagem final
    /// </summary>
    /// <returns>Mensagem RetornoLoteEventosAssincrono construída</returns>
    public RetornoLoteEventosAssincronoMessage Build()
    {
        return new RetornoLoteEventosAssincronoMessage(_version, _eFinanceira);
    }

    /// <summary>
    /// Cria um builder a partir de XML existente
    /// </summary>
    /// <param name="xmlContent">Conteúdo XML do retorno</param>
    /// <param name="version">Versão do esquema</param>
    /// <returns>Builder preenchido com os dados do XML</returns>
    public static RetornoLoteEventosAssincronoBuilder FromXml(string xmlContent, string version = "v1_0_0")
    {
        ArgumentNullException.ThrowIfNull(xmlContent);

        var serializer = new XmlSerializer(typeof(eFinanceira), "http://www.eFinanceira.gov.br/schemas/retornoLoteEventosAssincrono/v1_0_0");

        using var reader = new StringReader(xmlContent);
        using var xmlReader = XmlReader.Create(reader, new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null
        });

        var deserializedObject = serializer.Deserialize(xmlReader);
        if (deserializedObject is not eFinanceira eFinanceira)
        {
            throw new InvalidOperationException("Falha ao deserializar o XML para eFinanceira retornoLoteEventosAssincrono");
        }

        var builder = new RetornoLoteEventosAssincronoBuilder(version);
        builder._eFinanceira.retornoLoteEventosAssincrono = eFinanceira.retornoLoteEventosAssincrono;
        builder._eFinanceira.Signature = eFinanceira.Signature;

        return builder;
    }

    /// <summary>
    /// Cria um builder a partir de um objeto eFinanceira existente
    /// </summary>
    /// <param name="eFinanceira">Objeto eFinanceira</param>
    /// <param name="version">Versão do esquema</param>
    /// <returns>Builder preenchido com os dados do objeto</returns>
    public static RetornoLoteEventosAssincronoBuilder FromEFinanceira(eFinanceira eFinanceira, string version = "v1_0_0")
    {
        ArgumentNullException.ThrowIfNull(eFinanceira);

        var builder = new RetornoLoteEventosAssincronoBuilder(version);
        builder._eFinanceira.retornoLoteEventosAssincrono = eFinanceira.retornoLoteEventosAssincrono;
        builder._eFinanceira.Signature = eFinanceira.Signature;

        return builder;
    }
}

/// <summary>
/// Extensões para facilitar o uso do RetornoLoteEventosAssincronoBuilder
/// </summary>
public static class RetornoLoteEventosAssincronoBuilderExtensions
{
    /// <summary>
    /// Adiciona uma ocorrência de erro
    /// </summary>
    /// <param name="builder">Builder</param>
    /// <param name="codigo">Código do erro</param>
    /// <param name="descricao">Descrição do erro</param>
    /// <param name="localizacao">Localização do erro</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public static RetornoLoteEventosAssincronoBuilder ComErro(this RetornoLoteEventosAssincronoBuilder builder, string codigo, string descricao, string? localizacao = null)
    {
        return builder.AdicionarOcorrencia(codigo, descricao, 1, localizacao);
    }

    /// <summary>
    /// Adiciona uma ocorrência de aviso
    /// </summary>
    /// <param name="builder">Builder</param>
    /// <param name="codigo">Código do aviso</param>
    /// <param name="descricao">Descrição do aviso</param>
    /// <param name="localizacao">Localização do aviso</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public static RetornoLoteEventosAssincronoBuilder ComAviso(this RetornoLoteEventosAssincronoBuilder builder, string codigo, string descricao, string? localizacao = null)
    {
        return builder.AdicionarOcorrencia(codigo, descricao, 2, localizacao);
    }

    /// <summary>
    /// Define um status de sucesso
    /// </summary>
    /// <param name="builder">Builder</param>
    /// <param name="descricao">Descrição opcional</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public static RetornoLoteEventosAssincronoBuilder ComSucesso(this RetornoLoteEventosAssincronoBuilder builder, string? descricao = "Lote processado com sucesso")
    {
        return builder.ComStatus(0, descricao);
    }

    /// <summary>
    /// Define um status de erro
    /// </summary>
    /// <param name="builder">Builder</param>
    /// <param name="codigoErro">Código do erro</param>
    /// <param name="descricao">Descrição do erro</param>
    /// <returns>Builder para encadeamento fluente</returns>
    public static RetornoLoteEventosAssincronoBuilder ComErroProcessamento(this RetornoLoteEventosAssincronoBuilder builder, int codigoErro, string descricao)
    {
        return builder.ComStatus(codigoErro, descricao);
    }

    /// <summary>
    /// Verifica se o retorno indica sucesso
    /// </summary>
    /// <param name="message">Mensagem de retorno</param>
    /// <returns>True se o processamento foi bem-sucedido</returns>
    public static bool IsSuccessful(this RetornoLoteEventosAssincronoMessage message)
    {
        return message.EFinanceira.retornoLoteEventosAssincrono?.status?.cdResposta == 0;
    }

    /// <summary>
    /// Obtém todas as ocorrências de erro
    /// </summary>
    /// <param name="message">Mensagem de retorno</param>
    /// <returns>Lista de ocorrências de erro</returns>
    public static IEnumerable<TOcorrenciasOcorrencia> GetErros(this RetornoLoteEventosAssincronoMessage message)
    {
        var ocorrencias = message.EFinanceira.retornoLoteEventosAssincrono?.status?.ocorrencias;
        return ocorrencias?.Where(o => o.tipo == 1) ?? Enumerable.Empty<TOcorrenciasOcorrencia>();
    }

    /// <summary>
    /// Obtém todas as ocorrências de aviso
    /// </summary>
    /// <param name="message">Mensagem de retorno</param>
    /// <returns>Lista de ocorrências de aviso</returns>
    public static IEnumerable<TOcorrenciasOcorrencia> GetAvisos(this RetornoLoteEventosAssincronoMessage message)
    {
        var ocorrencias = message.EFinanceira.retornoLoteEventosAssincrono?.status?.ocorrencias;
        return ocorrencias?.Where(o => o.tipo == 2) ?? Enumerable.Empty<TOcorrenciasOcorrencia>();
    }

    /// <summary>
    /// Obtém todos os eventos retornados
    /// </summary>
    /// <param name="message">Mensagem de retorno</param>
    /// <returns>Lista de eventos</returns>
    public static IEnumerable<TArquivoeFinanceira> GetEventos(this RetornoLoteEventosAssincronoMessage message)
    {
        var eventos = message.EFinanceira.retornoLoteEventosAssincrono?.retornoEventos?.evento;
        return eventos ?? Enumerable.Empty<TArquivoeFinanceira>();
    }

    /// <summary>
    /// Obtém os dados de recepção do lote
    /// </summary>
    /// <param name="message">Mensagem de retorno</param>
    /// <returns>Dados de recepção do lote</returns>
    public static TDadosRecepcao? GetDadosRecepcao(this RetornoLoteEventosAssincronoMessage message)
    {
        return message.EFinanceira.retornoLoteEventosAssincrono?.dadosRecepcaoLote;
    }

    /// <summary>
    /// Obtém os dados de processamento do lote
    /// </summary>
    /// <param name="message">Mensagem de retorno</param>
    /// <returns>Dados de processamento do lote</returns>
    public static TDadosProcessamento? GetDadosProcessamento(this RetornoLoteEventosAssincronoMessage message)
    {
        return message.EFinanceira.retornoLoteEventosAssincrono?.dadosProcessamentoLote;
    }

    /// <summary>
    /// Obtém o CNPJ do declarante
    /// </summary>
    /// <param name="message">Mensagem de retorno</param>
    /// <returns>CNPJ do declarante</returns>
    public static string? GetCnpjDeclarante(this RetornoLoteEventosAssincronoMessage message)
    {
        return message.EFinanceira.retornoLoteEventosAssincrono?.cnpjDeclarante;
    }

    /// <summary>
    /// Obtém o protocolo de envio
    /// </summary>
    /// <param name="message">Mensagem de retorno</param>
    /// <returns>Protocolo de envio</returns>
    public static string? GetProtocoloEnvio(this RetornoLoteEventosAssincronoMessage message)
    {
        return message.EFinanceira.retornoLoteEventosAssincrono?.dadosRecepcaoLote?.protocoloEnvio;
    }
}
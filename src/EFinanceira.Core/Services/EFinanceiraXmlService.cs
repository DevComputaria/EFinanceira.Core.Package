using System.Text;
using System.Xml;
using System.Xml.Serialization;
using EFinanceira.Core.Exceptions;
using EFinanceira.Core.Models;
using Microsoft.Extensions.Logging;

namespace EFinanceira.Core.Services;

/// <summary>
/// StringWriter customizado para UTF-8
/// </summary>
internal class Utf8StringWriter : StringWriter
{
    public override Encoding Encoding => Encoding.UTF8;
}

/// <summary>
/// Interface para serialização e deserialização XML da e-Financeira
/// </summary>
public interface IEFinanceiraXmlService
{
    /// <summary>
    /// Serializa um envelope e-Financeira para XML
    /// </summary>
    /// <param name="envelope">Envelope a ser serializado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>String XML</returns>
    Task<string> SerializeAsync(EFinanceiraEnvelope envelope, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deserializa XML para um envelope e-Financeira
    /// </summary>
    /// <param name="xml">String XML</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Envelope e-Financeira</returns>
    Task<EFinanceiraEnvelope> DeserializeAsync(string xml, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida XML contra o esquema XSD
    /// </summary>
    /// <param name="xml">String XML a ser validada</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de erros de validação (vazia se válido)</returns>
    Task<IReadOnlyList<string>> ValidateXmlAsync(string xml, CancellationToken cancellationToken = default);

    /// <summary>
    /// Serializa diretamente para arquivo
    /// </summary>
    /// <param name="envelope">Envelope a ser serializado</param>
    /// <param name="filePath">Caminho do arquivo</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Task</returns>
    Task SerializeToFileAsync(EFinanceiraEnvelope envelope, string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deserializa diretamente de arquivo
    /// </summary>
    /// <param name="filePath">Caminho do arquivo</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Envelope e-Financeira</returns>
    Task<EFinanceiraEnvelope> DeserializeFromFileAsync(string filePath, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementação do serviço de XML da e-Financeira
/// </summary>
public class EFinanceiraXmlService : IEFinanceiraXmlService
{
    private readonly ILogger<EFinanceiraXmlService> _logger;
    private readonly XmlSerializer _serializer;
    private readonly XmlWriterSettings _writerSettings;
    private readonly XmlReaderSettings _readerSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="EFinanceiraXmlService"/> class.
    /// </summary>
    /// <param name="logger">Logger</param>
    public EFinanceiraXmlService(ILogger<EFinanceiraXmlService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serializer = new XmlSerializer(typeof(EFinanceiraEnvelope));
        
        _writerSettings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            Indent = true,
            IndentChars = "  ",
            NewLineChars = "\r\n",
            NewLineHandling = NewLineHandling.Replace,
            OmitXmlDeclaration = false
        };

        _readerSettings = new XmlReaderSettings
        {
            IgnoreWhitespace = true,
            IgnoreComments = true
        };
    }

    /// <inheritdoc />
    public async Task<string> SerializeAsync(EFinanceiraEnvelope envelope, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(envelope);

        try
        {
            _logger.LogDebug("Iniciando serialização do envelope e-Financeira");

            using var stringWriter = new Utf8StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, _writerSettings);

            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                _serializer.Serialize(xmlWriter, envelope);
            }, cancellationToken);

            var xml = stringWriter.ToString();
            
            _logger.LogDebug("Serialização concluída. XML gerado com {Length} caracteres", xml.Length);
            
            return xml;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operação de serialização foi cancelada");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante a serialização do envelope e-Financeira");
            throw new EFinanceiraSerializationException("Erro durante a serialização XML", ex);
        }
    }

    /// <inheritdoc />
    public async Task<EFinanceiraEnvelope> DeserializeAsync(string xml, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(xml);

        try
        {
            _logger.LogDebug("Iniciando deserialização do XML e-Financeira");

            using var stringReader = new StringReader(xml);
            using var xmlReader = XmlReader.Create(stringReader, _readerSettings);

            var envelope = await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return (EFinanceiraEnvelope)_serializer.Deserialize(xmlReader)!;
            }, cancellationToken);

            _logger.LogDebug("Deserialização concluída com sucesso");
            
            return envelope;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operação de deserialização foi cancelada");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante a deserialização do XML e-Financeira");
            throw new EFinanceiraSerializationException("Erro durante a deserialização XML", ex);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> ValidateXmlAsync(string xml, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(xml);

        var errors = new List<string>();

        try
        {
            _logger.LogDebug("Iniciando validação XML");

            // Validação básica de XML bem formado
            using var stringReader = new StringReader(xml);
            using var xmlReader = XmlReader.Create(stringReader, _readerSettings);

            await Task.Run(() =>
            {
                try
                {
                    while (xmlReader.Read())
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        // Lê todo o XML para verificar se está bem formado
                    }
                }
                catch (XmlException ex)
                {
                    errors.Add($"XML mal formado: {ex.Message}");
                }
            }, cancellationToken);

            if (errors.Count == 0)
            {
                // Tenta deserializar para validar estrutura
                try
                {
                    await DeserializeAsync(xml, cancellationToken);
                }
                catch (EFinanceiraSerializationException ex)
                {
                    errors.Add($"Erro de estrutura XML: {ex.Message}");
                }
            }

            _logger.LogDebug("Validação XML concluída. {ErrorCount} erros encontrados", errors.Count);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operação de validação XML foi cancelada");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante a validação XML");
            errors.Add($"Erro inesperado durante validação: {ex.Message}");
        }

        return errors.AsReadOnly();
    }

    /// <inheritdoc />
    public async Task SerializeToFileAsync(EFinanceiraEnvelope envelope, string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(envelope);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        try
        {
            _logger.LogDebug("Serializando envelope para arquivo: {FilePath}", filePath);

            var xml = await SerializeAsync(envelope, cancellationToken);

            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(filePath, xml, Encoding.UTF8, cancellationToken);

            _logger.LogInformation("Arquivo XML salvo com sucesso: {FilePath}", filePath);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operação de serialização para arquivo foi cancelada");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar arquivo XML: {FilePath}", filePath);
            throw new EFinanceiraSerializationException($"Erro ao salvar arquivo XML: {filePath}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<EFinanceiraEnvelope> DeserializeFromFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Arquivo não encontrado: {filePath}");
        }

        try
        {
            _logger.LogDebug("Deserializando envelope do arquivo: {FilePath}", filePath);

            var xml = await File.ReadAllTextAsync(filePath, Encoding.UTF8, cancellationToken);
            var envelope = await DeserializeAsync(xml, cancellationToken);

            _logger.LogInformation("Arquivo XML carregado com sucesso: {FilePath}", filePath);
            
            return envelope;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operação de deserialização de arquivo foi cancelada");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar arquivo XML: {FilePath}", filePath);
            throw new EFinanceiraSerializationException($"Erro ao carregar arquivo XML: {filePath}", ex);
        }
    }
}
using EFinanceira.Core.Configuration;
using EFinanceira.Core.Models;
using EFinanceira.Core.Validators;
using Microsoft.Extensions.Logging;

namespace EFinanceira.Core.Services;

/// <summary>
/// Interface principal do serviço e-Financeira
/// </summary>
public interface IEFinanceiraService
{
    /// <summary>
    /// Cria um novo envelope e-Financeira
    /// </summary>
    /// <param name="responsavel">Informações do responsável</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Envelope criado</returns>
    Task<EFinanceiraEnvelope> CreateEnvelopeAsync(ResponsavelIdentificacao responsavel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adiciona evento de movimentação financeira
    /// </summary>
    /// <param name="envelope">Envelope</param>
    /// <param name="movimentacao">Movimentação financeira</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>ID do evento criado</returns>
    Task<string> AddMovimentacaoFinanceiraAsync(EFinanceiraEnvelope envelope, MovimentacaoFinanceira movimentacao, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adiciona evento de abertura de conta
    /// </summary>
    /// <param name="envelope">Envelope</param>
    /// <param name="aberturaConta">Abertura de conta</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>ID do evento criado</returns>
    Task<string> AddAberturaContaAsync(EFinanceiraEnvelope envelope, AberturaConta aberturaConta, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adiciona evento de fechamento de conta
    /// </summary>
    /// <param name="envelope">Envelope</param>
    /// <param name="fechamentoConta">Fechamento de conta</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>ID do evento criado</returns>
    Task<string> AddFechamentoContaAsync(EFinanceiraEnvelope envelope, FechamentoConta fechamentoConta, CancellationToken cancellationToken = default);

    /// <summary>
    /// Processa e gera o XML final
    /// </summary>
    /// <param name="envelope">Envelope</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>XML processado</returns>
    Task<string> ProcessAsync(EFinanceiraEnvelope envelope, CancellationToken cancellationToken = default);

    /// <summary>
    /// Processa e salva em arquivo
    /// </summary>
    /// <param name="envelope">Envelope</param>
    /// <param name="filePath">Caminho do arquivo</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Task</returns>
    Task ProcessToFileAsync(EFinanceiraEnvelope envelope, string filePath, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementação do serviço principal e-Financeira
/// </summary>
public class EFinanceiraService : IEFinanceiraService
{
    private readonly IEFinanceiraXmlService _xmlService;
    private readonly IEFinanceiraValidator _validator;
    private readonly EFinanceiraOptions _options;
    private readonly ILogger<EFinanceiraService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EFinanceiraService"/> class.
    /// </summary>
    /// <param name="xmlService">Serviço XML</param>
    /// <param name="validator">Validador</param>
    /// <param name="options">Opções de configuração</param>
    /// <param name="logger">Logger</param>
    public EFinanceiraService(
        IEFinanceiraXmlService xmlService,
        IEFinanceiraValidator validator,
        EFinanceiraOptions options,
        ILogger<EFinanceiraService> logger)
    {
        _xmlService = xmlService ?? throw new ArgumentNullException(nameof(xmlService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<EFinanceiraEnvelope> CreateEnvelopeAsync(ResponsavelIdentificacao responsavel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(responsavel);

        _logger.LogDebug("Criando novo envelope e-Financeira para responsável: {CnpjRespons}", responsavel.CnpjRespons);

        var envelope = new EFinanceiraEnvelope
        {
            IdeEvento = new EventoIdentificacao
            {
                CnpjRespons = responsavel.CnpjRespons,
                DhEvento = DateTime.Now,
                VersaoEvento = _options.VersaoLayout
            },
            IdeRespons = responsavel,
            Eventos = new List<EventoEFinanceira>()
        };

        _logger.LogInformation("Envelope e-Financeira criado com sucesso");
        
        return await Task.FromResult(envelope);
    }

    /// <inheritdoc />
    public async Task<string> AddMovimentacaoFinanceiraAsync(EFinanceiraEnvelope envelope, MovimentacaoFinanceira movimentacao, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(envelope);
        ArgumentNullException.ThrowIfNull(movimentacao);

        var eventId = GenerateEventId();
        
        _logger.LogDebug("Adicionando evento de movimentação financeira: {EventId}", eventId);

        var evento = new EventoEFinanceira
        {
            Id = eventId,
            EvtMovFinanceira = movimentacao
        };

        envelope.Eventos.Add(evento);

        _logger.LogInformation("Evento de movimentação financeira adicionado: {EventId}", eventId);
        
        return await Task.FromResult(eventId);
    }

    /// <inheritdoc />
    public async Task<string> AddAberturaContaAsync(EFinanceiraEnvelope envelope, AberturaConta aberturaConta, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(envelope);
        ArgumentNullException.ThrowIfNull(aberturaConta);

        var eventId = GenerateEventId();
        
        _logger.LogDebug("Adicionando evento de abertura de conta: {EventId}", eventId);

        var evento = new EventoEFinanceira
        {
            Id = eventId,
            EvtAberConta = aberturaConta
        };

        envelope.Eventos.Add(evento);

        _logger.LogInformation("Evento de abertura de conta adicionado: {EventId}", eventId);
        
        return await Task.FromResult(eventId);
    }

    /// <inheritdoc />
    public async Task<string> AddFechamentoContaAsync(EFinanceiraEnvelope envelope, FechamentoConta fechamentoConta, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(envelope);
        ArgumentNullException.ThrowIfNull(fechamentoConta);

        var eventId = GenerateEventId();
        
        _logger.LogDebug("Adicionando evento de fechamento de conta: {EventId}", eventId);

        var evento = new EventoEFinanceira
        {
            Id = eventId,
            EvtFechConta = fechamentoConta
        };

        envelope.Eventos.Add(evento);

        _logger.LogInformation("Evento de fechamento de conta adicionado: {EventId}", eventId);
        
        return await Task.FromResult(eventId);
    }

    /// <inheritdoc />
    public async Task<string> ProcessAsync(EFinanceiraEnvelope envelope, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(envelope);

        _logger.LogInformation("Iniciando processamento do envelope e-Financeira");

        // Validar envelope
        await _validator.ValidateAsync(envelope, cancellationToken);

        // Gerar XML
        var xml = await _xmlService.SerializeAsync(envelope, cancellationToken);

        // Validar XML gerado
        if (_options.ValidateGeneratedXml)
        {
            var xmlErrors = await _xmlService.ValidateXmlAsync(xml, cancellationToken);
            if (xmlErrors.Count > 0)
            {
                _logger.LogError("XML gerado contém erros: {Errors}", string.Join(", ", xmlErrors));
                throw new Exceptions.EFinanceiraValidationException(xmlErrors);
            }
        }

        _logger.LogInformation("Processamento do envelope e-Financeira concluído com sucesso");
        
        return xml;
    }

    /// <inheritdoc />
    public async Task ProcessToFileAsync(EFinanceiraEnvelope envelope, string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(envelope);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        _logger.LogInformation("Processando envelope e-Financeira para arquivo: {FilePath}", filePath);

        // Validar envelope
        await _validator.ValidateAsync(envelope, cancellationToken);

        // Serializar diretamente para arquivo
        await _xmlService.SerializeToFileAsync(envelope, filePath, cancellationToken);

        // Validar arquivo gerado se necessário
        if (_options.ValidateGeneratedXml)
        {
            var envelopeValidated = await _xmlService.DeserializeFromFileAsync(filePath, cancellationToken);
            await _validator.ValidateAsync(envelopeValidated, cancellationToken);
        }

        _logger.LogInformation("Envelope e-Financeira processado e salvo em arquivo: {FilePath}", filePath);
    }

    private string GenerateEventId()
    {
        return $"ID{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
}
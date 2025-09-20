using System.ComponentModel.DataAnnotations;
using EFinanceira.Core.Exceptions;
using EFinanceira.Core.Models;
using Microsoft.Extensions.Logging;

namespace EFinanceira.Core.Validators;

/// <summary>
/// Interface para validação de dados da e-Financeira
/// </summary>
public interface IEFinanceiraValidator
{
    /// <summary>
    /// Valida o envelope da e-Financeira
    /// </summary>
    /// <param name="envelope">Envelope a ser validado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Task</returns>
    Task ValidateAsync(EFinanceiraEnvelope envelope, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida CNPJ
    /// </summary>
    /// <param name="cnpj">CNPJ a ser validado</param>
    /// <returns>True se válido</returns>
    bool ValidateCnpj(string cnpj);

    /// <summary>
    /// Valida CPF
    /// </summary>
    /// <param name="cpf">CPF a ser validado</param>
    /// <returns>True se válido</returns>
    bool ValidateCpf(string cpf);
}

/// <summary>
/// Implementação do validador da e-Financeira
/// </summary>
public class EFinanceiraValidator : IEFinanceiraValidator
{
    private readonly ILogger<EFinanceiraValidator> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EFinanceiraValidator"/> class.
    /// </summary>
    /// <param name="logger">Logger</param>
    public EFinanceiraValidator(ILogger<EFinanceiraValidator> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task ValidateAsync(EFinanceiraEnvelope envelope, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(envelope);

        _logger.LogDebug("Iniciando validação do envelope e-Financeira");

        var validationErrors = new List<string>();

        // Validação básica usando Data Annotations
        var validationContext = new ValidationContext(envelope);
        var validationResults = new List<ValidationResult>();

        if (!Validator.TryValidateObject(envelope, validationContext, validationResults, true))
        {
            validationErrors.AddRange(validationResults.Select(vr => vr.ErrorMessage ?? "Erro de validação desconhecido"));
        }

        // Validações específicas
        await ValidateEventoIdentificacaoAsync(envelope.IdeEvento, validationErrors, cancellationToken);
        await ValidateResponsavelIdentificacaoAsync(envelope.IdeRespons, validationErrors, cancellationToken);
        await ValidateEventosAsync(envelope.Eventos, validationErrors, cancellationToken);

        if (validationErrors.Count > 0)
        {
            _logger.LogWarning("Validação falhou com {ErrorCount} erros", validationErrors.Count);
            throw new EFinanceiraValidationException(validationErrors);
        }

        _logger.LogDebug("Validação do envelope e-Financeira concluída com sucesso");
    }

    /// <inheritdoc />
    public bool ValidateCnpj(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        // Remove caracteres não numéricos
        cnpj = new string(cnpj.Where(char.IsDigit).ToArray());

        if (cnpj.Length != 14)
            return false;

        // Verifica se todos os dígitos são iguais
        if (cnpj.All(c => c == cnpj[0]))
            return false;

        // Cálculo do primeiro dígito verificador
        var sum = 0;
        var weights1 = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        
        for (int i = 0; i < 12; i++)
        {
            sum += int.Parse(cnpj[i].ToString()) * weights1[i];
        }

        var remainder1 = sum % 11;
        var digit1 = remainder1 < 2 ? 0 : 11 - remainder1;

        if (int.Parse(cnpj[12].ToString()) != digit1)
            return false;

        // Cálculo do segundo dígito verificador
        sum = 0;
        var weights2 = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        
        for (int i = 0; i < 13; i++)
        {
            sum += int.Parse(cnpj[i].ToString()) * weights2[i];
        }

        var remainder2 = sum % 11;
        var digit2 = remainder2 < 2 ? 0 : 11 - remainder2;

        return int.Parse(cnpj[13].ToString()) == digit2;
    }

    /// <inheritdoc />
    public bool ValidateCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        // Remove caracteres não numéricos
        cpf = new string(cpf.Where(char.IsDigit).ToArray());

        if (cpf.Length != 11)
            return false;

        // Verifica se todos os dígitos são iguais
        if (cpf.All(c => c == cpf[0]))
            return false;

        // Cálculo do primeiro dígito verificador
        var sum = 0;
        for (int i = 0; i < 9; i++)
        {
            sum += int.Parse(cpf[i].ToString()) * (10 - i);
        }

        var remainder1 = sum % 11;
        var digit1 = remainder1 < 2 ? 0 : 11 - remainder1;

        if (int.Parse(cpf[9].ToString()) != digit1)
            return false;

        // Cálculo do segundo dígito verificador
        sum = 0;
        for (int i = 0; i < 10; i++)
        {
            sum += int.Parse(cpf[i].ToString()) * (11 - i);
        }

        var remainder2 = sum % 11;
        var digit2 = remainder2 < 2 ? 0 : 11 - remainder2;

        return int.Parse(cpf[10].ToString()) == digit2;
    }

    private async Task ValidateEventoIdentificacaoAsync(EventoIdentificacao ideEvento, List<string> validationErrors, CancellationToken cancellationToken)
    {
        if (!ValidateCnpj(ideEvento.CnpjRespons))
        {
            validationErrors.Add("CNPJ do responsável pelo evento é inválido");
        }

        if (ideEvento.DhEvento > DateTime.Now.AddDays(1))
        {
            validationErrors.Add("Data e hora do evento não pode ser futura");
        }

        await Task.CompletedTask;
    }

    private async Task ValidateResponsavelIdentificacaoAsync(ResponsavelIdentificacao ideRespons, List<string> validationErrors, CancellationToken cancellationToken)
    {
        if (!ValidateCnpj(ideRespons.CnpjRespons))
        {
            validationErrors.Add("CNPJ do responsável é inválido");
        }

        if (!string.IsNullOrWhiteSpace(ideRespons.CpfRespons) && !ValidateCpf(ideRespons.CpfRespons))
        {
            validationErrors.Add("CPF do responsável é inválido");
        }

        await Task.CompletedTask;
    }

    private async Task ValidateEventosAsync(List<EventoEFinanceira> eventos, List<string> validationErrors, CancellationToken cancellationToken)
    {
        if (eventos.Count == 0)
        {
            validationErrors.Add("Deve haver pelo menos um evento");
            return;
        }

        var duplicateIds = eventos
            .GroupBy(e => e.Id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);

        if (duplicateIds.Any())
        {
            validationErrors.Add($"IDs de eventos duplicados encontrados: {string.Join(", ", duplicateIds)}");
        }

        foreach (var evento in eventos)
        {
            await ValidateEventoAsync(evento, validationErrors, cancellationToken);
        }
    }

    private async Task ValidateEventoAsync(EventoEFinanceira evento, List<string> validationErrors, CancellationToken cancellationToken)
    {
        var eventTypes = new[]
        {
            evento.EvtMovFinanceira != null,
            evento.EvtAberConta != null,
            evento.EvtFechConta != null
        };

        var eventCount = eventTypes.Count(t => t);

        if (eventCount == 0)
        {
            validationErrors.Add($"Evento {evento.Id} deve ter pelo menos um tipo de evento definido");
        }
        else if (eventCount > 1)
        {
            validationErrors.Add($"Evento {evento.Id} deve ter apenas um tipo de evento definido");
        }

        // Validações específicas por tipo de evento
        if (evento.EvtMovFinanceira != null)
        {
            await ValidateMovimentacaoFinanceiraAsync(evento.EvtMovFinanceira, validationErrors, cancellationToken);
        }

        if (evento.EvtAberConta != null)
        {
            await ValidateAberturaContaAsync(evento.EvtAberConta, validationErrors, cancellationToken);
        }

        if (evento.EvtFechConta != null)
        {
            await ValidateFechamentoContaAsync(evento.EvtFechConta, validationErrors, cancellationToken);
        }
    }

    private async Task ValidateMovimentacaoFinanceiraAsync(MovimentacaoFinanceira movFinanceira, List<string> validationErrors, CancellationToken cancellationToken)
    {
        if (!ValidateCnpj(movFinanceira.IdeConta.CnpjInstituicao))
        {
            validationErrors.Add("CNPJ da instituição financeira é inválido");
        }

        if (movFinanceira.IniMovFin.DtIni > movFinanceira.IniMovFin.DtFim)
        {
            validationErrors.Add("Data inicial do período não pode ser maior que a data final");
        }

        if (movFinanceira.Movimentacoes.Count > 0)
        {
            foreach (var movimento in movFinanceira.Movimentacoes)
            {
                if (movimento.DtMov < movFinanceira.IniMovFin.DtIni || movimento.DtMov > movFinanceira.IniMovFin.DtFim)
                {
                    validationErrors.Add($"Data do movimento {movimento.DtMov:yyyy-MM-dd} está fora do período informado");
                }
            }
        }

        await Task.CompletedTask;
    }

    private async Task ValidateAberturaContaAsync(AberturaConta aberturaConta, List<string> validationErrors, CancellationToken cancellationToken)
    {
        if (!ValidateCnpj(aberturaConta.IdeConta.CnpjInstituicao))
        {
            validationErrors.Add("CNPJ da instituição financeira é inválido");
        }

        if (aberturaConta.DtAbertura > DateTime.Now.Date)
        {
            validationErrors.Add("Data de abertura da conta não pode ser futura");
        }

        await Task.CompletedTask;
    }

    private async Task ValidateFechamentoContaAsync(FechamentoConta fechamentoConta, List<string> validationErrors, CancellationToken cancellationToken)
    {
        if (!ValidateCnpj(fechamentoConta.IdeConta.CnpjInstituicao))
        {
            validationErrors.Add("CNPJ da instituição financeira é inválido");
        }

        if (fechamentoConta.DtFechamento > DateTime.Now.Date)
        {
            validationErrors.Add("Data de fechamento da conta não pode ser futura");
        }

        await Task.CompletedTask;
    }
}
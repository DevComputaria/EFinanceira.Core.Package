namespace EFinanceira.Core.Configuration;

/// <summary>
/// Opções de configuração para a biblioteca e-Financeira
/// </summary>
public class EFinanceiraOptions
{
    /// <summary>
    /// Seção de configuração padrão
    /// </summary>
    public const string DefaultSectionName = "EFinanceira";

    /// <summary>
    /// Versão do layout da e-Financeira
    /// </summary>
    public string VersaoLayout { get; set; } = "1.0.0";

    /// <summary>
    /// Valida XML gerado após serialização
    /// </summary>
    public bool ValidateGeneratedXml { get; set; } = true;

    /// <summary>
    /// Timeout para operações assíncronas (em milissegundos)
    /// </summary>
    public int TimeoutMs { get; set; } = 30000;

    /// <summary>
    /// Configurações de logging específicas
    /// </summary>
    public LoggingOptions Logging { get; set; } = new();

    /// <summary>
    /// Configurações de validação
    /// </summary>
    public ValidationOptions Validation { get; set; } = new();
}

/// <summary>
/// Opções de logging
/// </summary>
public class LoggingOptions
{
    /// <summary>
    /// Habilita logging detalhado de operações
    /// </summary>
    public bool EnableDetailedLogging { get; set; } = true;

    /// <summary>
    /// Habilita logging de dados sensíveis (para desenvolvimento apenas)
    /// </summary>
    public bool LogSensitiveData { get; set; } = false;

    /// <summary>
    /// Nível mínimo de log
    /// </summary>
    public string MinimumLevel { get; set; } = "Information";
}

/// <summary>
/// Opções de validação
/// </summary>
public class ValidationOptions
{
    /// <summary>
    /// Valida CNPJ/CPF usando algoritmo de dígito verificador
    /// </summary>
    public bool ValidateDocuments { get; set; } = true;

    /// <summary>
    /// Valida datas futuras
    /// </summary>
    public bool ValidateFutureDates { get; set; } = true;

    /// <summary>
    /// Permite valores zerados em movimentações
    /// </summary>
    public bool AllowZeroValues { get; set; } = false;

    /// <summary>
    /// Máximo de eventos por envelope
    /// </summary>
    public int MaxEventsPerEnvelope { get; set; } = 1000;

    /// <summary>
    /// Máximo de movimentações por evento
    /// </summary>
    public int MaxMovementsPerEvent { get; set; } = 10000;
}
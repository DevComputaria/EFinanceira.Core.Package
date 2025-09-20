namespace EFinanceira.Core.Exceptions;

/// <summary>
/// Exceção base para todas as exceções da biblioteca e-Financeira
/// </summary>
public abstract class EFinanceiraException : Exception
{
    /// <summary>
    /// Código do erro
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EFinanceiraException"/> class.
    /// </summary>
    /// <param name="errorCode">Código do erro</param>
    /// <param name="message">Mensagem de erro</param>
    protected EFinanceiraException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EFinanceiraException"/> class.
    /// </summary>
    /// <param name="errorCode">Código do erro</param>
    /// <param name="message">Mensagem de erro</param>
    /// <param name="innerException">Exceção interna</param>
    protected EFinanceiraException(string errorCode, string message, Exception innerException) 
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

/// <summary>
/// Exceção para erros de validação
/// </summary>
public class EFinanceiraValidationException : EFinanceiraException
{
    /// <summary>
    /// Erros de validação
    /// </summary>
    public IReadOnlyList<string> ValidationErrors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EFinanceiraValidationException"/> class.
    /// </summary>
    /// <param name="validationErrors">Lista de erros de validação</param>
    public EFinanceiraValidationException(IEnumerable<string> validationErrors) 
        : base("VALIDATION_ERROR", "Erro de validação dos dados")
    {
        ValidationErrors = validationErrors.ToList().AsReadOnly();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EFinanceiraValidationException"/> class.
    /// </summary>
    /// <param name="validationError">Erro de validação</param>
    public EFinanceiraValidationException(string validationError) 
        : this(new[] { validationError })
    {
    }
}

/// <summary>
/// Exceção para erros de serialização XML
/// </summary>
public class EFinanceiraSerializationException : EFinanceiraException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EFinanceiraSerializationException"/> class.
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    public EFinanceiraSerializationException(string message) 
        : base("SERIALIZATION_ERROR", message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EFinanceiraSerializationException"/> class.
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    /// <param name="innerException">Exceção interna</param>
    public EFinanceiraSerializationException(string message, Exception innerException) 
        : base("SERIALIZATION_ERROR", message, innerException)
    {
    }
}

/// <summary>
/// Exceção para erros de configuração
/// </summary>
public class EFinanceiraConfigurationException : EFinanceiraException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EFinanceiraConfigurationException"/> class.
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    public EFinanceiraConfigurationException(string message) 
        : base("CONFIGURATION_ERROR", message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EFinanceiraConfigurationException"/> class.
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    /// <param name="innerException">Exceção interna</param>
    public EFinanceiraConfigurationException(string message, Exception innerException) 
        : base("CONFIGURATION_ERROR", message, innerException)
    {
    }
}

/// <summary>
/// Exceção para erros de operação não suportada
/// </summary>
public class EFinanceiraNotSupportedException : EFinanceiraException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EFinanceiraNotSupportedException"/> class.
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    public EFinanceiraNotSupportedException(string message) 
        : base("NOT_SUPPORTED", message)
    {
    }
}
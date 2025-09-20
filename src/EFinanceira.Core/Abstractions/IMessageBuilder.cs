namespace EFinanceira.Core.Abstractions;

/// <summary>
/// Interface for building specific e-Financeira messages.
/// </summary>
/// <typeparam name="T">The type of message being built</typeparam>
public interface IMessageBuilder<T> where T : class
{
    /// <summary>
    /// Builds the message with the configured data.
    /// </summary>
    /// <returns>The built message</returns>
    T Build();

    /// <summary>
    /// Validates the current configuration before building.
    /// </summary>
    /// <returns>True if valid, false otherwise</returns>
    bool IsValid();

    /// <summary>
    /// Gets validation errors if any.
    /// </summary>
    /// <returns>Collection of validation errors</returns>
    IEnumerable<string> GetValidationErrors();
}
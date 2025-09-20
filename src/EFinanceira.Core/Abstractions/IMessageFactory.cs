using EFinanceira.Core.Factory;

namespace EFinanceira.Core.Abstractions;

/// <summary>
/// Factory for creating e-Financeira messages based on message kind and type.
/// </summary>
public interface IMessageFactory
{
    /// <summary>
    /// Creates a message of the specified kind and type.
    /// </summary>
    /// <param name="messageKind">The message kind (evt, lote, consulta)</param>
    /// <param name="messageType">The specific message type within the kind</param>
    /// <param name="version">The schema version</param>
    /// <returns>The created message instance</returns>
    IEFinanceiraMessage CreateMessage(MessageKind messageKind, string messageType, string version);

    /// <summary>
    /// Gets available message types for a given kind.
    /// </summary>
    /// <param name="messageKind">The message kind</param>
    /// <returns>Available message types</returns>
    IEnumerable<string> GetAvailableMessageTypes(MessageKind messageKind);

    /// <summary>
    /// Gets available versions for a given message kind and type.
    /// </summary>
    /// <param name="messageKind">The message kind</param>
    /// <param name="messageType">The message type</param>
    /// <returns>Available versions</returns>
    IEnumerable<string> GetAvailableVersions(MessageKind messageKind, string messageType);
}
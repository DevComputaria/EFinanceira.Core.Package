namespace EFinanceira.Core.Abstractions;

/// <summary>
/// Represents an e-Financeira message that can be processed, validated, and serialized.
/// </summary>
public interface IEFinanceiraMessage
{
    /// <summary>
    /// Gets the message kind (evt, lote, consulta).
    /// </summary>
    string MessageKind { get; }

    /// <summary>
    /// Gets the message type within the kind.
    /// </summary>
    string MessageType { get; }

    /// <summary>
    /// Gets the schema version for this message.
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Gets the underlying message data.
    /// </summary>
    object Data { get; }
}
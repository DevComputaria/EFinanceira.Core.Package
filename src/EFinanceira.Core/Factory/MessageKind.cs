namespace EFinanceira.Core.Factory;

/// <summary>
/// Enumeration of e-Financeira message kinds.
/// </summary>
public enum MessageKind
{
    /// <summary>
    /// Event messages (eventos)
    /// </summary>
    Evento,

    /// <summary>
    /// Batch messages (lotes)
    /// </summary>
    Lote,

    /// <summary>
    /// Query messages (consultas)
    /// </summary>
    Consulta
}
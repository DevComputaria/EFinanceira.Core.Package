namespace EFinanceira.Core.Factory;

/// <summary>
/// Categorias de mensagens do e-Financeira
/// </summary>
public enum MessageKindCategory
{
    /// <summary>
    /// Eventos individuais (abertura, movimentação, fechamento, etc.)
    /// </summary>
    Evento,

    /// <summary>
    /// Lotes de eventos para envio
    /// </summary>
    Lote,

    /// <summary>
    /// Consultas de informações
    /// </summary>
    Consulta
}

/// <summary>
/// Representa o tipo de uma mensagem do e-Financeira
/// </summary>
/// <param name="Category">Categoria da mensagem</param>
/// <param name="Name">Nome específico da mensagem</param>
public readonly record struct MessageKind(MessageKindCategory Category, string Name)
{
    /// <summary>
    /// Cria um MessageKind para evento
    /// </summary>
    public static MessageKind Evento(string name) => new(MessageKindCategory.Evento, name);

    /// <summary>
    /// Cria um MessageKind para lote
    /// </summary>
    public static MessageKind Lote(string name) => new(MessageKindCategory.Lote, name);

    /// <summary>
    /// Cria um MessageKind para consulta
    /// </summary>
    public static MessageKind Consulta(string name) => new(MessageKindCategory.Consulta, name);

    public override string ToString() => $"{Category}:{Name}";
}

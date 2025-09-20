namespace EFinanceira.Core.Abstractions;

/// <summary>
/// Interface para builders de mensagens do e-Financeira
/// </summary>
/// <typeparam name="T">Tipo da mensagem que o builder constrói</typeparam>
public interface IMessageBuilder<out T> where T : IEFinanceiraMessage
{
    /// <summary>
    /// Constrói a mensagem com as configurações aplicadas
    /// </summary>
    /// <returns>Instância da mensagem construída</returns>
    T Build();
}

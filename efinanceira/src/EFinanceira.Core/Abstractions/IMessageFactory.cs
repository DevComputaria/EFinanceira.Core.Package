using EFinanceira.Core.Factory;

namespace EFinanceira.Core.Abstractions;

/// <summary>
/// Factory para criação de mensagens do e-Financeira
/// </summary>
public interface IMessageFactory
{
    /// <summary>
    /// Cria uma instância de mensagem baseada no tipo e versão especificados
    /// </summary>
    /// <param name="kind">Tipo da mensagem (categoria e nome)</param>
    /// <param name="version">Versão do schema</param>
    /// <param name="seed">Ação opcional para inicializar o objeto raiz</param>
    /// <returns>Instância da mensagem criada</returns>
    IEFinanceiraMessage Create(MessageKind kind, string version, Action<object>? seed = null);
}

using EFinanceira.Core.Abstractions;

namespace EFinanceira.Core.Factory;

/// <summary>
/// Factory central para criação de mensagens do e-Financeira
/// </summary>
public sealed class EFinanceiraMessageFactory : IMessageFactory
{
    private readonly Dictionary<(MessageKindCategory Category, string Name, string Version), Func<Action<object>?, IEFinanceiraMessage>> _factories = new();

    /// <summary>
    /// Registra um factory para um tipo específico de mensagem
    /// </summary>
    /// <typeparam name="T">Tipo da mensagem</typeparam>
    /// <param name="category">Categoria da mensagem</param>
    /// <param name="name">Nome da mensagem</param>
    /// <param name="version">Versão do schema</param>
    /// <param name="factory">Função que cria a mensagem</param>
    public void RegisterFactory<T>(
        MessageKindCategory category,
        string name,
        string version,
        Func<Action<object>?, T> factory) where T : IEFinanceiraMessage
    {
        _factories[(category, name, version)] = seed => factory(seed);
    }

    /// <summary>
    /// Registra um factory usando MessageKind
    /// </summary>
    public void RegisterFactory<T>(
        MessageKind kind,
        string version,
        Func<Action<object>?, T> factory) where T : IEFinanceiraMessage
    {
        RegisterFactory(kind.Category, kind.Name, version, factory);
    }

    /// <inheritdoc />
    public IEFinanceiraMessage Create(MessageKind kind, string version, Action<object>? seed = null)
    {
        var key = (kind.Category, kind.Name, version);

        if (!_factories.TryGetValue(key, out var factory))
        {
            throw new NotSupportedException($"Mensagem não suportada: {kind} v{version}");
        }

        return factory(seed);
    }

    /// <summary>
    /// Lista todos os tipos de mensagem registrados
    /// </summary>
    /// <returns>Coleção de tipos registrados</returns>
    public IEnumerable<(MessageKind Kind, string Version)> GetRegisteredTypes()
    {
        return _factories.Keys.Select(k => (new MessageKind(k.Category, k.Name), k.Version));
    }
}

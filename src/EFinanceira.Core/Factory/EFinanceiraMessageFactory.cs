using EFinanceira.Core.Abstractions;

namespace EFinanceira.Core.Factory;

/// <summary>
/// Factory implementation for creating e-Financeira messages.
/// </summary>
public class EFinanceiraMessageFactory : IMessageFactory
{
    private readonly Dictionary<(MessageKind, string, string), Type> _messageTypes;

    public EFinanceiraMessageFactory()
    {
        _messageTypes = new Dictionary<(MessageKind, string, string), Type>();
        RegisterMessageTypes();
    }

    public IEFinanceiraMessage CreateMessage(MessageKind messageKind, string messageType, string version)
    {
        var key = (messageKind, messageType, version);
        if (!_messageTypes.TryGetValue(key, out var type))
        {
            throw new ArgumentException($"Unknown message type: {messageKind}/{messageType}/{version}");
        }

        var instance = Activator.CreateInstance(type);
        return instance as IEFinanceiraMessage ?? throw new InvalidOperationException($"Type {type.Name} does not implement IEFinanceiraMessage");
    }

    public IEnumerable<string> GetAvailableMessageTypes(MessageKind messageKind)
    {
        return _messageTypes.Keys
            .Where(k => k.Item1 == messageKind)
            .Select(k => k.Item2)
            .Distinct();
    }

    public IEnumerable<string> GetAvailableVersions(MessageKind messageKind, string messageType)
    {
        return _messageTypes.Keys
            .Where(k => k.Item1 == messageKind && k.Item2 == messageType)
            .Select(k => k.Item3)
            .Distinct();
    }

    private void RegisterMessageTypes()
    {
        // This will be populated as message types are implemented
        // For now, this is a placeholder structure
    }
}
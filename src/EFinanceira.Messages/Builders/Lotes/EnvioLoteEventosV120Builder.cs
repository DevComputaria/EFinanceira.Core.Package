using EFinanceira.Core.Abstractions;

namespace EFinanceira.Messages.Builders.Lotes;

/// <summary>
/// Builder for EnvioLoteEventos v1.2.0 messages.
/// </summary>
public class EnvioLoteEventosV120Builder : IMessageBuilder<object>
{
    private readonly List<string> _validationErrors = new();

    // Properties for building the message would go here
    // This is a placeholder implementation

    public object Build()
    {
        if (!IsValid())
            throw new InvalidOperationException("Cannot build message with validation errors");

        // This would return the actual message object
        // For now, returning a placeholder
        return new { MessageType = "EnvioLoteEventos", Version = "1.2.0" };
    }

    public bool IsValid()
    {
        _validationErrors.Clear();
        
        // Add validation logic here
        // For now, assuming it's always valid
        
        return !_validationErrors.Any();
    }

    public IEnumerable<string> GetValidationErrors()
    {
        return _validationErrors.AsReadOnly();
    }
}
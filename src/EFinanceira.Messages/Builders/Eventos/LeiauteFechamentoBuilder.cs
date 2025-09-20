using EFinanceira.Core.Abstractions;

namespace EFinanceira.Messages.Builders.Eventos;

/// <summary>
/// Builder for LeiauteFechamento messages.
/// </summary>
public class LeiauteFechamentoBuilder : IMessageBuilder<object>
{
    private readonly List<string> _validationErrors = new();

    public object Build()
    {
        if (!IsValid())
            throw new InvalidOperationException("Cannot build message with validation errors");

        return new { MessageType = "LeiauteFechamento", Version = "1.3.0" };
    }

    public bool IsValid()
    {
        _validationErrors.Clear();
        return !_validationErrors.Any();
    }

    public IEnumerable<string> GetValidationErrors()
    {
        return _validationErrors.AsReadOnly();
    }
}
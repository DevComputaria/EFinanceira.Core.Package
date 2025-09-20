using EFinanceira.Core.Abstractions;

namespace EFinanceira.Messages.Builders.Lotes;

/// <summary>
/// Builder for EnvioLoteEventosAssincrono v1.0.0 messages.
/// </summary>
public class EnvioLoteEventosAssincronoV100Builder : IMessageBuilder<object>
{
    private readonly List<string> _validationErrors = new();

    public object Build()
    {
        if (!IsValid())
            throw new InvalidOperationException("Cannot build message with validation errors");

        return new { MessageType = "EnvioLoteEventosAssincrono", Version = "1.0.0" };
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
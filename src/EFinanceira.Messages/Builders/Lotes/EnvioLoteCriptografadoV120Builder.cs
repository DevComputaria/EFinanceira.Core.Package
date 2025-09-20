using EFinanceira.Core.Abstractions;

namespace EFinanceira.Messages.Builders.Lotes;

/// <summary>
/// Builder for EnvioLoteCriptografado v1.2.0 messages.
/// </summary>
public class EnvioLoteCriptografadoV120Builder : IMessageBuilder<object>
{
    private readonly List<string> _validationErrors = new();

    public object Build()
    {
        if (!IsValid())
            throw new InvalidOperationException("Cannot build message with validation errors");

        return new { MessageType = "EnvioLoteCriptografado", Version = "1.2.0" };
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
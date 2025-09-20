using EFinanceira.Core.Abstractions;

namespace EFinanceira.Messages.Builders.Consultas;

/// <summary>
/// Builder for RetornoConsultaInformacoesRerct v1.0.1 messages.
/// </summary>
public class RetornoConsultaInformacoesRerctV101Builder : IMessageBuilder<object>
{
    private readonly List<string> _validationErrors = new();

    public object Build()
    {
        if (!IsValid())
            throw new InvalidOperationException("Cannot build message with validation errors");

        return new { MessageType = "RetornoConsultaInformacoesRerct", Version = "1.0.1" };
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
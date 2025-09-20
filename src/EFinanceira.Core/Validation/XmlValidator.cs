using System.Xml;
using System.Xml.Schema;
using EFinanceira.Core.Abstractions;

namespace EFinanceira.Core.Validation;

/// <summary>
/// XML validator implementation for validating against XSD schemas.
/// </summary>
public class XmlValidator : IXmlValidator
{
    private readonly List<string> _validationErrors = new();

    public bool ValidateXml(string xmlContent, string schemaPath)
    {
        var errors = ValidateXmlWithErrors(xmlContent, schemaPath);
        return !errors.Any();
    }

    public IEnumerable<string> ValidateXmlWithErrors(string xmlContent, string schemaPath)
    {
        _validationErrors.Clear();

        if (string.IsNullOrEmpty(xmlContent))
        {
            _validationErrors.Add("XML content cannot be null or empty");
            return _validationErrors;
        }

        if (string.IsNullOrEmpty(schemaPath) || !File.Exists(schemaPath))
        {
            _validationErrors.Add($"Schema file not found: {schemaPath}");
            return _validationErrors;
        }

        try
        {
            var settings = new XmlReaderSettings();
            settings.Schemas.Add(null, schemaPath);
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += ValidationEventHandler;

            using var reader = XmlReader.Create(new StringReader(xmlContent), settings);
            while (reader.Read()) { }
        }
        catch (Exception ex)
        {
            _validationErrors.Add($"Validation error: {ex.Message}");
        }

        return _validationErrors.ToList();
    }

    public bool ValidateMessage(IEFinanceiraMessage message)
    {
        if (message == null)
            return false;

        // This would need to be implemented based on the specific message structure
        // and schema locations. For now, this is a placeholder.
        var schemaPath = GetSchemaPath(message);
        if (string.IsNullOrEmpty(schemaPath))
            return false;

        // Convert message to XML and validate
        // This would require the serializer implementation
        return true; // Placeholder
    }

    private void ValidationEventHandler(object? sender, ValidationEventArgs e)
    {
        _validationErrors.Add($"{e.Severity}: {e.Message}");
    }

    private string? GetSchemaPath(IEFinanceiraMessage message)
    {
        // This would map message types to their corresponding XSD files
        // Based on the message kind, type, and version
        return null; // Placeholder
    }
}
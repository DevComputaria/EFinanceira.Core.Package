namespace EFinanceira.Core.Abstractions;

/// <summary>
/// Interface for XML validation against XSD schemas.
/// </summary>
public interface IXmlValidator
{
    /// <summary>
    /// Validates XML content against the appropriate XSD schema.
    /// </summary>
    /// <param name="xmlContent">The XML content to validate</param>
    /// <param name="schemaPath">Path to the XSD schema file</param>
    /// <returns>True if valid, false otherwise</returns>
    bool ValidateXml(string xmlContent, string schemaPath);

    /// <summary>
    /// Validates XML content and returns detailed validation errors.
    /// </summary>
    /// <param name="xmlContent">The XML content to validate</param>
    /// <param name="schemaPath">Path to the XSD schema file</param>
    /// <returns>Collection of validation errors, empty if valid</returns>
    IEnumerable<string> ValidateXmlWithErrors(string xmlContent, string schemaPath);

    /// <summary>
    /// Validates an e-Financeira message against its corresponding schema.
    /// </summary>
    /// <param name="message">The message to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    bool ValidateMessage(IEFinanceiraMessage message);
}
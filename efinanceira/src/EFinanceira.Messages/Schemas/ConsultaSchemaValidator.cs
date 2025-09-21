using System.Xml;
using System.Xml.Schema;
using EFinanceira.Core.Abstractions;

namespace EFinanceira.Messages.Schemas;

/// <summary>
/// Validador específico para schemas de consulta do e-Financeira
/// </summary>
public class ConsultaSchemaValidator : IXmlValidator
{
    private readonly Dictionary<string, XmlSchema> _schemasCache = new();
    private readonly object _lockObject = new();

    /// <summary>
    /// Valida XML de retorno de consulta cadastral
    /// </summary>
    public bool ValidateRetInfoCadastral(string xml)
    {
        return ValidateAgainstSchema(xml, "retInfoCadastral-v1_2_0.xsd");
    }

    /// <summary>
    /// Valida XML de retorno de consulta de intermediário
    /// </summary>
    public bool ValidateRetInfoIntermediario(string xml)
    {
        return ValidateAgainstSchema(xml, "retInfoIntermediario-v1_2_0.xsd");
    }

    /// <summary>
    /// Valida XML de retorno de consulta de patrocinado
    /// </summary>
    public bool ValidateRetInfoPatrocinado(string xml)
    {
        return ValidateAgainstSchema(xml, "retInfoPatrocinado-v1_2_0.xsd");
    }

    /// <summary>
    /// Valida XML de retorno de consulta de movimento
    /// </summary>
    public bool ValidateRetInfoMovimento(string xml)
    {
        return ValidateAgainstSchema(xml, "retInfoMovimento-v1_2_0.xsd");
    }

    /// <summary>
    /// Valida XML de retorno de consulta lista e-Financeira
    /// </summary>
    public bool ValidateRetListaeFinanceira(string xml)
    {
        return ValidateAgainstSchema(xml, "retListaeFinanceira-v1_2_0.xsd");
    }

    /// <summary>
    /// Valida XML de retorno de consulta RERCT
    /// </summary>
    public bool ValidateRetRERCT(string xml)
    {
        return ValidateAgainstSchema(xml, "retRERCT-v1_2_0.xsd");
    }

    /// <inheritdoc />
    public bool Validate(string xml, string? schemaPath = null)
    {
        if (string.IsNullOrEmpty(schemaPath))
        {
            throw new ArgumentException("Schema path é obrigatório para validação de consulta", nameof(schemaPath));
        }

        return ValidateAgainstSchema(xml, Path.GetFileName(schemaPath));
    }

    /// <inheritdoc />
    public Task<bool> ValidateAsync(string xml, string? schemaPath = null)
    {
        return Task.FromResult(Validate(xml, schemaPath));
    }

    private bool ValidateAgainstSchema(string xml, string schemaFileName)
    {
        try
        {
            var schema = GetCachedSchema(schemaFileName);
            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                ValidationFlags = XmlSchemaValidationFlags.ProcessInlineSchema |
                                XmlSchemaValidationFlags.ProcessSchemaLocation |
                                XmlSchemaValidationFlags.ReportValidationWarnings
            };

            settings.Schemas.Add(schema);

            var hasErrors = false;
            settings.ValidationEventHandler += (sender, e) =>
            {
                if (e.Severity == XmlSeverityType.Error)
                {
                    hasErrors = true;
                }
            };

            using var stringReader = new StringReader(xml);
            using var xmlReader = XmlReader.Create(stringReader, settings);

            while (xmlReader.Read()) { }

            return !hasErrors;
        }
        catch
        {
            return false;
        }
    }

    private XmlSchema GetCachedSchema(string schemaFileName)
    {
        if (_schemasCache.TryGetValue(schemaFileName, out var cachedSchema))
        {
            return cachedSchema;
        }

        lock (_lockObject)
        {
            if (_schemasCache.TryGetValue(schemaFileName, out cachedSchema))
            {
                return cachedSchema;
            }

            using var schemaStream = ConsultaSchemas.GetSchemaStream(schemaFileName);
            using var schemaReader = XmlReader.Create(schemaStream);
            
            var schema = XmlSchema.Read(schemaReader, null) 
                ?? throw new InvalidOperationException($"Não foi possível carregar o schema: {schemaFileName}");

            _schemasCache[schemaFileName] = schema;
            return schema;
        }
    }
}

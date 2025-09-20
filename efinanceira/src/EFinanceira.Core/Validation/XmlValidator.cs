using System.Xml;
using System.Xml.Schema;
using EFinanceira.Core.Abstractions;

namespace EFinanceira.Core.Validation;

/// <summary>
/// Implementação de validação XML contra schemas XSD
/// </summary>
public sealed class XmlValidator : IXmlValidator
{
    private readonly Dictionary<string, XmlSchemaSet> _cachedSchemas = new();

    /// <inheritdoc />
    public void Validate(string xml, IEnumerable<string> xsdPathsOrEmbedded)
    {
        var errors = ValidateAndGetErrors(xml, xsdPathsOrEmbedded);

        if (errors.Count > 0)
        {
            var errorMessage = string.Join(Environment.NewLine, errors);
            throw new XmlValidationException($"XML inválido:{Environment.NewLine}{errorMessage}");
        }
    }

    /// <inheritdoc />
    public IList<string> ValidateAndGetErrors(string xml, IEnumerable<string> xsdPathsOrEmbedded)
    {
        var errors = new List<string>();
        var schemaSet = GetOrCreateSchemaSet(xsdPathsOrEmbedded);

        var settings = new XmlReaderSettings
        {
            ValidationType = ValidationType.Schema,
            Schemas = schemaSet,
            ValidationFlags = XmlSchemaValidationFlags.ProcessInlineSchema |
                            XmlSchemaValidationFlags.ProcessSchemaLocation |
                            XmlSchemaValidationFlags.ReportValidationWarnings
        };

        settings.ValidationEventHandler += (sender, e) =>
        {
            var message = $"Linha {e.Exception?.LineNumber}, Posição {e.Exception?.LinePosition}: {e.Message}";
            errors.Add(message);
        };

        try
        {
            using var stringReader = new StringReader(xml);
            using var xmlReader = XmlReader.Create(stringReader, settings);

            while (xmlReader.Read()) { }
        }
        catch (XmlException ex)
        {
            errors.Add($"Erro de XML: {ex.Message}");
        }
        catch (Exception ex)
        {
            errors.Add($"Erro de validação: {ex.Message}");
        }

        return errors;
    }

    /// <summary>
    /// Limpa o cache de schemas
    /// </summary>
    public void ClearSchemaCache()
    {
        _cachedSchemas.Clear();
    }

    private XmlSchemaSet GetOrCreateSchemaSet(IEnumerable<string> xsdPaths)
    {
        var pathsArray = xsdPaths.ToArray();
        var cacheKey = string.Join("|", pathsArray.OrderBy(p => p));

        if (_cachedSchemas.TryGetValue(cacheKey, out var cachedSet))
        {
            return cachedSet;
        }

        var schemaSet = new XmlSchemaSet();

        foreach (var path in pathsArray)
        {
            try
            {
                if (File.Exists(path))
                {
                    // Arquivo físico
                    schemaSet.Add(null, path);
                }
                else if (IsEmbeddedResource(path))
                {
                    // Recurso embarcado
                    using var stream = GetEmbeddedResourceStream(path);
                    if (stream != null)
                    {
                        schemaSet.Add(null, XmlReader.Create(stream));
                    }
                }
                else
                {
                    throw new FileNotFoundException($"Schema XSD não encontrado: {path}");
                }
            }
            catch (Exception ex)
            {
                throw new XmlValidationException($"Erro ao carregar schema {path}: {ex.Message}", ex);
            }
        }

        try
        {
            schemaSet.Compile();
        }
        catch (XmlSchemaException ex)
        {
            throw new XmlValidationException($"Erro ao compilar schemas: {ex.Message}", ex);
        }

        _cachedSchemas[cacheKey] = schemaSet;
        return schemaSet;
    }

    private static bool IsEmbeddedResource(string path)
    {
        return path.StartsWith("embedded:", StringComparison.OrdinalIgnoreCase);
    }

    private static Stream? GetEmbeddedResourceStream(string embeddedPath)
    {
        // Remove prefixo "embedded:"
        var resourceName = embeddedPath.Substring("embedded:".Length);

        // Procura o recurso em todos os assemblies carregados
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null)
                return stream;
        }

        return null;
    }
}

/// <summary>
/// Exceção específica para erros de validação XML
/// </summary>
public class XmlValidationException : Exception
{
    public XmlValidationException(string message) : base(message) { }
    public XmlValidationException(string message, Exception innerException) : base(message, innerException) { }
}

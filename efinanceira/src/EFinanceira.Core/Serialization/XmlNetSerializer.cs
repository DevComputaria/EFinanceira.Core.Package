using System.Text;
using System.Xml;
using System.Xml.Serialization;
using EFinanceira.Core.Abstractions;

namespace EFinanceira.Core.Serialization;

/// <summary>
/// Implementação de serialização XML usando XmlSerializer do .NET
/// </summary>
public sealed class XmlNetSerializer : IXmlSerializer
{
    private readonly Dictionary<Type, XmlSerializer> _serializerCache = new();
    private readonly object _lock = new();

    /// <inheritdoc />
    public string Serialize(object root, XmlWriterSettings? settings = null)
    {
        ArgumentNullException.ThrowIfNull(root);

        var serializer = GetSerializer(root.GetType());
        var writerSettings = settings ?? CreateDefaultWriterSettings();

        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, writerSettings);

        // Remover namespaces desnecessários
        var namespaces = new XmlSerializerNamespaces();
        namespaces.Add("", ""); // Namespace padrão vazio

        serializer.Serialize(xmlWriter, root, namespaces);

        return stringWriter.ToString();
    }

    /// <inheritdoc />
    public T Deserialize<T>(string xml) where T : class
    {
        ArgumentException.ThrowIfNullOrEmpty(xml);

        var serializer = GetSerializer(typeof(T));

        using var stringReader = new StringReader(xml);
        using var xmlReader = XmlReader.Create(stringReader, CreateDefaultReaderSettings());

        var result = serializer.Deserialize(xmlReader);

        if (result is not T typedResult)
        {
            throw new InvalidOperationException($"Não foi possível deserializar para o tipo {typeof(T).Name}");
        }

        return typedResult;
    }

    /// <summary>
    /// Serializa com configurações específicas para e-Financeira (sem identação, UTF-8)
    /// </summary>
    public string SerializeForSigning(object root)
    {
        var settings = new XmlWriterSettings
        {
            Indent = false,
            Encoding = new UTF8Encoding(false), // UTF-8 sem BOM
            OmitXmlDeclaration = false,
            NewLineHandling = NewLineHandling.None
        };

        return Serialize(root, settings);
    }

    /// <summary>
    /// Serializa com identação para leitura humana
    /// </summary>
    public string SerializeFormatted(object root)
    {
        var settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  ",
            Encoding = new UTF8Encoding(false),
            OmitXmlDeclaration = false,
            NewLineHandling = NewLineHandling.Replace,
            NewLineChars = Environment.NewLine
        };

        return Serialize(root, settings);
    }

    /// <summary>
    /// Limpa o cache de serializers
    /// </summary>
    public void ClearCache()
    {
        lock (_lock)
        {
            _serializerCache.Clear();
        }
    }

    private XmlSerializer GetSerializer(Type type)
    {
        lock (_lock)
        {
            if (_serializerCache.TryGetValue(type, out var cachedSerializer))
            {
                return cachedSerializer;
            }

            var serializer = new XmlSerializer(type);
            _serializerCache[type] = serializer;
            return serializer;
        }
    }

    private static XmlWriterSettings CreateDefaultWriterSettings()
    {
        return new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  ",
            Encoding = new UTF8Encoding(false), // UTF-8 sem BOM
            OmitXmlDeclaration = false,
            NewLineHandling = NewLineHandling.Replace,
            NewLineChars = Environment.NewLine
        };
    }

    private static XmlReaderSettings CreateDefaultReaderSettings()
    {
        return new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            MaxCharactersFromEntities = 1024,
            MaxCharactersInDocument = 1024 * 1024 * 10 // 10MB
        };
    }
}

using System.Text;
using System.Xml;
using System.Xml.Serialization;
using EFinanceira.Core.Abstractions;

namespace EFinanceira.Core.Serialization;

/// <summary>
/// XML serializer implementation using System.Xml.Serialization.
/// </summary>
public class XmlNetSerializer : IXmlSerializer
{
    private readonly XmlSerializerNamespaces _namespaces;

    public XmlNetSerializer()
    {
        _namespaces = new XmlSerializerNamespaces();
        _namespaces.Add("", ""); // Remove default namespace prefixes
    }

    public string Serialize<T>(T obj) where T : class
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        var serializer = new XmlSerializer(typeof(T));
        
        var settings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            Indent = true,
            IndentChars = "  ",
            OmitXmlDeclaration = false
        };

        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, settings);
        
        serializer.Serialize(xmlWriter, obj, _namespaces);
        return stringWriter.ToString();
    }

    public T Deserialize<T>(string xmlContent) where T : class
    {
        if (string.IsNullOrEmpty(xmlContent))
            throw new ArgumentException("XML content cannot be null or empty", nameof(xmlContent));

        var serializer = new XmlSerializer(typeof(T));
        
        using var stringReader = new StringReader(xmlContent);
        using var xmlReader = XmlReader.Create(stringReader);
        
        var result = serializer.Deserialize(xmlReader);
        return result as T ?? throw new InvalidOperationException($"Failed to deserialize to type {typeof(T).Name}");
    }

    public string SerializeMessage(IEFinanceiraMessage message)
    {
        if (message?.Data == null)
            throw new ArgumentException("Message and its data cannot be null", nameof(message));

        // Use reflection to call the generic Serialize method with the actual data type
        var dataType = message.Data.GetType();
        var method = GetType().GetMethod(nameof(Serialize))?.MakeGenericMethod(dataType);
        
        var result = method?.Invoke(this, new[] { message.Data });
        return result as string ?? throw new InvalidOperationException("Failed to serialize message");
    }
}
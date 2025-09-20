namespace EFinanceira.Core.Abstractions;

/// <summary>
/// Interface for XML serialization and deserialization.
/// </summary>
public interface IXmlSerializer
{
    /// <summary>
    /// Serializes an object to XML string.
    /// </summary>
    /// <typeparam name="T">The type of object to serialize</typeparam>
    /// <param name="obj">The object to serialize</param>
    /// <returns>XML string representation</returns>
    string Serialize<T>(T obj) where T : class;

    /// <summary>
    /// Deserializes XML string to an object.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <param name="xmlContent">The XML content to deserialize</param>
    /// <returns>The deserialized object</returns>
    T Deserialize<T>(string xmlContent) where T : class;

    /// <summary>
    /// Serializes an e-Financeira message to XML.
    /// </summary>
    /// <param name="message">The message to serialize</param>
    /// <returns>XML string representation</returns>
    string SerializeMessage(IEFinanceiraMessage message);
}
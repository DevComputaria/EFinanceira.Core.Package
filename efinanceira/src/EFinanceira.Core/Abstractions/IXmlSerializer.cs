using System.Xml;

namespace EFinanceira.Core.Abstractions;

/// <summary>
/// Interface para serialização de objetos para XML
/// </summary>
public interface IXmlSerializer
{
    /// <summary>
    /// Serializa um objeto para XML
    /// </summary>
    /// <param name="root">Objeto raiz a ser serializado</param>
    /// <param name="settings">Configurações opcionais do XmlWriter</param>
    /// <returns>String XML serializada</returns>
    string Serialize(object root, XmlWriterSettings? settings = null);

    /// <summary>
    /// Deserializa um XML para objeto do tipo especificado
    /// </summary>
    /// <typeparam name="T">Tipo do objeto a ser deserializado</typeparam>
    /// <param name="xml">XML a ser deserializado</param>
    /// <returns>Objeto deserializado</returns>
    T Deserialize<T>(string xml) where T : class;
}

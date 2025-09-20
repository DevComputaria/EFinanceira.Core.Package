using EFinanceira.Core.Signing;

namespace EFinanceira.Core.Abstractions;

/// <summary>
/// Interface for XML digital signing using enveloped signatures with RSA-SHA256 and C14N.
/// </summary>
public interface IXmlSigner
{
    /// <summary>
    /// Signs an XML document with the specified options.
    /// </summary>
    /// <param name="xmlContent">The XML content to sign</param>
    /// <param name="options">Signing options including certificate and reference URI</param>
    /// <returns>The signed XML content</returns>
    string SignXml(string xmlContent, SignOptions options);

    /// <summary>
    /// Verifies the signature of a signed XML document.
    /// </summary>
    /// <param name="signedXml">The signed XML content</param>
    /// <returns>True if the signature is valid, false otherwise</returns>
    bool VerifySignature(string signedXml);
}
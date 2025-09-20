using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using EFinanceira.Core.Abstractions;

namespace EFinanceira.Core.Signing;

/// <summary>
/// XML signer implementation using enveloped signatures with RSA-SHA256 and C14N.
/// </summary>
public class XmlSigner : IXmlSigner
{
    public string SignXml(string xmlContent, SignOptions options)
    {
        if (string.IsNullOrEmpty(xmlContent))
            throw new ArgumentException("XML content cannot be null or empty", nameof(xmlContent));

        if (options?.Certificate == null)
            throw new ArgumentException("Certificate is required", nameof(options));

        var xmlDoc = new XmlDocument { PreserveWhitespace = true };
        xmlDoc.LoadXml(xmlContent);

        var signedXml = new SignedXml(xmlDoc);
        
        // Get the private key from the certificate
#pragma warning disable SYSLIB0028 // PrivateKey is obsolete but needed for compatibility
        var privateKey = options.Certificate.PrivateKey;
#pragma warning restore SYSLIB0028
        
        if (privateKey == null)
            throw new InvalidOperationException("Certificate does not contain a private key");
            
        signedXml.SigningKey = privateKey;

        // Create a reference to the document
        var reference = new Reference();
        reference.Uri = options.ReferenceUri ?? "";

        // Add an enveloped transformation to the reference
        var env = new XmlDsigEnvelopedSignatureTransform();
        reference.AddTransform(env);

        // Add C14N transformation
        var c14n = new XmlDsigC14NTransform();
        reference.AddTransform(c14n);

        // Set the digest algorithm
        reference.DigestMethod = options.DigestAlgorithm;

        signedXml.AddReference(reference);

        // Set canonicalization algorithm
        signedXml.SignedInfo.CanonicalizationMethod = options.CanonicalizationAlgorithm;

        // Set signature algorithm
        signedXml.SignedInfo.SignatureMethod = options.SignatureAlgorithm;

        // Include certificate if requested
        if (options.IncludeCertificate)
        {
            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(options.Certificate));
            signedXml.KeyInfo = keyInfo;
        }

        // Compute the signature
        signedXml.ComputeSignature();

        // Get the XML representation of the signature
        var xmlDigitalSignature = signedXml.GetXml();

        // Append the signature to the document
        xmlDoc.DocumentElement?.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));

        return xmlDoc.OuterXml;
    }

    public bool VerifySignature(string signedXml)
    {
        if (string.IsNullOrEmpty(signedXml))
            return false;

        try
        {
            var xmlDoc = new XmlDocument { PreserveWhitespace = true };
            xmlDoc.LoadXml(signedXml);

            var signedXmlDoc = new SignedXml(xmlDoc);
            var nodeList = xmlDoc.GetElementsByTagName("Signature");

            if (nodeList.Count == 0)
                return false;

            signedXmlDoc.LoadXml((XmlElement)nodeList[0]!);

            return signedXmlDoc.CheckSignature();
        }
        catch
        {
            return false;
        }
    }
}
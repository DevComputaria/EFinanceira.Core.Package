using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using EFinanceira.Core.Abstractions;

namespace EFinanceira.Core.Signing;

/// <summary>
/// Implementação de assinatura digital XML para e-Financeira
/// Utiliza enveloped signature com RSA-SHA256 e C14N 2001-03-15
/// </summary>
public sealed class XmlSigner : IXmlSigner
{
    /// <inheritdoc />
    public string Sign(string xml, SignOptions options)
    {
        options.Validate();

        var certificate = GetCertificate(options);
        var xmlDoc = LoadXmlDocument(xml);

        // Localizar elemento a ser assinado
        var elementToSign = FindElementToSign(xmlDoc, options);

        // Configurar assinatura
        var signedXml = new SignedXml(xmlDoc)
        {
            SigningKey = certificate.GetRSAPrivateKey()
        };

        // Criar referência para o elemento
        var reference = CreateReference(options);
        signedXml.AddReference(reference);

        // Configurar KeyInfo
        var keyInfo = CreateKeyInfo(certificate, options);
        signedXml.KeyInfo = keyInfo;

        // Computar assinatura
        signedXml.ComputeSignature();

        // Inserir assinatura no elemento
        var signatureElement = signedXml.GetXml();
        elementToSign.AppendChild(xmlDoc.ImportNode(signatureElement, true));

        return FormatXmlOutput(xmlDoc);
    }

    /// <inheritdoc />
    public bool VerifySignature(string signedXml)
    {
        try
        {
            var xmlDoc = LoadXmlDocument(signedXml);
            var signatureNodes = xmlDoc.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#");

            if (signatureNodes.Count == 0)
                return false;

            var signedXmlVerifier = new SignedXml(xmlDoc);
            signedXmlVerifier.LoadXml((XmlElement)signatureNodes[0]!);

            return signedXmlVerifier.CheckSignature();
        }
        catch
        {
            return false;
        }
    }

    private static X509Certificate2 GetCertificate(SignOptions options)
    {
        if (options.Certificate != null)
            return options.Certificate;

        return new X509Certificate2(options.PfxPath!, options.PfxPassword);
    }

    private static XmlDocument LoadXmlDocument(string xml)
    {
        var xmlDoc = new XmlDocument
        {
            PreserveWhitespace = true
        };
        xmlDoc.LoadXml(xml);
        return xmlDoc;
    }

    private static XmlElement FindElementToSign(XmlDocument xmlDoc, SignOptions options)
    {
        // Procurar por elemento com ID específico
        var elementWithId = xmlDoc.GetElementById(options.IdValue);
        if (elementWithId != null)
            return elementWithId;

        // Procurar por elemento com nome e atributo ID
        var xpath = $"//{options.ElementToSignName}[@{options.IdAttributeName}='{options.IdValue}']";
        var element = xmlDoc.SelectSingleNode(xpath) as XmlElement;

        if (element == null)
        {
            throw new InvalidOperationException($"Elemento {options.ElementToSignName} com {options.IdAttributeName}='{options.IdValue}' não encontrado");
        }

        return element;
    }

    private static Reference CreateReference(SignOptions options)
    {
        var reference = new Reference($"#{options.IdValue}")
        {
            DigestMethod = options.DigestMethod
        };

        // Adicionar transforms
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        reference.AddTransform(new XmlDsigC14NTransform());

        return reference;
    }

    private static KeyInfo CreateKeyInfo(X509Certificate2 certificate, SignOptions options)
    {
        var keyInfo = new KeyInfo();

        if (options.IncludeCertificate)
        {
            var x509Data = new KeyInfoX509Data();
            x509Data.AddCertificate(certificate);

            if (options.IncludeCertificateChain)
            {
                var chain = new X509Chain();
                chain.Build(certificate);

                foreach (var chainElement in chain.ChainElements)
                {
                    if (!chainElement.Certificate.Equals(certificate))
                    {
                        x509Data.AddCertificate(chainElement.Certificate);
                    }
                }
            }

            keyInfo.AddClause(x509Data);
        }

        return keyInfo;
    }

    private static string FormatXmlOutput(XmlDocument xmlDoc)
    {
        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
        {
            Indent = false,
            Encoding = System.Text.Encoding.UTF8,
            OmitXmlDeclaration = false
        });

        xmlDoc.Save(xmlWriter);
        return stringWriter.ToString();
    }
}

/// <summary>
/// Exceção específica para erros de assinatura
/// </summary>
public class XmlSigningException : Exception
{
    public XmlSigningException(string message) : base(message) { }
    public XmlSigningException(string message, Exception innerException) : base(message, innerException) { }
}

using System.Security.Cryptography.X509Certificates;

namespace EFinanceira.Core.Signing;

/// <summary>
/// Options for XML signing configuration.
/// </summary>
public class SignOptions
{
    /// <summary>
    /// The X.509 certificate to use for signing.
    /// </summary>
    public X509Certificate2 Certificate { get; set; } = null!;

    /// <summary>
    /// The reference URI for the signature. If null, signs the entire document.
    /// </summary>
    public string? ReferenceUri { get; set; }

    /// <summary>
    /// Whether to include the certificate in the signature.
    /// </summary>
    public bool IncludeCertificate { get; set; } = true;

    /// <summary>
    /// The signature algorithm to use. Defaults to RSA-SHA256.
    /// </summary>
    public string SignatureAlgorithm { get; set; } = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";

    /// <summary>
    /// The canonicalization algorithm to use. Defaults to C14N 2001.
    /// </summary>
    public string CanonicalizationAlgorithm { get; set; } = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";

    /// <summary>
    /// The digest algorithm to use. Defaults to SHA256.
    /// </summary>
    public string DigestAlgorithm { get; set; } = "http://www.w3.org/2001/04/xmlenc#sha256";
}
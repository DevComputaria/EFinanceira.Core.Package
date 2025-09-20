using System.Security.Cryptography.X509Certificates;

namespace EFinanceira.Core.Signing;

/// <summary>
/// Opções para assinatura digital de XML
/// </summary>
public sealed class SignOptions
{
    /// <summary>
    /// Caminho para arquivo PFX do certificado
    /// </summary>
    public string? PfxPath { get; set; }

    /// <summary>
    /// Senha do certificado PFX
    /// </summary>
    public string? PfxPassword { get; set; }

    /// <summary>
    /// Certificado X509 (alternativa ao PFX)
    /// </summary>
    public X509Certificate2? Certificate { get; set; }

    /// <summary>
    /// Nome do elemento a ser assinado (elemento raiz)
    /// </summary>
    public string ElementToSignName { get; set; } = string.Empty;

    /// <summary>
    /// Nome do atributo de ID do elemento a ser assinado
    /// </summary>
    public string IdAttributeName { get; set; } = "Id";

    /// <summary>
    /// Valor do ID do elemento a ser assinado
    /// </summary>
    public string IdValue { get; set; } = string.Empty;

    /// <summary>
    /// Algoritmo de canonicalização (padrão: C14N 2001-03-15)
    /// </summary>
    public string CanonicalizationMethod { get; set; } = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";

    /// <summary>
    /// Algoritmo de assinatura (padrão: RSA-SHA256)
    /// </summary>
    public string SignatureMethod { get; set; } = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";

    /// <summary>
    /// Algoritmo de digest (padrão: SHA256)
    /// </summary>
    public string DigestMethod { get; set; } = "http://www.w3.org/2001/04/xmlenc#sha256";

    /// <summary>
    /// Incluir certificado completo na assinatura
    /// </summary>
    public bool IncludeCertificate { get; set; } = true;

    /// <summary>
    /// Incluir cadeia de certificados
    /// </summary>
    public bool IncludeCertificateChain { get; set; }

    /// <summary>
    /// Valida se as opções estão completas
    /// </summary>
    public void Validate()
    {
        if (Certificate == null && (string.IsNullOrEmpty(PfxPath) || string.IsNullOrEmpty(PfxPassword)))
        {
            throw new InvalidOperationException("É necessário fornecer um certificado (Certificate) ou caminho/senha do PFX (PfxPath/PfxPassword)");
        }

        if (string.IsNullOrEmpty(ElementToSignName))
        {
            throw new InvalidOperationException("ElementToSignName é obrigatório");
        }

        if (string.IsNullOrEmpty(IdValue))
        {
            throw new InvalidOperationException("IdValue é obrigatório");
        }
    }
}

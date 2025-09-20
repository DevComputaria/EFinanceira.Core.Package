using EFinanceira.Core.Signing;

namespace EFinanceira.Core.Abstractions;

/// <summary>
/// Interface para assinatura digital de XMLs do e-Financeira
/// </summary>
public interface IXmlSigner
{
    /// <summary>
    /// Assina um XML usando certificado digital
    /// </summary>
    /// <param name="xml">XML a ser assinado</param>
    /// <param name="options">Opções de assinatura</param>
    /// <returns>XML assinado</returns>
    string Sign(string xml, SignOptions options);

    /// <summary>
    /// Verifica se um XML possui assinatura válida
    /// </summary>
    /// <param name="signedXml">XML assinado</param>
    /// <returns>True se a assinatura for válida</returns>
    bool VerifySignature(string signedXml);
}

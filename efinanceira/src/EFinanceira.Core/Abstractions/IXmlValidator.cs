namespace EFinanceira.Core.Abstractions;

/// <summary>
/// Interface para validação de XMLs contra schemas XSD
/// </summary>
public interface IXmlValidator
{
    /// <summary>
    /// Valida um XML contra os schemas XSD especificados
    /// </summary>
    /// <param name="xml">XML a ser validado</param>
    /// <param name="xsdPathsOrEmbedded">Caminhos para arquivos XSD ou recursos embarcados</param>
    /// <exception cref="XmlValidationException">Lançada quando o XML é inválido</exception>
    void Validate(string xml, IEnumerable<string> xsdPathsOrEmbedded);

    /// <summary>
    /// Valida um XML e retorna lista de erros sem lançar exceção
    /// </summary>
    /// <param name="xml">XML a ser validado</param>
    /// <param name="xsdPathsOrEmbedded">Caminhos para arquivos XSD</param>
    /// <returns>Lista de erros de validação (vazia se válido)</returns>
    IList<string> ValidateAndGetErrors(string xml, IEnumerable<string> xsdPathsOrEmbedded);
}

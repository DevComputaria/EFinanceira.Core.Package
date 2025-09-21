using System.ComponentModel.DataAnnotations;

namespace EFinanceira.Console.Sample.Configuration;

/// <summary>
/// Configurações principais da e-Financeira
/// </summary>
public class EFinanceiraSettings
{
    public const string SectionName = "EFinanceira";
    
    public string Environment { get; set; } = "Homologacao";
    
    [Required]
    public CertificateSettings Certificate { get; set; } = new();
    
    [Required]
    public DeclaranteSettings Declarante { get; set; } = new();
    
    public ValidationSettings Validation { get; set; } = new();
}

/// <summary>
/// Configurações do declarante
/// </summary>
public class DeclaranteSettings
{
    [Required]
    public string Cnpj { get; set; } = string.Empty;
    
    [Required]
    public string Nome { get; set; } = string.Empty;
}

/// <summary>
/// Configurações de certificados
/// </summary>
public class CertificateSettings
{
    [Required]
    public string PfxPath { get; set; } = string.Empty;
    
    [Required]
    public string PfxPassword { get; set; } = string.Empty;
}

/// <summary>
/// Configurações de validação
/// </summary>
public class ValidationSettings
{
    public string XsdBasePath { get; set; } = string.Empty;
}

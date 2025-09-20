using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace EFinanceira.Core.Models;

/// <summary>
/// Representa o envelope principal do arquivo XML da e-Financeira
/// </summary>
[XmlRoot("envEvtEFinanceira", Namespace = "http://www.portalfiscal.inf.br/e-financeira")]
public class EFinanceiraEnvelope
{
    /// <summary>
    /// Identificação do grupo de informações do evento
    /// </summary>
    [XmlElement("ideEvento")]
    [Required]
    public EventoIdentificacao IdeEvento { get; set; } = null!;

    /// <summary>
    /// Informações do responsável pela criação do arquivo
    /// </summary>
    [XmlElement("ideRespons")]
    [Required]
    public ResponsavelIdentificacao IdeRespons { get; set; } = null!;

    /// <summary>
    /// Lista de eventos da e-Financeira
    /// </summary>
    [XmlArray("evtEFinanceira")]
    [XmlArrayItem("evento")]
    public List<EventoEFinanceira> Eventos { get; set; } = new();
}

/// <summary>
/// Identificação do evento
/// </summary>
public class EventoIdentificacao
{
    /// <summary>
    /// Identificação do responsável pela criação do evento
    /// </summary>
    [XmlElement("cnpjRespons")]
    [Required]
    [StringLength(14, MinimumLength = 14)]
    public string CnpjRespons { get; set; } = string.Empty;

    /// <summary>
    /// Data e hora da criação do evento
    /// </summary>
    [XmlElement("dhEvento")]
    [Required]
    public DateTime DhEvento { get; set; }

    /// <summary>
    /// Sequencial único do evento
    /// </summary>
    [XmlElement("nrRecibo")]
    public string? NrRecibo { get; set; }

    /// <summary>
    /// Versão do layout
    /// </summary>
    [XmlElement("versaoEvento")]
    [Required]
    public string VersaoEvento { get; set; } = "1.0.0";
}

/// <summary>
/// Identificação do responsável
/// </summary>
public class ResponsavelIdentificacao
{
    /// <summary>
    /// CNPJ do responsável
    /// </summary>
    [XmlElement("cnpjRespons")]
    [Required]
    [StringLength(14, MinimumLength = 14)]
    public string CnpjRespons { get; set; } = string.Empty;

    /// <summary>
    /// Nome do responsável
    /// </summary>
    [XmlElement("nmRespons")]
    [Required]
    [StringLength(100)]
    public string NmRespons { get; set; } = string.Empty;

    /// <summary>
    /// CPF do responsável
    /// </summary>
    [XmlElement("cpfRespons")]
    [StringLength(11, MinimumLength = 11)]
    public string? CpfRespons { get; set; }

    /// <summary>
    /// Telefone do responsável
    /// </summary>
    [XmlElement("telefone")]
    [StringLength(20)]
    public string? Telefone { get; set; }

    /// <summary>
    /// Email do responsável
    /// </summary>
    [XmlElement("email")]
    [StringLength(100)]
    [EmailAddress]
    public string? Email { get; set; }
}
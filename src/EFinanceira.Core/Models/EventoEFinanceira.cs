using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace EFinanceira.Core.Models;

/// <summary>
/// Evento da e-Financeira
/// </summary>
public class EventoEFinanceira
{
    /// <summary>
    /// Identificação única do evento
    /// </summary>
    [XmlAttribute("id")]
    [Required]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Informações da conta
    /// </summary>
    [XmlElement("evtMovFinanceira")]
    public MovimentacaoFinanceira? EvtMovFinanceira { get; set; }

    /// <summary>
    /// Informações de abertura de conta
    /// </summary>
    [XmlElement("evtAberConta")]
    public AberturaConta? EvtAberConta { get; set; }

    /// <summary>
    /// Informações de fechamento de conta
    /// </summary>
    [XmlElement("evtFechConta")]
    public FechamentoConta? EvtFechConta { get; set; }
}

/// <summary>
/// Movimentação financeira
/// </summary>
public class MovimentacaoFinanceira
{
    /// <summary>
    /// Identificação da conta
    /// </summary>
    [XmlElement("ideConta")]
    [Required]
    public IdentificacaoConta IdeConta { get; set; } = null!;

    /// <summary>
    /// Período de referência
    /// </summary>
    [XmlElement("iniMovFin")]
    [Required]
    public PeriodoReferencia IniMovFin { get; set; } = null!;

    /// <summary>
    /// Lista de movimentações
    /// </summary>
    [XmlArray("movimentacao")]
    [XmlArrayItem("movimento")]
    public List<Movimento> Movimentacoes { get; set; } = new();
}

/// <summary>
/// Abertura de conta
/// </summary>
public class AberturaConta
{
    /// <summary>
    /// Identificação da conta
    /// </summary>
    [XmlElement("ideConta")]
    [Required]
    public IdentificacaoConta IdeConta { get; set; } = null!;

    /// <summary>
    /// Data de abertura
    /// </summary>
    [XmlElement("dtAbertura")]
    [Required]
    public DateTime DtAbertura { get; set; }

    /// <summary>
    /// Tipo de conta
    /// </summary>
    [XmlElement("tpConta")]
    [Required]
    public TipoConta TpConta { get; set; }
}

/// <summary>
/// Fechamento de conta
/// </summary>
public class FechamentoConta
{
    /// <summary>
    /// Identificação da conta
    /// </summary>
    [XmlElement("ideConta")]
    [Required]
    public IdentificacaoConta IdeConta { get; set; } = null!;

    /// <summary>
    /// Data de fechamento
    /// </summary>
    [XmlElement("dtFechamento")]
    [Required]
    public DateTime DtFechamento { get; set; }
}

/// <summary>
/// Identificação da conta
/// </summary>
public class IdentificacaoConta
{
    /// <summary>
    /// CNPJ da instituição financeira
    /// </summary>
    [XmlElement("cnpjInstituicao")]
    [Required]
    [StringLength(14, MinimumLength = 14)]
    public string CnpjInstituicao { get; set; } = string.Empty;

    /// <summary>
    /// Número da conta
    /// </summary>
    [XmlElement("numeroConta")]
    [Required]
    [StringLength(50)]
    public string NumeroConta { get; set; } = string.Empty;

    /// <summary>
    /// Dígito verificador da conta
    /// </summary>
    [XmlElement("digitoConta")]
    [StringLength(10)]
    public string? DigitoConta { get; set; }

    /// <summary>
    /// Tipo de conta
    /// </summary>
    [XmlElement("tpConta")]
    [Required]
    public TipoConta TpConta { get; set; }

    /// <summary>
    /// Subtipo de conta
    /// </summary>
    [XmlElement("subTpConta")]
    public SubtipoConta? SubTpConta { get; set; }

    /// <summary>
    /// Agência
    /// </summary>
    [XmlElement("agencia")]
    [StringLength(20)]
    public string? Agencia { get; set; }

    /// <summary>
    /// Dígito verificador da agência
    /// </summary>
    [XmlElement("digitoAgencia")]
    [StringLength(10)]
    public string? DigitoAgencia { get; set; }
}

/// <summary>
/// Período de referência
/// </summary>
public class PeriodoReferencia
{
    /// <summary>
    /// Data inicial
    /// </summary>
    [XmlElement("dtIni")]
    [Required]
    public DateTime DtIni { get; set; }

    /// <summary>
    /// Data final
    /// </summary>
    [XmlElement("dtFim")]
    [Required]
    public DateTime DtFim { get; set; }
}

/// <summary>
/// Movimento financeiro
/// </summary>
public class Movimento
{
    /// <summary>
    /// Data do movimento
    /// </summary>
    [XmlElement("dtMov")]
    [Required]
    public DateTime DtMov { get; set; }

    /// <summary>
    /// Tipo de movimento
    /// </summary>
    [XmlElement("tpMov")]
    [Required]
    public TipoMovimento TpMov { get; set; }

    /// <summary>
    /// Valor do movimento
    /// </summary>
    [XmlElement("valor")]
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Valor { get; set; }

    /// <summary>
    /// Descrição do movimento
    /// </summary>
    [XmlElement("descricao")]
    [StringLength(500)]
    public string? Descricao { get; set; }
}

/// <summary>
/// Tipos de conta
/// </summary>
public enum TipoConta
{
    /// <summary>
    /// Conta corrente
    /// </summary>
    [XmlEnum("01")]
    ContaCorrente = 1,

    /// <summary>
    /// Conta poupança
    /// </summary>
    [XmlEnum("02")]
    ContaPoupanca = 2,

    /// <summary>
    /// Conta de depósito a prazo
    /// </summary>
    [XmlEnum("03")]
    ContaDepositoPrazo = 3,

    /// <summary>
    /// Conta de investimento
    /// </summary>
    [XmlEnum("04")]
    ContaInvestimento = 4
}

/// <summary>
/// Subtipos de conta
/// </summary>
public enum SubtipoConta
{
    /// <summary>
    /// Conta individual
    /// </summary>
    [XmlEnum("01")]
    Individual = 1,

    /// <summary>
    /// Conta conjunta
    /// </summary>
    [XmlEnum("02")]
    Conjunta = 2,

    /// <summary>
    /// Conta corporativa
    /// </summary>
    [XmlEnum("03")]
    Corporativa = 3
}

/// <summary>
/// Tipos de movimento
/// </summary>
public enum TipoMovimento
{
    /// <summary>
    /// Crédito
    /// </summary>
    [XmlEnum("C")]
    Credito = 1,

    /// <summary>
    /// Débito
    /// </summary>
    [XmlEnum("D")]
    Debito = 2
}
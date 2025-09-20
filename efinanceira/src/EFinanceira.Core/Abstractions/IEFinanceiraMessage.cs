namespace EFinanceira.Core.Abstractions;

/// <summary>
/// Interface base para todas as mensagens do e-Financeira (eventos, lotes, consultas)
/// </summary>
public interface IEFinanceiraMessage
{
    /// <summary>
    /// Versão do schema da mensagem (ex: "v1_3_0")
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Nome do elemento raiz no XML (ex: "evtMovPP", "envioLoteEventos")
    /// </summary>
    string RootElementName { get; }

    /// <summary>
    /// Nome do atributo de ID quando aplicável (ex: "Id")
    /// </summary>
    string? IdAttributeName { get; }

    /// <summary>
    /// Valor do ID do elemento raiz (se houver)
    /// </summary>
    string? IdValue { get; }

    /// <summary>
    /// Objeto POCO gerado do XSD (elemento raiz)
    /// </summary>
    object Payload { get; }
}

using EFinanceira.Core.Abstractions;
using System.Xml;
using System.Xml.Schema;

namespace EFinanceira.Messages.Schemas;

/// <summary>
/// Validador para todos os schemas XSD do e-Financeira
/// </summary>
public class EFinanceiraSchemaValidator : IXmlValidator
{
    private static readonly Dictionary<string, XmlSchemaSet> _schemaCache = new();

    #region IXmlValidator Implementation

    /// <summary>
    /// Valida XML contra schemas especificados
    /// </summary>
    public bool Validate(string xml, IEnumerable<string> schemaFileNames)
    {
        var errors = ValidateAndGetErrors(xml, schemaFileNames);
        return !errors.Any();
    }

    /// <summary>
    /// Valida XML e retorna lista de erros
    /// </summary>
    public IEnumerable<string> ValidateAndGetErrors(string xml, IEnumerable<string> schemaFileNames)
    {
        var errors = new List<string>();

        try
        {
            var schemaSet = GetCombinedSchemaSet(schemaFileNames);
            
            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                Schemas = schemaSet
            };

            settings.ValidationEventHandler += (sender, e) =>
            {
                errors.Add($"{e.Severity}: {e.Message}");
            };

            using var stringReader = new StringReader(xml);
            using var xmlReader = XmlReader.Create(stringReader, settings);
            
            while (xmlReader.Read()) { /* Processa o XML */ }
        }
        catch (Exception ex)
        {
            errors.Add($"Erro durante validação: {ex.Message}");
        }

        return errors;
    }

    #endregion

    #region Métodos de Validação Específicos

    /// <summary>
    /// Valida XML de evento de abertura e-Financeira
    /// </summary>
    public bool ValidateEvtAberturaeFinanceira(string xml)
    {
        return Validate(xml, new[] { "evtAberturaeFinanceira-v1_2_1.xsd" });
    }

    /// <summary>
    /// Valida XML de evento de cadastro da empresa declarante
    /// </summary>
    public bool ValidateEvtCadEmpresaDeclarante(string xml)
    {
        return Validate(xml, new[] { "evtCadEmpresaDeclarante-v1_2_0.xsd" });
    }

    /// <summary>
    /// Valida XML de evento de intermediário
    /// </summary>
    public bool ValidateEvtIntermediario(string xml)
    {
        return Validate(xml, new[] { "evtIntermediario-v1_2_0.xsd" });
    }

    /// <summary>
    /// Valida XML de evento de patrocinado
    /// </summary>
    public bool ValidateEvtPatrocinado(string xml)
    {
        return Validate(xml, new[] { "evtPatrocinado-v1_2_0.xsd" });
    }

    /// <summary>
    /// Valida XML de evento de movimentação financeira
    /// </summary>
    public bool ValidateEvtMovimentacaoFinanceira(string xml)
    {
        return Validate(xml, new[] { "evtMovimentacaoFinanceira-v1_2_1.xsd" });
    }

    /// <summary>
    /// Valida XML de evento de movimentação financeira anual
    /// </summary>
    public bool ValidateEvtMovimentacaoFinanceiraAnual(string xml)
    {
        return Validate(xml, new[] { "evtMovimentacaoFinanceiraAnual-v1_2_2.xsd" });
    }

    /// <summary>
    /// Valida XML de evento de fechamento e-Financeira
    /// </summary>
    public bool ValidateEvtFechamentoeFinanceira(string xml)
    {
        return Validate(xml, new[] { "evtFechamentoeFinanceira-v1_2_2.xsd" });
    }

    /// <summary>
    /// Valida XML de evento de fechamento e-Financeira (versão alternativa)
    /// </summary>
    public bool ValidateEvtFechamentoeFinanceiraAlt(string xml)
    {
        return Validate(xml, new[] { "evtFechamentoeFinanceira-v1_2_2-alt.xsd" });
    }

    /// <summary>
    /// Valida XML de evento de exclusão
    /// </summary>
    public bool ValidateEvtExclusao(string xml)
    {
        return Validate(xml, new[] { "evtExclusao-v1_2_0.xsd" });
    }

    /// <summary>
    /// Valida XML de evento de exclusão e-Financeira
    /// </summary>
    public bool ValidateEvtExclusaoeFinanceira(string xml)
    {
        return Validate(xml, new[] { "evtExclusaoeFinanceira-v1_2_0.xsd" });
    }

    /// <summary>
    /// Valida XML de evento RERCT
    /// </summary>
    public bool ValidateEvtRERCT(string xml)
    {
        return Validate(xml, new[] { "evtRERCT-v1_2_0.xsd" });
    }

    /// <summary>
    /// Valida XML de evento de previdência privada
    /// </summary>
    public bool ValidateEvtPrevidenciaPrivada(string xml)
    {
        return Validate(xml, new[] { "evtPrevidenciaPrivada-v1_2_5.xsd" });
    }

    /// <summary>
    /// Valida XML de envio de lote de eventos
    /// </summary>
    public bool ValidateEnvioLoteEventos(string xml)
    {
        return Validate(xml, new[] { "envioLoteEventos-v1_2_0.xsd" });
    }

    /// <summary>
    /// Valida XML de envio de lote de eventos assíncrono
    /// </summary>
    public bool ValidateEnvioLoteEventosAssincrono(string xml)
    {
        return Validate(xml, new[] { "envioLoteEventosAssincrono-v1_0_0.xsd" });
    }

    /// <summary>
    /// Valida XML de envio de lote criptografado
    /// </summary>
    public bool ValidateEnvioLoteCriptografado(string xml)
    {
        return Validate(xml, new[] { "envioLoteCriptografado-v1_2_0.xsd" });
    }

    /// <summary>
    /// Valida XML de retorno de lote de eventos (v1.2.0)
    /// </summary>
    public bool ValidateRetornoLoteEventos(string xml)
    {
        return Validate(xml, new[] { "retornoLoteEventos-v1_2_0.xsd" });
    }

    /// <summary>
    /// Valida XML de retorno de lote de eventos (v1.3.0)
    /// </summary>
    public bool ValidateRetornoLoteEventosV130(string xml)
    {
        return Validate(xml, new[] { "retornoLoteEventos-v1_3_0.xsd" });
    }

    /// <summary>
    /// Valida XML de retorno de lote de eventos assíncrono
    /// </summary>
    public bool ValidateRetornoLoteEventosAssincrono(string xml)
    {
        return Validate(xml, new[] { "retornoLoteEventosAssincrono-v1_0_0.xsd" });
    }

    #region Consultas (mantendo compatibilidade com ConsultaSchemaValidator)

    /// <summary>
    /// Valida XML de retorno da consulta de informações cadastrais
    /// </summary>
    public bool ValidateRetInfoCadastral(string xml)
    {
        return Validate(xml, new[] { "retInfoCadastral-v1_2_0.xsd" });
    }

    /// <summary>
    /// Valida XML de retorno da consulta de informações de intermediário
    /// </summary>
    public bool ValidateRetInfoIntermediario(string xml)
    {
        return Validate(xml, new[] { "retInfoIntermediario-v1_2_0.xsd" });
    }

    /// <summary>
    /// Valida XML de retorno da consulta de informações de patrocinado
    /// </summary>
    public bool ValidateRetInfoPatrocinado(string xml)
    {
        return Validate(xml, new[] { "retInfoPatrocinado-v1_2_0.xsd" });
    }

    /// <summary>
    /// Valida XML de retorno da consulta de informações de movimento
    /// </summary>
    public bool ValidateRetInfoMovimento(string xml)
    {
        return Validate(xml, new[] { "retInfoMovimento-v1_2_0.xsd" });
    }

    /// <summary>
    /// Valida XML de retorno da consulta lista de e-Financeira
    /// </summary>
    public bool ValidateRetListaeFinanceira(string xml)
    {
        return Validate(xml, new[] { "retListaeFinanceira-v1_2_0.xsd" });
    }

    /// <summary>
    /// Valida XML de retorno da consulta do módulo específico RERCT
    /// </summary>
    public bool ValidateRetRERCT(string xml)
    {
        return Validate(xml, new[] { "retRERCT-v1_2_0.xsd" });
    }

    #endregion

    #endregion

    #region Métodos Auxiliares

    /// <summary>
    /// Obtém ou cria um conjunto de schemas combinado
    /// </summary>
    private XmlSchemaSet GetCombinedSchemaSet(IEnumerable<string> schemaFileNames)
    {
        var key = string.Join("|", schemaFileNames.OrderBy(x => x));
        
        if (_schemaCache.TryGetValue(key, out var cachedSchemaSet))
        {
            return cachedSchemaSet;
        }

        var schemaSet = new XmlSchemaSet();
        
        foreach (var schemaFileName in schemaFileNames)
        {
            try
            {
                using var schemaStream = EFinanceiraSchemas.GetSchemaStream(schemaFileName);
                using var schemaReader = XmlReader.Create(schemaStream);
                schemaSet.Add(null, schemaReader);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Erro ao carregar schema '{schemaFileName}': {ex.Message}", ex);
            }
        }

        schemaSet.Compile();
        _schemaCache[key] = schemaSet;
        
        return schemaSet;
    }

    /// <summary>
    /// Valida XML e retorna erros detalhados
    /// </summary>
    public ValidationResult ValidateWithDetails(string xml, IEnumerable<string> schemaFileNames)
    {
        var errors = ValidateAndGetErrors(xml, schemaFileNames).ToList();
        
        return new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors,
            SchemaFileNames = schemaFileNames.ToList()
        };
    }

    /// <summary>
    /// Limpa o cache de schemas
    /// </summary>
    public static void ClearSchemaCache()
    {
        _schemaCache.Clear();
    }

    #endregion

    /// <summary>
    /// Resultado de validação detalhado
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> SchemaFileNames { get; set; } = new();
    }
}

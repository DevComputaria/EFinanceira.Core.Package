using System.Reflection;

namespace EFinanceira.Messages.Schemas;

/// <summary>
/// Helper para acesso a todos os schemas XSD do e-Financeira incorporados no assembly
/// </summary>
public static class EFinanceiraSchemas
{
    private static readonly Assembly _assembly = typeof(EFinanceiraSchemas).Assembly;

    #region Eventos - Abertura
    
    /// <summary>
    /// Schema para evento de abertura e-Financeira (v1.2.1)
    /// </summary>
    public static Stream GetEvtAberturaeFinanceiraSchema()
    {
        return GetSchemaStream("evtAberturaeFinanceira-v1_2_1.xsd");
    }

    #endregion

    #region Eventos - Cadastro

    /// <summary>
    /// Schema para evento de cadastro da empresa declarante (v1.2.0)
    /// </summary>
    public static Stream GetEvtCadEmpresaDeclaranteSchema()
    {
        return GetSchemaStream("evtCadEmpresaDeclarante-v1_2_0.xsd");
    }

    /// <summary>
    /// Schema para evento de intermediário (v1.2.0)
    /// </summary>
    public static Stream GetEvtIntermediarioSchema()
    {
        return GetSchemaStream("evtIntermediario-v1_2_0.xsd");
    }

    /// <summary>
    /// Schema para evento de patrocinado (v1.2.0)
    /// </summary>
    public static Stream GetEvtPatrocinadoSchema()
    {
        return GetSchemaStream("evtPatrocinado-v1_2_0.xsd");
    }

    #endregion

    #region Eventos - Movimentação

    /// <summary>
    /// Schema para evento de movimentação financeira (v1.2.1)
    /// </summary>
    public static Stream GetEvtMovimentacaoFinanceiraSchema()
    {
        return GetSchemaStream("evtMovimentacaoFinanceira-v1_2_1.xsd");
    }

    /// <summary>
    /// Schema para evento de movimentação financeira anual (v1.2.2)
    /// </summary>
    public static Stream GetEvtMovimentacaoFinanceiraAnualSchema()
    {
        return GetSchemaStream("evtMovimentacaoFinanceiraAnual-v1_2_2.xsd");
    }

    #endregion

    #region Eventos - Fechamento e Exclusão

    /// <summary>
    /// Schema para evento de fechamento e-Financeira (v1.2.2)
    /// </summary>
    public static Stream GetEvtFechamentoeFinanceiraSchema()
    {
        return GetSchemaStream("evtFechamentoeFinanceira-v1_2_2.xsd");
    }

    /// <summary>
    /// Schema alternativo para evento de fechamento e-Financeira (v1.2.2)
    /// </summary>
    public static Stream GetEvtFechamentoeFinanceiraAltSchema()
    {
        return GetSchemaStream("evtFechamentoeFinanceira-v1_2_2-alt.xsd");
    }

    /// <summary>
    /// Schema para evento de exclusão (v1.2.0)
    /// </summary>
    public static Stream GetEvtExclusaoSchema()
    {
        return GetSchemaStream("evtExclusao-v1_2_0.xsd");
    }

    /// <summary>
    /// Schema para evento de exclusão e-Financeira (v1.2.0)
    /// </summary>
    public static Stream GetEvtExclusaoeFinanceiraSchema()
    {
        return GetSchemaStream("evtExclusaoeFinanceira-v1_2_0.xsd");
    }

    #endregion

    #region Eventos - Específicos

    /// <summary>
    /// Schema para evento RERCT (v1.2.0)
    /// </summary>
    public static Stream GetEvtRERCTSchema()
    {
        return GetSchemaStream("evtRERCT-v1_2_0.xsd");
    }

    /// <summary>
    /// Schema para evento de previdência privada (v1.2.5)
    /// </summary>
    public static Stream GetEvtPrevidenciaPrivadaSchema()
    {
        return GetSchemaStream("evtPrevidenciaPrivada-v1_2_5.xsd");
    }

    #endregion

    #region Envio de Lotes

    /// <summary>
    /// Schema para envio de lote de eventos (v1.2.0)
    /// </summary>
    public static Stream GetEnvioLoteEventosSchema()
    {
        return GetSchemaStream("envioLoteEventos-v1_2_0.xsd");
    }

    /// <summary>
    /// Schema para envio de lote de eventos assíncrono (v1.0.0)
    /// </summary>
    public static Stream GetEnvioLoteEventosAssincronoSchema()
    {
        return GetSchemaStream("envioLoteEventosAssincrono-v1_0_0.xsd");
    }

    /// <summary>
    /// Schema para envio de lote criptografado (v1.2.0)
    /// </summary>
    public static Stream GetEnvioLoteCriptografadoSchema()
    {
        return GetSchemaStream("envioLoteCriptografado-v1_2_0.xsd");
    }

    #endregion

    #region Retorno de Lotes

    /// <summary>
    /// Schema para retorno de lote de eventos (v1.2.0)
    /// </summary>
    public static Stream GetRetornoLoteEventosSchema()
    {
        return GetSchemaStream("retornoLoteEventos-v1_2_0.xsd");
    }

    /// <summary>
    /// Schema para retorno de lote de eventos (v1.3.0)
    /// </summary>
    public static Stream GetRetornoLoteEventosV130Schema()
    {
        return GetSchemaStream("retornoLoteEventos-v1_3_0.xsd");
    }

    /// <summary>
    /// Schema para retorno de lote de eventos assíncrono (v1.0.0)
    /// </summary>
    public static Stream GetRetornoLoteEventosAssincronoSchema()
    {
        return GetSchemaStream("retornoLoteEventosAssincrono-v1_0_0.xsd");
    }

    #endregion

    #region Consultas - Retornos

    /// <summary>
    /// Schema para retorno da consulta de informações cadastrais (v1.2.0)
    /// </summary>
    public static Stream GetRetInfoCadastralSchema()
    {
        return GetSchemaStream("retInfoCadastral-v1_2_0.xsd");
    }

    /// <summary>
    /// Schema para retorno da consulta de informações de intermediário (v1.2.0)
    /// </summary>
    public static Stream GetRetInfoIntermediarioSchema()
    {
        return GetSchemaStream("retInfoIntermediario-v1_2_0.xsd");
    }

    /// <summary>
    /// Schema para retorno da consulta de informações de patrocinado (v1.2.0)
    /// </summary>
    public static Stream GetRetInfoPatrocinadoSchema()
    {
        return GetSchemaStream("retInfoPatrocinado-v1_2_0.xsd");
    }

    /// <summary>
    /// Schema para retorno da consulta de informações de movimento (v1.2.0)
    /// </summary>
    public static Stream GetRetInfoMovimentoSchema()
    {
        return GetSchemaStream("retInfoMovimento-v1_2_0.xsd");
    }

    /// <summary>
    /// Schema para retorno da consulta lista de e-Financeira (v1.2.0)
    /// </summary>
    public static Stream GetRetListaeFinanceiraSchema()
    {
        return GetSchemaStream("retListaeFinanceira-v1_2_0.xsd");
    }

    /// <summary>
    /// Schema para retorno da consulta do módulo específico RERCT (v1.2.0)
    /// </summary>
    public static Stream GetRetRERCTSchema()
    {
        return GetSchemaStream("retRERCT-v1_2_0.xsd");
    }

    #endregion

    #region Assinatura Digital

    /// <summary>
    /// Schema para assinatura digital XML (xmldsig-core-schema)
    /// </summary>
    public static Stream GetXmldsigCoreSchema()
    {
        return GetSchemaStream("xmldsig-core-schema.xsd");
    }

    #endregion

    #region Métodos Utilitários

    /// <summary>
    /// Obtém o conteúdo de um schema como string
    /// </summary>
    public static string GetSchemaContent(string schemaFileName)
    {
        using var stream = GetSchemaStream(schemaFileName);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Lista todos os schemas disponíveis
    /// </summary>
    public static IEnumerable<string> GetAvailableSchemas()
    {
        return new[]
        {
            // Eventos
            "evtAberturaeFinanceira-v1_2_1.xsd",
            "evtCadEmpresaDeclarante-v1_2_0.xsd",
            "evtIntermediario-v1_2_0.xsd",
            "evtPatrocinado-v1_2_0.xsd",
            "evtMovimentacaoFinanceira-v1_2_1.xsd",
            "evtMovimentacaoFinanceiraAnual-v1_2_2.xsd",
            "evtFechamentoeFinanceira-v1_2_2.xsd",
            "evtFechamentoeFinanceira-v1_2_2-alt.xsd",
            "evtExclusao-v1_2_0.xsd",
            "evtExclusaoeFinanceira-v1_2_0.xsd",
            "evtRERCT-v1_2_0.xsd",
            "evtPrevidenciaPrivada-v1_2_5.xsd",
            
            // Envio de Lotes
            "envioLoteEventos-v1_2_0.xsd",
            "envioLoteEventosAssincrono-v1_0_0.xsd",
            "envioLoteCriptografado-v1_2_0.xsd",
            
            // Retorno de Lotes
            "retornoLoteEventos-v1_2_0.xsd",
            "retornoLoteEventos-v1_3_0.xsd",
            "retornoLoteEventosAssincrono-v1_0_0.xsd",
            
            // Consultas
            "retInfoCadastral-v1_2_0.xsd",
            "retInfoIntermediario-v1_2_0.xsd",
            "retInfoPatrocinado-v1_2_0.xsd",
            "retInfoMovimento-v1_2_0.xsd",
            "retListaeFinanceira-v1_2_0.xsd",
            "retRERCT-v1_2_0.xsd",
            
            // Assinatura
            "xmldsig-core-schema.xsd"
        };
    }

    /// <summary>
    /// Obtém schemas por categoria
    /// </summary>
    public static IEnumerable<string> GetSchemasByCategory(string category)
    {
        return category.ToLowerInvariant() switch
        {
            "eventos" => GetAvailableSchemas().Where(s => s.StartsWith("evt")),
            "lotes" => GetAvailableSchemas().Where(s => s.StartsWith("envio") || s.StartsWith("retorno")),
            "consultas" => GetAvailableSchemas().Where(s => s.StartsWith("ret") && !s.StartsWith("retorno")),
            "assinatura" => GetAvailableSchemas().Where(s => s.Contains("xmldsig")),
            _ => Enumerable.Empty<string>()
        };
    }

    /// <summary>
    /// Obtém um stream para o schema especificado
    /// </summary>
    public static Stream GetSchemaStream(string schemaFileName)
    {
        var resourceName = $"EFinanceira.Messages.Schemas.{schemaFileName}";
        
        var stream = _assembly.GetManifestResourceStream(resourceName);
        
        if (stream == null)
        {
            var availableResources = _assembly.GetManifestResourceNames()
                .Where(r => r.Contains("Schemas"))
                .ToArray();
                
            throw new FileNotFoundException(
                $"Schema '{schemaFileName}' não encontrado nos recursos incorporados.\n" +
                $"Recurso esperado: {resourceName}\n" +
                $"Recursos disponíveis: {string.Join(", ", availableResources)}");
        }

        return stream;
    }

    #endregion
}

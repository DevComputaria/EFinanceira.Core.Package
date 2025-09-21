using System.Reflection;

namespace EFinanceira.Messages.Schemas;

/// <summary>
/// Helper para acesso aos schemas XSD de consulta incorporados no assembly
/// </summary>
public static class ConsultaSchemas
{
    private static readonly Assembly _assembly = typeof(ConsultaSchemas).Assembly;

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
    /// Lista todos os schemas de consulta disponíveis
    /// </summary>
    public static IEnumerable<string> GetAvailableSchemas()
    {
        return new[]
        {
            "retInfoCadastral-v1_2_0.xsd",
            "retInfoIntermediario-v1_2_0.xsd", 
            "retInfoPatrocinado-v1_2_0.xsd",
            "retInfoMovimento-v1_2_0.xsd",
            "retListaeFinanceira-v1_2_0.xsd",
            "retRERCT-v1_2_0.xsd"
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
            throw new FileNotFoundException(
                $"Schema '{schemaFileName}' não encontrado nos recursos incorporados. " +
                $"Recurso esperado: {resourceName}");
        }

        return stream;
    }
}

using System.Xml;
using EFinanceira.Core.Factory;
using EFinanceira.Messages.Extensions;
using EFinanceira.Messages.Generated.Lotes.RetornoLoteEventos_v1_2_0;

namespace EFinanceira.Examples.Lotes.RetornoLoteEventos;

/// <summary>
/// Exemplos práticos de uso do RetornoLoteEventosBuilder
/// </summary>
public static class RetornoLoteEventosExamples
{
    /// <summary>
    /// Exemplo 1: Criando um retorno de sucesso simples
    /// </summary>
    public static void ExemploRetornoSucesso()
    {
        var factory = new EFinanceiraMessageFactory();

        var retorno = factory
            .CriarRetornoLoteEventos()
            .ComIdTransmissor("12345678000195")
            .ComSucesso("Lote processado com sucesso")
            .ComId("LOTE001")
            .Build();

        Console.WriteLine($"Status: {retorno.EFinanceira.retornoLoteEventos.status.cdStatus}");
        Console.WriteLine($"Descrição: {retorno.EFinanceira.retornoLoteEventos.status.descRetorno}");
        Console.WriteLine($"Sucesso: {retorno.IsSuccessful()}");
    }

    /// <summary>
    /// Exemplo 2: Criando um retorno com erros
    /// </summary>
    public static void ExemploRetornoComErros()
    {
        var factory = new EFinanceiraMessageFactory();

        var retorno = factory
            .CriarRetornoLoteEventos()
            .ComIdTransmissor("12345678000195")
            .ComErroProcessamento("001", "Erro no processamento do lote")
            .ComErro("E001", "CNPJ inválido", "evento[1]/ideDeclarante/cnpjDeclarante")
            .ComErro("E002", "Data inválida", "evento[2]/dataOperacao")
            .ComAviso("W001", "Campo opcional não informado", "evento[1]/observacoes")
            .ComId("LOTE002")
            .Build();

        Console.WriteLine($"Status: {retorno.EFinanceira.retornoLoteEventos.status.cdStatus}");
        Console.WriteLine($"Sucesso: {retorno.IsSuccessful()}");

        Console.WriteLine("\nErros encontrados:");
        foreach (var erro in retorno.GetErros())
        {
            Console.WriteLine($"- {erro.codigo}: {erro.descricao} (Local: {erro.localizacaoErroAviso})");
        }

        Console.WriteLine("\nAvisos encontrados:");
        foreach (var aviso in retorno.GetAvisos())
        {
            Console.WriteLine($"- {aviso.codigo}: {aviso.descricao} (Local: {aviso.localizacaoErroAviso})");
        }
    }

    /// <summary>
    /// Exemplo 3: Criando retorno com eventos processados
    /// </summary>
    public static void ExemploRetornoComEventos()
    {
        var factory = new EFinanceiraMessageFactory();

        // Simular XMLs de eventos processados
        var eventoXml1 = CreateSampleEventXml("EVT001", "Evento processado");
        var eventoXml2 = CreateSampleEventXml("EVT002", "Evento processado");

        var retorno = factory
            .CriarRetornoLoteEventos()
            .ComIdTransmissor("12345678000195")
            .ComSucesso("Lote processado parcialmente")
            .AdicionarEvento(eventoXml1, "evento-001")
            .AdicionarEvento(eventoXml2, "evento-002")
            .ComAviso("W001", "Alguns campos opcionais não foram preenchidos")
            .ComId("LOTE003")
            .Build();

        Console.WriteLine($"Status: {retorno.EFinanceira.retornoLoteEventos.status.cdStatus}");
        Console.WriteLine($"Eventos retornados: {retorno.GetEventos().Count()}");

        foreach (var evento in retorno.GetEventos())
        {
            Console.WriteLine($"- Evento ID: {evento.id}");
        }
    }

    /// <summary>
    /// Exemplo 4: Parsing de XML de retorno existente
    /// </summary>
    public static void ExemploParsingXml()
    {
        var factory = new EFinanceiraMessageFactory();

        var xmlRetorno = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<eFinanceira xmlns=""http://www.eFinanceira.gov.br/schemas/retornoLoteEventos/v1_2_0"">
    <retornoLoteEventos id=""LOTE004"">
        <ideTransmissor>
            <IdTransmissor>12345678000195</IdTransmissor>
        </ideTransmissor>
        <status>
            <cdStatus>0</cdStatus>
            <descRetorno>Processado com sucesso</descRetorno>
        </status>
    </retornoLoteEventos>
</eFinanceira>";

        try
        {
            var retornoBuilder = factory.ParseRetornoLoteEventos(xmlRetorno);
            var retorno = retornoBuilder.Build();

            Console.WriteLine($"XML parseado com sucesso!");
            Console.WriteLine($"ID Transmissor: {retorno.EFinanceira.retornoLoteEventos.ideTransmissor.IdTransmissor}");
            Console.WriteLine($"Status: {retorno.EFinanceira.retornoLoteEventos.status.cdStatus}");
            Console.WriteLine($"Sucesso: {retorno.IsSuccessful()}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao fazer parse do XML: {ex.Message}");
        }
    }

    /// <summary>
    /// Exemplo 5: Retorno com múltiplas ocorrências organizadas
    /// </summary>
    public static void ExemploOcorrenciasComplexas()
    {
        var factory = new EFinanceiraMessageFactory();

        var ocorrencias = new List<TRegistroOcorrenciasOcorrencias>
        {
            new() { tipo = "1", codigo = "E001", descricao = "CNPJ inválido", localizacaoErroAviso = "evento[1]" },
            new() { tipo = "1", codigo = "E002", descricao = "Data futura", localizacaoErroAviso = "evento[2]" },
            new() { tipo = "2", codigo = "W001", descricao = "Campo recomendado", localizacaoErroAviso = "evento[3]" }
        };

        var retorno = factory
            .CriarRetornoLoteEventos()
            .ComIdTransmissor("98765432000198")
            .ComStatus("1", "Processado com erros")
            .AdicionarOcorrencias(ocorrencias)
            .ComId("LOTE005")
            .Build();

        Console.WriteLine($"Status: {retorno.EFinanceira.retornoLoteEventos.status.cdStatus}");
        Console.WriteLine($"Total de ocorrências: {retorno.EFinanceira.retornoLoteEventos.status.dadosRegistroOcorrenciaLote?.Length ?? 0}");
        Console.WriteLine($"Erros: {retorno.GetErros().Count()}");
        Console.WriteLine($"Avisos: {retorno.GetAvisos().Count()}");
    }

    /// <summary>
    /// Exemplo 6: Construção fluente com limpeza e reconstrução
    /// </summary>
    public static void ExemploConstrucaoFluente()
    {
        var factory = new EFinanceiraMessageFactory();

        var builder = factory.CriarRetornoLoteEventos();

        // Primeira versão com erros
        builder
            .ComIdTransmissor("11111111000111")
            .ComErroProcessamento("999", "Erro inicial")
            .ComErro("E999", "Erro temporário");

        // Limpar e reconstruir
        var retorno = builder
            .LimparOcorrencias()
            .ComSucesso("Reprocessado com sucesso")
            .ComAviso("W100", "Reprocessamento realizado")
            .ComId("LOTE006")
            .Build();

        Console.WriteLine($"Status final: {retorno.EFinanceira.retornoLoteEventos.status.cdStatus}");
        Console.WriteLine($"Sucesso: {retorno.IsSuccessful()}");
        Console.WriteLine($"Avisos: {retorno.GetAvisos().Count()}");
    }

    /// <summary>
    /// Método auxiliar para criar XML de exemplo
    /// </summary>
    private static XmlElement CreateSampleEventXml(string eventId, string status)
    {
        var xmlDoc = new XmlDocument();
        var eventoElement = xmlDoc.CreateElement("evento", "http://www.eFinanceira.gov.br/schemas/evtAberturaeFinanceira/v1_2_1");
        eventoElement.SetAttribute("id", eventId);

        var statusElement = xmlDoc.CreateElement("status", "http://www.eFinanceira.gov.br/schemas/evtAberturaeFinanceira/v1_2_1");
        statusElement.InnerText = status;
        eventoElement.AppendChild(statusElement);

        xmlDoc.AppendChild(eventoElement);
        return xmlDoc.DocumentElement!;
    }
}
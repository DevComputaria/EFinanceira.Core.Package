using System.Xml;
using EFinanceira.Core.Factory;
using EFinanceira.Messages.Extensions;
using EFinanceira.Messages.Generated.Lotes.RetornoLoteEventosAssincrono;

namespace EFinanceira.Examples.Lotes.RetornoLoteEventosAssincrono;

/// <summary>
/// Exemplos práticos de uso do RetornoLoteEventosAssincronoBuilder
/// </summary>
public static class RetornoLoteEventosAssincronoExamples
{
    /// <summary>
    /// Exemplo 1: Criando um retorno de sucesso simples
    /// </summary>
    public static void ExemploRetornoSucesso()
    {
        var factory = new EFinanceiraMessageFactory();

        var retorno = factory
            .CriarRetornoLoteEventosAssincrono()
            .ComCnpjDeclarante("12345678000195")
            .ComDadosRecepcao(
                dhRecepcao: DateTime.Now.AddMinutes(-10),
                versaoAplicativoRecepcao: "1.0.0",
                protocoloEnvio: "PROT001"
            )
            .ComDadosProcessamento(
                dhProcessamento: DateTime.Now,
                versaoAplicativoProcessamento: "2.0.0"
            )
            .ComSucesso("Lote processado com sucesso")
            .ComId("LOTE001")
            .Build();

        Console.WriteLine($"Status: {retorno.EFinanceira.retornoLoteEventosAssincrono.status.cdResposta}");
        Console.WriteLine($"Descrição: {retorno.EFinanceira.retornoLoteEventosAssincrono.status.descResposta}");
        Console.WriteLine($"Sucesso: {retorno.IsSuccessful()}");
        Console.WriteLine($"CNPJ: {retorno.GetCnpjDeclarante()}");
        Console.WriteLine($"Protocolo: {retorno.GetProtocoloEnvio()}");
    }

    /// <summary>
    /// Exemplo 2: Criando um retorno com erros
    /// </summary>
    public static void ExemploRetornoComErros()
    {
        var factory = new EFinanceiraMessageFactory();

        var retorno = factory
            .CriarRetornoLoteEventosAssincrono()
            .ComCnpjDeclarante("98765432000198")
            .ComDadosRecepcao(
                dhRecepcao: DateTime.Now.AddMinutes(-20),
                versaoAplicativoRecepcao: "1.0.0",
                protocoloEnvio: "PROT002"
            )
            .ComDadosProcessamento(
                dhProcessamento: DateTime.Now.AddMinutes(-5),
                versaoAplicativoProcessamento: "2.0.0"
            )
            .ComErroProcessamento(1, "Erro no processamento do lote")
            .ComErro("E001", "CNPJ inválido", "cnpjDeclarante")
            .ComErro("E002", "Formato de data inválido", "evento[1]/dataOperacao")
            .ComAviso("W001", "Campo opcional não informado", "evento[2]/observacoes")
            .ComId("LOTE002")
            .Build();

        Console.WriteLine($"Status: {retorno.EFinanceira.retornoLoteEventosAssincrono.status.cdResposta}");
        Console.WriteLine($"Sucesso: {retorno.IsSuccessful()}");

        Console.WriteLine("\nErros encontrados:");
        foreach (var erro in retorno.GetErros())
        {
            Console.WriteLine($"- {erro.codigo}: {erro.descricao} (Local: {erro.localizacao})");
        }

        Console.WriteLine("\nAvisos encontrados:");
        foreach (var aviso in retorno.GetAvisos())
        {
            Console.WriteLine($"- {aviso.codigo}: {aviso.descricao} (Local: {aviso.localizacao})");
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
            .CriarRetornoLoteEventosAssincrono()
            .ComCnpjDeclarante("11111111000111")
            .ComDadosRecepcao(
                dhRecepcao: DateTime.Now.AddHours(-1),
                versaoAplicativoRecepcao: "1.0.0",
                protocoloEnvio: "PROT003"
            )
            .ComDadosProcessamento(
                dhProcessamento: DateTime.Now.AddMinutes(-30),
                versaoAplicativoProcessamento: "2.0.0"
            )
            .ComSucesso("Lote processado parcialmente")
            .AdicionarEvento(eventoXml1, "evento-001")
            .AdicionarEvento(eventoXml2, "evento-002")
            .ComAviso("W001", "Alguns campos opcionais não foram preenchidos")
            .ComId("LOTE003")
            .Build();

        Console.WriteLine($"Status: {retorno.EFinanceira.retornoLoteEventosAssincrono.status.cdResposta}");
        Console.WriteLine($"Eventos retornados: {retorno.GetEventos().Count()}");

        var dadosRecepcao = retorno.GetDadosRecepcao();
        if (dadosRecepcao != null)
        {
            Console.WriteLine($"Data recepção: {dadosRecepcao.dhRecepcao}");
            Console.WriteLine($"Versão app recepção: {dadosRecepcao.versaoAplicativoRecepcao}");
        }

        var dadosProcessamento = retorno.GetDadosProcessamento();
        if (dadosProcessamento != null)
        {
            Console.WriteLine($"Data processamento: {dadosProcessamento.dhProcessamento}");
            Console.WriteLine($"Versão app processamento: {dadosProcessamento.versaoAplicativoProcessamentoLote}");
        }

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
<eFinanceira xmlns=""http://www.eFinanceira.gov.br/schemas/retornoLoteEventosAssincrono/v1_0_0"">
    <retornoLoteEventosAssincrono id=""LOTE004"">
        <cnpjDeclarante>12345678000195</cnpjDeclarante>
        <status>
            <cdResposta>0</cdResposta>
            <descResposta>Processado com sucesso</descResposta>
        </status>
        <dadosRecepcaoLote>
            <dhRecepcao>2024-01-15T10:00:00</dhRecepcao>
            <versaoAplicativoRecepcao>1.0.0</versaoAplicativoRecepcao>
            <protocoloEnvio>PROT004</protocoloEnvio>
        </dadosRecepcaoLote>
        <dadosProcessamentoLote>
            <dhProcessamento>2024-01-15T10:30:00</dhProcessamento>
            <versaoAplicativoProcessamentoLote>2.0.0</versaoAplicativoProcessamentoLote>
        </dadosProcessamentoLote>
    </retornoLoteEventosAssincrono>
</eFinanceira>";

        try
        {
            var retornoBuilder = factory.ParseRetornoLoteEventosAssincrono(xmlRetorno);
            var retorno = retornoBuilder.Build();

            Console.WriteLine($"XML parseado com sucesso!");
            Console.WriteLine($"CNPJ: {retorno.GetCnpjDeclarante()}");
            Console.WriteLine($"Status: {retorno.EFinanceira.retornoLoteEventosAssincrono.status.cdResposta}");
            Console.WriteLine($"Sucesso: {retorno.IsSuccessful()}");
            Console.WriteLine($"Protocolo: {retorno.GetProtocoloEnvio()}");

            var dadosRecepcao = retorno.GetDadosRecepcao();
            if (dadosRecepcao != null)
            {
                Console.WriteLine($"Data recepção: {dadosRecepcao.dhRecepcao}");
                Console.WriteLine($"Versão app: {dadosRecepcao.versaoAplicativoRecepcao}");
            }
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

        var ocorrencias = new List<TOcorrenciasOcorrencia>
        {
            new() { codigo = "E001", descricao = "CNPJ inválido", tipo = 1, localizacao = "cnpjDeclarante" },
            new() { codigo = "E002", descricao = "Data futura", tipo = 1, localizacao = "evento[1]/dataOperacao" },
            new() { codigo = "W001", descricao = "Campo recomendado", tipo = 2, localizacao = "evento[2]/observacoes" }
        };

        var retorno = factory
            .CriarRetornoLoteEventosAssincrono()
            .ComCnpjDeclarante("22222222000222")
            .ComDadosRecepcao(
                dhRecepcao: DateTime.Now.AddMinutes(-40),
                versaoAplicativoRecepcao: "1.0.0",
                protocoloEnvio: "PROT005"
            )
            .ComDadosProcessamento(
                dhProcessamento: DateTime.Now.AddMinutes(-20),
                versaoAplicativoProcessamento: "2.0.0"
            )
            .ComStatus(1, "Processado com erros")
            .AdicionarOcorrencias(ocorrencias)
            .ComId("LOTE005")
            .Build();

        Console.WriteLine($"Status: {retorno.EFinanceira.retornoLoteEventosAssincrono.status.cdResposta}");
        Console.WriteLine($"Total de ocorrências: {retorno.EFinanceira.retornoLoteEventosAssincrono.status.ocorrencias?.Length ?? 0}");
        Console.WriteLine($"Erros: {retorno.GetErros().Count()}");
        Console.WriteLine($"Avisos: {retorno.GetAvisos().Count()}");
    }

    /// <summary>
    /// Exemplo 6: Construção fluente com limpeza e reconstrução
    /// </summary>
    public static void ExemploConstrucaoFluente()
    {
        var factory = new EFinanceiraMessageFactory();

        var builder = factory.CriarRetornoLoteEventosAssincrono();

        // Primeira versão com erros
        builder
            .ComCnpjDeclarante("33333333000333")
            .ComDadosRecepcao(
                dhRecepcao: DateTime.Now.AddHours(-2),
                versaoAplicativoRecepcao: "1.0.0",
                protocoloEnvio: "PROT006"
            )
            .ComErroProcessamento(999, "Erro inicial")
            .ComErro("E999", "Erro temporário");

        // Limpar e reconstruir
        var retorno = builder
            .LimparOcorrencias()
            .ComDadosProcessamento(
                dhProcessamento: DateTime.Now.AddMinutes(-10),
                versaoAplicativoProcessamento: "2.0.0"
            )
            .ComSucesso("Reprocessado com sucesso")
            .ComAviso("W100", "Reprocessamento realizado")
            .ComId("LOTE006")
            .Build();

        Console.WriteLine($"Status final: {retorno.EFinanceira.retornoLoteEventosAssincrono.status.cdResposta}");
        Console.WriteLine($"Sucesso: {retorno.IsSuccessful()}");
        Console.WriteLine($"Avisos: {retorno.GetAvisos().Count()}");
    }

    /// <summary>
    /// Exemplo 7: Exemplo completo com todos os campos
    /// </summary>
    public static void ExemploCompleto()
    {
        var factory = new EFinanceiraMessageFactory();

        var eventoXml = CreateSampleEventXml("EVT007", "Evento processado completamente");

        var retorno = factory
            .CriarRetornoLoteEventosAssincrono()
            .ComCnpjDeclarante("44444444000444")
            .ComDadosRecepcao(
                dhRecepcao: new DateTime(2024, 1, 15, 10, 0, 0),
                versaoAplicativoRecepcao: "1.0.0",
                protocoloEnvio: "PROT007"
            )
            .ComDadosProcessamento(
                dhProcessamento: new DateTime(2024, 1, 15, 10, 30, 0),
                versaoAplicativoProcessamento: "2.0.0"
            )
            .ComSucesso("Lote processado e entregue com sucesso")
            .AdicionarEvento(eventoXml, "evento-007")
            .ComAviso("W001", "Processamento executado fora do horário comercial")
            .ComId("LOTE007")
            .Build();

        Console.WriteLine("=== Exemplo Completo ===");
        Console.WriteLine($"CNPJ Declarante: {retorno.GetCnpjDeclarante()}");
        Console.WriteLine($"Status: {retorno.EFinanceira.retornoLoteEventosAssincrono.status.cdResposta}");
        Console.WriteLine($"Sucesso: {retorno.IsSuccessful()}");

        var dadosRecepcao = retorno.GetDadosRecepcao();
        if (dadosRecepcao != null)
        {
            Console.WriteLine($"Data Recepção: {dadosRecepcao.dhRecepcao}");
            Console.WriteLine($"Protocolo: {dadosRecepcao.protocoloEnvio}");
            Console.WriteLine($"Versão App Recepção: {dadosRecepcao.versaoAplicativoRecepcao}");
        }

        var dadosProcessamento = retorno.GetDadosProcessamento();
        if (dadosProcessamento != null)
        {
            Console.WriteLine($"Data Processamento: {dadosProcessamento.dhProcessamento}");
            Console.WriteLine($"Versão App Processamento: {dadosProcessamento.versaoAplicativoProcessamentoLote}");
        }

        Console.WriteLine($"Total Eventos: {retorno.GetEventos().Count()}");
        Console.WriteLine($"Total Avisos: {retorno.GetAvisos().Count()}");
    }

    /// <summary>
    /// Exemplo 8: Comparação com outros builders de retorno
    /// </summary>
    public static void ExemploComparacaoBuilders()
    {
        var factory = new EFinanceiraMessageFactory();

        Console.WriteLine("=== Comparação de Builders de Retorno ===");
        Console.WriteLine("\nRetornoLoteEventosAssincrono v1.0.0:");

        var retornoAssincrono = factory
            .CriarRetornoLoteEventosAssincrono()
            .ComCnpjDeclarante("55555555000555")
            .ComDadosRecepcao(
                dhRecepcao: DateTime.Now.AddMinutes(-30),
                versaoAplicativoRecepcao: "1.0.0",
                protocoloEnvio: "PROT008"
            )
            .ComDadosProcessamento(
                dhProcessamento: DateTime.Now.AddMinutes(-15),
                versaoAplicativoProcessamento: "2.0.0"
            )
            .ComSucesso("Processado assincronamente")
            .ComId("LOTE008")
            .Build();

        Console.WriteLine($"- Estrutura: retornoLoteEventosAssincrono (assíncrono)");
        Console.WriteLine($"- CNPJ: {retornoAssincrono.GetCnpjDeclarante()}");
        Console.WriteLine($"- Protocolo: {retornoAssincrono.GetProtocoloEnvio()}");
        Console.WriteLine($"- Status: {retornoAssincrono.EFinanceira.retornoLoteEventosAssincrono.status.cdResposta}");
        Console.WriteLine($"- Namespace: retornoLoteEventosAssincrono/v1_0_0");

        Console.WriteLine("\nCaracterísticas específicas:");
        Console.WriteLine("- Processamento assíncrono de lotes");
        Console.WriteLine("- Dados de recepção e processamento separados");
        Console.WriteLine("- Status com cdResposta (int) vs cdStatus (string)");
        Console.WriteLine("- Suporte a eventos com XmlElement genérico");
        Console.WriteLine("- Protocolo de envio para rastreamento");
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
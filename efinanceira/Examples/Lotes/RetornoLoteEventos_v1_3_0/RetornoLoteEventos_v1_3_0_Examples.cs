using System.Xml;
using EFinanceira.Core.Factory;
using EFinanceira.Messages.Extensions;
using EFinanceira.Messages.Generated.Lotes.RetornoLoteEventos_v1_3_0;

namespace EFinanceira.Examples.Lotes.RetornoLoteEventos_v1_3_0;

/// <summary>
/// Exemplos práticos de uso do RetornoLoteEventos_v1_3_0_Builder
/// </summary>
public static class RetornoLoteEventos_v1_3_0_Examples
{
    /// <summary>
    /// Exemplo 1: Criando um retorno de sucesso simples
    /// </summary>
    public static void ExemploRetornoSucesso()
    {
        var factory = new EFinanceiraMessageFactory();

        var retorno = factory
            .CriarRetornoLoteEventos_v1_3_0()
            .ComEmpresaDeclarante("12345678000195")
            .ComDadosRecepcao(
                dhRecepcao: DateTime.Now.AddMinutes(-5),
                dhProcessamento: DateTime.Now,
                tipoEvento: "F200",
                idEvento: "EVT001",
                hash: "ABC123DEF456",
                nrRecibo: "REC001"
            )
            .ComSucesso("Evento processado com sucesso")
            .ComId("EVT001")
            .Build();

        Console.WriteLine($"Status: {retorno.EFinanceira.retornoEvento.status.cdRetorno}");
        Console.WriteLine($"Descrição: {retorno.EFinanceira.retornoEvento.status.descRetorno}");
        Console.WriteLine($"Sucesso: {retorno.IsSuccessful()}");
        Console.WriteLine($"CNPJ: {retorno.GetCnpjEmpresaDeclarante()}");
    }

    /// <summary>
    /// Exemplo 2: Criando um retorno com erros
    /// </summary>
    public static void ExemploRetornoComErros()
    {
        var factory = new EFinanceiraMessageFactory();

        var retorno = factory
            .CriarRetornoLoteEventos_v1_3_0()
            .ComEmpresaDeclarante("98765432000198")
            .ComDadosRecepcao(
                dhRecepcao: DateTime.Now.AddMinutes(-10),
                dhProcessamento: DateTime.Now,
                tipoEvento: "F200",
                idEvento: "EVT002",
                hash: "XYZ789ABC123",
                nrRecibo: "REC002"
            )
            .ComErroProcessamento("001", "Erro na validação dos dados")
            .ComErro("E001", "CNPJ inválido", "identificacaoEmpresaDeclarante/cnpjEmpresaDeclarante")
            .ComErro("E002", "Data inválida", "dadosRecepcaoEvento/dhRecepcao")
            .ComAviso("W001", "Campo opcional não informado", "observacoes")
            .ComId("EVT002")
            .Build();

        Console.WriteLine($"Status: {retorno.EFinanceira.retornoEvento.status.cdRetorno}");
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
    /// Exemplo 3: Criando retorno com recibo de entrega
    /// </summary>
    public static void ExemploRetornoComRecibo()
    {
        var factory = new EFinanceiraMessageFactory();

        var retorno = factory
            .CriarRetornoLoteEventos_v1_3_0()
            .ComEmpresaDeclarante("11111111000111")
            .ComDadosRecepcao(
                dhRecepcao: DateTime.Now.AddHours(-1),
                dhProcessamento: DateTime.Now.AddMinutes(-30),
                tipoEvento: "F500",
                idEvento: "EVT003",
                hash: "HASH123456789",
                nrRecibo: "REC003"
            )
            .ComReciboEntrega("REC003", DateTime.Now.AddMinutes(-15))
            .ComSucesso("Evento entregue com sucesso")
            .ComId("EVT003")
            .Build();

        Console.WriteLine($"Status: {retorno.EFinanceira.retornoEvento.status.cdRetorno}");
        Console.WriteLine($"Sucesso: {retorno.IsSuccessful()}");

        var dadosRecepcao = retorno.GetDadosRecepcao();
        if (dadosRecepcao != null)
        {
            Console.WriteLine($"Tipo Evento: {dadosRecepcao.tipoEvento}");
            Console.WriteLine($"Hash: {dadosRecepcao.hash}");
            Console.WriteLine($"Recibo: {dadosRecepcao.nrRecibo}");
        }

        var reciboEntrega = retorno.GetReciboEntrega();
        if (reciboEntrega != null)
        {
            Console.WriteLine($"Recibo Entrega: {reciboEntrega.numeroRecibo}");
            // Note: dhEntrega property doesn't exist in v1.3.0 schema
        }
    }

    /// <summary>
    /// Exemplo 4: Parsing de XML de retorno existente
    /// </summary>
    public static void ExemploParsingXml()
    {
        var factory = new EFinanceiraMessageFactory();

        var xmlRetorno = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<eFinanceira xmlns=""http://www.eFinanceira.gov.br/schemas/retornoEvento/v1_3_0"">
    <retornoEvento id=""EVT004"">
        <identificacaoEmpresaDeclarante>
            <cnpjEmpresaDeclarante>12345678000195</cnpjEmpresaDeclarante>
        </identificacaoEmpresaDeclarante>
        <dadosRecepcaoEvento>
            <dhRecepcao>2024-01-15T10:00:00</dhRecepcao>
            <dhProcessamento>2024-01-15T10:05:00</dhProcessamento>
            <tipoEvento>F200</tipoEvento>
            <idEvento>EVT004</idEvento>
            <hash>ABCDEF123456</hash>
            <nrRecibo>REC004</nrRecibo>
        </dadosRecepcaoEvento>
        <status>
            <cdRetorno>0</cdRetorno>
            <descRetorno>Processado com sucesso</descRetorno>
        </status>
    </retornoEvento>
</eFinanceira>";

        try
        {
            var retornoBuilder = factory.ParseRetornoLoteEventos_v1_3_0(xmlRetorno);
            var retorno = retornoBuilder.Build();

            Console.WriteLine($"XML parseado com sucesso!");
            Console.WriteLine($"CNPJ: {retorno.GetCnpjEmpresaDeclarante()}");
            Console.WriteLine($"Status: {retorno.EFinanceira.retornoEvento.status.cdRetorno}");
            Console.WriteLine($"Sucesso: {retorno.IsSuccessful()}");

            var dadosRecepcao = retorno.GetDadosRecepcao();
            if (dadosRecepcao != null)
            {
                Console.WriteLine($"Tipo Evento: {dadosRecepcao.tipoEvento}");
                Console.WriteLine($"ID Evento: {dadosRecepcao.idEvento}");
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

        var ocorrencias = new List<TRegistroOcorrenciasOcorrencias>
        {
            new() { tipo = "1", codigo = "E001", descricao = "CNPJ inválido", localizacaoErroAviso = "identificacaoEmpresaDeclarante" },
            new() { tipo = "1", codigo = "E002", descricao = "Data futura", localizacaoErroAviso = "dadosRecepcaoEvento/dhRecepcao" },
            new() { tipo = "2", codigo = "W001", descricao = "Campo recomendado", localizacaoErroAviso = "observacoes" }
        };

        var retorno = factory
            .CriarRetornoLoteEventos_v1_3_0()
            .ComEmpresaDeclarante("22222222000222")
            .ComDadosRecepcao(
                dhRecepcao: DateTime.Now.AddMinutes(-20),
                dhProcessamento: DateTime.Now.AddMinutes(-10),
                tipoEvento: "F300",
                idEvento: "EVT005",
                hash: "HASH987654321",
                nrRecibo: "REC005"
            )
            .ComStatus("1", "Processado com erros")
            .AdicionarOcorrencias(ocorrencias)
            .ComId("EVT005")
            .Build();

        Console.WriteLine($"Status: {retorno.EFinanceira.retornoEvento.status.cdRetorno}");
        Console.WriteLine($"Total de ocorrências: {retorno.EFinanceira.retornoEvento.status.dadosRegistroOcorrenciaEvento?.Length ?? 0}");
        Console.WriteLine($"Erros: {retorno.GetErros().Count()}");
        Console.WriteLine($"Avisos: {retorno.GetAvisos().Count()}");
    }

    /// <summary>
    /// Exemplo 6: Construção fluente com limpeza e reconstrução
    /// </summary>
    public static void ExemploConstrucaoFluente()
    {
        var factory = new EFinanceiraMessageFactory();

        var builder = factory.CriarRetornoLoteEventos_v1_3_0();

        // Primeira versão com erros
        builder
            .ComEmpresaDeclarante("33333333000333")
            .ComDadosRecepcao(
                dhRecepcao: DateTime.Now.AddMinutes(-30),
                dhProcessamento: DateTime.Now.AddMinutes(-20),
                tipoEvento: "F400",
                idEvento: "EVT006",
                hash: "HASH111222333",
                nrRecibo: "REC006"
            )
            .ComErroProcessamento("999", "Erro inicial")
            .ComErro("E999", "Erro temporário");

        // Limpar e reconstruir
        var retorno = builder
            .LimparOcorrencias()
            .ComSucesso("Reprocessado com sucesso")
            .ComAviso("W100", "Reprocessamento realizado")
            .ComReciboEntrega("REC006", DateTime.Now.AddMinutes(-5))
            .ComId("EVT006")
            .Build();

        Console.WriteLine($"Status final: {retorno.EFinanceira.retornoEvento.status.cdRetorno}");
        Console.WriteLine($"Sucesso: {retorno.IsSuccessful()}");
        Console.WriteLine($"Avisos: {retorno.GetAvisos().Count()}");

        var reciboEntrega = retorno.GetReciboEntrega();
        if (reciboEntrega != null)
        {
            Console.WriteLine($"Recibo Entrega: {reciboEntrega.numeroRecibo}");
        }
    }

    /// <summary>
    /// Exemplo 7: Exemplo completo com todos os campos
    /// </summary>
    public static void ExemploCompleto()
    {
        var factory = new EFinanceiraMessageFactory();

        var retorno = factory
            .CriarRetornoLoteEventos_v1_3_0()
            .ComEmpresaDeclarante("44444444000444")
            .ComDadosRecepcao(
                dhRecepcao: new DateTime(2024, 1, 15, 10, 0, 0),
                dhProcessamento: new DateTime(2024, 1, 15, 10, 5, 0),
                tipoEvento: "F200",
                idEvento: "EVT007",
                hash: "SHA256HASH123456789ABCDEF",
                nrRecibo: "REC007"
            )
            .ComReciboEntrega("REC007", new DateTime(2024, 1, 15, 10, 10, 0))
            .ComSucesso("Evento processado e entregue com sucesso")
            .ComAviso("W001", "Processamento executado fora do horário comercial")
            .ComId("EVT007")
            .Build();

        Console.WriteLine("=== Exemplo Completo ===");
        Console.WriteLine($"CNPJ Empresa: {retorno.GetCnpjEmpresaDeclarante()}");
        Console.WriteLine($"Status: {retorno.EFinanceira.retornoEvento.status.cdRetorno}");
        Console.WriteLine($"Sucesso: {retorno.IsSuccessful()}");

        var dadosRecepcao = retorno.GetDadosRecepcao();
        if (dadosRecepcao != null)
        {
            Console.WriteLine($"Data Recepção: {dadosRecepcao.dhRecepcao}");
            Console.WriteLine($"Data Processamento: {dadosRecepcao.dhProcessamento}");
            Console.WriteLine($"Tipo Evento: {dadosRecepcao.tipoEvento}");
            Console.WriteLine($"Hash: {dadosRecepcao.hash}");
        }

        var reciboEntrega = retorno.GetReciboEntrega();
        if (reciboEntrega != null)
        {
            Console.WriteLine($"Recibo: {reciboEntrega.numeroRecibo}");
            // Note: dhEntrega property doesn't exist in v1.3.0 schema
        }

        Console.WriteLine($"Total Avisos: {retorno.GetAvisos().Count()}");
    }

    /// <summary>
    /// Exemplo 8: Comparação de versões v1.2.0 vs v1.3.0
    /// </summary>
    public static void ExemploComparacaoVersoes()
    {
        var factory = new EFinanceiraMessageFactory();

        Console.WriteLine("=== Comparação de Versões ===");
        Console.WriteLine("\nv1.3.0 - Nova estrutura:");

        var retorno_v1_3_0 = factory
            .CriarRetornoLoteEventos_v1_3_0()
            .ComEmpresaDeclarante("55555555000555")
            .ComDadosRecepcao(
                dhRecepcao: DateTime.Now.AddMinutes(-15),
                dhProcessamento: DateTime.Now.AddMinutes(-10),
                tipoEvento: "F200",
                idEvento: "EVT008",
                hash: "NEWHASH123",
                nrRecibo: "REC008"
            )
            .ComSucesso("Processado na v1.3.0")
            .ComId("EVT008")
            .Build();

        Console.WriteLine($"- Estrutura: retornoEvento (individual)");
        Console.WriteLine($"- CNPJ: {retorno_v1_3_0.GetCnpjEmpresaDeclarante()}");
        Console.WriteLine($"- Tipo Evento: {retorno_v1_3_0.GetDadosRecepcao()?.tipoEvento}");
        Console.WriteLine($"- Hash: {retorno_v1_3_0.GetDadosRecepcao()?.hash}");
        Console.WriteLine($"- Namespace: retornoEvento/v1_3_0");

        Console.WriteLine("\nDiferenças principais:");
        Console.WriteLine("- v1.3.0 foca em eventos individuais vs v1.2.0 em lotes");
        Console.WriteLine("- v1.3.0 tem dadosRecepcaoEvento e dadosReciboEntrega");
        Console.WriteLine("- v1.3.0 tem identificacaoEmpresaDeclarante específica");
        Console.WriteLine("- v1.3.0 usa cdRetorno vs v1.2.0 usa cdStatus");
    }
}
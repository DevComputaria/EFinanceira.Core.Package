using System;
using System.Text;
using EFinanceira.Messages.Builders.Lotes;
using EFinanceira.Messages.Builders.Lotes.EnvioLoteCriptografado;

namespace EFinanceira.Messages.Examples.Lotes;

/// <summary>
/// Exemplos de uso do builder EnvioLoteCriptografado
/// </summary>
public static class EnvioLoteCriptografadoExamples
{
    /// <summary>
    /// Exemplo básico de criação de lote criptografado com dados manuais
    /// </summary>
    /// <returns>Mensagem de lote criptografado configurada</returns>
    public static EnvioLoteCriptografadoMessage ExemploBasico()
    {
        return EnvioLoteCriptografadoBuilder
            .Create()
            .WithId("LOTE_CRIPTO_001")
            .ComDadosManuais(
                idCertificado: "CERT_RF_2024",
                chave: "UGxhY2Vob2xkZXJfY2hhdmVfY3JpcHRvZ3JhZmljYQ==", // Base64: Placeholder_chave_criptográfica
                lote: "UGxhY2Vob2xkZXJfbG90ZV9jcmlwdG9ncmFmaWNv") // Base64: Placeholder_lote_criptografico
            .Build();
    }

    /// <summary>
    /// Exemplo de criação de lote criptografado com bytes
    /// </summary>
    /// <returns>Mensagem de lote criptografado configurada</returns>
    public static EnvioLoteCriptografadoMessage ExemploComBytes()
    {
        var chaveBytes = Encoding.UTF8.GetBytes("MinhaChaveCriptográficaSecreta123");
        var loteBytes = Encoding.UTF8.GetBytes("ConteúdoDoLoteCriptografado");

        return EnvioLoteCriptografadoBuilder
            .Create()
            .WithId("LOTE_CRIPTO_002")
            .ComDadosBytes(
                idCertificado: "CERT_RF_2024",
                chaveBytes: chaveBytes,
                loteBytes: loteBytes)
            .Build();
    }

    /// <summary>
    /// Exemplo de criação com configuração detalhada do lote criptografado
    /// </summary>
    /// <returns>Mensagem de lote criptografado configurada</returns>
    public static EnvioLoteCriptografadoMessage ExemploConfiguracaoDetalhada()
    {
        return EnvioLoteCriptografadoBuilder
            .Create("v1_2_0")
            .WithId("LOTE_CRIPTO_003")
            .WithLoteCriptografado(lote => lote
                .WithIdCertificado("CERT_RF_PRODUCAO_2024")
                .WithChave("RXN0YSBlIHVtYSBjaGF2ZSBkZSBleGVtcGxvIGVtIEJhc2U2NA==")
                .WithLote("RXN0ZSBlIHVtIGNvbnRldWRvIGRlIGxvdGUgZGUgZXhlbXBsbyBlbSBCYXNlNjQ="))
            .Build();
    }

    /// <summary>
    /// Exemplo de criação com lote gerado automaticamente
    /// </summary>
    /// <returns>Mensagem de lote criptografado configurada</returns>
    public static EnvioLoteCriptografadoMessage ExemploLoteAutomatico()
    {
        // Simular uma mensagem de lote usando o builder
        var loteEventosSimulado = new EnvioLoteEventosV120Builder()
            .WithId("LOTE_SIMULADO")
            .Build();

        return EnvioLoteCriptografadoBuilder
            .Create()
            .WithId("LOTE_CRIPTO_AUTO_001")
            .ComLoteCompleto(
                idCertificado: "CERT_RF_2024_AUTO",
                loteMessage: loteEventosSimulado,
                chaveAES: null) // Chave será gerada automaticamente
            .Build();
    }

    /// <summary>
    /// Exemplo usando API fluente para configuração step-by-step
    /// </summary>
    /// <returns>Mensagem de lote criptografado configurada</returns>
    public static EnvioLoteCriptografadoMessage ExemploStepByStep()
    {
        var builder = EnvioLoteCriptografadoBuilder.Create();

        // Configurar ID
        builder.WithId($"LOTE_{DateTime.Now:yyyyMMddHHmmss}");

        // Configurar lote criptografado
        builder.WithLoteCriptografado(lote =>
        {
            lote.WithIdCertificado("CERT_STEP_BY_STEP");
            
            // Gerar chave AES e obter referência
            lote.WithChaveAESAleatoria(out var chaveAES);
            
            // Usar a chave para algum processamento adicional se necessário
            Console.WriteLine($"Chave AES gerada com {chaveAES.Length} bytes");
            
            // Adicionar conteúdo do lote
            var conteudoLote = "Conteúdo específico do lote para processamento";
            var loteBytes = Encoding.UTF8.GetBytes(conteudoLote);
            lote.WithLote(loteBytes);
        });

        return builder.Build();
    }

    /// <summary>
    /// Exemplo de validação de dados Base64
    /// </summary>
    /// <returns>Mensagem de lote criptografado se válida, null caso contrário</returns>
    public static EnvioLoteCriptografadoMessage? ExemploValidacao()
    {
        var chaveBase64 = "VGVzdGVEZUNoYXZlRW1CYXNlNjQ=";
        var loteBase64 = "VGVzdGVEZUxvdGVFbUJhc2U2NA==";

        // Validar se as strings estão em formato Base64 válido
        if (!LoteCriptografiaUtils.IsValidBase64(chaveBase64) ||
            !LoteCriptografiaUtils.IsValidBase64(loteBase64))
        {
            Console.WriteLine("Dados em formato Base64 inválido!");
            return null;
        }

        return EnvioLoteCriptografadoBuilder
            .Create()
            .WithId("LOTE_VALIDADO")
            .ComDadosManuais(
                idCertificado: "CERT_VALIDACAO",
                chave: chaveBase64,
                lote: loteBase64)
            .Build();
    }

    /// <summary>
    /// Exemplo de uso dos utilitários de criptografia
    /// </summary>
    /// <returns>Mensagem de lote criptografado configurada</returns>
    public static EnvioLoteCriptografadoMessage ExemploUtilitariosCriptografia()
    {
        // Gerar chave AES
        var chaveAES = LoteCriptografiaUtils.GerarChaveAES();
        
        // Simular mensagem de lote usando o builder
        var loteEventos = new EnvioLoteEventosV120Builder()
            .WithId("LOTE_UTILS")
            .Build();
        
        // Criptografar usando utilitários
        var (chaveBase64, loteBase64) = LoteCriptografiaUtils.CriptografarLote(loteEventos, chaveAES);

        return EnvioLoteCriptografadoBuilder
            .Create()
            .WithId("LOTE_UTILS_CRIPTO")
            .ComDadosManuais(
                idCertificado: "CERT_UTILS_2024",
                chave: chaveBase64,
                lote: loteBase64)
            .Build();
    }

    /// <summary>
    /// Exemplo de tratamento de erro com validação
    /// </summary>
    /// <returns>Mensagem de lote criptografado ou dispara exceção</returns>
    public static EnvioLoteCriptografadoMessage ExemploTratamentoErro()
    {
        try
        {
            // Tentar criar lote sem dados obrigatórios para demonstrar validação
            return EnvioLoteCriptografadoBuilder
                .Create()
                .WithId("") // ID vazio deveria gerar erro
                .WithLoteCriptografado(lote => lote
                    .WithIdCertificado("CERT_ERRO"))

                // Não configurar chave nem lote - deveria gerar erro na validação
                .Build();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Erro esperado na validação: {ex.Message}");
            
            // Criar lote válido após capturar o erro
            return EnvioLoteCriptografadoBuilder
                .Create()
                .WithId("LOTE_CORRIGIDO")
                .ComDadosManuais(
                    idCertificado: "CERT_CORRECAO",
                    chave: "Q2hhdmVDb3JyaWdpZGE=",
                    lote: "TG90ZUNvcnJpZ2lkbw==")
                .Build();
        }
    }

    /// <summary>
    /// Exemplo completo com todas as funcionalidades
    /// </summary>
    /// <returns>Mensagem de lote criptografado completa</returns>
    public static EnvioLoteCriptografadoMessage ExemploCompleto()
    {
        const string certificadoId = "CERT_PRODUCAO_RF_2024";
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var loteId = $"LOTE_COMPLETO_{timestamp}";

        // Simular uma mensagem de lote de eventos real usando o builder
        var loteEventos = new EnvioLoteEventosV120Builder()
            .WithId($"LOTE_EVENTOS_{timestamp}")
            .Build();

        return EnvioLoteCriptografadoBuilder
            .Create("v1_2_0") // Versão específica
            .WithId(loteId) // ID único
            .WithLoteCriptografado(loteCripto => loteCripto
                .WithIdCertificado(certificadoId)
                .WithLoteCriptografadoCompleto(loteEventos)) // Criptografia automática
            .Build();
    }
}
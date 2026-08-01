using System;
using EFinanceira.Messages.Builders.Eventos.EvtRERCT;

namespace EFinanceira.Messages.Examples;

/// <summary>
/// Exemplos de uso do EvtRERCT Builder
/// </summary>
public static class EvtRERCTExample
{
    /// <summary>
    /// Exemplo completo de criação de evento RERCT
    /// </summary>
    /// <returns>Mensagem EvtRERCT configurada</returns>
    public static EvtRERCTMessage CreateCompleteExample()
    {
        var message = EvtRERCTBuilder
            .Create("v1_2_0")
            .WithId("ID_RERCT_2025_001")
            .WithIdeEvento(ideEvento => ideEvento
                .WithIdeEventoRERCT(123456789)
                .WithIndRetificacao(0) // 0-Original
                .WithAmbiente(2) // 2-Homologação
                .WithAplicativoEmi(1) // 1-Aplicativo do contribuinte
                .WithVersaoAplic("1.0.0"))
            .WithIdeDeclarante(ideDeclarante => ideDeclarante
                .WithCnpjDeclarante("12345678000123"))
            .WithIdeDeclarado(ideDeclarado => ideDeclarado
                .WithCpfCnpjDeclarado(cpfCnpj => cpfCnpj
                    .WithTipoInscricao(1) // 1-CPF
                    .WithNumeroInscricao("12345678901")))
            .AddRERCT(rerct => rerct
                .WithNomeBancoOrigem("Bank of International")
                .WithPaisOrigem("US")
                .WithBICBancoOrigem("BOFAUS3N")
                .AddInfoContaExterior(conta => conta
                    .WithTipoContaExterior(1) // Tipo conforme tabela específica
                    .WithNumeroContaExterior("US123456789")
                    .WithValorUltimoDia("150000.00")
                    .WithMoeda("USD")
                    .AddTitular(titular => titular
                        .WithNomeTitular("João Silva")
                        .WithCpfCnpjTitular(cpfCnpj => cpfCnpj
                            .WithTipoInscricao(1) // 1-CPF
                            .WithNumeroInscricao("12345678901"))
                        .WithNIFTitular("123456789US"))
                    .AddBeneficiarioFinal(beneficiario => beneficiario
                        .WithNomeBeneficiarioFinal("Maria Silva")
                        .WithCpfBeneficiarioFinal("98765432100")
                        .WithNIFBeneficiarioFinal("987654321US"))))
            .Build();

        return message;
    }

    /// <summary>
    /// Exemplo mínimo válido
    /// </summary>
    /// <returns>Mensagem EvtRERCT com campos mínimos</returns>
    public static EvtRERCTMessage CreateMinimalExample()
    {
        var message = EvtRERCTBuilder
            .Create()
            .WithIdeEvento(ideEvento => ideEvento
                .WithIdeEventoRERCT(111222333))
            .WithIdeDeclarante(ideDeclarante => ideDeclarante
                .WithCnpjDeclarante("00000000000100"))
            .WithIdeDeclarado(ideDeclarado => ideDeclarado
                .WithCpfCnpjDeclarado(cpfCnpj => cpfCnpj
                    .WithTipoInscricao(1)
                    .WithNumeroInscricao("00000000001")))
            .AddRERCT(rerct => rerct
                .WithNomeBancoOrigem("Minimal Bank")
                .WithPaisOrigem("XX")
                .AddInfoContaExterior(conta => conta
                    .WithNumeroContaExterior("MIN123")
                    .WithValorUltimoDia("1000.00")
                    .WithMoeda("USD")
                    .AddTitular(titular => titular
                        .WithNomeTitular("Titular Mínimo")
                        .WithCpfCnpjTitular(cpfCnpj => cpfCnpj
                            .WithTipoInscricao(1)
                            .WithNumeroInscricao("00000000001")))))
            .Build();

        return message;
    }

    /// <summary>
    /// Exemplo de retificação
    /// </summary>
    /// <returns>Mensagem EvtRERCT de retificação</returns>
    public static EvtRERCTMessage CreateRetificationExample()
    {
        var message = EvtRERCTBuilder
            .Create()
            .WithIdeEvento(ideEvento => ideEvento
                .WithIdeEventoRERCT(123456789)
                .WithIndRetificacao(1) // 1-Retificação
                .WithNrRecibo("REC123456789") // Obrigatório para retificação
                .WithAmbiente(1)) // 1-Produção
            .WithIdeDeclarante(ideDeclarante => ideDeclarante
                .WithCnpjDeclarante("12345678000123"))
            .WithIdeDeclarado(ideDeclarado => ideDeclarado
                .WithCpfCnpjDeclarado(cpfCnpj => cpfCnpj
                    .WithTipoInscricao(1) // 1-CPF
                    .WithNumeroInscricao("12345678901")))
            .AddRERCT(rerct => rerct
                .WithNomeBancoOrigem("Bank of International UPDATED")
                .WithPaisOrigem("US")
                .WithBICBancoOrigem("BOFAUS3N")
                .AddInfoContaExterior(conta => conta
                    .WithTipoContaExterior(1)
                    .WithNumeroContaExterior("US123456789")
                    .WithValorUltimoDia("175000.00") // Valor corrigido
                    .WithMoeda("USD")
                    .AddTitular(titular => titular
                        .WithNomeTitular("João Silva")
                        .WithCpfCnpjTitular(cpfCnpj => cpfCnpj
                            .WithTipoInscricao(1)
                            .WithNumeroInscricao("12345678901")))))
            .Build();

        return message;
    }
}
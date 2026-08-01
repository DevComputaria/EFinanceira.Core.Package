using EFinanceira.Core.Factory;
using EFinanceira.Messages.Builders.Eventos.EvtPrevidenciaPrivada;

namespace EFinanceira.Console.Sample.Eventos;

/// <summary>
/// Demonstração do uso do evento de movimentação de previdência privada e-Financeira
/// Especializado para operações previdenciárias (PGBL, VGBL, planos fechados)
/// </summary>
public static class EvtPrevidenciaPrivadaSample
{
    /// <summary>
    /// Demonstra a criação de um evento de movimentação de previdência privada
    /// </summary>
    /// <param name="factory">Factory configurado</param>
    public static void DemonstrarEvtPrevidenciaPrivada(EFinanceiraMessageFactory factory)
    {
        System.Console.WriteLine("=== EVENTO DE MOVIMENTAÇÃO DE PREVIDÊNCIA PRIVADA ===");
        System.Console.WriteLine();

        // Cenário 1: PGBL com aplicação mensal
        System.Console.WriteLine("1. PGBL com Aplicação Mensal:");
        
        var mensagemPGBL = factory.CreateMessage(
            MessageKind.Evento("EvtPrevidenciaPrivada"), 
            "v1_2_5", 
            (Action<EvtPrevidenciaPrivadaBuilder>)(builder => builder
                .ComId("ID1234567890123456789012345678901234567890")
                .ComIdeEvento(ideEvento => ideEvento
                    .ComIndRetificacao(1) // Evento Original
                    .ComTpAmb(2) // Homologação
                    .ComAplicEmi(1) // Aplicativo do declarante
                    .ComVerAplic("1.0.0"))
                .ComIdeDeclarante(ideDeclarante => ideDeclarante
                    .ComCnpjDeclarante("12345678000195"))
                .ComIdeDeclarado(ideDeclarado => ideDeclarado
                    .ComTpNI(1) // CPF
                    .ComNIDeclarado("12345678901")
                    .ComNomeDeclarado("João Silva Santos"))
                .ComMesCaixa(mesCaixa => mesCaixa
                    .ComAnoMesCaixa("2024-12")
                    .ComInfoPrevPriv(infoPrevPriv => infoPrevPriv
                        .ComNumProposta("PGBL202412001")
                        .ComNumProcesso("15414.901234/2020-12")
                        .ComProduto(produto => produto
                            .ComTpProduto(1) // PGBL
                            .ComOpcaoTributacao(2)) // Regressiva
                        .ComPlano(plano => plano
                            .ComTpPlano(1) // Aberto
                            .ComPlanoFechado(0)) // Não é plano fechado
                        .ComOpPrevPriv(opPrevPriv => opPrevPriv
                            .ComSaldoInicial(saldoInicial => saldoInicial
                                .ComVlrPrincipal("15000.50")
                                .ComVlrRendimentos("2500.25"))
                            .ComAplic(aplic => aplic
                                .ComVlrContribuicao("1200.00")
                                .ComVlrCarregamento("60.00")
                                .ComVlrPartPF("1200.00")
                                .ComVlrPartPJ("0.00"))
                            .ComSaldoFinal(saldoFinal => saldoFinal
                                .ComVlrPrincipal("16200.50")
                                .ComVlrRendimentos("2650.75")))))));

        System.Console.WriteLine($"Evento criado: {mensagemPGBL.RootElementName}");
        System.Console.WriteLine($"ID: {mensagemPGBL.IdValue}");
        System.Console.WriteLine($"Versão: {mensagemPGBL.Version}");
        System.Console.WriteLine();

        // Cenário 2: VGBL com tributação progressiva
        System.Console.WriteLine("2. VGBL com Tributação Progressiva:");
        
        var mensagemVGBL = factory.CreateMessage(
            MessageKind.Evento("EvtPrevidenciaPrivada"), 
            "v1_2_5", 
            (Action<EvtPrevidenciaPrivadaBuilder>)(builder => builder
                .ComId("ID9876543210987654321098765432109876543210")
                .ComIdeEvento(ideEvento => ideEvento
                    .ComIndRetificacao(1) // Evento Original
                    .ComTpAmb(2) // Homologação
                    .ComAplicEmi(1) // Aplicativo do declarante
                    .ComVerAplic("1.0.0"))
                .ComIdeDeclarante(ideDeclarante => ideDeclarante
                    .ComCnpjDeclarante("12345678000195"))
                .ComIdeDeclarado(ideDeclarado => ideDeclarado
                    .ComTpNI(1) // CPF
                    .ComNIDeclarado("98765432109")
                    .ComNomeDeclarado("Maria Oliveira Lima"))
                .ComMesCaixa(mesCaixa => mesCaixa
                    .ComAnoMesCaixa("2024-12")
                    .ComInfoPrevPriv(infoPrevPriv => infoPrevPriv
                        .ComNumProposta("VGBL202412002")
                        .ComNumProcesso("15414.901234/2021-08")
                        .ComProduto(produto => produto
                            .ComTpProduto(2) // VGBL
                            .ComOpcaoTributacao(1)) // Progressiva
                        .ComPlano(plano => plano
                            .ComTpPlano(1) // Aberto
                            .ComPlanoFechado(0)) // Não é plano fechado
                        .ComOpPrevPriv(opPrevPriv => opPrevPriv
                            .ComSaldoInicial(saldoInicial => saldoInicial
                                .ComVlrPrincipal("8500.00")
                                .ComVlrRendimentos("1200.30"))
                            .ComAplic(aplic => aplic
                                .ComVlrContribuicao("800.00")
                                .ComVlrCarregamento("40.00")
                                .ComVlrPartPF("800.00")
                                .ComVlrPartPJ("0.00"))
                            .ComSaldoFinal(saldoFinal => saldoFinal
                                .ComVlrPrincipal("9300.00")
                                .ComVlrRendimentos("1285.45")))))));

        System.Console.WriteLine($"Evento criado: {mensagemVGBL.RootElementName}");
        System.Console.WriteLine($"ID: {mensagemVGBL.IdValue}");
        System.Console.WriteLine($"Versão: {mensagemVGBL.Version}");
        System.Console.WriteLine();

        // Cenário 3: Plano Fechado de Previdência Complementar
        System.Console.WriteLine("3. Plano Fechado de Previdência Complementar:");
        
        var mensagemPlanoFechado = factory.CreateMessage(
            MessageKind.Evento("EvtPrevidenciaPrivada"), 
            "v1_2_5", 
            (Action<EvtPrevidenciaPrivadaBuilder>)(builder => builder
                .ComId("ID5555555555555555555555555555555555555555")
                .ComIdeEvento(ideEvento => ideEvento
                    .ComIndRetificacao(1) // Evento Original
                    .ComTpAmb(2) // Homologação
                    .ComAplicEmi(1) // Aplicativo do declarante
                    .ComVerAplic("1.0.0"))
                .ComIdeDeclarante(ideDeclarante => ideDeclarante
                    .ComCnpjDeclarante("12345678000195"))
                .ComIdeDeclarado(ideDeclarado => ideDeclarado
                    .ComTpNI(1) // CPF
                    .ComNIDeclarado("11122233309")
                    .ComNomeDeclarado("Carlos Eduardo Pereira"))
                .ComMesCaixa(mesCaixa => mesCaixa
                    .ComAnoMesCaixa("2024-12")
                    .ComInfoPrevPriv(infoPrevPriv => infoPrevPriv
                        .ComNumProposta("PREV202412003")
                        .ComNumProcesso("15414.901234/2019-05")
                        .ComProduto(produto => produto
                            .ComTpProduto(3)) // Outros produtos
                        .ComPlano(plano => plano
                            .ComTpPlano(2) // Fechado
                            .ComPlanoFechado(1) // É plano fechado
                            .ComCnpjPlano("33444555000177")
                            .ComTpPlanoFechado(2)) // Contribuição Definida
                        .ComOpPrevPriv(opPrevPriv => opPrevPriv
                            .ComSaldoInicial(saldoInicial => saldoInicial
                                .ComVlrPrincipal("45000.00")
                                .ComVlrRendimentos("12500.80"))
                            .ComAplic(aplic => aplic
                                .ComVlrContribuicao("2500.00")
                                .ComVlrCarregamento("0.00") // Sem carregamento
                                .ComVlrPartPF("1250.00")
                                .ComVlrPartPJ("1250.00")
                                .ComCnpj("99888777000166")) // CNPJ patrocinador
                            .ComSaldoFinal(saldoFinal => saldoFinal
                                .ComVlrPrincipal("47500.00")
                                .ComVlrRendimentos("13150.90")))))));

        System.Console.WriteLine($"Evento criado: {mensagemPlanoFechado.RootElementName}");
        System.Console.WriteLine($"ID: {mensagemPlanoFechado.IdValue}");
        System.Console.WriteLine($"Versão: {mensagemPlanoFechado.Version}");
        System.Console.WriteLine();

        // Cenário 4: Validação de estrutura XML (simulação)
        System.Console.WriteLine("4. Estrutura XML Gerada (simulação):");
        System.Console.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        System.Console.WriteLine("<eFinanceira xmlns=\"http://www.eFinanceira.gov.br/schemas/evtMovPP/v1_2_5\">");
        System.Console.WriteLine("  <evtMovPP id=\"ID1234567890123456789012345678901234567890\">");
        System.Console.WriteLine("    <ideEvento>");
        System.Console.WriteLine("      <indRetificacao>1</indRetificacao>");
        System.Console.WriteLine("      <tpAmb>2</tpAmb>");
        System.Console.WriteLine("      <aplicEmi>1</aplicEmi>");
        System.Console.WriteLine("      <verAplic>1.0.0</verAplic>");
        System.Console.WriteLine("    </ideEvento>");
        System.Console.WriteLine("    <ideDeclarante>");
        System.Console.WriteLine("      <cnpjDeclarante>12345678000195</cnpjDeclarante>");
        System.Console.WriteLine("    </ideDeclarante>");
        System.Console.WriteLine("    <ideDeclarado>");
        System.Console.WriteLine("      <tpNI>1</tpNI>");
        System.Console.WriteLine("      <NIDeclarado>12345678901</NIDeclarado>");
        System.Console.WriteLine("      <NomeDeclarado>João Silva Santos</NomeDeclarado>");
        System.Console.WriteLine("    </ideDeclarado>");
        System.Console.WriteLine("    <mesCaixa>");
        System.Console.WriteLine("      <anoMesCaixa>2024-12</anoMesCaixa>");
        System.Console.WriteLine("      <infoPrevPriv>");
        System.Console.WriteLine("        <numProposta>PGBL202412001</numProposta>");
        System.Console.WriteLine("        <numProcesso>15414.901234/2020-12</numProcesso>");
        System.Console.WriteLine("        <Produto>");
        System.Console.WriteLine("          <tpProduto>1</tpProduto>");
        System.Console.WriteLine("          <opcaoTributacao>2</opcaoTributacao>");
        System.Console.WriteLine("        </Produto>");
        System.Console.WriteLine("        <Plano>");
        System.Console.WriteLine("          <tpPlano>1</tpPlano>");
        System.Console.WriteLine("          <planoFechado>0</planoFechado>");
        System.Console.WriteLine("        </Plano>");
        System.Console.WriteLine("        <opPrevPriv>");
        System.Console.WriteLine("          <saldoInicial>");
        System.Console.WriteLine("            <vlrPrincipal>15000.50</vlrPrincipal>");
        System.Console.WriteLine("            <vlrRendimentos>2500.25</vlrRendimentos>");
        System.Console.WriteLine("          </saldoInicial>");
        System.Console.WriteLine("          <aplic>");
        System.Console.WriteLine("            <vlrContribuicao>1200.00</vlrContribuicao>");
        System.Console.WriteLine("            <vlrCarregamento>60.00</vlrCarregamento>");
        System.Console.WriteLine("            <vlrPartPF>1200.00</vlrPartPF>");
        System.Console.WriteLine("            <vlrPartPJ>0.00</vlrPartPJ>");
        System.Console.WriteLine("          </aplic>");
        System.Console.WriteLine("          <saldoFinal>");
        System.Console.WriteLine("            <vlrPrincipal>16200.50</vlrPrincipal>");
        System.Console.WriteLine("            <vlrRendimentos>2650.75</vlrRendimentos>");
        System.Console.WriteLine("          </saldoFinal>");
        System.Console.WriteLine("        </opPrevPriv>");
        System.Console.WriteLine("      </infoPrevPriv>");
        System.Console.WriteLine("    </mesCaixa>");
        System.Console.WriteLine("  </evtMovPP>");
        System.Console.WriteLine("</eFinanceira>");
        System.Console.WriteLine();

        System.Console.WriteLine("=== CARACTERÍSTICAS DO EVENTO ===");
        System.Console.WriteLine("• PGBL/VGBL: Suporte para produtos de previdência aberta");
        System.Console.WriteLine("• Planos Fechados: Suporte para fundos de pensão empresarial");
        System.Console.WriteLine("• Tributação: Progressiva e Regressiva configuráveis");
        System.Console.WriteLine("• Aplicações: Contribuições com carregamento e participações PF/PJ");
        System.Console.WriteLine("• Saldos: Controle de principal e rendimentos inicial/final");
        System.Console.WriteLine("• Patrocínio: Suporte a CNPJ patrocinador em planos fechados");
        System.Console.WriteLine("• Processo SUSEP: Vinculação com processos regulatórios");
        System.Console.WriteLine();
    }
}

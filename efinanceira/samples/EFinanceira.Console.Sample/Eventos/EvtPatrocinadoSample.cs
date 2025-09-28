using EFinanceira.Core.Factory;
using EFinanceira.Messages.Builders.Eventos.EvtPatrocinado;

namespace EFinanceira.Console.Sample.Eventos;

/// <summary>
/// Demonstração do uso do evento de cadastro de patrocinado e-Financeira
/// Especializado para entidades patrocinadas em estruturas de compliance internacional (FATCA/CRS)
/// </summary>
public static class EvtPatrocinadoSample
{
    /// <summary>
    /// Demonstra a criação de um evento de cadastro de patrocinado
    /// </summary>
    /// <param name="factory">Factory configurado</param>
    public static void DemonstrarEvtPatrocinado(EFinanceiraMessageFactory factory)
    {
        System.Console.WriteLine("=== EVENTO DE CADASTRO DE PATROCINADO ===");
        System.Console.WriteLine();

        // Cenário 1: Entidade jurídica brasileira patrocinada com GIIN
        System.Console.WriteLine("1. Entidade Jurídica Brasileira Patrocinada:");
        
        var mensagemPatrocinadoBrasileiro = factory.CreateMessage(
            MessageKind.Evento("EvtPatrocinado"), 
            "v1_2_0", 
            (Action<EvtPatrocinadoBuilder>)(builder => builder
                .ComId("ID1234567890123456789012345678901234567890")
                .ComIdeEvento(ideEvento => ideEvento
                    .ComIndRetificacao(1) // Evento Original
                    .ComTpAmb(2) // Homologação
                    .ComAplicEmi(1) // Aplicativo do declarante
                    .ComVerAplic("1.0.0"))
                .ComIdeDeclarante(ideDeclarante => ideDeclarante
                    .ComCnpjDeclarante("12345678000195"))
                .ComInfoPatrocinado(infoPatrocinado => infoPatrocinado
                    .ComGIIN("BR1234.56789.SL.123") // GIIN brasileiro
                    .ComCNPJ("98765432000187")
                    .ComNomePatrocinado("Empresa Patrocinada LTDA")
                    .ComTpNome("2") // Razão Social
                    .ComEndereco(endereco => endereco
                        .ComEnderecoLivre("Av. Paulista, 1000 - Bela Vista")
                        .ComMunicipio("São Paulo")
                        .ComPais("BR"))
                    .ComTpEndereco("2") // Comercial
                    .ComPaisResidencia(paisResid => paisResid
                        .ComPais("BR")))));

        System.Console.WriteLine($"Evento criado: {mensagemPatrocinadoBrasileiro.RootElementName}");
        System.Console.WriteLine($"ID: {mensagemPatrocinadoBrasileiro.IdValue}");
        System.Console.WriteLine($"Versão: {mensagemPatrocinadoBrasileiro.Version}");
        System.Console.WriteLine();

        // Cenário 2: Entidade internacional com múltiplos NIFs
        System.Console.WriteLine("2. Entidade Internacional com Múltiplos NIFs:");
        
        var mensagemPatrocinadoInternacional = factory.CreateMessage(
            MessageKind.Evento("EvtPatrocinado"), 
            "v1_2_0", 
            (Action<EvtPatrocinadoBuilder>)(builder => builder
                .ComId("ID9876543210987654321098765432109876543210")
                .ComIdeEvento(ideEvento => ideEvento
                    .ComIndRetificacao(1) // Evento Original
                    .ComTpAmb(2) // Homologação
                    .ComAplicEmi(1) // Aplicativo do declarante
                    .ComVerAplic("1.0.0"))
                .ComIdeDeclarante(ideDeclarante => ideDeclarante
                    .ComCnpjDeclarante("12345678000195"))
                .ComInfoPatrocinado(infoPatrocinado => infoPatrocinado
                    .ComGIIN("US1234.56789.SL.456") // GIIN americano
                    .ComNIF(nif => nif
                        .ComPaisEmissorNIF("US")
                        .ComNumeroNIF("12-3456789")) // EIN americano
                    .ComNIF(nif => nif
                        .ComPaisEmissorNIF("GB")
                        .ComNumeroNIF("GB123456789")) // UTR britânico
                    .ComNomePatrocinado("Global Investment Fund LLC")
                    .ComTpNome("2") // Razão Social
                    .ComEndereco(endereco => endereco
                        .ComEnderecoLivre("1234 Wall Street, Suite 5678")
                        .ComMunicipio("New York")
                        .ComPais("US"))
                    .ComTpEndereco("2") // Comercial
                    .ComPaisResidencia(paisResid => paisResid
                        .ComPais("US"))
                    .ComPaisResidencia(paisResid => paisResid
                        .ComPais("GB")))));

        System.Console.WriteLine($"Evento criado: {mensagemPatrocinadoInternacional.RootElementName}");
        System.Console.WriteLine($"ID: {mensagemPatrocinadoInternacional.IdValue}");
        System.Console.WriteLine($"Versão: {mensagemPatrocinadoInternacional.Version}");
        System.Console.WriteLine();

        // Cenário 3: Retificação de cadastro de patrocinado
        System.Console.WriteLine("3. Retificação de Cadastro de Patrocinado:");
        
        var mensagemRetificacao = factory.CreateMessage(
            MessageKind.Evento("EvtPatrocinado"), 
            "v1_2_0", 
            (Action<EvtPatrocinadoBuilder>)(builder => builder
                .ComId("ID5555555555555555555555555555555555555555")
                .ComIdeEvento(ideEvento => ideEvento
                    .ComIndRetificacao(2) // Evento de Retificação
                    .ComNrRecibo("1234567890123456789012345") // Recibo do evento original
                    .ComTpAmb(2) // Homologação
                    .ComAplicEmi(1) // Aplicativo do declarante
                    .ComVerAplic("1.0.0"))
                .ComIdeDeclarante(ideDeclarante => ideDeclarante
                    .ComCnpjDeclarante("12345678000195"))
                .ComInfoPatrocinado(infoPatrocinado => infoPatrocinado
                    .ComGIIN("BR9999.88888.SL.777") // GIIN corrigido
                    .ComCNPJ("11111111000199") // CNPJ corrigido
                    .ComNomePatrocinado("Fundo de Investimento Corrigido LTDA")
                    .ComTpNome("2") // Razão Social
                    .ComEndereco(endereco => endereco
                        .ComEnderecoLivre("Rua Corrigida, 999 - Centro")
                        .ComMunicipio("Rio de Janeiro")
                        .ComPais("BR"))
                    .ComTpEndereco("1") // Residencial
                    .ComPaisResidencia(paisResid => paisResid
                        .ComPais("BR")))));

        System.Console.WriteLine($"Evento criado: {mensagemRetificacao.RootElementName}");
        System.Console.WriteLine($"ID: {mensagemRetificacao.IdValue}");
        System.Console.WriteLine($"Versão: {mensagemRetificacao.Version}");
        System.Console.WriteLine();

        // Cenário 4: Validação de estrutura XML (simulação)
        System.Console.WriteLine("4. Estrutura XML Gerada (simulação):");
        System.Console.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        System.Console.WriteLine("<eFinanceira xmlns=\"http://www.eFinanceira.gov.br/schemas/evtCadPatrocinado/v1_2_0\">");
        System.Console.WriteLine("  <evtCadPatrocinado id=\"ID1234567890123456789012345678901234567890\">");
        System.Console.WriteLine("    <ideEvento>");
        System.Console.WriteLine("      <indRetificacao>1</indRetificacao>");
        System.Console.WriteLine("      <tpAmb>2</tpAmb>");
        System.Console.WriteLine("      <aplicEmi>1</aplicEmi>");
        System.Console.WriteLine("      <verAplic>1.0.0</verAplic>");
        System.Console.WriteLine("    </ideEvento>");
        System.Console.WriteLine("    <ideDeclarante>");
        System.Console.WriteLine("      <cnpjDeclarante>12345678000195</cnpjDeclarante>");
        System.Console.WriteLine("    </ideDeclarante>");
        System.Console.WriteLine("    <infoPatrocinado>");
        System.Console.WriteLine("      <GIIN>BR1234.56789.SL.123</GIIN>");
        System.Console.WriteLine("      <CNPJ>98765432000187</CNPJ>");
        System.Console.WriteLine("      <nomePatrocinado>Empresa Patrocinada LTDA</nomePatrocinado>");
        System.Console.WriteLine("      <tpNome>2</tpNome>");
        System.Console.WriteLine("      <endereco>");
        System.Console.WriteLine("        <enderecoLivre>Av. Paulista, 1000 - Bela Vista</enderecoLivre>");
        System.Console.WriteLine("        <municipio>São Paulo</municipio>");
        System.Console.WriteLine("        <pais>BR</pais>");
        System.Console.WriteLine("      </endereco>");
        System.Console.WriteLine("      <tpEndereco>2</tpEndereco>");
        System.Console.WriteLine("      <paisResid>");
        System.Console.WriteLine("        <pais>BR</pais>");
        System.Console.WriteLine("      </paisResid>");
        System.Console.WriteLine("    </infoPatrocinado>");
        System.Console.WriteLine("  </evtCadPatrocinado>");
        System.Console.WriteLine("</eFinanceira>");
        System.Console.WriteLine();

        System.Console.WriteLine("=== CARACTERÍSTICAS DO EVENTO ===");
        System.Console.WriteLine("• GIIN: Identificação global de intermediário financeiro");
        System.Console.WriteLine("• Múltiplos NIFs: Suporte para diferentes jurisdições fiscais");
        System.Console.WriteLine("• Endereços internacionais: Flexibilidade para entidades globais");
        System.Console.WriteLine("• Países de residência: Múltiplas residências fiscais");
        System.Console.WriteLine("• Compliance FATCA/CRS: Atende regulamentações internacionais");
        System.Console.WriteLine();
    }
}

using System.Xml;
using EFinanceira.Console.Sample.Configuration;
using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Builders.Eventos;
using EFinanceira.Messages.Builders.Xmldsig;
using EFinanceira.Messages.Generated.Eventos.EvtAberturaeFinanceira;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EFinanceira.Console.Sample;

/// <summary>
/// Exemplo completo de criação de mensagem e-Financeira com assinatura digital
/// </summary>
public static class ExemploAssinatura
{
    /// <summary>
    /// Cria uma mensagem completa de abertura e-Financeira com assinatura digital
    /// </summary>
    /// <param name="serviceProvider">Service provider para obter dependências</param>
    /// <param name="logger">Logger para registrar operações</param>
    /// <returns>XML assinado digitalmente</returns>
    public static async Task<XmlDocument> CriarMensagemComAssinatura(
        IServiceProvider serviceProvider, 
        ILogger logger)
    {
        logger.LogInformation("=== Criando Mensagem e-Financeira com Assinatura Digital ===");

        // 1. Obter configurações
        var settings = serviceProvider.GetRequiredService<IOptions<EFinanceiraSettings>>().Value;
        var xmlSerializer = serviceProvider.GetRequiredService<IXmlSerializer>();

        logger.LogInformation("📋 Configurações carregadas:");
        logger.LogInformation("  - Declarante: {Cnpj} - {Nome}", settings.Declarante.Cnpj, settings.Declarante.Nome);
        logger.LogInformation("  - Certificado: {Path}", settings.Certificate.PfxPath);

        // 2. Criar evento de abertura e-Financeira
        logger.LogInformation("\n🏗️ Passo 1: Criando evento EvtAberturaeFinanceira...");
        
        var eventoId = $"EVT_ASSINADO_{DateTime.Now:yyyyMMddHHmmss}";
        var evento = new EFinanceira.Messages.Builders.Eventos.EvtAberturaeFinanceira.EvtAberturaeFinanceiraBuilder()
            .WithId(eventoId)
            .WithIdeDeclarante(decl => decl
                .WithCnpjDeclarante(settings.Declarante.Cnpj))
            .WithInfoAbertura(info => info
                .WithDataInicio(new DateTime(2024, 1, 1))
                .WithDataFim(new DateTime(2024, 12, 31)))
            .WithAberturaPP(pp => pp
                .AddTipoEmpresa(empresa => empresa
                    .WithTipoPrevidenciaPrivada("1")))
            .WithAberturaMovOpFin(mov => mov
                .WithResponsavelRMF(rmf => rmf
                    .WithCpf("12345678901")
                    .WithNome("João Silva Santos")
                    .WithSetor("Financeiro"))
                .WithResponsaveisFinanceiros(responsaveis => responsaveis
                    .AddResponsavelFinanceiro(resp => resp
                        .WithCpf("12345678901")
                        .WithNome("João Silva Santos")
                        .WithSetor("Financeiro")
                        .WithEmail("joao.silva@empresa.com.br"))))
            .Build();

        logger.LogInformation("✅ Evento criado com sucesso:");
        logger.LogInformation("  - ID: {Id}", evento.IdValue);
        logger.LogInformation("  - Tipo: {Tipo}", evento.GetType().Name);

        // 3. Serializar para XML
        logger.LogInformation("\n📄 Passo 2: Serializando evento para XML...");
        
        // Criar o elemento raiz eFinanceira que contém o evento
        var eFinanceiraRaiz = new eFinanceira
        {
            evtAberturaeFinanceira = evento.Evento
        };
        
        var xmlString = xmlSerializer.Serialize(eFinanceiraRaiz);
        var xmlDocument = new XmlDocument { PreserveWhitespace = true };
        xmlDocument.LoadXml(xmlString);

        logger.LogInformation("✅ XML serializado:");
        logger.LogInformation("  - Tamanho: {Size:N0} caracteres", xmlString.Length);
        logger.LogInformation("  - Elemento raiz: {Root}", xmlDocument.DocumentElement?.Name);

        // 4. Configurar assinatura digital
        logger.LogInformation("\n🔐 Passo 3: Configurando assinatura digital...");
        
        using var xmldsigBuilder = new XmldsigBuilder();
        
        try
        {
            // Tentar carregar certificado do arquivo configurado
            xmldsigBuilder.WithCertificateFromFile(settings.Certificate.PfxPath, settings.Certificate.PfxPassword);
            logger.LogInformation("✅ Certificado carregado do arquivo: {Path}", settings.Certificate.PfxPath);
        }
        catch (FileNotFoundException)
        {
            logger.LogWarning("⚠️ Arquivo de certificado não encontrado, tentando seleção interativa...");
            
            try
            {
                xmldsigBuilder.WithInteractiveCertificateSelection();
                logger.LogInformation("✅ Certificado selecionado interativamente");
            }
            catch (Exception ex)
            {
                logger.LogError("❌ Erro ao selecionar certificado: {Error}", ex.Message);
                throw new InvalidOperationException("Não foi possível obter certificado para assinatura", ex);
            }
        }

        // 5. Assinar digitalmente
        logger.LogInformation("\n✍️ Passo 4: Assinando XML digitalmente...");
        
        var xmlAssinado = xmldsigBuilder.SignXmlEvent(xmlDocument);
        
        logger.LogInformation("✅ XML assinado com sucesso:");
        logger.LogInformation("  - Algoritmo: RSA-SHA256 (com fallback SHA1)");
        logger.LogInformation("  - Tamanho final: {Size:N0} caracteres", xmlAssinado.OuterXml.Length);

        // 6. Validar assinatura
        logger.LogInformation("\n🔍 Passo 5: Validando assinatura...");
        
        var assinaturaValida = XmldsigBuilder.ValidateSignature(xmlAssinado);
        
        if (assinaturaValida)
        {
            logger.LogInformation("✅ Assinatura digital válida!");
        }
        else
        {
            logger.LogWarning("⚠️ Assinatura digital não pôde ser validada completamente");
        }

        // 7. Salvar arquivo final
        logger.LogInformation("\n💾 Passo 6: Salvando arquivo assinado...");
        
        var nomeArquivo = $"evento_assinado_{DateTime.Now:yyyyMMdd_HHmmss}.xml";
        var caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), nomeArquivo);
        
        xmlAssinado.Save(caminhoArquivo);
        
        logger.LogInformation("✅ Arquivo salvo: {Path}", caminhoArquivo);
        logger.LogInformation("📊 Estatísticas finais:");
        logger.LogInformation("  - Tamanho original: {Original:N0} chars", xmlString.Length);
        logger.LogInformation("  - Tamanho assinado: {Signed:N0} chars", xmlAssinado.OuterXml.Length);
        logger.LogInformation("  - Aumento: {Increase:N0} chars ({Percent:F1}%)", 
            xmlAssinado.OuterXml.Length - xmlString.Length,
            ((double)(xmlAssinado.OuterXml.Length - xmlString.Length) / xmlString.Length) * 100);

        logger.LogInformation("\n🎉 Mensagem e-Financeira com assinatura digital criada com sucesso!");
        
        return xmlAssinado;
    }

    /// <summary>
    /// Demonstra o processo completo de verificação de uma mensagem assinada
    /// </summary>
    /// <param name="xmlAssinado">XML com assinatura digital</param>
    /// <param name="logger">Logger para registrar operações</param>
    public static void VerificarMensagemAssinada(XmlDocument xmlAssinado, ILogger logger)
    {
        logger.LogInformation("\n=== Verificando Mensagem Assinada ===");

        // 1. Verificar estrutura da assinatura
        var assinaturas = xmlAssinado.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#");
        logger.LogInformation("🔍 Assinaturas encontradas: {Count}", assinaturas.Count);

        if (assinaturas.Count > 0)
        {
            var assinatura = (XmlElement)assinaturas[0]!;
            logger.LogInformation("📋 Detalhes da assinatura:");
            
            // Método de assinatura
            var signatureMethod = assinatura.SelectSingleNode(".//ds:SignatureMethod/@Algorithm", CreateNamespaceManager(xmlAssinado));
            if (signatureMethod?.Value != null)
            {
                var algoritmo = signatureMethod.Value switch
                {
                    "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256" => "RSA-SHA256",
                    "http://www.w3.org/2000/09/xmldsig#rsa-sha1" => "RSA-SHA1",
                    _ => signatureMethod.Value
                };
                logger.LogInformation("  - Algoritmo: {Algorithm}", algoritmo);
            }

            // Método de digest
            var digestMethod = assinatura.SelectSingleNode(".//ds:DigestMethod/@Algorithm", CreateNamespaceManager(xmlAssinado));
            if (digestMethod?.Value != null)
            {
                var digest = digestMethod.Value switch
                {
                    "http://www.w3.org/2001/04/xmlenc#sha256" => "SHA256",
                    "http://www.w3.org/2000/09/xmldsig#sha1" => "SHA1",
                    _ => digestMethod.Value
                };
                logger.LogInformation("  - Digest: {Digest}", digest);
            }

            // Informações do certificado
            var x509Certificate = assinatura.SelectSingleNode(".//ds:X509Certificate", CreateNamespaceManager(xmlAssinado));
            if (x509Certificate != null)
            {
                logger.LogInformation("  - Certificado X.509: Presente");
                logger.LogInformation("  - Tamanho do certificado: {Size} caracteres", x509Certificate.InnerText?.Length ?? 0);
            }
        }

        // 2. Validar assinatura
        logger.LogInformation("\n🔐 Validando integridade...");
        
        try
        {
            var isValid = XmldsigBuilder.ValidateSignature(xmlAssinado);
            if (isValid)
            {
                logger.LogInformation("✅ Assinatura digital válida - Documento íntegro");
            }
            else
            {
                logger.LogWarning("⚠️ Assinatura digital inválida ou não verificável");
            }
        }
        catch (Exception ex)
        {
            logger.LogError("❌ Erro na validação: {Error}", ex.Message);
        }

        logger.LogInformation("✅ Verificação concluída");
    }

    private static XmlNamespaceManager CreateNamespaceManager(XmlDocument document)
    {
        var nsManager = new XmlNamespaceManager(document.NameTable);
        nsManager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
        return nsManager;
    }
}

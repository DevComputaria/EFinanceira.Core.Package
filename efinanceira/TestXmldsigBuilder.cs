using EFinanceira.Messages.Builders.Xmldsig;

Console.WriteLine("=== Demonstração XmldsigBuilder Consolidado ===\n");

// Exemplo 1: Teste de validação básica
var builderBasico = new XmldsigBuilder();
Console.WriteLine($"Builder vazio é válido: {builderBasico.IsValid()}");

builderBasico.WithId("SIGNATURE_001");
Console.WriteLine($"Builder com ID é válido: {builderBasico.IsValid()}");

// Exemplo 2: Demonstrar uso com certificado de arquivo (irá falhar por arquivo não existir)
try
{
    using var builderArquivo = new XmldsigBuilder();
    var assinatura = builderArquivo
        .WithId("SIGNATURE_ARQUIVO_001")
        .WithCertificateFromFile(@"C:\Certificados\MeuCertificado.pfx", "senha")
        .WithXmlContent("#EventoId", "<evento>Conteúdo XML</evento>")
        .Build();

    Console.WriteLine("✓ Assinatura criada com certificado de arquivo");
    Console.WriteLine($"  - ID: {assinatura.IdValue}");
}
catch (FileNotFoundException ex)
{
    Console.WriteLine("⚠ Arquivo de certificado não encontrado (esperado para demonstração)");
    Console.WriteLine($"  - Erro: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"⚠ Erro ao processar certificado: {ex.Message}");
}

// Exemplo 3: Demonstrar uso com certificado do store (irá falhar por thumbprint inexistente)
try
{
    using var builderStore = new XmldsigBuilder();
    var assinatura = builderStore
        .WithId("SIGNATURE_STORE_001")
        .WithCertificateFromStore("1234567890ABCDEF1234567890ABCDEF12345678")
        .WithXmlContent("#MovimentoId", "<movimento>Dados do movimento</movimento>")
        .Build();

    Console.WriteLine("✓ Assinatura criada com certificado do store");
    Console.WriteLine($"  - ID: {assinatura.IdValue}");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine("⚠ Certificado não encontrado no repositório (esperado para demonstração)");
    Console.WriteLine($"  - Erro: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"⚠ Erro ao acessar repositório: {ex.Message}");
}

Console.WriteLine("\n=== Características do XmldsigBuilder de Produção ===");
Console.WriteLine("✓ Builder unificado - não precisamos de classes separadas");
Console.WriteLine("✓ Carrega certificados de arquivos .pfx/.p12");
Console.WriteLine("✓ Acessa certificados do repositório do Windows");
Console.WriteLine("✓ Calcula digest real com canonicalização C14N");
Console.WriteLine("✓ Usa algoritmos seguros (RSA-SHA256)");
Console.WriteLine("✓ Valida presença de chave privada");
Console.WriteLine("✓ Implementa IDisposable para liberação de recursos");
Console.WriteLine("✓ Suporte completo ao padrão XMLDSig");

Console.WriteLine("\n=== Uso em Produção ===");
Console.WriteLine("1. Configure certificado válido com chave privada");
Console.WriteLine("2. Use WithCertificateFromFile() ou WithCertificateFromStore()");
Console.WriteLine("3. Chame WithXmlContent() com XML real a ser assinado");
Console.WriteLine("4. Execute Build() para obter assinatura válida");
Console.WriteLine("5. Use using para garantir liberação de recursos");

Console.WriteLine("\nPressione qualquer tecla para finalizar...");
Console.ReadKey();

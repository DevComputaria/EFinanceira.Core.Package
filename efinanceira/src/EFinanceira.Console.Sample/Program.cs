using EFinanceira.Core.Abstractions;
using EFinanceira.Core.Factory;
using EFinanceira.Core.Serialization;
using EFinanceira.Core.Signing;
using EFinanceira.Core.Validation;
using EFinanceira.Messages.Builders.Eventos;
using EFinanceira.Messages.Builders.Lotes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EFinanceira.Console.Sample;

/// <summary>
/// Aplicação de exemplo demonstrando o uso completo da biblioteca EFinanceira
/// Fluxo: Criar evento -> Criar lote -> Serializar -> Validar -> Assinar -> Salvar
/// </summary>
public class Program
{
    private static ILogger<Program>? _logger;
    private static IConfiguration? _configuration;
    private static IServiceProvider? _serviceProvider;

    public static async Task Main(string[] args)
    {
        try
        {
            // Configurar aplicação
            ConfigureServices();

            _logger = _serviceProvider!.GetRequiredService<ILogger<Program>>();
            _logger.LogInformation("=== EFinanceira Console Sample ===");
            _logger.LogInformation("Demonstrando uso completo da biblioteca");

            // Executar demonstrações
            await DemonstrarCriacaoEvento();
            await DemonstrarCriacaoLote();
            await DemonstrarCriacaoConsulta();
            await DemonstrarValidacao();
            await DemonstrarAssinatura();
            await DemonstrarFluxoCompleto();

            _logger.LogInformation("=== Demonstração concluída com sucesso! ===");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Erro durante a execução: {ex.Message}");
            System.Console.WriteLine($"Stack trace: {ex.StackTrace}");
            Environment.Exit(1);
        }

        System.Console.WriteLine("\nPressione qualquer tecla para sair...");
        System.Console.ReadKey();
    }

    private static void ConfigureServices()
    {
        // Configuração
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        // Serviços
        var services = new ServiceCollection();

        // Logging
        services.AddLogging(builder =>
            builder.AddConsole().AddConfiguration(_configuration.GetSection("Logging")));

        // Core services
        services.AddSingleton<IXmlSerializer, XmlNetSerializer>();
        services.AddSingleton<IXmlValidator, XmlValidator>();
        services.AddSingleton<IXmlSigner, XmlSigner>();
        services.AddSingleton<IMessageFactory, EFinanceiraMessageFactory>();

        // Configuração
        services.AddSingleton(_configuration);

        _serviceProvider = services.BuildServiceProvider();
    }

    private static async Task DemonstrarCriacaoEvento()
    {
        _logger!.LogInformation("\n--- 1. Demonstração: Criação de Evento ---");

        try
        {
            var cnpjDeclarante = _configuration!["EFinanceira:Declarante:Cnpj"]!;

            // Criar evento usando builder fluente
            var eventoBuilder = new LeiauteMovimentacaoFinanceiraBuilder()
                .WithId("EVT20241219001")
                .WithDeclarante(cnpjDeclarante)
                .WithAmbiente(2) // Homologação
                .WithRetificacao(1) // Original
                .WithVersaoAplicacao("1.0.0")
                .WithDeclarado(declarado =>
                {
                    declarado
                        .WithTipoNI(2) // CNPJ
                        .WithNumeroInscricao("98765432000188")
                        .WithNome("Cliente Exemplo S.A.")
                        .WithEndereco(endereco =>
                        {
                            endereco
                                .WithPais("076") // Brasil
                                .WithEndereco("Rua Exemplo, 123, Centro")
                                .WithCidade("São Paulo");
                        });
                })
                .WithMovimentacao(mov =>
                {
                    mov
                        .WithDataInicio(new DateTime(2024, 1, 1))
                        .WithDataFim(new DateTime(2024, 12, 31))
                        .AdicionarReportavel(rep =>
                        {
                            rep
                                .WithTipoReportavel(1)
                                .WithInformacoesConta(conta =>
                                {
                                    conta
                                        .WithTipoConta(1)
                                        .WithSubTipoConta("001")
                                        .WithMoeda("986"); // Real
                                });
                        });
                });

            var evento = eventoBuilder.Build();

            _logger.LogInformation("✓ Evento criado com sucesso");
            _logger.LogInformation("  - ID: {Id}", evento.IdValue);
            _logger.LogInformation("  - Tipo: {Tipo}", evento.RootElementName);
            _logger.LogInformation("  - Versão: {Versao}", evento.Version);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar evento");
            throw;
        }
    }

    private static async Task DemonstrarCriacaoLote()
    {
        _logger!.LogInformation("\n--- 2. Demonstração: Criação de Lote ---");

        try
        {
            var cnpjTransmissor = _configuration!["EFinanceira:Declarante:Cnpj"]!;
            var nomeTransmissor = _configuration["EFinanceira:Declarante:Nome"]!;

            // Criar múltiplos eventos
            var evento1 = new LeiauteMovimentacaoFinanceiraBuilder()
                .WithId("EVT20241219001")
                .WithDeclarante(cnpjTransmissor)
                .WithDeclarado(d => d.WithTipoNI(2).WithNumeroInscricao("11111111000111").WithNome("Cliente 1"))
                .WithMovimentacao(m => m.WithDataInicio(DateTime.Today.AddDays(-30)).WithDataFim(DateTime.Today))
                .Build();

            var evento2 = new LeiauteMovimentacaoFinanceiraBuilder()
                .WithId("EVT20241219002")
                .WithDeclarante(cnpjTransmissor)
                .WithDeclarado(d => d.WithTipoNI(2).WithNumeroInscricao("22222222000222").WithNome("Cliente 2"))
                .WithMovimentacao(m => m.WithDataInicio(DateTime.Today.AddDays(-30)).WithDataFim(DateTime.Today))
                .Build();

            // Criar lote usando builder fluente
            var loteBuilder = new EnvioLoteEventosV120Builder()
                .WithId("LOTE20241219001")
                .ComTransmissor(cnpjTransmissor, nomeTransmissor, "contato@empresa.com.br", "+5511999999999")
                .AdicionarEvento(evento1)
                .AdicionarEvento(evento2);

            var lote = loteBuilder.Build();

            _logger.LogInformation("✓ Lote criado com sucesso");
            _logger.LogInformation("  - ID: {Id}", lote.IdValue);
            _logger.LogInformation("  - Transmissor: {Transmissor}", lote.Lote.IdeTransmissor.CnpjTransmissor);
            _logger.LogInformation("  - Eventos: {Count}", lote.Lote.Eventos.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar lote");
            throw;
        }
    }

    private static async Task DemonstrarCriacaoConsulta()
    {
        _logger!.LogInformation("\n--- 3. Demonstração: Criação de Consulta ---");

        try
        {
            // Criar consulta diretamente usando o builder
            var consulta = new EFinanceira.Messages.Builders.Consultas.RetInfoCadastral.RetInfoCadastralBuilder("v1_2_0")
                .WithId("CONSULTA_001")
                .WithDataHoraProcessamento(DateTime.UtcNow)
                .WithNumeroRecibo("REC123456789")
                .WithStatus(status => status
                    .WithCodigo("200")
                    .WithDescricao("Consulta processada com sucesso"))
                .WithEmpresaDeclarante(empresa => empresa
                    .WithCnpj("12345678000199"))
                .WithInformacoesCadastrais(info => info
                    .WithCnpj("98765432000188")
                    .WithNome("Instituição Financeira XYZ Ltda")
                    .WithEndereco("Rua das Flores, 123, Centro")
                    .WithMunicipio(3550308) // São Paulo
                    .WithUf("SP")
                    .WithGiin("ABC123.45678.LE.987"))
                .Build();

            _logger.LogInformation("✓ Consulta RetInfoCadastral criada com sucesso");
            _logger.LogInformation("  - Tipo: {Type}", consulta.GetType().Name);
            _logger.LogInformation("  - ID: {Id}", consulta.IdValue);
            _logger.LogInformation("  - Versão: {Version}", consulta.Version);
            _logger.LogInformation("  - Elemento Raiz: {Root}", consulta.RootElementName);

            // Demonstrar serialização da consulta
            var serializer = _serviceProvider!.GetRequiredService<IXmlSerializer>();
            var xml = serializer.Serialize(consulta.Payload); // Serializar o POCO gerado

            _logger.LogInformation("✓ Consulta serializada para XML");
            _logger.LogInformation("  - Tamanho: {Size} caracteres", xml.Length);

            // Salvar exemplo da consulta
            var consultaFile = Path.Combine(Directory.GetCurrentDirectory(), "consulta_exemplo.xml");
            await File.WriteAllTextAsync(consultaFile, xml);
            _logger.LogInformation("  - Salva em: {File}", consultaFile);

            // Demonstrar criação de consulta RetInfoIntermediario
            _logger.LogInformation("\n--- Demonstrando RetInfoIntermediario ---");
            var consultaIntermediario = new EFinanceira.Messages.Builders.Consultas.RetInfoIntermediario.RetInfoIntermediarioBuilder("v1_2_0")
                .WithDataHoraProcessamento(DateTime.UtcNow)
                .WithStatus(status => status
                    .WithCodigo("200")
                    .WithDescricao("Consulta de intermediário processada com sucesso"))
                .WithEmpresaDeclarante(empresa => empresa
                    .WithCnpj("12345678000199"))
                .WithIdentificacoesIntermediarios(intermediarios => intermediarios
                    .AddIntermediario(i => i
                        .WithGiin("ABC123.45678.LE.987")
                        .WithTipoNI(1)
                        .WithNIIntermediario("12345678000188")
                        .WithNumeroRecibo("REC987654321")
                        .WithId("INT_001"))
                    .AddIntermediario(i => i
                        .WithGiin("DEF456.78901.LE.654")
                        .WithTipoNI(2)
                        .WithNIIntermediario("98765432000177")
                        .WithNumeroRecibo("REC123789456")
                        .WithId("INT_002")))
                .Build();

            _logger.LogInformation("✓ Consulta RetInfoIntermediario criada com sucesso");
            _logger.LogInformation("  - Tipo: {Type}", consultaIntermediario.GetType().Name);
            _logger.LogInformation("  - Versão: {Version}", consultaIntermediario.Version);

            // Serializar consulta de intermediário
            var xmlIntermediario = serializer.Serialize(consultaIntermediario.Payload);
            _logger.LogInformation("✓ Consulta de intermediário serializada para XML");
            _logger.LogInformation("  - Tamanho: {Size} caracteres", xmlIntermediario.Length);

            // Salvar exemplo da consulta de intermediário
            var intermediarioFile = Path.Combine(Directory.GetCurrentDirectory(), "consulta_intermediario_exemplo.xml");
            await File.WriteAllTextAsync(intermediarioFile, xmlIntermediario);
            _logger.LogInformation("  - Salva em: {File}", intermediarioFile);

            // Demonstrar criação de consulta RetInfoMovimento
            _logger.LogInformation("\n--- Demonstrando RetInfoMovimento ---");
            var consultaMovimento = new EFinanceira.Messages.Builders.Consultas.RetInfoMovimento.RetInfoMovimentoBuilder("v1_2_0")
                .WithDataHoraProcessamento(DateTime.UtcNow)
                .WithStatus(status => status
                    .WithCodigo("200")
                    .WithDescricao("Consulta de movimento processada com sucesso"))
                .WithEmpresaDeclarante(empresa => empresa
                    .WithCnpj("12345678000199"))
                .WithInformacoesMovimento(movimentos => movimentos
                    .AddMovimento(m => m
                        .WithTipoMovimento("1")
                        .WithTipoNI("1")
                        .WithNI("12345678000188")
                        .WithAnoMesCaixa("202412")
                        .WithAnoCaixa("2024")
                        .WithSemestre("2")
                        .WithSituacao("1")
                        .WithNumeroRecibo("REC111222333")
                        .WithId("MOV_001"))
                    .AddMovimento(m => m
                        .WithTipoMovimento("2")
                        .WithTipoNI("2")
                        .WithNI("98765432000177")
                        .WithAnoMesCaixa("202411")
                        .WithAnoCaixa("2024")
                        .WithSemestre("2")
                        .WithSituacao("2")
                        .WithNumeroRecibo("REC444555666")
                        .WithId("MOV_002")))
                .Build();

            _logger.LogInformation("✓ Consulta RetInfoMovimento criada com sucesso");
            _logger.LogInformation("  - Tipo: {Type}", consultaMovimento.GetType().Name);
            _logger.LogInformation("  - Versão: {Version}", consultaMovimento.Version);

            // Serializar consulta de movimento
            var xmlMovimento = serializer.Serialize(consultaMovimento.Payload);
            _logger.LogInformation("✓ Consulta de movimento serializada para XML");
            _logger.LogInformation("  - Tamanho: {Size} caracteres", xmlMovimento.Length);

            // Salvar exemplo da consulta de movimento
            var movimentoFile = Path.Combine(Directory.GetCurrentDirectory(), "consulta_movimento_exemplo.xml");
            await File.WriteAllTextAsync(movimentoFile, xmlMovimento);
            _logger.LogInformation("  - Salva em: {File}", movimentoFile);

            // Demonstrar criação de consulta RetInfoPatrocinado
            _logger.LogInformation("\n--- Demonstrando RetInfoPatrocinado ---");
            var consultaPatrocinado = new EFinanceira.Messages.Builders.Consultas.RetInfoPatrocinado.RetInfoPatrocinadoBuilder("v1_2_0")
                .WithDataHoraProcessamento(DateTime.UtcNow)
                .WithStatus(status => status
                    .WithCodigo("200")
                    .WithDescricao("Consulta de patrocinado processada com sucesso"))
                .WithEmpresaDeclarante(empresa => empresa
                    .WithCnpj("12345678000199")
                    .WithGiin("ABC123.45678.LE.987"))
                .WithIdentificacoesPatrocinados(patrocinados => patrocinados
                    .AddPatrocinado(p => p
                        .WithGiin("XYZ987.65432.LE.321")
                        .WithCnpj("11111111000100")
                        .WithNumeroRecibo("REC777888999")
                        .WithId("PAT_001"))
                    .AddPatrocinado(p => p
                        .WithGiin("DEF456.78901.LE.654")
                        .WithCnpj("22222222000200")
                        .WithNumeroRecibo("REC111222333")
                        .WithId("PAT_002")))
                .Build();

            _logger.LogInformation("✓ Consulta RetInfoPatrocinado criada com sucesso");
            _logger.LogInformation("  - Tipo: {Type}", consultaPatrocinado.GetType().Name);
            _logger.LogInformation("  - Versão: {Version}", consultaPatrocinado.Version);

            // Serializar consulta de patrocinado
            var xmlPatrocinado = serializer.Serialize(consultaPatrocinado.Payload);
            _logger.LogInformation("✓ Consulta de patrocinado serializada para XML");
            _logger.LogInformation("  - Tamanho: {Size} caracteres", xmlPatrocinado.Length);

            // Salvar exemplo da consulta de patrocinado
            var patrocinadoFile = Path.Combine(Directory.GetCurrentDirectory(), "consulta_patrocinado_exemplo.xml");
            await File.WriteAllTextAsync(patrocinadoFile, xmlPatrocinado);
            _logger.LogInformation("  - Salva em: {File}", patrocinadoFile);

            // Demonstrar uso do factory
            _logger.LogInformation("\n--- Demonstrando Factory Pattern ---");
            var factory = EFinanceira.Messages.Factory.MessagesFactoryExtensions.CreateConfiguredFactory();

            // Listar tipos registrados
            var tiposRegistrados = factory.GetRegisteredTypes();
            _logger.LogInformation("✓ Factory configurado com {Count} tipos de mensagem", tiposRegistrados.Count());

            foreach (var (kind, version) in tiposRegistrados)
            {
                _logger.LogInformation("  - {Kind} v{Version}", kind, version);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar consulta");
            throw;
        }
    }

    private static async Task DemonstrarValidacao()
    {
        _logger!.LogInformation("\n--- 3. Demonstração: Validação XML ---");

        try
        {
            var serializer = _serviceProvider!.GetRequiredService<IXmlSerializer>();
            var validator = _serviceProvider.GetRequiredService<IXmlValidator>();

            // Criar evento para validar
            var evento = new LeiauteMovimentacaoFinanceiraBuilder()
                .WithId("EVT_VALIDACAO_001")
                .WithDeclarante("12345678000199")
                .WithDeclarado(d => d.WithTipoNI(2).WithNumeroInscricao("98765432000188").WithNome("Teste"))
                .WithMovimentacao(m => m.WithDataInicio(DateTime.Today).WithDataFim(DateTime.Today))
                .Build();

            // Serializar
            var xml = serializer.Serialize(evento.Payload);
            _logger.LogInformation("✓ XML serializado ({Length} caracteres)", xml.Length);

            // Validar (comentado pois não temos XSDs compilados ainda)
            /*
            var xsdPaths = new[]
            {
                "../EFinanceira.Messages/Generated/Eventos/v1_3_0/evtMovimentacaoFinanceira-v1_2_1.xsd",
                "../EFinanceira.Messages/Generated/xmldsig/v1/xmldsig-core-schema.xsd"
            };

            var errors = validator.ValidateAndGetErrors(xml, xsdPaths);
            
            if (errors.Count == 0)
            {
                _logger.LogInformation("✓ XML é válido contra os schemas");
            }
            else
            {
                _logger.LogWarning("⚠ XML possui {Count} erro(s) de validação:", errors.Count);
                foreach (var error in errors.Take(3))
                {
                    _logger.LogWarning("  - {Error}", error);
                }
            }
            */

            _logger.LogInformation("✓ Demonstração de validação preparada (XSDs necessários para validação completa)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante validação");
            throw;
        }
    }

    private static async Task DemonstrarAssinatura()
    {
        _logger!.LogInformation("\n--- 4. Demonstração: Assinatura Digital ---");

        try
        {
            var serializer = _serviceProvider!.GetRequiredService<IXmlSerializer>();
            var signer = _serviceProvider.GetRequiredService<IXmlSigner>();

            // Criar evento
            var evento = new LeiauteMovimentacaoFinanceiraBuilder()
                .WithId("EVT_ASSINATURA_001")
                .WithDeclarante("12345678000199")
                .WithDeclarado(d => d.WithTipoNI(2).WithNumeroInscricao("98765432000188").WithNome("Teste Assinatura"))
                .WithMovimentacao(m => m.WithDataInicio(DateTime.Today).WithDataFim(DateTime.Today))
                .Build();

            // Serializar para assinatura (sem identação)
            var xmlSerializer = serializer as XmlNetSerializer;
            var xml = xmlSerializer!.SerializeForSigning(evento.Payload);

            _logger.LogInformation("✓ XML preparado para assinatura");

            // Configurar opções de assinatura (comentado pois precisaria de certificado real)
            /*
            var signOptions = new SignOptions
            {
                PfxPath = _configuration["EFinanceira:Certificate:PfxPath"],
                PfxPassword = _configuration["EFinanceira:Certificate:PfxPassword"],
                ElementToSignName = evento.RootElementName,
                IdAttributeName = evento.IdAttributeName ?? "Id",
                IdValue = evento.IdValue ?? "EVT_ASSINATURA_001"
            };

            var signedXml = signer.Sign(xml, signOptions);
            _logger.LogInformation("✓ XML assinado com sucesso");

            // Verificar assinatura
            var isValid = signer.VerifySignature(signedXml);
            _logger.LogInformation("✓ Verificação de assinatura: {Valid}", isValid ? "VÁLIDA" : "INVÁLIDA");
            */

            _logger.LogInformation("✓ Demonstração de assinatura preparada (certificado necessário para assinatura completa)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante assinatura");
            throw;
        }
    }

    private static async Task DemonstrarFluxoCompleto()
    {
        _logger!.LogInformation("\n--- 5. Demonstração: Fluxo Completo ---");

        try
        {
            var serializer = _serviceProvider!.GetRequiredService<IXmlSerializer>();
            var cnpjDeclarante = _configuration!["EFinanceira:Declarante:Cnpj"]!;

            _logger.LogInformation("Simulando fluxo completo de produção...");

            // 1. Criar múltiplos eventos
            var eventos = new List<IEFinanceiraMessage>();

            for (int i = 1; i <= 3; i++)
            {
                var evento = new LeiauteMovimentacaoFinanceiraBuilder()
                    .WithId($"EVT_COMPLETO_{i:D3}")
                    .WithDeclarante(cnpjDeclarante)
                    .WithAmbiente(2) // Homologação
                    .WithDeclarado(d =>
                    {
                        d.WithTipoNI(2)
                         .WithNumeroInscricao($"{i:D8}000{i:D3}")
                         .WithNome($"Cliente Completo {i}")
                         .WithEndereco(e => e.WithPais("076").WithCidade("São Paulo"));
                    })
                    .WithMovimentacao(m =>
                    {
                        m.WithDataInicio(DateTime.Today.AddDays(-30))
                         .WithDataFim(DateTime.Today)
                         .AdicionarReportavel(r =>
                         {
                             r.WithTipoReportavel(1)
                              .WithInformacoesConta(c => c.WithTipoConta(1).WithMoeda("986"));
                         });
                    })
                    .Build();

                eventos.Add(evento);
                _logger.LogInformation("  ✓ Evento {Index} criado: {Id}", i, evento.IdValue);
            }

            // 2. Criar lote
            var loteBuilder = new EnvioLoteEventosV120Builder()
                .WithId("LOTE_COMPLETO_001")
                .ComTransmissor(cnpjDeclarante, "Empresa Demo", "demo@empresa.com");

            foreach (var evento in eventos)
            {
                loteBuilder.AdicionarEvento(evento);
            }

            var lote = loteBuilder.Build();
            _logger.LogInformation("  ✓ Lote criado com {Count} eventos", lote.Lote.Eventos.Count);

            // 3. Serializar lote
            var xmlLote = serializer.Serialize(lote.Payload);
            _logger.LogInformation("  ✓ Lote serializado ({Length} caracteres)", xmlLote.Length);

            // 4. Salvar arquivos
            var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "output");
            Directory.CreateDirectory(outputDir);

            // Salvar eventos individuais
            for (int i = 0; i < eventos.Count; i++)
            {
                var xmlEvento = serializer.Serialize(eventos[i].Payload);
                var fileName = Path.Combine(outputDir, $"evento_{i + 1:D2}_{eventos[i].IdValue}.xml");
                await File.WriteAllTextAsync(fileName, xmlEvento);
                _logger.LogInformation("  ✓ Evento salvo: {FileName}", Path.GetFileName(fileName));
            }

            // Salvar lote
            var loteFileName = Path.Combine(outputDir, $"lote_{lote.IdValue}.xml");
            await File.WriteAllTextAsync(loteFileName, xmlLote);
            _logger.LogInformation("  ✓ Lote salvo: {FileName}", Path.GetFileName(loteFileName));

            // 5. Relatório final
            _logger.LogInformation("=== Relatório do Fluxo Completo ===");
            _logger.LogInformation("  - Eventos criados: {Count}", eventos.Count);
            _logger.LogInformation("  - Lote ID: {LoteId}", lote.IdValue);
            _logger.LogInformation("  - Arquivos salvos em: {OutputDir}", outputDir);
            _logger.LogInformation("  - Tamanho total XML: {Size:N0} caracteres", xmlLote.Length);

            // Simular próximos passos
            _logger.LogInformation("\nPróximos passos para produção:");
            _logger.LogInformation("  1. Validar XMLs contra schemas XSD");
            _logger.LogInformation("  2. Assinar digitalmente com certificado A1/A3");
            _logger.LogInformation("  3. Enviar para webservice da Receita Federal");
            _logger.LogInformation("  4. Processar recibos de entrega");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante fluxo completo");
            throw;
        }
    }
}

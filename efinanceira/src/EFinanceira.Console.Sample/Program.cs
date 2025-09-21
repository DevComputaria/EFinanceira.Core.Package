using System.Xml;
using EFinanceira.Console.Sample.Configuration;
using EFinanceira.Core.Abstractions;
using EFinanceira.Core.Factory;
using EFinanceira.Core.Serialization;
using EFinanceira.Core.Signing;
using EFinanceira.Core.Validation;
using EFinanceira.Messages.Builders.Eventos;
using EFinanceira.Messages.Builders.Lotes;
using EFinanceira.Messages.Builders.Xmldsig;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
            await DemonstrarEvtAberturaeFinanceira();
            await DemonstrarXmldsigBuilder();
            await DemonstrarMensagemComAssinatura();
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
        
        // Configuração tipada da e-Financeira
        services.Configure<EFinanceiraSettings>(_configuration.GetSection(EFinanceiraSettings.SectionName));

        _serviceProvider = services.BuildServiceProvider();
    }

    private static async Task DemonstrarCriacaoEvento()
    {
        _logger!.LogInformation("\n--- 1. Demonstração: Criação de Evento ---");

        try
        {
            var cnpjDeclarante = _configuration!["EFinanceira:Declarante:Cnpj"]!;

            // Criar evento de abertura e-Financeira usando builder fluente
            var eventoBuilder = new EFinanceira.Messages.Builders.Eventos.EvtAberturaeFinanceira.EvtAberturaeFinanceiraBuilder()
                .WithId("EVT20241219001")
                .WithIdeDeclarante(decl => decl
                    .WithCnpjDeclarante(cnpjDeclarante))
                .WithInfoAbertura(info => info
                    .WithDataInicio(new DateTime(2024, 1, 1))
                    .WithDataFim(new DateTime(2024, 12, 31)))
                .WithAberturaMovOpFin(mov => mov
                    .WithResponsavelRMF(rmf => rmf
                        .WithCnpj(cnpjDeclarante)
                        .WithNome("João da Silva")
                        .WithSetor("Financeiro")
                        .WithTelefone(tel => tel
                            .WithDDD("11")
                            .WithNumero("987654321")
                            .WithRamal("1001"))
                        .WithEndereco(end => end
                            .WithLogradouro("Rua das Flores")
                            .WithNumero("123")
                            .WithBairro("Centro")
                            .WithCEP("01310-100")
                            .WithMunicipio("São Paulo")
                            .WithUF("SP"))));

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
            var cnpjTransmissor = _configuration!["EFinanceira:Declarante:Cnpj"] ?? "12345678000195";
            var nomeTransmissor = _configuration["EFinanceira:Declarante:Nome"] ?? "Empresa Exemplo LTDA";

            // Criar múltiplos eventos de abertura e-Financeira
            var evento1 = new EFinanceira.Messages.Builders.Eventos.EvtAberturaeFinanceira.EvtAberturaeFinanceiraBuilder()
                .WithId("EVT20241219001")
                .WithIdeDeclarante(decl => decl.WithCnpjDeclarante(cnpjTransmissor))
                .WithInfoAbertura(info => info
                    .WithDataInicio(new DateTime(2024, 1, 1))
                    .WithDataFim(new DateTime(2024, 12, 31)))
                .Build();

            var evento2 = new EFinanceira.Messages.Builders.Eventos.EvtAberturaeFinanceira.EvtAberturaeFinanceiraBuilder()
                .WithId("EVT20241219002")
                .WithIdeDeclarante(decl => decl.WithCnpjDeclarante(cnpjTransmissor))
                .WithInfoAbertura(info => info
                    .WithDataInicio(new DateTime(2024, 1, 1))
                    .WithDataFim(new DateTime(2024, 12, 31)))
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
            _logger.LogWarning("⚠ Erro na demonstração de lote (esperado em ambiente de demonstração): {Message}", ex.Message);
            _logger.LogInformation("  - Para funcionar completamente, configure dados válidos em appsettings.json");
            
            // Não propagar a exceção para não interromper outras demonstrações
            await Task.CompletedTask;
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

            // Demonstrar criação de consulta RetListaeFinanceira
            _logger.LogInformation("\n--- Demonstrando RetListaeFinanceira ---");
            var consultaLista = new EFinanceira.Messages.Builders.Consultas.RetListaeFinanceira.RetListaeFinanceiraBuilder("v1_2_0")
                .WithDataHoraProcessamento(DateTime.UtcNow)
                .WithStatus("200", "Consulta de lista processada com sucesso")
                .WithEmpresaDeclarante("12345678000199")
                .AddInformacoesEFinanceira(
                    new DateTime(2024, 1, 1),
                    new DateTime(2024, 6, 30),
                    "Ativa",
                    "REC100001",
                    "ABT_001",
                    "REC100002",
                    "FEC_001")
                .AddInformacoesEFinanceira(
                    new DateTime(2024, 7, 1),
                    new DateTime(2024, 12, 31),
                    "Encerrada",
                    "REC200001",
                    "ABT_002",
                    "REC200002",
                    "FEC_002")
                .AddOcorrencia("1", "Campo obrigatório", "E001", "Campo CNPJ é obrigatório")
                .Build();

            _logger.LogInformation("✓ Consulta RetListaeFinanceira criada com sucesso");
            _logger.LogInformation("  - Tipo: {Type}", consultaLista.GetType().Name);
            _logger.LogInformation("  - Versão: {Version}", consultaLista.Version);

            // Serializar consulta de lista
            var xmlLista = serializer.Serialize(consultaLista.Payload);
            _logger.LogInformation("✓ Consulta de lista serializada para XML");
            _logger.LogInformation("  - Tamanho: {Size} caracteres", xmlLista.Length);

            // Salvar exemplo da consulta de lista
            var listaFile = Path.Combine(Directory.GetCurrentDirectory(), "consulta_lista_exemplo.xml");
            await File.WriteAllTextAsync(listaFile, xmlLista);
            _logger.LogInformation("  - Salva em: {File}", listaFile);

            // Demonstrar criação de consulta RetRERCT
            _logger.LogInformation("\n--- Demonstrando RetRERCT ---");
            var consultaRERCT = new EFinanceira.Messages.Builders.Consultas.RetRERCT.RetRERCTBuilder("v1_2_0")
                .ComDadosProcessamento(dp => dp
                    .ComCodigoStatus("200")
                    .ComDescricaoResposta("Consulta RERCT processada com sucesso")
                    .ComOcorrencias(oc => oc
                        .AdicionarOcorrencia(o => o
                            .ComTipo("1")
                            .ComCodigo("S001")
                            .ComDescricao("Processamento realizado com sucesso")
                            .ComLocalizacaoErroAviso("Sistema"))))
                .ComDadosEventos(de => de
                    .AdicionarEvento(e => e
                        .ComIdentificacaoEvento(ie => ie
                            .ComIdEvento("EVT20241219001")
                            .ComIdeEventoRERCT("RERCT001")
                            .ComSituacao("Processado")
                            .ComNumeroRecibo("REC123456789"))
                        .ComIdentificacaoDeclarado(id => id
                            .ComTipoInscricao("2")
                            .ComInscricao("12345678000199"))
                        .ComIdentificacoesTitulares(it => it
                            .AdicionarTitular(t => t
                                .ComTipoInscricao("1")
                                .ComInscricao("12345678909")
                                .ComNome("João Silva Santos")
                                .ComNif("PT123456789")))
                        .ComBeneficiarios(b => b
                            .AdicionarBeneficiario(bf => bf
                                .ComInscricao("98765432109")
                                .ComNome("Maria Santos Silva")
                                .ComNif("ES987654321")))))
                .Build();

            _logger.LogInformation("✓ Consulta RetRERCT criada com sucesso");
            _logger.LogInformation("  - Tipo: {Type}", consultaRERCT.GetType().Name);

            // Serializar consulta RERCT
            var xmlRERCT = serializer.Serialize(consultaRERCT.Payload);
            _logger.LogInformation("✓ Consulta RERCT serializada para XML");
            _logger.LogInformation("  - Tamanho: {Size} caracteres", xmlRERCT.Length);

            // Salvar exemplo da consulta RERCT
            var rerctFile = Path.Combine(Directory.GetCurrentDirectory(), "consulta_rerct_exemplo.xml");
            await File.WriteAllTextAsync(rerctFile, xmlRERCT);
            _logger.LogInformation("  - Salva em: {File}", rerctFile);

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

    private static async Task DemonstrarEvtAberturaeFinanceira()
    {
        _logger!.LogInformation("\n--- 4. Demonstração: Evento EvtAberturaeFinanceira ---");

        try
        {
            var cnpjDeclarante = _configuration!["EFinanceira:Declarante:Cnpj"]!;

            // Criar evento de abertura e-Financeira usando builder fluente
            var eventoAbertura = new EFinanceira.Messages.Builders.Eventos.EvtAberturaeFinanceira.EvtAberturaeFinanceiraBuilder("v1_2_1")
                .WithId("ABERT_20241219001")
                .WithIdeEvento(ide => ide
                    .WithIndRetificacao(1) // Original
                    .WithTipoAmbiente(2) // Homologação
                    .WithAplicativoEmissor(1) // Aplicativo do contribuinte
                    .WithVersaoAplicacao("1.0.0"))
                .WithIdeDeclarante(decl => decl
                    .WithCnpjDeclarante(cnpjDeclarante))
                .WithInfoAbertura(info => info
                    .WithDataInicio(new DateTime(2024, 1, 1))
                    .WithDataFim(new DateTime(2024, 12, 31)))
                .WithAberturaPP(pp => pp
                    .AddTipoEmpresa(te => te
                        .WithTipoPrevidenciaPrivada("1")))
                .WithAberturaMovOpFin(mov => mov
                    .WithResponsavelRMF(rmf => rmf
                        .WithCnpj(cnpjDeclarante)
                        .WithNome("João da Silva Responsável")
                        .WithSetor("Financeiro")
                        .WithTelefone(tel => tel
                            .WithDDD("11")
                            .WithNumero("987654321")
                            .WithRamal("1001"))
                        .WithEndereco(end => end
                            .WithLogradouro("Rua das Flores")
                            .WithNumero("123")
                            .WithComplemento("Sala 1001")
                            .WithBairro("Centro")
                            .WithCEP("01310-100")
                            .WithMunicipio("São Paulo")
                            .WithUF("SP")))
                    .WithResponsaveisFinanceiros(resps => resps
                        .AddResponsavelFinanceiro(rf => rf
                            .WithCpf("12345678909")
                            .WithNome("Maria Santos Financeiro")
                            .WithSetor("Compliance")
                            .WithEmail("maria.santos@empresa.com")
                            .WithTelefone(tel => tel
                                .WithDDD("11")
                                .WithNumero("876543210")
                                .WithRamal("2002"))
                            .WithEndereco(end => end
                                .WithLogradouro("Av. Paulista")
                                .WithNumero("1000")
                                .WithBairro("Bela Vista")
                                .WithCEP("01310-200")
                                .WithMunicipio("São Paulo")
                                .WithUF("SP")))
                        .AddResponsavelFinanceiro(rf => rf
                            .WithCpf("98765432100")
                            .WithNome("Carlos Oliveira Junior")
                            .WithSetor("Operações")
                            .WithEmail("carlos.oliveira@empresa.com")
                            .WithTelefone(tel => tel
                                .WithDDD("11")
                                .WithNumero("765432109")
                                .WithRamal("3003"))
                            .WithEndereco(end => end
                                .WithLogradouro("Rua Augusta")
                                .WithNumero("500")
                                .WithComplemento("Andar 10")
                                .WithBairro("Consolação")
                                .WithCEP("01305-000")
                                .WithMunicipio("São Paulo")
                                .WithUF("SP"))))
                    .WithRepresentanteLegal(rep => rep
                        .WithCpf("11111111111")
                        .WithSetor("Diretoria")
                        .WithTelefone(tel => tel
                            .WithDDD("11")
                            .WithNumero("654321098")
                            .WithRamal("1000"))))
                .Build();

            _logger.LogInformation("✓ Evento EvtAberturaeFinanceira criado com sucesso");
            _logger.LogInformation("  - Tipo: {Type}", eventoAbertura.GetType().Name);
            _logger.LogInformation("  - ID: {Id}", eventoAbertura.IdValue);
            _logger.LogInformation("  - Versão: {Version}", eventoAbertura.Version);
            _logger.LogInformation("  - Elemento Raiz: {Root}", eventoAbertura.RootElementName);

            // Demonstrar serialização do evento
            var serializer = _serviceProvider!.GetRequiredService<IXmlSerializer>();
            var xml = serializer.Serialize(eventoAbertura.Payload);

            _logger.LogInformation("✓ Evento serializado para XML");
            _logger.LogInformation("  - Tamanho: {Size} caracteres", xml.Length);

            // Salvar exemplo do evento
            var eventoFile = Path.Combine(Directory.GetCurrentDirectory(), "evento_abertura_exemplo.xml");
            await File.WriteAllTextAsync(eventoFile, xml);
            _logger.LogInformation("  - Salva em: {File}", eventoFile);

            // Demonstrar uso do factory para criar o evento
            _logger.LogInformation("\n--- Demonstrando criação via Factory ---");
            var factory = EFinanceira.Messages.Factory.MessagesFactoryExtensions.CreateConfiguredFactory();
            
            // Configuração simples para o factory
            Action<object>? factoryConfig = builder =>
            {
                var typedBuilder = (EFinanceira.Messages.Builders.Eventos.EvtAberturaeFinanceira.EvtAberturaeFinanceiraBuilder)builder!;
                typedBuilder
                    .WithId("FACTORY_ABERT_001")
                    .WithIdeDeclarante(decl => decl
                        .WithCnpjDeclarante(cnpjDeclarante))
                    .WithInfoAbertura(info => info
                        .WithDataInicio(new DateTime(2024, 1, 1))
                        .WithDataFim(new DateTime(2024, 12, 31)));
            };

            var eventoViaFactory = (EFinanceira.Messages.Builders.Eventos.EvtAberturaeFinanceira.EvtAberturaeFinanceiraMessage)factory.Create(
                EFinanceira.Core.Factory.MessageKind.Evento("EvtAberturaeFinanceira"),
                "v1_2_1",
                factoryConfig);

            _logger.LogInformation("✓ Evento criado via Factory com sucesso");
            _logger.LogInformation("  - ID via Factory: {Id}", eventoViaFactory.IdValue);

            // Serializar evento criado via factory
            var xmlFactory = serializer.Serialize(eventoViaFactory.Payload);
            _logger.LogInformation("✓ Evento via Factory serializado");
            _logger.LogInformation("  - Tamanho: {Size} caracteres", xmlFactory.Length);

            // Salvar evento criado via factory
            var factoryFile = Path.Combine(Directory.GetCurrentDirectory(), "evento_abertura_factory.xml");
            await File.WriteAllTextAsync(factoryFile, xmlFactory);
            _logger.LogInformation("  - Salva em: {File}", factoryFile);

            // Relatório do evento
            _logger.LogInformation("\n=== Relatório do Evento EvtAberturaeFinanceira ===");
            _logger.LogInformation("  - Evento ID: {EventoId}", eventoAbertura.IdValue);
            _logger.LogInformation("  - Declarante: {Declarante}", cnpjDeclarante);
            _logger.LogInformation("  - Período: 01/01/2024 a 31/12/2024");
            _logger.LogInformation("  - Responsáveis financeiros: 2");
            _logger.LogInformation("  - Arquivo XML: {Size:N0} caracteres", xml.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar evento EvtAberturaeFinanceira");
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

            // Criar evento de abertura e-Financeira para validar
            var evento = new EFinanceira.Messages.Builders.Eventos.EvtAberturaeFinanceira.EvtAberturaeFinanceiraBuilder()
                .WithId("EVT_VALIDACAO_001")
                .WithIdeDeclarante(decl => decl.WithCnpjDeclarante("12345678000199"))
                .WithInfoAbertura(info => info
                    .WithDataInicio(DateTime.Today)
                    .WithDataFim(DateTime.Today.AddDays(365)))
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

            // Criar evento de abertura e-Financeira
            var evento = new EFinanceira.Messages.Builders.Eventos.EvtAberturaeFinanceira.EvtAberturaeFinanceiraBuilder()
                .WithId("EVT_ASSINATURA_001")
                .WithIdeDeclarante(decl => decl.WithCnpjDeclarante("12345678000199"))
                .WithInfoAbertura(info => info
                    .WithDataInicio(DateTime.Today)
                    .WithDataFim(DateTime.Today.AddDays(365)))
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
            var cnpjDeclarante = _configuration!["EFinanceira:Declarante:Cnpj"] ?? "12345678000195";

            _logger.LogInformation("Simulando fluxo completo de produção...");

            // 1. Criar múltiplos eventos de abertura e-Financeira
            var eventos = new List<IEFinanceiraMessage>();

            for (int i = 1; i <= 3; i++)
            {
                var evento = new EFinanceira.Messages.Builders.Eventos.EvtAberturaeFinanceira.EvtAberturaeFinanceiraBuilder()
                    .WithId($"EVT_COMPLETO_{i:D3}")
                    .WithIdeDeclarante(decl => decl.WithCnpjDeclarante(cnpjDeclarante))
                    .WithInfoAbertura(info => info
                        .WithDataInicio(DateTime.Today.AddDays(-30))
                        .WithDataFim(DateTime.Today.AddDays(335))) // ~1 ano
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

            // 3. Serializar lote (com tratamento de erro)
            try
            {
                var xmlLote = serializer.Serialize(lote.Payload);
                _logger.LogInformation("  ✓ Lote serializado ({Length} caracteres)", xmlLote.Length);

                // 4. Salvar arquivos
                var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "output");
                Directory.CreateDirectory(outputDir);

                // Salvar eventos individuais
                for (int i = 0; i < eventos.Count; i++)
                {
                    try
                    {
                        var xmlEvento = serializer.Serialize(eventos[i].Payload);
                        var fileName = Path.Combine(outputDir, $"evento_{i + 1:D2}_{eventos[i].IdValue}.xml");
                        await File.WriteAllTextAsync(fileName, xmlEvento);
                        _logger.LogInformation("  ✓ Evento salvo: {FileName}", Path.GetFileName(fileName));
                    }
                    catch (Exception exEvento)
                    {
                        _logger.LogWarning("  ⚠ Erro ao serializar evento {Index}: {Message}", i + 1, exEvento.Message);
                    }
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
            }
            catch (Exception exSerial)
            {
                _logger.LogWarning("⚠ Erro na serialização (comum em ambiente de demonstração): {Message}", exSerial.Message);
                _logger.LogInformation("  - Fluxo de criação funcionou corretamente");
                _logger.LogInformation("  - Para serialização completa, configure esquemas XSD apropriados");
            }

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

    private static async Task DemonstrarXmldsigBuilder()
    {
        try
        {
            _logger.LogInformation("\n=== Demonstração: XMLDSig Builder (Baseado no Exemplo Oficial RF) ===");
            
            // Mostrar configurações carregadas
            var settings = _serviceProvider!.GetRequiredService<IOptions<EFinanceiraSettings>>().Value;
            _logger.LogInformation("Configurações carregadas do appsettings.json:");
            _logger.LogInformation("  - Declarante: {Cnpj} - {Nome}", settings.Declarante.Cnpj, settings.Declarante.Nome);
            _logger.LogInformation("  - Certificado: {Path}", settings.Certificate.PfxPath);
            _logger.LogInformation("  - Ambiente: {Environment}", settings.Environment);
            _logger.LogInformation("  - XSD Base Path: {XsdBasePath}", settings.Validation.XsdBasePath);

            // Exemplo 1: Demonstrar seleção interativa de certificado
            _logger.LogInformation("\nDemonstrando seleção de certificado...");
            
            try
            {
                using var builderInterativo = new XmldsigBuilder();
                builderInterativo.WithInteractiveCertificateSelection();
                _logger.LogInformation("✓ Certificado selecionado com sucesso");
                
                // Criar um XML de evento de exemplo para assinar
                var xmlEvento = CreateSampleEventXml();
                var xmlAssinado = builderInterativo.SignXmlEvent(xmlEvento);
                
                _logger.LogInformation("✓ Evento XML assinado com sucesso");
                _logger.LogInformation("  - Tamanho XML assinado: {Size} caracteres", xmlAssinado.OuterXml.Length);
                
                // Validar assinatura
                var isValid = XmldsigBuilder.ValidateSignature(xmlAssinado);
                _logger.LogInformation("  - Assinatura válida: {IsValid}", isValid);
                
                // Salvar exemplo
                var outputPath = Path.Combine("exemplos", "evento-assinado-rf.xml");
                Directory.CreateDirectory("exemplos");
                xmlAssinado.Save(outputPath);
                _logger.LogInformation("  - Arquivo salvo: {Path}", outputPath);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("⚠ Certificado não disponível (esperado em demonstração): {Message}", ex.Message);
            }

            // Exemplo 2: Demonstrar assinatura com certificado de arquivo (usando appsettings.json)
            _logger.LogInformation("\nDemonstrando uso com certificado de arquivo (configurado em appsettings.json)...");
            
            try
            {
                // Obter configurações do appsettings.json
                var eFinanceiraSettings = _serviceProvider!.GetRequiredService<IOptions<EFinanceiraSettings>>().Value;
                
                using var builderArquivo = new XmldsigBuilder();
                builderArquivo.WithCertificateFromFile(eFinanceiraSettings.Certificate.PfxPath, eFinanceiraSettings.Certificate.PfxPassword);
                
                var xmlEvento = CreateSampleEventXml();
                var xmlAssinado = builderArquivo.SignXmlEvent(xmlEvento);
                
                _logger.LogInformation("✓ Evento assinado com certificado de arquivo");
                _logger.LogInformation("  - Certificado: {Path}", eFinanceiraSettings.Certificate.PfxPath);
                _logger.LogInformation("  - Configurado em: appsettings.json");
            }
            catch (FileNotFoundException)
            {
                _logger.LogWarning("⚠ Arquivo de certificado não encontrado");
                _logger.LogInformation("  - Verifique o caminho em appsettings.json: EFinanceira:Certificate:PfxPath");
            }
            catch (DirectoryNotFoundException)
            {
                _logger.LogWarning("⚠ Diretório de certificado não encontrado");
                _logger.LogInformation("  - Verifique o PfxPath em appsettings.json: {Path}", _configuration!["EFinanceira:Certificate:PfxPath"]);
            }

            // Exemplo 3: Demonstrar assinatura de lote completo
            _logger.LogInformation("\nDemonstrando assinatura de lote de eventos...");
            
            try
            {
                using var builderLote = new XmldsigBuilder();
                builderLote.WithCertificateFromStore("THUMBPRINT_EXEMPLO");
                
                var xmlLote = CreateSampleBatchXml();
                var xmlLoteAssinado = builderLote.SignXmlDocument(xmlLote);
                
                _logger.LogInformation("✓ Lote de eventos assinado com sucesso");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("⚠ Certificado não encontrado no repositório: {Message}", ex.Message);
                _logger.LogInformation("  - Para usar: instale certificado válido no repositório do Windows");
            }

            // Exemplo 4: Criar assinatura básica para compatibilidade
            _logger.LogInformation("\nDemonstrando criação de assinatura básica...");
            
            var assinaturaBasica = XmldsigBuilder.BuildBasicSignature("SIGNATURE_BASICA_001");
            
            _logger.LogInformation("✓ Assinatura básica criada");
            _logger.LogInformation("  - ID: {Id}", assinaturaBasica.IdValue);
            _logger.LogInformation("  - Algoritmo: RSA-SHA256");

            _logger.LogInformation("\n=== Características do XmldsigBuilder Melhorado ===");
            _logger.LogInformation("✓ Baseado no exemplo oficial da Receita Federal");
            _logger.LogInformation("✓ Algoritmos corretos: RSA-SHA256 e SHA256");
            _logger.LogInformation("✓ Suporte a assinatura de eventos individuais");
            _logger.LogInformation("✓ Suporte a assinatura de lotes completos");
            _logger.LogInformation("✓ Validação automática de assinaturas");
            _logger.LogInformation("✓ Detecção automática de tipos de evento e-Financeira");
            _logger.LogInformation("✓ Seleção interativa de certificados");
            _logger.LogInformation("✓ Validação robusta de certificados");
            _logger.LogInformation("✓ Gestão adequada de recursos com IDisposable");

            _logger.LogInformation("\n=== Tipos de Eventos Suportados ===");
            _logger.LogInformation("• evtCadDeclarante - Cadastro de Declarante");
            _logger.LogInformation("• evtAberturaeFinanceira - Abertura de Conta");
            _logger.LogInformation("• evtCadIntermediario - Cadastro de Intermediário");
            _logger.LogInformation("• evtCadPatrocinado - Cadastro de Patrocinado");
            _logger.LogInformation("• evtExclusaoeFinanceira - Exclusão de Conta");
            _logger.LogInformation("• evtExclusao - Exclusão Geral");
            _logger.LogInformation("• evtFechamentoeFinanceira - Fechamento de Conta");
            _logger.LogInformation("• evtMovOpFin - Movimento de Operação Financeira");
            _logger.LogInformation("• evtMovPP - Movimento de Pessoa Política");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao demonstrar XMLDSig builder");
            throw;
        }
    }

    private static XmlDocument CreateSampleEventXml()
    {
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<evtAberturaeFinanceira id=""EVENTO_001"" xmlns=""http://www.eFinanceira.gov.br/schemas/evtAberturaeFinanceira/v1_2_1"">
    <ideEvento>
        <indRetif>1</indRetif>
        <tpAmb>2</tpAmb>
        <aplicEmi>1</aplicEmi>
        <verAplic>1.0.0</verAplic>
    </ideEvento>
    <ideDeclarante>
        <cnpjDeclarante>12345678000195</cnpjDeclarante>
    </ideDeclarante>
    <infoAbertura>
        <dtInicio>2024-01-01</dtInicio>
        <dtFim>2024-12-31</dtFim>
    </infoAbertura>
</evtAberturaeFinanceira>";

        var xmlDoc = new XmlDocument { PreserveWhitespace = true };
        xmlDoc.LoadXml(xml);
        return xmlDoc;
    }

    private static XmlDocument CreateSampleBatchXml()
    {
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<eFinanceira xmlns=""http://www.eFinanceira.gov.br/schemas/loteEventos/v1_2_0"">
    <loteEventos id=""LOTE_001"">
        <ideTransmissor>
            <cnpjTransmissor>12345678000195</cnpjTransmissor>
            <nmTransmissor>Empresa Exemplo</nmTransmissor>
        </ideTransmissor>
        <evento id=""EVT_001"">
            <evtAberturaeFinanceira id=""EVENTO_001"">
                <ideEvento>
                    <indRetif>1</indRetif>
                    <tpAmb>2</tpAmb>
                    <aplicEmi>1</aplicEmi>
                    <verAplic>1.0.0</verAplic>
                </ideEvento>
                <ideDeclarante>
                    <cnpjDeclarante>12345678000195</cnpjDeclarante>
                </ideDeclarante>
                <infoAbertura>
                    <dtInicio>2024-01-01</dtInicio>
                    <dtFim>2024-12-31</dtFim>
                </infoAbertura>
            </evtAberturaeFinanceira>
        </evento>
    </loteEventos>
</eFinanceira>";

        var xmlDoc = new XmlDocument { PreserveWhitespace = true };
        xmlDoc.LoadXml(xml);
        return xmlDoc;
    }

    private static async Task DemonstrarMensagemComAssinatura()
    {
        try
        {
            _logger!.LogInformation("\n--- 🔐 Demonstração: Mensagem Completa com Assinatura Digital ---");

            // Criar mensagem completa com assinatura
            var xmlAssinado = await ExemploAssinatura.CriarMensagemComAssinatura(_serviceProvider!, _logger);

            // Verificar mensagem assinada
            ExemploAssinatura.VerificarMensagemAssinada(xmlAssinado, _logger);

            _logger.LogInformation("✅ Demonstração de mensagem com assinatura concluída com sucesso!");
        }
        catch (Exception ex)
        {
            _logger!.LogError(ex, "❌ Erro na demonstração de mensagem com assinatura");
            _logger.LogInformation("💡 Dica: Verifique se o certificado está configurado corretamente em appsettings.json");
        }
    }
}

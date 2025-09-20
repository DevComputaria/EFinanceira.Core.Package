using EFinanceira.Core.Extensions;
using EFinanceira.Core.Models;
using EFinanceira.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EFinanceira.Example.ConsoleApp;

class Program
{
    static async Task Main(string[] args)
    {
        // Configurar host com Dependency Injection
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Registrar os serviços da e-Financeira
                services.AddEFinanceira(options =>
                {
                    options.VersaoLayout = "1.0.0";
                    options.ValidateGeneratedXml = true;
                    options.TimeoutMs = 30000;
                });

                // Registrar serviços do exemplo
                services.AddTransient<EFinanceiraExampleService>();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            })
            .Build();

        try
        {
            Console.WriteLine("=== EFinanceira.Core.Package - Exemplo de Uso ===\n");

            var exampleService = host.Services.GetRequiredService<EFinanceiraExampleService>();
            await exampleService.ExecutarExemplo();

            Console.WriteLine("\n=== Exemplo executado com sucesso! ===");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro durante execução: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Erro interno: {ex.InnerException.Message}");
            }
        }

        Console.WriteLine("\nPressione qualquer tecla para sair...");
        Console.ReadKey();
    }
}

public class EFinanceiraExampleService
{
    private readonly IEFinanceiraService _eFinanceiraService;
    private readonly ILogger<EFinanceiraExampleService> _logger;

    public EFinanceiraExampleService(
        IEFinanceiraService eFinanceiraService,
        ILogger<EFinanceiraExampleService> logger)
    {
        _eFinanceiraService = eFinanceiraService;
        _logger = logger;
    }

    public async Task ExecutarExemplo()
    {
        _logger.LogInformation("Iniciando exemplo de geração de arquivo e-Financeira");

        // 1. Criar informações do responsável
        var responsavel = new ResponsavelIdentificacao
        {
            CnpjRespons = "11222333000181",
            NmRespons = "Empresa Exemplo Ltda",
            CpfRespons = "12345678909",
            Email = "contato@empresa.com.br",
            Telefone = "11999999999"
        };

        Console.WriteLine($"1. Responsável configurado: {responsavel.NmRespons} ({responsavel.CnpjRespons})");

        // 2. Criar envelope
        var envelope = await _eFinanceiraService.CreateEnvelopeAsync(responsavel);
        Console.WriteLine("2. Envelope e-Financeira criado");

        // 3. Adicionar movimentação financeira
        var movimentacao = new MovimentacaoFinanceira
        {
            IdeConta = new IdentificacaoConta
            {
                CnpjInstituicao = "60746948000112", // Banco Bradesco
                NumeroConta = "123456",
                DigitoConta = "7",
                TpConta = TipoConta.ContaCorrente,
                Agencia = "1234",
                DigitoAgencia = "5"
            },
            IniMovFin = new PeriodoReferencia
            {
                DtIni = new DateTime(2024, 1, 1),
                DtFim = new DateTime(2024, 1, 31)
            },
            Movimentacoes = new List<Movimento>
            {
                new Movimento
                {
                    DtMov = new DateTime(2024, 1, 15),
                    TpMov = TipoMovimento.Credito,
                    Valor = 1500.00m,
                    Descricao = "Depósito em conta"
                },
                new Movimento
                {
                    DtMov = new DateTime(2024, 1, 20),
                    TpMov = TipoMovimento.Debito,
                    Valor = 750.00m,
                    Descricao = "Saque em caixa eletrônico"
                }
            }
        };

        var eventoId1 = await _eFinanceiraService.AddMovimentacaoFinanceiraAsync(envelope, movimentacao);
        Console.WriteLine($"3. Movimentação financeira adicionada (ID: {eventoId1})");

        // 4. Adicionar evento de abertura de conta
        var aberturaConta = new AberturaConta
        {
            IdeConta = new IdentificacaoConta
            {
                CnpjInstituicao = "60746948000112",
                NumeroConta = "654321",
                DigitoConta = "3",
                TpConta = TipoConta.ContaPoupanca,
                Agencia = "1234",
                DigitoAgencia = "5"
            },
            DtAbertura = new DateTime(2024, 1, 10),
            TpConta = TipoConta.ContaPoupanca
        };

        var eventoId2 = await _eFinanceiraService.AddAberturaContaAsync(envelope, aberturaConta);
        Console.WriteLine($"4. Abertura de conta adicionada (ID: {eventoId2})");

        // 5. Processar e gerar XML
        var xml = await _eFinanceiraService.ProcessAsync(envelope);
        Console.WriteLine("5. XML gerado e validado com sucesso");

        // 6. Salvar em arquivo
        var filePath = Path.Combine(Path.GetTempPath(), $"e-financeira-exemplo-{DateTime.Now:yyyyMMdd-HHmmss}.xml");
        await _eFinanceiraService.ProcessToFileAsync(envelope, filePath);
        Console.WriteLine($"6. Arquivo salvo em: {filePath}");

        // 7. Exibir resumo
        Console.WriteLine("\n--- RESUMO ---");
        Console.WriteLine($"Total de eventos: {envelope.Eventos.Count}");
        Console.WriteLine($"Responsável: {envelope.IdeRespons.NmRespons}");
        Console.WriteLine($"Data/Hora: {envelope.IdeEvento.DhEvento:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"Versão: {envelope.IdeEvento.VersaoEvento}");
        Console.WriteLine($"Tamanho do XML: {xml.Length:N0} caracteres");

        // 8. Exibir trecho do XML (primeiros 500 caracteres)
        Console.WriteLine("\n--- PREVIEW DO XML ---");
        var preview = xml.Length > 500 ? xml.Substring(0, 500) + "..." : xml;
        Console.WriteLine(preview);

        _logger.LogInformation("Exemplo executado com sucesso. Arquivo salvo em {FilePath}", filePath);
    }
}

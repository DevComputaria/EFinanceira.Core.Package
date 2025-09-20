# EFinanceira.Core.Package

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![NuGet](https://img.shields.io/nuget/v/EFinanceira.Core.Package.svg)](https://www.nuget.org/packages/EFinanceira.Core.Package/)

Biblioteca .NET para gerar os arquivos XML da e-Financeira, baseada nos esquemas XSD oficiais da Receita Federal. Facilita a criação, validação e serialização dos dados seguindo as melhores práticas do .NET Core.

## Características

- ✅ **Conformidade com XSD Oficial**: Baseado nos esquemas oficiais da Receita Federal
- ✅ **Validação Robusta**: Validação completa de CNPJ, CPF e estruturas de dados
- ✅ **Async/Await**: Suporte completo a operações assíncronas
- ✅ **Dependency Injection**: Integração nativa com DI do .NET
- ✅ **Logging**: Logging estruturado com Microsoft.Extensions.Logging
- ✅ **Tratamento de Erros**: Exceções específicas e tratamento centralizado
- ✅ **Testável**: Arquitetura modular com interfaces para testes
- ✅ **Performance**: Otimizado para processamento eficiente de grandes volumes

## Instalação

```bash
dotnet add package EFinanceira.Core.Package
```

## Uso Básico

### 1. Configuração no Startup/Program.cs

```csharp
using EFinanceira.Core.Extensions;

// Registro dos serviços com configuração padrão
builder.Services.AddEFinanceira();

// Ou com configuração personalizada
builder.Services.AddEFinanceira(options =>
{
    options.VersaoLayout = "1.0.0";
    options.ValidateGeneratedXml = true;
    options.TimeoutMs = 30000;
});

// Ou através de configuração (appsettings.json)
builder.Services.AddEFinanceira(builder.Configuration);
```

### 2. Configuração no appsettings.json

```json
{
  "EFinanceira": {
    "VersaoLayout": "1.0.0",
    "ValidateGeneratedXml": true,
    "TimeoutMs": 30000,
    "Logging": {
      "EnableDetailedLogging": true,
      "LogSensitiveData": false,
      "MinimumLevel": "Information"
    },
    "Validation": {
      "ValidateDocuments": true,
      "ValidateFutureDates": true,
      "AllowZeroValues": false,
      "MaxEventsPerEnvelope": 1000,
      "MaxMovementsPerEvent": 10000
    }
  }
}
```

### 3. Exemplo Completo

```csharp
using EFinanceira.Core.Models;
using EFinanceira.Core.Services;
using Microsoft.Extensions.DependencyInjection;

// Injeção do serviço
public class EFinanceiraController : ControllerBase
{
    private readonly IEFinanceiraService _eFinanceiraService;

    public EFinanceiraController(IEFinanceiraService eFinanceiraService)
    {
        _eFinanceiraService = eFinanceiraService;
    }

    public async Task<IActionResult> GerarArquivoEFinanceira()
    {
        try
        {
            // 1. Criar responsável
            var responsavel = new ResponsavelIdentificacao
            {
                CnpjRespons = "11222333000181",
                NmRespons = "Empresa Exemplo Ltda",
                CpfRespons = "12345678909",
                Email = "contato@empresa.com.br",
                Telefone = "11999999999"
            };

            // 2. Criar envelope
            var envelope = await _eFinanceiraService.CreateEnvelopeAsync(responsavel);

            // 3. Adicionar movimentação financeira
            var movimentacao = new MovimentacaoFinanceira
            {
                IdeConta = new IdentificacaoConta
                {
                    CnpjInstituicao = "60746948000112",
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

            await _eFinanceiraService.AddMovimentacaoFinanceiraAsync(envelope, movimentacao);

            // 4. Processar e gerar XML
            var xml = await _eFinanceiraService.ProcessAsync(envelope);

            // 5. Salvar em arquivo (opcional)
            var filePath = Path.Combine(Path.GetTempPath(), $"e-financeira-{DateTime.Now:yyyyMMdd-HHmmss}.xml");
            await _eFinanceiraService.ProcessToFileAsync(envelope, filePath);

            return Ok(new { Message = "Arquivo gerado com sucesso", FilePath = filePath, Xml = xml });
        }
        catch (EFinanceiraValidationException ex)
        {
            return BadRequest(new { Message = "Erro de validação", Errors = ex.ValidationErrors });
        }
        catch (EFinanceiraException ex)
        {
            return StatusCode(500, new { Message = ex.Message, ErrorCode = ex.ErrorCode });
        }
    }
}
```

## Tipos de Eventos Suportados

### 1. Movimentação Financeira

```csharp
var movimentacao = new MovimentacaoFinanceira
{
    IdeConta = new IdentificacaoConta
    {
        CnpjInstituicao = "60746948000112",
        NumeroConta = "123456",
        TpConta = TipoConta.ContaCorrente
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
            Valor = 1500.00m
        }
    }
};

await _eFinanceiraService.AddMovimentacaoFinanceiraAsync(envelope, movimentacao);
```

### 2. Abertura de Conta

```csharp
var aberturaConta = new AberturaConta
{
    IdeConta = new IdentificacaoConta
    {
        CnpjInstituicao = "60746948000112",
        NumeroConta = "654321",
        TpConta = TipoConta.ContaPoupanca
    },
    DtAbertura = new DateTime(2024, 1, 10),
    TpConta = TipoConta.ContaPoupanca
};

await _eFinanceiraService.AddAberturaContaAsync(envelope, aberturaConta);
```

### 3. Fechamento de Conta

```csharp
var fechamentoConta = new FechamentoConta
{
    IdeConta = new IdentificacaoConta
    {
        CnpjInstituicao = "60746948000112",
        NumeroConta = "654321",
        TpConta = TipoConta.ContaPoupanca
    },
    DtFechamento = new DateTime(2024, 12, 31)
};

await _eFinanceiraService.AddFechamentoContaAsync(envelope, fechamentoConta);
```

## Validação

A biblioteca inclui validação automática para:

- **CNPJ**: Validação de dígito verificador
- **CPF**: Validação de dígito verificador  
- **Datas**: Validação de datas futuras
- **Estruturas**: Validação de dados obrigatórios
- **XML**: Validação contra esquema XSD

```csharp
// Validação manual (opcional)
try
{
    await _validator.ValidateAsync(envelope);
}
catch (EFinanceiraValidationException ex)
{
    foreach (var error in ex.ValidationErrors)
    {
        Console.WriteLine($"Erro: {error}");
    }
}
```

## Tratamento de Erros

```csharp
try
{
    var xml = await _eFinanceiraService.ProcessAsync(envelope);
}
catch (EFinanceiraValidationException ex)
{
    // Erros de validação de dados
    Console.WriteLine($"Erros de validação: {string.Join(", ", ex.ValidationErrors)}");
}
catch (EFinanceiraSerializationException ex)
{
    // Erros de serialização XML
    Console.WriteLine($"Erro de serialização: {ex.Message}");
}
catch (EFinanceiraConfigurationException ex)
{
    // Erros de configuração
    Console.WriteLine($"Erro de configuração: {ex.Message}");
}
catch (EFinanceiraException ex)
{
    // Outros erros da biblioteca
    Console.WriteLine($"Erro: {ex.Message} (Código: {ex.ErrorCode})");
}
```

## Logging

A biblioteca utiliza `Microsoft.Extensions.Logging` para logging estruturado:

```csharp
// Os logs são automaticamente gerados durante as operações
// Configure o nível de log conforme necessário

builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});
```

## Extensibilidade

### Validador Customizado

```csharp
public class MeuValidadorCustomizado : IEFinanceiraValidator
{
    // Implementar interface customizada
}

// Registrar validador customizado
services.AddEFinanceiraValidator<MeuValidadorCustomizado>();
```

### Serviço XML Customizado

```csharp
public class MeuServicoXmlCustomizado : IEFinanceiraXmlService
{
    // Implementar interface customizada
}

// Registrar serviço customizado
services.AddEFinanceiraXmlService<MeuServicoXmlCustomizado>();
```

## Testes

```bash
# Executar todos os testes
dotnet test

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## Contribuindo

1. Fork o projeto
2. Crie sua feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## Suporte

- **Documentação**: [GitHub Wiki](https://github.com/DevComputaria/EFinanceira.Core.Package/wiki)
- **Issues**: [GitHub Issues](https://github.com/DevComputaria/EFinanceira.Core.Package/issues)
- **Discussões**: [GitHub Discussions](https://github.com/DevComputaria/EFinanceira.Core.Package/discussions)

## Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.

## Roadmap

- [ ] Suporte a esquemas XSD 2.0
- [ ] Geração de relatórios de validação
- [ ] Integração com Azure Service Bus
- [ ] CLI para processamento batch
- [ ] Suporte a assinatura digital

---

**Desenvolvido com ❤️ pela [DevComputaria](https://github.com/DevComputaria)**

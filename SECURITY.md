# Guia de Segurança e Performance - EFinanceira.Core.Package

## Considerações de Segurança

### 1. Validação de Dados
- **CNPJ/CPF**: Validação automática de dígitos verificadores
- **Datas**: Validação de datas futuras e formatos
- **Valores**: Validação de valores negativos e ranges
- **Estruturas**: Validação de dados obrigatórios e formatos

```csharp
// Validação automática habilitada por padrão
services.AddEFinanceira(options =>
{
    options.Validation.ValidateDocuments = true;
    options.Validation.ValidateFutureDates = true;
    options.Validation.AllowZeroValues = false;
});
```

### 2. Tratamento de Dados Sensíveis
- **Logging**: Controle de dados sensíveis em logs
- **Serialização**: Dados sensíveis não são expostos
- **Memória**: Limpeza automática de objetos temporários

```csharp
services.AddEFinanceira(options =>
{
    options.Logging.LogSensitiveData = false; // NUNCA em produção
    options.Logging.EnableDetailedLogging = true;
});
```

### 3. Validação de Input
- **XSS**: Prevenção através de validação rigorosa
- **Injection**: Validação de entrada e sanitização
- **Overflow**: Controle de tamanho de dados

### 4. HTTPS e Comunicação Segura
```csharp
// Configure HTTPS em produção
app.UseHttpsRedirection();
app.UseHsts(); // HTTP Strict Transport Security
```

### 5. Autenticação e Autorização (Recomendado)
```csharp
// Implementar JWT para APIs
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Configuração JWT
    });
```

## Otimização de Performance

### 1. Configuração Assíncrona
```csharp
// Use sempre async/await
var xml = await _eFinanceiraService.ProcessAsync(envelope, cancellationToken);

// Configure timeouts adequados
services.AddEFinanceira(options =>
{
    options.TimeoutMs = 30000; // 30 segundos
});
```

### 2. Processamento em Lote
```csharp
// Para grandes volumes, processe em lotes
var envelopes = SplitIntoSmallBatches(largeDataSet, batchSize: 100);
var tasks = envelopes.Select(ProcessEnvelopeAsync);
await Task.WhenAll(tasks);
```

### 3. Cache e Reutilização
```csharp
// Reutilize instâncias do serviço (via DI)
services.AddEFinanceira(); // Singleton para options, Scoped para serviços

// Cache validações quando possível
private readonly Dictionary<string, bool> _cnpjValidationCache = new();
```

### 4. Streaming para Arquivos Grandes
```csharp
// Para arquivos grandes, use serialização direta para arquivo
await _eFinanceiraService.ProcessToFileAsync(envelope, filePath, cancellationToken);
```

### 5. Configuração de Logging
```csharp
// Configure níveis de log adequados para produção
{
  "Logging": {
    "LogLevel": {
      "EFinanceira": "Information", // Não use Debug em produção
      "Microsoft": "Warning"
    }
  }
}
```

### 6. Monitoramento de Memória
```csharp
// Configure limites para prevenir OutOfMemory
services.AddEFinanceira(options =>
{
    options.Validation.MaxEventsPerEnvelope = 1000;
    options.Validation.MaxMovementsPerEvent = 10000;
});
```

### 7. Pool de Objetos (Avançado)
```csharp
// Para cenários de alta throughput
services.AddSingleton<ObjectPool<XmlSerializer>>();
services.AddSingleton<ObjectPool<StringBuilder>>();
```

## Boas Práticas

### 1. Tratamento de Exceções
```csharp
try
{
    var xml = await _eFinanceiraService.ProcessAsync(envelope);
}
catch (EFinanceiraValidationException ex)
{
    // Log específico para validação
    _logger.LogWarning("Validation errors: {Errors}", ex.ValidationErrors);
    return BadRequest(new { Errors = ex.ValidationErrors });
}
catch (EFinanceiraException ex)
{
    // Log para erros da biblioteca
    _logger.LogError(ex, "EFinanceira error: {ErrorCode}", ex.ErrorCode);
    return StatusCode(500, new { Error = ex.Message, Code = ex.ErrorCode });
}
catch (Exception ex)
{
    // Log para erros inesperados
    _logger.LogError(ex, "Unexpected error during processing");
    return StatusCode(500, new { Error = "Internal server error" });
}
```

### 2. Validação de Entrada
```csharp
[ApiController]
public class EFinanceiraController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Generate([FromBody] GenerateRequest request)
    {
        // Validação de modelo
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validação de negócio
        if (request.Eventos?.Count > 1000)
        {
            return BadRequest("Máximo de 1000 eventos por envelope");
        }

        // Processamento...
    }
}
```

### 3. Configuração por Ambiente
```csharp
// appsettings.Development.json
{
  "EFinanceira": {
    "Logging": {
      "EnableDetailedLogging": true,
      "LogSensitiveData": true
    }
  }
}

// appsettings.Production.json
{
  "EFinanceira": {
    "Logging": {
      "EnableDetailedLogging": false,
      "LogSensitiveData": false,
      "MinimumLevel": "Warning"
    }
  }
}
```

### 4. Testes de Performance
```csharp
[Fact]
public async Task ProcessLargeEnvelope_ShouldCompleteWithinTimeout()
{
    // Arrange
    var envelope = CreateLargeEnvelope(1000); // 1000 eventos
    var stopwatch = Stopwatch.StartNew();

    // Act
    var xml = await _service.ProcessAsync(envelope);

    // Assert
    stopwatch.Stop();
    stopwatch.ElapsedMilliseconds.Should().BeLessThan(30000); // 30 segundos
    xml.Should().NotBeNullOrEmpty();
}
```

### 5. Health Checks
```csharp
services.AddHealthChecks()
    .AddCheck<EFinanceiraHealthCheck>("e-financeira");

public class EFinanceiraHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Teste básico de validação
            var validator = context.Registration.Factory.GetService<IEFinanceiraValidator>();
            var isValid = validator.ValidateCnpj("11222333000181");
            
            return isValid 
                ? HealthCheckResult.Healthy("EFinanceira service is healthy")
                : HealthCheckResult.Unhealthy("EFinanceira validation failed");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("EFinanceira service error", ex);
        }
    }
}
```

## Checklist de Produção

- [ ] **Segurança**
  - [ ] HTTPS configurado
  - [ ] Autenticação implementada
  - [ ] Validação de entrada habilitada
  - [ ] Logs de dados sensíveis desabilitados

- [ ] **Performance**
  - [ ] Timeouts configurados
  - [ ] Limits de processamento definidos
  - [ ] Cache implementado onde aplicável
  - [ ] Monitoring ativo

- [ ] **Monitoramento**
  - [ ] Health checks configurados
  - [ ] Logs estruturados
  - [ ] Métricas de performance
  - [ ] Alertas configurados

- [ ] **Testes**
  - [ ] Testes unitários > 80% cobertura
  - [ ] Testes de integração
  - [ ] Testes de performance
  - [ ] Testes de segurança
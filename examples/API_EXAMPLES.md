# Exemplos de Uso da API e-Financeira

## Swagger/OpenAPI

A biblioteca inclui suporte completo ao Swagger/OpenAPI para documentação automática da API.

### Configuração Swagger

```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EFinanceira API",
        Version = "v1.0.0",
        Description = "API para geração de arquivos XML da e-Financeira",
        Contact = new OpenApiContact
        {
            Name = "DevComputaria",
            Email = "contato@devcomputaria.com",
            Url = new Uri("https://github.com/DevComputaria")
        }
    });

    // Incluir comentários XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
```

## Exemplos de Requisições

### 1. Gerar XML e-Financeira - Movimentação Financeira

**POST** `/api/e-financeira/generate`

```json
{
  "responsavel": {
    "cnpjRespons": "11222333000181",
    "nmRespons": "Empresa Exemplo Ltda",
    "cpfRespons": "12345678909",
    "telefone": "11999999999",
    "email": "contato@empresa.com.br"
  },
  "movimentacoes": [
    {
      "ideConta": {
        "cnpjInstituicao": "60746948000112",
        "numeroConta": "123456",
        "digitoConta": "7",
        "tpConta": 1,
        "agencia": "1234",
        "digitoAgencia": "5"
      },
      "iniMovFin": {
        "dtIni": "2024-01-01T00:00:00",
        "dtFim": "2024-01-31T23:59:59"
      },
      "movimentacoes": [
        {
          "dtMov": "2024-01-15T10:30:00",
          "tpMov": 1,
          "valor": 1500.00,
          "descricao": "Depósito em conta"
        },
        {
          "dtMov": "2024-01-20T14:15:00",
          "tpMov": 2,
          "valor": 750.00,
          "descricao": "Saque em caixa eletrônico"
        }
      ]
    }
  ]
}
```

**Resposta:**
```json
{
  "success": true,
  "xml": "<?xml version=\"1.0\" encoding=\"utf-8\"?>...",
  "eventCount": 1,
  "responsavel": "Empresa Exemplo Ltda",
  "generatedAt": "2024-01-15T10:30:00Z"
}
```

### 2. Gerar XML e-Financeira - Abertura de Conta

**POST** `/api/e-financeira/generate`

```json
{
  "responsavel": {
    "cnpjRespons": "11222333000181",
    "nmRespons": "Empresa Exemplo Ltda",
    "email": "contato@empresa.com.br"
  },
  "aberturaContas": [
    {
      "ideConta": {
        "cnpjInstituicao": "60746948000112",
        "numeroConta": "654321",
        "digitoConta": "3",
        "tpConta": 2,
        "agencia": "1234",
        "digitoAgencia": "5"
      },
      "dtAbertura": "2024-01-10T00:00:00",
      "tpConta": 2
    }
  ]
}
```

### 3. Gerar XML e-Financeira - Fechamento de Conta

**POST** `/api/e-financeira/generate`

```json
{
  "responsavel": {
    "cnpjRespons": "11222333000181",
    "nmRespons": "Empresa Exemplo Ltda",
    "email": "contato@empresa.com.br"
  },
  "fechamentoContas": [
    {
      "ideConta": {
        "cnpjInstituicao": "60746948000112",
        "numeroConta": "654321",
        "digitoConta": "3",
        "tpConta": 2,
        "agencia": "1234",
        "digitoAgencia": "5"
      },
      "dtFechamento": "2024-12-31T23:59:59"
    }
  ]
}
```

### 4. Validar Dados sem Gerar XML

**POST** `/api/e-financeira/validate`

```json
{
  "responsavel": {
    "cnpjRespons": "11222333000181",
    "nmRespons": "Empresa Exemplo Ltda",
    "email": "contato@empresa.com.br"
  }
}
```

**Resposta:**
```json
{
  "success": true,
  "message": "Dados válidos"
}
```

## Códigos de Resposta

| Código | Descrição | Exemplo |
|--------|-----------|---------|
| 200 | Sucesso | XML gerado com sucesso |
| 400 | Erro de validação | CNPJ inválido, dados obrigatórios ausentes |
| 500 | Erro interno | Erro de serialização, timeout |

## Exemplos de Erro

### Erro de Validação (400)
```json
{
  "error": "Erro de validação dos dados",
  "details": [
    "CNPJ do responsável é inválido",
    "Data de abertura da conta não pode ser futura"
  ]
}
```

### Erro de Serialização (500)
```json
{
  "error": "Erro durante a serialização XML",
  "errorCode": "SERIALIZATION_ERROR"
}
```

## Enums e Valores

### TipoConta
- `1` - Conta Corrente
- `2` - Conta Poupança
- `3` - Conta de Depósito a Prazo
- `4` - Conta de Investimento

### SubtipoConta
- `1` - Individual
- `2` - Conjunta
- `3` - Corporativa

### TipoMovimento
- `1` - Crédito
- `2` - Débito

## Exemplo com curl

```bash
# Gerar XML e-Financeira
curl -X POST "https://localhost:5001/api/e-financeira/generate" \
  -H "Content-Type: application/json" \
  -d '{
    "responsavel": {
      "cnpjRespons": "11222333000181",
      "nmRespons": "Empresa Exemplo Ltda",
      "email": "contato@empresa.com.br"
    },
    "movimentacoes": [
      {
        "ideConta": {
          "cnpjInstituicao": "60746948000112",
          "numeroConta": "123456",
          "tpConta": 1
        },
        "iniMovFin": {
          "dtIni": "2024-01-01T00:00:00",
          "dtFim": "2024-01-31T23:59:59"
        },
        "movimentacoes": [
          {
            "dtMov": "2024-01-15T10:30:00",
            "tpMov": 1,
            "valor": 1500.00
          }
        ]
      }
    ]
  }'
```

## Exemplo com PowerShell

```powershell
$body = @{
    responsavel = @{
        cnpjRespons = "11222333000181"
        nmRespons = "Empresa Exemplo Ltda"
        email = "contato@empresa.com.br"
    }
    movimentacoes = @(
        @{
            ideConta = @{
                cnpjInstituicao = "60746948000112"
                numeroConta = "123456"
                tpConta = 1
            }
            iniMovFin = @{
                dtIni = "2024-01-01T00:00:00"
                dtFim = "2024-01-31T23:59:59"
            }
            movimentacoes = @(
                @{
                    dtMov = "2024-01-15T10:30:00"
                    tpMov = 1
                    valor = 1500.00
                }
            )
        }
    )
} | ConvertTo-Json -Depth 10

Invoke-RestMethod -Uri "https://localhost:5001/api/e-financeira/generate" `
    -Method POST `
    -Body $body `
    -ContentType "application/json"
```

## Exemplo com C# HttpClient

```csharp
using System.Text;
using System.Text.Json;

var client = new HttpClient();
var request = new
{
    responsavel = new
    {
        cnpjRespons = "11222333000181",
        nmRespons = "Empresa Exemplo Ltda",
        email = "contato@empresa.com.br"
    },
    movimentacoes = new[]
    {
        new
        {
            ideConta = new
            {
                cnpjInstituicao = "60746948000112",
                numeroConta = "123456",
                tpConta = 1
            },
            iniMovFin = new
            {
                dtIni = "2024-01-01T00:00:00",
                dtFim = "2024-01-31T23:59:59"
            },
            movimentacoes = new[]
            {
                new
                {
                    dtMov = "2024-01-15T10:30:00",
                    tpMov = 1,
                    valor = 1500.00
                }
            }
        }
    }
};

var json = JsonSerializer.Serialize(request);
var content = new StringContent(json, Encoding.UTF8, "application/json");

var response = await client.PostAsync(
    "https://localhost:5001/api/e-financeira/generate", 
    content);

var result = await response.Content.ReadAsStringAsync();
Console.WriteLine(result);
```

## Postman Collection

Importe a seguinte collection no Postman para testar os endpoints:

```json
{
  "info": {
    "name": "EFinanceira API",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "item": [
    {
      "name": "Generate XML",
      "request": {
        "method": "POST",
        "header": [
          {
            "key": "Content-Type",
            "value": "application/json"
          }
        ],
        "body": {
          "mode": "raw",
          "raw": "{\n  \"responsavel\": {\n    \"cnpjRespons\": \"11222333000181\",\n    \"nmRespons\": \"Empresa Exemplo Ltda\",\n    \"email\": \"contato@empresa.com.br\"\n  },\n  \"movimentacoes\": [\n    {\n      \"ideConta\": {\n        \"cnpjInstituicao\": \"60746948000112\",\n        \"numeroConta\": \"123456\",\n        \"tpConta\": 1\n      },\n      \"iniMovFin\": {\n        \"dtIni\": \"2024-01-01T00:00:00\",\n        \"dtFim\": \"2024-01-31T23:59:59\"\n      },\n      \"movimentacoes\": [\n        {\n          \"dtMov\": \"2024-01-15T10:30:00\",\n          \"tpMov\": 1,\n          \"valor\": 1500.00\n        }\n      ]\n    }\n  ]\n}"
        },
        "url": {
          "raw": "{{baseUrl}}/api/e-financeira/generate",
          "host": ["{{baseUrl}}"],
          "path": ["api", "e-financeira", "generate"]
        }
      }
    }
  ],
  "variable": [
    {
      "key": "baseUrl",
      "value": "https://localhost:5001"
    }
  ]
}
```
# RetornoLoteEventos v1.3.0 Builder

Este builder permite a criação e manipulação de mensagens de retorno de eventos e-Financeira na versão 1.3.0 do esquema XSD.

## Características da Versão v1.3.0

A versão v1.3.0 do esquema retornoEvento introduz mudanças significativas em relação à v1.2.0:

### Principais Diferenças

- **Estrutura Individual**: Foca em eventos individuais (`retornoEvento`) vs. lotes de eventos (`retornoLoteEventos`)
- **Namespace**: `http://www.eFinanceira.gov.br/schemas/retornoEvento/v1_3_0`
- **Dados de Recepção**: Novo objeto `dadosRecepcaoEvento` com informações detalhadas
- **Dados de Entrega**: Novo objeto `dadosReciboEntrega` para controle de entrega
- **Identificação**: Objeto específico `identificacaoEmpresaDeclarante`
- **Status**: Usa `cdRetorno` ao invés de `cdStatus`

### Estrutura do Schema v1.3.0

```xml
<eFinanceira xmlns="http://www.eFinanceira.gov.br/schemas/retornoEvento/v1_3_0">
    <retornoEvento id="...">
        <identificacaoEmpresaDeclarante>
            <cnpjEmpresaDeclarante>...</cnpjEmpresaDeclarante>
        </identificacaoEmpresaDeclarante>
        <dadosRecepcaoEvento>
            <dhRecepcao>...</dhRecepcao>
            <dhProcessamento>...</dhProcessamento>
            <tipoEvento>...</tipoEvento>
            <idEvento>...</idEvento>
            <hash>...</hash>
            <nrRecibo>...</nrRecibo>
        </dadosRecepcaoEvento>
        <dadosReciboEntrega>
            <nrRecibo>...</nrRecibo>
            <dhEntrega>...</dhEntrega>
        </dadosReciboEntrega>
        <status>
            <cdRetorno>...</cdRetorno>
            <descRetorno>...</descRetorno>
            <dadosRegistroOcorrenciaEvento>
                <tipo>...</tipo>
                <codigo>...</codigo>
                <descricao>...</descricao>
                <localizacaoErroAviso>...</localizacaoErroAviso>
            </dadosRegistroOcorrenciaEvento>
        </status>
    </retornoEvento>
    <Signature>...</Signature>
</eFinanceira>
```

## Uso Básico

### Criação de Retorno de Sucesso

```csharp
var factory = new EFinanceiraMessageFactory();

var retorno = factory
    .CriarRetornoLoteEventos_v1_3_0()
    .ComEmpresaDeclarante("12345678000195")
    .ComDadosRecepcao(
        dhRecepcao: DateTime.Now.AddMinutes(-5),
        dhProcessamento: DateTime.Now,
        tipoEvento: "F200",
        idEvento: "EVT001",
        hash: "ABC123DEF456",
        nrRecibo: "REC001"
    )
    .ComSucesso("Evento processado com sucesso")
    .ComId("EVT001")
    .Build();
```

### Criação de Retorno com Erros

```csharp
var retorno = factory
    .CriarRetornoLoteEventos_v1_3_0()
    .ComEmpresaDeclarante("98765432000198")
    .ComDadosRecepcao(
        dhRecepcao: DateTime.Now.AddMinutes(-10),
        dhProcessamento: DateTime.Now,
        tipoEvento: "F200",
        idEvento: "EVT002",
        hash: "XYZ789ABC123",
        nrRecibo: "REC002"
    )
    .ComErroProcessamento("001", "Erro na validação dos dados")
    .ComErro("E001", "CNPJ inválido", "identificacaoEmpresaDeclarante/cnpjEmpresaDeclarante")
    .ComAviso("W001", "Campo opcional não informado", "observacoes")
    .ComId("EVT002")
    .Build();
```

### Parsing de XML Existente

```csharp
var xmlRetorno = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<eFinanceira xmlns=""http://www.eFinanceira.gov.br/schemas/retornoEvento/v1_3_0"">
    <retornoEvento id=""EVT004"">
        <identificacaoEmpresaDeclarante>
            <cnpjEmpresaDeclarante>12345678000195</cnpjEmpresaDeclarante>
        </identificacaoEmpresaDeclarante>
        <status>
            <cdRetorno>0</cdRetorno>
            <descRetorno>Processado com sucesso</descRetorno>
        </status>
    </retornoEvento>
</eFinanceira>";

var retornoBuilder = factory.ParseRetornoLoteEventos_v1_3_0(xmlRetorno);
var retorno = retornoBuilder.Build();
```

## Métodos do Builder

### Métodos Principais

| Método | Descrição |
|--------|-----------|
| `ComEmpresaDeclarante(cnpj)` | Define o CNPJ da empresa declarante |
| `ComDadosRecepcao(...)` | Define dados completos de recepção do evento |
| `ComReciboEntrega(nrRecibo, dhEntrega?)` | Define dados do recibo de entrega |
| `ComStatus(codigo, descricao?)` | Define status customizado |
| `ComId(id)` | Define ID do evento |
| `ComAssinatura(signature)` | Define assinatura digital |

### Métodos de Ocorrências

| Método | Descrição |
|--------|-----------|
| `AdicionarOcorrencia(tipo, codigo, descricao, localizacao?)` | Adiciona ocorrência genérica |
| `AdicionarOcorrencias(lista)` | Adiciona múltiplas ocorrências |
| `LimparOcorrencias()` | Remove todas as ocorrências |

### Métodos de Extensão (Fluent)

| Método | Descrição |
|--------|-----------|
| `ComSucesso(descricao?)` | Define status de sucesso (código "0") |
| `ComErroProcessamento(codigo, descricao)` | Define status de erro |
| `ComErro(codigo, descricao, localizacao?)` | Adiciona erro (tipo "1") |
| `ComAviso(codigo, descricao, localizacao?)` | Adiciona aviso (tipo "2") |

### Métodos de Consulta

| Método | Descrição |
|--------|-----------|
| `IsSuccessful()` | Verifica se o processamento foi bem-sucedido |
| `GetErros()` | Obtém todas as ocorrências de erro |
| `GetAvisos()` | Obtém todas as ocorrências de aviso |
| `GetDadosRecepcao()` | Obtém dados de recepção do evento |
| `GetReciboEntrega()` | Obtém dados do recibo de entrega |
| `GetCnpjEmpresaDeclarante()` | Obtém CNPJ da empresa declarante |

## Métodos Estáticos

### Criação a partir de Fontes Externas

```csharp
// A partir de XML
var builder = RetornoLoteEventos_v1_3_0_Builder.FromXml(xmlContent, version);

// A partir de objeto eFinanceira
var builder = RetornoLoteEventos_v1_3_0_Builder.FromEFinanceira(eFinanceira, version);
```

## Estrutura das Classes Principais

### TIdeEmpresaDeclarante
- `cnpjEmpresaDeclarante`: CNPJ da empresa declarante

### TDadosRecepcaoEvento
- `dhRecepcao`: Data/hora de recepção
- `dhProcessamento`: Data/hora de processamento
- `tipoEvento`: Tipo do evento (ex: F200, F500)
- `idEvento`: ID único do evento
- `hash`: Hash/digest do evento
- `nrRecibo`: Número do recibo

### TDadosReciboEntrega
- `numeroRecibo`: Número do recibo de entrega

### TStatus
- `cdRetorno`: Código de retorno ("0" = sucesso)
- `descRetorno`: Descrição do retorno
- `dadosRegistroOcorrenciaEvento`: Array de ocorrências

### TRegistroOcorrenciasOcorrencias
- `tipo`: Tipo da ocorrência ("1" = Erro, "2" = Aviso)
- `codigo`: Código da ocorrência
- `descricao`: Descrição da ocorrência
- `localizacaoErroAviso`: Localização do erro/aviso (opcional)

## Exemplo Completo

```csharp
var factory = new EFinanceiraMessageFactory();

var retorno = factory
    .CriarRetornoLoteEventos_v1_3_0()
    .ComEmpresaDeclarante("44444444000444")
    .ComDadosRecepcao(
        dhRecepcao: new DateTime(2024, 1, 15, 10, 0, 0),
        dhProcessamento: new DateTime(2024, 1, 15, 10, 5, 0),
        tipoEvento: "F200",
        idEvento: "EVT007",
        hash: "SHA256HASH123456789ABCDEF",
        nrRecibo: "REC007"
    )
    .ComReciboEntrega("REC007", new DateTime(2024, 1, 15, 10, 10, 0))
    .ComSucesso("Evento processado e entregue com sucesso")
    .ComAviso("W001", "Processamento executado fora do horário comercial")
    .ComId("EVT007")
    .Build();

// Verificações
Console.WriteLine($"Sucesso: {retorno.IsSuccessful()}");
Console.WriteLine($"CNPJ: {retorno.GetCnpjEmpresaDeclarante()}");
Console.WriteLine($"Tipo Evento: {retorno.GetDadosRecepcao()?.tipoEvento}");
Console.WriteLine($"Recibo: {retorno.GetReciboEntrega()?.nrRecibo}");
Console.WriteLine($"Avisos: {retorno.GetAvisos().Count()}");
```

## Comparação com v1.2.0

| Aspecto | v1.2.0 | v1.3.0 |
|---------|--------|--------|
| Foco | Lotes de eventos | Eventos individuais |
| Elemento raiz | `retornoLoteEventos` | `retornoEvento` |
| Identificação | `ideTransmissor.IdTransmissor` | `identificacaoEmpresaDeclarante.cnpjEmpresaDeclarante` |
| Status | `status.cdStatus` | `status.cdRetorno` |
| Dados específicos | - | `dadosRecepcaoEvento`, `dadosReciboEntrega` |
| Namespace | `retornoLoteEventos/v1_2_0` | `retornoEvento/v1_3_0` |

## Considerações de Versionamento

- Use `v1_3_0` para novos desenvolvimentos que precisem das funcionalidades específicas de eventos individuais
- `v1_2_0` continua adequado para processamento de lotes
- Os builders são independentes e podem coexistir na mesma aplicação
- Métodos de factory separados garantem isolamento entre versões
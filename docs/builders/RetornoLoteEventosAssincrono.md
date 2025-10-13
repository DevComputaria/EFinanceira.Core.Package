# RetornoLoteEventosAssincrono Builder

## Visão Geral

O `RetornoLoteEventosAssincronoBuilder` é um builder especializado para criar mensagens de retorno de lotes processados de forma assíncrona no sistema e-Financeira. Este builder oferece uma interface fluente para construir, validar e serializar mensagens de retorno conforme o schema XSD v1.0.0.

## Características Principais

### Processamento Assíncrono
- **Protocolo de Controle**: Rastreamento via `protocoloEnvio` para correlação com envios
- **Timestamps Separados**: `dhRecepcao` (recepção) e `dhProcessamento` (processamento)
- **Status com Códigos Inteiros**: `cdResposta` (int) ao invés de string como nas versões síncronas

### Flexibilidade de Eventos
- **Eventos XML Genéricos**: Suporte a `XmlElement` para qualquer tipo de evento
- **Identificação Única**: Cada evento possui um `id` para rastreamento
- **Validação Automática**: Conversão segura de strings XML para elementos

### Interface Fluente
- **Method Chaining**: Todos os métodos retornam o builder para encadeamento
- **Validação Integrada**: Verificações automáticas de parâmetros obrigatórios
- **Métodos Auxiliares**: Shortcuts para cenários comuns (sucesso, erro, aviso)

## Uso Básico

### Criação de Retorno de Sucesso

```csharp
var retorno = new RetornoLoteEventosAssincronoBuilder()
    .ComCnpjDeclarante("12345678000195")
    .ComDadosRecepcao(
        DateTime.Now.AddHours(-1),
        "1.0.0",
        "PROT2024001"
    )
    .ComDadosProcessamento(
        DateTime.Now,
        "2.0.0"
    )
    .ComSucesso("Lote processado com sucesso")
    .ComId("LOTE_RET_001")
    .Build();
```

### Criação de Retorno com Erros

```csharp
var retornoComErro = new RetornoLoteEventosAssincronoBuilder()
    .ComCnpjDeclarante("12345678000195")
    .ComDadosRecepcao(DateTime.Now.AddHours(-2), "1.0.0", "PROT2024002")
    .ComDadosProcessamento(DateTime.Now, "2.0.0")
    .ComErroProcessamento(1, "Erro na validação do lote")
    .ComErro("E001", "CNPJ inválido", "cnpjDeclarante")
    .ComErro("E002", "Evento duplicado", "evento[2]")
    .ComAviso("W001", "Campo opcional não preenchido", "telefone")
    .Build();
```

### Adição de Eventos XML

```csharp
var retornoComEventos = new RetornoLoteEventosAssincronoBuilder()
    .ComCnpjDeclarante("12345678000195")
    .ComDadosRecepcao(DateTime.Now.AddHours(-1), "1.0.0", "PROT2024003")
    .ComSucesso("Processado")
    .AdicionarEvento(
        @"<evtMovimentacaoFinanceira>
            <ideEvento>
                <indRetificacao>1</indRetificacao>
                <tpAmb>2</tpAmb>
            </ideEvento>
          </evtMovimentacaoFinanceira>",
        "EVT_001"
    )
    .AdicionarEvento(xmlElement, "EVT_002")
    .Build();
```

## Métodos Principais

### Configuração Básica

#### `ComCnpjDeclarante(string cnpj)`
Define o CNPJ do declarante responsável pelo lote.

#### `ComId(string id)`
Define o identificador único do lote de retorno.

#### `ComAssinatura(SignatureType signature)`
Adiciona assinatura digital à mensagem.

### Dados de Processamento

#### `ComDadosRecepcao(DateTime dhRecepcao, string versaoApp, string protocolo)`
Define os dados de recepção do lote:
- **dhRecepcao**: Data/hora de recepção
- **versaoApp**: Versão da aplicação que recebeu
- **protocolo**: Protocolo único para rastreamento

#### `ComDadosProcessamento(DateTime dhProcessamento, string versaoApp)`
Define os dados de processamento do lote:
- **dhProcessamento**: Data/hora de processamento
- **versaoApp**: Versão da aplicação que processou

### Configuração de Status

#### `ComStatus(int codigo, string descricao)`
Define o status personalizado do processamento.

#### `ComSucesso(string descricao = "Processado com sucesso")`
Configura status de sucesso (código 0).

#### `ComErroProcessamento(int codigoErro, string descricao)`
Define status de erro com código específico.

### Gestão de Ocorrências

#### `AdicionarOcorrencia(string codigo, string descricao, byte tipo, string? localizacao = null)`
Adiciona ocorrência personalizada:
- **tipo**: 1 = Erro, 2 = Aviso, 3 = Informação

#### `ComErro(string codigo, string descricao, string? localizacao = null)`
Adiciona erro (tipo = 1).

#### `ComAviso(string codigo, string descricao, string? localizacao = null)`
Adiciona aviso (tipo = 2).

#### `AdicionarOcorrencias(IEnumerable<TOcorrenciasOcorrencia> ocorrencias)`
Adiciona múltiplas ocorrências de uma vez.

#### `LimparOcorrencias()`
Remove todas as ocorrências.

### Gestão de Eventos

#### `AdicionarEvento(XmlElement elemento, string id)`
Adiciona evento a partir de XmlElement.

#### `AdicionarEvento(string xmlString, string id)`
Adiciona evento a partir de string XML (com conversão automática).

#### `LimparEventos()`
Remove todos os eventos do retorno.

### Métodos de Parse

#### `FromXml(string xmlContent)`
Cria builder a partir de XML de retorno existente.

#### `FromEFinanceira(eFinanceira eFinanceira)`
Cria builder a partir de objeto eFinanceira existente.

## Propriedades da Mensagem

### Informações Básicas
- **Version**: "v1_0_0"
- **RootElementName**: "eFinanceira"
- **IdAttributeName**: "id"

### Métodos de Consulta

#### `GetCnpjDeclarante()`
Retorna o CNPJ do declarante.

#### `GetProtocoloEnvio()`
Retorna o protocolo de envio para rastreamento.

#### `GetDadosRecepcao()`
Retorna dados completos de recepção.

#### `GetDadosProcessamento()`
Retorna dados completos de processamento.

#### `IsSuccessful()`
Verifica se o processamento foi bem-sucedido (código = 0).

#### `GetErros()`
Retorna apenas as ocorrências de erro (tipo = 1).

#### `GetAvisos()`
Retorna apenas as ocorrências de aviso (tipo = 2).

#### `GetEventos()`
Retorna todos os eventos processados.

## Diferenças das Versões Síncronas

### Estrutura de Dados
1. **Status**: Usa `cdResposta` (int) ao invés de `cdStatus` (string)
2. **Protocolo**: Campo `protocoloEnvio` para correlação assíncrona
3. **Timestamps**: Separação entre recepção e processamento
4. **Eventos**: Suporte genérico via `XmlElement` ao invés de tipos específicos

### Fluxo de Processamento
1. **Recepção**: Lote é recebido e validado inicialmente
2. **Protocolo**: Gerado identificador único para rastreamento
3. **Processamento**: Lote é processado assincronamente
4. **Retorno**: Mensagem com resultados e eventos processados

## Exemplos Avançados

### Workflow Completo
```csharp
var workflow = new RetornoLoteEventosAssincronoBuilder()
    .ComCnpjDeclarante("12345678000195")
    .ComDadosRecepcao(
        new DateTime(2024, 1, 15, 10, 0, 0),
        "SRF-eFinanceira-v1.0.0",
        "PROT20240115001"
    )
    .ComDadosProcessamento(
        new DateTime(2024, 1, 15, 10, 30, 0),
        "SRF-ProcessadorLotes-v2.0.0"
    )
    .ComSucesso("Lote processado com sucesso")
    .AdicionarEvento(eventoXml1, "EVT_MOVFIN_001")
    .AdicionarEvento(eventoXml2, "EVT_ABERTURA_002")
    .ComAviso("W001", "Evento com campos opcionais vazios", "EVT_MOVFIN_001")
    .ComId("LOTE_RET_20240115_001")
    .Build();
```

### Parse de XML Existente
```csharp
var xmlRetorno = File.ReadAllText("retorno_lote.xml");
var builder = RetornoLoteEventosAssincronoBuilder.FromXml(xmlRetorno);

// Adicionar informações complementares
var retornoEnriquecido = builder
    .ComAviso("W100", "Processamento em ambiente de homologação")
    .Build();
```

## Integração com Factory

```csharp
// Via extensão da factory
var factory = new EFinanceiraMessageFactory();

var retorno1 = factory
    .CriarRetornoLoteEventosAssincrono()
    .ComCnpjDeclarante("12345678000195")
    .ComSucesso()
    .Build();

var retorno2 = factory
    .ParseRetornoLoteEventosAssincrono(xmlContent)
    .ComAviso("W001", "Informação adicional")
    .Build();
```

## Serialização XML

```csharp
var retorno = builder.Build();
var xml = retorno.ToXml();
var xmlFormatado = retorno.ToXml(formatOutput: true);
```

## Validação e Logs

O builder inclui validações automáticas:
- CNPJ obrigatório
- Versões de aplicativo obrigatórias
- Protocolo obrigatório nos dados de recepção
- XML válido para eventos

## Considerações de Performance

- **Lazy Loading**: Objetos criados apenas quando necessário
- **Reutilização**: Builder pode ser reutilizado após Build()
- **Memory Efficient**: Evita duplicação desnecessária de dados
- **XML Parsing**: Cache interno para elementos XML convertidos
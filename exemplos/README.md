# Exemplos de Arquivos do e-Financeira

Este diretório contém todos os exemplos oficiais de arquivos do sistema e-Financeira da Receita Federal do Brasil.

## Origem
Arquivos baixados do site oficial: http://sped.rfb.gov.br/pasta/show/1846

## Estrutura dos Diretórios

### 📁 xml-sem-assinatura/ (13 arquivos)
Exemplos de arquivos XML sem assinatura digital, ideais para:
- Compreender a estrutura dos eventos
- Testes e desenvolvimento
- Validação de schema

#### Eventos de Ciclo de Vida
- `evtAberturaeFinanceira.xml` - Abertura de declaração e-Financeira
- `evtFechamentoeFinanceira.xml` - Fechamento de declaração
- `evtExclusaoeFinanceira.xml` - Exclusão de declaração
- `evtExclusao.xml` - Exclusão de evento

#### Eventos de Cadastro
- `evtCadEmpresaDeclarante.xml` - Cadastro da empresa declarante
- `evtCadEmpresaDeclarante_Retificacao.xml` - Retificação de cadastro
- `evtPatrocinado.xml` - Cadastro de patrocinado
- `evtIntermediario.xml` - Cadastro de intermediário

#### Eventos de Movimentação
- `evtMovOpFin.xml` - Movimentação de operações financeiras (mensal)
- `evtMovOpFinAnual.xml` - Movimentação de operações financeiras (anual)
- `evtPrevidenciaPrivada.xml` - Eventos de previdência privada

#### Eventos Especiais
- `evtRERCT.xml` - Evento RERCT (Regime Especial de Regularização Cambial e Tributária)

#### Arquivos de Retorno
- `ExemploArquivoRetorno.xml` - Exemplo de arquivo de retorno do sistema

### 📁 xml-com-assinatura/ (5 arquivos)
Exemplos de arquivos XML com assinatura digital e estrutura de lote, ideais para:
- Compreender o formato de produção
- Implementar assinatura digital
- Entender estrutura de lotes

#### Arquivos com Assinatura e Lote
- `evtCadEmpresaDeclarante_assinado_lote.xml` - Cadastro empresa declarante (assinado/lote)
- `evtPatrocinado_assinado_lote.xml` - Cadastro patrocinado (assinado/lote)
- `evtAberturaeFinanceira_assinado_lote.xml` - Abertura e-Financeira (assinado/lote)
- `evtMovOpFin_assinado_lote.xml` - Movimentação financeira (assinado/lote)
- `evtFechamentoeFinanceira_assinado_lote.xml` - Fechamento e-Financeira (assinado/lote)

### 📁 xml-gerados/ (36 arquivos)
XMLs gerados em tempo de execução pelo `EFinanceira.Console.Sample`, organizados por contexto, ideais para:
- Visualizar o formato real produzido pela biblioteca
- Validação de schema
- Testes de integração

#### Consultas (6 arquivos)
- `consulta_exemplo.xml` - Consulta genérica
- `consulta_intermediario_exemplo.xml` - Consulta de intermediário
- `consulta_lista_exemplo.xml` - Consulta de lista
- `consulta_movimento_exemplo.xml` - Consulta de movimento
- `consulta_patrocinado_exemplo.xml` - Consulta de patrocinado
- `consulta_rerct_exemplo.xml` - Consulta RERCT

#### Eventos (20 arquivos)
Eventos gerados pelas factories (`evento_*_factory.xml`) e pelos exemplos de uso (`evento_*_exemplo.xml`):
- `evento_abertura_exemplo.xml` / `evento_abertura_factory.xml` - Abertura e-Financeira
- `evento_cad_declarante_exemplo.xml` / `evento_cad_declarante_factory.xml` - Cadastro da empresa declarante
- `evento_exclusao_exemplo.xml` / `evento_exclusao_factory.xml` - Exclusão de evento
- `evento_exclusao_efinanceira_exemplo.xml` / `evento_exclusao_efinanceira_factory.xml` - Exclusão de declaração
- `evento_fechamento_alt_exemplo.xml` / `evento_fechamento_alt_factory.xml` - Fechamento alternativo
- `evento_fechamento_efinanceira_exemplo.xml` / `evento_fechamento_efinanceira_factory.xml` - Fechamento e-Financeira
- `evento_intermediario_exemplo.xml` / `evento_intermediario_factory.xml` - Cadastro de intermediário
- `evento_movimentacao_financeira_exemplo.xml` / `evento_movimentacao_financeira_factory.xml` - Movimentação financeira (mensal)
- `evento_movimentacao_financeira_anual_exemplo.xml` / `evento_movimentacao_financeira_anual_factory.xml` - Movimentação financeira (anual)
- `evento_patrocinado_exemplo.xml` - Cadastro de patrocinado
- `evento_previdencia_privada_exemplo.xml` - Previdência privada

#### Assinados (10 arquivos)
- `evento_assinado_*.xml` - Eventos com assinatura digital, gerados com certificado de teste

### 📁 codigo-fonte/ (2 arquivos)
Exemplos de código fonte e implementações, ideais para:
- Implementar assinatura digital
- Compreender criptografia de lotes
- Integração com sistemas

#### Código Fonte
- `ExemploAssinadorXML_256bytes.zip` - Código fonte para assinatura de eventos
- `ExemploCriptografiaLoteEFinanceira.zip` - Exemplo de criptografia de lotes
- `EvtRERCTExample.cs` - Exemplo de evento RERCT
- `xmldsig-exemplo-producao.cs` - Exemplo de assinatura em produção
- `Eventos/EvtPatrocinadoSample.cs` - Exemplo de evento de patrocinado
- `Eventos/EvtPrevidenciaPrivadaSample.cs` - Exemplo de evento de previdência privada
- `Lotes/` - Exemplos de envio e retorno de lotes de eventos (síncrono, assíncrono e v1.3.0)

## Características dos Arquivos

### Arquivos XML sem Assinatura
- **Formato**: XML puro, sem assinatura digital
- **Estrutura**: Evento individual
- **Uso**: Desenvolvimento, testes, compreensão da estrutura
- **Validação**: Podem ser validados contra os XSD schemas

### Arquivos XML com Assinatura
- **Formato**: XML com assinatura digital W3C
- **Estrutura**: Lote contendo eventos assinados
- **Uso**: Produção, envio real ao sistema
- **Validação**: Incluem assinatura digital e estrutura de lote

## Tamanhos dos Arquivos

### Pequenos (< 1 KB)
- `evtExclusao.xml` (525 bytes)
- `evtExclusaoeFinanceira.xml` (579 bytes)
- `evtCadEmpresaDeclarante.xml` (815 bytes)
- `evtFechamentoeFinanceira.xml` (827 bytes)
- `evtCadEmpresaDeclarante_Retificacao.xml` (868 bytes)

### Médios (1-5 KB)
- `evtPatrocinado.xml` (1.2 KB)
- `evtIntermediario.xml` (1.3 KB)
- `evtRERCT.xml` (1.9 KB)
- `evtPrevidenciaPrivada.xml` (2.6 KB)
- `evtAberturaeFinanceira.xml` (2.9 KB)
- `evtMovOpFinAnual.xml` (4.9 KB)
- `evtMovOpFin.xml` (5.4 KB)

### Grandes (5+ KB)
- `evtAberturaeFinanceira_assinado_lote.xml` (7.5 KB)
- `ExemploArquivoRetorno.xml` (9.8 KB)
- `evtMovOpFin_assinado_lote.xml` (10.5 KB)

### Arquivos ZIP
- `ExemploAssinadorXML_256bytes.zip` (14.2 KB)
- `ExemploCriptografiaLoteEFinanceira.zip` (41.3 KB)

## Como Usar os Exemplos

### 1. Para Desenvolvimento
```csharp
// Carregar e analisar um exemplo
var xmlContent = File.ReadAllText("exemplos/xml-sem-assinatura/evtAberturaeFinanceira.xml");
var documento = XDocument.Parse(xmlContent);

// Validar contra schema
var schemas = new XmlSchemaSet();
schemas.Add(null, "schemas/evtAberturaeFinanceira-v1_2_1.xsd");
// ... código de validação
```

### 2. Para Testes Unitários
```csharp
[Test]
public void DeveCarregarExemploAbertura()
{
    var caminhoExemplo = Path.Combine("exemplos", "xml-sem-assinatura", "evtAberturaeFinanceira.xml");
    var evento = CarregarEventoAbertura(caminhoExemplo);
    
    Assert.IsNotNull(evento);
    Assert.AreEqual("1", evento.ideEvento.indRetificacao);
}
```

### 3. Para Compreender Estruturas
```xml
<!-- Exemplo de estrutura básica (evtAberturaeFinanceira.xml) -->
<evtAberturaeFinanceira xmlns="http://www.eFinanceira.gov.br/schemas/evtAberturaeFinanceira/v1_2_1">
  <ideEvento>
    <indRetificacao>1</indRetificacao>
    <tpAmb>2</tpAmb>
    <cnpjDeclarante>11111111000191</cnpjDeclarante>
    <!-- ... outros campos -->
  </ideEvento>
  <!-- ... resto da estrutura -->
</evtAberturaeFinanceira>
```

### 4. Para Implementar Assinatura
Use os arquivos ZIP em `codigo-fonte/` que contêm:
- Exemplos de código para assinatura digital
- Implementações de criptografia
- Utilitários para manipulação de lotes

## Diferenças entre Versões

### Sem Assinatura vs Com Assinatura

**Sem Assinatura:**
```xml
<evtAberturaeFinanceira xmlns="...">
  <ideEvento>...</ideEvento>
  <infoAbertura>...</infoAbertura>
</evtAberturaeFinanceira>
```

**Com Assinatura e Lote:**
```xml
<envioLoteEventos xmlns="...">
  <ideTransmissor>...</ideTransmissor>
  <loteEventos>
    <evento id="ID123456">
      <evtAberturaeFinanceira>
        <ideEvento>...</ideEvento>
        <infoAbertura>...</infoAbertura>
        <Signature xmlns="http://www.w3.org/2000/09/xmldsig#">
          <!-- Assinatura digital -->
        </Signature>
      </evtAberturaeFinanceira>
    </evento>
  </loteEventos>
</envioLoteEventos>
```

## Validação dos Exemplos

Todos os exemplos podem ser validados usando:

1. **XSD Schemas** (pasta `schemas/`)
2. **Validadores online** do Portal SPED
3. **Código de validação** dos exemplos em `codigo-fonte/`

## Casos de Uso por Exemplo

| Arquivo | Caso de Uso | Cenário |
|---------|-------------|---------|
| `evtAberturaeFinanceira.xml` | Abertura de declaração | Início de período declaratório |
| `evtCadEmpresaDeclarante.xml` | Cadastro inicial | Primeira declaração da empresa |
| `evtPatrocinado.xml` | Cadastro de titular | Pessoa física com conta no exterior |
| `evtMovOpFin.xml` | Movimentação mensal | Informar operações do mês |
| `evtFechamentoeFinanceira.xml` | Fechamento | Finalizar período declaratório |
| `evtExclusaoeFinanceira.xml` | Cancelamento | Cancelar declaração incorreta |

## Status do Download
- **Total de exemplos identificados**: 20
- **Arquivos baixados com sucesso**: 20
- **Arquivos com erro**: 0

## Data do Download
20 de setembro de 2025

## Links Úteis
- [Exemplos de arquivos e-Financeira](http://sped.rfb.gov.br/pasta/show/1846)
- [Schemas XSD](../schemas/)
- [Tabelas de códigos](../tabelas-codigos/)
- [Site oficial e-Financeira](http://sped.rfb.gov.br/projeto/show/1179)

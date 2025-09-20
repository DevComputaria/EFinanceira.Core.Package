# Schemas XSD do e-Financeira

Este diretório contém todos os arquivos XSD (XML Schema Definition) do sistema e-Financeira da Receita Federal do Brasil.

## Origem
Arquivos baixados do site oficial: http://sped.rfb.gov.br/pasta/show/1854

## Estrutura dos Arquivos

### Eventos (24 arquivos)
Schemas para os diferentes tipos de eventos do e-Financeira:

#### Eventos Principais
- `evtAberturaeFinanceira-v1_2_1.xsd` - Evento de Abertura (v1.2.1 - Produção: outubro/2023)
- `evtCadEmpresaDeclarante-v1_2_0.xsd` - Evento Cadastro da Empresa Declarante (v1.2.0 - Produção: janeiro/2018)
- `evtIntermediario-v1_2_0.xsd` - Evento de Intermediário (v1.2.0 - Produção: janeiro/2018)
- `evtPatrocinado-v1_2_0.xsd` - Evento de Patrocinado (v1.2.0 - Produção: janeiro/2018)
- `evtExclusao-v1_2_0.xsd` - Evento de Exclusão (v1.2.0 - Produção: janeiro/2018)
- `evtFechamentoeFinanceira-v1_2_2.xsd` - Evento de Fechamento (v1.2.2 - Produção: outubro/2023)
- `evtFechamentoeFinanceira-v1_2_2-alt.xsd` - Evento de Fechamento alternativo (v1.2.2 - Produção: 02/outubro/2023)
- `evtMovimentacaoFinanceira-v1_2_1.xsd` - Evento de Movimentação Financeira (v1.2.1 - Produção: outubro/2023)
- `evtMovimentacaoFinanceiraAnual-v1_2_2.xsd` - Evento de Movimentação Financeira Anual (v1.2.2 - Produção: outubro/2023)
- `evtExclusaoeFinanceira-v1_2_0.xsd` - Evento de Exclusão de e-Financeira (v1.2.0 - Produção: janeiro/2018)
- `evtRERCT-v1_2_0.xsd` - Evento RERCT (v1.2.0 - Produção: janeiro/2018)
- `evtPrevidenciaPrivada-v1_2_5.xsd` - Evento de Previdência Privada (v1.2.5 - Produção: 15/abril/2024)

#### Retornos de Consulta
- `retInfoCadastral-v1_2_0.xsd` - Retorno da consulta de informações cadastrais (v1.2.0 - Produção: janeiro/2018)
- `retInfoIntermediario-v1_2_0.xsd` - Retorno da consulta de informações de intermediário (v1.2.0 - Produção: janeiro/2018)
- `retInfoPatrocinado-v1_2_0.xsd` - Retorno da consulta de informações patrocinado (v1.2.0 - Produção: janeiro/2018)
- `retInfoMovimento-v1_2_0.xsd` - Retorno da consulta de informações movimento (v1.2.0 - Produção: janeiro/2018)
- `retListaeFinanceira-v1_2_0.xsd` - Retorno da consulta lista de e-Financeira (v1.2.0 - Produção: janeiro/2018)
- `retRERCT-v1_2_0.xsd` - Retorno da consulta do módulo específico RERCT (v1.2.0 - Produção: janeiro/2018)

#### Envio de Lotes
- `envioLoteEventosAssincrono-v1_0_0.xsd` - Envio Lote de Eventos Modo Assíncrono (v1.0.0 - Produção restrita: outubro/2024, Produção: janeiro/2025)
- `retornoLoteEventosAssincrono-v1_0_0.xsd` - Retorno Lote Eventos Modo Assíncrono (v1.0.0)
- `envioLoteEventos-v1_2_0.xsd` - Envio Lote Eventos Síncrono (v1.2.0)
- `retornoLoteEventos-v1_2_0.xsd` - Retorno Lote Eventos Síncrono (v1.2.0)
- `retornoLoteEventos-v1_3_0.xsd` - Retorno Lote Eventos (v1.3.0) - Independente do tipo de envio
- `envioLoteCriptografado-v1_2_0.xsd` - Envio Lote Criptografado (v1.2.0)

### Validação da Assinatura
- `xmldsig-core-schema.xsd` - Validação da Assinatura (Padrão W3C)

## Status do Download
- **Total de arquivos identificados**: 25
- **Arquivos baixados com sucesso**: 25
- **Arquivos com erro**: 0

## Uso
Estes arquivos XSD podem ser utilizados para:
- Validação de documentos XML do e-Financeira
- Geração de classes C# utilizando ferramentas como `xsd.exe` ou bibliotecas similares
- Desenvolvimento de aplicações que integram com o sistema e-Financeira
- Documentação técnica dos formatos de dados aceitos

## Versionamento
As versões dos schemas seguem o padrão semântico e são atualizadas conforme necessário pela Receita Federal. Sempre consulte o site oficial para verificar se há versões mais recentes disponíveis.

## Data do Download
20 de setembro de 2025

## Links Úteis
- [Site oficial e-Financeira](http://sped.rfb.gov.br/projeto/show/1179)
- [Schemas e-Financeira](http://sped.rfb.gov.br/pasta/show/1854)
- [Documentação oficial](http://sped.rfb.gov.br/item/show/1499)
- [Legislação](http://sped.rfb.gov.br/item/show/1501)
- [Perguntas Frequentes](http://sped.rfb.gov.br/item/show/1502)

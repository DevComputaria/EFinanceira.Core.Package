# Tabelas de Códigos do e-Financeira

Este diretório contém todas as tabelas de códigos oficiais do sistema e-Financeira da Receita Federal do Brasil.

## Origem
Arquivos baixados do site oficial: http://sped.rfb.gov.br/pasta/show/1932

## Estrutura dos Arquivos

### Tabelas Geográficas (3 arquivos)
- `Municipios.txt` - Lista completa de municípios brasileiros com códigos IBGE
- `Paises.txt` - Lista de países com códigos internacionais
- `Unidades_da_Federacao.txt` - Unidades da Federação (estados) do Brasil

### Categorias e Tipos de Declarante (5 arquivos)
- `Categorias_de_Declarante.txt` - Categorias de empresas declarantes
- `Categorias_de_Patrocinador.txt` - Categorias de patrocinadores
- `Tipos_de_Declarado.txt` - Tipos de pessoas/entidades declaradas
- `Tipo_de_Relacao_de_Declarado.txt` - Tipos de relacionamento com declarado
- `Tipo_de_Proprietario.txt` - Tipos de proprietários de contas

### Identificação e Nomenclatura (3 arquivos)
- `Tipos_de_Nome.txt` - Tipos de nomenclatura permitidos
- `Tipos_de_Endereco.txt` - Tipos de endereço aceitos
- `Tipos_de_NI.txt` - Tipos de Número de Identificação

### Contas e Movimentação Financeira (5 arquivos)
- `Tipos_de_Conta.txt` - Tipos de contas financeiras
- `Subtipos_de_Conta.txt` - Subtipos específicos de contas
- `Tipos_de_Numero_de_Conta.txt` - Tipos de numeração de contas
- `Tipo_de_Pagamento.txt` - Tipos de formas de pagamento
- `Moedas.txt` - Códigos de moedas internacionais (ISO 4217)

### Previdência Privada (3 arquivos)
- `Tipo_de_Plano_de_Previdencia_Privada.txt` - Tipos de planos de previdência
- `Tipo_de_Produto_de_Previdencia_Privada.txt` - Tipos de produtos previdenciários
- `Tipo_de_Empresa_de_Previdencia_Privada.txt` - Tipos de empresas de previdência

### Impostos e Retenções (1 arquivo)
- `Tabela_de_Codigo_de_Retencao_de_IR.txt` - Códigos de retenção de Imposto de Renda

### RERCT e Eventos (2 arquivos)
- `Identificacao_do_Evento_RERCT.txt` - Identificação de eventos RERCT
- `Tipo_de_Evento_do_Arquivo_de_Retorno.txt` - Tipos de eventos em arquivos de retorno

### Informações Internacionais (1 arquivo)
- `Informacao_de_NIF_por_pais.txt` - Informações sobre obrigatoriedade de NIF por país

## Formato dos Arquivos

Todos os arquivos estão no formato texto (TXT) com estrutura delimitada, geralmente contendo:
- Código
- Descrição
- Informações adicionais (quando aplicável)

## Status do Download
- **Total de tabelas identificadas**: 23
- **Arquivos baixados com sucesso**: 23
- **Arquivos com erro**: 0

## Como Usar

### 1. Importação em Banco de Dados
```sql
-- Exemplo para SQL Server
BULK INSERT TabelaMunicipios
FROM 'C:\caminho\para\Municipios.txt'
WITH (
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '\n',
    FIRSTROW = 2
);
```

### 2. Leitura em C#
```csharp
public class TabelaCodigosService
{
    public List<Municipio> CarregarMunicipios(string caminhoArquivo)
    {
        var municipios = new List<Municipio>();
        var linhas = File.ReadAllLines(caminhoArquivo);
        
        foreach (var linha in linhas.Skip(1)) // Pular cabeçalho
        {
            var campos = linha.Split('\t');
            municipios.Add(new Municipio
            {
                Codigo = campos[0],
                Nome = campos[1],
                UF = campos[2]
            });
        }
        
        return municipios;
    }
}
```

### 3. Validação de Códigos
```csharp
public class ValidadorCodigos
{
    private readonly HashSet<string> _codigosPaises;
    private readonly HashSet<string> _codigosMunicipios;
    
    public ValidadorCodigos()
    {
        _codigosPaises = CarregarCodigos("Paises.txt");
        _codigosMunicipios = CarregarCodigos("Municipios.txt");
    }
    
    public bool ValidarCodigoPais(string codigo)
    {
        return _codigosPaises.Contains(codigo);
    }
    
    public bool ValidarCodigoMunicipio(string codigo)
    {
        return _codigosMunicipios.Contains(codigo);
    }
}
```

## Arquivos por Tamanho

### Grandes (> 100KB)
- `Informacao_de_NIF_por_pais.txt` (117.8 KB) - Informações detalhadas por país
- `Municipios.txt` (172.0 KB) - Todos os municípios brasileiros

### Médios (1-10KB)
- `Paises.txt` (6.4 KB) - Lista de países
- `Moedas.txt` (5.0 KB) - Códigos de moedas
- `Tabela_de_Codigo_de_Retencao_de_IR.txt` (5.4 KB) - Códigos de retenção IR

### Pequenos (< 1KB)
- Todas as demais tabelas são compactas e específicas

## Atualizações

As tabelas são atualizadas periodicamente pela Receita Federal. Para verificar atualizações:

1. **Manual**: Execute novamente o script `download-tabelas-codigos.ps1`
2. **Portal Online**: Consulte o [Portal SPED Tabelas](http://www.sped.fazenda.gov.br/spedtabelas/AppConsulta/publico/aspx/ConsultaTabelasExternas.aspx?CodSistema=efdfinanceira)

## Data do Download
20 de setembro de 2025

## Links Úteis
- [Tabelas de códigos e-Financeira](http://sped.rfb.gov.br/pasta/show/1932)
- [Portal SPED Tabelas Online](http://www.sped.fazenda.gov.br/spedtabelas/AppConsulta/publico/aspx/ConsultaTabelasExternas.aspx?CodSistema=efdfinanceira)
- [Site oficial e-Financeira](http://sped.rfb.gov.br/projeto/show/1179)
- [Documentação oficial](http://sped.rfb.gov.br/item/show/1499)

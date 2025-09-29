# EvtRERCT Builder

O **EvtRERCTBuilder** é um builder completo para criação de eventos RERCT (Registro de Contas Exteriores e Transferências) no sistema e-Financeira. Este builder implementa o padrão fluent interface, facilitando a construção de eventos estruturados de forma intuitiva e validada.

## Características Principais

- ✅ **Interface Fluente**: Padrão builder com encadeamento natural de métodos
- ✅ **Validação Hierárquica**: Validação de campos obrigatórios em todos os níveis
- ✅ **Múltiplas Contas**: Suporte a múltiplas contas exteriores via `AddRERCTs()`
- ✅ **Múltiplos Titulares**: Gestão de contas com vários titulares
- ✅ **Múltiplos Beneficiários**: Suporte a beneficiários finais
- ✅ **Configuração Flexível**: Suporte a retificação, diferentes moedas e tipos de conta
- ✅ **Conformidade Internacional**: Códigos ISO de moeda e identificação fiscal

## Estrutura do Builder

### Builder Principal
- **EvtRERCTBuilder**: Builder principal com 650+ linhas de código

### Sub-Builders Especializados
1. **IdeEventoBuilder**: Identificação do evento
2. **IdeDeclaranteBuilder**: Dados do declarante
3. **IdeDeclaradoBuilder**: Informações do declarado
4. **CpfCnpjDeclaradoBuilder**: CPF/CNPJ do declarado
5. **RERCTBuilder**: Registro de conta exterior
6. **InfoContaExteriorBuilder**: Informações da conta
7. **TitularBuilder**: Dados do titular
8. **CpfCnpjTitularBuilder**: CPF/CNPJ do titular
9. **BeneficiarioFinalBuilder**: Beneficiário final

## Uso Básico

### Exemplo Mínimo

```csharp
var message = EvtRERCTBuilder
    .Create()
    .WithIdeEvento(ideEvento => ideEvento
        .WithIdeEventoRERCT(111222333))
    .WithIdeDeclarante(ideDeclarante => ideDeclarante
        .WithCnpjDeclarante("00000000000100"))
    .WithIdeDeclarado(ideDeclarado => ideDeclarado
        .WithCpfCnpjDeclarado(cpfCnpj => cpfCnpj
            .WithTipoInscricao(1) // 1-CPF
            .WithNumeroInscricao("00000000001")))
    .AddRERCT(rerct => rerct
        .WithNomeBancoOrigem("Minimal Bank")
        .WithPaisOrigem("XX")
        .AddInfoContaExterior(conta => conta
            .WithNumeroContaExterior("MIN123")
            .WithValorUltimoDia("1000.00")
            .WithMoeda("USD")
            .AddTitular(titular => titular
                .WithNomeTitular("Titular Mínimo")
                .WithCpfCnpjTitular(cpfCnpj => cpfCnpj
                    .WithTipoInscricao(1)
                    .WithNumeroInscricao("00000000001")))))
    .Build();
```

### Exemplo Completo

```csharp
var message = EvtRERCTBuilder
    .Create("v1_2_0")
    .WithId("ID_RERCT_2025_001")
    .WithIdeEvento(ideEvento => ideEvento
        .WithIdeEventoRERCT(123456789)
        .WithIndRetificacao(0) // 0-Original
        .WithAmbiente(2) // 2-Homologação
        .WithAplicativoEmi(1) // 1-Aplicativo do contribuinte
        .WithVersaoAplic("1.0.0"))
    .WithIdeDeclarante(ideDeclarante => ideDeclarante
        .WithCnpjDeclarante("12345678000123"))
    .WithIdeDeclarado(ideDeclarado => ideDeclarado
        .WithCpfCnpjDeclarado(cpfCnpj => cpfCnpj
            .WithTipoInscricao(1) // 1-CPF
            .WithNumeroInscricao("12345678901")))
    .AddRERCT(rerct => rerct
        .WithNomeBancoOrigem("Bank of International")
        .WithPaisOrigem("US")
        .WithBICBancoOrigem("BOFAUS3N")
        .AddInfoContaExterior(conta => conta
            .WithTipoContaExterior(1)
            .WithNumeroContaExterior("US123456789")
            .WithValorUltimoDia("150000.00")
            .WithMoeda("USD")
            .AddTitular(titular => titular
                .WithNomeTitular("João Silva")
                .WithCpfCnpjTitular(cpfCnpj => cpfCnpj
                    .WithTipoInscricao(1)
                    .WithNumeroInscricao("12345678901"))
                .WithNIFTitular("123456789US"))
            .AddBeneficiarioFinal(beneficiario => beneficiario
                .WithNomeBeneficiarioFinal("Maria Silva")
                .WithCpfBeneficiarioFinal("98765432100")
                .WithNIFBeneficiarioFinal("987654321US"))))
    .Build();
```

## Funcionalidades Avançadas

### Múltiplas Contas Exteriores

```csharp
.AddRERCTs(
    // Primeira conta - USD
    rerct => rerct
        .WithNomeBancoOrigem("Chase Bank")
        .WithPaisOrigem("US")
        .WithBICBancoOrigem("CHASUS33")
        .AddInfoContaExterior(conta => conta
            .WithTipoContaExterior(1)
            .WithNumeroContaExterior("US987654321")
            .WithValorUltimoDia("250000.00")
            .WithMoeda("USD")
            .AddTitular(titular => titular
                .WithNomeTitular("Empresa ABC Ltda")
                .WithCpfCnpjTitular(cpfCnpj => cpfCnpj
                    .WithTipoInscricao(2) // 2-CNPJ
                    .WithNumeroInscricao("98765432000198")))),
    
    // Segunda conta - EUR
    rerct => rerct
        .WithNomeBancoOrigem("Deutsche Bank")
        .WithPaisOrigem("DE")
        .WithBICBancoOrigem("DEUTDEFF")
        .AddInfoContaExterior(conta => conta
            .WithTipoContaExterior(2)
            .WithNumeroContaExterior("DE123456789012345678")
            .WithValorUltimoDia("180000.00")
            .WithMoeda("EUR")
            .AddTitular(titular => titular
                .WithNomeTitular("Empresa ABC Ltda")
                .WithCpfCnpjTitular(cpfCnpj => cpfCnpj
                    .WithTipoInscricao(2)
                    .WithNumeroInscricao("98765432000198"))
                .WithNIFTitular("DE123456789"))))
```

### Múltiplos Titulares e Beneficiários

```csharp
.AddInfoContaExterior(conta => conta
    .WithTipoContaExterior(3)
    .WithNumeroContaExterior("CH93000762011623852957")
    .WithValorUltimoDia("500000.00")
    .WithMoeda("CHF")
    
    // Múltiplos titulares
    .AddTitulares(
        titular => titular
            .WithNomeTitular("João Silva")
            .WithCpfCnpjTitular(cpfCnpj => cpfCnpj
                .WithTipoInscricao(1)
                .WithNumeroInscricao("11223344556"))
            .WithNIFTitular("CH123456789"),
        titular => titular
            .WithNomeTitular("Maria Silva")
            .WithCpfCnpjTitular(cpfCnpj => cpfCnpj
                .WithTipoInscricao(1)
                .WithNumeroInscricao("66778899001"))
            .WithNIFTitular("CH987654321"))
    
    // Múltiplos beneficiários finais
    .AddBeneficiariosFinais(
        beneficiario => beneficiario
            .WithNomeBeneficiarioFinal("Pedro Silva")
            .WithCpfBeneficiarioFinal("12312312300")
            .WithNIFBeneficiarioFinal("CH111222333"),
        beneficiario => beneficiario
            .WithNomeBeneficiarioFinal("Ana Silva")
            .WithCpfBeneficiarioFinal("45645645600")
            .WithNIFBeneficiarioFinal("CH444555666")))
```

### Retificação

```csharp
.WithIdeEvento(ideEvento => ideEvento
    .WithIdeEventoRERCT(123456789)
    .WithIndRetificacao(1) // 1-Retificação
    .WithNrRecibo("REC123456789") // Obrigatório para retificação
    .WithAmbiente(1)) // 1-Produção
```

## Validações

O builder implementa validações automáticas para:

- ✅ **IdeEvento** é obrigatório
- ✅ **IdeDeclarante** é obrigatório  
- ✅ **IdeDeclarado** é obrigatório
- ✅ **Pelo menos um registro RERCT** é obrigatório
- ✅ **Número do recibo** obrigatório quando indRetificacao = 1

## Códigos de Configuração

### Tipo de Inscrição
- `1` - CPF
- `2` - CNPJ

### Indicador de Retificação
- `0` - Original
- `1` - Retificação

### Tipo de Ambiente
- `1` - Produção
- `2` - Homologação

### Aplicativo Emissor
- `1` - Aplicativo do contribuinte

## Factory Integration

O EvtRERCT está registrado no EFinanceiraMessageFactory:

```csharp
factory.RegisterFactory(
    MessageKind.Evento("EvtRERCT"),
    "v1_2_0",
    (Action<object>? seed) =>
    {
        var builder = new EvtRERCTBuilder("v1_2_0");
        
        if (seed is Action<EvtRERCTBuilder> configure)
        {
            configure(builder);
        }
        
        return builder.Build();
    });
```

## Testes

O builder possui cobertura completa de testes unitários:

- ✅ Criação básica do builder
- ✅ Configuração mínima válida
- ✅ Configuração completa
- ✅ Validação de campos obrigatórios
- ✅ Múltiplos registros RERCT
- ✅ Estrutura de dados complexa

Execute os testes com:

```bash
dotnet test --filter "EvtRERCTBuilderTests"
```

## Arquivos Relacionados

- **Builder**: `src/EFinanceira.Messages/Builders/Eventos/EvtRERCT/EvtRERCTBuilder.cs`
- **Exemplos**: `src/EFinanceira.Messages/Examples/EvtRERCTExample.cs`
- **Testes**: `tests/EFinanceira.Tests/Messages/Builders/Eventos/EvtRERCT/EvtRERCTBuilderTests.cs`
- **Factory**: `src/EFinanceira.Messages/Factory/MessagesFactoryExtensions.cs`
- **Modelo XSD**: `src/EFinanceira.Messages/Generated/Eventos/EvtRERCT/evtRERCT-v1_2_0.cs`

## Versão

- **Versão do Schema**: v1_2_0
- **Namespace**: `EFinanceira.Messages.Builders.Eventos.EvtRERCT`
- **Implementado em**: 2025-09-28
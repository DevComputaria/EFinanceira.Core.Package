# EFinanceira.Core.Package

## Biblioteca .NET para integração com a e-Financeira (Receita Federal)

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Build](https://img.shields.io/badge/Build-Passing-brightgreen.svg)](#)

### 🚀 Características

- **Modular**: Arquitetura separada em Core, Messages e Tools
- **Type-Safe**: Builders fluentes para criação de mensagens
- **Validação**: Validação automática contra schemas XSD
- **Assinatura Digital**: Suporte a certificados A1 e A3
- **Code Generation**: Ferramentas CLI para geração de POCOs
- **Logging**: Integração completa com Microsoft.Extensions.Logging

### 📦 Pacotes

| Pacote | Descrição | Status |
|--------|-----------|---------|
| `EFinanceira.Core` | Infraestrutura base (serialização, validação, assinatura) | ✅ |
| `EFinanceira.Messages` | Builders e POCOs para mensagens | ✅ |
| `EFinanceira.Tools.CodeGen` | CLI para geração de código | ✅ |

### 🛠️ Instalação

```bash
# Instalar pacote principal
dotnet add package EFinanceira.Core

# Instalar builders de mensagens
dotnet add package EFinanceira.Messages

# Instalar ferramenta de geração de código (global)
dotnet tool install --global EFinanceira.Tools.CodeGen
```

### 📖 Uso Rápido

#### 1. Criar Evento de Movimentação Financeira

```csharp
using EFinanceira.Messages.Builders.Eventos;

var evento = new LeiauteMovimentacaoFinanceiraBuilder()
    .WithId("EVT20241219001")
    .WithDeclarante("12345678000199")
    .WithAmbiente(2) // Homologação
    .WithDeclarado(declarado =>
    {
        declarado
            .WithTipoNI(2) // CNPJ
            .WithNumeroInscricao("98765432000188")
            .WithNome("Cliente Exemplo S.A.")
            .WithEndereco(endereco =>
            {
                endereco
                    .WithPais("076") // Brasil
                    .WithEndereco("Rua Exemplo, 123")
                    .WithCidade("São Paulo");
            });
    })
    .WithMovimentacao(mov =>
    {
        mov
            .WithDataInicio(new DateTime(2024, 1, 1))
            .WithDataFim(new DateTime(2024, 12, 31))
            .AdicionarReportavel(rep =>
            {
                rep
                    .WithTipoReportavel(1)
                    .WithInformacoesConta(conta =>
                    {
                        conta
                            .WithTipoConta(1)
                            .WithSubTipoConta("001")
                            .WithMoeda("986"); // Real
                    });
            });
    })
    .Build();
```

#### 2. Criar Lote de Eventos

```csharp
using EFinanceira.Messages.Builders.Lotes;

var lote = new EnvioLoteEventosV120Builder()
    .WithId("LOTE20241219001")
    .ComTransmissor("12345678000199", "Empresa Demo", "contato@empresa.com.br")
    .AdicionarEvento(evento1)
    .AdicionarEvento(evento2)
    .Build();
```

#### 3. Serializar, Validar e Assinar

```csharp
using EFinanceira.Core.Serialization;
using EFinanceira.Core.Validation;
using EFinanceira.Core.Signing;

// Serializar
var serializer = new XmlNetSerializer();
var xml = serializer.Serialize(evento.Payload);

// Validar (opcional)
var validator = new XmlValidator();
var errors = validator.ValidateAndGetErrors(xml, xsdPaths);

// Assinar
var signer = new XmlSigner();
var signOptions = new SignOptions
{
    PfxPath = "certificado.pfx",
    PfxPassword = "senha",
    ElementToSignName = evento.RootElementName,
    IdValue = evento.IdValue
};

var signedXml = signer.Sign(xml, signOptions);
```

### 🔧 Configuração com Dependency Injection

```csharp
// Program.cs / Startup.cs
services.AddSingleton<IXmlSerializer, XmlNetSerializer>();
services.AddSingleton<IXmlValidator, XmlValidator>();
services.AddSingleton<IXmlSigner, XmlSigner>();
services.AddSingleton<IMessageFactory, EFinanceiraMessageFactory>();
```

### 🛠️ Ferramentas de Desenvolvimento

#### Geração de POCOs a partir de XSD

```bash
# Usando XmlSchemaClassGenerator (recomendado)
efinanceira-codegen xsc generate 
  --input "./schemas/evtMovimentacaoFinanceira-v1_2_1.xsd" 
  --output "./Generated/Eventos" 
  --namespace "EFinanceira.Messages.Generated.Eventos.V121"

# Usando xsd.exe (Windows)
efinanceira-codegen xsd generate 
  --input "./schemas/evtMovimentacaoFinanceira-v1_2_1.xsd" 
  --output "./Generated/Eventos" 
  --namespace "EFinanceira.Messages.Generated.Eventos.V121"
```

#### Scripts de Build

```bash
# Windows PowerShell
./scripts/build.ps1

# Linux/macOS
./scripts/build.sh
```

### 📁 Estrutura do Projeto

```
efinanceira/
├── src/
│   ├── EFinanceira.Core/           # Infraestrutura base
│   │   ├── Abstractions/           # Interfaces e contratos
│   │   ├── Serialization/          # Serialização XML
│   │   ├── Validation/             # Validação contra XSD
│   │   ├── Signing/                # Assinatura digital
│   │   └── Factory/                # Factory para mensagens
│   ├── EFinanceira.Messages/       # Builders e POCOs
│   │   ├── Builders/               # Builders fluentes
│   │   └── Generated/              # POCOs geradas por XSD
│   ├── EFinanceira.Tools.CodeGen/  # CLI para geração
│   └── EFinanceira.Console.Sample/ # Aplicação exemplo
├── tests/
│   └── EFinanceira.Tests/          # Testes unitários
├── scripts/                       # Scripts de build
└── docs/                          # Documentação
```

### 🔍 Exemplos Avançados

#### Configuração Completa com appsettings.json

```json
{
  "EFinanceira": {
    "Declarante": {
      "Cnpj": "12345678000199",
      "Nome": "Empresa Demo Ltda"
    },
    "Certificate": {
      "PfxPath": "./certificates/certificado.pfx",
      "PfxPassword": "senha123"
    },
    "Validation": {
      "SchemasPath": "./schemas",
      "ValidateOnSerialization": true
    },
    "Environment": {
      "TipoAmbiente": 2,
      "VersaoAplicacao": "1.0.0"
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "EFinanceira": "Debug"
    }
  }
}
```

#### Exemplo com Múltiplos Reportáveis

```csharp
var evento = new LeiauteMovimentacaoFinanceiraBuilder()
    .WithId("EVT_MULTI_001")
    .WithDeclarante("12345678000199")
    .WithDeclarado(d => d.WithTipoNI(2).WithNumeroInscricao("98765432000188").WithNome("Cliente"))
    .WithMovimentacao(m =>
    {
        m.WithDataInicio(DateTime.Today.AddDays(-365)).WithDataFim(DateTime.Today)
         // Conta corrente
         .AdicionarReportavel(r => 
             r.WithTipoReportavel(1)
              .WithInformacoesConta(c => c.WithTipoConta(1).WithMoeda("986")))
         // Conta poupança
         .AdicionarReportavel(r => 
             r.WithTipoReportavel(1)
              .WithInformacoesConta(c => c.WithTipoConta(2).WithMoeda("986")))
         // Investimentos
         .AdicionarReportavel(r => 
             r.WithTipoReportavel(2)
              .WithInformacoesConta(c => c.WithTipoConta(3).WithMoeda("986")));
    })
    .Build();
```

### 📚 Documentação

- [📖 Guia de Instalação](docs/installation.md)
- [🚀 Guia de Início Rápido](docs/quickstart.md)
- [🏗️ Arquitetura](docs/architecture.md)
- [🔧 Configuração](docs/configuration.md)
- [📝 API Reference](docs/api-reference.md)
- [❓ FAQ](docs/faq.md)

### 🧪 Testes

```bash
# Executar todos os testes
dotnet test

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Executar testes específicos
dotnet test --filter "FullyQualifiedName~EFinanceiraIntegrationTests"
```

### 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

### 🔄 Versionamento

Este projeto segue [Semantic Versioning](https://semver.org/):

- **MAJOR**: Mudanças incompatíveis na API
- **MINOR**: Funcionalidades adicionadas de forma compatível
- **PATCH**: Correções de bugs compatíveis

### 📋 Roadmap

- [ ] **v1.0.0**: Release inicial com funcionalidades básicas
- [ ] **v1.1.0**: Suporte a novos tipos de eventos
- [ ] **v1.2.0**: Integração com Azure Key Vault para certificados
- [ ] **v2.0.0**: Suporte a múltiplas versões de schemas simultaneamente

### ⚠️ Limitações Conhecidas

1. **Certificados**: Atualmente suporta apenas certificados A1 (arquivo .pfx)
2. **Schemas**: Requer schemas XSD locais para validação completa
3. **Plataforma**: Testado apenas em Windows (Linux/macOS em desenvolvimento)

### 📞 Suporte

- 🐛 **Issues**: [GitHub Issues](https://github.com/seuusuario/EFinanceira.Core.Package/issues)
- 💬 **Discussões**: [GitHub Discussions](https://github.com/seuusuario/EFinanceira.Core.Package/discussions)
- 📧 **Email**: suporte@empresa.com.br

### 📄 Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.

### 👥 Autores

- **Seu Nome** - *Trabalho inicial* - [@seuusuario](https://github.com/seuusuario)

### 🙏 Agradecimentos

- Receita Federal do Brasil pelas especificações da e-Financeira
- Comunidade .NET pelo ecossistema robusto
- Contribuidores e testadores do projeto

---

⭐ **Se este projeto te ajudou, deixe uma estrela!** ⭐

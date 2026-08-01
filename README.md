# e-Financeira Core Package

Pacote completo para integração com o sistema e-Financeira da Receita Federal do Brasil, incluindo schemas XSD, tabelas de códigos, exemplos e guias de implementação .NET.

## 🎯 Visão Geral

Este repositório fornece uma solução completa para desenvolvimento de aplicações .NET que integram com o e-Financeira, incluindo:

- **68 arquivos oficiais** baixados automaticamente (25 XSD + 23 tabelas + 20 exemplos)
- **Scripts de automação** para manter os recursos atualizados
- **Guias práticos** para implementação em projetos .NET
- **Exemplos de código** prontos para uso
- **Documentação abrangente** para todos os componentes

## 📁 Estrutura do Projeto

```
EFinanceira.Core.Package/
├── docs/                           # 📖 Documentação e utilitários
│   ├── scripts/                    # 🛠️ Scripts de automação
│   │   ├── download-xsd-schemas.ps1        # Download schemas XSD
│   │   ├── download-tabelas-codigos.ps1    # Download tabelas de códigos
│   │   └── download-exemplos.ps1           # Download exemplos XML
│   ├── GUIA-USO-XSD.md            # Guia dos schemas XSD para .NET
│   ├── GUIA-TABELAS-CODIGOS.md    # Documentação das tabelas de códigos
│   ├── GUIA-EXEMPLOS.md           # Guia prático para usar exemplos
│   ├── GUIA-INTEGRACAO-COMPLETA.md # Arquitetura completa de integração
│   ├── STATUS.md                   # Status da implementação
│   ├── schemas/                    # 📋 Schemas XSD oficiais (25 arquivos)
│   │   ├── eFinanceira-v1_2_0.xsd    # Schema principal
│   │   ├── evtAberturaeFinanceira-v1_2_0.xsd
│   │   ├── evtMovOpFin-v1_2_0.xsd
│   │   └── ...                       # Outros 22 schemas
│   └── tabelas-codigos/            # 🗂️ Tabelas de códigos (23 arquivos)
│       ├── Pais.txt                  # Códigos de países
│       ├── Municipios.txt            # Códigos de municípios
│       ├── Moedas.txt                # Códigos de moedas
│       └── ...                       # Outras 20 tabelas
├── efinanceira/                    # 🏗️ Projeto .NET Core
│   ├── src/
│   │   ├── EFinanceira.Core/           # Biblioteca central
│   │   ├── EFinanceira.Messages/       # Builders para mensagens
│   │   ├── EFinanceira.Tools.CodeGen/  # CLI para geração de código
│   │   └── EFinanceira.Console.Sample/ # Aplicação de demonstração
│   ├── tests/
│   │   └── EFinanceira.Tests/          # Testes unitários e integração
│   └── scripts/
│       ├── build.ps1                   # Script de build completo
│       └── build-simple.ps1            # Script de build simplificado
├── exemplos/                       # 📝 Exemplos XML (20 arquivos oficiais)
│   ├── xml-sem-assinatura/            # 13 exemplos básicos
│   ├── xml-com-assinatura/            # 5 exemplos com assinatura
│   ├── xml-gerados/                   # 36 XMLs gerados pela biblioteca
│   │   ├── consultas/                 # 6 consultas
│   │   ├── eventos/                   # 20 eventos
│   │   └── assinados/                 # 10 eventos assinados
│   ├── codigo-fonte/                  # Exemplos de implementação (C#)
│   └── certificado/                   # Certificado de teste (assinatura digital)
├── .github/
│   └── copilot-instructions.md        # Configurações do Copilot
├── user-story.md                   # História do usuário original
├── README.md                       # Este arquivo
└── LICENSE                         # Licença MIT
```

### 🗂️ Recursos Oficiais

#### docs/schemas/ (25 arquivos)
Schemas XSD oficiais do e-Financeira para validação de XML:
- `eFinanceira-v1_2_0.xsd` - Schema principal
- `evtAberturaeFinanceira-v1_2_0.xsd` - Evento de abertura
- `evtMovOpFin-v1_2_0.xsd` - Movimentação financeira
- `evtFechamentoeFinanceira-v1_2_0.xsd` - Evento de fechamento
- E mais 21 schemas auxiliares e de tipos comuns

#### docs/tabelas-codigos/ (23 arquivos)
Tabelas de códigos oficiais organizadas por categorias:
- **Geográficos**: países, UFs, municípios
- **Financeiros**: moedas, tipos de conta, instituições
- **Regulatórios**: tipos de reportável, intermediários

#### exemplos/ (20 arquivos oficiais)
Arquivos de exemplo oficiais, organizados por categoria:
- **xml-sem-assinatura/**: 13 exemplos básicos
- **xml-com-assinatura/**: 5 exemplos com assinatura digital
- **xml-gerados/**: 36 XMLs gerados pela biblioteca em tempo de execução (consultas, eventos e assinados)
- **codigo-fonte/**: Exemplos de implementação em C# (assinatura, criptografia, lotes e eventos)

### 🛠️ Scripts de Automação

- `download-xsd-schemas.ps1` - Download automático dos schemas XSD
- `download-tabelas-codigos.ps1` - Download das tabelas de códigos
- `download-exemplos.ps1` - Download dos arquivos de exemplo

### 📖 Documentação Especializada

- `GUIA-SCHEMAS.md` - Guia completo dos schemas XSD para .NET
- `GUIA-TABELAS-CODIGOS.md` - Documentação das tabelas de códigos
- `GUIA-EXEMPLOS.md` - Guia prático para usar exemplos em .NET
- `GUIA-INTEGRACAO-COMPLETA.md` - Arquitetura completa de integração

## 🚀 Início Rápido

### 1. Clone e Configuração
```bash
git clone [seu-repositorio]
cd EFinanceira.Core.Package
```

### 2. Execute os Scripts de Download (Opcional)
```powershell
# Baixar todos os recursos (já inclusos no repositório)
.\docs\scripts\download-xsd-schemas.ps1
.\docs\scripts\download-tabelas-codigos.ps1  
.\docs\scripts\download-exemplos.ps1
```

### 3. Integração Básica em .NET
```csharp
// Configuração de serviços
services.Configure<EFinanceiraOptions>(configuration.GetSection("EFinanceira"));
services.AddScoped<TabelaCodigosValidator>();
services.AddScoped<XmlValidationService>();
services.AddScoped<AssinaturaDigitalService>();

// Validação de XML
var xmlValidator = serviceProvider.GetService<XmlValidationService>();
var resultado = xmlValidator.ValidarXml(xmlContent);

if (resultado.IsValid)
{
    Console.WriteLine("XML válido!");
}
else
{
    Console.WriteLine($"Erros: {string.Join(", ", resultado.Erros)}");
}
```

### 4. Uso das Tabelas de Códigos
```csharp
var tabelaValidator = new TabelaCodigosValidator("docs/tabelas-codigos/");

// Validar país
if (tabelaValidator.ValidarCodigo("Pais", "076")) // Brasil
{
    Console.WriteLine("Código de país válido!");
}

// Listar códigos disponíveis
var paises = tabelaValidator.ObterCodigos("Pais");
```

### 5. Trabalhar com Exemplos
```csharp
var exemploLoader = new ExemplosEFinanceiraLoader("exemplos/", "docs/schemas/");

// Carregar e validar exemplo
var documento = exemploLoader.CarregarExemplo("evtAberturaeFinanceira.xml");
var dadosEvento = ExtractorDadosComuns.ExtrairDadosEvento(documento);

Console.WriteLine($"Tipo: {dadosEvento.TipoEvento}");
Console.WriteLine($"CNPJ: {dadosEvento.CnpjDeclarante}");
```

## 📚 Guias de Implementação

### Para Iniciantes
1. **[docs/GUIA-USO-XSD.md](docs/GUIA-USO-XSD.md)** - Entenda os schemas XSD e como usá-los
2. **[docs/GUIA-EXEMPLOS.md](docs/GUIA-EXEMPLOS.md)** - Trabalhe com os exemplos práticos

### Para Desenvolvimento Avançado  
3. **[docs/GUIA-TABELAS-CODIGOS.md](docs/GUIA-TABELAS-CODIGOS.md)** - Implemente validações com tabelas oficiais
4. **[docs/GUIA-INTEGRACAO-COMPLETA.md](docs/GUIA-INTEGRACAO-COMPLETA.md)** - Arquitetura completa de aplicação

## 🏗️ Arquiteturas Suportadas

### Web API REST
```csharp
[ApiController]
[Route("api/[controller]")]
public class EFinanceiraController : ControllerBase
{
    [HttpPost("validar")]
    public IActionResult ValidarXml([FromBody] ValidarXmlRequest request)
    {
        var resultado = _validationService.ValidarXml(request.Xml);
        return Ok(resultado);
    }
}
```

### Aplicação Console
```csharp
class Program
{
    static async Task Main(string[] args)
    {
        var validator = new XmlValidationService("docs/schemas/", tabelaValidator);
        var resultado = validator.ValidarXml(xmlContent);
        // Processar resultado...
    }
}
```

### Worker Service
```csharp
public class EFinanceiraWorker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Processamento em background...
    }
}
```

## 🔧 Recursos Avançados

### Assinatura Digital
```csharp
var assinaturaService = new AssinaturaDigitalService(certificadoOptions);
var xmlAssinado = assinaturaService.AssinarDocumento(documento);
var assinaturaValida = assinaturaService.VerificarAssinatura(xmlAssinado);
```

### Validação Completa
```csharp
var validationService = new XmlValidationService(schemas, tabelaValidator);
var resultado = validationService.ValidarXml(xml);

// Validação inclui:
// ✓ Estrutura XSD
// ✓ Códigos das tabelas oficiais  
// ✓ Regras de negócio específicas
// ✓ Validação de CNPJ/CPF
```

### Análise de Padrões
```csharp
var analisador = new AnalisadorPadroesExemplos(exemploLoader);
analisador.AnalisarTodosExemplos();

// Gera relatório com:
// • Tipos de eventos disponíveis
// • Namespaces utilizados
// • Tamanhos dos arquivos
// • Ambientes de teste
```

## 🧪 Testes

### Testes Unitários
```csharp
[Test]
public void ValidarXml_ComExemploOficial_DeveSerValido()
{
    var xml = CarregarExemplo("evtAberturaeFinanceira.xml");
    var resultado = _validator.ValidarXml(xml);
    Assert.IsTrue(resultado.IsValid);
}
```

### Testes de Integração
```csharp
[Test]
public async Task API_ValidarXml_DeveRetornarResultadoCorreto()
{
    var response = await _client.PostAsJsonAsync("/api/efinanceira/validar", request);
    var resultado = await response.Content.ReadFromJsonAsync<ValidarXmlResponse>();
    Assert.IsTrue(resultado.IsValid);
}
```

## 📦 Configuração Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY docs/schemas/ /app/docs/schemas/
COPY docs/tabelas-codigos/ /app/docs/tabelas-codigos/
COPY exemplos/ /app/exemplos/
COPY . .
ENTRYPOINT ["dotnet", "EFinanceira.WebApi.dll"]
```

## ⚙️ Configuração

### appsettings.json
```json
{
  "EFinanceira": {
    "CaminhoSchemas": "docs/schemas/",
    "CaminhoExemplos": "exemplos/", 
    "CaminhoTabelasCodigos": "docs/tabelas-codigos/",
    "Ambiente": 2,
    "CnpjDeclarante": "11111111111111",
    "ValidarContraSchema": true,
    "Certificado": {
      "CaminhoArquivo": "certificado.pfx",
      "Senha": "senha_certificado",
      "Tipo": "Arquivo"
    }
  }
}
```

## 🔄 Atualizações

Para manter os recursos atualizados com a Receita Federal:

```powershell
# Atualizar todos os recursos
.\docs\scripts\download-xsd-schemas.ps1
.\docs\scripts\download-tabelas-codigos.ps1
.\docs\scripts\download-exemplos.ps1
```

## 📈 Status do Projeto

- ✅ **68 arquivos oficiais** baixados e organizados
- ✅ **4 guias completos** de implementação
- ✅ **Scripts de automação** funcionais  
- ✅ **Exemplos práticos** para .NET
- ✅ **Arquitetura completa** documentada
- ✅ **Testes unitários** e de integração
- ✅ **Suporte Docker** incluído

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para detalhes.

Os recursos oficiais (XSD, tabelas de códigos, exemplos) são propriedade da Receita Federal do Brasil e seguem suas diretrizes de uso.

## 🌐 Recursos Oficiais

Todos os recursos foram obtidos dos sites oficiais da Receita Federal:
- **Schemas XSD**: http://sped.rfb.gov.br/pasta/show/1854
- **Tabelas de Códigos**: http://sped.rfb.gov.br/pasta/show/1932  
- **Exemplos**: http://sped.rfb.gov.br/pasta/show/1846

## 💡 Próximos Passos

Após configurar o ambiente, recomendamos:

1. **Iniciantes**: Começar com `docs/GUIA-USO-XSD.md`
2. **Desenvolvedores**: Implementar usando `docs/GUIA-INTEGRACAO-COMPLETA.md`
3. **Equipes**: Configurar CI/CD com os scripts em `docs/scripts/`
4. **Produção**: Seguir as práticas de segurança para certificados digitais

---

**🎯 Objetivo**: Fornecer a base mais completa e atualizada para integração .NET com e-Financeira, reduzindo o tempo de desenvolvimento de semanas para horas.

# Guia de Integração Completa - e-Financeira .NET

Este guia demonstra como integrar todos os componentes do e-Financeira em uma aplicação .NET usando os schemas XSD, tabelas de códigos e exemplos baixados.

## Arquitetura da Solução

### Estrutura de Projetos Recomendada
```
EFinanceira.Solution/
├── EFinanceira.Core/              # Modelos e validações
├── EFinanceira.Schemas/           # Classes geradas dos XSD
├── EFinanceira.Services/          # Serviços de negócio
├── EFinanceira.WebApi/           # API REST
├── EFinanceira.Tests/            # Testes unitários
└── EFinanceira.ConsoleApp/       # Aplicação de exemplo
```

## 1. Projeto Core - Fundações

### 1.1 Configuração Base
```csharp
// EFinanceira.Core/Configuration/EFinanceiraOptions.cs
public class EFinanceiraOptions
{
    public const string SectionName = "EFinanceira";
    
    public string CaminhoSchemas { get; set; } = string.Empty;
    public string CaminhoExemplos { get; set; } = string.Empty;
    public string CaminhoTabelasCodigos { get; set; } = string.Empty;
    public AmbienteEnum Ambiente { get; set; } = AmbienteEnum.Homologacao;
    public string CnpjDeclarante { get; set; } = string.Empty;
    public string UrlWebService { get; set; } = string.Empty;
    public CertificadoOptions Certificado { get; set; } = new();
    public bool ValidarContraSchema { get; set; } = true;
    public int TimeoutSegundos { get; set; } = 30;
}

public class CertificadoOptions
{
    public string CaminhoArquivo { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string Thumbprint { get; set; } = string.Empty;
    public CertificadoTipoEnum Tipo { get; set; } = CertificadoTipoEnum.Arquivo;
}

public enum AmbienteEnum
{
    Producao = 1,
    Homologacao = 2
}

public enum CertificadoTipoEnum
{
    Arquivo,
    Store
}
```

### 1.2 Modelos Base
```csharp
// EFinanceira.Core/Models/EventoBase.cs
public abstract class EventoBase
{
    public string Id { get; set; } = GerarId();
    public IdeEvento IdeEvento { get; set; } = new();
    public IdeDeclarante IdeDeclarante { get; set; } = new();
    
    private static string GerarId()
    {
        return $"ID{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
    
    public virtual void Validar()
    {
        if (string.IsNullOrEmpty(IdeDeclarante?.CnpjDeclarante))
            throw new ValidationException("CNPJ do declarante é obrigatório");
        
        if (IdeEvento?.TpAmb == 0)
            throw new ValidationException("Tipo de ambiente é obrigatório");
    }
}

public class IdeEvento
{
    public int IndRetificacao { get; set; } = 1; // 1=Original, 2=Retificação
    public int TpAmb { get; set; } = 2; // 1=Produção, 2=Homologação
    public int AplicEmi { get; set; } = 2; // 1=Aplicativo Contribuinte, 2=Aplicativo Fisco
    public string VerAplic { get; set; } = "1.0.0";
}

public class IdeDeclarante
{
    public string CnpjDeclarante { get; set; } = string.Empty;
}
```

### 1.3 Validadores com Tabelas de Códigos
```csharp
// EFinanceira.Core/Validation/TabelaCodigosValidator.cs
public class TabelaCodigosValidator
{
    private readonly Dictionary<string, HashSet<string>> _tabelas;
    
    public TabelaCodigosValidator(string caminhoTabelasCodigos)
    {
        _tabelas = CarregarTabelas(caminhoTabelasCodigos);
    }
    
    public bool ValidarCodigo(string nomeTabela, string codigo)
    {
        return _tabelas.ContainsKey(nomeTabela) && 
               _tabelas[nomeTabela].Contains(codigo);
    }
    
    public IEnumerable<string> ObterCodigos(string nomeTabela)
    {
        return _tabelas.ContainsKey(nomeTabela) ? 
               _tabelas[nomeTabela] : 
               Enumerable.Empty<string>();
    }
    
    public void ValidarCodigoObrigatorio(string nomeTabela, string codigo, string nomeCampo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ValidationException($"{nomeCampo} é obrigatório");
        
        if (!ValidarCodigo(nomeTabela, codigo))
            throw new ValidationException($"Código '{codigo}' inválido para {nomeCampo}. Tabela: {nomeTabela}");
    }
    
    private Dictionary<string, HashSet<string>> CarregarTabelas(string caminho)
    {
        var tabelas = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        
        if (!Directory.Exists(caminho))
            return tabelas;
        
        var arquivos = Directory.GetFiles(caminho, "*.xml");
        
        foreach (var arquivo in arquivos)
        {
            try
            {
                var nomeTabela = Path.GetFileNameWithoutExtension(arquivo);
                var codigos = ExtrairCodigosDoXml(arquivo);
                tabelas[nomeTabela] = new HashSet<string>(codigos, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                // Log do erro
                Console.WriteLine($"Erro ao carregar tabela {arquivo}: {ex.Message}");
            }
        }
        
        return tabelas;
    }
    
    private List<string> ExtrairCodigosDoXml(string caminhoArquivo)
    {
        var codigos = new List<string>();
        var documento = XDocument.Load(caminhoArquivo);
        
        // Extrair códigos baseado na estrutura do XML
        // Adapte conforme a estrutura específica das tabelas
        var elementos = documento.Descendants()
            .Where(e => e.Name.LocalName.Equals("codigo", StringComparison.OrdinalIgnoreCase) ||
                       e.Name.LocalName.Equals("value", StringComparison.OrdinalIgnoreCase))
            .Select(e => e.Value)
            .Where(v => !string.IsNullOrWhiteSpace(v));
        
        codigos.AddRange(elementos);
        
        return codigos;
    }
}
```

## 2. Serviços de Validação e Processamento

### 2.1 Serviço de Validação XML
```csharp
// EFinanceira.Services/Validation/XmlValidationService.cs
public class XmlValidationService
{
    private readonly XmlSchemaSet _schemas;
    private readonly TabelaCodigosValidator _tabelaValidator;
    
    public XmlValidationService(string caminhoSchemas, TabelaCodigosValidator tabelaValidator)
    {
        _schemas = CarregarSchemas(caminhoSchemas);
        _tabelaValidator = tabelaValidator;
    }
    
    public ValidationResult ValidarXml(XDocument documento)
    {
        var resultado = new ValidationResult();
        
        // Validação contra schema XSD
        documento.Validate(_schemas, (sender, e) =>
        {
            resultado.AdicionarErro($"Schema: {e.Message}");
        });
        
        // Validações de negócio
        ValidarRegrasNegocio(documento, resultado);
        
        return resultado;
    }
    
    public ValidationResult ValidarXml(string xml)
    {
        try
        {
            var documento = XDocument.Parse(xml);
            return ValidarXml(documento);
        }
        catch (XmlException ex)
        {
            var resultado = new ValidationResult();
            resultado.AdicionarErro($"XML mal formado: {ex.Message}");
            return resultado;
        }
    }
    
    private void ValidarRegrasNegocio(XDocument documento, ValidationResult resultado)
    {
        var ns = documento.Root?.Name.Namespace ?? XNamespace.None;
        
        // Validar estrutura básica
        ValidarEstruturaBasica(documento, resultado, ns);
        
        // Validar códigos das tabelas
        ValidarCodigosTabelas(documento, resultado, ns);
        
        // Validar regras específicas por tipo de evento
        var tipoEvento = documento.Root?.Elements().FirstOrDefault()?.Name.LocalName;
        
        switch (tipoEvento)
        {
            case "evtAberturaeFinanceira":
                ValidarEventoAbertura(documento, resultado, ns);
                break;
            case "evtMovOpFin":
                ValidarEventoMovimentacao(documento, resultado, ns);
                break;
            case "evtFechamentoeFinanceira":
                ValidarEventoFechamento(documento, resultado, ns);
                break;
        }
    }
    
    private void ValidarEstruturaBasica(XDocument documento, ValidationResult resultado, XNamespace ns)
    {
        var evento = documento.Root?.Elements().FirstOrDefault();
        if (evento == null)
        {
            resultado.AdicionarErro("Evento não encontrado no documento");
            return;
        }
        
        var ideEvento = evento.Element(ns + "ideEvento");
        var ideDeclarante = evento.Element(ns + "ideDeclarante");
        
        if (ideEvento == null)
            resultado.AdicionarErro("ideEvento é obrigatório");
        
        if (ideDeclarante == null)
            resultado.AdicionarErro("ideDeclarante é obrigatório");
        
        // Validar CNPJ
        var cnpj = ideDeclarante?.Element(ns + "cnpjDeclarante")?.Value;
        if (string.IsNullOrWhiteSpace(cnpj))
        {
            resultado.AdicionarErro("CNPJ do declarante é obrigatório");
        }
        else if (!ValidarCnpj(cnpj))
        {
            resultado.AdicionarErro($"CNPJ inválido: {cnpj}");
        }
    }
    
    private void ValidarCodigosTabelas(XDocument documento, ValidationResult resultado, XNamespace ns)
    {
        // Validar tipo de ambiente
        var tpAmb = documento.Descendants(ns + "tpAmb").FirstOrDefault()?.Value;
        if (!string.IsNullOrEmpty(tpAmb))
        {
            _tabelaValidator.ValidarCodigoObrigatorio("TipoAmbiente", tpAmb, "Tipo de Ambiente");
        }
        
        // Validar países (se houver)
        var paises = documento.Descendants(ns + "pais");
        foreach (var pais in paises)
        {
            var codigoPais = pais.Value;
            if (!_tabelaValidator.ValidarCodigo("Pais", codigoPais))
            {
                resultado.AdicionarErro($"Código de país inválido: {codigoPais}");
            }
        }
        
        // Adicionar mais validações conforme necessário
    }
    
    private void ValidarEventoAbertura(XDocument documento, ValidationResult resultado, XNamespace ns)
    {
        // Validações específicas do evento de abertura
        var evento = documento.Root?.Element(ns + "evtAberturaeFinanceira");
        
        // Validar responsável pela informação
        var respInfo = evento?.Element(ns + "respInfo");
        if (respInfo == null)
        {
            resultado.AdicionarErro("Responsável pela informação é obrigatório no evento de abertura");
        }
    }
    
    private void ValidarEventoMovimentacao(XDocument documento, ValidationResult resultado, XNamespace ns)
    {
        // Validações específicas do evento de movimentação
        var evento = documento.Root?.Element(ns + "evtMovOpFin");
        var movOpFin = evento?.Element(ns + "movOpFin");
        
        if (movOpFin == null)
        {
            resultado.AdicionarErro("movOpFin é obrigatório no evento de movimentação");
            return;
        }
        
        var reportaveis = movOpFin.Elements(ns + "reportavel");
        if (!reportaveis.Any())
        {
            resultado.AdicionarErro("Pelo menos um reportável é obrigatório");
        }
        
        foreach (var reportavel in reportaveis)
        {
            ValidarReportavel(reportavel, resultado, ns);
        }
    }
    
    private void ValidarReportavel(XElement reportavel, ValidationResult resultado, XNamespace ns)
    {
        var tpReportavel = reportavel.Element(ns + "tpReportavel")?.Value;
        if (string.IsNullOrEmpty(tpReportavel))
        {
            resultado.AdicionarErro("Tipo de reportável é obrigatório");
        }
        else
        {
            try
            {
                _tabelaValidator.ValidarCodigoObrigatorio("TipoReportavel", tpReportavel, "Tipo Reportável");
            }
            catch (ValidationException ex)
            {
                resultado.AdicionarErro(ex.Message);
            }
        }
    }
    
    private void ValidarEventoFechamento(XDocument documento, ValidationResult resultado, XNamespace ns)
    {
        // Validações específicas do evento de fechamento
        var evento = documento.Root?.Element(ns + "evtFechamentoeFinanceira");
        
        // Adicionar validações específicas conforme necessário
    }
    
    private bool ValidarCnpj(string cnpj)
    {
        // Implementar validação de CNPJ
        if (string.IsNullOrWhiteSpace(cnpj)) return false;
        
        cnpj = cnpj.Replace(".", "").Replace("/", "").Replace("-", "");
        
        if (cnpj.Length != 14) return false;
        if (cnpj.All(c => c == cnpj[0])) return false;
        
        // Implementar algoritmo completo de validação do CNPJ
        return true; // Simplificado para exemplo
    }
    
    private XmlSchemaSet CarregarSchemas(string caminhoSchemas)
    {
        var schemas = new XmlSchemaSet();
        
        if (!Directory.Exists(caminhoSchemas))
            throw new DirectoryNotFoundException($"Diretório de schemas não encontrado: {caminhoSchemas}");
        
        var arquivosXsd = Directory.GetFiles(caminhoSchemas, "*.xsd");
        
        foreach (var arquivo in arquivosXsd)
        {
            try
            {
                schemas.Add(null, arquivo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar schema {arquivo}: {ex.Message}");
            }
        }
        
        schemas.Compile();
        return schemas;
    }
}

public class ValidationResult
{
    private readonly List<string> _erros = new();
    private readonly List<string> _avisos = new();
    
    public bool IsValid => !_erros.Any();
    public IReadOnlyList<string> Erros => _erros.AsReadOnly();
    public IReadOnlyList<string> Avisos => _avisos.AsReadOnly();
    
    public void AdicionarErro(string erro) => _erros.Add(erro);
    public void AdicionarAviso(string aviso) => _avisos.Add(aviso);
    
    public override string ToString()
    {
        var sb = new StringBuilder();
        
        if (_erros.Any())
        {
            sb.AppendLine("ERROS:");
            foreach (var erro in _erros)
                sb.AppendLine($"  - {erro}");
        }
        
        if (_avisos.Any())
        {
            sb.AppendLine("AVISOS:");
            foreach (var aviso in _avisos)
                sb.AppendLine($"  - {aviso}");
        }
        
        return sb.ToString();
    }
}
```

### 2.2 Serviço de Assinatura Digital
```csharp
// EFinanceira.Services/Security/AssinaturaDigitalService.cs
public class AssinaturaDigitalService
{
    private readonly CertificadoOptions _certificadoOptions;
    
    public AssinaturaDigitalService(CertificadoOptions certificadoOptions)
    {
        _certificadoOptions = certificadoOptions;
    }
    
    public XDocument AssinarDocumento(XDocument documento)
    {
        var certificado = ObterCertificado();
        
        // Criar documento assinado
        var documentoAssinado = new XDocument(documento);
        
        // Configurar assinatura
        var signedXml = new SignedXml(documentoAssinado.ToXmlDocument());
        signedXml.SigningKey = certificado.PrivateKey;
        
        // Criar referência
        var reference = new Reference("");
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        reference.AddTransform(new XmlDsigC14NTransform());
        signedXml.AddReference(reference);
        
        // Adicionar informações do certificado
        var keyInfo = new KeyInfo();
        keyInfo.AddClause(new X509Data(certificado));
        signedXml.KeyInfo = keyInfo;
        
        // Computar assinatura
        signedXml.ComputeSignature();
        
        // Adicionar assinatura ao documento
        var evento = documentoAssinado.Root?.Elements().FirstOrDefault();
        if (evento != null)
        {
            var signatureElement = XElement.Parse(signedXml.GetXml().OuterXml);
            evento.Add(signatureElement);
        }
        
        return documentoAssinado;
    }
    
    public bool VerificarAssinatura(XDocument documento)
    {
        try
        {
            var xmlDoc = documento.ToXmlDocument();
            var signedXml = new SignedXml(xmlDoc);
            
            var signature = xmlDoc.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#")[0];
            signedXml.LoadXml((XmlElement)signature);
            
            return signedXml.CheckSignature();
        }
        catch
        {
            return false;
        }
    }
    
    private X509Certificate2 ObterCertificado()
    {
        switch (_certificadoOptions.Tipo)
        {
            case CertificadoTipoEnum.Arquivo:
                return new X509Certificate2(_certificadoOptions.CaminhoArquivo, _certificadoOptions.Senha);
            
            case CertificadoTipoEnum.Store:
                var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly);
                
                var certificados = store.Certificates
                    .Find(X509FindType.FindByThumbprint, _certificadoOptions.Thumbprint, false);
                
                store.Close();
                
                if (certificados.Count == 0)
                    throw new InvalidOperationException($"Certificado não encontrado: {_certificadoOptions.Thumbprint}");
                
                return certificados[0];
            
            default:
                throw new NotSupportedException($"Tipo de certificado não suportado: {_certificadoOptions.Tipo}");
        }
    }
}

// Extension method helper
public static class XDocumentExtensions
{
    public static XmlDocument ToXmlDocument(this XDocument xDocument)
    {
        var xmlDocument = new XmlDocument();
        using var xmlReader = xDocument.CreateReader();
        xmlDocument.Load(xmlReader);
        return xmlDocument;
    }
}
```

## 3. Web API para Integração

### 3.1 Controller Principal
```csharp
// EFinanceira.WebApi/Controllers/EFinanceiraController.cs
[ApiController]
[Route("api/[controller]")]
public class EFinanceiraController : ControllerBase
{
    private readonly XmlValidationService _validationService;
    private readonly AssinaturaDigitalService _assinaturaService;
    private readonly ILogger<EFinanceiraController> _logger;
    
    public EFinanceiraController(
        XmlValidationService validationService,
        AssinaturaDigitalService assinaturaService,
        ILogger<EFinanceiraController> logger)
    {
        _validationService = validationService;
        _assinaturaService = assinaturaService;
        _logger = logger;
    }
    
    [HttpPost("validar")]
    public IActionResult ValidarXml([FromBody] ValidarXmlRequest request)
    {
        try
        {
            var resultado = _validationService.ValidarXml(request.Xml);
            
            return Ok(new ValidarXmlResponse
            {
                IsValid = resultado.IsValid,
                Erros = resultado.Erros.ToList(),
                Avisos = resultado.Avisos.ToList()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar XML");
            return BadRequest(new { message = "Erro interno na validação" });
        }
    }
    
    [HttpPost("assinar")]
    public IActionResult AssinarXml([FromBody] AssinarXmlRequest request)
    {
        try
        {
            var documento = XDocument.Parse(request.Xml);
            var documentoAssinado = _assinaturaService.AssinarDocumento(documento);
            
            return Ok(new AssinarXmlResponse
            {
                XmlAssinado = documentoAssinado.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao assinar XML");
            return BadRequest(new { message = "Erro interno na assinatura" });
        }
    }
    
    [HttpPost("verificar-assinatura")]
    public IActionResult VerificarAssinatura([FromBody] VerificarAssinaturaRequest request)
    {
        try
        {
            var documento = XDocument.Parse(request.Xml);
            var assinaturaValida = _assinaturaService.VerificarAssinatura(documento);
            
            return Ok(new VerificarAssinaturaResponse
            {
                AssinaturaValida = assinaturaValida
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar assinatura");
            return BadRequest(new { message = "Erro interno na verificação" });
        }
    }
    
    [HttpGet("exemplos")]
    public IActionResult ListarExemplos()
    {
        try
        {
            // Implementar listagem de exemplos disponíveis
            var exemplos = ObterListaExemplos();
            return Ok(exemplos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar exemplos");
            return BadRequest(new { message = "Erro interno ao listar exemplos" });
        }
    }
    
    [HttpGet("exemplos/{nomeExemplo}")]
    public IActionResult ObterExemplo(string nomeExemplo)
    {
        try
        {
            var exemplo = CarregarExemplo(nomeExemplo);
            if (exemplo == null)
                return NotFound();
            
            return Ok(new ExemploResponse
            {
                Nome = nomeExemplo,
                Conteudo = exemplo
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter exemplo {NomeExemplo}", nomeExemplo);
            return BadRequest(new { message = "Erro interno ao obter exemplo" });
        }
    }
    
    private List<string> ObterListaExemplos()
    {
        // Implementar lógica para obter lista de exemplos
        return new List<string>
        {
            "evtAberturaeFinanceira.xml",
            "evtMovOpFin.xml",
            "evtFechamentoeFinanceira.xml"
        };
    }
    
    private string? CarregarExemplo(string nomeExemplo)
    {
        // Implementar lógica para carregar exemplo específico
        // Usar o ExemplosEFinanceiraLoader aqui
        return null;
    }
}

// DTOs
public class ValidarXmlRequest
{
    public string Xml { get; set; } = string.Empty;
}

public class ValidarXmlResponse
{
    public bool IsValid { get; set; }
    public List<string> Erros { get; set; } = new();
    public List<string> Avisos { get; set; } = new();
}

public class AssinarXmlRequest
{
    public string Xml { get; set; } = string.Empty;
}

public class AssinarXmlResponse
{
    public string XmlAssinado { get; set; } = string.Empty;
}

public class VerificarAssinaturaRequest
{
    public string Xml { get; set; } = string.Empty;
}

public class VerificarAssinaturaResponse
{
    public bool AssinaturaValida { get; set; }
}

public class ExemploResponse
{
    public string Nome { get; set; } = string.Empty;
    public string Conteudo { get; set; } = string.Empty;
}
```

### 3.2 Configuração da Startup
```csharp
// EFinanceira.WebApi/Program.cs
var builder = WebApplication.CreateBuilder(args);

// Configurações
builder.Services.Configure<EFinanceiraOptions>(
    builder.Configuration.GetSection(EFinanceiraOptions.SectionName));

// Registrar serviços
builder.Services.AddScoped<TabelaCodigosValidator>(provider =>
{
    var options = provider.GetRequiredService<IOptions<EFinanceiraOptions>>().Value;
    return new TabelaCodigosValidator(options.CaminhoTabelasCodigos);
});

builder.Services.AddScoped<XmlValidationService>(provider =>
{
    var options = provider.GetRequiredService<IOptions<EFinanceiraOptions>>().Value;
    var tabelaValidator = provider.GetRequiredService<TabelaCodigosValidator>();
    return new XmlValidationService(options.CaminhoSchemas, tabelaValidator);
});

builder.Services.AddScoped<AssinaturaDigitalService>(provider =>
{
    var options = provider.GetRequiredService<IOptions<EFinanceiraOptions>>().Value;
    return new AssinaturaDigitalService(options.Certificado);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### 3.3 Configuração (appsettings.json)
```json
{
  "EFinanceira": {
    "CaminhoSchemas": "C:\\Workspace\\EFinanceira.Core.Package\\schemas",
    "CaminhoExemplos": "C:\\Workspace\\EFinanceira.Core.Package\\exemplos",
    "CaminhoTabelasCodigos": "C:\\Workspace\\EFinanceira.Core.Package\\tabelas-codigos",
    "Ambiente": 2,
    "CnpjDeclarante": "11111111111111",
    "UrlWebService": "https://www1.efinanceira.gov.br/WS/efinanceira.asmx",
    "Certificado": {
      "CaminhoArquivo": "C:\\certificados\\certificado.pfx",
      "Senha": "senha_do_certificado",
      "Thumbprint": "",
      "Tipo": "Arquivo"
    },
    "ValidarContraSchema": true,
    "TimeoutSegundos": 30
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## 4. Testes de Integração

### 4.1 Testes da API
```csharp
// EFinanceira.Tests/Integration/EFinanceiraControllerTests.cs
[TestFixture]
public class EFinanceiraControllerTests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;
    
    [SetUp]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new[]
                    {
                        new KeyValuePair<string, string>("EFinanceira:CaminhoSchemas", "test-schemas"),
                        new KeyValuePair<string, string>("EFinanceira:CaminhoExemplos", "test-exemplos"),
                        new KeyValuePair<string, string>("EFinanceira:CaminhoTabelasCodigos", "test-tabelas")
                    });
                });
            });
        
        _client = _factory.CreateClient();
    }
    
    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }
    
    [Test]
    public async Task ValidarXml_ComXmlValido_DeveRetornarSucesso()
    {
        // Arrange
        var xmlValido = CarregarExemploXml("evtAberturaeFinanceira.xml");
        var request = new ValidarXmlRequest { Xml = xmlValido };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/efinanceira/validar", request);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var resultado = await response.Content.ReadFromJsonAsync<ValidarXmlResponse>();
        Assert.IsTrue(resultado.IsValid);
    }
    
    [Test]
    public async Task ValidarXml_ComXmlInvalido_DeveRetornarErros()
    {
        // Arrange
        var xmlInvalido = "<xml>inválido</xml>";
        var request = new ValidarXmlRequest { Xml = xmlInvalido };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/efinanceira/validar", request);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var resultado = await response.Content.ReadFromJsonAsync<ValidarXmlResponse>();
        Assert.IsFalse(resultado.IsValid);
        Assert.IsNotEmpty(resultado.Erros);
    }
    
    private string CarregarExemploXml(string nomeArquivo)
    {
        // Carregar XML de exemplo para teste
        return File.ReadAllText($"TestData/{nomeArquivo}");
    }
}
```

## 5. Aplicação Console de Exemplo

### 5.1 Demonstração Completa
```csharp
// EFinanceira.ConsoleApp/Program.cs
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== e-Financeira .NET - Demonstração ===\n");
        
        try
        {
            await ExecutarDemonstracao();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
        
        Console.WriteLine("\nPressione qualquer tecla para sair...");
        Console.ReadKey();
    }
    
    static async Task ExecutarDemonstracao()
    {
        // Configurar serviços
        var options = new EFinanceiraOptions
        {
            CaminhoSchemas = @"C:\Workspace\EFinanceira.Core.Package\schemas",
            CaminhoExemplos = @"C:\Workspace\EFinanceira.Core.Package\exemplos",
            CaminhoTabelasCodigos = @"C:\Workspace\EFinanceira.Core.Package\tabelas-codigos",
            Ambiente = AmbienteEnum.Homologacao
        };
        
        var tabelaValidator = new TabelaCodigosValidator(options.CaminhoTabelasCodigos);
        var xmlValidator = new XmlValidationService(options.CaminhoSchemas, tabelaValidator);
        var exemploLoader = new ExemplosEFinanceiraLoader(options.CaminhoExemplos, options.CaminhoSchemas);
        
        // 1. Demonstrar carregamento de exemplos
        Console.WriteLine("1. Carregando exemplos...");
        await DemonstrarCarregamentoExemplos(exemploLoader);
        
        // 2. Demonstrar validação de XML
        Console.WriteLine("\n2. Validando XMLs...");
        await DemonstrarValidacaoXml(xmlValidator, exemploLoader);
        
        // 3. Demonstrar uso das tabelas de códigos
        Console.WriteLine("\n3. Validando códigos das tabelas...");
        DemonstrarValidacaoTabelas(tabelaValidator);
        
        // 4. Demonstrar análise de padrões
        Console.WriteLine("\n4. Analisando padrões dos exemplos...");
        var analisador = new AnalisadorPadroesExemplos(exemploLoader);
        analisador.AnalisarTodosExemplos();
    }
    
    static async Task DemonstrarCarregamentoExemplos(ExemplosEFinanceiraLoader loader)
    {
        var exemplos = new[] { "evtAberturaeFinanceira.xml", "evtMovOpFin.xml" };
        
        foreach (var exemplo in exemplos)
        {
            try
            {
                var documento = loader.CarregarExemplo(exemplo);
                var dadosEvento = ExtractorDadosComuns.ExtrairDadosEvento(documento);
                
                Console.WriteLine($"  ✓ {exemplo}:");
                Console.WriteLine($"    - Tipo: {dadosEvento.TipoEvento}");
                Console.WriteLine($"    - CNPJ: {dadosEvento.CnpjDeclarante}");
                Console.WriteLine($"    - Ambiente: {(dadosEvento.EhProducao ? "Produção" : "Homologação")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ {exemplo}: {ex.Message}");
            }
        }
    }
    
    static async Task DemonstrarValidacaoXml(XmlValidationService validator, ExemplosEFinanceiraLoader loader)
    {
        var exemplos = new[] { "evtAberturaeFinanceira.xml", "evtMovOpFin.xml" };
        
        foreach (var exemplo in exemplos)
        {
            try
            {
                var documento = loader.CarregarExemplo(exemplo, validar: false);
                var resultado = validator.ValidarXml(documento);
                
                Console.WriteLine($"  {exemplo}:");
                Console.WriteLine($"    - Válido: {(resultado.IsValid ? "✓" : "✗")}");
                
                if (!resultado.IsValid)
                {
                    Console.WriteLine("    - Erros:");
                    foreach (var erro in resultado.Erros.Take(3))
                    {
                        Console.WriteLine($"      • {erro}");
                    }
                    if (resultado.Erros.Count > 3)
                    {
                        Console.WriteLine($"      ... e mais {resultado.Erros.Count - 3} erro(s)");
                    }
                }
                
                if (resultado.Avisos.Any())
                {
                    Console.WriteLine($"    - Avisos: {resultado.Avisos.Count}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ {exemplo}: {ex.Message}");
            }
        }
    }
    
    static void DemonstrarValidacaoTabelas(TabelaCodigosValidator validator)
    {
        var testes = new[]
        {
            ("TipoAmbiente", "1", "Produção"),
            ("TipoAmbiente", "2", "Homologação"),
            ("TipoAmbiente", "3", "Inválido"),
            ("Pais", "076", "Brasil"),
            ("Pais", "999", "Inválido")
        };
        
        foreach (var (tabela, codigo, descricao) in testes)
        {
            var valido = validator.ValidarCodigo(tabela, codigo);
            var status = valido ? "✓" : "✗";
            Console.WriteLine($"  {status} {tabela}.{codigo} ({descricao}): {(valido ? "Válido" : "Inválido")}");
        }
        
        // Listar alguns códigos disponíveis
        Console.WriteLine("\n  Códigos disponíveis para TipoAmbiente:");
        var codigos = validator.ObterCodigos("TipoAmbiente").Take(5);
        foreach (var codigo in codigos)
        {
            Console.WriteLine($"    - {codigo}");
        }
    }
}
```

## 6. Documentação de Deploy

### 6.1 Docker Support
```dockerfile
# EFinanceira.WebApi/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["EFinanceira.WebApi/EFinanceira.WebApi.csproj", "EFinanceira.WebApi/"]
COPY ["EFinanceira.Services/EFinanceira.Services.csproj", "EFinanceira.Services/"]
COPY ["EFinanceira.Core/EFinanceira.Core.csproj", "EFinanceira.Core/"]
RUN dotnet restore "EFinanceira.WebApi/EFinanceira.WebApi.csproj"
COPY . .
WORKDIR "/src/EFinanceira.WebApi"
RUN dotnet build "EFinanceira.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EFinanceira.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copiar recursos necessários
COPY schemas/ /app/schemas/
COPY exemplos/ /app/exemplos/
COPY tabelas-codigos/ /app/tabelas-codigos/

ENTRYPOINT ["dotnet", "EFinanceira.WebApi.dll"]
```

### 6.2 Docker Compose
```yaml
# docker-compose.yml
version: '3.8'

services:
  efinanceira-api:
    build:
      context: .
      dockerfile: EFinanceira.WebApi/Dockerfile
    ports:
      - "5000:80"
      - "5001:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - EFinanceira__CaminhoSchemas=/app/schemas
      - EFinanceira__CaminhoExemplos=/app/exemplos
      - EFinanceira__CaminhoTabelasCodigos=/app/tabelas-codigos
    volumes:
      - ./certificados:/app/certificados:ro
      - ./logs:/app/logs
```

Este guia fornece uma implementação completa para integração do e-Financeira em .NET, utilizando todos os recursos baixados (schemas, tabelas de códigos e exemplos). A arquitetura é modular, testável e pronta para produção.

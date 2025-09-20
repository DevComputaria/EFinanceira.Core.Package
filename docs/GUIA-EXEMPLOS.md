# Guia Prático: Usando Exemplos do e-Financeira em .NET

Este guia mostra como utilizar os exemplos de arquivos XML do e-Financeira em aplicações .NET.

## Estrutura dos Exemplos XML

### Formato Básico dos Eventos
Todos os eventos seguem uma estrutura similar:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<eFinanceira xmlns="http://www.eFinanceira.gov.br/schemas/[TIPO_EVENTO]/[VERSAO]">
  <[NOME_EVENTO] id="ID000000000000000001">
    <ideEvento>
      <indRetificacao>1</indRetificacao>
      <tpAmb>1</tpAmb>
      <aplicEmi>2</aplicEmi>
      <verAplic>00000000000000000001</verAplic>
    </ideEvento>
    <ideDeclarante>
      <cnpjDeclarante>11111111111111</cnpjDeclarante>
    </ideDeclarante>
    <!-- Conteúdo específico do evento -->
  </[NOME_EVENTO]>
</eFinanceira>
```

## Carregando e Validando Exemplos

### 1. Classe Helper para Carregar Exemplos
```csharp
public class ExemplosEFinanceiraLoader
{
    private readonly string _caminhoExemplos;
    private readonly XmlSchemaSet _schemas;
    
    public ExemplosEFinanceiraLoader(string caminhoExemplos, string caminhoSchemas)
    {
        _caminhoExemplos = caminhoExemplos;
        _schemas = CarregarSchemas(caminhoSchemas);
    }
    
    public XDocument CarregarExemplo(string nomeArquivo, bool validar = true)
    {
        var caminhoCompleto = Path.Combine(_caminhoExemplos, "xml-sem-assinatura", nomeArquivo);
        
        if (!File.Exists(caminhoCompleto))
            throw new FileNotFoundException($"Exemplo não encontrado: {nomeArquivo}");
        
        var documento = XDocument.Load(caminhoCompleto);
        
        if (validar)
        {
            ValidarDocumento(documento);
        }
        
        return documento;
    }
    
    public T CarregarExemplo<T>(string nomeArquivo) where T : class
    {
        var documento = CarregarExemplo(nomeArquivo);
        return DeserializarDocumento<T>(documento);
    }
    
    private void ValidarDocumento(XDocument documento)
    {
        var erros = new List<string>();
        
        documento.Validate(_schemas, (sender, e) =>
        {
            erros.Add($"Linha {e.Exception?.LineNumber}: {e.Message}");
        });
        
        if (erros.Any())
        {
            throw new XmlSchemaValidationException($"Documento inválido:\n{string.Join("\n", erros)}");
        }
    }
    
    private T DeserializarDocumento<T>(XDocument documento) where T : class
    {
        var serializer = new XmlSerializer(typeof(T));
        using var reader = documento.CreateReader();
        return (T)serializer.Deserialize(reader);
    }
    
    private XmlSchemaSet CarregarSchemas(string caminhoSchemas)
    {
        var schemas = new XmlSchemaSet();
        var arquivosXsd = Directory.GetFiles(caminhoSchemas, "*.xsd");
        
        foreach (var arquivo in arquivosXsd)
        {
            schemas.Add(null, arquivo);
        }
        
        return schemas;
    }
}
```

### 2. Trabalhando com Exemplos Específicos

#### Exemplo: Carregando Evento de Abertura
```csharp
public class ExemploAberturaService
{
    private readonly ExemplosEFinanceiraLoader _loader;
    
    public ExemploAberturaService(ExemplosEFinanceiraLoader loader)
    {
        _loader = loader;
    }
    
    public void AnalisarExemploAbertura()
    {
        // Carregar exemplo como XDocument
        var documento = _loader.CarregarExemplo("evtAberturaeFinanceira.xml");
        
        // Extrair informações básicas
        var ns = documento.Root.Name.Namespace;
        var evento = documento.Root.Element(ns + "evtAberturaeFinanceira");
        var ideEvento = evento.Element(ns + "ideEvento");
        
        Console.WriteLine($"Tipo de Ambiente: {ideEvento.Element(ns + "tpAmb").Value}");
        Console.WriteLine($"Retificação: {ideEvento.Element(ns + "indRetificacao").Value}");
        
        // Ou carregar como objeto tipado (se você tiver as classes geradas)
        // var eventoTipado = _loader.CarregarExemplo<evtAberturaeFinanceira>("evtAberturaeFinanceira.xml");
    }
}
```

#### Exemplo: Analisando Movimentação Financeira
```csharp
public class ExemploMovimentacaoService
{
    private readonly ExemplosEFinanceiraLoader _loader;
    
    public ExemploMovimentacaoService(ExemplosEFinanceiraLoader loader)
    {
        _loader = loader;
    }
    
    public void AnalisarExemploMovimentacao()
    {
        var documento = _loader.CarregarExemplo("evtMovOpFin.xml");
        var ns = documento.Root.Name.Namespace;
        var evento = documento.Root.Element(ns + "evtMovOpFin");
        
        // Extrair informações do declarado
        var ideDeclarado = evento.Element(ns + "ideDeclarado");
        var tipoNI = ideDeclarado.Element(ns + "tpNI").Value;
        
        Console.WriteLine($"Tipo de NI: {tipoNI}");
        
        // Analisar movimentações
        var movOpFin = evento.Element(ns + "movOpFin");
        var reportaveis = movOpFin.Elements(ns + "reportavel");
        
        Console.WriteLine($"Número de reportáveis: {reportaveis.Count()}");
        
        foreach (var reportavel in reportaveis)
        {
            var tipoReportavel = reportavel.Element(ns + "tpReportavel").Value;
            var infoConta = reportavel.Element(ns + "infoConta");
            var tpConta = infoConta?.Element(ns + "tpConta")?.Value;
            
            Console.WriteLine($"  - Tipo Reportável: {tipoReportavel}, Tipo Conta: {tpConta}");
        }
    }
}
```

### 3. Comparando Exemplos com e sem Assinatura

```csharp
public class ComparadorExemplos
{
    private readonly ExemplosEFinanceiraLoader _loader;
    
    public ComparadorExemplos(ExemplosEFinanceiraLoader loader)
    {
        _loader = loader;
    }
    
    public void CompararExemplos()
    {
        // Carregar exemplo sem assinatura
        var exemploSemAssinatura = _loader.CarregarExemplo("evtAberturaeFinanceira.xml");
        
        // Carregar exemplo com assinatura
        var caminhoComAssinatura = Path.Combine(_loader.CaminhoExemplos, "xml-com-assinatura", "evtAberturaeFinanceira_assinado_lote.xml");
        var exemploComAssinatura = XDocument.Load(caminhoComAssinatura);
        
        Console.WriteLine("=== Comparação de Estruturas ===");
        
        // Analisar estrutura sem assinatura
        Console.WriteLine($"Sem Assinatura - Root: {exemploSemAssinatura.Root.Name.LocalName}");
        var eventoSemAssinatura = exemploSemAssinatura.Root.Elements().First();
        Console.WriteLine($"Sem Assinatura - Evento: {eventoSemAssinatura.Name.LocalName}");
        
        // Analisar estrutura com assinatura e lote
        Console.WriteLine($"Com Assinatura - Root: {exemploComAssinatura.Root.Name.LocalName}");
        var loteEventos = exemploComAssinatura.Root.Element(exemploComAssinatura.Root.Name.Namespace + "loteEventos");
        var evento = loteEventos.Element(loteEventos.Name.Namespace + "evento");
        var eventoComAssinatura = evento.Elements().First(e => e.Name.LocalName.StartsWith("evt"));
        
        Console.WriteLine($"Com Assinatura - Evento dentro do lote: {eventoComAssinatura.Name.LocalName}");
        
        // Verificar presença de assinatura
        var assinatura = eventoComAssinatura.Element(XNamespace.Get("http://www.w3.org/2000/09/xmldsig#") + "Signature");
        Console.WriteLine($"Possui assinatura digital: {assinatura != null}");
    }
}
```

## Extraindo Dados dos Exemplos

### 1. Extrair Informações Comuns
```csharp
public class ExtractorDadosComuns
{
    public static DadosEvento ExtrairDadosEvento(XDocument documento)
    {
        var ns = documento.Root.Name.Namespace;
        var primeiroEvento = documento.Root.Elements().First();
        var ideEvento = primeiroEvento.Element(ns + "ideEvento");
        var ideDeclarante = primeiroEvento.Element(ns + "ideDeclarante");
        
        return new DadosEvento
        {
            TipoEvento = primeiroEvento.Name.LocalName,
            Id = primeiroEvento.Attribute("id")?.Value,
            IndRetificacao = ideEvento?.Element(ns + "indRetificacao")?.Value,
            TipoAmbiente = ideEvento?.Element(ns + "tpAmb")?.Value,
            CnpjDeclarante = ideDeclarante?.Element(ns + "cnpjDeclarante")?.Value,
            AplicacaoEmissora = ideEvento?.Element(ns + "aplicEmi")?.Value,
            VersaoAplicacao = ideEvento?.Element(ns + "verAplic")?.Value
        };
    }
}

public class DadosEvento
{
    public string TipoEvento { get; set; }
    public string Id { get; set; }
    public string IndRetificacao { get; set; }
    public string TipoAmbiente { get; set; }
    public string CnpjDeclarante { get; set; }
    public string AplicacaoEmissora { get; set; }
    public string VersaoAplicacao { get; set; }
    
    public bool EhRetificacao => IndRetificacao == "2";
    public bool EhProducao => TipoAmbiente == "1";
    public bool EhHomologacao => TipoAmbiente == "2";
}
```

### 2. Analisador de Padrões nos Exemplos
```csharp
public class AnalisadorPadroesExemplos
{
    private readonly ExemplosEFinanceiraLoader _loader;
    
    public AnalisadorPadroesExemplos(ExemplosEFinanceiraLoader loader)
    {
        _loader = loader;
    }
    
    public void AnalisarTodosExemplos()
    {
        var exemplos = Directory.GetFiles(
            Path.Combine(_loader.CaminhoExemplos, "xml-sem-assinatura"), 
            "*.xml"
        );
        
        var padroes = new List<PadraoExemplo>();
        
        foreach (var arquivo in exemplos)
        {
            try
            {
                var documento = XDocument.Load(arquivo);
                var dados = ExtractorDadosComuns.ExtrairDadosEvento(documento);
                
                padroes.Add(new PadraoExemplo
                {
                    NomeArquivo = Path.GetFileName(arquivo),
                    TipoEvento = dados.TipoEvento,
                    TamanhoBytes = new FileInfo(arquivo).Length,
                    Namespace = documento.Root.Name.Namespace.NamespaceName,
                    TemAssinatura = documento.Descendants(XNamespace.Get("http://www.w3.org/2000/09/xmldsig#") + "Signature").Any(),
                    DadosEvento = dados
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar {arquivo}: {ex.Message}");
            }
        }
        
        // Gerar relatório
        GerarRelatorio(padroes);
    }
    
    private void GerarRelatorio(List<PadraoExemplo> padroes)
    {
        Console.WriteLine("=== RELATÓRIO DE ANÁLISE DOS EXEMPLOS ===\n");
        
        Console.WriteLine("Tipos de Eventos:");
        var tiposEventos = padroes.GroupBy(p => p.TipoEvento).OrderBy(g => g.Key);
        foreach (var grupo in tiposEventos)
        {
            Console.WriteLine($"  - {grupo.Key}: {grupo.Count()} exemplo(s)");
        }
        
        Console.WriteLine("\nNamespaces utilizados:");
        var namespaces = padroes.GroupBy(p => p.Namespace).OrderBy(g => g.Key);
        foreach (var grupo in namespaces)
        {
            Console.WriteLine($"  - {grupo.Key}");
        }
        
        Console.WriteLine("\nTamanhos dos arquivos:");
        foreach (var padrao in padroes.OrderBy(p => p.TamanhoBytes))
        {
            Console.WriteLine($"  - {padrao.NomeArquivo}: {padrao.TamanhoBytes:N0} bytes");
        }
        
        Console.WriteLine("\nAmbiente de teste:");
        var ambientes = padroes.GroupBy(p => p.DadosEvento.TipoAmbiente);
        foreach (var grupo in ambientes)
        {
            var descricao = grupo.Key == "1" ? "Produção" : "Homologação";
            Console.WriteLine($"  - {descricao}: {grupo.Count()} exemplo(s)");
        }
    }
}

public class PadraoExemplo
{
    public string NomeArquivo { get; set; }
    public string TipoEvento { get; set; }
    public long TamanhoBytes { get; set; }
    public string Namespace { get; set; }
    public bool TemAssinatura { get; set; }
    public DadosEvento DadosEvento { get; set; }
}
```

## Testes Unitários com Exemplos

### 1. Testes de Validação
```csharp
[TestFixture]
public class TestesValidacaoExemplos
{
    private ExemplosEFinanceiraLoader _loader;
    
    [SetUp]
    public void Setup()
    {
        var caminhoExemplos = @"C:\caminho\para\exemplos";
        var caminhoSchemas = @"C:\caminho\para\schemas";
        _loader = new ExemplosEFinanceiraLoader(caminhoExemplos, caminhoSchemas);
    }
    
    [Test]
    [TestCase("evtAberturaeFinanceira.xml")]
    [TestCase("evtMovOpFin.xml")]
    [TestCase("evtFechamentoeFinanceira.xml")]
    public void DevemSerValidosContraSchema(string nomeArquivo)
    {
        Assert.DoesNotThrow(() => _loader.CarregarExemplo(nomeArquivo, validar: true));
    }
    
    [Test]
    public void ExemploAbertura_DeveConterInformacoesObrigatorias()
    {
        var documento = _loader.CarregarExemplo("evtAberturaeFinanceira.xml");
        var dados = ExtractorDadosComuns.ExtrairDadosEvento(documento);
        
        Assert.IsNotNull(dados.TipoEvento);
        Assert.IsNotNull(dados.Id);
        Assert.IsNotNull(dados.IndRetificacao);
        Assert.IsNotNull(dados.TipoAmbiente);
        Assert.IsNotNull(dados.CnpjDeclarante);
    }
}
```

### 2. Testes de Serialização/Deserialização
```csharp
[TestFixture]
public class TestesSerializacaoExemplos
{
    [Test]
    public void DeveSerializarEDeserializarExemploAbertura()
    {
        // Carregar exemplo original
        var caminhoOriginal = @"exemplos\xml-sem-assinatura\evtAberturaeFinanceira.xml";
        var xmlOriginal = File.ReadAllText(caminhoOriginal);
        
        // Deserializar para objeto (assumindo que você tem as classes geradas)
        var serializer = new XmlSerializer(typeof(eFinanceira));
        eFinanceira evento;
        
        using (var reader = new StringReader(xmlOriginal))
        {
            evento = (eFinanceira)serializer.Deserialize(reader);
        }
        
        // Serializar de volta para XML
        string xmlSerializado;
        using (var writer = new StringWriter())
        {
            serializer.Serialize(writer, evento);
            xmlSerializado = writer.ToString();
        }
        
        // Validar que o XML serializado é válido
        Assert.DoesNotThrow(() => XDocument.Parse(xmlSerializado));
        
        // Opcional: comparar estruturas (removendo diferenças de formatação)
        var docOriginal = XDocument.Parse(xmlOriginal);
        var docSerializado = XDocument.Parse(xmlSerializado);
        
        // Aqui você pode implementar comparações específicas
        Assert.AreEqual(docOriginal.Root.Name, docSerializado.Root.Name);
    }
}
```

## Configuração de Injeção de Dependência

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExemplosEFinanceira(
        this IServiceCollection services, 
        string caminhoExemplos, 
        string caminhoSchemas)
    {
        services.AddSingleton(provider => 
            new ExemplosEFinanceiraLoader(caminhoExemplos, caminhoSchemas));
        
        services.AddScoped<ExemploAberturaService>();
        services.AddScoped<ExemploMovimentacaoService>();
        services.AddScoped<ComparadorExemplos>();
        services.AddScoped<AnalisadorPadroesExemplos>();
        
        return services;
    }
}

// No Startup.cs ou Program.cs
public void ConfigureServices(IServiceCollection services)
{
    var caminhoExemplos = Configuration.GetValue<string>("EFinanceira:CaminhoExemplos");
    var caminhoSchemas = Configuration.GetValue<string>("EFinanceira:CaminhoSchemas");
    
    services.AddExemplosEFinanceira(caminhoExemplos, caminhoSchemas);
}
```

## Melhores Práticas

1. **Sempre valide** os exemplos contra os schemas XSD
2. **Use os exemplos** como base para testes unitários
3. **Analise as diferenças** entre versões com e sem assinatura
4. **Extraia padrões** comuns para criar templates
5. **Mantenha** os exemplos atualizados com as versões mais recentes
6. **Documente** as particularidades encontradas em cada exemplo
7. **Use** os códigos fonte em ZIP para implementar assinatura digital

Este guia fornece uma base sólida para trabalhar com os exemplos do e-Financeira em suas aplicações .NET.

# Guia Prático: Utilizando Tabelas de Códigos do e-Financeira em .NET

Este guia mostra como importar e utilizar as tabelas de códigos do e-Financeira em aplicações .NET.

## Estrutura dos Arquivos de Tabela

### Formato Padrão
Todas as tabelas seguem um formato similar:
```
versão=X CAMPO1, CAMPO2, CAMPO3, CAMPO4
VALOR1|VALOR2|VALOR3|VALOR4
```

### Exemplo: Tipos de Conta
```
versão=3 COD_TIPO, NOM_TIPO, DT_INI, DT_FIM
1|Conta de Depósito|01012014|
2|Conta de Custódia|01012014|
3|Conta de Investimento|01012014|
4|Conta de Seguro|01012014|
5|Consórcio|01012014|
```

## Classes de Modelo

### 1. Classe Base para Tabelas de Código
```csharp
public abstract class TabelaCodigoBase
{
    public string Codigo { get; set; }
    public string Descricao { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    
    public bool EstaAtivo(DateTime? data = null)
    {
        var dataVerificacao = data ?? DateTime.Now;
        return dataVerificacao >= DataInicio && 
               (DataFim == null || dataVerificacao <= DataFim);
    }
}
```

### 2. Classes Específicas
```csharp
public class Pais : TabelaCodigoBase
{
    // COD_PAIS, NOM_PAIS, DT_INI, DT_FIM
}

public class Municipio : TabelaCodigoBase
{
    // COD_MUNICIPIO, NOM_MUNICIPIO, COD_UF, DT_INI, DT_FIM
    public string CodigoUF { get; set; }
}

public class TipoConta : TabelaCodigoBase
{
    // COD_TIPO, NOM_TIPO, DT_INI, DT_FIM
}

public class Moeda : TabelaCodigoBase
{
    // SIMB_MOEDA, NOM_MOEDA, DT_INI, DT_FIM
}

public class TipoDeclarado : TabelaCodigoBase
{
    // COD_TIPO, NOM_TIPO, DT_INI, DT_FIM
}
```

## Serviço de Importação

### Parser Genérico para Tabelas
```csharp
public class TabelaCodigosParser
{
    public List<T> ParseTabela<T>(string caminhoArquivo, Func<string[], T> factory) 
        where T : TabelaCodigoBase
    {
        var resultado = new List<T>();
        var linhas = File.ReadAllLines(caminhoArquivo, Encoding.UTF8);
        
        if (linhas.Length < 2) return resultado;
        
        // Pular primeira linha (cabeçalho com versão)
        for (int i = 1; i < linhas.Length; i++)
        {
            var linha = linhas[i].Trim();
            if (string.IsNullOrEmpty(linha)) continue;
            
            var campos = linha.Split('|');
            if (campos.Length >= 3)
            {
                try
                {
                    var item = factory(campos);
                    if (item != null)
                        resultado.Add(item);
                }
                catch (Exception ex)
                {
                    // Log erro na linha
                    Console.WriteLine($"Erro ao processar linha {i}: {ex.Message}");
                }
            }
        }
        
        return resultado;
    }
    
    private DateTime ParseData(string data)
    {
        if (string.IsNullOrEmpty(data)) 
            return DateTime.MinValue;
            
        if (DateTime.TryParseExact(data, "ddMMyyyy", null, DateTimeStyles.None, out var result))
            return result;
            
        return DateTime.MinValue;
    }
    
    private DateTime? ParseDataOpcional(string data)
    {
        if (string.IsNullOrEmpty(data)) 
            return null;
            
        var resultado = ParseData(data);
        return resultado == DateTime.MinValue ? null : resultado;
    }
}
```

### Factories Específicas
```csharp
public class TabelaCodigosFactories
{
    public static Pais CreatePais(string[] campos)
    {
        return new Pais
        {
            Codigo = campos[0],
            Descricao = campos[1],
            DataInicio = ParseData(campos[2]),
            DataFim = ParseDataOpcional(campos[3])
        };
    }
    
    public static Municipio CreateMunicipio(string[] campos)
    {
        return new Municipio
        {
            Codigo = campos[0],
            Descricao = campos[1],
            CodigoUF = campos[2],
            DataInicio = ParseData(campos[3]),
            DataFim = ParseDataOpcional(campos[4])
        };
    }
    
    public static TipoConta CreateTipoConta(string[] campos)
    {
        return new TipoConta
        {
            Codigo = campos[0],
            Descricao = campos[1],
            DataInicio = ParseData(campos[2]),
            DataFim = ParseDataOpcional(campos[3])
        };
    }
    
    public static Moeda CreateMoeda(string[] campos)
    {
        return new Moeda
        {
            Codigo = campos[0],
            Descricao = campos[1],
            DataInicio = ParseData(campos[2]),
            DataFim = ParseDataOpcional(campos[3])
        };
    }
    
    private static DateTime ParseData(string data)
    {
        if (string.IsNullOrEmpty(data)) 
            return DateTime.MinValue;
            
        if (DateTime.TryParseExact(data, "ddMMyyyy", null, DateTimeStyles.None, out var result))
            return result;
            
        return DateTime.MinValue;
    }
    
    private static DateTime? ParseDataOpcional(string data)
    {
        if (string.IsNullOrEmpty(data)) 
            return null;
            
        var resultado = ParseData(data);
        return resultado == DateTime.MinValue ? null : resultado;
    }
}
```

## Serviço de Carregamento e Cache

```csharp
public class TabelaCodigosService
{
    private readonly string _caminhoTabelas;
    private readonly TabelaCodigosParser _parser;
    
    // Cache em memória
    private List<Pais> _paises;
    private List<Municipio> _municipios;
    private List<TipoConta> _tiposContas;
    private List<Moeda> _moedas;
    
    public TabelaCodigosService(string caminhoTabelas)
    {
        _caminhoTabelas = caminhoTabelas;
        _parser = new TabelaCodigosParser();
    }
    
    public List<Pais> GetPaises()
    {
        if (_paises == null)
        {
            var arquivo = Path.Combine(_caminhoTabelas, "Paises.txt");
            _paises = _parser.ParseTabela(arquivo, TabelaCodigosFactories.CreatePais);
        }
        return _paises;
    }
    
    public List<Municipio> GetMunicipios()
    {
        if (_municipios == null)
        {
            var arquivo = Path.Combine(_caminhoTabelas, "Municipios.txt");
            _municipios = _parser.ParseTabela(arquivo, TabelaCodigosFactories.CreateMunicipio);
        }
        return _municipios;
    }
    
    public List<TipoConta> GetTiposContas()
    {
        if (_tiposContas == null)
        {
            var arquivo = Path.Combine(_caminhoTabelas, "Tipos_de_Conta.txt");
            _tiposContas = _parser.ParseTabela(arquivo, TabelaCodigosFactories.CreateTipoConta);
        }
        return _tiposContas;
    }
    
    public List<Moeda> GetMoedas()
    {
        if (_moedas == null)
        {
            var arquivo = Path.Combine(_caminhoTabelas, "Moedas.txt");
            _moedas = _parser.ParseTabela(arquivo, TabelaCodigosFactories.CreateMoeda);
        }
        return _moedas;
    }
}
```

## Serviço de Validação

```csharp
public class ValidadorCodigosEFinanceira
{
    private readonly TabelaCodigosService _tabelasService;
    
    public ValidadorCodigosEFinanceira(TabelaCodigosService tabelasService)
    {
        _tabelasService = tabelasService;
    }
    
    public ValidationResult ValidarPais(string codigoPais, DateTime? data = null)
    {
        var paises = _tabelasService.GetPaises();
        var pais = paises.FirstOrDefault(p => p.Codigo == codigoPais);
        
        if (pais == null)
            return ValidationResult.Error($"País com código '{codigoPais}' não encontrado");
            
        if (!pais.EstaAtivo(data))
            return ValidationResult.Error($"País '{pais.Descricao}' não está ativo na data especificada");
            
        return ValidationResult.Success();
    }
    
    public ValidationResult ValidarMunicipio(string codigoMunicipio, DateTime? data = null)
    {
        var municipios = _tabelasService.GetMunicipios();
        var municipio = municipios.FirstOrDefault(m => m.Codigo == codigoMunicipio);
        
        if (municipio == null)
            return ValidationResult.Error($"Município com código '{codigoMunicipio}' não encontrado");
            
        if (!municipio.EstaAtivo(data))
            return ValidationResult.Error($"Município '{municipio.Descricao}' não está ativo na data especificada");
            
        return ValidationResult.Success();
    }
    
    public ValidationResult ValidarTipoConta(string codigoTipo, DateTime? data = null)
    {
        var tipos = _tabelasService.GetTiposContas();
        var tipo = tipos.FirstOrDefault(t => t.Codigo == codigoTipo);
        
        if (tipo == null)
            return ValidationResult.Error($"Tipo de conta '{codigoTipo}' não encontrado");
            
        if (!tipo.EstaAtivo(data))
            return ValidationResult.Error($"Tipo de conta '{tipo.Descricao}' não está ativo na data especificada");
            
        return ValidationResult.Success();
    }
    
    public ValidationResult ValidarMoeda(string codigoMoeda, DateTime? data = null)
    {
        var moedas = _tabelasService.GetMoedas();
        var moeda = moedas.FirstOrDefault(m => m.Codigo == codigoMoeda);
        
        if (moeda == null)
            return ValidationResult.Error($"Moeda '{codigoMoeda}' não encontrada");
            
        if (!moeda.EstaAtivo(data))
            return ValidationResult.Error($"Moeda '{moeda.Descricao}' não está ativa na data especificada");
            
        return ValidationResult.Success();
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; }
    
    public static ValidationResult Success() => new ValidationResult { IsValid = true };
    public static ValidationResult Error(string message) => new ValidationResult { IsValid = false, ErrorMessage = message };
}
```

## Integração com Entity Framework

```csharp
public class EFinanceiraDbContext : DbContext
{
    public DbSet<Pais> Paises { get; set; }
    public DbSet<Municipio> Municipios { get; set; }
    public DbSet<TipoConta> TiposContas { get; set; }
    public DbSet<Moeda> Moedas { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuração das entidades
        modelBuilder.Entity<Pais>(entity =>
        {
            entity.HasKey(e => e.Codigo);
            entity.Property(e => e.Codigo).HasMaxLength(2);
            entity.Property(e => e.Descricao).HasMaxLength(100);
        });
        
        modelBuilder.Entity<Municipio>(entity =>
        {
            entity.HasKey(e => e.Codigo);
            entity.Property(e => e.Codigo).HasMaxLength(7);
            entity.Property(e => e.Descricao).HasMaxLength(100);
            entity.Property(e => e.CodigoUF).HasMaxLength(2);
        });
        
        // ... outras configurações
    }
}
```

## Exemplo de Uso Completo

```csharp
public class ExemploUso
{
    public async Task ExemploValidacao()
    {
        // Configurar serviços
        var tabelasService = new TabelaCodigosService(@"C:\caminho\para\tabelas-codigos");
        var validador = new ValidadorCodigosEFinanceira(tabelasService);
        
        // Validar país
        var resultadoPais = validador.ValidarPais("BR", DateTime.Now);
        if (!resultadoPais.IsValid)
        {
            Console.WriteLine($"Erro: {resultadoPais.ErrorMessage}");
            return;
        }
        
        // Validar tipo de conta
        var resultadoConta = validador.ValidarTipoConta("1", DateTime.Now);
        if (!resultadoConta.IsValid)
        {
            Console.WriteLine($"Erro: {resultadoConta.ErrorMessage}");
            return;
        }
        
        // Buscar municípios de SP
        var municipios = tabelasService.GetMunicipios();
        var municipiosSP = municipios.Where(m => m.CodigoUF == "35").ToList();
        
        Console.WriteLine($"Encontrados {municipiosSP.Count} municípios em SP");
    }
}
```

## Configuração de Injeção de Dependência

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Configurar caminho das tabelas
    var caminhoTabelas = Configuration.GetValue<string>("EFinanceira:CaminhoTabelas");
    
    services.AddSingleton<TabelaCodigosService>(provider => 
        new TabelaCodigosService(caminhoTabelas));
    
    services.AddScoped<ValidadorCodigosEFinanceira>();
}
```

## Boas Práticas

1. **Cache**: Mantenha as tabelas em cache para evitar múltiplas leituras
2. **Encoding**: Use UTF-8 para leitura dos arquivos
3. **Validação de Data**: Sempre considere as datas de início e fim dos códigos
4. **Tratamento de Erro**: Implemente tratamento robusto de erros
5. **Atualização**: Crie processo para atualizar as tabelas periodicamente
6. **Performance**: Para grandes volumes, considere usar um banco de dados
7. **Logs**: Mantenha logs das validações para auditoria

Este guia fornece uma base sólida para integrar as tabelas de códigos do e-Financeira em suas aplicações .NET.

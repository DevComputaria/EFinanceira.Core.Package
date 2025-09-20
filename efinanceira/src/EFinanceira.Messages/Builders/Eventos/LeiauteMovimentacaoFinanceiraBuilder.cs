using EFinanceira.Core.Abstractions;

namespace EFinanceira.Messages.Builders.Eventos;

/// <summary>
/// Mensagem do evento de Movimento de Operações Financeiras
/// </summary>
public sealed class LeiauteMovimentacaoFinanceiraMessage : IEFinanceiraMessage
{
    public string Version { get; }
    public string RootElementName => "evtMovimentacaoFinanceira";
    public string? IdAttributeName => "Id";
    public string? IdValue { get; internal set; }
    public object Payload => Evento;

    /// <summary>
    /// Evento tipado (será substituído por POCO gerado do XSD)
    /// </summary>
    public MovimentacaoFinanceiraEvento Evento { get; }

    internal LeiauteMovimentacaoFinanceiraMessage(MovimentacaoFinanceiraEvento evento, string version)
    {
        Evento = evento;
        Version = version;
        IdValue = evento.Id;
    }
}

/// <summary>
/// Builder fluente para evento de Movimento de Operações Financeiras
/// </summary>
public sealed class LeiauteMovimentacaoFinanceiraBuilder : IMessageBuilder<LeiauteMovimentacaoFinanceiraMessage>
{
    private readonly MovimentacaoFinanceiraEvento _evento;
    private readonly string _version;

    public LeiauteMovimentacaoFinanceiraBuilder(string version = "v1_2_1")
    {
        _version = version;
        _evento = new MovimentacaoFinanceiraEvento
        {
            Id = GenerateId(),
            IdeEvento = new IdeEvento
            {
                IndRetificacao = 1, // Original
                TpAmb = 2, // Homologação
                AplicEmi = 1, // Aplicativo do contribuinte
                VerAplic = "1.0.0"
            }
        };
    }

    /// <summary>
    /// Define o ID do evento
    /// </summary>
    public LeiauteMovimentacaoFinanceiraBuilder WithId(string id)
    {
        _evento.Id = id;
        return this;
    }

    /// <summary>
    /// Define os dados do declarante
    /// </summary>
    public LeiauteMovimentacaoFinanceiraBuilder WithDeclarante(string cnpj)
    {
        _evento.IdeDeclarante = new IdeDeclarante
        {
            CnpjDeclarante = cnpj
        };
        return this;
    }

    /// <summary>
    /// Define o tipo de ambiente (1=Produção, 2=Homologação)
    /// </summary>
    public LeiauteMovimentacaoFinanceiraBuilder WithAmbiente(int tipoAmbiente)
    {
        _evento.IdeEvento.TpAmb = tipoAmbiente;
        return this;
    }

    /// <summary>
    /// Define se é retificação (1=Original, 2=Retificação)
    /// </summary>
    public LeiauteMovimentacaoFinanceiraBuilder WithRetificacao(int indRetificacao)
    {
        _evento.IdeEvento.IndRetificacao = indRetificacao;
        return this;
    }

    /// <summary>
    /// Define a versão da aplicação
    /// </summary>
    public LeiauteMovimentacaoFinanceiraBuilder WithVersaoAplicacao(string versao)
    {
        _evento.IdeEvento.VerAplic = versao;
        return this;
    }

    /// <summary>
    /// Adiciona dados do declarado
    /// </summary>
    public LeiauteMovimentacaoFinanceiraBuilder WithDeclarado(Action<IdeDeclaradoBuilder> configurar)
    {
        var builder = new IdeDeclaradoBuilder();
        configurar(builder);
        _evento.IdeDeclarado = builder.Build();
        return this;
    }

    /// <summary>
    /// Adiciona movimentação financeira
    /// </summary>
    public LeiauteMovimentacaoFinanceiraBuilder WithMovimentacao(Action<MovimentacaoFinanceiraBuilder> configurar)
    {
        var builder = new MovimentacaoFinanceiraBuilder();
        configurar(builder);
        _evento.MovimentacaoFinanceira = builder.Build();
        return this;
    }

    /// <inheritdoc />
    public LeiauteMovimentacaoFinanceiraMessage Build()
    {
        ValidateRequiredFields();
        return new LeiauteMovimentacaoFinanceiraMessage(_evento, _version);
    }

    private void ValidateRequiredFields()
    {
        if (string.IsNullOrEmpty(_evento.Id))
            throw new InvalidOperationException("Id é obrigatório");

        if (_evento.IdeDeclarante?.CnpjDeclarante == null)
            throw new InvalidOperationException("CNPJ do declarante é obrigatório");

        if (_evento.IdeDeclarado == null)
            throw new InvalidOperationException("Dados do declarado são obrigatórios");

        if (_evento.MovimentacaoFinanceira == null)
            throw new InvalidOperationException("Movimentação financeira é obrigatória");
    }

    private static string GenerateId()
    {
        return $"ID{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
}

/// <summary>
/// Builder para dados do declarado
/// </summary>
public sealed class IdeDeclaradoBuilder
{
    private readonly IdeDeclarado _ideDeclarado = new();

    public IdeDeclaradoBuilder WithTipoNI(int tipoNI)
    {
        _ideDeclarado.TpNI = tipoNI;
        return this;
    }

    public IdeDeclaradoBuilder WithNumeroInscricao(string numeroInscricao)
    {
        _ideDeclarado.NumeroInscricao = numeroInscricao;
        return this;
    }

    public IdeDeclaradoBuilder WithNome(string nome)
    {
        _ideDeclarado.Nome = nome;
        return this;
    }

    public IdeDeclaradoBuilder WithEndereco(Action<EnderecoBuilder> configurar)
    {
        var builder = new EnderecoBuilder();
        configurar(builder);
        _ideDeclarado.Endereco = builder.Build();
        return this;
    }

    internal IdeDeclarado Build() => _ideDeclarado;
}

/// <summary>
/// Builder para endereço
/// </summary>
public sealed class EnderecoBuilder
{
    private readonly Endereco _endereco = new();

    public EnderecoBuilder WithPais(string pais)
    {
        _endereco.Pais = pais;
        return this;
    }

    public EnderecoBuilder WithEndereco(string endereco)
    {
        _endereco.EnderecoCompleto = endereco;
        return this;
    }

    public EnderecoBuilder WithCidade(string cidade)
    {
        _endereco.Cidade = cidade;
        return this;
    }

    internal Endereco Build() => _endereco;
}

/// <summary>
/// Builder para movimentação financeira
/// </summary>
public sealed class MovimentacaoFinanceiraBuilder
{
    private readonly MovimentacaoFinanceira _movimentacao = new();

    public MovimentacaoFinanceiraBuilder WithDataInicio(DateTime dataInicio)
    {
        _movimentacao.DataInicio = dataInicio;
        return this;
    }

    public MovimentacaoFinanceiraBuilder WithDataFim(DateTime dataFim)
    {
        _movimentacao.DataFim = dataFim;
        return this;
    }

    public MovimentacaoFinanceiraBuilder AdicionarReportavel(Action<ReportavelBuilder> configurar)
    {
        var builder = new ReportavelBuilder();
        configurar(builder);
        _movimentacao.Reportaveis.Add(builder.Build());
        return this;
    }

    internal MovimentacaoFinanceira Build() => _movimentacao;
}

/// <summary>
/// Builder para reportável
/// </summary>
public sealed class ReportavelBuilder
{
    private readonly Reportavel _reportavel = new();

    public ReportavelBuilder WithTipoReportavel(int tipoReportavel)
    {
        _reportavel.TpReportavel = tipoReportavel;
        return this;
    }

    public ReportavelBuilder WithInformacoesConta(Action<InfoContaBuilder> configurar)
    {
        var builder = new InfoContaBuilder();
        configurar(builder);
        _reportavel.InfoConta = builder.Build();
        return this;
    }

    internal Reportavel Build() => _reportavel;
}

/// <summary>
/// Builder para informações da conta
/// </summary>
public sealed class InfoContaBuilder
{
    private readonly InfoConta _infoConta = new();

    public InfoContaBuilder WithTipoConta(int tipoConta)
    {
        _infoConta.TpConta = tipoConta;
        return this;
    }

    public InfoContaBuilder WithSubTipoConta(string subTipoConta)
    {
        _infoConta.SubTpConta = subTipoConta;
        return this;
    }

    public InfoContaBuilder WithMoeda(string moeda)
    {
        _infoConta.Moeda = moeda;
        return this;
    }

    internal InfoConta Build() => _infoConta;
}

// Classes de modelo temporárias (serão substituídas pelos POCOs gerados do XSD)
#region Modelos Temporários

public class MovimentacaoFinanceiraEvento
{
    public string Id { get; set; } = string.Empty;
    public IdeEvento IdeEvento { get; set; } = new();
    public IdeDeclarante? IdeDeclarante { get; set; }
    public IdeDeclarado? IdeDeclarado { get; set; }
    public MovimentacaoFinanceira? MovimentacaoFinanceira { get; set; }
}

public class IdeEvento
{
    public int IndRetificacao { get; set; }
    public int TpAmb { get; set; }
    public int AplicEmi { get; set; }
    public string VerAplic { get; set; } = string.Empty;
}

public class IdeDeclarante
{
    public string CnpjDeclarante { get; set; } = string.Empty;
}

public class IdeDeclarado
{
    public int TpNI { get; set; }
    public string NumeroInscricao { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public Endereco? Endereco { get; set; }
}

public class Endereco
{
    public string Pais { get; set; } = string.Empty;
    public string EnderecoCompleto { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
}

public class MovimentacaoFinanceira
{
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public List<Reportavel> Reportaveis { get; set; } = new();
}

public class Reportavel
{
    public int TpReportavel { get; set; }
    public InfoConta? InfoConta { get; set; }
}

public class InfoConta
{
    public int TpConta { get; set; }
    public string SubTpConta { get; set; } = string.Empty;
    public string Moeda { get; set; } = string.Empty;
}

#endregion

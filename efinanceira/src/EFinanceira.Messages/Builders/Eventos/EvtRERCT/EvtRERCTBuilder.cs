using System;
using System.Collections.Generic;
using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Generated.Eventos.EvtRERCT;

namespace EFinanceira.Messages.Builders.Eventos.EvtRERCT;

/// <summary>
/// Mensagem de evento RERCT (Registro de Contas Exteriores e Transferências)
/// </summary>
public sealed class EvtRERCTMessage : IEFinanceiraMessage
{
    public string Version { get; private set; }
    public string RootElementName => "evtRERCT";
    public string? IdAttributeName => "id";
    public string? IdValue { get; internal set; }
    public object Payload => Evento;

    /// <summary>
    /// Evento tipado gerado do XSD
    /// </summary>
    public eFinanceiraEvtRERCT Evento { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EvtRERCTMessage"/> class.
    /// Construtor sem parâmetros necessário para serialização XML.
    /// </summary>
    public EvtRERCTMessage()
    {
        Version = "v1_2_0";
        Evento = new eFinanceiraEvtRERCT();
        IdValue = string.Empty;
    }

    internal EvtRERCTMessage(eFinanceiraEvtRERCT evento, string version)
    {
        Evento = evento;
        Version = version;
        IdValue = evento.id;
    }
}

/// <summary>
/// Builder principal para construção de eventos RERCT (Registro de Contas Exteriores e Transferências).
/// Utiliza o padrão fluent interface para facilitar a criação de eventos estruturados.
/// </summary>
public sealed class EvtRERCTBuilder : IMessageBuilder<EvtRERCTMessage>
{
    private readonly eFinanceiraEvtRERCT _evento;
    private readonly string _version;

    /// <summary>
    /// Initializes a new instance of the <see cref="EvtRERCTBuilder"/> class.
    /// </summary>
    /// <param name="version">Versão do schema (padrão: v1_2_0)</param>
    public EvtRERCTBuilder(string version = "v1_2_0")
    {
        _version = version;
        _evento = new eFinanceiraEvtRERCT
        {
            id = GenerateId(),
            ideEvento = new eFinanceiraEvtRERCTIdeEvento
            {
                tpAmb = 2, // Homologação
                aplicEmi = 1, // Aplicativo do contribuinte
                verAplic = "1.0.0"
            }
        };
    }

    /// <summary>
    /// Cria uma nova instância do builder EvtRERCT
    /// </summary>
    /// <param name="version">Versão do schema</param>
    /// <returns>Nova instância do builder</returns>
    public static EvtRERCTBuilder Create(string version = "v1_2_0") => new(version);

    /// <summary>
    /// Define o ID do evento
    /// </summary>
    /// <param name="id">ID único do evento</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtRERCTBuilder WithId(string id)
    {
        _evento.id = id;
        return this;
    }

    /// <summary>
    /// Configura os dados da identificação do evento
    /// </summary>
    /// <param name="configAction">Ação para configurar o IdeEvento</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtRERCTBuilder WithIdeEvento(Action<IdeEventoBuilder> configAction)
    {
        var builder = new IdeEventoBuilder();
        configAction(builder);
        _evento.ideEvento = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura os dados da identificação do declarante
    /// </summary>
    /// <param name="configAction">Ação para configurar o IdeDeclarante</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtRERCTBuilder WithIdeDeclarante(Action<IdeDeclaranteBuilder> configAction)
    {
        var builder = new IdeDeclaranteBuilder();
        configAction(builder);
        _evento.ideDeclarante = builder.Build();
        return this;
    }

    /// <summary>
    /// Configura os dados da identificação do declarado
    /// </summary>
    /// <param name="configAction">Ação para configurar o IdeDeclarado</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtRERCTBuilder WithIdeDeclarado(Action<IdeDeclaradoBuilder> configAction)
    {
        var builder = new IdeDeclaradoBuilder();
        configAction(builder);
        _evento.ideDeclarado = builder.Build();
        return this;
    }

    /// <summary>
    /// Adiciona um registro RERCT ao evento
    /// </summary>
    /// <param name="configAction">Ação para configurar o RERCT</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtRERCTBuilder AddRERCT(Action<RERCTBuilder> configAction)
    {
        var builder = new RERCTBuilder();
        configAction(builder);

        var rerctList = new List<eFinanceiraEvtRERCTRERCT>(_evento.RERCT ?? []);
        rerctList.Add(builder.Build());
        _evento.RERCT = rerctList.ToArray();

        return this;
    }

    /// <summary>
    /// Adiciona múltiplos registros RERCT ao evento
    /// </summary>
    /// <param name="configActions">Ações para configurar cada RERCT</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EvtRERCTBuilder AddRERCTs(params Action<RERCTBuilder>[] configActions)
    {
        foreach (var configAction in configActions)
        {
            AddRERCT(configAction);
        }
        return this;
    }

    /// <summary>
    /// Constrói a instância final da mensagem de evento
    /// </summary>
    /// <returns>Instância configurada de EvtRERCTMessage</returns>
    public EvtRERCTMessage Build()
    {
        ValidateRequiredFields();
        return new EvtRERCTMessage(_evento, _version);
    }

    /// <summary>
    /// Valida se todos os campos obrigatórios foram preenchidos
    /// </summary>
    private void ValidateRequiredFields()
    {
        if (_evento.ideEvento == null)
            throw new InvalidOperationException("IdeEvento é obrigatório");

        if (_evento.ideDeclarante == null)
            throw new InvalidOperationException("IdeDeclarante é obrigatório");

        if (_evento.ideDeclarado == null)
            throw new InvalidOperationException("IdeDeclarado é obrigatório");

        if (_evento.RERCT == null || _evento.RERCT.Length == 0)
            throw new InvalidOperationException("Pelo menos um registro RERCT é obrigatório");
    }

    /// <summary>
    /// Gera um ID único para o evento
    /// </summary>
    /// <returns>ID único no formato ID_GUID</returns>
    private static string GenerateId() => $"ID_{Guid.NewGuid():N}";
}

#region Builders auxiliares

/// <summary>
/// Builder para configuração da identificação do evento
/// </summary>
public sealed class IdeEventoBuilder
{
    private readonly eFinanceiraEvtRERCTIdeEvento _ideEvento;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdeEventoBuilder"/> class.
    /// </summary>
    public IdeEventoBuilder()
    {
        _ideEvento = new eFinanceiraEvtRERCTIdeEvento
        {
            tpAmb = 2, // Homologação
            aplicEmi = 1, // Aplicativo do contribuinte
            verAplic = "1.0.0"
        };
    }

    /// <summary>
    /// Define o identificador do evento RERCT
    /// </summary>
    /// <param name="ideEventoRERCT">Identificador único do evento</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeEventoBuilder WithIdeEventoRERCT(uint ideEventoRERCT)
    {
        _ideEvento.ideEventoRERCT = ideEventoRERCT;
        return this;
    }

    /// <summary>
    /// Define o indicador de retificação
    /// </summary>
    /// <param name="indRetificacao">0-Original, 1-Retificação</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeEventoBuilder WithIndRetificacao(uint indRetificacao)
    {
        _ideEvento.indRetificacao = indRetificacao;
        return this;
    }

    /// <summary>
    /// Define o número do recibo (obrigatório se indRetificacao = 1)
    /// </summary>
    /// <param name="nrRecibo">Número do recibo do evento original</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeEventoBuilder WithNrRecibo(string nrRecibo)
    {
        _ideEvento.nrRecibo = nrRecibo;
        return this;
    }

    /// <summary>
    /// Define o tipo de ambiente
    /// </summary>
    /// <param name="ambiente">1-Produção, 2-Homologação</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeEventoBuilder WithAmbiente(uint ambiente)
    {
        _ideEvento.tpAmb = ambiente;
        return this;
    }

    /// <summary>
    /// Define o aplicativo emissor
    /// </summary>
    /// <param name="aplicativo">1-Aplicativo do contribuinte</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeEventoBuilder WithAplicativoEmi(uint aplicativo)
    {
        _ideEvento.aplicEmi = aplicativo;
        return this;
    }

    /// <summary>
    /// Define a versão do aplicativo
    /// </summary>
    /// <param name="versao">Versão do aplicativo emissor</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeEventoBuilder WithVersaoAplic(string versao)
    {
        _ideEvento.verAplic = versao;
        return this;
    }

    /// <summary>
    /// Constrói o objeto IdeEvento
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtRERCTIdeEvento</returns>
    internal eFinanceiraEvtRERCTIdeEvento Build() => _ideEvento;
}

/// <summary>
/// Builder para configuração da identificação do declarante
/// </summary>
public sealed class IdeDeclaranteBuilder
{
    private readonly eFinanceiraEvtRERCTIdeDeclarante _ideDeclarante;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdeDeclaranteBuilder"/> class.
    /// </summary>
    public IdeDeclaranteBuilder()
    {
        _ideDeclarante = new eFinanceiraEvtRERCTIdeDeclarante();
    }

    /// <summary>
    /// Define o CNPJ do declarante
    /// </summary>
    /// <param name="cnpj">CNPJ da instituição declarante</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeDeclaranteBuilder WithCnpjDeclarante(string cnpj)
    {
        _ideDeclarante.cnpjDeclarante = cnpj;
        return this;
    }

    /// <summary>
    /// Constrói o objeto IdeDeclarante
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtRERCTIdeDeclarante</returns>
    internal eFinanceiraEvtRERCTIdeDeclarante Build() => _ideDeclarante;
}

/// <summary>
/// Builder para configuração da identificação do declarado
/// </summary>
public sealed class IdeDeclaradoBuilder
{
    private readonly eFinanceiraEvtRERCTIdeDeclarado _ideDeclarado;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdeDeclaradoBuilder"/> class.
    /// </summary>
    public IdeDeclaradoBuilder()
    {
        _ideDeclarado = new eFinanceiraEvtRERCTIdeDeclarado();
    }

    /// <summary>
    /// Configura o CPF/CNPJ do declarado
    /// </summary>
    /// <param name="configAction">Ação para configurar o CPF/CNPJ</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public IdeDeclaradoBuilder WithCpfCnpjDeclarado(Action<CpfCnpjDeclaradoBuilder> configAction)
    {
        var builder = new CpfCnpjDeclaradoBuilder();
        configAction(builder);
        _ideDeclarado.cpfCnpjDeclarado = builder.Build();
        return this;
    }

    /// <summary>
    /// Constrói o objeto IdeDeclarado
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtRERCTIdeDeclarado</returns>
    internal eFinanceiraEvtRERCTIdeDeclarado Build() => _ideDeclarado;
}

/// <summary>
/// Builder para configuração do CPF/CNPJ do declarado
/// </summary>
public sealed class CpfCnpjDeclaradoBuilder
{
    private readonly eFinanceiraEvtRERCTIdeDeclaradoCpfCnpjDeclarado _cpfCnpjDeclarado;

    /// <summary>
    /// Initializes a new instance of the <see cref="CpfCnpjDeclaradoBuilder"/> class.
    /// </summary>
    public CpfCnpjDeclaradoBuilder()
    {
        _cpfCnpjDeclarado = new eFinanceiraEvtRERCTIdeDeclaradoCpfCnpjDeclarado();
    }

    /// <summary>
    /// Define o tipo de inscrição
    /// </summary>
    /// <param name="tipoInscricao">1-CPF, 2-CNPJ</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public CpfCnpjDeclaradoBuilder WithTipoInscricao(uint tipoInscricao)
    {
        _cpfCnpjDeclarado.tpInscr = tipoInscricao;
        return this;
    }

    /// <summary>
    /// Define o número da inscrição (CPF ou CNPJ)
    /// </summary>
    /// <param name="numeroInscricao">Número do CPF ou CNPJ</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public CpfCnpjDeclaradoBuilder WithNumeroInscricao(string numeroInscricao)
    {
        _cpfCnpjDeclarado.nrInscr = numeroInscricao;
        return this;
    }

    /// <summary>
    /// Constrói o objeto CpfCnpjDeclarado
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtRERCTIdeDeclaradoCpfCnpjDeclarado</returns>
    internal eFinanceiraEvtRERCTIdeDeclaradoCpfCnpjDeclarado Build() => _cpfCnpjDeclarado;
}

/// <summary>
/// Builder para configuração de um registro RERCT
/// </summary>
public sealed class RERCTBuilder
{
    private readonly eFinanceiraEvtRERCTRERCT _rerct;

    /// <summary>
    /// Initializes a new instance of the <see cref="RERCTBuilder"/> class.
    /// </summary>
    public RERCTBuilder()
    {
        _rerct = new eFinanceiraEvtRERCTRERCT();
    }

    /// <summary>
    /// Define o nome do banco de origem
    /// </summary>
    /// <param name="nomeBanco">Nome da instituição financeira de origem</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public RERCTBuilder WithNomeBancoOrigem(string nomeBanco)
    {
        _rerct.nomeBancoOrigem = nomeBanco;
        return this;
    }

    /// <summary>
    /// Define o país de origem
    /// </summary>
    /// <param name="pais">Código do país de origem</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public RERCTBuilder WithPaisOrigem(string pais)
    {
        _rerct.paisOrigem = pais;
        return this;
    }

    /// <summary>
    /// Define o código BIC do banco de origem
    /// </summary>
    /// <param name="bicBanco">Código BIC (Bank Identifier Code)</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public RERCTBuilder WithBICBancoOrigem(string bicBanco)
    {
        _rerct.BICBancoOrigem = bicBanco;
        return this;
    }

    /// <summary>
    /// Adiciona uma informação de conta exterior
    /// </summary>
    /// <param name="configAction">Ação para configurar a conta exterior</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public RERCTBuilder AddInfoContaExterior(Action<InfoContaExteriorBuilder> configAction)
    {
        var builder = new InfoContaExteriorBuilder();
        configAction(builder);

        var contasList = new List<eFinanceiraEvtRERCTRERCTInfoContaExterior>(_rerct.infoContaExterior ?? []);
        contasList.Add(builder.Build());
        _rerct.infoContaExterior = contasList.ToArray();

        return this;
    }

    /// <summary>
    /// Adiciona múltiplas informações de contas exteriores
    /// </summary>
    /// <param name="configActions">Ações para configurar cada conta exterior</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public RERCTBuilder AddInfoContasExteriores(params Action<InfoContaExteriorBuilder>[] configActions)
    {
        foreach (var configAction in configActions)
        {
            AddInfoContaExterior(configAction);
        }
        return this;
    }

    /// <summary>
    /// Constrói o objeto RERCT
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtRERCTRERCT</returns>
    internal eFinanceiraEvtRERCTRERCT Build() => _rerct;
}

/// <summary>
/// Builder para configuração de informações de conta exterior
/// </summary>
public sealed class InfoContaExteriorBuilder
{
    private readonly eFinanceiraEvtRERCTRERCTInfoContaExterior _infoContaExterior;

    /// <summary>
    /// Initializes a new instance of the <see cref="InfoContaExteriorBuilder"/> class.
    /// </summary>
    public InfoContaExteriorBuilder()
    {
        _infoContaExterior = new eFinanceiraEvtRERCTRERCTInfoContaExterior();
    }

    /// <summary>
    /// Define o tipo de conta exterior
    /// </summary>
    /// <param name="tipoConta">Tipo da conta exterior conforme tabela específica</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoContaExteriorBuilder WithTipoContaExterior(uint tipoConta)
    {
        _infoContaExterior.tpContaExterior = tipoConta;
        _infoContaExterior.tpContaExteriorSpecified = true;
        return this;
    }

    /// <summary>
    /// Define o número da conta exterior
    /// </summary>
    /// <param name="numeroConta">Número identificador da conta</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoContaExteriorBuilder WithNumeroContaExterior(string numeroConta)
    {
        _infoContaExterior.numContaExterior = numeroConta;
        return this;
    }

    /// <summary>
    /// Define o valor do último dia do ano-calendário
    /// </summary>
    /// <param name="valor">Valor em formato decimal</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoContaExteriorBuilder WithValorUltimoDia(string valor)
    {
        _infoContaExterior.vlrUltDia = valor;
        return this;
    }

    /// <summary>
    /// Define a moeda
    /// </summary>
    /// <param name="moeda">Código da moeda conforme ISO 4217</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoContaExteriorBuilder WithMoeda(string moeda)
    {
        _infoContaExterior.moeda = moeda;
        return this;
    }

    /// <summary>
    /// Adiciona um titular da conta
    /// </summary>
    /// <param name="configAction">Ação para configurar o titular</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoContaExteriorBuilder AddTitular(Action<TitularBuilder> configAction)
    {
        var builder = new TitularBuilder();
        configAction(builder);

        var titularesList = new List<eFinanceiraEvtRERCTRERCTInfoContaExteriorTitular>(_infoContaExterior.titular ?? []);
        titularesList.Add(builder.Build());
        _infoContaExterior.titular = titularesList.ToArray();

        return this;
    }

    /// <summary>
    /// Adiciona múltiplos titulares da conta
    /// </summary>
    /// <param name="configActions">Ações para configurar cada titular</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoContaExteriorBuilder AddTitulares(params Action<TitularBuilder>[] configActions)
    {
        foreach (var configAction in configActions)
        {
            AddTitular(configAction);
        }
        return this;
    }

    /// <summary>
    /// Adiciona um beneficiário final
    /// </summary>
    /// <param name="configAction">Ação para configurar o beneficiário final</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoContaExteriorBuilder AddBeneficiarioFinal(Action<BeneficiarioFinalBuilder> configAction)
    {
        var builder = new BeneficiarioFinalBuilder();
        configAction(builder);

        var beneficiariosList = new List<eFinanceiraEvtRERCTRERCTInfoContaExteriorBeneficiarioFinal>(_infoContaExterior.beneficiarioFinal ?? []);
        beneficiariosList.Add(builder.Build());
        _infoContaExterior.beneficiarioFinal = beneficiariosList.ToArray();

        return this;
    }

    /// <summary>
    /// Adiciona múltiplos beneficiários finais
    /// </summary>
    /// <param name="configActions">Ações para configurar cada beneficiário final</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public InfoContaExteriorBuilder AddBeneficiariosFinais(params Action<BeneficiarioFinalBuilder>[] configActions)
    {
        foreach (var configAction in configActions)
        {
            AddBeneficiarioFinal(configAction);
        }
        return this;
    }

    /// <summary>
    /// Constrói o objeto InfoContaExterior
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtRERCTRERCTInfoContaExterior</returns>
    internal eFinanceiraEvtRERCTRERCTInfoContaExterior Build() => _infoContaExterior;
}

/// <summary>
/// Builder para configuração de titular da conta
/// </summary>
public sealed class TitularBuilder
{
    private readonly eFinanceiraEvtRERCTRERCTInfoContaExteriorTitular _titular;

    /// <summary>
    /// Initializes a new instance of the <see cref="TitularBuilder"/> class.
    /// </summary>
    public TitularBuilder()
    {
        _titular = new eFinanceiraEvtRERCTRERCTInfoContaExteriorTitular();
    }

    /// <summary>
    /// Define o nome do titular
    /// </summary>
    /// <param name="nome">Nome completo do titular</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public TitularBuilder WithNomeTitular(string nome)
    {
        _titular.nomeTitular = nome;
        return this;
    }

    /// <summary>
    /// Configura o CPF/CNPJ do titular
    /// </summary>
    /// <param name="configAction">Ação para configurar o CPF/CNPJ</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public TitularBuilder WithCpfCnpjTitular(Action<CpfCnpjTitularBuilder> configAction)
    {
        var builder = new CpfCnpjTitularBuilder();
        configAction(builder);
        _titular.cpfCnpjTitular = builder.Build();
        return this;
    }

    /// <summary>
    /// Define o NIF (Número de Identificação Fiscal) do titular
    /// </summary>
    /// <param name="nif">NIF do titular no país estrangeiro</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public TitularBuilder WithNIFTitular(string nif)
    {
        _titular.NIFTitular = nif;
        return this;
    }

    /// <summary>
    /// Constrói o objeto Titular
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtRERCTRERCTInfoContaExteriorTitular</returns>
    internal eFinanceiraEvtRERCTRERCTInfoContaExteriorTitular Build() => _titular;
}

/// <summary>
/// Builder para configuração do CPF/CNPJ do titular
/// </summary>
public sealed class CpfCnpjTitularBuilder
{
    private readonly eFinanceiraEvtRERCTRERCTInfoContaExteriorTitularCpfCnpjTitular _cpfCnpjTitular;

    /// <summary>
    /// Initializes a new instance of the <see cref="CpfCnpjTitularBuilder"/> class.
    /// </summary>
    public CpfCnpjTitularBuilder()
    {
        _cpfCnpjTitular = new eFinanceiraEvtRERCTRERCTInfoContaExteriorTitularCpfCnpjTitular();
    }

    /// <summary>
    /// Define o tipo de inscrição
    /// </summary>
    /// <param name="tipoInscricao">1-CPF, 2-CNPJ</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public CpfCnpjTitularBuilder WithTipoInscricao(uint tipoInscricao)
    {
        _cpfCnpjTitular.tpInscr = tipoInscricao;
        return this;
    }

    /// <summary>
    /// Define o número da inscrição (CPF ou CNPJ)
    /// </summary>
    /// <param name="numeroInscricao">Número do CPF ou CNPJ</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public CpfCnpjTitularBuilder WithNumeroInscricao(string numeroInscricao)
    {
        _cpfCnpjTitular.nrInscr = numeroInscricao;
        return this;
    }

    /// <summary>
    /// Constrói o objeto CpfCnpjTitular
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtRERCTRERCTInfoContaExteriorTitularCpfCnpjTitular</returns>
    internal eFinanceiraEvtRERCTRERCTInfoContaExteriorTitularCpfCnpjTitular Build() => _cpfCnpjTitular;
}

/// <summary>
/// Builder para configuração de beneficiário final
/// </summary>
public sealed class BeneficiarioFinalBuilder
{
    private readonly eFinanceiraEvtRERCTRERCTInfoContaExteriorBeneficiarioFinal _beneficiarioFinal;

    /// <summary>
    /// Initializes a new instance of the <see cref="BeneficiarioFinalBuilder"/> class.
    /// </summary>
    public BeneficiarioFinalBuilder()
    {
        _beneficiarioFinal = new eFinanceiraEvtRERCTRERCTInfoContaExteriorBeneficiarioFinal();
    }

    /// <summary>
    /// Define o nome do beneficiário final
    /// </summary>
    /// <param name="nome">Nome completo do beneficiário final</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public BeneficiarioFinalBuilder WithNomeBeneficiarioFinal(string nome)
    {
        _beneficiarioFinal.nomeBeneficiarioFinal = nome;
        return this;
    }

    /// <summary>
    /// Define o CPF do beneficiário final
    /// </summary>
    /// <param name="cpf">CPF do beneficiário final</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public BeneficiarioFinalBuilder WithCpfBeneficiarioFinal(string cpf)
    {
        _beneficiarioFinal.cpfBeneficiarioFinal = cpf;
        return this;
    }

    /// <summary>
    /// Define o NIF (Número de Identificação Fiscal) do beneficiário final
    /// </summary>
    /// <param name="nif">NIF do beneficiário final no país estrangeiro</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public BeneficiarioFinalBuilder WithNIFBeneficiarioFinal(string nif)
    {
        _beneficiarioFinal.NIFBeneficiarioFinal = nif;
        return this;
    }

    /// <summary>
    /// Constrói o objeto BeneficiarioFinal
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraEvtRERCTRERCTInfoContaExteriorBeneficiarioFinal</returns>
    internal eFinanceiraEvtRERCTRERCTInfoContaExteriorBeneficiarioFinal Build() => _beneficiarioFinal;
}

#endregion
using System;
using System.Collections.Generic;
using EFinanceira.Core.Abstractions;
using EFinanceira.Core.Factory;
using EFinanceira.Messages.Generated.Eventos.EvtPrevidenciaPrivada;

namespace EFinanceira.Messages.Builders.Eventos.EvtPrevidenciaPrivada
{
    /// <summary>
    /// Mensagem para evento de movimentação de previdência privada
    /// </summary>
    public sealed class EvtPrevidenciaPrivadaMessage : IEFinanceiraMessage
    {
        public eFinanceiraEvtMovPP Evento { get; }
        public object Payload => Evento;
        public string IdValue => Evento?.id ?? string.Empty;
        public string IdAttributeName => "id";
        public string Version => "v1_2_5";
        public string RootElementName => "evtMovPP";

        public EvtPrevidenciaPrivadaMessage(eFinanceiraEvtMovPP evento)
        {
            Evento = evento ?? throw new ArgumentNullException(nameof(evento));
        }
    }

    /// <summary>
    /// Builder para criação de eventos de movimentação de previdência privada e-Financeira
    /// Especializado para operações previdenciárias (PGBL, VGBL, planos fechados)
    /// </summary>
    public sealed class EvtPrevidenciaPrivadaBuilder
    {
        private readonly eFinanceiraEvtMovPP _evento;

        /// <summary>
        /// Initializes a new instance of the <see cref="EvtPrevidenciaPrivadaBuilder"/> class.
        /// </summary>
        public EvtPrevidenciaPrivadaBuilder()
        {
            _evento = new eFinanceiraEvtMovPP();
        }

        /// <summary>
        /// Define o ID do evento
        /// </summary>
        /// <param name="id">ID único do evento</param>
        /// <returns>Builder para continuar a configuração</returns>
        public EvtPrevidenciaPrivadaBuilder ComId(string id)
        {
            _evento.id = id ?? throw new ArgumentNullException(nameof(id));
            return this;
        }

        /// <summary>
        /// Configura a identificação do evento
        /// </summary>
        /// <param name="configurador">Action para configurar o IdeEvento</param>
        /// <returns>Builder para continuar a configuração</returns>
        public EvtPrevidenciaPrivadaBuilder ComIdeEvento(Action<IdeEventoBuilder> configurador)
        {
            ArgumentNullException.ThrowIfNull(configurador);

            var builder = new IdeEventoBuilder();
            configurador(builder);
            _evento.ideEvento = builder.Build();
            return this;
        }

        /// <summary>
        /// Configura a identificação do declarante
        /// </summary>
        /// <param name="configurador">Action para configurar o IdeDeclarante</param>
        /// <returns>Builder para continuar a configuração</returns>
        public EvtPrevidenciaPrivadaBuilder ComIdeDeclarante(Action<IdeDeclaranteBuilder> configurador)
        {
            ArgumentNullException.ThrowIfNull(configurador);

            var builder = new IdeDeclaranteBuilder();
            configurador(builder);
            _evento.ideDeclarante = builder.Build();
            return this;
        }

        /// <summary>
        /// Configura a identificação do declarado
        /// </summary>
        /// <param name="configurador">Action para configurar o IdeDeclarado</param>
        /// <returns>Builder para continuar a configuração</returns>
        public EvtPrevidenciaPrivadaBuilder ComIdeDeclarado(Action<IdeDeclaradoBuilder> configurador)
        {
            ArgumentNullException.ThrowIfNull(configurador);

            var builder = new IdeDeclaradoBuilder();
            configurador(builder);
            _evento.ideDeclarado = builder.Build();
            return this;
        }

        /// <summary>
        /// Configura o mês caixa com as movimentações de previdência privada
        /// </summary>
        /// <param name="configurador">Action para configurar o MesCaixa</param>
        /// <returns>Builder para continuar a configuração</returns>
        public EvtPrevidenciaPrivadaBuilder ComMesCaixa(Action<MesCaixaBuilder> configurador)
        {
            ArgumentNullException.ThrowIfNull(configurador);

            var builder = new MesCaixaBuilder();
            configurador(builder);
            _evento.mesCaixa = builder.Build();
            return this;
        }

        /// <summary>
        /// Constrói a mensagem completa
        /// </summary>
        /// <returns>Mensagem construída</returns>
        public EvtPrevidenciaPrivadaMessage Build()
        {
            return new EvtPrevidenciaPrivadaMessage(_evento);
        }
    }

    /// <summary>
    /// Builder para configuração da identificação do evento
    /// </summary>
    public sealed class IdeEventoBuilder
    {
        private readonly eFinanceiraEvtMovPPIdeEvento _ideEvento;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdeEventoBuilder"/> class.
        /// </summary>
        public IdeEventoBuilder()
        {
            _ideEvento = new eFinanceiraEvtMovPPIdeEvento();
        }

        /// <summary>
        /// Define o indicador de retificação
        /// </summary>
        /// <param name="indRetificacao">1-Evento Original, 2-Evento de Retificação</param>
        /// <returns>Builder para continuar a configuração</returns>
        public IdeEventoBuilder ComIndRetificacao(byte indRetificacao)
        {
            _ideEvento.indRetificacao = indRetificacao;
            return this;
        }

        /// <summary>
        /// Define o número do recibo (obrigatório para retificações)
        /// </summary>
        /// <param name="nrRecibo">Número do recibo do evento original</param>
        /// <returns>Builder para continuar a configuração</returns>
        public IdeEventoBuilder ComNrRecibo(string nrRecibo)
        {
            _ideEvento.nrRecibo = nrRecibo;
            return this;
        }

        /// <summary>
        /// Define o tipo de ambiente
        /// </summary>
        /// <param name="tpAmb">1-Produção, 2-Homologação</param>
        /// <returns>Builder para continuar a configuração</returns>
        public IdeEventoBuilder ComTpAmb(byte tpAmb)
        {
            _ideEvento.tpAmb = tpAmb;
            return this;
        }

        /// <summary>
        /// Define o aplicativo emissor
        /// </summary>
        /// <param name="aplicEmi">1-Aplicativo do declarante</param>
        /// <returns>Builder para continuar a configuração</returns>
        public IdeEventoBuilder ComAplicEmi(byte aplicEmi)
        {
            _ideEvento.aplicEmi = aplicEmi;
            return this;
        }

        /// <summary>
        /// Define a versão do aplicativo emissor
        /// </summary>
        /// <param name="verAplic">Versão do aplicativo emissor</param>
        /// <returns>Builder para continuar a configuração</returns>
        public IdeEventoBuilder ComVerAplic(string verAplic)
        {
            _ideEvento.verAplic = verAplic;
            return this;
        }

        /// <summary>
        /// Constrói o objeto IdeEvento
        /// </summary>
        /// <returns>Objeto construído</returns>
        internal eFinanceiraEvtMovPPIdeEvento Build()
        {
            return _ideEvento;
        }
    }

    /// <summary>
    /// Builder para configuração da identificação do declarante
    /// </summary>
    public sealed class IdeDeclaranteBuilder
    {
        private readonly eFinanceiraEvtMovPPIdeDeclarante _ideDeclarante;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdeDeclaranteBuilder"/> class.
        /// </summary>
        public IdeDeclaranteBuilder()
        {
            _ideDeclarante = new eFinanceiraEvtMovPPIdeDeclarante();
        }

        /// <summary>
        /// Define o CNPJ do declarante
        /// </summary>
        /// <param name="cnpjDeclarante">CNPJ do declarante</param>
        /// <returns>Builder para continuar a configuração</returns>
        public IdeDeclaranteBuilder ComCnpjDeclarante(string cnpjDeclarante)
        {
            _ideDeclarante.cnpjDeclarante = cnpjDeclarante;
            return this;
        }

        /// <summary>
        /// Constrói o objeto IdeDeclarante
        /// </summary>
        /// <returns>Objeto construído</returns>
        internal eFinanceiraEvtMovPPIdeDeclarante Build()
        {
            return _ideDeclarante;
        }
    }

    /// <summary>
    /// Builder para configuração da identificação do declarado
    /// </summary>
    public sealed class IdeDeclaradoBuilder
    {
        private readonly eFinanceiraEvtMovPPIdeDeclarado _ideDeclarado;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdeDeclaradoBuilder"/> class.
        /// </summary>
        public IdeDeclaradoBuilder()
        {
            _ideDeclarado = new eFinanceiraEvtMovPPIdeDeclarado();
        }

        /// <summary>
        /// Define o tipo de número de identificação
        /// </summary>
        /// <param name="tpNI">1-CPF, 2-CNPJ, 3-Passaporte, 4-Outro</param>
        /// <returns>Builder para continuar a configuração</returns>
        public IdeDeclaradoBuilder ComTpNI(byte tpNI)
        {
            _ideDeclarado.tpNI = tpNI;
            return this;
        }

        /// <summary>
        /// Define o número de identificação do declarado
        /// </summary>
        /// <param name="niDeclarado">Número de identificação do declarado</param>
        /// <returns>Builder para continuar a configuração</returns>
        public IdeDeclaradoBuilder ComNIDeclarado(string niDeclarado)
        {
            _ideDeclarado.NIDeclarado = niDeclarado;
            return this;
        }

        /// <summary>
        /// Define o nome do declarado
        /// </summary>
        /// <param name="nomeDeclarado">Nome do declarado</param>
        /// <returns>Builder para continuar a configuração</returns>
        public IdeDeclaradoBuilder ComNomeDeclarado(string nomeDeclarado)
        {
            _ideDeclarado.NomeDeclarado = nomeDeclarado;
            return this;
        }

        /// <summary>
        /// Constrói o objeto IdeDeclarado
        /// </summary>
        /// <returns>Objeto construído</returns>
        internal eFinanceiraEvtMovPPIdeDeclarado Build()
        {
            return _ideDeclarado;
        }
    }

    /// <summary>
    /// Builder para configuração do mês caixa
    /// </summary>
    public sealed class MesCaixaBuilder
    {
        private readonly eFinanceiraEvtMovPPMesCaixa _mesCaixa;
        private readonly List<eFinanceiraEvtMovPPMesCaixaInfoPrevPriv> _infoPrevPrivs;

        /// <summary>
        /// Initializes a new instance of the <see cref="MesCaixaBuilder"/> class.
        /// </summary>
        public MesCaixaBuilder()
        {
            _mesCaixa = new eFinanceiraEvtMovPPMesCaixa();
            _infoPrevPrivs = new List<eFinanceiraEvtMovPPMesCaixaInfoPrevPriv>();
        }

        /// <summary>
        /// Define o ano e mês da caixa
        /// </summary>
        /// <param name="anoMesCaixa">Ano e mês no formato AAAA-MM</param>
        /// <returns>Builder para continuar a configuração</returns>
        public MesCaixaBuilder ComAnoMesCaixa(string anoMesCaixa)
        {
            _mesCaixa.anoMesCaixa = anoMesCaixa;
            return this;
        }

        /// <summary>
        /// Adiciona informações de previdência privada
        /// </summary>
        /// <param name="configurador">Action para configurar InfoPrevPriv</param>
        /// <returns>Builder para continuar a configuração</returns>
        public MesCaixaBuilder ComInfoPrevPriv(Action<InfoPrevPrivBuilder> configurador)
        {
            ArgumentNullException.ThrowIfNull(configurador);

            var builder = new InfoPrevPrivBuilder();
            configurador(builder);
            _infoPrevPrivs.Add(builder.Build());
            return this;
        }

        /// <summary>
        /// Constrói o objeto MesCaixa
        /// </summary>
        /// <returns>Objeto construído</returns>
        internal eFinanceiraEvtMovPPMesCaixa Build()
        {
            if (_infoPrevPrivs.Count > 0)
            {
                _mesCaixa.infoPrevPriv = _infoPrevPrivs.ToArray();
            }

            return _mesCaixa;
        }
    }

    /// <summary>
    /// Builder para configuração de informações de previdência privada
    /// </summary>
    public sealed class InfoPrevPrivBuilder
    {
        private readonly eFinanceiraEvtMovPPMesCaixaInfoPrevPriv _infoPrevPriv;

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoPrevPrivBuilder"/> class.
        /// </summary>
        public InfoPrevPrivBuilder()
        {
            _infoPrevPriv = new eFinanceiraEvtMovPPMesCaixaInfoPrevPriv();
        }

        /// <summary>
        /// Define o número da proposta
        /// </summary>
        /// <param name="numProposta">Número da proposta</param>
        /// <returns>Builder para continuar a configuração</returns>
        public InfoPrevPrivBuilder ComNumProposta(string numProposta)
        {
            _infoPrevPriv.numProposta = numProposta;
            return this;
        }

        /// <summary>
        /// Define o número do processo
        /// </summary>
        /// <param name="numProcesso">Número do processo SUSEP</param>
        /// <returns>Builder para continuar a configuração</returns>
        public InfoPrevPrivBuilder ComNumProcesso(string numProcesso)
        {
            _infoPrevPriv.numProcesso = numProcesso;
            return this;
        }

        /// <summary>
        /// Configura o produto de previdência
        /// </summary>
        /// <param name="configurador">Action para configurar Produto</param>
        /// <returns>Builder para continuar a configuração</returns>
        public InfoPrevPrivBuilder ComProduto(Action<ProdutoBuilder> configurador)
        {
            ArgumentNullException.ThrowIfNull(configurador);

            var builder = new ProdutoBuilder();
            configurador(builder);
            _infoPrevPriv.Produto = builder.Build();
            return this;
        }

        /// <summary>
        /// Configura o plano de previdência
        /// </summary>
        /// <param name="configurador">Action para configurar Plano</param>
        /// <returns>Builder para continuar a configuração</returns>
        public InfoPrevPrivBuilder ComPlano(Action<PlanoBuilder> configurador)
        {
            ArgumentNullException.ThrowIfNull(configurador);

            var builder = new PlanoBuilder();
            configurador(builder);
            _infoPrevPriv.Plano = builder.Build();
            return this;
        }

        /// <summary>
        /// Configura as operações de previdência privada
        /// </summary>
        /// <param name="configurador">Action para configurar OpPrevPriv</param>
        /// <returns>Builder para continuar a configuração</returns>
        public InfoPrevPrivBuilder ComOpPrevPriv(Action<OpPrevPrivBuilder> configurador)
        {
            ArgumentNullException.ThrowIfNull(configurador);

            var builder = new OpPrevPrivBuilder();
            configurador(builder);
            _infoPrevPriv.opPrevPriv = builder.Build();
            return this;
        }

        /// <summary>
        /// Constrói o objeto InfoPrevPriv
        /// </summary>
        /// <returns>Objeto construído</returns>
        internal eFinanceiraEvtMovPPMesCaixaInfoPrevPriv Build()
        {
            return _infoPrevPriv;
        }
    }

    /// <summary>
    /// Builder para configuração de produto
    /// </summary>
    public sealed class ProdutoBuilder
    {
        private readonly eFinanceiraEvtMovPPMesCaixaInfoPrevPrivProduto _produto;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProdutoBuilder"/> class.
        /// </summary>
        public ProdutoBuilder()
        {
            _produto = new eFinanceiraEvtMovPPMesCaixaInfoPrevPrivProduto();
        }

        /// <summary>
        /// Define o tipo de produto
        /// </summary>
        /// <param name="tpProduto">1-PGBL, 2-VGBL, 3-Outros</param>
        /// <returns>Builder para continuar a configuração</returns>
        public ProdutoBuilder ComTpProduto(byte tpProduto)
        {
            _produto.tpProduto = tpProduto;
            return this;
        }

        /// <summary>
        /// Define a opção de tributação (opcional)
        /// </summary>
        /// <param name="opcaoTributacao">1-Progressiva, 2-Regressiva</param>
        /// <returns>Builder para continuar a configuração</returns>
        public ProdutoBuilder ComOpcaoTributacao(byte? opcaoTributacao)
        {
            if (opcaoTributacao.HasValue)
            {
                _produto.opcaoTributacao = opcaoTributacao.Value;
                _produto.opcaoTributacaoSpecified = true;
            }
            return this;
        }

        /// <summary>
        /// Constrói o objeto Produto
        /// </summary>
        /// <returns>Objeto construído</returns>
        internal eFinanceiraEvtMovPPMesCaixaInfoPrevPrivProduto Build()
        {
            return _produto;
        }
    }

    /// <summary>
    /// Builder para configuração de plano
    /// </summary>
    public sealed class PlanoBuilder
    {
        private readonly eFinanceiraEvtMovPPMesCaixaInfoPrevPrivPlano _plano;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanoBuilder"/> class.
        /// </summary>
        public PlanoBuilder()
        {
            _plano = new eFinanceiraEvtMovPPMesCaixaInfoPrevPrivPlano();
        }

        /// <summary>
        /// Define o tipo de plano (opcional)
        /// </summary>
        /// <param name="tpPlano">1-Aberto, 2-Fechado</param>
        /// <returns>Builder para continuar a configuração</returns>
        public PlanoBuilder ComTpPlano(byte? tpPlano)
        {
            if (tpPlano.HasValue)
            {
                _plano.tpPlano = tpPlano.Value;
                _plano.tpPlanoSpecified = true;
            }
            return this;
        }

        /// <summary>
        /// Define se é plano fechado
        /// </summary>
        /// <param name="planoFechado">1-Sim, 0-Não</param>
        /// <returns>Builder para continuar a configuração</returns>
        public PlanoBuilder ComPlanoFechado(byte planoFechado)
        {
            _plano.planoFechado = planoFechado;
            return this;
        }

        /// <summary>
        /// Define o CNPJ do plano (se aplicável)
        /// </summary>
        /// <param name="cnpjPlano">CNPJ do plano</param>
        /// <returns>Builder para continuar a configuração</returns>
        public PlanoBuilder ComCnpjPlano(string cnpjPlano)
        {
            _plano.cnpjPlano = cnpjPlano;
            return this;
        }

        /// <summary>
        /// Define o tipo de plano fechado (opcional)
        /// </summary>
        /// <param name="tpPlanoFechado">1-Benefício Definido, 2-Contribuição Definida, 3-Contribuição Variável</param>
        /// <returns>Builder para continuar a configuração</returns>
        public PlanoBuilder ComTpPlanoFechado(byte? tpPlanoFechado)
        {
            if (tpPlanoFechado.HasValue)
            {
                _plano.tpPlanoFechado = tpPlanoFechado.Value;
                _plano.tpPlanoFechadoSpecified = true;
            }
            return this;
        }

        /// <summary>
        /// Constrói o objeto Plano
        /// </summary>
        /// <returns>Objeto construído</returns>
        internal eFinanceiraEvtMovPPMesCaixaInfoPrevPrivPlano Build()
        {
            return _plano;
        }
    }

    /// <summary>
    /// Builder para configuração de operações de previdência privada
    /// </summary>
    public sealed class OpPrevPrivBuilder
    {
        private readonly eFinanceiraEvtMovPPMesCaixaInfoPrevPrivOpPrevPriv _opPrevPriv;
        private readonly List<eFinanceiraEvtMovPPMesCaixaInfoPrevPrivOpPrevPrivAplic> _aplics;
        private readonly List<eFinanceiraEvtMovPPMesCaixaInfoPrevPrivOpPrevPrivResg> _resgs;
        private readonly List<eFinanceiraEvtMovPPMesCaixaInfoPrevPrivOpPrevPrivBenef> _benefs;
        private readonly List<eFinanceiraEvtMovPPMesCaixaInfoPrevPrivOpPrevPrivPortabilidade> _portabilidades;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpPrevPrivBuilder"/> class.
        /// </summary>
        public OpPrevPrivBuilder()
        {
            _opPrevPriv = new eFinanceiraEvtMovPPMesCaixaInfoPrevPrivOpPrevPriv();
            _aplics = new List<eFinanceiraEvtMovPPMesCaixaInfoPrevPrivOpPrevPrivAplic>();
            _resgs = new List<eFinanceiraEvtMovPPMesCaixaInfoPrevPrivOpPrevPrivResg>();
            _benefs = new List<eFinanceiraEvtMovPPMesCaixaInfoPrevPrivOpPrevPrivBenef>();
            _portabilidades = new List<eFinanceiraEvtMovPPMesCaixaInfoPrevPrivOpPrevPrivPortabilidade>();
        }

        /// <summary>
        /// Configura o saldo inicial
        /// </summary>
        /// <param name="configurador">Action para configurar SaldoInicial</param>
        /// <returns>Builder para continuar a configuração</returns>
        public OpPrevPrivBuilder ComSaldoInicial(Action<SaldoInicialBuilder> configurador)
        {
            ArgumentNullException.ThrowIfNull(configurador);

            var builder = new SaldoInicialBuilder();
            configurador(builder);
            _opPrevPriv.saldoInicial = builder.Build();
            return this;
        }

        /// <summary>
        /// Adiciona uma aplicação
        /// </summary>
        /// <param name="configurador">Action para configurar Aplic</param>
        /// <returns>Builder para continuar a configuração</returns>
        public OpPrevPrivBuilder ComAplic(Action<AplicBuilder> configurador)
        {
            ArgumentNullException.ThrowIfNull(configurador);

            var builder = new AplicBuilder();
            configurador(builder);
            _aplics.Add(builder.Build());
            return this;
        }

        /// <summary>
        /// Configura o saldo final
        /// </summary>
        /// <param name="configurador">Action para configurar SaldoFinal</param>
        /// <returns>Builder para continuar a configuração</returns>
        public OpPrevPrivBuilder ComSaldoFinal(Action<SaldoFinalBuilder> configurador)
        {
            ArgumentNullException.ThrowIfNull(configurador);

            var builder = new SaldoFinalBuilder();
            configurador(builder);
            _opPrevPriv.saldoFinal = builder.Build();
            return this;
        }

        /// <summary>
        /// Constrói o objeto OpPrevPriv
        /// </summary>
        /// <returns>Objeto construído</returns>
        internal eFinanceiraEvtMovPPMesCaixaInfoPrevPrivOpPrevPriv Build()
        {
            if (_aplics.Count > 0)
            {
                _opPrevPriv.aplic = _aplics.ToArray();
            }

            if (_resgs.Count > 0)
            {
                _opPrevPriv.resg = _resgs.ToArray();
            }

            if (_benefs.Count > 0)
            {
                _opPrevPriv.benef = _benefs.ToArray();
            }

            if (_portabilidades.Count > 0)
            {
                _opPrevPriv.portabilidade = _portabilidades.ToArray();
            }

            return _opPrevPriv;
        }
    }

    /// <summary>
    /// Builder para configuração de saldo inicial
    /// </summary>
    public sealed class SaldoInicialBuilder
    {
        private readonly eFinanceiraEvtMovPPMesCaixaInfoPrevPrivOpPrevPrivSaldoInicial _saldoInicial;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaldoInicialBuilder"/> class.
        /// </summary>
        public SaldoInicialBuilder()
        {
            _saldoInicial = new eFinanceiraEvtMovPPMesCaixaInfoPrevPrivOpPrevPrivSaldoInicial();
        }

        /// <summary>
        /// Define o valor principal
        /// </summary>
        /// <param name="vlrPrincipal">Valor principal</param>
        /// <returns>Builder para continuar a configuração</returns>
        public SaldoInicialBuilder ComVlrPrincipal(string vlrPrincipal)
        {
            _saldoInicial.vlrPrincipal = vlrPrincipal;
            return this;
        }

        /// <summary>
        /// Define o valor de rendimentos
        /// </summary>
        /// <param name="vlrRendimentos">Valor de rendimentos</param>
        /// <returns>Builder para continuar a configuração</returns>
        public SaldoInicialBuilder ComVlrRendimentos(string vlrRendimentos)
        {
            _saldoInicial.vlrRendimentos = vlrRendimentos;
            return this;
        }

        /// <summary>
        /// Constrói o objeto SaldoInicial
        /// </summary>
        /// <returns>Objeto construído</returns>
        internal eFinanceiraEvtMovPPMesCaixaInfoPrevPrivOpPrevPrivSaldoInicial Build()
        {
            return _saldoInicial;
        }
    }

    /// <summary>
    /// Builder para configuração de aplicação
    /// </summary>
    public sealed class AplicBuilder
    {
        private readonly eFinanceiraEvtMovPPMesCaixaInfoPrevPrivOpPrevPrivAplic _aplic;

        /// <summary>
        /// Initializes a new instance of the <see cref="AplicBuilder"/> class.
        /// </summary>
        public AplicBuilder()
        {
            _aplic = new eFinanceiraEvtMovPPMesCaixaInfoPrevPrivOpPrevPrivAplic();
        }

        /// <summary>
        /// Define o valor da contribuição
        /// </summary>
        /// <param name="vlrContribuicao">Valor da contribuição</param>
        /// <returns>Builder para continuar a configuração</returns>
        public AplicBuilder ComVlrContribuicao(string vlrContribuicao)
        {
            _aplic.vlrContribuicao = vlrContribuicao;
            return this;
        }

        /// <summary>
        /// Define o valor do carregamento
        /// </summary>
        /// <param name="vlrCarregamento">Valor do carregamento</param>
        /// <returns>Builder para continuar a configuração</returns>
        public AplicBuilder ComVlrCarregamento(string vlrCarregamento)
        {
            _aplic.vlrCarregamento = vlrCarregamento;
            return this;
        }

        /// <summary>
        /// Define o valor da participação PF
        /// </summary>
        /// <param name="vlrPartPF">Valor da participação pessoa física</param>
        /// <returns>Builder para continuar a configuração</returns>
        public AplicBuilder ComVlrPartPF(string vlrPartPF)
        {
            _aplic.vlrPartPF = vlrPartPF;
            return this;
        }

        /// <summary>
        /// Define o valor da participação PJ
        /// </summary>
        /// <param name="vlrPartPJ">Valor da participação pessoa jurídica</param>
        /// <returns>Builder para continuar a configuração</returns>
        public AplicBuilder ComVlrPartPJ(string vlrPartPJ)
        {
            _aplic.vlrPartPJ = vlrPartPJ;
            return this;
        }

        /// <summary>
        /// Define o CNPJ do patrocinador
        /// </summary>
        /// <param name="cnpj">CNPJ do patrocinador</param>
        /// <returns>Builder para continuar a configuração</returns>
        public AplicBuilder ComCnpj(string cnpj)
        {
            _aplic.cnpj = cnpj;
            return this;
        }

        /// <summary>
        /// Constrói o objeto Aplic
        /// </summary>
        /// <returns>Objeto construído</returns>
        internal eFinanceiraEvtMovPPMesCaixaInfoPrevPrivOpPrevPrivAplic Build()
        {
            return _aplic;
        }
    }

    /// <summary>
    /// Builder para configuração de saldo final
    /// </summary>
    public sealed class SaldoFinalBuilder
    {
        private readonly eFinanceiraEvtMovPPMesCaixaInfoPrevPrivOpPrevPrivSaldoFinal _saldoFinal;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaldoFinalBuilder"/> class.
        /// </summary>
        public SaldoFinalBuilder()
        {
            _saldoFinal = new eFinanceiraEvtMovPPMesCaixaInfoPrevPrivOpPrevPrivSaldoFinal();
        }

        /// <summary>
        /// Define o valor principal
        /// </summary>
        /// <param name="vlrPrincipal">Valor principal</param>
        /// <returns>Builder para continuar a configuração</returns>
        public SaldoFinalBuilder ComVlrPrincipal(string vlrPrincipal)
        {
            _saldoFinal.vlrPrincipal = vlrPrincipal;
            return this;
        }

        /// <summary>
        /// Define o valor de rendimentos
        /// </summary>
        /// <param name="vlrRendimentos">Valor de rendimentos</param>
        /// <returns>Builder para continuar a configuração</returns>
        public SaldoFinalBuilder ComVlrRendimentos(string vlrRendimentos)
        {
            _saldoFinal.vlrRendimentos = vlrRendimentos;
            return this;
        }

        /// <summary>
        /// Constrói o objeto SaldoFinal
        /// </summary>
        /// <returns>Objeto construído</returns>
        internal eFinanceiraEvtMovPPMesCaixaInfoPrevPrivOpPrevPrivSaldoFinal Build()
        {
            return _saldoFinal;
        }
    }
}

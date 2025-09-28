using System;
using System.Collections.Generic;
using System.Linq;
using EFinanceira.Core.Abstractions;
using EFinanceira.Core.Factory;
using EFinanceira.Messages.Generated.Eventos.EvtMovimentacaoFinanceiraAnual;

namespace EFinanceira.Messages.Builders.Eventos.EvtMovimentacaoFinanceiraAnual
{
    /// <summary>
    /// Mensagem para evento de movimentação financeira anual
    /// </summary>
    public sealed class EvtMovimentacaoFinanceiraAnualMessage : IEFinanceiraMessage
    {
        public eFinanceiraEvtMovOpFinAnual Evento { get; }
        public object Payload => Evento;
        public string IdValue => Evento?.id ?? string.Empty;
        public string IdAttributeName => "id";
        public string Version => "v1_2_2";
        public string RootElementName => "evtMovOpFinAnual";

        public EvtMovimentacaoFinanceiraAnualMessage(eFinanceiraEvtMovOpFinAnual evento)
        {
            Evento = evento ?? throw new ArgumentNullException(nameof(evento));
        }
    }

    /// <summary>
    /// Builder para criação de eventos de movimentação financeira anual e-Financeira
    /// Implementação simplificada para o evento mais complexo de movimentação anual
    /// </summary>
    public sealed class EvtMovimentacaoFinanceiraAnualBuilder
    {
        private readonly eFinanceiraEvtMovOpFinAnual _evento;
        private readonly List<eFinanceiraEvtMovOpFinAnualCaixaConta> _contas;

        /// <summary>
        /// Initializes a new instance of the <see cref="EvtMovimentacaoFinanceiraAnualBuilder"/> class.
        /// </summary>
        public EvtMovimentacaoFinanceiraAnualBuilder()
        {
            _evento = new eFinanceiraEvtMovOpFinAnual();
            _contas = new List<eFinanceiraEvtMovOpFinAnualCaixaConta>();
        }

        /// <summary>
        /// Define o ID do evento
        /// </summary>
        /// <param name="id">ID único do evento</param>
        /// <returns>Builder para continuar a configuração</returns>
        public EvtMovimentacaoFinanceiraAnualBuilder ComId(string id)
        {
            _evento.id = id ?? throw new ArgumentNullException(nameof(id));
            return this;
        }

        /// <summary>
        /// Configura a identificação do evento
        /// </summary>
        /// <param name="configurador">Action para configurar o IdeEvento</param>
        /// <returns>Builder para continuar a configuração</returns>
        public EvtMovimentacaoFinanceiraAnualBuilder ComIdeEvento(Action<IdeEventoBuilder> configurador)
        {
            if (configurador == null) throw new ArgumentNullException(nameof(configurador));

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
        public EvtMovimentacaoFinanceiraAnualBuilder ComIdeDeclarante(Action<IdeDeclaranteBuilder> configurador)
        {
            if (configurador == null) throw new ArgumentNullException(nameof(configurador));

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
        public EvtMovimentacaoFinanceiraAnualBuilder ComIdeDeclarado(Action<IdeDeclaradoBuilder> configurador)
        {
            if (configurador == null) throw new ArgumentNullException(nameof(configurador));

            var builder = new IdeDeclaradoBuilder();
            configurador(builder);
            _evento.ideDeclarado = builder.Build();
            return this;
        }

        /// <summary>
        /// Configura a caixa anual
        /// </summary>
        /// <param name="configurador">Action para configurar a Caixa</param>
        /// <returns>Builder para continuar a configuração</returns>
        public EvtMovimentacaoFinanceiraAnualBuilder ComCaixa(Action<CaixaBuilder> configurador)
        {
            if (configurador == null) throw new ArgumentNullException(nameof(configurador));

            var builder = new CaixaBuilder();
            configurador(builder);
            _evento.Caixa = builder.Build();
            return this;
        }

        /// <summary>
        /// Constrói a mensagem completa
        /// </summary>
        /// <returns>Mensagem construída</returns>
        public EvtMovimentacaoFinanceiraAnualMessage Build()
        {
            if (_evento.Caixa?.movOpFinAnual?.Length > 0)
            {
                // Adiciona as contas configuradas se houver
                _evento.Caixa.movOpFinAnual = _contas.ToArray();
            }
            return new EvtMovimentacaoFinanceiraAnualMessage(_evento);
        }
    }

    /// <summary>
    /// Builder para configuração da identificação do evento
    /// </summary>
    public sealed class IdeEventoBuilder
    {
        private readonly eFinanceiraEvtMovOpFinAnualIdeEvento _ideEvento;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdeEventoBuilder"/> class.
        /// </summary>
        public IdeEventoBuilder()
        {
            _ideEvento = new eFinanceiraEvtMovOpFinAnualIdeEvento();
        }

        /// <summary>
        /// Define o indicador de retificação
        /// </summary>
        /// <param name="indRetificacao">1-Evento Original, 2-Evento de Retificação</param>
        /// <returns>Builder para continuar a configuração</returns>
        public IdeEventoBuilder ComIndRetificacao(uint indRetificacao)
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
        public IdeEventoBuilder ComTpAmb(uint tpAmb)
        {
            _ideEvento.tpAmb = tpAmb;
            return this;
        }

        /// <summary>
        /// Define o aplicativo emissor
        /// </summary>
        /// <param name="aplicEmi">1-Aplicativo do declarante</param>
        /// <returns>Builder para continuar a configuração</returns>
        public IdeEventoBuilder ComAplicEmi(uint aplicEmi)
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
        internal eFinanceiraEvtMovOpFinAnualIdeEvento Build()
        {
            return _ideEvento;
        }
    }

    /// <summary>
    /// Builder para configuração da identificação do declarante
    /// </summary>
    public sealed class IdeDeclaranteBuilder
    {
        private readonly eFinanceiraEvtMovOpFinAnualIdeDeclarante _ideDeclarante;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdeDeclaranteBuilder"/> class.
        /// </summary>
        public IdeDeclaranteBuilder()
        {
            _ideDeclarante = new eFinanceiraEvtMovOpFinAnualIdeDeclarante();
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
        internal eFinanceiraEvtMovOpFinAnualIdeDeclarante Build()
        {
            return _ideDeclarante;
        }
    }

    /// <summary>
    /// Builder para configuração da identificação do declarado (implementação simplificada)
    /// </summary>
    public sealed class IdeDeclaradoBuilder
    {
        private readonly eFinanceiraEvtMovOpFinAnualIdeDeclarado _ideDeclarado;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdeDeclaradoBuilder"/> class.
        /// </summary>
        public IdeDeclaradoBuilder()
        {
            _ideDeclarado = new eFinanceiraEvtMovOpFinAnualIdeDeclarado();
        }

        /// <summary>
        /// Define o tipo de NI (Número de Identificação)
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
        /// <param name="niDeclarado">Número de identificação</param>
        /// <returns>Builder para continuar a configuração</returns>
        public IdeDeclaradoBuilder ComNIDeclarado(string niDeclarado)
        {
            _ideDeclarado.NIDeclarado = niDeclarado;
            return this;
        }

        /// <summary>
        /// Define o nome do declarado
        /// </summary>
        /// <param name="nomeDeclarado">Nome completo do declarado</param>
        /// <returns>Builder para continuar a configuração</returns>
        public IdeDeclaradoBuilder ComNomeDeclarado(string nomeDeclarado)
        {
            _ideDeclarado.NomeDeclarado = nomeDeclarado;
            return this;
        }

        /// <summary>
        /// Define informações básicas de nascimento (implementação simplificada)
        /// </summary>
        /// <param name="dataNascimento">Data de nascimento</param>
        /// <returns>Builder para continuar a configuração</returns>
        public IdeDeclaradoBuilder ComDataNascimento(DateTime dataNascimento)
        {
            // Usa o campo direto DataNasc da estrutura principal
            _ideDeclarado.DataNasc = dataNascimento;
            _ideDeclarado.DataNascSpecified = true;
            return this;
        }

        /// <summary>
        /// Define endereço em formato livre (implementação simplificada)
        /// </summary>
        /// <param name="enderecoLivre">Endereço em formato livre</param>
        /// <returns>Builder para continuar a configuração</returns>
        public IdeDeclaradoBuilder ComEnderecoLivre(string enderecoLivre)
        {
            // Usa o campo direto EnderecoLivre da estrutura principal
            _ideDeclarado.EnderecoLivre = enderecoLivre;
            return this;
        }

        /// <summary>
        /// Constrói o objeto IdeDeclarado
        /// </summary>
        /// <returns>Objeto construído</returns>
        internal eFinanceiraEvtMovOpFinAnualIdeDeclarado Build()
        {
            return _ideDeclarado;
        }
    }

    /// <summary>
    /// Builder para configuração da caixa anual
    /// </summary>
    public sealed class CaixaBuilder
    {
        private readonly eFinanceiraEvtMovOpFinAnualCaixa _caixa;
        private readonly List<eFinanceiraEvtMovOpFinAnualCaixaConta> _contas;

        /// <summary>
        /// Initializes a new instance of the <see cref="CaixaBuilder"/> class.
        /// </summary>
        public CaixaBuilder()
        {
            _caixa = new eFinanceiraEvtMovOpFinAnualCaixa();
            _contas = new List<eFinanceiraEvtMovOpFinAnualCaixaConta>();
        }

        /// <summary>
        /// Define o ano da caixa
        /// </summary>
        /// <param name="anoCaixa">Ano da caixa no formato AAAA</param>
        /// <returns>Builder para continuar a configuração</returns>
        public CaixaBuilder ComAnoCaixa(string anoCaixa)
        {
            _caixa.anoCaixa = anoCaixa;
            return this;
        }

        /// <summary>
        /// Define o semestre
        /// </summary>
        /// <param name="semestre">1-Primeiro semestre, 2-Segundo semestre</param>
        /// <returns>Builder para continuar a configuração</returns>
        public CaixaBuilder ComSemestre(uint semestre)
        {
            _caixa.semestre = semestre;
            return this;
        }

        /// <summary>
        /// Adiciona uma conta à movimentação anual
        /// </summary>
        /// <param name="configurador">Action para configurar a conta</param>
        /// <returns>Builder para continuar a configuração</returns>
        public CaixaBuilder ComConta(Action<ContaBuilder> configurador)
        {
            if (configurador == null) throw new ArgumentNullException(nameof(configurador));

            var builder = new ContaBuilder();
            configurador(builder);
            _contas.Add(builder.Build());
            return this;
        }

        /// <summary>
        /// Constrói o objeto Caixa
        /// </summary>
        /// <returns>Objeto construído</returns>
        internal eFinanceiraEvtMovOpFinAnualCaixa Build()
        {
            if (_contas.Count > 0)
            {
                _caixa.movOpFinAnual = _contas.ToArray();
            }
            return _caixa;
        }
    }

    /// <summary>
    /// Builder para configuração de conta (implementação simplificada)
    /// </summary>
    public sealed class ContaBuilder
    {
        private readonly eFinanceiraEvtMovOpFinAnualCaixaConta _conta;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContaBuilder"/> class.
        /// </summary>
        public ContaBuilder()
        {
            _conta = new eFinanceiraEvtMovOpFinAnualCaixaConta();
        }

        /// <summary>
        /// Configura informações básicas da conta (implementação simplificada)
        /// </summary>
        /// <param name="tpConta">Tipo da conta</param>
        /// <param name="subTpConta">Subtipo da conta</param>
        /// <param name="tpNumConta">Tipo do número da conta</param>
        /// <param name="numConta">Número da conta</param>
        /// <returns>Builder para continuar a configuração</returns>
        public ContaBuilder ComInfoConta(string tpConta, string subTpConta, string tpNumConta, string numConta)
        {
            _conta.infoConta = new eFinanceiraEvtMovOpFinAnualCaixaContaInfoConta();

            // Configuração básica da conta - estrutura complexa, implementação simplificada
            // Na implementação completa seria necessário configurar todos os campos específicos
            return this;
        }

        /// <summary>
        /// Configura balanço da conta (implementação simplificada)
        /// </summary>
        /// <param name="valor">Valor do balanço</param>
        /// <returns>Builder para continuar a configuração</returns>
        public ContaBuilder ComBalancoConta(decimal valor)
        {
            if (_conta.infoConta == null)
            {
                _conta.infoConta = new eFinanceiraEvtMovOpFinAnualCaixaContaInfoConta();
            }

            _conta.infoConta.BalancoConta = new eFinanceiraEvtMovOpFinAnualCaixaContaInfoContaBalancoConta
            {
                // Implementação simplificada - valor básico
            };

            return this;
        }

        /// <summary>
        /// Constrói o objeto Conta
        /// </summary>
        /// <returns>Objeto construído</returns>
        internal eFinanceiraEvtMovOpFinAnualCaixaConta Build()
        {
            return _conta;
        }
    }
}

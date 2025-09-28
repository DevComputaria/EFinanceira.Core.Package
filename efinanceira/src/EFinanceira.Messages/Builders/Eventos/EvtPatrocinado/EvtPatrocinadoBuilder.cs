using System;
using System.Collections.Generic;
using EFinanceira.Core.Abstractions;
using EFinanceira.Core.Factory;
using EFinanceira.Messages.Generated.Eventos.EvtPatrocinado;

namespace EFinanceira.Messages.Builders.Eventos.EvtPatrocinado
{
    /// <summary>
    /// Mensagem para evento de cadastro de patrocinado
    /// </summary>
    public sealed class EvtPatrocinadoMessage : IEFinanceiraMessage
    {
        public eFinanceiraEvtCadPatrocinado Evento { get; }
        public object Payload => Evento;
        public string IdValue => Evento?.id ?? string.Empty;
        public string IdAttributeName => "id";
        public string Version => "v1_2_0";
        public string RootElementName => "evtCadPatrocinado";

        public EvtPatrocinadoMessage(eFinanceiraEvtCadPatrocinado evento)
        {
            Evento = evento ?? throw new ArgumentNullException(nameof(evento));
        }
    }

    /// <summary>
    /// Builder para criação de eventos de cadastro de patrocinado e-Financeira
    /// Especializado para entidades patrocinadas em estruturas de compliance internacional
    /// </summary>
    public sealed class EvtPatrocinadoBuilder
    {
        private readonly eFinanceiraEvtCadPatrocinado _evento;

        /// <summary>
        /// Initializes a new instance of the <see cref="EvtPatrocinadoBuilder"/> class.
        /// </summary>
        public EvtPatrocinadoBuilder()
        {
            _evento = new eFinanceiraEvtCadPatrocinado();
        }

        /// <summary>
        /// Define o ID do evento
        /// </summary>
        /// <param name="id">ID único do evento</param>
        /// <returns>Builder para continuar a configuração</returns>
        public EvtPatrocinadoBuilder ComId(string id)
        {
            _evento.id = id ?? throw new ArgumentNullException(nameof(id));
            return this;
        }

        /// <summary>
        /// Configura a identificação do evento
        /// </summary>
        /// <param name="configurador">Action para configurar o IdeEvento</param>
        /// <returns>Builder para continuar a configuração</returns>
        public EvtPatrocinadoBuilder ComIdeEvento(Action<IdeEventoBuilder> configurador)
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
        public EvtPatrocinadoBuilder ComIdeDeclarante(Action<IdeDeclaranteBuilder> configurador)
        {
            ArgumentNullException.ThrowIfNull(configurador);

            var builder = new IdeDeclaranteBuilder();
            configurador(builder);
            _evento.ideDeclarante = builder.Build();
            return this;
        }

        /// <summary>
        /// Configura as informações do patrocinado
        /// </summary>
        /// <param name="configurador">Action para configurar o InfoPatrocinado</param>
        /// <returns>Builder para continuar a configuração</returns>
        public EvtPatrocinadoBuilder ComInfoPatrocinado(Action<InfoPatrocinadoBuilder> configurador)
        {
            ArgumentNullException.ThrowIfNull(configurador);

            var builder = new InfoPatrocinadoBuilder();
            configurador(builder);
            _evento.infoPatrocinado = builder.Build();
            return this;
        }

        /// <summary>
        /// Constrói a mensagem completa
        /// </summary>
        /// <returns>Mensagem construída</returns>
        public EvtPatrocinadoMessage Build()
        {
            return new EvtPatrocinadoMessage(_evento);
        }
    }

    /// <summary>
    /// Builder para configuração da identificação do evento
    /// </summary>
    public sealed class IdeEventoBuilder
    {
        private readonly eFinanceiraEvtCadPatrocinadoIdeEvento _ideEvento;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdeEventoBuilder"/> class.
        /// </summary>
        public IdeEventoBuilder()
        {
            _ideEvento = new eFinanceiraEvtCadPatrocinadoIdeEvento();
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
        internal eFinanceiraEvtCadPatrocinadoIdeEvento Build()
        {
            return _ideEvento;
        }
    }

    /// <summary>
    /// Builder para configuração da identificação do declarante
    /// </summary>
    public sealed class IdeDeclaranteBuilder
    {
        private readonly eFinanceiraEvtCadPatrocinadoIdeDeclarante _ideDeclarante;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdeDeclaranteBuilder"/> class.
        /// </summary>
        public IdeDeclaranteBuilder()
        {
            _ideDeclarante = new eFinanceiraEvtCadPatrocinadoIdeDeclarante();
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
        internal eFinanceiraEvtCadPatrocinadoIdeDeclarante Build()
        {
            return _ideDeclarante;
        }
    }

    /// <summary>
    /// Builder para configuração das informações do patrocinado
    /// </summary>
    public sealed class InfoPatrocinadoBuilder
    {
        private readonly eFinanceiraEvtCadPatrocinadoInfoPatrocinado _infoPatrocinado;
        private readonly List<eFinanceiraEvtCadPatrocinadoInfoPatrocinadoNIF> _nifs;
        private readonly List<eFinanceiraEvtCadPatrocinadoInfoPatrocinadoEnderecoOutros> _enderecosOutros;
        private readonly List<eFinanceiraEvtCadPatrocinadoInfoPatrocinadoPaisResid> _paisesResid;

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoPatrocinadoBuilder"/> class.
        /// </summary>
        public InfoPatrocinadoBuilder()
        {
            _infoPatrocinado = new eFinanceiraEvtCadPatrocinadoInfoPatrocinado();
            _nifs = new List<eFinanceiraEvtCadPatrocinadoInfoPatrocinadoNIF>();
            _enderecosOutros = new List<eFinanceiraEvtCadPatrocinadoInfoPatrocinadoEnderecoOutros>();
            _paisesResid = new List<eFinanceiraEvtCadPatrocinadoInfoPatrocinadoPaisResid>();
        }

        /// <summary>
        /// Define o GIIN (Global Intermediary Identification Number) do patrocinado
        /// </summary>
        /// <param name="giin">GIIN do patrocinado</param>
        /// <returns>Builder para continuar a configuração</returns>
        public InfoPatrocinadoBuilder ComGIIN(string giin)
        {
            _infoPatrocinado.GIIN = giin;
            return this;
        }

        /// <summary>
        /// Define o CNPJ do patrocinado
        /// </summary>
        /// <param name="cnpj">CNPJ do patrocinado</param>
        /// <returns>Builder para continuar a configuração</returns>
        public InfoPatrocinadoBuilder ComCNPJ(string cnpj)
        {
            _infoPatrocinado.CNPJ = cnpj;
            return this;
        }

        /// <summary>
        /// Adiciona um NIF (Número de Identificação Fiscal) do patrocinado
        /// </summary>
        /// <param name="configurador">Action para configurar o NIF</param>
        /// <returns>Builder para continuar a configuração</returns>
        public InfoPatrocinadoBuilder ComNIF(Action<NIFBuilder> configurador)
        {
            ArgumentNullException.ThrowIfNull(configurador);

            var builder = new NIFBuilder();
            configurador(builder);
            _nifs.Add(builder.Build());
            return this;
        }

        /// <summary>
        /// Define o nome do patrocinado
        /// </summary>
        /// <param name="nomePatrocinado">Nome do patrocinado</param>
        /// <returns>Builder para continuar a configuração</returns>
        public InfoPatrocinadoBuilder ComNomePatrocinado(string nomePatrocinado)
        {
            _infoPatrocinado.nomePatrocinado = nomePatrocinado;
            return this;
        }

        /// <summary>
        /// Define o tipo de nome do patrocinado
        /// </summary>
        /// <param name="tpNome">Tipo de nome (1-Nome Completo, 2-Razão Social)</param>
        /// <returns>Builder para continuar a configuração</returns>
        public InfoPatrocinadoBuilder ComTpNome(string tpNome)
        {
            _infoPatrocinado.tpNome = tpNome;
            return this;
        }

        /// <summary>
        /// Configura o endereço do patrocinado
        /// </summary>
        /// <param name="configurador">Action para configurar o endereço</param>
        /// <returns>Builder para continuar a configuração</returns>
        public InfoPatrocinadoBuilder ComEndereco(Action<EnderecoBuilder> configurador)
        {
            ArgumentNullException.ThrowIfNull(configurador);

            var builder = new EnderecoBuilder();
            configurador(builder);
            _infoPatrocinado.endereco = builder.Build();
            return this;
        }

        /// <summary>
        /// Define o tipo de endereço
        /// </summary>
        /// <param name="tpEndereco">Tipo de endereço (1-Residencial, 2-Comercial)</param>
        /// <returns>Builder para continuar a configuração</returns>
        public InfoPatrocinadoBuilder ComTpEndereco(string tpEndereco)
        {
            _infoPatrocinado.tpEndereco = tpEndereco;
            return this;
        }

        /// <summary>
        /// Adiciona um país de residência do patrocinado
        /// </summary>
        /// <param name="configurador">Action para configurar o país</param>
        /// <returns>Builder para continuar a configuração</returns>
        public InfoPatrocinadoBuilder ComPaisResidencia(Action<PaisResidBuilder> configurador)
        {
            ArgumentNullException.ThrowIfNull(configurador);

            var builder = new PaisResidBuilder();
            configurador(builder);
            _paisesResid.Add(builder.Build());
            return this;
        }

        /// <summary>
        /// Constrói o objeto InfoPatrocinado
        /// </summary>
        /// <returns>Objeto construído</returns>
        internal eFinanceiraEvtCadPatrocinadoInfoPatrocinado Build()
        {
            // Adicionar NIFs se houver
            if (_nifs.Count > 0)
            {
                _infoPatrocinado.NIF = _nifs.ToArray();
            }

            // Adicionar endereços alternativos se houver
            if (_enderecosOutros.Count > 0)
            {
                _infoPatrocinado.EnderecoOutros = _enderecosOutros.ToArray();
            }

            // Adicionar países de residência se houver
            if (_paisesResid.Count > 0)
            {
                _infoPatrocinado.paisResid = _paisesResid.ToArray();
            }

            return _infoPatrocinado;
        }
    }

    /// <summary>
    /// Builder para configuração de NIF (Número de Identificação Fiscal)
    /// </summary>
    public sealed class NIFBuilder
    {
        private readonly eFinanceiraEvtCadPatrocinadoInfoPatrocinadoNIF _nif;

        /// <summary>
        /// Initializes a new instance of the <see cref="NIFBuilder"/> class.
        /// </summary>
        public NIFBuilder()
        {
            _nif = new eFinanceiraEvtCadPatrocinadoInfoPatrocinadoNIF();
        }

        /// <summary>
        /// Define o país emissor do NIF
        /// </summary>
        /// <param name="paisEmissorNIF">Código do país emissor do NIF (ISO 3166-1 alpha-2)</param>
        /// <returns>Builder para continuar a configuração</returns>
        public NIFBuilder ComPaisEmissorNIF(string paisEmissorNIF)
        {
            _nif.PaisEmissao = paisEmissorNIF;
            return this;
        }

        /// <summary>
        /// Define o número do NIF
        /// </summary>
        /// <param name="numeroNIF">Número do NIF</param>
        /// <returns>Builder para continuar a configuração</returns>
        public NIFBuilder ComNumeroNIF(string numeroNIF)
        {
            _nif.NumeroNIF = numeroNIF;
            return this;
        }

        /// <summary>
        /// Constrói o objeto NIF
        /// </summary>
        /// <returns>Objeto construído</returns>
        internal eFinanceiraEvtCadPatrocinadoInfoPatrocinadoNIF Build()
        {
            return _nif;
        }
    }

    /// <summary>
    /// Builder para configuração de endereço
    /// </summary>
    public sealed class EnderecoBuilder
    {
        private readonly eFinanceiraEvtCadPatrocinadoInfoPatrocinadoEndereco _endereco;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnderecoBuilder"/> class.
        /// </summary>
        public EnderecoBuilder()
        {
            _endereco = new eFinanceiraEvtCadPatrocinadoInfoPatrocinadoEndereco();
        }

        /// <summary>
        /// Define o endereço em formato livre
        /// </summary>
        /// <param name="enderecoLivre">Endereço completo em formato livre</param>
        /// <returns>Builder para continuar a configuração</returns>
        public EnderecoBuilder ComEnderecoLivre(string enderecoLivre)
        {
            _endereco.enderecoLivre = enderecoLivre;
            return this;
        }

        /// <summary>
        /// Define o município
        /// </summary>
        /// <param name="municipio">Nome do município</param>
        /// <returns>Builder para continuar a configuração</returns>
        public EnderecoBuilder ComMunicipio(string municipio)
        {
            _endereco.municipio = municipio;
            return this;
        }

        /// <summary>
        /// Define o país
        /// </summary>
        /// <param name="pais">Código do país (ISO 3166-1 alpha-2)</param>
        /// <returns>Builder para continuar a configuração</returns>
        public EnderecoBuilder ComPais(string pais)
        {
            _endereco.pais = pais;
            return this;
        }

        /// <summary>
        /// Constrói o objeto Endereco
        /// </summary>
        /// <returns>Objeto construído</returns>
        internal eFinanceiraEvtCadPatrocinadoInfoPatrocinadoEndereco Build()
        {
            return _endereco;
        }
    }

    /// <summary>
    /// Builder para configuração de país de residência
    /// </summary>
    public sealed class PaisResidBuilder
    {
        private readonly eFinanceiraEvtCadPatrocinadoInfoPatrocinadoPaisResid _paisResid;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaisResidBuilder"/> class.
        /// </summary>
        public PaisResidBuilder()
        {
            _paisResid = new eFinanceiraEvtCadPatrocinadoInfoPatrocinadoPaisResid();
        }

        /// <summary>
        /// Define o país de residência
        /// </summary>
        /// <param name="pais">Código do país de residência (ISO 3166-1 alpha-2)</param>
        /// <returns>Builder para continuar a configuração</returns>
        public PaisResidBuilder ComPais(string pais)
        {
            _paisResid.Pais = pais;
            return this;
        }

        /// <summary>
        /// Constrói o objeto PaisResid
        /// </summary>
        /// <returns>Objeto construído</returns>
        internal eFinanceiraEvtCadPatrocinadoInfoPatrocinadoPaisResid Build()
        {
            return _paisResid;
        }
    }
}

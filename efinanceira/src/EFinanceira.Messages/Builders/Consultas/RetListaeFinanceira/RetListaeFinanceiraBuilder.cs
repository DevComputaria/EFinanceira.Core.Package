using System;
using System.Collections.Generic;
using System.Linq;
using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Generated.Consultas.RetListaeFinanceira;

namespace EFinanceira.Messages.Builders.Consultas.RetListaeFinanceira
{
    /// <summary>
    /// Builder fluente para construção de mensagens RetListaeFinanceira.
    /// Permite criar consultas de retorno de lista de e-Financeira de forma fluida e validada.
    /// </summary>
    public class RetListaeFinanceiraBuilder
    {
        private readonly eFinanceira _consulta;
        private readonly string _version;

        public RetListaeFinanceiraBuilder(string version = "v1_2_0")
        {
            _version = version;
            _consulta = new eFinanceira
            {
                retornoConsultaListaEFinanceira = new eFinanceiraRetornoConsultaListaEFinanceira
                {
                    dhProcessamento = DateTime.Now
                }
            };
        }

        /// <summary>
        /// Define a data e hora de processamento da consulta.
        /// </summary>
        /// <param name="dataHoraProcessamento">Data e hora do processamento</param>
        /// <returns>Builder para encadeamento fluente</returns>
        public RetListaeFinanceiraBuilder WithDataHoraProcessamento(DateTime dataHoraProcessamento)
        {
            if (_consulta.retornoConsultaListaEFinanceira != null)
            {
                _consulta.retornoConsultaListaEFinanceira.dhProcessamento = dataHoraProcessamento;
            }
            return this;
        }

        /// <summary>
        /// Define o status da consulta.
        /// </summary>
        /// <param name="cdRetorno">Código de retorno</param>
        /// <param name="descRetorno">Descrição do retorno</param>
        /// <returns>Builder para encadeamento fluente</returns>
        public RetListaeFinanceiraBuilder WithStatus(string cdRetorno, string? descRetorno = null)
        {
            if (_consulta.retornoConsultaListaEFinanceira != null)
            {
                _consulta.retornoConsultaListaEFinanceira.status = new TStatus
                {
                    cdRetorno = cdRetorno,
                    descRetorno = descRetorno
                };
            }
            return this;
        }

        /// <summary>
        /// Define a empresa declarante.
        /// </summary>
        /// <param name="cnpj">CNPJ da empresa declarante</param>
        /// <returns>Builder para encadeamento fluente</returns>
        public RetListaeFinanceiraBuilder WithEmpresaDeclarante(string cnpj)
        {
            if (_consulta.retornoConsultaListaEFinanceira != null)
            {
                _consulta.retornoConsultaListaEFinanceira.identificacaoEmpresaDeclarante = new TIdeEmpresaDeclarante
                {
                    cnpjEmpresaDeclarante = cnpj
                };
            }
            return this;
        }

        /// <summary>
        /// Adiciona informações de e-Financeira.
        /// </summary>
        /// <param name="dhInicial">Data e hora inicial</param>
        /// <param name="dhFinal">Data e hora final</param>
        /// <param name="situacao">Situação da e-Financeira</param>
        /// <param name="numeroReciboAbertura">Número do recibo de abertura</param>
        /// <param name="idAbertura">ID de abertura</param>
        /// <param name="numeroReciboFechamento">Número do recibo de fechamento</param>
        /// <param name="idFechamento">ID de fechamento</param>
        /// <returns>Builder para encadeamento fluente</returns>
        public RetListaeFinanceiraBuilder AddInformacoesEFinanceira(
            DateTime dhInicial,
            DateTime dhFinal,
            string? situacao = null,
            string? numeroReciboAbertura = null,
            string? idAbertura = null,
            string? numeroReciboFechamento = null,
            string? idFechamento = null)
        {
            if (_consulta.retornoConsultaListaEFinanceira != null)
            {
                var informacoes = new TInformacoesEFinanceira
                {
                    dhInicial = dhInicial,
                    dhFinal = dhFinal,
                    situacaoEFinanceira = situacao,
                    numeroReciboAbertura = numeroReciboAbertura,
                    idAbertura = idAbertura,
                    numeroReciboFechamento = numeroReciboFechamento,
                    idFechamento = idFechamento
                };

                var listaAtual = _consulta.retornoConsultaListaEFinanceira.informacoesEFinanceira?.ToList() ?? new List<TInformacoesEFinanceira>();
                listaAtual.Add(informacoes);
                _consulta.retornoConsultaListaEFinanceira.informacoesEFinanceira = listaAtual.ToArray();
            }
            return this;
        }

        /// <summary>
        /// Adiciona uma ocorrência ao status.
        /// </summary>
        /// <param name="tipo">Tipo da ocorrência</param>
        /// <param name="localizacao">Localização do erro/aviso</param>
        /// <param name="codigo">Código da ocorrência</param>
        /// <param name="descricao">Descrição da ocorrência</param>
        /// <returns>Builder para encadeamento fluente</returns>
        public RetListaeFinanceiraBuilder AddOcorrencia(string? tipo = null, string? localizacao = null, string? codigo = null, string? descricao = null)
        {
            if (_consulta.retornoConsultaListaEFinanceira?.status != null)
            {
                var ocorrencia = new TRegistroOcorrenciasOcorrencias
                {
                    tipo = tipo,
                    localizacaoErroAviso = localizacao,
                    codigo = codigo,
                    descricao = descricao
                };

                var listaAtual = _consulta.retornoConsultaListaEFinanceira.status.dadosRegistroOcorrenciaEvento?.ToList() ?? new List<TRegistroOcorrenciasOcorrencias>();
                listaAtual.Add(ocorrencia);
                _consulta.retornoConsultaListaEFinanceira.status.dadosRegistroOcorrenciaEvento = listaAtual.ToArray();
            }
            return this;
        }

        /// <summary>
        /// Constrói a mensagem RetListaeFinanceira validada.
        /// </summary>
        /// <returns>Mensagem RetListaeFinanceira construída</returns>
        /// <exception cref="InvalidOperationException">Quando campos obrigatórios não foram configurados</exception>
        public RetListaeFinanceiraMessage Build()
        {
            ValidarCamposObrigatorios();
            return new RetListaeFinanceiraMessage(_consulta, _version);
        }

        private void ValidarCamposObrigatorios()
        {
            if (_consulta.retornoConsultaListaEFinanceira?.status == null)
                throw new InvalidOperationException("Status é obrigatório. Use WithStatus() para defini-lo.");

            if (_consulta.retornoConsultaListaEFinanceira?.identificacaoEmpresaDeclarante == null)
                throw new InvalidOperationException("Empresa declarante é obrigatória. Use WithEmpresaDeclarante() para defini-la.");
        }

        private static string GenerateId() => $"RETLISTA_{DateTime.Now:yyyyMMddHHmmss}_{Random.Shared.Next(1000, 9999)}";
    }

#pragma warning disable SA1402 // File may only contain a single type
    /// <summary>
    /// Mensagem de consulta de lista de e-Financeira
    /// </summary>
    public sealed class RetListaeFinanceiraMessage : IEFinanceiraMessage
    {
        public string Version { get; }
        public string RootElementName => "eFinanceira";
        public string? IdAttributeName => null;
        public string? IdValue { get; internal set; }
        public object Payload => Consulta;

        /// <summary>
        /// Consulta tipada gerada do XSD
        /// </summary>
        public eFinanceira Consulta { get; }

        internal RetListaeFinanceiraMessage(eFinanceira consulta, string version)
        {
            Consulta = consulta;
            Version = version;
        }
    }
}

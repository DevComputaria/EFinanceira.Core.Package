using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Generated.Consultas.RetListaeFinanceira;

namespace EFinanceira.Messages.Builders.Consultas.RetListaeFinanceira
{
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

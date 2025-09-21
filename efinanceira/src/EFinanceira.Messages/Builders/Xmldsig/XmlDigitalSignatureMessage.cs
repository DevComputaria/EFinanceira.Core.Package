using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Generated.Xmldsig.Core;

namespace EFinanceira.Messages.Builders.Xmldsig
{
    /// <summary>
    /// Mensagem wrapper para assinatura digital XML
    /// </summary>
    public sealed class XmlDigitalSignatureMessage : IEFinanceiraMessage
    {
        public string Version { get; }
        public string RootElementName => "Signature";
        public string? IdAttributeName => "Id";
        public string? IdValue { get; }
        public object Payload => Signature;

        /// <summary>
        /// Assinatura digital tipada gerada do XSD
        /// </summary>
        public SignatureType Signature { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDigitalSignatureMessage"/> class.
        /// </summary>
        internal XmlDigitalSignatureMessage(SignatureType signature, string version = "core")
        {
            Signature = signature;
            Version = version;
            IdValue = signature.Id;
        }
    }
}

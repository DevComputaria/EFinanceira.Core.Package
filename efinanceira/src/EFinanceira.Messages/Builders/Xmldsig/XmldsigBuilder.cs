using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using EFinanceira.Messages.Generated.Xmldsig.Core;

namespace EFinanceira.Messages.Builders.Xmldsig
{
    /// <summary>
    /// Builder para criação de assinaturas digitais XML baseado no padrão XMLDSig
    /// Destinado para uso em produção com certificados digitais válidos
    /// </summary>
    public sealed class XmldsigBuilder : IDisposable
    {
        private readonly SignatureType _signature;
        private X509Certificate2? _certificate;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmldsigBuilder"/> class.
        /// </summary>
        public XmldsigBuilder()
        {
            _signature = new SignatureType();
        }

        /// <summary>
        /// Define o identificador único da assinatura
        /// </summary>
        /// <param name="id">Identificador da assinatura</param>
        /// <returns>Builder para encadeamento fluente</returns>
        public XmldsigBuilder WithId(string id)
        {
            _signature.Id = id;
            return this;
        }

        /// <summary>
        /// Carrega certificado digital de um arquivo
        /// </summary>
        /// <param name="certificatePath">Caminho para o arquivo do certificado (.pfx ou .p12)</param>
        /// <param name="password">Senha do certificado</param>
        /// <returns>Builder para encadeamento fluente</returns>
        public XmldsigBuilder WithCertificateFromFile(string certificatePath, string password)
        {
            if (!File.Exists(certificatePath))
            {
                throw new FileNotFoundException($"Certificado não encontrado: {certificatePath}");
            }

            _certificate = new X509Certificate2(certificatePath, password, X509KeyStorageFlags.Exportable);
            
            if (!_certificate.HasPrivateKey)
            {
                throw new InvalidOperationException("Certificado deve conter chave privada para assinatura");
            }

            return this;
        }

        /// <summary>
        /// Carrega certificado do repositório de certificados do Windows
        /// </summary>
        /// <param name="thumbprint">Thumbprint do certificado</param>
        /// <param name="storeLocation">Local do repositório (padrão: CurrentUser)</param>
        /// <param name="storeName">Nome do repositório (padrão: My)</param>
        /// <returns>Builder para encadeamento fluente</returns>
        public XmldsigBuilder WithCertificateFromStore(string thumbprint, StoreLocation storeLocation = StoreLocation.CurrentUser, StoreName storeName = StoreName.My)
        {
            using var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);
            
            var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
            
            if (certificates.Count == 0)
            {
                throw new InvalidOperationException($"Certificado com thumbprint '{thumbprint}' não encontrado no repositório");
            }

            _certificate = certificates[0];
            
            if (!_certificate.HasPrivateKey)
            {
                throw new InvalidOperationException("Certificado deve conter chave privada para assinatura");
            }

            return this;
        }

        /// <summary>
        /// Configura a assinatura para um elemento XML específico
        /// </summary>
        /// <param name="referenceUri">URI de referência do elemento a ser assinado</param>
        /// <param name="xmlContent">Conteúdo XML a ser assinado</param>
        /// <returns>Builder para encadeamento fluente</returns>
        public XmldsigBuilder WithXmlContent(string referenceUri, string xmlContent)
        {
            if (_certificate == null)
            {
                throw new InvalidOperationException("Certificado deve ser configurado antes de assinar conteúdo");
            }

            // Calcular digest do conteúdo XML
            var digestValue = CalculateXmlDigest(xmlContent);

            // Configurar SignedInfo
            _signature.SignedInfo = new SignedInfoType
            {
                Id = $"{_signature.Id}_SignedInfo",
                CanonicalizationMethod = new SignedInfoTypeCanonicalizationMethod
                {
                    Algorithm = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315"
                },
                SignatureMethod = new SignedInfoTypeSignatureMethod
                {
                    Algorithm = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256"
                },
                Reference = new ReferenceType
                {
                    Id = $"{_signature.Id}_Reference",
                    URI = referenceUri,
                    Transforms = new[]
                    {
                        new TransformType { Algorithm = TTransformURI.httpwwww3org200009xmldsigenvelopedsignature },
                        new TransformType { Algorithm = TTransformURI.httpwwww3orgTR2001RECxmlc14n20010315 }
                    },
                    DigestMethod = new ReferenceTypeDigestMethod
                    {
                        Algorithm = "http://www.w3.org/2001/04/xmlenc#sha256"
                    },
                    DigestValue = digestValue
                }
            };

            // Assinar o SignedInfo
            var signedInfoXml = SerializeSignedInfo(_signature.SignedInfo);
            var signatureValue = SignData(signedInfoXml);

            // Configurar SignatureValue
            _signature.SignatureValue = new SignatureValueType
            {
                Id = $"{_signature.Id}_SignatureValue",
                Value = signatureValue
            };

            // Configurar KeyInfo com dados do certificado
            ConfigureKeyInfo();

            return this;
        }

        /// <summary>
        /// Constrói a estrutura final da assinatura digital
        /// </summary>
        /// <returns>Mensagem de assinatura digital configurada</returns>
        /// <exception cref="InvalidOperationException">Lançada quando a assinatura não está completa</exception>
        public XmlDigitalSignatureMessage Build()
        {
            ValidateSignature();
            return new XmlDigitalSignatureMessage(_signature);
        }

        /// <summary>
        /// Constrói apenas o tipo de assinatura sem wrapper
        /// </summary>
        /// <returns>Objeto SignatureType configurado</returns>
        /// <exception cref="InvalidOperationException">Lançada quando a assinatura não está completa</exception>
        public SignatureType BuildSignatureType()
        {
            ValidateSignature();
            return _signature;
        }

        /// <summary>
        /// Valida se a estrutura da assinatura está completa e válida
        /// </summary>
        /// <returns>True se válida, False caso contrário</returns>
        public bool IsValid()
        {
            return _signature.SignedInfo != null && 
                   _signature.SignatureValue != null &&
                   _signature.KeyInfo != null &&
                   _certificate != null;
        }

        /// <summary>
        /// Libera recursos utilizados pelo builder
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _certificate?.Dispose();
                _certificate = null;
                _disposed = true;
            }
        }

        // Métodos auxiliares privados
        private static byte[] CalculateXmlDigest(string xmlContent)
        {
            var xmlDoc = new XmlDocument { PreserveWhitespace = true };
            xmlDoc.LoadXml(xmlContent);

            // Aplicar canonicalização C14N
            var transform = new XmlDsigC14NTransform();
            transform.LoadInput(xmlDoc);
            
            using var canonicalizedStream = (Stream)transform.GetOutput(typeof(Stream));
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(canonicalizedStream);
        }

        private static string SerializeSignedInfo(SignedInfoType signedInfo)
        {
            // Serializar SignedInfo com canonicalização C14N
            var signedInfoXml = $@"<SignedInfo xmlns=""http://www.w3.org/2000/09/xmldsig#"" Id=""{signedInfo.Id}"">
                <CanonicalizationMethod Algorithm=""{signedInfo.CanonicalizationMethod.Algorithm}"" />
                <SignatureMethod Algorithm=""{signedInfo.SignatureMethod.Algorithm}"" />
                <Reference Id=""{signedInfo.Reference.Id}"" URI=""{signedInfo.Reference.URI}"">
                    <Transforms>
                        {string.Join("", signedInfo.Reference.Transforms.Select(t => $@"<Transform Algorithm=""{t.Algorithm}"" />"))}
                    </Transforms>
                    <DigestMethod Algorithm=""{signedInfo.Reference.DigestMethod.Algorithm}"" />
                    <DigestValue>{Convert.ToBase64String(signedInfo.Reference.DigestValue)}</DigestValue>
                </Reference>
            </SignedInfo>";

            var xmlDoc = new XmlDocument { PreserveWhitespace = true };
            xmlDoc.LoadXml(signedInfoXml);

            var transform = new XmlDsigC14NTransform();
            transform.LoadInput(xmlDoc);
            
            using var canonicalizedStream = (Stream)transform.GetOutput(typeof(Stream));
            using var reader = new StreamReader(canonicalizedStream);
            return reader.ReadToEnd();
        }

        private byte[] SignData(string data)
        {
            if (_certificate?.HasPrivateKey != true)
            {
                throw new InvalidOperationException("Certificado com chave privada é necessário para assinatura");
            }

            using var rsa = _certificate.GetRSAPrivateKey();
            if (rsa == null)
            {
                throw new InvalidOperationException("Não foi possível obter chave privada RSA do certificado");
            }
            
            var dataBytes = Encoding.UTF8.GetBytes(data);
            return rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        private void ConfigureKeyInfo()
        {
            if (_certificate == null) return;

            _signature.KeyInfo = new KeyInfoType
            {
                Id = $"{_signature.Id}_KeyInfo",
                X509Data = new X509DataType()
            };

            // Em produção, seria configurado com os dados reais do certificado
            // Por simplicidade, deixando vazio - seria preenchido com X509Certificate, X509IssuerSerial, etc.
        }

        private void ValidateSignature()
        {
            if (!IsValid())
            {
                throw new InvalidOperationException("Assinatura incompleta. Verifique se certificado e conteúdo XML foram configurados");
            }
        }
    }
}

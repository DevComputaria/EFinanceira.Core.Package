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
    /// Implementado conforme exemplo oficial da Receita Federal para e-Financeira
    /// </summary>
    public sealed class XmldsigBuilder : IDisposable
    {
        // Constantes dos algoritmos conforme especificação e-Financeira
        private const string SignatureMethodSha256 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
        private const string SignatureMethodSha1 = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
        private const string DigestMethodSha256 = "http://www.w3.org/2001/04/xmlenc#sha256";
        private const string DigestMethodSha1 = "http://www.w3.org/2000/09/xmldsig#sha1";
        private const string AtributoId = "id";

        private X509Certificate2? _certificate;
        private bool _disposed;

        static XmldsigBuilder()
        {
            // Garantir que os algoritmos RSA-SHA256 estejam disponíveis
            // No .NET moderno, estes algoritmos já estão registrados por padrão
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmldsigBuilder"/> class.
        /// </summary>
        public XmldsigBuilder()
        {
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
            
            ValidateCertificate();
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
            ValidateCertificate();
            return this;
        }

        /// <summary>
        /// Seleciona certificado interativamente do repositório do Windows (para aplicações desktop)
        /// </summary>
        /// <returns>Builder para encadeamento fluente</returns>
        public XmldsigBuilder WithInteractiveCertificateSelection()
        {
            var store = new X509Store("MY", StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            
            var certs = store.Certificates;
            var certsParaAssinatura = certs.Find(X509FindType.FindByKeyUsage, X509KeyUsageFlags.DigitalSignature, false);
            
            if (certsParaAssinatura.Count == 0)
            {
                throw new InvalidOperationException("Nenhum certificado digital encontrado para assinatura");
            }

            // Para aplicações console/server, usar o primeiro certificado encontrado
            _certificate = certsParaAssinatura[0];
            ValidateCertificate();
            
            store.Close();
            return this;
        }

        /// <summary>
        /// Assina um documento XML inteiro (para lotes de eventos)
        /// </summary>
        /// <param name="xmlDocument">Documento XML a ser assinado</param>
        /// <returns>Documento XML assinado</returns>
        public XmlDocument SignXmlDocument(XmlDocument xmlDocument)
        {
            if (_certificate == null)
            {
                throw new InvalidOperationException("Certificado deve ser configurado antes de assinar");
            }

            // Verifica se XML possui eventos e-Financeira
            var nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("eFinanceira", xmlDocument.DocumentElement?.NamespaceURI ?? "");
            var eventos = xmlDocument.SelectNodes("//eFinanceira:loteEventos/eFinanceira:evento", nsmgr);

            if (eventos?.Count <= 0)
            {
                throw new InvalidOperationException("Não foram encontrados eventos no XML para assinar");
            }

            // Assina cada evento do arquivo            
            foreach (XmlNode node in eventos!)
            {
                var xmlDocEvento = new XmlDocument();
                xmlDocEvento.LoadXml(node.InnerXml);
                  
                var tagEventoParaAssinar = ObtemTagEventoAssinar(xmlDocEvento);

                if (string.IsNullOrWhiteSpace(tagEventoParaAssinar))
                {
                    throw new InvalidOperationException($"Tipo de evento inválido para e-Financeira: '{tagEventoParaAssinar}'");
                }

                var xmlDocEventoAssinado = AssinarXmlEvento(xmlDocEvento, tagEventoParaAssinar);
                
                if (xmlDocEventoAssinado == null)
                {
                    throw new InvalidOperationException("Falha ao assinar evento XML");
                }

                node.InnerXml = xmlDocEventoAssinado.InnerXml;
            }

            return xmlDocument;
        }

        /// <summary>
        /// Assina um evento XML específico
        /// </summary>
        /// <param name="xmlEvento">XML do evento a ser assinado</param>
        /// <param name="tagEvento">Tag do tipo de evento (ex: evtAberturaeFinanceira)</param>
        /// <returns>XML do evento assinado</returns>
        public XmlDocument SignXmlEvent(XmlDocument xmlEvento, string tagEvento)
        {
            if (_certificate == null)
            {
                throw new InvalidOperationException("Certificado deve ser configurado antes de assinar");
            }

            return AssinarXmlEvento(xmlEvento, tagEvento);
        }

        /// <summary>
        /// Assina um evento XML (detecta automaticamente o tipo)
        /// </summary>
        /// <param name="xmlEvento">XML do evento a ser assinado</param>
        /// <returns>XML do evento assinado</returns>
        public XmlDocument SignXmlEvent(XmlDocument xmlEvento)
        {
            var tagEvento = ObtemTagEventoAssinar(xmlEvento);
            if (string.IsNullOrWhiteSpace(tagEvento))
            {
                throw new InvalidOperationException("Não foi possível detectar o tipo de evento no XML");
            }

            return SignXmlEvent(xmlEvento, tagEvento);
        }

        /// <summary>
        /// Valida a assinatura de um documento XML
        /// </summary>
        /// <param name="xmlAssinado">Documento XML assinado</param>
        /// <returns>True se todas as assinaturas são válidas</returns>
        public static bool ValidateSignature(XmlDocument xmlAssinado)
        {
            try
            {
                var nodeList = xmlAssinado.GetElementsByTagName("Signature", "*");

                if (nodeList.Count <= 0)
                {
                    return false;
                }

                foreach (XmlNode assinatura in nodeList)
                {
                    var evento = new XmlDocument { PreserveWhitespace = true };
                    evento.LoadXml(assinatura.ParentNode!.OuterXml);

                    var signedXml = new SignedXml(evento);
                    signedXml.LoadXml((XmlElement)assinatura);

                    // Carregar certificado do XML
                    var pubKey = signedXml.KeyInfo.GetXml().InnerText;
                    var pubKeyBytes = Convert.FromBase64String(pubKey);
                    var x509 = new X509Certificate2(pubKeyBytes);

                    // Verifica se a assinatura é RSA-SHA256 ou RSA-SHA1
                    if (!string.Equals(signedXml.SignatureMethod, SignatureMethodSha256, StringComparison.Ordinal) &&
                        !string.Equals(signedXml.SignatureMethod, SignatureMethodSha1, StringComparison.Ordinal))
                    {
                        return false;
                    }

                    // Valida assinatura
                    if (!signedXml.CheckSignature(x509, false))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Constrói uma mensagem XmlDigitalSignature básica (para compatibilidade)
        /// </summary>
        /// <param name="id">ID da assinatura</param>
        /// <returns>Mensagem de assinatura digital</returns>
        public static XmlDigitalSignatureMessage BuildBasicSignature(string id)
        {
            var signature = new SignatureType
            {
                Id = id,
                SignedInfo = new SignedInfoType
                {
                    Id = $"{id}_SignedInfo",
                    CanonicalizationMethod = new SignedInfoTypeCanonicalizationMethod
                    {
                        Algorithm = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315"
                    },
                    SignatureMethod = new SignedInfoTypeSignatureMethod
                    {
                        Algorithm = SignatureMethodSha256
                    }
                },
                SignatureValue = new SignatureValueType
                {
                    Id = $"{id}_SignatureValue",
                    Value = Array.Empty<byte>() // Placeholder
                },
                KeyInfo = new KeyInfoType
                {
                    Id = $"{id}_KeyInfo",
                    X509Data = new X509DataType()
                }
            };

            return new XmlDigitalSignatureMessage(signature);
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

        // Métodos privados auxiliares
        private void ValidateCertificate()
        {
            if (_certificate == null) return;

            if (!_certificate.HasPrivateKey)
            {
                throw new InvalidOperationException("Certificado deve conter chave privada para assinatura");
            }

            // Verifica se certificado tem uso para assinatura digital
            var keyUsageExt = _certificate.Extensions
                .OfType<X509KeyUsageExtension>()
                .FirstOrDefault();

            if (keyUsageExt != null && !keyUsageExt.KeyUsages.HasFlag(X509KeyUsageFlags.DigitalSignature))
            {
                throw new InvalidOperationException("Certificado não autorizado para assinatura digital");
            }
        }

        private static string ObtemTagEventoAssinar(XmlDocument arquivo)
        {
            var xml = arquivo.OuterXml;
            
            if (xml.Contains("evtCadDeclarante")) return "evtCadDeclarante";
            if (xml.Contains("evtAberturaeFinanceira")) return "evtAberturaeFinanceira";
            if (xml.Contains("evtCadIntermediario")) return "evtCadIntermediario";
            if (xml.Contains("evtCadPatrocinado")) return "evtCadPatrocinado";
            if (xml.Contains("evtExclusaoeFinanceira")) return "evtExclusaoeFinanceira";
            if (xml.Contains("evtExclusao")) return "evtExclusao";
            if (xml.Contains("evtFechamentoeFinanceira")) return "evtFechamentoeFinanceira";
            if (xml.Contains("evtMovOpFin")) return "evtMovOpFin";
            if (xml.Contains("evtMovPP")) return "evtMovPP";
            
            return string.Empty;
        }

        private XmlDocument AssinarXmlEvento(XmlDocument xmlDocEvento, string tagEventoParaAssinar)
        {
            // Primeiro tentar SHA256, depois SHA1 como fallback
            Exception? lastException = null;
            
            // Tentar SHA256 primeiro (recomendado pela RF)
            try
            {
                return AssinarXmlEventoComAlgoritmo(xmlDocEvento, tagEventoParaAssinar, SignatureMethodSha256, DigestMethodSha256);
            }
            catch (CryptographicException ex) when (ex.Message.Contains("SignatureDescription"))
            {
                lastException = ex;
                
                // SHA256 falhou, tentar SHA1 como fallback
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Falha ao assinar XML evento: {ex.Message}", ex);
            }

            // Fallback para SHA1 se SHA256 não funcionou
            try
            {
                return AssinarXmlEventoComAlgoritmo(xmlDocEvento, tagEventoParaAssinar, SignatureMethodSha1, DigestMethodSha1);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Falha ao assinar XML evento (tentativas SHA256 e SHA1): {ex.Message}. Erro SHA256: {lastException?.Message}", ex);
            }
        }

        private XmlDocument AssinarXmlEventoComAlgoritmo(XmlDocument xmlDocEvento, string tagEventoParaAssinar, string signatureMethod, string digestMethod)
        {
            try
            {
                var nodeParaAssinatura = xmlDocEvento.GetElementsByTagName(tagEventoParaAssinar);
                if (nodeParaAssinatura.Count == 0)
                {
                    throw new InvalidOperationException($"Elemento '{tagEventoParaAssinar}' não encontrado no XML");
                }

                var elementoParaAssinar = (XmlElement)nodeParaAssinatura[0]!;
                
                // Criar SignedXml com documento ao invés do elemento
                var signedXml = new SignedXml(xmlDocEvento);
                
                // Configurar algoritmos explicitamente
                signedXml.SignedInfo!.SignatureMethod = signatureMethod;
                signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigCanonicalizationUrl;

                // Adicionar chave privada para assinar o documento
                using var chavePrivada = ObterChavePrivada(_certificate!);
                signedXml.SigningKey = chavePrivada;

                // Configurar referência
                var idAttribute = elementoParaAssinar.Attributes[AtributoId];
                if (idAttribute == null)
                {
                    throw new InvalidOperationException($"Elemento '{tagEventoParaAssinar}' deve ter atributo 'id'");
                }

                var reference = new Reference("#" + idAttribute.Value);
                reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
                reference.AddTransform(new XmlDsigC14NTransform());
                reference.DigestMethod = digestMethod;
                signedXml.AddReference(reference);

                // Configurar informações da chave
                var keyInfo = new KeyInfo();
                keyInfo.AddClause(new KeyInfoX509Data(_certificate));
                signedXml.KeyInfo = keyInfo;

                // Calcular assinatura
                signedXml.ComputeSignature();

                // Adicionar assinatura ao elemento 
                var xmlElementAssinado = signedXml.GetXml();
                elementoParaAssinar.AppendChild(xmlElementAssinado);

                // Retornar o documento original com a assinatura adicionada
                return xmlDocEvento;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Falha ao assinar XML evento: {ex.Message}", ex);
            }
        }

        private static RSA ObterChavePrivada(X509Certificate2 certificado)
        {
            // Verificar se certificado tem chave privada
            if (!certificado.HasPrivateKey)
            {
                throw new InvalidOperationException("Certificado não possui chave privada");
            }

            // Método moderno para obter chave privada
            var rsa = certificado.GetRSAPrivateKey();
            if (rsa == null)
            {
                throw new InvalidOperationException("Não foi possível obter chave privada RSA do certificado");
            }

            // Verificar algoritmo suportado
            if (rsa.SignatureAlgorithm == null)
            {
                throw new InvalidOperationException("Certificado não suporta algoritmos de assinatura RSA");
            }

            return rsa;
        }
    }
}

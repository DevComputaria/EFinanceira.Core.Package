// Exemplo de uso em produção:
var xmlSignature = new XmldsigBuilder()
    .WithId("AssinaturaEvento001")
    .WithCertificateFromFile(@"C:\Certs\certificado.pfx", "senha")
    .WithXmlContent("#EventoId", xmlContentToSign)
    .Build();
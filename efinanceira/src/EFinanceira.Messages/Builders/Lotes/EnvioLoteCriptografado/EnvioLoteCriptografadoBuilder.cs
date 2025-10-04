using System;
using System.Security.Cryptography;
using System.Text;
using EFinanceira.Core.Abstractions;
using EFinanceira.Messages.Generated.Lotes.EnvioLoteCriptografado;

namespace EFinanceira.Messages.Builders.Lotes.EnvioLoteCriptografado;

/// <summary>
/// Mensagem de lote criptografado para envio ao e-Financeira
/// </summary>
public sealed class EnvioLoteCriptografadoMessage : IEFinanceiraMessage
{
    public string Version { get; private set; }
    public string RootElementName => "eFinanceira";
    public string? IdAttributeName => null;
    public string? IdValue { get; internal set; }
    public object Payload => EFinanceira;

    /// <summary>
    /// Evento tipado gerado do XSD
    /// </summary>
    public eFinanceira EFinanceira { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnvioLoteCriptografadoMessage"/> class.
    /// Construtor sem parâmetros necessário para serialização XML.
    /// </summary>
    public EnvioLoteCriptografadoMessage()
    {
        Version = "v1_2_0";
        EFinanceira = new eFinanceira();
        IdValue = string.Empty;
    }

    internal EnvioLoteCriptografadoMessage(eFinanceira eFinanceira, string version)
    {
        EFinanceira = eFinanceira;
        Version = version;
        IdValue = eFinanceira.loteCriptografado?.id;
    }
}

/// <summary>
/// Builder principal para construção de lotes criptografados e-Financeira.
/// Utiliza o padrão fluent interface para facilitar a criação de lotes criptografados estruturados.
/// </summary>
public sealed class EnvioLoteCriptografadoBuilder : IMessageBuilder<EnvioLoteCriptografadoMessage>
{
    private readonly eFinanceira _eFinanceira;
    private readonly string _version;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnvioLoteCriptografadoBuilder"/> class.
    /// </summary>
    /// <param name="version">Versão do schema (padrão: v1_2_0)</param>
    public EnvioLoteCriptografadoBuilder(string version = "v1_2_0")
    {
        _version = version;
        _eFinanceira = new eFinanceira
        {
            loteCriptografado = new eFinanceiraLoteCriptografado
            {
                id = GenerateId()
            }
        };
    }

    /// <summary>
    /// Cria uma nova instância do builder EnvioLoteCriptografado
    /// </summary>
    /// <param name="version">Versão do schema</param>
    /// <returns>Nova instância do builder</returns>
    public static EnvioLoteCriptografadoBuilder Create(string version = "v1_2_0") => new(version);

    /// <summary>
    /// Define o ID do lote criptografado
    /// </summary>
    /// <param name="id">ID único do lote</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EnvioLoteCriptografadoBuilder WithId(string id)
    {
        _eFinanceira.loteCriptografado.id = id;
        return this;
    }

    /// <summary>
    /// Configura o lote criptografado
    /// </summary>
    /// <param name="configAction">Ação para configurar o LoteCriptografado</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public EnvioLoteCriptografadoBuilder WithLoteCriptografado(Action<LoteCriptografadoBuilder> configAction)
    {
        var builder = new LoteCriptografadoBuilder(_eFinanceira.loteCriptografado);
        configAction(builder);
        _eFinanceira.loteCriptografado = builder.Build();
        return this;
    }

    /// <summary>
    /// Constrói a instância final da mensagem de lote criptografado
    /// </summary>
    /// <returns>Instância configurada de EnvioLoteCriptografadoMessage</returns>
    public EnvioLoteCriptografadoMessage Build()
    {
        ValidateRequiredFields();
        return new EnvioLoteCriptografadoMessage(_eFinanceira, _version);
    }

    /// <summary>
    /// Valida se todos os campos obrigatórios foram preenchidos
    /// </summary>
    private void ValidateRequiredFields()
    {
        if (_eFinanceira.loteCriptografado == null)
            throw new InvalidOperationException("LoteCriptografado é obrigatório");
        
        if (string.IsNullOrEmpty(_eFinanceira.loteCriptografado.id))
            throw new InvalidOperationException("Id do lote é obrigatório");
        
        if (string.IsNullOrEmpty(_eFinanceira.loteCriptografado.idCertificado))
            throw new InvalidOperationException("IdCertificado é obrigatório");
        
        if (string.IsNullOrEmpty(_eFinanceira.loteCriptografado.chave))
            throw new InvalidOperationException("Chave criptográfica é obrigatória");
        
        if (string.IsNullOrEmpty(_eFinanceira.loteCriptografado.lote))
            throw new InvalidOperationException("Lote criptografado é obrigatório");
    }

    /// <summary>
    /// Gera um ID único para o lote
    /// </summary>
    /// <returns>ID único no formato LOTE_GUID</returns>
    private static string GenerateId() => $"LOTE_{Guid.NewGuid():N}";
}

#region Builders auxiliares

/// <summary>
/// Builder para configuração do lote criptografado
/// </summary>
public sealed class LoteCriptografadoBuilder
{
    private readonly eFinanceiraLoteCriptografado _loteCriptografado;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoteCriptografadoBuilder"/> class.
    /// </summary>
    /// <param name="loteCriptografado">Instância existente do lote criptografado (opcional)</param>
    public LoteCriptografadoBuilder(eFinanceiraLoteCriptografado? loteCriptografado = null)
    {
        _loteCriptografado = loteCriptografado ?? new eFinanceiraLoteCriptografado();
    }

    /// <summary>
    /// Define o ID do lote criptografado
    /// </summary>
    /// <param name="id">Identificador único do lote</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public LoteCriptografadoBuilder WithId(string id)
    {
        _loteCriptografado.id = id;
        return this;
    }

    /// <summary>
    /// Define o ID do certificado digital
    /// </summary>
    /// <param name="idCertificado">Identificador do certificado usado na criptografia</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public LoteCriptografadoBuilder WithIdCertificado(string idCertificado)
    {
        _loteCriptografado.idCertificado = idCertificado;
        return this;
    }

    /// <summary>
    /// Define a chave criptográfica
    /// </summary>
    /// <param name="chave">Chave simétrica criptografada com a chave pública da Receita Federal</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public LoteCriptografadoBuilder WithChave(string chave)
    {
        _loteCriptografado.chave = chave;
        return this;
    }

    /// <summary>
    /// Define a chave criptográfica a partir de bytes
    /// </summary>
    /// <param name="chaveBytes">Bytes da chave criptográfica</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public LoteCriptografadoBuilder WithChave(byte[] chaveBytes)
    {
        _loteCriptografado.chave = Convert.ToBase64String(chaveBytes);
        return this;
    }

    /// <summary>
    /// Define o lote criptografado
    /// </summary>
    /// <param name="lote">Conteúdo do lote criptografado em Base64</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public LoteCriptografadoBuilder WithLote(string lote)
    {
        _loteCriptografado.lote = lote;
        return this;
    }

    /// <summary>
    /// Define o lote criptografado a partir de bytes
    /// </summary>
    /// <param name="loteBytes">Bytes do lote criptografado</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public LoteCriptografadoBuilder WithLote(byte[] loteBytes)
    {
        _loteCriptografado.lote = Convert.ToBase64String(loteBytes);
        return this;
    }

    /// <summary>
    /// Define o lote criptografado a partir de uma mensagem de lote não criptografado
    /// </summary>
    /// <param name="loteMessage">Mensagem de lote a ser criptografada</param>
    /// <param name="chaveAES">Chave AES para criptografia</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public LoteCriptografadoBuilder WithLoteFromMessage(IEFinanceiraMessage loteMessage, byte[] chaveAES)
    {
        // Serializar a mensagem para XML
        var xmlContent = SerializeToXml(loteMessage);
        
        // Criptografar o conteúdo
        var loteCriptografado = EncryptContent(xmlContent, chaveAES);
        
        _loteCriptografado.lote = Convert.ToBase64String(loteCriptografado);
        return this;
    }

    /// <summary>
    /// Gera uma chave AES aleatória e a define como chave criptográfica
    /// </summary>
    /// <param name="chaveAES">Retorna a chave AES gerada para uso na criptografia do lote</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public LoteCriptografadoBuilder WithChaveAESAleatoria(out byte[] chaveAES)
    {
        chaveAES = GenerateAESKey();
        _loteCriptografado.chave = Convert.ToBase64String(chaveAES);
        return this;
    }

    /// <summary>
    /// Configura tanto a chave quanto o lote de uma só vez, criptografando o lote com AES
    /// </summary>
    /// <param name="loteMessage">Mensagem de lote a ser criptografada</param>
    /// <param name="chaveAES">Chave AES para criptografia (se null, uma nova será gerada)</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public LoteCriptografadoBuilder WithLoteCriptografadoCompleto(IEFinanceiraMessage loteMessage, byte[]? chaveAES = null)
    {
        // Gerar chave se não fornecida
        chaveAES ??= GenerateAESKey();
        
        // Definir a chave
        WithChave(chaveAES);
        
        // Criptografar e definir o lote
        WithLoteFromMessage(loteMessage, chaveAES);
        
        return this;
    }

    /// <summary>
    /// Constrói o objeto LoteCriptografado
    /// </summary>
    /// <returns>Instância configurada de eFinanceiraLoteCriptografado</returns>
    internal eFinanceiraLoteCriptografado Build() => _loteCriptografado;

    /// <summary>
    /// Serializa uma mensagem para XML
    /// </summary>
    /// <param name="message">Mensagem a ser serializada</param>
    /// <returns>Conteúdo XML como string</returns>
    private static string SerializeToXml(IEFinanceiraMessage message)
    {
        // Implementação básica - em produção deveria usar o serializer apropriado
        // Por enquanto, retorna um placeholder
        return $"<{message.RootElementName}>{message.Payload}</{message.RootElementName}>";
    }

    /// <summary>
    /// Criptografa conteúdo usando AES
    /// </summary>
    /// <param name="content">Conteúdo a ser criptografado</param>
    /// <param name="key">Chave AES</param>
    /// <returns>Conteúdo criptografado</returns>
    private static byte[] EncryptContent(string content, byte[] key)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = key;
            aes.GenerateIV();
            
            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            {
                var contentBytes = Encoding.UTF8.GetBytes(content);
                
                // Criptografar conteúdo primeiro para saber o tamanho real
                var encrypted = encryptor.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                
                // Agora alocar array com tamanho correto
                var encryptedContent = new byte[aes.IV.Length + encrypted.Length];
                
                // Adicionar IV no início
                Array.Copy(aes.IV, 0, encryptedContent, 0, aes.IV.Length);
                
                // Adicionar conteúdo criptografado
                Array.Copy(encrypted, 0, encryptedContent, aes.IV.Length, encrypted.Length);
                
                return encryptedContent;
            }
        }
    }

    /// <summary>
    /// Gera uma chave AES aleatória
    /// </summary>
    /// <returns>Chave AES de 256 bits</returns>
    private static byte[] GenerateAESKey()
    {
        using (var aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.GenerateKey();
            return aes.Key;
        }
    }
}

#endregion

#region Extensões e Utilitários

/// <summary>
/// Extensões para facilitar o uso do builder de lotes criptografados
/// </summary>
public static class EnvioLoteCriptografadoBuilderExtensions
{
    /// <summary>
    /// Configura um lote criptografado completo a partir de uma mensagem de lote
    /// </summary>
    /// <param name="builder">Builder do lote criptografado</param>
    /// <param name="idCertificado">ID do certificado digital</param>
    /// <param name="loteMessage">Mensagem de lote a ser criptografada</param>
    /// <param name="chaveAES">Chave AES (se null, uma nova será gerada)</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public static EnvioLoteCriptografadoBuilder ComLoteCompleto(
        this EnvioLoteCriptografadoBuilder builder,
        string idCertificado,
        IEFinanceiraMessage loteMessage,
        byte[]? chaveAES = null)
    {
        return builder.WithLoteCriptografado(lote => lote
            .WithIdCertificado(idCertificado)
            .WithLoteCriptografadoCompleto(loteMessage, chaveAES));
    }

    /// <summary>
    /// Configura um lote criptografado com dados manuais
    /// </summary>
    /// <param name="builder">Builder do lote criptografado</param>
    /// <param name="idCertificado">ID do certificado digital</param>
    /// <param name="chave">Chave criptográfica em Base64</param>
    /// <param name="lote">Lote criptografado em Base64</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public static EnvioLoteCriptografadoBuilder ComDadosManuais(
        this EnvioLoteCriptografadoBuilder builder,
        string idCertificado,
        string chave,
        string lote)
    {
        return builder.WithLoteCriptografado(loteCripto => loteCripto
            .WithIdCertificado(idCertificado)
            .WithChave(chave)
            .WithLote(lote));
    }

    /// <summary>
    /// Configura um lote criptografado com bytes
    /// </summary>
    /// <param name="builder">Builder do lote criptografado</param>
    /// <param name="idCertificado">ID do certificado digital</param>
    /// <param name="chaveBytes">Bytes da chave criptográfica</param>
    /// <param name="loteBytes">Bytes do lote criptografado</param>
    /// <returns>Instância atual do builder para encadeamento</returns>
    public static EnvioLoteCriptografadoBuilder ComDadosBytes(
        this EnvioLoteCriptografadoBuilder builder,
        string idCertificado,
        byte[] chaveBytes,
        byte[] loteBytes)
    {
        return builder.WithLoteCriptografado(loteCripto => loteCripto
            .WithIdCertificado(idCertificado)
            .WithChave(chaveBytes)
            .WithLote(loteBytes));
    }
}

/// <summary>
/// Utilitários para criptografia de lotes
/// </summary>
public static class LoteCriptografiaUtils
{
    /// <summary>
    /// Criptografa uma mensagem de lote usando AES
    /// </summary>
    /// <param name="loteMessage">Mensagem de lote a ser criptografada</param>
    /// <param name="chaveAES">Chave AES para criptografia</param>
    /// <returns>Tupla com chave e lote criptografados em Base64</returns>
    public static (string ChaveBase64, string LoteBase64) CriptografarLote(
        IEFinanceiraMessage loteMessage, 
        byte[] chaveAES)
    {
        var loteBuilder = new LoteCriptografadoBuilder();
        
        loteBuilder
            .WithChave(chaveAES)
            .WithLoteFromMessage(loteMessage, chaveAES);
        
        var resultado = loteBuilder.Build();
        return (resultado.chave, resultado.lote);
    }

    /// <summary>
    /// Gera uma nova chave AES de 256 bits
    /// </summary>
    /// <returns>Chave AES</returns>
    public static byte[] GerarChaveAES()
    {
        using (var aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.GenerateKey();
            return aes.Key;
        }
    }

    /// <summary>
    /// Valida se uma string está em formato Base64 válido
    /// </summary>
    /// <param name="base64String">String a ser validada</param>
    /// <returns>True se válida</returns>
    public static bool IsValidBase64(string base64String)
    {
        try
        {
            if (string.IsNullOrEmpty(base64String))
                return false;
                
            Convert.FromBase64String(base64String);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

#endregion
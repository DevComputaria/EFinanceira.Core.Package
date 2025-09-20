using Xunit;
using EFinanceira.Core.Signing;
using System.Security.Cryptography.X509Certificates;

namespace EFinanceira.Tests;

/// <summary>
/// Tests for XML signing functionality.
/// </summary>
public class SigningTests
{
    [Fact]
    public void XmlSigner_Constructor_ShouldCreateInstance()
    {
        // Arrange & Act
        var signer = new XmlSigner();

        // Assert
        Assert.NotNull(signer);
    }

    [Fact]
    public void SignXml_WithNullXml_ShouldThrowArgumentException()
    {
        // Arrange
        var signer = new XmlSigner();
        var options = new SignOptions();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => signer.SignXml(null!, options));
    }

    [Fact]
    public void SignXml_WithEmptyXml_ShouldThrowArgumentException()
    {
        // Arrange
        var signer = new XmlSigner();
        var options = new SignOptions();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => signer.SignXml("", options));
    }

    [Fact]
    public void SignXml_WithNullOptions_ShouldThrowArgumentException()
    {
        // Arrange
        var signer = new XmlSigner();
        var xml = "<root>test</root>";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => signer.SignXml(xml, null!));
    }

    [Fact]
    public void VerifySignature_WithNullXml_ShouldReturnFalse()
    {
        // Arrange
        var signer = new XmlSigner();

        // Act
        var result = signer.VerifySignature(null!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void VerifySignature_WithEmptyXml_ShouldReturnFalse()
    {
        // Arrange
        var signer = new XmlSigner();

        // Act
        var result = signer.VerifySignature("");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void SignOptions_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var options = new SignOptions();

        // Assert
        Assert.True(options.IncludeCertificate);
        Assert.Equal("http://www.w3.org/2001/04/xmldsig-more#rsa-sha256", options.SignatureAlgorithm);
        Assert.Equal("http://www.w3.org/TR/2001/REC-xml-c14n-20010315", options.CanonicalizationAlgorithm);
        Assert.Equal("http://www.w3.org/2001/04/xmlenc#sha256", options.DigestAlgorithm);
    }
}
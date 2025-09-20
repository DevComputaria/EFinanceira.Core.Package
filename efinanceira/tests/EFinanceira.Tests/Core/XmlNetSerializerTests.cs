using EFinanceira.Core.Serialization;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace EFinanceira.Tests.Core;

/// <summary>
/// Testes unitários para a classe XmlNetSerializer
/// </summary>
public class XmlNetSerializerTests
{
    private readonly ITestOutputHelper _output;
    private readonly XmlNetSerializer _serializer;

    public XmlNetSerializerTests(ITestOutputHelper output)
    {
        _output = output;
        _serializer = new XmlNetSerializer();
    }

    [Fact]
    public void Serialize_DeveRetornarXmlValidoComHeader()
    {
        // Arrange
        var testObject = new TestClass { Id = "TEST_001", Nome = "Teste" };

        // Act
        var result = _serializer.Serialize(testObject);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().StartWith("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        result.Should().Contain("<TestClass");
        result.Should().Contain("<Id>TEST_001</Id>");
        result.Should().Contain("<Nome>Teste</Nome>");

        _output.WriteLine($"XML gerado: {result}");
    }

    [Fact]
    public void SerializeForSigning_DeveRetornarXmlSemHeader()
    {
        // Arrange
        var testObject = new TestClass { Id = "TEST_SIGN_001", Nome = "Teste Assinatura" };

        // Act
        var result = _serializer.SerializeForSigning(testObject);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().NotStartWith("<?xml");
        result.Should().StartWith("<TestClass");
        result.Should().Contain("<Id>TEST_SIGN_001</Id>");
        result.Should().NotContain("\r\n"); // Não deve ter quebras de linha
        result.Should().NotContain("  "); // Não deve ter indentação

        _output.WriteLine($"XML para assinatura: {result}");
    }

    [Fact]
    public void Deserialize_DeveReconstruirObjetoCorretamente()
    {
        // Arrange
        var originalObject = new TestClass { Id = "TEST_DESERIALIZE", Nome = "Teste Deserialização" };
        var xml = _serializer.Serialize(originalObject);

        // Act
        var result = _serializer.Deserialize<TestClass>(xml);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("TEST_DESERIALIZE");
        result.Nome.Should().Be("Teste Deserialização");

        _output.WriteLine($"Objeto deserializado: Id={result.Id}, Nome={result.Nome}");
    }

    [Fact]
    public void Deserialize_ComTipoGenerico_DeveReconstruirObjetoCorretamente()
    {
        // Arrange
        var originalObject = new TestClass { Id = "TEST_GENERIC", Nome = "Teste Genérico" };
        var xml = _serializer.Serialize(originalObject);

        // Act
        var result = _serializer.Deserialize<TestClass>(xml);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<TestClass>();

        result.Id.Should().Be("TEST_GENERIC");
        result.Nome.Should().Be("Teste Genérico");
    }

    [Fact]
    public void Serialize_ComObjetoNulo_DeveLancarArgumentNullException()
    {
        // Act & Assert
        var action = () => _serializer.Serialize(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Deserialize_ComXmlInvalido_DeveLancarException()
    {
        // Arrange
        var invalidXml = "<InvalidXml><NoClosingTag></InvalidXml>";

        // Act & Assert
        var action = () => _serializer.Deserialize<TestClass>(invalidXml);
        action.Should().Throw<Exception>();
    }

    [Fact]
    public void SerializeForSigning_ComObjetoComplexo_DeveManterEstrutura()
    {
        // Arrange
        var complexObject = new ComplexTestClass
        {
            Id = "COMPLEX_001",
            Data = DateTime.Parse("2024-12-19"),
            Items = new List<string> { "Item1", "Item2", "Item3" },
            Nested = new TestClass { Id = "NESTED_001", Nome = "Aninhado" }
        };

        // Act
        var result = _serializer.SerializeForSigning(complexObject);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("COMPLEX_001");
        result.Should().Contain("2024-12-19");
        result.Should().Contain("Item1");
        result.Should().Contain("NESTED_001");
        result.Should().NotContain("\r\n");

        _output.WriteLine($"XML complexo para assinatura: {result[..Math.Min(200, result.Length)]}...");
    }

    public class TestClass
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
    }

    public class ComplexTestClass
    {
        public string Id { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public List<string> Items { get; set; } = new();
        public TestClass? Nested { get; set; }
    }
}

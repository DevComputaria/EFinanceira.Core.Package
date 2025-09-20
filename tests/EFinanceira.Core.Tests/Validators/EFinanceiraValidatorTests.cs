using EFinanceira.Core.Validators;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace EFinanceira.Core.Tests.Validators;

public class EFinanceiraValidatorTests
{
    private readonly Mock<ILogger<EFinanceiraValidator>> _loggerMock;
    private readonly EFinanceiraValidator _validator;

    public EFinanceiraValidatorTests()
    {
        _loggerMock = new Mock<ILogger<EFinanceiraValidator>>();
        _validator = new EFinanceiraValidator(_loggerMock.Object);
    }

    [Theory]
    [InlineData("11.222.333/0001-81", true)]  // CNPJ válido
    [InlineData("11222333000181", true)]      // CNPJ válido sem formatação
    [InlineData("00.000.000/0000-00", false)] // CNPJ inválido (todos zeros)
    [InlineData("11.111.111/1111-11", false)] // CNPJ inválido (todos iguais)
    [InlineData("12.345.678/0001-00", false)] // CNPJ inválido (dígito verificador errado)
    [InlineData("", false)]                   // CNPJ vazio
    [InlineData(null, false)]                 // CNPJ nulo
    [InlineData("123", false)]                // CNPJ muito curto
    public void ValidateCnpj_ShouldReturnExpectedResult(string cnpj, bool expected)
    {
        // Act
        var result = _validator.ValidateCnpj(cnpj);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("123.456.789-09", true)]  // CPF válido
    [InlineData("12345678909", true)]     // CPF válido sem formatação
    [InlineData("000.000.000-00", false)] // CPF inválido (todos zeros)
    [InlineData("111.111.111-11", false)] // CPF inválido (todos iguais)
    [InlineData("123.456.789-00", false)] // CPF inválido (dígito verificador errado)
    [InlineData("", false)]               // CPF vazio
    [InlineData(null, false)]             // CPF nulo
    [InlineData("123", false)]            // CPF muito curto
    public void ValidateCpf_ShouldReturnExpectedResult(string cpf, bool expected)
    {
        // Act
        var result = _validator.ValidateCpf(cpf);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new EFinanceiraValidator(null!);
        action.Should().Throw<ArgumentNullException>();
    }
}
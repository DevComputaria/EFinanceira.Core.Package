using EFinanceira.Core.Configuration;
using EFinanceira.Core.Extensions;
using EFinanceira.Core.Services;
using EFinanceira.Core.Validators;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EFinanceira.Core.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddEFinanceira_ShouldRegisterAllRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddEFinanceira();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        serviceProvider.GetService<EFinanceiraOptions>().Should().NotBeNull();
        serviceProvider.GetService<IEFinanceiraValidator>().Should().NotBeNull();
        serviceProvider.GetService<IEFinanceiraXmlService>().Should().NotBeNull();
        serviceProvider.GetService<IEFinanceiraService>().Should().NotBeNull();
    }

    [Fact]
    public void AddEFinanceira_WithConfiguration_ShouldConfigureOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddEFinanceira(options =>
        {
            options.VersaoLayout = "2.0.0";
            options.ValidateGeneratedXml = false;
            options.TimeoutMs = 60000;
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<EFinanceiraOptions>();

        options.VersaoLayout.Should().Be("2.0.0");
        options.ValidateGeneratedXml.Should().BeFalse();
        options.TimeoutMs.Should().Be(60000);
    }

    [Fact]
    public void AddEFinanceiraCore_ShouldRegisterServicesWithDefaultConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddEFinanceiraCore();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        serviceProvider.GetService<EFinanceiraOptions>().Should().NotBeNull();
        serviceProvider.GetService<IEFinanceiraValidator>().Should().NotBeNull();
        serviceProvider.GetService<IEFinanceiraXmlService>().Should().NotBeNull();
        serviceProvider.GetService<IEFinanceiraService>().Should().NotBeNull();

        var options = serviceProvider.GetRequiredService<EFinanceiraOptions>();
        options.VersaoLayout.Should().Be("1.0.0");
        options.ValidateGeneratedXml.Should().BeTrue();
    }

    [Fact]
    public void AddEFinanceira_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => ServiceCollectionExtensions.AddEFinanceira(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddEFinanceiraCore_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => ServiceCollectionExtensions.AddEFinanceiraCore(null!);
        action.Should().Throw<ArgumentNullException>();
    }
}
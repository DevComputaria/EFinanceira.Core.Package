using EFinanceira.Core.Configuration;
using EFinanceira.Core.Services;
using EFinanceira.Core.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace EFinanceira.Core.Extensions;

/// <summary>
/// Extensões para configuração de Dependency Injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adiciona os serviços da e-Financeira ao container de DI
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configure">Configuração opcional</param>
    /// <returns>Service collection para método chaining</returns>
    public static IServiceCollection AddEFinanceira(
        this IServiceCollection services,
        Action<EFinanceiraOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Configurar opções
        var options = new EFinanceiraOptions();
        configure?.Invoke(options);

        services.TryAddSingleton(options);

        // Registrar serviços
        services.TryAddScoped<IEFinanceiraValidator, EFinanceiraValidator>();
        services.TryAddScoped<IEFinanceiraXmlService, EFinanceiraXmlService>();
        services.TryAddScoped<IEFinanceiraService, EFinanceiraService>();

        // Garantir que logging está configurado
        services.TryAddSingleton<ILoggerFactory, LoggerFactory>();

        return services;
    }

    /// <summary>
    /// Adiciona os serviços da e-Financeira ao container de DI com configuração por seção
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration provider</param>
    /// <param name="sectionName">Nome da seção de configuração</param>
    /// <returns>Service collection para método chaining</returns>
    public static IServiceCollection AddEFinanceira(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = EFinanceiraOptions.DefaultSectionName)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // Configurar opções a partir da configuração
        var options = new EFinanceiraOptions();
        configuration.GetSection(sectionName).Bind(options);

        return services.AddEFinanceira(_ => 
        {
            _.VersaoLayout = options.VersaoLayout;
            _.ValidateGeneratedXml = options.ValidateGeneratedXml;
            _.TimeoutMs = options.TimeoutMs;
            _.Logging = options.Logging;
            _.Validation = options.Validation;
        });
    }

    /// <summary>
    /// Adiciona apenas os serviços essenciais da e-Financeira (sem configuração)
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection para método chaining</returns>
    public static IServiceCollection AddEFinanceiraCore(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Usar configuração padrão
        return services.AddEFinanceira();
    }

    /// <summary>
    /// Adiciona validadores customizados
    /// </summary>
    /// <typeparam name="TValidator">Tipo do validador customizado</typeparam>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection para método chaining</returns>
    public static IServiceCollection AddEFinanceiraValidator<TValidator>(this IServiceCollection services)
        where TValidator : class, IEFinanceiraValidator
    {
        ArgumentNullException.ThrowIfNull(services);

        services.Replace(ServiceDescriptor.Scoped<IEFinanceiraValidator, TValidator>());

        return services;
    }

    /// <summary>
    /// Adiciona serviço XML customizado
    /// </summary>
    /// <typeparam name="TXmlService">Tipo do serviço XML customizado</typeparam>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection para método chaining</returns>
    public static IServiceCollection AddEFinanceiraXmlService<TXmlService>(this IServiceCollection services)
        where TXmlService : class, IEFinanceiraXmlService
    {
        ArgumentNullException.ThrowIfNull(services);

        services.Replace(ServiceDescriptor.Scoped<IEFinanceiraXmlService, TXmlService>());

        return services;
    }

    /// <summary>
    /// Adiciona serviço principal customizado
    /// </summary>
    /// <typeparam name="TService">Tipo do serviço principal customizado</typeparam>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection para método chaining</returns>
    public static IServiceCollection AddEFinanceiraService<TService>(this IServiceCollection services)
        where TService : class, IEFinanceiraService
    {
        ArgumentNullException.ThrowIfNull(services);

        services.Replace(ServiceDescriptor.Scoped<IEFinanceiraService, TService>());

        return services;
    }
}

/// <summary>
/// Extensões para logging
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Cria um logger com categoria específica para e-Financeira
    /// </summary>
    /// <param name="loggerFactory">Logger factory</param>
    /// <param name="categoryName">Nome da categoria</param>
    /// <returns>Logger configurado</returns>
    public static ILogger CreateEFinanceiraLogger(this ILoggerFactory loggerFactory, string categoryName)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryName);

        return loggerFactory.CreateLogger($"EFinanceira.{categoryName}");
    }

    /// <summary>
    /// Cria um logger com categoria específica para e-Financeira usando tipo
    /// </summary>
    /// <typeparam name="T">Tipo para categoria</typeparam>
    /// <param name="loggerFactory">Logger factory</param>
    /// <returns>Logger configurado</returns>
    public static ILogger<T> CreateEFinanceiraLogger<T>(this ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);

        return loggerFactory.CreateLogger<T>();
    }
}
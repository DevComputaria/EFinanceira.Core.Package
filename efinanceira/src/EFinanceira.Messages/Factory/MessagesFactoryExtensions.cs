using EFinanceira.Core.Abstractions;
using EFinanceira.Core.Factory;
using EFinanceira.Messages.Builders.Consultas.RetInfoCadastral;
using EFinanceira.Messages.Builders.Consultas.RetInfoIntermediario;

namespace EFinanceira.Messages.Factory;

/// <summary>
/// Configurador para registrar mensagens do projeto Messages no factory do Core
/// </summary>
public static class MessagesFactoryExtensions
{
    /// <summary>
    /// Registra todas as mensagens de consulta no factory
    /// </summary>
    /// <param name="factory">Factory do Core para configurar</param>
    /// <returns>O mesmo factory para fluent interface</returns>
    public static EFinanceiraMessageFactory AddConsultas(this EFinanceiraMessageFactory factory)
    {
        // Registro da consulta de informações cadastrais
        factory.RegisterFactory(
            MessageKind.Consulta("RetInfoCadastral"),
            "v1_2_0",
            (Action<object>? seed) =>
            {
                var builder = new RetInfoCadastralBuilder("v1_2_0");

                // Aplicar configurações do seed se fornecido
                if (seed is Action<RetInfoCadastralBuilder> configure)
                {
                    configure(builder);
                }

                return builder.Build();
            });

        // Registro da consulta de informações de intermediário
        factory.RegisterFactory(
            MessageKind.Consulta("RetInfoIntermediario"),
            "v1_2_0",
            (Action<object>? seed) =>
            {
                var builder = new RetInfoIntermediarioBuilder("v1_2_0");

                // Aplicar configurações do seed se fornecido
                if (seed is Action<RetInfoIntermediarioBuilder> configure)
                {
                    configure(builder);
                }

                return builder.Build();
            });

        return factory;
    }

    /// <summary>
    /// Registra todas as mensagens de eventos no factory (placeholder para futuro)
    /// </summary>
    /// <param name="factory">Factory do Core para configurar</param>
    /// <returns>O mesmo factory para fluent interface</returns>
    public static EFinanceiraMessageFactory AddEventos(this EFinanceiraMessageFactory factory)
    {
        // TODO: Implementar quando houver builders de eventos
        return factory;
    }

    /// <summary>
    /// Registra todas as mensagens de lotes no factory (placeholder para futuro)
    /// </summary>
    /// <param name="factory">Factory do Core para configurar</param>
    /// <returns>O mesmo factory para fluent interface</returns>
    public static EFinanceiraMessageFactory AddLotes(this EFinanceiraMessageFactory factory)
    {
        // TODO: Implementar quando houver builders de lotes
        return factory;
    }

    /// <summary>
    /// Registra todas as mensagens suportadas no factory
    /// </summary>
    /// <param name="factory">Factory do Core para configurar</param>
    /// <returns>O mesmo factory para fluent interface</returns>
    public static EFinanceiraMessageFactory AddAllMessages(this EFinanceiraMessageFactory factory)
    {
        return factory
            .AddConsultas()
            .AddEventos()
            .AddLotes();
    }

    /// <summary>
    /// Cria e configura um factory completo com todas as mensagens
    /// </summary>
    /// <returns>Factory configurado</returns>
    public static EFinanceiraMessageFactory CreateConfiguredFactory()
    {
        var factory = new EFinanceiraMessageFactory();
        return factory.AddAllMessages();
    }
}

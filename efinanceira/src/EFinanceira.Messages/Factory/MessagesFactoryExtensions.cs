using EFinanceira.Core.Abstractions;
using EFinanceira.Core.Factory;
using EFinanceira.Messages.Builders.Consultas.RetInfoCadastral;
using EFinanceira.Messages.Builders.Consultas.RetInfoIntermediario;
using EFinanceira.Messages.Builders.Consultas.RetInfoMovimento;
using EFinanceira.Messages.Builders.Consultas.RetInfoPatrocinado;
using EFinanceira.Messages.Builders.Consultas.RetListaeFinanceira;
using EFinanceira.Messages.Builders.Consultas.RetRERCT;
using EFinanceira.Messages.Builders.Eventos.EvtAberturaeFinanceira;
using EFinanceira.Messages.Builders.Eventos.EvtCadDeclarante;
using EFinanceira.Messages.Builders.Xmldsig;

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

        // Registro da consulta de informações de movimento
        factory.RegisterFactory(
            MessageKind.Consulta("RetInfoMovimento"),
            "v1_2_0",
            (Action<object>? seed) =>
            {
                var builder = new RetInfoMovimentoBuilder("v1_2_0");

                // Aplicar configurações do seed se fornecido
                if (seed is Action<RetInfoMovimentoBuilder> configure)
                {
                    configure(builder);
                }

                return builder.Build();
            });

        // Registro da consulta de informações de patrocinado
        factory.RegisterFactory(
            MessageKind.Consulta("RetInfoPatrocinado"),
            "v1_2_0",
            (Action<object>? seed) =>
            {
                var builder = new RetInfoPatrocinadoBuilder("v1_2_0");

                // Aplicar configurações do seed se fornecido
                if (seed is Action<RetInfoPatrocinadoBuilder> configure)
                {
                    configure(builder);
                }

                return builder.Build();
            });

        // Registro da consulta de lista de e-Financeira
        factory.RegisterFactory(
            MessageKind.Consulta("RetListaeFinanceira"),
            "v1_2_0",
            (Action<object>? seed) =>
            {
                var builder = new RetListaeFinanceiraBuilder();

                // Aplicar configurações do seed se fornecido
                if (seed is Action<RetListaeFinanceiraBuilder> configure)
                {
                    configure(builder);
                }

                return builder.Build();
            });

        // Registro da consulta RERCT
        factory.RegisterFactory(
            MessageKind.Consulta("RetRERCT"),
            "v1_2_0",
            (Action<object>? seed) =>
            {
                var builder = new RetRERCTBuilder("v1_2_0");

                // Aplicar configurações do seed se fornecido
                if (seed is Action<RetRERCTBuilder> configure)
                {
                    configure(builder);
                }

                return builder.Build();
            });

        return factory;
    }

    /// <summary>
    /// Registra todas as mensagens de eventos no factory
    /// </summary>
    /// <param name="factory">Factory do Core para configurar</param>
    /// <returns>O mesmo factory para fluent interface</returns>
    public static EFinanceiraMessageFactory AddEventos(this EFinanceiraMessageFactory factory)
    {
        // Registro do evento de Abertura e-Financeira
        factory.RegisterFactory(
            MessageKind.Evento("EvtAberturaeFinanceira"),
            "v1_2_1",
            (Action<object>? seed) =>
            {
                var builder = new EvtAberturaeFinanceiraBuilder("v1_2_1");

                // Aplicar configurações do seed se fornecido
                if (seed is Action<EvtAberturaeFinanceiraBuilder> configure)
                {
                    configure(builder);
                }

                return builder.Build();
            });

        // Registro do evento de Cadastro de Declarante
        factory.RegisterFactory(
            MessageKind.Evento("EvtCadDeclarante"),
            "v1_2_0",
            (Action<object>? seed) =>
            {
                var builder = new EvtCadDeclaranteBuilder("v1_2_0");

                // Aplicar configurações do seed se fornecido
                if (seed is Action<EvtCadDeclaranteBuilder> configure)
                {
                    configure(builder);
                }

                return builder.Build();
            });

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
            .AddLotes()
            .AddXmldsigBuilder();
    }

    /// <summary>
    /// Registra o builder de assinatura digital XMLDSig no factory
    /// </summary>
    /// <param name="factory">Factory do Core para configurar</param>
    /// <returns>O mesmo factory para fluent interface</returns>
    public static EFinanceiraMessageFactory AddXmldsigBuilder(this EFinanceiraMessageFactory factory)
    {
        // Registro do builder de assinatura digital XMLDSig como um tipo especial de evento
        factory.RegisterFactory(
            MessageKind.Evento("XmlDigitalSignature"),
            "core",
            (Action<object>? seed) =>
            {
                var signatureId = "XMLSignature_" + Guid.NewGuid().ToString("N")[..8];
                
                // Para compatibilidade com factory, criar assinatura básica
                return XmldsigBuilder.BuildBasicSignature(signatureId);
            });

        return factory;
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

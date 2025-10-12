using System;
using EFinanceira.Messages.Builders.Eventos.EvtMovimentacaoFinanceira;
using EFinanceira.Messages.Builders.Lotes.EnvioLoteEventosAssincrono;

namespace EFinanceira.Messages.Examples.Lotes.EnvioLoteEventosAssincronoExamples;

/// <summary>
/// Exemplos de uso do EnvioLoteEventosAssincronoBuilder baseado em XSD
/// </summary>
public static class EnvioLoteEventosAssincronoExamples
{
    /// <summary>
    /// Demonstra uso básico do builder com um evento
    /// </summary>
    public static void ExemploBasico()
    {
        Console.WriteLine("=== Exemplo básico ===\n");

        var evento1 = new EvtMovimentacaoFinanceiraBuilder()
            .ComId("MOV_001")
            .ComIdeEvento(ide => ide
                .WithIndRetificacao(1)
                .WithTpAmb(2))
            .ComIdeDeclarante(decl => decl.WithCnpjDeclarante("12345678000123"))
            .ComIdeDeclarado(decl => decl.WithTpNi(1).WithNiDeclarado("11122233344"))
            .Build();

        var loteAssincrono = new EnvioLoteEventosAssincronoBuilder()
            .ComCnpjDeclarante("12345678000123")
            .AdicionarEvento(evento1)
            .Build();

        Console.WriteLine($"✓ Lote assíncrono criado com {loteAssincrono.EFinanceira.loteEventosAssincrono.eventos.evento.Length} evento(s)");
        Console.WriteLine($"  - CNPJ Declarante: {loteAssincrono.EFinanceira.loteEventosAssincrono.cnpjDeclarante}");
        Console.WriteLine($"  - Versão: {loteAssincrono.Version}");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstra como adicionar múltiplos eventos usando fluent interface
    /// </summary>
    public static void ExemploMultiplosEventos()
    {
        Console.WriteLine("=== Exemplo múltiplos eventos ===\n");

        var evento1 = new EvtMovimentacaoFinanceiraBuilder()
            .ComId("MOV_001")
            .ComIdeEvento(ide => ide.WithIndRetificacao(1).WithTpAmb(2))
            .ComIdeDeclarante(decl => decl.WithCnpjDeclarante("12345678000123"))
            .ComIdeDeclarado(decl => decl.WithTpNi(1).WithNiDeclarado("11122233344"))
            .Build();

        var evento2 = new EvtMovimentacaoFinanceiraBuilder()
            .ComId("MOV_002")
            .ComIdeEvento(ide => ide.WithIndRetificacao(1).WithTpAmb(2))
            .ComIdeDeclarante(decl => decl.WithCnpjDeclarante("12345678000123"))
            .ComIdeDeclarado(decl => decl.WithTpNi(1).WithNiDeclarado("55566677788"))
            .Build();

        var loteAssincrono = new EnvioLoteEventosAssincronoBuilder()
            .ComCnpjDeclarante("12345678000123")
            .AdicionarEvento(evento1, "CUSTOM_001")
            .AdicionarEvento(evento2, "CUSTOM_002")
            .Build();

        Console.WriteLine($"✓ Lote assíncrono criado com {loteAssincrono.EFinanceira.loteEventosAssincrono.eventos.evento.Length} eventos");
        foreach (var evento in loteAssincrono.EFinanceira.loteEventosAssincrono.eventos.evento)
        {
            Console.WriteLine($"  - Evento ID: {evento.id}");
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstra uso com builders de eventos
    /// </summary>
    public static void ExemploComBuilders()
    {
        Console.WriteLine("=== Exemplo com builders ===\n");

        var eventoBuilder1 = new EvtMovimentacaoFinanceiraBuilder()
            .ComId("MOV_BUILDER_001")
            .ComIdeEvento(ide => ide.WithIndRetificacao(1).WithTpAmb(2))
            .ComIdeDeclarante(decl => decl.WithCnpjDeclarante("12345678000123"))
            .ComIdeDeclarado(decl => decl.WithTpNi(1).WithNiDeclarado("11122233344"));

        var eventoBuilder2 = new EvtMovimentacaoFinanceiraBuilder()
            .ComId("MOV_BUILDER_002")
            .ComIdeEvento(ide => ide.WithIndRetificacao(1).WithTpAmb(2))
            .ComIdeDeclarante(decl => decl.WithCnpjDeclarante("12345678000123"))
            .ComIdeDeclarado(decl => decl.WithTpNi(1).WithNiDeclarado("55566677788"));

        var loteAssincrono = new EnvioLoteEventosAssincronoBuilder()
            .ComCnpjDeclarante("12345678000123")
            .AdicionarEvento(eventoBuilder1)
            .AdicionarEvento(eventoBuilder2)
            .Build();

        Console.WriteLine($"✓ Lote assíncrono criado usando builders com {loteAssincrono.EFinanceira.loteEventosAssincrono.eventos.evento.Length} eventos");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstra uso com extensões fluentes
    /// </summary>
    public static void ExemploComExtensoes()
    {
        Console.WriteLine("=== Exemplo com extensões fluentes ===\n");

        var eventos = new[]
        {
            new EvtMovimentacaoFinanceiraBuilder()
                .ComId("MOV_EXT_001")
                .ComIdeEvento(ide => ide.WithIndRetificacao(1).WithTpAmb(2))
                .ComIdeDeclarante(decl => decl.WithCnpjDeclarante("12345678000123"))
                .ComIdeDeclarado(decl => decl.WithTpNi(1).WithNiDeclarado("11122233344"))
                .Build(),

            new EvtMovimentacaoFinanceiraBuilder()
                .ComId("MOV_EXT_002")
                .ComIdeEvento(ide => ide.WithIndRetificacao(1).WithTpAmb(2))
                .ComIdeDeclarante(decl => decl.WithCnpjDeclarante("12345678000123"))
                .ComIdeDeclarado(decl => decl.WithTpNi(1).WithNiDeclarado("55566677788"))
                .Build()
        };

        var loteAssincrono = EnvioLoteEventosAssincronoBuilderExtensions
            .Create("v1_0_0")
            .ComCnpjDeclarante("12345678000123")
            .ComEventos(eventos)
            .Build();

        Console.WriteLine($"✓ Lote assíncrono criado com extensões fluentes: {loteAssincrono.EFinanceira.loteEventosAssincrono.eventos.evento.Length} eventos");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstra controle de capacidade do lote
    /// </summary>
    public static void ExemploControleCapacidade()
    {
        Console.WriteLine("=== Exemplo controle de capacidade ===\n");

        var builder = new EnvioLoteEventosAssincronoBuilder()
            .ComCnpjDeclarante("12345678000123");

        // Adicionar eventos até próximo do limite
        for (int i = 1; i <= 3; i++)
        {
            var evento = new EvtMovimentacaoFinanceiraBuilder()
                .ComId($"MOV_CAP_{i:000}")
                .ComIdeEvento(ide => ide.WithIndRetificacao(1).WithTpAmb(2))
                .ComIdeDeclarante(decl => decl.WithCnpjDeclarante("12345678000123"))
                .ComIdeDeclarado(decl => decl.WithTpNi(1).WithNiDeclarado("11122233344"))
                .Build();

            builder.AdicionarEvento(evento);

            Console.WriteLine($"  ✓ Evento {i} adicionado. Total: {builder.ContarEventos()}");
        }

        Console.WriteLine($"  - Lote vazio? {builder.EstaVazio()}");
        Console.WriteLine($"  - Limite máximo: {EnvioLoteEventosAssincronoBuilder.MaxEventosPorLote} eventos");

        var loteAssincrono = builder.Build();
        Console.WriteLine($"✓ Lote assíncrono finalizado com {loteAssincrono.EFinanceira.loteEventosAssincrono.eventos.evento.Length} eventos");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstra gerenciamento de eventos no builder
    /// </summary>
    public static void ExemploGerenciamentoEventos()
    {
        Console.WriteLine("=== Exemplo gerenciamento de eventos ===\n");

        var builder = new EnvioLoteEventosAssincronoBuilder()
            .ComCnpjDeclarante("12345678000123");

        // Adicionar eventos
        var evento1 = new EvtMovimentacaoFinanceiraBuilder()
            .ComId("MOV_MGMT_001")
            .ComIdeEvento(ide => ide.WithIndRetificacao(1).WithTpAmb(2))
            .ComIdeDeclarante(decl => decl.WithCnpjDeclarante("12345678000123"))
            .ComIdeDeclarado(decl => decl.WithTpNi(1).WithNiDeclarado("11122233344"))
            .Build();

        var evento2 = new EvtMovimentacaoFinanceiraBuilder()
            .ComId("MOV_MGMT_002")
            .ComIdeEvento(ide => ide.WithIndRetificacao(1).WithTpAmb(2))
            .ComIdeDeclarante(decl => decl.WithCnpjDeclarante("12345678000123"))
            .ComIdeDeclarado(decl => decl.WithTpNi(1).WithNiDeclarado("55566677788"))
            .Build();

        builder.AdicionarEvento(evento1)
               .AdicionarEvento(evento2);

        Console.WriteLine($"  ✓ Eventos adicionados: {builder.ContarEventos()}");
        Console.WriteLine($"  - Lote vazio? {builder.EstaVazio()}");

        // Limpar eventos
        builder.LimparEventos();
        Console.WriteLine($"  ✓ Eventos limpos: {builder.ContarEventos()}");
        Console.WriteLine($"  - Lote vazio? {builder.EstaVazio()}");

        // Adicionar novamente
        builder.AdicionarEvento(evento1);
        Console.WriteLine($"  ✓ Evento re-adicionado: {builder.ContarEventos()}");

        var loteAssincrono = builder.Build();
        Console.WriteLine($"✓ Lote assíncrono final: {loteAssincrono.EFinanceira.loteEventosAssincrono.eventos.evento.Length} evento(s)");
        Console.WriteLine();
    }

    /// <summary>
    /// Executa todos os exemplos
    /// </summary>
    public static void ExecutarTodosExemplos()
    {
        Console.WriteLine("🚀 EXECUTANDO EXEMPLOS DO ENVIO LOTE EVENTOS ASSÍNCRONO BUILDER 🚀\n");
        Console.WriteLine("================================================================\n");

        try
        {
            ExemploBasico();
            ExemploMultiplosEventos();
            ExemploComBuilders();
            ExemploComExtensoes();
            ExemploControleCapacidade();
            ExemploGerenciamentoEventos();

            Console.WriteLine("✅ TODOS OS EXEMPLOS EXECUTADOS COM SUCESSO!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERRO AO EXECUTAR EXEMPLOS: {ex.Message}");
            Console.WriteLine($"   Stack Trace: {ex.StackTrace}");
        }
    }
}
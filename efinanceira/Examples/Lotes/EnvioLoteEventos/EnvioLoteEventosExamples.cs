using System;
using EFinanceira.Messages.Builders.Eventos.EvtMovimentacaoFinanceira;
using EFinanceira.Messages.Builders.Lotes.EnvioLoteEventos;

namespace EFinanceira.Messages.Examples.Lotes.EnvioLoteEventosExamples;

/// <summary>
/// Exemplos de uso do EnvioLoteEventosBuilder baseado em XSD
/// </summary>
public static class EnvioLoteEventosExamples
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
                .ComIndRetificacao(1)
                .ComTpAmb(2)
                .ComAplicEmi(1)
                .ComVerAplic("1.0.0"))
            .ComIdeDeclarante(declarante => declarante
                .ComCnpjDeclarante("12345678000195"))
            .ComIdeDeclarado(declarado => declarado
                .ComTpNI(1)
                .ComNIDeclarado("12345678909"))
            .Build();
        
        var lote = new EnvioLoteEventos.EnvioLoteEventosBuilder()
            .WithId("LOTE_BASICO_001")
            .WithTransmissor(transmissor => transmissor
                .WithCnpj("12345678000195")
                .WithNome("Empresa Teste LTDA"))
            .AddEvento(evento1)
            .Build();
        
        Console.WriteLine($"Lote criado: {lote.IdValue}");
        Console.WriteLine($"Eventos no lote: {lote.Lote.loteEventos.evento.Length}");
        Console.WriteLine("\n");
    }
    
    /// <summary>
    /// Demonstra como adicionar múltiplos eventos
    /// </summary>
    public static void ExemploMultiplosEventos()
    {
        Console.WriteLine("=== Exemplo com múltiplos eventos ===\n");
        
        var evento1 = new EvtMovimentacaoFinanceiraBuilder()
            .ComId("MOV_001")
            .ComIdeEvento(ide => ide
                .ComIndRetificacao(1)
                .ComTpAmb(2)
                .ComAplicEmi(1)
                .ComVerAplic("1.0.0"))
            .ComIdeDeclarante(declarante => declarante
                .ComCnpjDeclarante("12345678000195"))
            .ComIdeDeclarado(declarado => declarado
                .ComTpNI(1)
                .ComNIDeclarado("12345678909"))
            .Build();
        
        var evento2 = new EvtMovimentacaoFinanceiraBuilder()
            .ComId("MOV_002")
            .ComIdeEvento(ide => ide
                .ComIndRetificacao(2)
                .ComTpAmb(2)
                .ComAplicEmi(1)
                .ComVerAplic("1.0.0"))
            .ComIdeDeclarante(declarante => declarante
                .ComCnpjDeclarante("12345678000195"))
            .ComIdeDeclarado(declarado => declarado
                .ComTpNI(1)
                .ComNIDeclarado("98765432109"))
            .Build();
        
        var lote = new EnvioLoteEventos.EnvioLoteEventosBuilder()
            .WithId("LOTE_MULTIPLOS_001")
            .WithTransmissor(transmissor => transmissor
                .WithCnpj("12345678000195")
                .WithNome("Empresa Teste LTDA")
                .WithEmail("contato@empresa.com"))
            .AddEvento(evento1)
            .AddEvento(evento2)
            .Build();
        
        Console.WriteLine($"Lote criado: {lote.IdValue}");
        Console.WriteLine($"Eventos no lote: {lote.Lote.loteEventos.evento.Length}");
        Console.WriteLine("\n");
    }
    
    /// <summary>
    /// Demonstra como usar fluent interface para adicionar muitos eventos
    /// </summary>
    public static void ExemploFluentInterface()
    {
        Console.WriteLine("=== Exemplo com fluent interface ===\n");
        
        var builder = new EnvioLoteEventos.EnvioLoteEventosBuilder()
            .WithId("LOTE_FLUENT_001")
            .WithTransmissor(transmissor => transmissor
                .WithCnpj("12345678000195")
                .WithNome("Empresa Teste LTDA"));
        
        // Adicionando múltiplos eventos usando fluent interface
        for (int i = 1; i <= 5; i++)
        {
            var evento = new EvtMovimentacaoFinanceiraBuilder()
                .ComId($"MOV_{i:000}")
                .ComIdeEvento(ide => ide
                    .ComIndRetificacao(1)
                    .ComTpAmb(2)
                    .ComAplicEmi(1)
                    .ComVerAplic("1.0.0"))
                .ComIdeDeclarante(declarante => declarante
                    .ComCnpjDeclarante("12345678000195"))
                .ComIdeDeclarado(declarado => declarado
                    .ComTpNI(1)
                    .ComNIDeclarado($"1234567890{i % 10}"))
                .Build();
            
            builder.AddEvento(evento);
        }
        
        var lote = builder.Build();
        
        Console.WriteLine($"Lote criado: {lote.IdValue}");
        Console.WriteLine($"Eventos no lote: {lote.Lote.loteEventos.evento.Length}");
        Console.WriteLine("\n");
    }
    
    /// <summary>
    /// Demonstra controle de ID único para eventos
    /// </summary>
    public static void ExemploControleId()
    {
        Console.WriteLine("=== Exemplo com controle de ID ===\n");
        
        var eventoSimples = new EvtMovimentacaoFinanceiraBuilder()
            .ComId("MOV_SIMPLES_001")
            .ComIdeEvento(ide => ide
                .ComIndRetificacao(1)
                .ComTpAmb(2)
                .ComAplicEmi(1)
                .ComVerAplic("1.0.0"))
            .ComIdeDeclarante(declarante => declarante
                .ComCnpjDeclarante("12345678000195"))
            .ComIdeDeclarado(declarado => declarado
                .ComTpNI(1)
                .ComNIDeclarado("12345678909"))
            .Build();
        
        var lote = new EnvioLoteEventos.EnvioLoteEventosBuilder()
            .WithId("LOTE_CONTROLE_ID_001")
            .WithTransmissor(transmissor => transmissor
                .WithCnpj("12345678000195")
                .WithNome("Empresa Teste LTDA"))
            .AddEvento(eventoSimples)
            .Build();
        
        Console.WriteLine($"Lote criado: {lote.IdValue}");
        Console.WriteLine($"ID do primeiro evento: {lote.Lote.loteEventos.evento[0].Id}");
        Console.WriteLine("\n");
    }
    
    /// <summary>
    /// Demonstra diferentes tipos de eventos em um lote
    /// </summary>
    public static void ExemploEventosMistos()
    {
        Console.WriteLine("=== Exemplo com eventos mistos ===\n");
        
        var evento1 = new EvtMovimentacaoFinanceiraBuilder()
            .ComId("MOV_001")
            .ComIdeEvento(ide => ide
                .ComIndRetificacao(2)
                .ComTpAmb(2)
                .ComAplicEmi(1)
                .ComVerAplic("1.0.0"))
            .ComIdeDeclarante(declarante => declarante
                .ComCnpjDeclarante("12345678000195"))
            .ComIdeDeclarado(declarado => declarado
                .ComTpNI(1)
                .ComNIDeclarado("12345678909"))
            .Build();
        
        var lote = new EnvioLoteEventos.EnvioLoteEventosBuilder()
            .WithId("LOTE_MISTOS_001")
            .WithTransmissor(transmissor => transmissor
                .WithCnpj("98765432000187")
                .WithNome("Instituição Financeira XYZ")
                .WithEmail("efinanceira@empresa.com")
                .WithTelefone("1133334444"))
            .AddEvento(evento1)
            .Build();
        
        Console.WriteLine($"Lote criado: {lote.IdValue}");
        Console.WriteLine($"Eventos no lote: {lote.Lote.loteEventos.evento.Length}");
        Console.WriteLine("\n");
    }
    
    /// <summary>
    /// Demonstra como usar o builder com callback
    /// </summary>
    public static void ExemploComCallback()
    {
        Console.WriteLine("=== Exemplo com callback ===\n");
        
        var evento = new EvtMovimentacaoFinanceiraBuilder()
            .ComId("MOV_CALLBACK_001")
            .ComIdeEvento(ide => ide
                .ComIndRetificacao(1)
                .ComTpAmb(2)
                .ComAplicEmi(1)
                .ComVerAplic("1.0.0"))
            .ComIdeDeclarante(declarante => declarante
                .ComCnpjDeclarante("12345678000195"))
            .ComIdeDeclarado(declarado => declarado
                .ComTpNI(1)
                .ComNIDeclarado("12345678909"))
            .Build();
        
        // Usando callback para configurar o transmissor
        var lote = new EnvioLoteEventos.EnvioLoteEventosBuilder()
            .WithId("LOTE_CALLBACK_001")
            .WithTransmissor(transmissor => transmissor
                .WithCnpj("12345678000195")
                .WithNome("Empresa Teste LTDA"))
            .AddEvento(evento)
            .Build();
        
        Console.WriteLine($"Lote com callback criado: {lote.IdValue}");
        Console.WriteLine($"Eventos no lote: {lote.Lote.loteEventos.evento.Length}");
        Console.WriteLine("\n");
    }
}
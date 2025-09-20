using EFinanceira.Core.Abstractions;
using EFinanceira.Core.Factory;
using EFinanceira.Core.Serialization;
using EFinanceira.Core.Signing;
using EFinanceira.Core.Validation;
using EFinanceira.Messages.Builders.Eventos;
using System;

namespace EFinanceira.Console.Sample;

/// <summary>
/// Sample console application demonstrating e-Financeira XML processing.
/// Example: montar, validar, assinar, serializar
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        System.Console.WriteLine("EFinanceira Sample Application");
        System.Console.WriteLine("=============================");

        try
        {
            // Example of building a message
            await DemonstrateMessageBuilding();
            
            // Example of validation
            await DemonstrateValidation();
            
            // Example of serialization
            await DemonstrateSerialization();
            
            // Note: Signing example would require an actual certificate
            System.Console.WriteLine("\nNote: XML signing example requires a valid X.509 certificate");
            
            System.Console.WriteLine("\nSample completed successfully!");
        }
        catch (Exception ex)
        {
            System.Console.Error.WriteLine($"Error: {ex.Message}");
        }
    }

    private static async Task DemonstrateMessageBuilding()
    {
        System.Console.WriteLine("\n1. Message Building Demo");
        System.Console.WriteLine("------------------------");

        var builder = new LeiauteAberturaBuilder();
        
        if (builder.IsValid())
        {
            var message = builder.Build();
            System.Console.WriteLine($"Built message: {message}");
        }
        else
        {
            System.Console.WriteLine("Message validation failed:");
            foreach (var error in builder.GetValidationErrors())
            {
                System.Console.WriteLine($"  - {error}");
            }
        }

        await Task.CompletedTask;
    }

    private static async Task DemonstrateValidation()
    {
        System.Console.WriteLine("\n2. XML Validation Demo");
        System.Console.WriteLine("----------------------");

        var validator = new XmlValidator();
        
        // This would require actual XML content and schema files
        System.Console.WriteLine("XML validation functionality is available via IXmlValidator");
        System.Console.WriteLine("Example: validator.ValidateXml(xmlContent, schemaPath)");

        await Task.CompletedTask;
    }

    private static async Task DemonstrateSerialization()
    {
        System.Console.WriteLine("\n3. XML Serialization Demo");
        System.Console.WriteLine("-------------------------");

        var serializer = new XmlNetSerializer();
        
        // Example with a simple object
        var sampleData = new { Name = "Test", Value = 123 };
        
        try
        {
            var xml = serializer.Serialize(sampleData);
            System.Console.WriteLine("Serialized XML:");
            System.Console.WriteLine(xml);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Serialization example failed: {ex.Message}");
            System.Console.WriteLine("Note: Anonymous types may not serialize properly without attributes");
        }

        await Task.CompletedTask;
    }
}
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace EFinanceira.Tools.CodeGen;

/// <summary>
/// Aplicação CLI para geração de código a partir de XSDs do e-Financeira
/// </summary>
public class Program
{
    private static ILogger<Program>? _logger;

    public static async Task<int> Main(string[] args)
    {
        // Configurar logging
        using var loggerFactory = LoggerFactory.Create(builder =>
            builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        _logger = loggerFactory.CreateLogger<Program>();

        // Definir opções de linha de comando
        var rootCommand = CreateRootCommand();

        // Executar comando
        return await rootCommand.InvokeAsync(args);
    }

    private static RootCommand CreateRootCommand()
    {
        var rootCommand = new RootCommand("EFinanceira Code Generator - Gera POCOs a partir de schemas XSD");

        // Comando para gerar código de um XSD específico
        var generateSingleCommand = CreateGenerateSingleCommand();
        rootCommand.AddCommand(generateSingleCommand);

        // Comando para gerar código de todos os XSDs em um diretório
        var generateAllCommand = CreateGenerateAllCommand();
        rootCommand.AddCommand(generateAllCommand);

        // Comando para listar XSDs disponíveis
        var listCommand = CreateListCommand();
        rootCommand.AddCommand(listCommand);

        return rootCommand;
    }

    private static Command CreateGenerateSingleCommand()
    {
        var inputOption = new Option<FileInfo>(
            aliases: ["--input", "-i"],
            description: "Caminho para o arquivo XSD de entrada")
        {
            IsRequired = true
        };

        var outputOption = new Option<DirectoryInfo>(
            aliases: ["--output", "-o"],
            description: "Diretório de saída para as classes geradas")
        {
            IsRequired = true
        };

        var namespaceOption = new Option<string>(
            aliases: ["--namespace", "-n"],
            description: "Namespace das classes geradas")
        {
            IsRequired = true
        };

        var toolOption = new Option<CodeGenTool>(
            aliases: ["--tool", "-t"],
            description: "Ferramenta de geração (XmlSchemaClassGenerator ou XsdExe)")
        {
            IsRequired = false
        };
        toolOption.SetDefaultValue(CodeGenTool.XmlSchemaClassGenerator);

        var command = new Command("generate", "Gera POCOs a partir de um arquivo XSD específico");
        command.AddOption(inputOption);
        command.AddOption(outputOption);
        command.AddOption(namespaceOption);
        command.AddOption(toolOption);

        command.SetHandler(async (input, output, namespaceName, tool) =>
        {
            await GenerateSingle(input, output, namespaceName, tool);
        }, inputOption, outputOption, namespaceOption, toolOption);

        return command;
    }

    private static Command CreateGenerateAllCommand()
    {
        var inputOption = new Option<DirectoryInfo>(
            aliases: ["--input", "-i"],
            description: "Diretório contendo arquivos XSD")
        {
            IsRequired = true
        };

        var outputOption = new Option<DirectoryInfo>(
            aliases: ["--output", "-o"],
            description: "Diretório base de saída")
        {
            IsRequired = true
        };

        var namespaceOption = new Option<string>(
            aliases: ["--namespace", "-n"],
            description: "Namespace base das classes geradas")
        {
            IsRequired = true
        };

        var toolOption = new Option<CodeGenTool>(
            aliases: ["--tool", "-t"],
            description: "Ferramenta de geração")
        {
            IsRequired = false
        };
        toolOption.SetDefaultValue(CodeGenTool.XmlSchemaClassGenerator);

        var command = new Command("generate-all", "Gera POCOs para todos os XSDs em um diretório");
        command.AddOption(inputOption);
        command.AddOption(outputOption);
        command.AddOption(namespaceOption);
        command.AddOption(toolOption);

        command.SetHandler(async (input, output, namespaceName, tool) =>
        {
            await GenerateAll(input, output, namespaceName, tool);
        }, inputOption, outputOption, namespaceOption, toolOption);

        return command;
    }

    private static Command CreateListCommand()
    {
        var inputOption = new Option<DirectoryInfo>(
            aliases: ["--input", "-i"],
            description: "Diretório para listar XSDs")
        {
            IsRequired = true
        };

        var command = new Command("list", "Lista arquivos XSD disponíveis");
        command.AddOption(inputOption);

        command.SetHandler(ListXsds, inputOption);

        return command;
    }

    private static async Task GenerateSingle(FileInfo input, DirectoryInfo output, string namespaceName, CodeGenTool tool)
    {
        _logger?.LogInformation("Gerando código para: {Input}", input.FullName);
        _logger?.LogInformation("Saída: {Output}", output.FullName);
        _logger?.LogInformation("Namespace: {Namespace}", namespaceName);
        _logger?.LogInformation("Ferramenta: {Tool}", tool);

        try
        {
            if (!input.Exists)
            {
                _logger?.LogError("Arquivo XSD não encontrado: {Input}", input.FullName);
                return;
            }

            // Criar diretório de saída se não existir
            if (!output.Exists)
            {
                output.Create();
            }

            var generator = CreateGenerator(tool);
            await generator.GenerateAsync(input.FullName, output.FullName, namespaceName);

            _logger?.LogInformation("Código gerado com sucesso!");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao gerar código");
        }
    }

    private static async Task GenerateAll(DirectoryInfo input, DirectoryInfo output, string namespaceBase, CodeGenTool tool)
    {
        _logger?.LogInformation("Gerando código para todos os XSDs em: {Input}", input.FullName);

        try
        {
            if (!input.Exists)
            {
                _logger?.LogError("Diretório de entrada não encontrado: {Input}", input.FullName);
                return;
            }

            var xsdFiles = input.GetFiles("*.xsd", SearchOption.AllDirectories);
            _logger?.LogInformation("Encontrados {Count} arquivos XSD", xsdFiles.Length);

            var generator = CreateGenerator(tool);

            foreach (var xsdFile in xsdFiles)
            {
                try
                {
                    // Calcular namespace e diretório de saída baseado na estrutura
                    var relativePath = Path.GetRelativePath(input.FullName, xsdFile.DirectoryName!);
                    var namespaceForFile = string.IsNullOrEmpty(relativePath)
                        ? namespaceBase
                        : $"{namespaceBase}.{relativePath.Replace(Path.DirectorySeparatorChar, '.')}";

                    var outputPath = Path.Combine(output.FullName, relativePath);
                    Directory.CreateDirectory(outputPath);

                    _logger?.LogInformation("Processando: {File}", xsdFile.Name);
                    await generator.GenerateAsync(xsdFile.FullName, outputPath, namespaceForFile);
                    _logger?.LogInformation("✓ {File} processado", xsdFile.Name);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Erro ao processar {File}", xsdFile.Name);
                }
            }

            _logger?.LogInformation("Processamento concluído!");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro durante o processamento em lote");
        }
    }

    private static void ListXsds(DirectoryInfo input)
    {
        _logger?.LogInformation("Listando XSDs em: {Input}", input.FullName);

        try
        {
            if (!input.Exists)
            {
                _logger?.LogError("Diretório não encontrado: {Input}", input.FullName);
                return;
            }

            var xsdFiles = input.GetFiles("*.xsd", SearchOption.AllDirectories);

            if (xsdFiles.Length == 0)
            {
                _logger?.LogInformation("Nenhum arquivo XSD encontrado");
                return;
            }

            _logger?.LogInformation("Arquivos XSD encontrados ({Count}):", xsdFiles.Length);

            foreach (var file in xsdFiles.OrderBy(f => f.FullName))
            {
                var relativePath = Path.GetRelativePath(input.FullName, file.FullName);
                var sizeKb = file.Length / 1024.0;
                _logger?.LogInformation("  {Path} ({Size:F1} KB)", relativePath, sizeKb);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao listar arquivos");
        }
    }

    private static ICodeGenerator CreateGenerator(CodeGenTool tool)
    {
        return tool switch
        {
            CodeGenTool.XmlSchemaClassGenerator => new XscGenCodeGenerator(_logger!),
            CodeGenTool.XsdExe => new XsdExeCodeGenerator(_logger!),
            _ => throw new ArgumentException($"Ferramenta não suportada: {tool}")
        };
    }
}

/// <summary>
/// Ferramentas de geração de código disponíveis
/// </summary>
public enum CodeGenTool
{
    XmlSchemaClassGenerator,
    XsdExe
}

/// <summary>
/// Interface para geradores de código
/// </summary>
public interface ICodeGenerator
{
    Task GenerateAsync(string xsdPath, string outputPath, string namespaceName);
}

/// <summary>
/// Gerador usando XmlSchemaClassGenerator (dotnet tool)
/// </summary>
public class XscGenCodeGenerator : ICodeGenerator
{
    private readonly ILogger _logger;

    public XscGenCodeGenerator(ILogger logger)
    {
        _logger = logger;
    }

    public async Task GenerateAsync(string xsdPath, string outputPath, string namespaceName)
    {
        // Verificar se a ferramenta está instalada
        if (!await IsToolInstalled())
        {
            throw new InvalidOperationException("XmlSchemaClassGenerator não está instalado. Execute: dotnet tool install -g dotnet-xscgen");
        }

        var arguments = new List<string>
        {
            "-n", $"*={namespaceName}",
            "-o", outputPath,
            xsdPath
        };

        var startInfo = new ProcessStartInfo
        {
            FileName = "xscgen",
            Arguments = string.Join(" ", arguments.Select(arg => $"\"{arg}\"")),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        _logger.LogDebug("Executando: {FileName} {Arguments}", startInfo.FileName, startInfo.Arguments);

        using var process = new Process { StartInfo = startInfo };
        process.Start();

        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"xscgen falhou com código {process.ExitCode}. Erro: {error}");
        }

        if (!string.IsNullOrEmpty(output))
        {
            _logger.LogDebug("Saída xscgen: {Output}", output);
        }
    }

    private static async Task<bool> IsToolInstalled()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "xscgen",
                Arguments = "--version",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = startInfo };
            process.Start();
            await process.WaitForExitAsync();
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Gerador usando xsd.exe (framework clássico)
/// </summary>
public class XsdExeCodeGenerator : ICodeGenerator
{
    private readonly ILogger _logger;

    public XsdExeCodeGenerator(ILogger logger)
    {
        _logger = logger;
    }

    public async Task GenerateAsync(string xsdPath, string outputPath, string namespaceName)
    {
        var outputFile = Path.Combine(outputPath, Path.GetFileNameWithoutExtension(xsdPath) + ".cs");

        var arguments = new List<string>
        {
            $"\"{xsdPath}\"",
            "/c", // Generate classes
            $"/n:{namespaceName}",
            $"/o:\"{outputPath}\""
        };

        var startInfo = new ProcessStartInfo
        {
            FileName = "xsd.exe",
            Arguments = string.Join(" ", arguments),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        _logger.LogDebug("Executando: {FileName} {Arguments}", startInfo.FileName, startInfo.Arguments);

        using var process = new Process { StartInfo = startInfo };
        process.Start();

        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"xsd.exe falhou com código {process.ExitCode}. Erro: {error}");
        }

        if (!string.IsNullOrEmpty(output))
        {
            _logger.LogDebug("Saída xsd.exe: {Output}", output);
        }
    }
}

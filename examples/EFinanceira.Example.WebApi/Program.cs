using EFinanceira.Core.Extensions;
using EFinanceira.Core.Models;
using EFinanceira.Core.Services;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add e-Financeira services
builder.Services.AddEFinanceira(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EFinanceira API",
        Version = "v1.0.0",
        Description = "API para geração de arquivos XML da e-Financeira",
        Contact = new OpenApiContact
        {
            Name = "DevComputaria",
            Email = "contato@devcomputaria.com",
            Url = new Uri("https://github.com/DevComputaria")
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Add security definition for future JWT implementation
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EFinanceira API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at app's root
    });
}

app.UseHttpsRedirection();

// e-Financeira Endpoints
app.MapPost("/api/e-financeira/generate", async (
    IEFinanceiraService eFinanceiraService,
    GenerateEFinanceiraRequest request) =>
{
    try
    {
        // Create envelope
        var envelope = await eFinanceiraService.CreateEnvelopeAsync(request.Responsavel);

        // Add events based on request
        foreach (var movimentacao in request.Movimentacoes ?? [])
        {
            await eFinanceiraService.AddMovimentacaoFinanceiraAsync(envelope, movimentacao);
        }

        foreach (var abertura in request.AberturaContas ?? [])
        {
            await eFinanceiraService.AddAberturaContaAsync(envelope, abertura);
        }

        foreach (var fechamento in request.FechamentoContas ?? [])
        {
            await eFinanceiraService.AddFechamentoContaAsync(envelope, fechamento);
        }

        // Generate XML
        var xml = await eFinanceiraService.ProcessAsync(envelope);

        return Results.Ok(new GenerateEFinanceiraResponse
        {
            Success = true,
            Xml = xml,
            EventCount = envelope.Eventos.Count,
            Responsavel = envelope.IdeRespons.NmRespons,
            GeneratedAt = DateTime.UtcNow
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
})
.WithName("GenerateEFinanceira")
.WithTags("e-Financeira")
.WithOpenApi(operation => new(operation)
{
    Summary = "Gera arquivo XML da e-Financeira",
    Description = "Endpoint para gerar arquivos XML da e-Financeira baseado nos dados fornecidos"
});

app.MapPost("/api/e-financeira/validate", async (
    IEFinanceiraService eFinanceiraService,
    ValidateEFinanceiraRequest request) =>
{
    try
    {
        var envelope = await eFinanceiraService.CreateEnvelopeAsync(request.Responsavel);
        
        // This will validate the envelope
        var xml = await eFinanceiraService.ProcessAsync(envelope);

        return Results.Ok(new { Success = true, Message = "Dados válidos" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Success = false, Error = ex.Message });
    }
})
.WithName("ValidateEFinanceira")
.WithTags("e-Financeira")
.WithOpenApi(operation => new(operation)
{
    Summary = "Valida dados da e-Financeira",
    Description = "Endpoint para validar dados da e-Financeira sem gerar o XML"
});

app.MapControllers();

app.Run();

// DTOs
public record GenerateEFinanceiraRequest
{
    public required ResponsavelIdentificacao Responsavel { get; init; }
    public List<MovimentacaoFinanceira>? Movimentacoes { get; init; }
    public List<AberturaConta>? AberturaContas { get; init; }
    public List<FechamentoConta>? FechamentoContas { get; init; }
}

public record GenerateEFinanceiraResponse
{
    public bool Success { get; init; }
    public string? Xml { get; init; }
    public int EventCount { get; init; }
    public string? Responsavel { get; init; }
    public DateTime GeneratedAt { get; init; }
}

public record ValidateEFinanceiraRequest
{
    public required ResponsavelIdentificacao Responsavel { get; init; }
}

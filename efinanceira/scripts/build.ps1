# Build script para o projeto EFinanceira
# Executa build, testes e empacotamento

param(
    [string]$Configuration = "Release",
    [switch]$SkipTests,
    [switch]$Pack,
    [switch]$GenerateCode
)

Write-Host "🚀 EFinanceira Build Script" -ForegroundColor Green
Write-Host "Configuração: $Configuration" -ForegroundColor Yellow

# Definir caminhos
$RootPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$SrcPath = Join-Path $RootPath "src"
$OutputPath = Join-Path $RootPath "artifacts"

# Criar diretório de saída
if (!(Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
}

try {
    # 1. Limpar builds anteriores
    Write-Host "🧹 Limpando builds anteriores..." -ForegroundColor Cyan
    dotnet clean --configuration $Configuration --verbosity minimal
    if ($LASTEXITCODE -ne 0) { throw "Falha na limpeza" }

    # 2. Restaurar dependências
    Write-Host "📦 Restaurando dependências..." -ForegroundColor Cyan
    dotnet restore
    if ($LASTEXITCODE -ne 0) { throw "Falha na restauração de dependências" }

    # 3. Gerar código se solicitado
    if ($GenerateCode) {
        Write-Host "🔧 Gerando código a partir de schemas XSD..." -ForegroundColor Cyan
        
        $CodeGenProject = Join-Path $SrcPath "EFinanceira.Tools.CodeGen"
        $SchemasPath = Join-Path $RootPath "schemas"
        $MessagesPath = Join-Path $SrcPath "EFinanceira.Messages"
        
        if (Test-Path $SchemasPath) {
            # Compilar ferramenta de geração primeiro
            Push-Location $CodeGenProject
            try {
                dotnet build --configuration $Configuration
                if ($LASTEXITCODE -ne 0) { throw "Falha na compilação da ferramenta CodeGen" }
                
                # Gerar POCOs para eventos
                $EventosSchemas = Get-ChildItem -Path $SchemasPath -Filter "evt*.xsd" -Recurse
                foreach ($schema in $EventosSchemas) {
                    $outputDir = Join-Path $MessagesPath "Generated\Eventos\$(Split-Path -Leaf (Split-Path -Parent $schema.FullName))"
                    Write-Host "  Gerando para: $($schema.Name)" -ForegroundColor Gray
                    
                    dotnet run -- xsc generate `
                        --input "$($schema.FullName)" `
                        --output "$outputDir" `
                        --namespace "EFinanceira.Messages.Generated.Eventos.$(Split-Path -Leaf (Split-Path -Parent $schema.FullName))"
                }
                
                # Gerar POCOs para xmldsig
                $XmldsigSchemas = Get-ChildItem -Path $SchemasPath -Filter "*xmldsig*.xsd" -Recurse
                foreach ($schema in $XmldsigSchemas) {
                    $outputDir = Join-Path $MessagesPath "Generated\Xmldsig\$(Split-Path -Leaf (Split-Path -Parent $schema.FullName))"
                    Write-Host "  Gerando para: $($schema.Name)" -ForegroundColor Gray
                    
                    dotnet run -- xsc generate `
                        --input "$($schema.FullName)" `
                        --output "$outputDir" `
                        --namespace "EFinanceira.Messages.Generated.Xmldsig.$(Split-Path -Leaf (Split-Path -Parent $schema.FullName))"
                }
            }
            finally {
                Pop-Location
            }
        }
        else {
            Write-Warning "Diretório de schemas não encontrado: $SchemasPath"
        }
    }

    # 4. Build da solução
    Write-Host "🔨 Compilando solução..." -ForegroundColor Cyan
    dotnet build --configuration $Configuration --no-restore --verbosity minimal
    if ($LASTEXITCODE -ne 0) { throw "Falha na compilação" }

    # 5. Executar testes (se não pular)
    if (!$SkipTests) {
        Write-Host "🧪 Executando testes..." -ForegroundColor Cyan
        dotnet test --configuration $Configuration --no-build --verbosity minimal --logger "trx" --results-directory $OutputPath
        if ($LASTEXITCODE -ne 0) { throw "Falha nos testes" }
    }
    else {
        Write-Host "⏭️  Testes pulados" -ForegroundColor Yellow
    }

    # 6. Empacotar (se solicitado)
    if ($Pack) {
        Write-Host "📦 Criando pacotes NuGet..." -ForegroundColor Cyan
        
        # Empacotar projetos principais
        $ProjectsToPack = @(
            "EFinanceira.Core",
            "EFinanceira.Messages",
            "EFinanceira.Tools.CodeGen"
        )
        
        foreach ($project in $ProjectsToPack) {
            $projectPath = Join-Path $SrcPath $project
            if (Test-Path "$projectPath\$project.csproj") {
                Write-Host "  Empacotando: $project" -ForegroundColor Gray
                dotnet pack "$projectPath\$project.csproj" `
                    --configuration $Configuration `
                    --no-build `
                    --output $OutputPath `
                    --verbosity minimal
                if ($LASTEXITCODE -ne 0) { throw "Falha ao empacotar $project" }
            }
        }
    }

    # 7. Relatório final
    Write-Host "`n✅ Build concluído com sucesso!" -ForegroundColor Green
    Write-Host "📁 Artefatos disponíveis em: $OutputPath" -ForegroundColor Gray
    
    if (Test-Path $OutputPath) {
        $artifacts = Get-ChildItem $OutputPath
        if ($artifacts) {
            Write-Host "`n📋 Artefatos gerados:" -ForegroundColor Cyan
            foreach ($artifact in $artifacts) {
                Write-Host "  - $($artifact.Name) ($([math]::Round($artifact.Length / 1KB, 2)) KB)" -ForegroundColor Gray
            }
        }
    }

}
catch {
    Write-Host "`n❌ Build falhou: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host "`n🎉 Script concluído!" -ForegroundColor Green

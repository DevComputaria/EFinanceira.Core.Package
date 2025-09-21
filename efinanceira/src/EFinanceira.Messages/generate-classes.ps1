# Script para gerar classes C# a partir dos schemas XSD do e-Financeira
# Utiliza xsd.exe para criar POCOs organizados por categoria

param(
    [string]$SchemasPath = ".\Schemas",
    [string]$OutputPath = ".\Generated",
    [switch]$Clean,
    [switch]$Verbose
)

# Configurações
$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

# Verificar se xsd.exe está disponível
$xsdExe = Get-Command "xsd.exe" -ErrorAction SilentlyContinue
if (-not $xsdExe) {
    # Tentar localizar no SDK do Windows
    $possiblePaths = @(
        "${env:ProgramFiles(x86)}\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\xsd.exe",
        "${env:ProgramFiles}\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\xsd.exe",
        "${env:ProgramFiles(x86)}\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\xsd.exe"
    )
    
    foreach ($path in $possiblePaths) {
        if (Test-Path $path) {
            $xsdExe = @{ Source = $path }
            break
        }
    }
    
    if (-not $xsdExe) {
        Write-Error "xsd.exe nao encontrado. Certifique-se de que o .NET Framework SDK ou Visual Studio esta instalado."
        exit 1
    }
}

Write-Host "Iniciando geracao de classes C# a partir dos schemas XSD..." -ForegroundColor Green
Write-Host "Schemas: $SchemasPath" -ForegroundColor Gray
Write-Host "Output: $OutputPath" -ForegroundColor Gray
Write-Host "xsd.exe: $($xsdExe.Source)" -ForegroundColor Gray
Write-Host ""

# Limpar diretório se solicitado
if ($Clean -and (Test-Path $OutputPath)) {
    Write-Host "Limpando diretorio de saida..." -ForegroundColor Yellow
    Remove-Item -Path "$OutputPath\*" -Recurse -Force
}

# Criar estrutura de diretórios
$categories = @{
    "Eventos" = @{
        "Pattern" = "evt*.xsd"
        "Namespace" = "EFinanceira.Messages.Generated.Eventos"
        "Description" = "Eventos do e-Financeira"
    }
    "Lotes" = @{
        "Pattern" = "*Lote*.xsd"
        "Namespace" = "EFinanceira.Messages.Generated.Lotes"
        "Description" = "Envio e retorno de lotes"
    }
    "Consultas" = @{
        "Pattern" = "ret*.xsd"
        "Namespace" = "EFinanceira.Messages.Generated.Consultas"
        "Description" = "Retornos de consultas"
    }
    "Xmldsig" = @{
        "Pattern" = "*xmldsig*.xsd"
        "Namespace" = "EFinanceira.Messages.Generated.Xmldsig"
        "Description" = "Assinatura digital XML"
    }
}

foreach ($category in $categories.Keys) {
    $categoryInfo = $categories[$category]
    $categoryPath = Join-Path $OutputPath $category
    
    if (-not (Test-Path $categoryPath)) {
        New-Item -ItemType Directory -Path $categoryPath -Force | Out-Null
    }
    
    Write-Host "Processando categoria: $category ($($categoryInfo.Description))" -ForegroundColor Cyan
    
    # Buscar arquivos XSD para esta categoria
    $xsdFiles = Get-ChildItem -Path $SchemasPath -Filter $categoryInfo.Pattern | Sort-Object Name
    
    if ($xsdFiles.Count -eq 0) {
        Write-Host "   Nenhum arquivo encontrado para o padrao: $($categoryInfo.Pattern)" -ForegroundColor Yellow
        continue
    }
    
    Write-Host "   Encontrados $($xsdFiles.Count) arquivo(s)" -ForegroundColor Gray
    
    foreach ($xsdFile in $xsdFiles) {
        $fileName = [System.IO.Path]::GetFileNameWithoutExtension($xsdFile.Name)
        $outputFile = Join-Path $categoryPath "$fileName.cs"
        
        Write-Host "   Gerando: $fileName.cs" -ForegroundColor White
        
        try {
            # Executar xsd.exe
            $arguments = @(
                "/c"                                    # Gerar classes
                "/namespace:$($categoryInfo.Namespace)" # Namespace
                "/outputdir:$categoryPath"              # Diretório de saída
                $xsdFile.FullName                       # Arquivo XSD
            )
            
            if ($Verbose) {
                Write-Host "      Comando: xsd.exe $($arguments -join ' ')" -ForegroundColor DarkGray
            }
            
            $process = Start-Process -FilePath $xsdExe.Source -ArgumentList $arguments -Wait -NoNewWindow -PassThru
            
            if ($process.ExitCode -ne 0) {
                Write-Warning "      Erro ao processar $($xsdFile.Name) (Exit Code: $($process.ExitCode))"
                continue
            }
            
            # Renomear arquivo gerado (xsd.exe gera com nome genérico)
            $generatedFile = Get-ChildItem -Path $categoryPath -Filter "*.cs" | 
                Where-Object { $_.LastWriteTime -gt (Get-Date).AddMinutes(-1) } |
                Sort-Object LastWriteTime -Descending |
                Select-Object -First 1
            
            if ($generatedFile -and $generatedFile.Name -ne "$fileName.cs") {
                Rename-Item -Path $generatedFile.FullName -NewName "$fileName.cs" -Force
                if ($Verbose) {
                    Write-Host "      Renomeado: $($generatedFile.Name) -> $fileName.cs" -ForegroundColor DarkGray
                }
            }
            
            Write-Host "      Concluido" -ForegroundColor Green
            
        }
        catch {
            Write-Warning "      Erro: $($_.Exception.Message)"
        }
    }
    
    Write-Host ""
}

# Resumo final
Write-Host "Resumo da geracao:" -ForegroundColor Green
foreach ($category in $categories.Keys) {
    $categoryPath = Join-Path $OutputPath $category
    if (Test-Path $categoryPath) {
        $csFiles = Get-ChildItem -Path $categoryPath -Filter "*.cs" | Measure-Object
        Write-Host "   $category`: $($csFiles.Count) arquivo(s) gerado(s)" -ForegroundColor White
    }
}

# Verificar estrutura final
Write-Host ""
Write-Host "Estrutura gerada:" -ForegroundColor Green
if (Test-Path $OutputPath) {
    Get-ChildItem -Path $OutputPath -Recurse -Filter "*.cs" | 
        ForEach-Object {
            $relativePath = $_.FullName.Replace((Resolve-Path $OutputPath).Path, "").TrimStart('\', '/')
            Write-Host "   $relativePath" -ForegroundColor Gray
        }
}

Write-Host ""
Write-Host "Geracao de classes concluida!" -ForegroundColor Green
Write-Host "As classes estao organizadas por categoria em: $OutputPath" -ForegroundColor Cyan

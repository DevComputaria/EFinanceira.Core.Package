# Script aprimorado para gerar classes C# com resolução de dependências
# Processa schemas xmldsig primeiro, depois os demais com referências corretas

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

Write-Host "Iniciando geracao de classes C# com resolucao de dependencias..." -ForegroundColor Green
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
@("Eventos", "Lotes", "Consultas", "Xmldsig") | ForEach-Object {
    $categoryPath = Join-Path $OutputPath $_
    if (-not (Test-Path $categoryPath)) {
        New-Item -ItemType Directory -Path $categoryPath -Force | Out-Null
    }
}

# Função para processar um schema individual
function Process-Schema {
    param(
        [string]$SchemaFile,
        [string]$OutputDir,
        [string]$Namespace,
        [string[]]$Dependencies = @()
    )
    
    $fileName = [System.IO.Path]::GetFileNameWithoutExtension($SchemaFile)
    Write-Host "   Gerando: $fileName.cs" -ForegroundColor White
    
    try {
        # Construir argumentos para xsd.exe
        $arguments = @(
            "/c"                                    # Gerar classes
            "/namespace:$Namespace"                 # Namespace
            "/outputdir:$OutputDir"                # Diretório de saída
        )
        
        # Adicionar dependências se houver
        if ($Dependencies.Count -gt 0) {
            $arguments += $Dependencies
        }
        
        # Adicionar o schema principal
        $arguments += $SchemaFile
        
        if ($Verbose) {
            Write-Host "      Comando: xsd.exe $($arguments -join ' ')" -ForegroundColor DarkGray
        }
        
        $process = Start-Process -FilePath $xsdExe.Source -ArgumentList $arguments -Wait -NoNewWindow -PassThru
        
        if ($process.ExitCode -ne 0) {
            Write-Warning "      Erro ao processar $([System.IO.Path]::GetFileName($SchemaFile)) (Exit Code: $($process.ExitCode))"
            return $false
        }
        
        # Renomear arquivo gerado se necessário
        $generatedFile = Get-ChildItem -Path $OutputDir -Filter "*.cs" | 
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
        return $true
        
    }
    catch {
        Write-Warning "      Erro: $($_.Exception.Message)"
        return $false
    }
}

# 1. Processar schema de assinatura digital primeiro (dependência base)
Write-Host "1. Processando schema de assinatura digital..." -ForegroundColor Cyan
$xmldsigSchema = Join-Path $SchemasPath "xmldsig-core-schema.xsd"
if (Test-Path $xmldsigSchema) {
    $success = Process-Schema -SchemaFile $xmldsigSchema -OutputDir (Join-Path $OutputPath "Xmldsig") -Namespace "EFinanceira.Messages.Generated.Xmldsig"
    if (-not $success) {
        Write-Warning "Falha ao processar schema de assinatura digital"
    }
} else {
    Write-Warning "Schema de assinatura digital nao encontrado: $xmldsigSchema"
}

Write-Host ""

# 2. Processar schemas de consulta (geralmente mais simples)
Write-Host "2. Processando schemas de consulta..." -ForegroundColor Cyan
$consultaSchemas = Get-ChildItem -Path $SchemasPath -Filter "ret*.xsd" | Where-Object { $_.Name -notlike "*retorno*" }
foreach ($schema in $consultaSchemas) {
    Process-Schema -SchemaFile $schema.FullName -OutputDir (Join-Path $OutputPath "Consultas") -Namespace "EFinanceira.Messages.Generated.Consultas" | Out-Null
}

Write-Host ""

# 3. Processar schemas de lotes (sem dependências complexas)
Write-Host "3. Processando schemas de lotes..." -ForegroundColor Cyan
$loteSchemas = Get-ChildItem -Path $SchemasPath -Filter "*Lote*.xsd"
foreach ($schema in $loteSchemas) {
    # Tentar com dependência do xmldsig
    $xmldsigDep = if (Test-Path $xmldsigSchema) { $xmldsigSchema } else { $null }
    $dependencies = if ($xmldsigDep) { @($xmldsigDep) } else { @() }
    
    Process-Schema -SchemaFile $schema.FullName -OutputDir (Join-Path $OutputPath "Lotes") -Namespace "EFinanceira.Messages.Generated.Lotes" -Dependencies $dependencies | Out-Null
}

Write-Host ""

# 4. Processar retornos de lote separadamente
Write-Host "4. Processando retornos de lote..." -ForegroundColor Cyan
$retornoSchemas = Get-ChildItem -Path $SchemasPath -Filter "retorno*.xsd"
foreach ($schema in $retornoSchemas) {
    # Tentar com dependência do xmldsig
    $xmldsigDep = if (Test-Path $xmldsigSchema) { $xmldsigSchema } else { $null }
    $dependencies = if ($xmldsigDep) { @($xmldsigDep) } else { @() }
    
    Process-Schema -SchemaFile $schema.FullName -OutputDir (Join-Path $OutputPath "Lotes") -Namespace "EFinanceira.Messages.Generated.Lotes" -Dependencies $dependencies | Out-Null
}

Write-Host ""

# 5. Processar eventos (mais complexos, podem ter dependências)
Write-Host "5. Processando schemas de eventos..." -ForegroundColor Cyan
$eventoSchemas = Get-ChildItem -Path $SchemasPath -Filter "evt*.xsd"
foreach ($schema in $eventoSchemas) {
    # Tentar com dependência do xmldsig
    $xmldsigDep = if (Test-Path $xmldsigSchema) { $xmldsigSchema } else { $null }
    $dependencies = if ($xmldsigDep) { @($xmldsigDep) } else { @() }
    
    Process-Schema -SchemaFile $schema.FullName -OutputDir (Join-Path $OutputPath "Eventos") -Namespace "EFinanceira.Messages.Generated.Eventos" -Dependencies $dependencies | Out-Null
}

Write-Host ""

# Resumo final
Write-Host "Resumo da geracao:" -ForegroundColor Green
foreach ($category in @("Consultas", "Lotes", "Eventos", "Xmldsig")) {
    $categoryPath = Join-Path $OutputPath $category
    if (Test-Path $categoryPath) {
        $csFiles = Get-ChildItem -Path $categoryPath -Filter "*.cs" | Measure-Object
        Write-Host "   $category`: $($csFiles.Count) arquivo(s) gerado(s)" -ForegroundColor White
    }
}

Write-Host ""
Write-Host "Geracao de classes concluida!" -ForegroundColor Green
Write-Host "As classes estao organizadas por categoria em: $OutputPath" -ForegroundColor Cyan

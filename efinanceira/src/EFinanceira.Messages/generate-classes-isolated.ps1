# Script para gerar classes C# organizadas por schema individual
# Cada schema gera sua classe em uma pasta específica com namespace isolado

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

Write-Host "Iniciando geracao de classes C# organizadas por schema individual..." -ForegroundColor Green
Write-Host "Schemas: $SchemasPath" -ForegroundColor Gray
Write-Host "Output: $OutputPath" -ForegroundColor Gray
Write-Host "xsd.exe: $($xsdExe.Source)" -ForegroundColor Gray
Write-Host ""

# Limpar diretório se solicitado
if ($Clean -and (Test-Path $OutputPath)) {
    Write-Host "Limpando diretorio de saida..." -ForegroundColor Yellow
    Remove-Item -Path "$OutputPath\*" -Recurse -Force
}

# Função para processar um schema individual
function Process-Schema-Individual {
    param(
        [string]$SchemaFile,
        [string]$CategoryPath,
        [string]$SchemaName,
        [string]$BaseNamespace
    )
    
    $fileName = [System.IO.Path]::GetFileNameWithoutExtension($SchemaFile)
    $schemaFolder = Join-Path $CategoryPath $SchemaName
    
    # Criar pasta específica para o schema
    if (-not (Test-Path $schemaFolder)) {
        New-Item -ItemType Directory -Path $schemaFolder -Force | Out-Null
    }
    
    Write-Host "   Gerando: $SchemaName\$fileName.cs" -ForegroundColor White
    
    try {
        # Construir argumentos para xsd.exe (SEM dependências para evitar duplicações)
        $arguments = @(
            "/c"                                    # Gerar classes
            "/namespace:$BaseNamespace.$SchemaName" # Namespace específico
            "/outputdir:$schemaFolder"              # Diretório específico
            $SchemaFile                             # Schema individual
        )
        
        if ($Verbose) {
            Write-Host "      Comando: xsd.exe $($arguments -join ' ')" -ForegroundColor DarkGray
        }
        
        $process = Start-Process -FilePath $xsdExe.Source -ArgumentList $arguments -Wait -NoNewWindow -PassThru
        
        if ($process.ExitCode -ne 0) {
            Write-Warning "      Erro ao processar $([System.IO.Path]::GetFileName($SchemaFile)) (Exit Code: $($process.ExitCode))"
            return $false
        }
        
        # Renomear arquivo gerado se necessário
        $generatedFile = Get-ChildItem -Path $schemaFolder -Filter "*.cs" | 
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

# Configurações de schemas por categoria
$schemaCategories = @{
    "Consultas" = @{
        "Schemas"       = @{
            "retInfoCadastral-v1_2_0.xsd"     = "RetInfoCadastral"
            "retInfoIntermediario-v1_2_0.xsd" = "RetInfoIntermediario"
            "retInfoPatrocinado-v1_2_0.xsd"   = "RetInfoPatrocinado"
            "retInfoMovimento-v1_2_0.xsd"     = "RetInfoMovimento"
            "retListaeFinanceira-v1_2_0.xsd"  = "RetListaeFinanceira"
            "retRERCT-v1_2_0.xsd"             = "RetRERCT"
        }
        "BaseNamespace" = "EFinanceira.Messages.Generated.Consultas"
    }
    "Eventos"   = @{
        "Schemas"       = @{
            "evtAberturaeFinanceira-v1_2_1.xsd"         = "EvtAberturaeFinanceira"
            "evtCadEmpresaDeclarante-v1_2_0.xsd"        = "EvtCadEmpresaDeclarante"
            "evtIntermediario-v1_2_0.xsd"               = "EvtIntermediario"
            "evtPatrocinado-v1_2_0.xsd"                 = "EvtPatrocinado"
            "evtMovimentacaoFinanceira-v1_2_1.xsd"      = "EvtMovimentacaoFinanceira"
            "evtMovimentacaoFinanceiraAnual-v1_2_2.xsd" = "EvtMovimentacaoFinanceiraAnual"
            "evtFechamentoeFinanceira-v1_2_2.xsd"       = "EvtFechamentoeFinanceira"
            "evtFechamentoeFinanceira-v1_2_2-alt.xsd"   = "EvtFechamentoeFinanceiraAlt"
            "evtExclusao-v1_2_0.xsd"                    = "EvtExclusao"
            "evtExclusaoeFinanceira-v1_2_0.xsd"         = "EvtExclusaoeFinanceira"
            "evtRERCT-v1_2_0.xsd"                       = "EvtRERCT"
            "evtPrevidenciaPrivada-v1_2_5.xsd"          = "EvtPrevidenciaPrivada"
        }
        "BaseNamespace" = "EFinanceira.Messages.Generated.Eventos"
    }
    "Lotes"     = @{
        "Schemas"       = @{
            "envioLoteEventos-v1_2_0.xsd"             = "EnvioLoteEventos"
            "envioLoteEventosAssincrono-v1_0_0.xsd"   = "EnvioLoteEventosAssincrono"
            "envioLoteCriptografado-v1_2_0.xsd"       = "EnvioLoteCriptografado"
            "retornoLoteEventos-v1_2_0.xsd"           = "RetornoLoteEventos"
            "retornoLoteEventos-v1_3_0.xsd"           = "RetornoLoteEventos"
            "retornoLoteEventosAssincrono-v1_0_0.xsd" = "RetornoLoteEventosAssincrono"
        }
        "BaseNamespace" = "EFinanceira.Messages.Generated.Lotes"
    }
    "Xmldsig"   = @{
        "Schemas"       = @{
            "xmldsig-core-schema.xsd" = "Core"
        }
        "BaseNamespace" = "EFinanceira.Messages.Generated.Xmldsig"
    }
}

# Processar cada categoria
foreach ($categoryName in $schemaCategories.Keys) {
    $categoryInfo = $schemaCategories[$categoryName]
    $categoryPath = Join-Path $OutputPath $categoryName
    
    if (-not (Test-Path $categoryPath)) {
        New-Item -ItemType Directory -Path $categoryPath -Force | Out-Null
    }
    
    Write-Host "Processando categoria: $categoryName" -ForegroundColor Cyan
    
    $successCount = 0
    foreach ($schemaFile in $categoryInfo.Schemas.Keys) {
        $schemaName = $categoryInfo.Schemas[$schemaFile]
        $fullSchemaPath = Join-Path $SchemasPath $schemaFile
        
        if (Test-Path $fullSchemaPath) {
            $success = Process-Schema-Individual -SchemaFile $fullSchemaPath -CategoryPath $categoryPath -SchemaName $schemaName -BaseNamespace $categoryInfo.BaseNamespace
            if ($success) { $successCount++ }
        }
        else {
            Write-Warning "   Schema nao encontrado: $schemaFile"
        }
    }
    
    Write-Host "   $successCount/$($categoryInfo.Schemas.Count) schemas processados com sucesso" -ForegroundColor Gray
    Write-Host ""
}

# Resumo final
Write-Host "Resumo da geracao:" -ForegroundColor Green
foreach ($categoryName in $schemaCategories.Keys) {
    $categoryPath = Join-Path $OutputPath $categoryName
    if (Test-Path $categoryPath) {
        $csFiles = Get-ChildItem -Path $categoryPath -Recurse -Filter "*.cs" | Measure-Object
        Write-Host "   $categoryName`: $($csFiles.Count) arquivo(s) gerado(s)" -ForegroundColor White
    }
}

Write-Host ""
Write-Host "Geracao de classes concluida!" -ForegroundColor Green
Write-Host "Cada schema foi gerado em sua pasta especifica com namespace isolado." -ForegroundColor Cyan

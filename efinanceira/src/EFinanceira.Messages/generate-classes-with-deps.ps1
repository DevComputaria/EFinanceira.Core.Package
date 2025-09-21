# Script para gerar classes C# com dependências xmldsig
# Gera xmldsig primeiro, depois os schemas que dependem dele

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

Write-Host "Iniciando geracao de classes C# com dependencias..." -ForegroundColor Green
Write-Host "Schemas: $SchemasPath" -ForegroundColor Gray
Write-Host "Output: $OutputPath" -ForegroundColor Gray
Write-Host "xsd.exe: $($xsdExe.Source)" -ForegroundColor Gray
Write-Host ""

# Limpar diretório se solicitado
if ($Clean -and (Test-Path $OutputPath)) {
    Write-Host "Limpando diretorio de saida..." -ForegroundColor Yellow
    Remove-Item -Path "$OutputPath\*" -Recurse -Force
}

# Função para processar um schema
function Process-Schema {
    param(
        [string[]]$SchemaFiles,
        [string]$CategoryPath,
        [string]$SchemaName,
        [string]$BaseNamespace
    )
    
    $schemaFolder = Join-Path $CategoryPath $SchemaName
    
    # Criar pasta específica para o schema
    if (-not (Test-Path $schemaFolder)) {
        New-Item -ItemType Directory -Path $schemaFolder -Force | Out-Null
    }
    
    Write-Host "   Gerando: $SchemaName" -ForegroundColor White
    
    try {
        # Construir argumentos para xsd.exe
        $arguments = @(
            "/c"                                    # Gerar classes
            "/namespace:$BaseNamespace.$SchemaName" # Namespace específico
            "/outputdir:$schemaFolder"              # Diretório específico
        )
        
        # Adicionar todos os schemas (principal + dependências)
        $arguments += $SchemaFiles
        
        if ($Verbose) {
            Write-Host "      Comando: xsd.exe $($arguments -join ' ')" -ForegroundColor DarkGray
        }
        
        $process = Start-Process -FilePath $xsdExe.Source -ArgumentList $arguments -Wait -NoNewWindow -PassThru
        
        if ($process.ExitCode -ne 0) {
            Write-Warning "      Erro ao processar $SchemaName (Exit Code: $($process.ExitCode))"
            return $false
        }
        
        Write-Host "      Concluido" -ForegroundColor Green
        return $true
        
    }
    catch {
        Write-Warning "      Erro: $($_.Exception.Message)"
        return $false
    }
}

# Primeiro, gerar xmldsig isoladamente
Write-Host "Processando categoria: Xmldsig" -ForegroundColor Cyan
$xmldsigPath = Join-Path $OutputPath "Xmldsig"
if (-not (Test-Path $xmldsigPath)) {
    New-Item -ItemType Directory -Path $xmldsigPath -Force | Out-Null
}

$xmldsigSchemaPath = Join-Path $SchemasPath "xmldsig-core-schema.xsd"
if (Test-Path $xmldsigSchemaPath) {
    $success = Process-Schema -SchemaFiles @($xmldsigSchemaPath) -CategoryPath $xmldsigPath -SchemaName "Core" -BaseNamespace "EFinanceira.Messages.Generated.Xmldsig"
    if ($success) {
        Write-Host "   1/1 schemas processados com sucesso" -ForegroundColor Gray
    }
}
else {
    Write-Warning "   Schema xmldsig nao encontrado: $xmldsigSchemaPath"
}
Write-Host ""

# Configurações de schemas por categoria (com dependências)
$schemaCategories = @{
    "Consultas" = @{
        "Schemas"       = @{
            "RetInfoCadastral"     = @("retInfoCadastral-v1_2_0.xsd")
            "RetInfoIntermediario" = @("retInfoIntermediario-v1_2_0.xsd")
            "RetInfoPatrocinado"   = @("retInfoPatrocinado-v1_2_0.xsd")
            "RetInfoMovimento"     = @("retInfoMovimento-v1_2_0.xsd")
            "RetListaeFinanceira"  = @("retListaeFinanceira-v1_2_0.xsd")
            "RetRERCT"             = @("retRERCT-v1_2_0.xsd")
        }
        "BaseNamespace" = "EFinanceira.Messages.Generated.Consultas"
    }
    "Eventos"   = @{
        "Schemas"       = @{
            "EvtAberturaeFinanceira"         = @("evtAberturaeFinanceira-v1_2_1.xsd", "xmldsig-core-schema.xsd")
            "EvtCadEmpresaDeclarante"        = @("evtCadEmpresaDeclarante-v1_2_0.xsd", "xmldsig-core-schema.xsd")
            "EvtIntermediario"               = @("evtIntermediario-v1_2_0.xsd", "xmldsig-core-schema.xsd")
            "EvtPatrocinado"                 = @("evtPatrocinado-v1_2_0.xsd", "xmldsig-core-schema.xsd")
            "EvtMovimentacaoFinanceira"      = @("evtMovimentacaoFinanceira-v1_2_1.xsd", "xmldsig-core-schema.xsd")
            "EvtMovimentacaoFinanceiraAnual" = @("evtMovimentacaoFinanceiraAnual-v1_2_2.xsd", "xmldsig-core-schema.xsd")
            "EvtFechamentoeFinanceira"       = @("evtFechamentoeFinanceira-v1_2_2.xsd", "xmldsig-core-schema.xsd")
            "EvtFechamentoeFinanceiraAlt"    = @("evtFechamentoeFinanceira-v1_2_2-alt.xsd", "xmldsig-core-schema.xsd")
            "EvtExclusao"                    = @("evtExclusao-v1_2_0.xsd", "xmldsig-core-schema.xsd")
            "EvtExclusaoeFinanceira"         = @("evtExclusaoeFinanceira-v1_2_0.xsd", "xmldsig-core-schema.xsd")
            "EvtRERCT"                       = @("evtRERCT-v1_2_0.xsd", "xmldsig-core-schema.xsd")
            "EvtPrevidenciaPrivada"          = @("evtPrevidenciaPrivada-v1_2_5.xsd", "xmldsig-core-schema.xsd")
        }
        "BaseNamespace" = "EFinanceira.Messages.Generated.Eventos"
    }
    "Lotes"     = @{
        "Schemas"       = @{
            "EnvioLoteEventos"             = @("envioLoteEventos-v1_2_0.xsd")
            "EnvioLoteEventosAssincrono"   = @("envioLoteEventosAssincrono-v1_0_0.xsd")
            "EnvioLoteCriptografado"       = @("envioLoteCriptografado-v1_2_0.xsd")
            "RetornoLoteEventos_v1_2_0"    = @("retornoLoteEventos-v1_2_0.xsd", "xmldsig-core-schema.xsd")
            "RetornoLoteEventos_v1_3_0"    = @("retornoLoteEventos-v1_3_0.xsd", "xmldsig-core-schema.xsd")
            "RetornoLoteEventosAssincrono" = @("retornoLoteEventosAssincrono-v1_0_0.xsd", "xmldsig-core-schema.xsd")
        }
        "BaseNamespace" = "EFinanceira.Messages.Generated.Lotes"
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
    foreach ($schemaName in $categoryInfo.Schemas.Keys) {
        $schemaFiles = $categoryInfo.Schemas[$schemaName]
        
        # Converter nomes de arquivo para caminhos completos
        $fullSchemaPaths = @()
        $allSchemasExist = $true
        
        foreach ($schemaFile in $schemaFiles) {
            $fullSchemaPath = Join-Path $SchemasPath $schemaFile
            if (Test-Path $fullSchemaPath) {
                $fullSchemaPaths += $fullSchemaPath
            }
            else {
                Write-Warning "   Schema nao encontrado: $schemaFile"
                $allSchemasExist = $false
                break
            }
        }
        
        if ($allSchemasExist) {
            $success = Process-Schema -SchemaFiles $fullSchemaPaths -CategoryPath $categoryPath -SchemaName $schemaName -BaseNamespace $categoryInfo.BaseNamespace
            if ($success) { $successCount++ }
        }
    }
    
    Write-Host "   $successCount/$($categoryInfo.Schemas.Count) schemas processados com sucesso" -ForegroundColor Gray
    Write-Host ""
}

# Resumo final
Write-Host "Resumo da geracao:" -ForegroundColor Green
foreach ($categoryName in @("Xmldsig") + $schemaCategories.Keys) {
    $categoryPath = Join-Path $OutputPath $categoryName
    if (Test-Path $categoryPath) {
        $csFiles = Get-ChildItem -Path $categoryPath -Recurse -Filter "*.cs" | Measure-Object
        Write-Host "   $categoryName`: $($csFiles.Count) arquivo(s) gerado(s)" -ForegroundColor White
    }
}

Write-Host ""
Write-Host "Geracao de classes concluida!" -ForegroundColor Green
Write-Host "Classes geradas com dependencias xmldsig resolvidas." -ForegroundColor Cyan

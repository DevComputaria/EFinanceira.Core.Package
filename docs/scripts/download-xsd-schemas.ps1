# Script para baixar todos os arquivos XSD do e-Financeira
# Baseado na estrutura do site http://sped.rfb.gov.br/pasta/show/1854

# Criar diretório para schemas se não existir
$schemasDir = ".\schemas"
if (!(Test-Path $schemasDir)) {
    New-Item -ItemType Directory -Path $schemasDir -Force
}

Write-Host "Iniciando download dos arquivos XSD do e-Financeira..." -ForegroundColor Green

# Lista de URLs dos arquivos XSD identificados
$xsdFiles = @{
    # Eventos
    "evtAberturaeFinanceira-v1_2_1.xsd"         = "http://sped.rfb.gov.br/arquivo/download/2445"
    "evtCadEmpresaDeclarante-v1_2_0.xsd"        = "http://sped.rfb.gov.br/arquivo/download/2446"
    "evtIntermediario-v1_2_0.xsd"               = "http://sped.rfb.gov.br/arquivo/download/2447"
    "evtPatrocinado-v1_2_0.xsd"                 = "http://sped.rfb.gov.br/arquivo/download/2448"
    "evtExclusao-v1_2_0.xsd"                    = "http://sped.rfb.gov.br/arquivo/download/2449"
    "evtFechamentoeFinanceira-v1_2_2.xsd"       = "http://sped.rfb.gov.br/arquivo/download/2451"
    "evtFechamentoeFinanceira-v1_2_2-alt.xsd"   = "http://sped.rfb.gov.br/arquivo/download/2598"
    "evtMovimentacaoFinanceira-v1_2_1.xsd"      = "http://sped.rfb.gov.br/arquivo/download/2450"
    "evtMovimentacaoFinanceiraAnual-v1_2_2.xsd" = "http://sped.rfb.gov.br/arquivo/download/2599"
    "evtExclusaoeFinanceira-v1_2_0.xsd"         = "http://sped.rfb.gov.br/arquivo/download/2452"
    "evtRERCT-v1_2_0.xsd"                       = "http://sped.rfb.gov.br/arquivo/download/2453"
    "evtPrevidenciaPrivada-v1_2_5.xsd"          = "http://sped.rfb.gov.br/arquivo/download/7367"
    
    # Consulta-evento (Retornos de consulta)
    "retInfoCadastral-v1_2_0.xsd"               = "http://sped.rfb.gov.br/arquivo/download/2439"
    "retInfoIntermediario-v1_2_0.xsd"           = "http://sped.rfb.gov.br/arquivo/download/2440"
    "retInfoPatrocinado-v1_2_0.xsd"             = "http://sped.rfb.gov.br/arquivo/download/2441"
    "retInfoMovimento-v1_2_0.xsd"               = "http://sped.rfb.gov.br/arquivo/download/2442"
    "retListaeFinanceira-v1_2_0.xsd"            = "http://sped.rfb.gov.br/arquivo/download/2443"
    "retRERCT-v1_2_0.xsd"                       = "http://sped.rfb.gov.br/arquivo/download/2444"
    
    # Envio-lote-assíncrono
    "envioLoteEventosAssincrono-v1_0_0.xsd"     = "http://sped.rfb.gov.br/arquivo/download/7554"
    "retornoLoteEventosAssincrono-v1_0_0.xsd"   = "http://sped.rfb.gov.br/arquivo/download/7555"
    
    # Envio-lote-síncrono
    "envioLoteEventos-v1_2_0.xsd"               = "http://sped.rfb.gov.br/arquivo/download/7557"
    "retornoLoteEventos-v1_2_0.xsd"             = "http://sped.rfb.gov.br/arquivo/download/7558"
    
    # Lote-criptografado
    "envioLoteCriptografado-v1_2_0.xsd"         = "http://sped.rfb.gov.br/arquivo/download/7560"
    
    # Validação da Assinatura (Padrão W3C)
    "xmldsig-core-schema.xsd"                   = "http://sped.rfb.gov.br/arquivo/download/1871"
    
    # Retorno Lote Eventos
    "retornoLoteEventos-v1_3_0.xsd"             = "http://sped.rfb.gov.br/arquivo/download/7834"
}

# Contador para progresso
$total = $xsdFiles.Count
$current = 0

# Baixar cada arquivo
foreach ($fileName in $xsdFiles.Keys) {
    $current++
    $url = $xsdFiles[$fileName]
    $outputPath = Join-Path $schemasDir $fileName
    
    Write-Host "[$current/$total] Baixando: $fileName" -ForegroundColor Yellow
    
    try {
        Invoke-WebRequest -Uri $url -OutFile $outputPath -ErrorAction Stop
        Write-Host "  ✓ Sucesso: $fileName" -ForegroundColor Green
    }
    catch {
        Write-Host "  ✗ Erro ao baixar $fileName : $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`nDownload concluído!" -ForegroundColor Green
Write-Host "Arquivos salvos em: $((Get-Item $schemasDir).FullName)" -ForegroundColor Cyan

# Listar arquivos baixados
Write-Host "`nArquivos baixados:" -ForegroundColor Cyan
Get-ChildItem $schemasDir -Filter "*.xsd" | Format-Table Name, Length, LastWriteTime -AutoSize

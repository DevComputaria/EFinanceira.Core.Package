# Script para baixar todos os exemplos de arquivos do e-Financeira
# Baseado na estrutura do site http://sped.rfb.gov.br/pasta/show/1846

# Criar diretórios para exemplos se não existirem
$exemplosDirs = @(
    ".\exemplos\xml-sem-assinatura",
    ".\exemplos\xml-com-assinatura", 
    ".\exemplos\codigo-fonte"
)

foreach ($dir in $exemplosDirs) {
    if (!(Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir -Force
    }
}

Write-Host "Iniciando download dos exemplos de arquivos do e-Financeira..." -ForegroundColor Green

# Lista de URLs dos exemplos de arquivos XML sem assinatura
$exemplosSemAssinatura = @{
    "evtMovOpFin.xml"                         = "http://sped.rfb.gov.br/arquivo/download/1889"
    "evtFechamentoeFinanceira.xml"            = "http://sped.rfb.gov.br/arquivo/download/1890"
    "evtExclusaoeFinanceira.xml"              = "http://sped.rfb.gov.br/arquivo/download/1891"
    "evtPatrocinado.xml"                      = "http://sped.rfb.gov.br/arquivo/download/1892"
    "evtCadEmpresaDeclarante.xml"             = "http://sped.rfb.gov.br/arquivo/download/1893"
    "evtCadEmpresaDeclarante_Retificacao.xml" = "http://sped.rfb.gov.br/arquivo/download/1894"
    "evtAberturaeFinanceira.xml"              = "http://sped.rfb.gov.br/arquivo/download/1895"
    "evtExclusao.xml"                         = "http://sped.rfb.gov.br/arquivo/download/1896"
    "ExemploArquivoRetorno.xml"               = "http://sped.rfb.gov.br/arquivo/download/1897"
    "evtMovOpFinAnual.xml"                    = "http://sped.rfb.gov.br/arquivo/download/4123"
    "evtPrevidenciaPrivada.xml"               = "http://sped.rfb.gov.br/arquivo/download/4124"
    "evtIntermediario.xml"                    = "http://sped.rfb.gov.br/arquivo/download/4125"
    "evtRERCT.xml"                            = "http://sped.rfb.gov.br/arquivo/download/4126"
}

# Lista de URLs dos exemplos de arquivos XML com assinatura e lote
$exemplosComAssinatura = @{
    "evtCadEmpresaDeclarante_assinado_lote.xml"  = "http://sped.rfb.gov.br/arquivo/download/1848"
    "evtPatrocinado_assinado_lote.xml"           = "http://sped.rfb.gov.br/arquivo/download/1849"
    "evtAberturaeFinanceira_assinado_lote.xml"   = "http://sped.rfb.gov.br/arquivo/download/1850"
    "evtMovOpFin_assinado_lote.xml"              = "http://sped.rfb.gov.br/arquivo/download/1851"
    "evtFechamentoeFinanceira_assinado_lote.xml" = "http://sped.rfb.gov.br/arquivo/download/1852"
}

# Lista de URLs dos exemplos de código fonte e criptografia
$exemplosCodigoFonte = @{
    "ExemploAssinadorXML_256bytes.zip"       = "http://sped.rfb.gov.br/arquivo/download/5869"
    "ExemploCriptografiaLoteEFinanceira.zip" = "http://sped.rfb.gov.br/arquivo/download/2689"
}

# Função para baixar arquivos com progresso
function Baixar-Arquivos {
    param(
        [hashtable]$arquivos,
        [string]$diretorio,
        [string]$categoria
    )
    
    $total = $arquivos.Count
    $current = 0
    
    Write-Host "`n=== $categoria ===" -ForegroundColor Cyan
    
    foreach ($fileName in $arquivos.Keys) {
        $current++
        $url = $arquivos[$fileName]
        $outputPath = Join-Path $diretorio $fileName
        
        Write-Host "[$current/$total] Baixando: $fileName" -ForegroundColor Yellow
        
        try {
            Invoke-WebRequest -Uri $url -OutFile $outputPath -ErrorAction Stop
            Write-Host "  ✓ Sucesso: $fileName" -ForegroundColor Green
        }
        catch {
            Write-Host "  ✗ Erro ao baixar $fileName : $($_.Exception.Message)" -ForegroundColor Red
        }
    }
}

# Baixar todos os exemplos
Baixar-Arquivos $exemplosSemAssinatura ".\exemplos\xml-sem-assinatura" "Exemplos XML sem Assinatura"
Baixar-Arquivos $exemplosComAssinatura ".\exemplos\xml-com-assinatura" "Exemplos XML com Assinatura e Lote"
Baixar-Arquivos $exemplosCodigoFonte ".\exemplos\codigo-fonte" "Exemplos de Código Fonte e Criptografia"

Write-Host "`nDownload concluído!" -ForegroundColor Green
Write-Host "Arquivos salvos em: $((Get-Item ".\exemplos").FullName)" -ForegroundColor Cyan

# Exibir relatório por categoria
Write-Host "`n=== RELATÓRIO DOS ARQUIVOS BAIXADOS ===" -ForegroundColor Green

Write-Host "`n1. Exemplos XML sem Assinatura:" -ForegroundColor Yellow
Get-ChildItem ".\exemplos\xml-sem-assinatura" -Filter "*.xml" | Sort-Object Name | Format-Table Name, Length, LastWriteTime -AutoSize

Write-Host "`n2. Exemplos XML com Assinatura e Lote:" -ForegroundColor Yellow  
Get-ChildItem ".\exemplos\xml-com-assinatura" -Filter "*.xml" | Sort-Object Name | Format-Table Name, Length, LastWriteTime -AutoSize

Write-Host "`n3. Exemplos de Código Fonte:" -ForegroundColor Yellow
Get-ChildItem ".\exemplos\codigo-fonte" -Filter "*.zip" | Sort-Object Name | Format-Table Name, Length, LastWriteTime -AutoSize

# Calcular totais
$totalXMLSemAssinatura = (Get-ChildItem ".\exemplos\xml-sem-assinatura" -Filter "*.xml").Count
$totalXMLComAssinatura = (Get-ChildItem ".\exemplos\xml-com-assinatura" -Filter "*.xml").Count  
$totalCodigoFonte = (Get-ChildItem ".\exemplos\codigo-fonte" -Filter "*.zip").Count
$totalGeral = $totalXMLSemAssinatura + $totalXMLComAssinatura + $totalCodigoFonte

Write-Host "`n=== RESUMO ===" -ForegroundColor Magenta
Write-Host "XML sem Assinatura: $totalXMLSemAssinatura arquivos" -ForegroundColor White
Write-Host "XML com Assinatura: $totalXMLComAssinatura arquivos" -ForegroundColor White
Write-Host "Código Fonte/Criptografia: $totalCodigoFonte arquivos" -ForegroundColor White
Write-Host "Total Geral: $totalGeral arquivos" -ForegroundColor Cyan

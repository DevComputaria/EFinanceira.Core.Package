# Script para baixar todas as tabelas de códigos do e-Financeira
# Baseado na estrutura do site http://sped.rfb.gov.br/pasta/show/1932

# Criar diretório para tabelas de códigos se não existir
$tabelasDir = ".\tabelas-codigos"
if (!(Test-Path $tabelasDir)) {
    New-Item -ItemType Directory -Path $tabelasDir -Force
}

Write-Host "Iniciando download das tabelas de códigos do e-Financeira..." -ForegroundColor Green

# Lista de URLs das tabelas de códigos identificadas
$tabelasCodigos = @{
    # Tabelas Geográficas
    "Municipios.txt"                             = "http://sped.rfb.gov.br/arquivo/download/1934"
    "Paises.txt"                                 = "http://sped.rfb.gov.br/arquivo/download/1937"
    "Unidades_da_Federacao.txt"                  = "http://sped.rfb.gov.br/arquivo/download/1949"
    
    # Categorias e Tipos de Declarante
    "Categorias_de_Declarante.txt"               = "http://sped.rfb.gov.br/arquivo/download/2455"
    "Categorias_de_Patrocinador.txt"             = "http://sped.rfb.gov.br/arquivo/download/2456"
    "Tipos_de_Declarado.txt"                     = "http://sped.rfb.gov.br/arquivo/download/1947"
    "Tipo_de_Relacao_de_Declarado.txt"           = "http://sped.rfb.gov.br/arquivo/download/1945"
    "Tipo_de_Proprietario.txt"                   = "http://sped.rfb.gov.br/arquivo/download/1943"
    
    # Tipos de Identificação e Nome
    "Tipos_de_Nome.txt"                          = "http://sped.rfb.gov.br/arquivo/download/2457"
    "Tipos_de_Endereco.txt"                      = "http://sped.rfb.gov.br/arquivo/download/2458"
    "Tipos_de_NI.txt"                            = "http://sped.rfb.gov.br/arquivo/download/1948"
    
    # Contas e Movimentação Financeira
    "Tipos_de_Conta.txt"                         = "http://sped.rfb.gov.br/arquivo/download/1946"
    "Subtipos_de_Conta.txt"                      = "http://sped.rfb.gov.br/arquivo/download/1939"
    "Tipos_de_Numero_de_Conta.txt"               = "http://sped.rfb.gov.br/arquivo/download/1940"
    "Tipo_de_Pagamento.txt"                      = "http://sped.rfb.gov.br/arquivo/download/1942"
    "Moedas.txt"                                 = "http://sped.rfb.gov.br/arquivo/download/2174"
    
    # Previdência Privada
    "Tipo_de_Plano_de_Previdencia_Privada.txt"   = "http://sped.rfb.gov.br/arquivo/download/2867"
    "Tipo_de_Produto_de_Previdencia_Privada.txt" = "http://sped.rfb.gov.br/arquivo/download/2868"
    "Tipo_de_Empresa_de_Previdencia_Privada.txt" = "http://sped.rfb.gov.br/arquivo/download/2869"
    
    # Impostos e Retenções
    "Tabela_de_Codigo_de_Retencao_de_IR.txt"     = "http://sped.rfb.gov.br/arquivo/download/2874"
    
    # RERCT e Eventos
    "Identificacao_do_Evento_RERCT.txt"          = "http://sped.rfb.gov.br/arquivo/download/2173"
    "Tipo_de_Evento_do_Arquivo_de_Retorno.txt"   = "http://sped.rfb.gov.br/arquivo/download/1941"
    
    # Informações Internacionais
    "Informacao_de_NIF_por_pais.txt"             = "http://sped.rfb.gov.br/arquivo/download/7445"
}

# Contador para progresso
$total = $tabelasCodigos.Count
$current = 0

# Baixar cada arquivo
foreach ($fileName in $tabelasCodigos.Keys) {
    $current++
    $url = $tabelasCodigos[$fileName]
    $outputPath = Join-Path $tabelasDir $fileName
    
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
Write-Host "Arquivos salvos em: $((Get-Item $tabelasDir).FullName)" -ForegroundColor Cyan

# Listar arquivos baixados
Write-Host "`nArquivos baixados:" -ForegroundColor Cyan
Get-ChildItem $tabelasDir -Filter "*.txt" | Sort-Object Name | Format-Table Name, Length, LastWriteTime -AutoSize

# Exibir informações sobre o Portal SPED Tabelas
Write-Host "`nInformação Adicional:" -ForegroundColor Magenta
Write-Host "Para consulta online e download das tabelas SPED da e-Financeira, acesse:" -ForegroundColor White
Write-Host "http://www.sped.fazenda.gov.br/spedtabelas/AppConsulta/publico/aspx/ConsultaTabelasExternas.aspx?CodSistema=efdfinanceira" -ForegroundColor Cyan

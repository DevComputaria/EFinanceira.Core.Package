# Build script simples para o projeto EFinanceira
param(
    [string]$Configuration = "Release",
    [switch]$SkipTests
)

Write-Host "🚀 EFinanceira Build Script" -ForegroundColor Green
Write-Host "Configuração: $Configuration" -ForegroundColor Yellow

try {
    # 1. Limpar builds anteriores
    Write-Host "🧹 Limpando builds anteriores..." -ForegroundColor Cyan
    dotnet clean --configuration $Configuration --verbosity minimal

    # 2. Restaurar dependências
    Write-Host "📦 Restaurando dependências..." -ForegroundColor Cyan
    dotnet restore

    # 3. Build da solução
    Write-Host "🔨 Compilando solução..." -ForegroundColor Cyan
    dotnet build --configuration $Configuration --no-restore --verbosity minimal

    # 4. Executar testes (se não pular)
    if (!$SkipTests) {
        Write-Host "🧪 Executando testes..." -ForegroundColor Cyan
        dotnet test --configuration $Configuration --no-build --verbosity minimal
    }
    else {
        Write-Host "⏭️  Testes pulados" -ForegroundColor Yellow
    }

    Write-Host "`n✅ Build concluído com sucesso!" -ForegroundColor Green
}
catch {
    Write-Host "`n❌ Build falhou: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host "`n🎉 Script concluído!" -ForegroundColor Green

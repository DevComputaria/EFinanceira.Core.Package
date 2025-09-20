# Code generation script for e-Financeira (PowerShell)
Write-Host "EFinanceira Code Generation Script" -ForegroundColor Green
Write-Host "==================================" -ForegroundColor Green

# Check if .NET SDK is available
try {
    $dotnetVersion = dotnet --version
    Write-Host "Using .NET SDK version: $dotnetVersion" -ForegroundColor Blue
} catch {
    Write-Error ".NET SDK not found. Please install .NET 8.0 or later."
    exit 1
}

# Base directory for XSD files
$XsdBase = "src/EFinanceira.Messages/Generated"

if (-not (Test-Path $XsdBase)) {
    Write-Error "Generated directory not found: $XsdBase"
    exit 1
}

Write-Host "Searching for XSD files in $XsdBase..." -ForegroundColor Yellow

# Find all XSD files
$xsdFiles = Get-ChildItem -Path $XsdBase -Recurse -Filter "*.xsd"

if ($xsdFiles.Count -eq 0) {
    Write-Warning "No XSD files found in $XsdBase"
    exit 0
}

foreach ($xsdFile in $xsdFiles) {
    Write-Host "Processing: $($xsdFile.FullName)" -ForegroundColor Cyan
    
    # Get the directory containing the XSD file
    $xsdDir = $xsdFile.Directory.FullName
    
    # This is where actual code generation would happen
    # Example using xsd.exe (if available on Windows):
    # & xsd.exe $xsdFile.FullName /classes /namespace:EFinanceira.Messages.Generated /outputdir:$xsdDir
    
    Write-Host "  Would generate C# classes in: $xsdDir" -ForegroundColor Gray
}

Write-Host ""
Write-Host "Code generation completed!" -ForegroundColor Green
Write-Host "Note: This is a placeholder script. Actual implementation depends on available XSD tools." -ForegroundColor Yellow
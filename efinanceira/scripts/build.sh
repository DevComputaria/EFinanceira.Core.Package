#!/bin/bash

# Build script para o projeto EFinanceira (Linux/macOS)
# Executa build, testes e empacotamento

set -e  # Exit on any error

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color

# Parâmetros
CONFIGURATION="Release"
SKIP_TESTS=false
PACK=false
GENERATE_CODE=false

# Parse argumentos
while [[ $# -gt 0 ]]; do
    case $1 in
        --configuration)
            CONFIGURATION="$2"
            shift 2
            ;;
        --skip-tests)
            SKIP_TESTS=true
            shift
            ;;
        --pack)
            PACK=true
            shift
            ;;
        --generate-code)
            GENERATE_CODE=true
            shift
            ;;
        -h|--help)
            echo "Uso: $0 [--configuration Release|Debug] [--skip-tests] [--pack] [--generate-code]"
            exit 0
            ;;
        *)
            echo "Parâmetro desconhecido: $1"
            exit 1
            ;;
    esac
done

echo -e "${GREEN}🚀 EFinanceira Build Script${NC}"
echo -e "${YELLOW}Configuração: $CONFIGURATION${NC}"

# Definir caminhos
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_PATH="$(dirname "$SCRIPT_DIR")"
SRC_PATH="$ROOT_PATH/src"
TESTS_PATH="$ROOT_PATH/tests"
OUTPUT_PATH="$ROOT_PATH/artifacts"

# Criar diretório de saída
mkdir -p "$OUTPUT_PATH"

# Função para verificar se comando existe
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Verificar dependências
if ! command_exists dotnet; then
    echo -e "${RED}❌ .NET SDK não encontrado. Instale o .NET 8.0 SDK.${NC}"
    exit 1
fi

echo -e "${CYAN}🧹 Limpando builds anteriores...${NC}"
dotnet clean --configuration "$CONFIGURATION" --verbosity minimal

echo -e "${CYAN}📦 Restaurando dependências...${NC}"
dotnet restore

# Gerar código se solicitado
if [ "$GENERATE_CODE" = true ]; then
    echo -e "${CYAN}🔧 Gerando código a partir de schemas XSD...${NC}"
    
    CODEGEN_PROJECT="$SRC_PATH/EFinanceira.Tools.CodeGen"
    SCHEMAS_PATH="$ROOT_PATH/schemas"
    MESSAGES_PATH="$SRC_PATH/EFinanceira.Messages"
    
    if [ -d "$SCHEMAS_PATH" ]; then
        # Compilar ferramenta de geração primeiro
        pushd "$CODEGEN_PROJECT" > /dev/null
        
        echo -e "${GRAY}  Compilando ferramenta CodeGen...${NC}"
        dotnet build --configuration "$CONFIGURATION"
        
        # Gerar POCOs para eventos
        find "$SCHEMAS_PATH" -name "evt*.xsd" -type f | while read -r schema; do
            schema_dir=$(dirname "$schema")
            schema_name=$(basename "$schema_dir")
            output_dir="$MESSAGES_PATH/Generated/Eventos/$schema_name"
            
            echo -e "${GRAY}  Gerando para: $(basename "$schema")${NC}"
            mkdir -p "$output_dir"
            
            dotnet run -- xsc generate \
                --input "$schema" \
                --output "$output_dir" \
                --namespace "EFinanceira.Messages.Generated.Eventos.$schema_name"
        done
        
        # Gerar POCOs para xmldsig
        find "$SCHEMAS_PATH" -name "*xmldsig*.xsd" -type f | while read -r schema; do
            schema_dir=$(dirname "$schema")
            schema_name=$(basename "$schema_dir")
            output_dir="$MESSAGES_PATH/Generated/Xmldsig/$schema_name"
            
            echo -e "${GRAY}  Gerando para: $(basename "$schema")${NC}"
            mkdir -p "$output_dir"
            
            dotnet run -- xsc generate \
                --input "$schema" \
                --output "$output_dir" \
                --namespace "EFinanceira.Messages.Generated.Xmldsig.$schema_name"
        done
        
        popd > /dev/null
    else
        echo -e "${YELLOW}⚠️  Diretório de schemas não encontrado: $SCHEMAS_PATH${NC}"
    fi
fi

echo -e "${CYAN}🔨 Compilando solução...${NC}"
dotnet build --configuration "$CONFIGURATION" --no-restore --verbosity minimal

# Executar testes (se não pular)
if [ "$SKIP_TESTS" = false ]; then
    echo -e "${CYAN}🧪 Executando testes...${NC}"
    dotnet test --configuration "$CONFIGURATION" --no-build --verbosity minimal --logger "trx" --results-directory "$OUTPUT_PATH"
else
    echo -e "${YELLOW}⏭️  Testes pulados${NC}"
fi

# Empacotar (se solicitado)
if [ "$PACK" = true ]; then
    echo -e "${CYAN}📦 Criando pacotes NuGet...${NC}"
    
    # Projetos para empacotar
    PROJECTS_TO_PACK=(
        "EFinanceira.Core"
        "EFinanceira.Messages"
        "EFinanceira.Tools.CodeGen"
    )
    
    for project in "${PROJECTS_TO_PACK[@]}"; do
        project_path="$SRC_PATH/$project"
        if [ -f "$project_path/$project.csproj" ]; then
            echo -e "${GRAY}  Empacotando: $project${NC}"
            dotnet pack "$project_path/$project.csproj" \
                --configuration "$CONFIGURATION" \
                --no-build \
                --output "$OUTPUT_PATH" \
                --verbosity minimal
        fi
    done
fi

# Relatório final
echo -e "\n${GREEN}✅ Build concluído com sucesso!${NC}"
echo -e "${GRAY}📁 Artefatos disponíveis em: $OUTPUT_PATH${NC}"

if [ -d "$OUTPUT_PATH" ] && [ "$(ls -A "$OUTPUT_PATH")" ]; then
    echo -e "\n${CYAN}📋 Artefatos gerados:${NC}"
    ls -la "$OUTPUT_PATH" | tail -n +2 | while read -r line; do
        filename=$(echo "$line" | awk '{print $9}')
        size=$(echo "$line" | awk '{print $5}')
        if [ "$filename" != "." ] && [ "$filename" != ".." ]; then
            size_kb=$((size / 1024))
            echo -e "${GRAY}  - $filename (${size_kb} KB)${NC}"
        fi
    done
fi

echo -e "\n${GREEN}🎉 Script concluído!${NC}"

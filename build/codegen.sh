#!/bin/bash

# Code generation script for e-Financeira
echo "EFinanceira Code Generation Script"
echo "=================================="

# Check if xsd tool is available
if ! command -v xsd &> /dev/null; then
    echo "Warning: xsd tool not found. Install Mono for Linux/Mac or .NET Framework SDK for Windows"
    echo "Falling back to manual generation instructions..."
    echo ""
    echo "To generate C# classes from XSD files:"
    echo "1. Install the appropriate XSD tool for your platform"
    echo "2. Run: xsd.exe <schema.xsd> /classes /namespace:<namespace>"
    echo "3. Place generated .cs files in the appropriate Generated folder"
    exit 1
fi

# Find all XSD files in the Generated structure
XSD_BASE="src/EFinanceira.Messages/Generated"

if [ ! -d "$XSD_BASE" ]; then
    echo "Error: Generated directory not found: $XSD_BASE"
    exit 1
fi

echo "Searching for XSD files in $XSD_BASE..."

find "$XSD_BASE" -name "*.xsd" | while read xsd_file; do
    echo "Processing: $xsd_file"
    
    # Get the directory containing the XSD file
    xsd_dir=$(dirname "$xsd_file")
    
    # Generate classes in the same directory
    # Note: This is a placeholder - actual implementation would depend on the XSD tool available
    echo "  Would generate C# classes in: $xsd_dir"
done

echo ""
echo "Code generation completed!"
echo "Note: This is a placeholder script. Actual implementation depends on available XSD tools."
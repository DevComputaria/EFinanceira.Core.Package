# Guia de Utilização dos Schemas XSD do e-Financeira em .NET

Este guia mostra como utilizar os arquivos XSD baixados para desenvolvimento de aplicações .NET que integram com o sistema e-Financeira.

## Gerando Classes C# a partir dos XSD

### Opção 1: Usando xsd.exe (Ferramenta do .NET Framework)

```powershell
# Para gerar classes a partir de um único XSD
xsd.exe evtAberturaeFinanceira-v1_2_1.xsd /classes /namespace:EFinanceira.Schemas

# Para gerar classes a partir de múltiplos XSD relacionados
xsd.exe *.xsd /classes /namespace:EFinanceira.Schemas /out:Generated
```

### Opção 2: Usando Microsoft.XmlSerializer.Generator (Recomendado para .NET Core/5+)

1. Adicione o pacote NuGet ao seu projeto:
```xml
<PackageReference Include="Microsoft.XmlSerializer.Generator" Version="2.0.0" />
```

2. Configure o projeto para gerar classes automaticamente:
```xml
<PropertyGroup>
    <GenerateXmlSerializerAssembly>true</GenerateXmlSerializerAssembly>
</PropertyGroup>
```

### Opção 3: Usando dotnet-xscgen

1. Instale a ferramenta:
```powershell
dotnet tool install -g dotnet-xscgen
```

2. Gere as classes:
```powershell
xscgen -o Generated --namespace EFinanceira.Schemas *.xsd
```

## Exemplo de Uso das Classes Geradas

```csharp
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using EFinanceira.Schemas;

public class EFinanceiraService
{
    public void SerializarEventoAbertura()
    {
        var evento = new evtAberturaeFinanceira
        {
            // Preencher propriedades do evento
            ideEvento = new ideEvento
            {
                indRetificacao = "1",
                tpAmb = "2", // Homologação
                // ... outras propriedades
            }
        };

        var serializer = new XmlSerializer(typeof(evtAberturaeFinanceira));
        
        using (var writer = new StringWriter())
        {
            serializer.Serialize(writer, evento);
            var xml = writer.ToString();
            Console.WriteLine(xml);
        }
    }

    public evtAberturaeFinanceira DeserializarEventoAbertura(string xmlContent)
    {
        var serializer = new XmlSerializer(typeof(evtAberturaeFinanceira));
        
        using (var reader = new StringReader(xmlContent))
        {
            return (evtAberturaeFinanceira)serializer.Deserialize(reader);
        }
    }
}
```

## Validação XML usando XSD

```csharp
using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

public class XmlValidator
{
    public bool ValidarXml(string xmlFilePath, string xsdFilePath)
    {
        var isValid = true;
        var settings = new XmlReaderSettings();
        
        settings.Schemas.Add(null, xsdFilePath);
        settings.ValidationType = ValidationType.Schema;
        settings.ValidationEventHandler += (sender, e) =>
        {
            Console.WriteLine($"Erro de validação: {e.Message}");
            isValid = false;
        };

        using (var reader = XmlReader.Create(xmlFilePath, settings))
        {
            try
            {
                while (reader.Read()) { }
            }
            catch (XmlException ex)
            {
                Console.WriteLine($"Erro XML: {ex.Message}");
                return false;
            }
        }

        return isValid;
    }
}
```

## Estrutura do Projeto Recomendada

```
EFinanceira.Core.Package/
├── schemas/                    # Arquivos XSD originais
├── src/
│   ├── EFinanceira.Schemas/    # Classes geradas dos XSD
│   ├── EFinanceira.Core/       # Lógica de negócio
│   ├── EFinanceira.Api/        # API para integração
│   └── EFinanceira.Tests/      # Testes unitários
└── samples/                    # Exemplos de XML
```

## Tipos de Eventos Principais

### Eventos de Ciclo de Vida
- **Abertura**: `evtAberturaeFinanceira` - Abertura da declaração
- **Fechamento**: `evtFechamentoeFinanceira` - Fechamento da declaração
- **Exclusão**: `evtExclusaoeFinanceira` - Exclusão de declaração

### Eventos de Cadastro
- **Empresa Declarante**: `evtCadEmpresaDeclarante` - Cadastro da empresa
- **Intermediário**: `evtIntermediario` - Cadastro de intermediário
- **Patrocinado**: `evtPatrocinado` - Cadastro de patrocinado

### Eventos de Movimentação
- **Movimentação Financeira**: `evtMovimentacaoFinanceira` - Movimentações mensais
- **Movimentação Anual**: `evtMovimentacaoFinanceiraAnual` - Consolidação anual
- **Previdência Privada**: `evtPrevidenciaPrivada` - Eventos específicos de previdência
- **RERCT**: `evtRERCT` - Regime Especial de Regularização Cambial e Tributária

## Configuração de Serialização XML

```csharp
[Serializable]
[XmlRoot("evtAberturaeFinanceira")]
public partial class EventoAbertura
{
    [XmlAttribute("id")]
    public string Id { get; set; }

    [XmlElement("ideEvento")]
    public IdeEvento IdeEvento { get; set; }

    // Configurar namespaces se necessário
    [XmlNamespaceDeclarations]
    public XmlSerializerNamespaces Namespaces { get; set; }

    public EventoAbertura()
    {
        Namespaces = new XmlSerializerNamespaces();
        Namespaces.Add("", "http://www.eFinanceira.gov.br/schemas/evtAberturaeFinanceira/v1_2_1");
    }
}
```

## Boas Práticas

1. **Versionamento**: Sempre use a versão mais recente dos schemas
2. **Validação**: Valide sempre o XML antes de enviar
3. **Logs**: Mantenha logs detalhados das operações
4. **Testes**: Crie testes unitários para validação dos XMLs
5. **Certificados**: Implemente assinatura digital corretamente
6. **Ambientes**: Use ambiente de homologação para testes

## Próximos Passos

1. Gere as classes C# usando uma das opções acima
2. Implemente a lógica de serialização/deserialização
3. Configure a validação XML
4. Implemente testes unitários
5. Configure a assinatura digital (se necessário)
6. Teste em ambiente de homologação

## Recursos Adicionais

- [Documentação oficial e-Financeira](http://sped.rfb.gov.br/item/show/1499)
- [Manual de orientação](http://sped.rfb.gov.br/item/show/1502)
- [Webservices](http://sped.rfb.gov.br/item/show/1777)

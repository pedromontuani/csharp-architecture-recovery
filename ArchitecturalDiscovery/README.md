# Architectural Discovery

## Descrição

O projeto **Architectural Discovery** é uma ferramenta para análise e visualização de clusters arquiteturais em projetos de software. Ele utiliza técnicas de aprendizado de máquina para identificar e visualizar relações entre diferentes componentes do código.

## Funcionalidades

- Análise de projetos C# utilizando o Roslyn.
- Clusterização de entidades com base em características extraídas do código.
- Visualização 2D e 3D dos clusters.
- Geração de gráficos de relacionamento entre clusters.
- Exportação de gráficos para arquivos PNG.

## Requisitos

- .NET 6.0 ou superior
- MSBuild
- Pacotes NuGet:
    - `Microsoft.Msagl`
    - `System.Drawing.Common`
    - `Plotly.NET`

## Instalação

1. Clone o repositório:
   ```sh
   git clone https://github.com/seu-usuario/architectural-discovery.git
   cd architectural-discovery
   ```

2. Adicione os pacotes NuGet necessários:
   ```sh
   dotnet add package Microsoft.Msagl
   dotnet add package System.Drawing.Common
   dotnet add package Plotly.NET
   ```

## Uso

1. Compile o projeto:
   ```sh
   dotnet build
   ```

2. Execute o programa fornecendo o caminho do projeto C# a ser analisado:
   ```sh
   dotnet run -- caminho/do/projeto
   ```

3. O programa irá gerar visualizações dos clusters e salvar os gráficos em arquivos PNG na pasta `output`.

## Estrutura do Projeto

- `ArchitecturalDiscovery/core`: Contém as classes principais para análise e clusterização.
- `ArchitecturalDiscovery/dto`: Contém as classes de transferência de dados.
- `ArchitecturalDiscovery/utils`: Contém utilitários auxiliares.
- `ArchitecturalDiscovery/models`: Contém os modelos de dados.
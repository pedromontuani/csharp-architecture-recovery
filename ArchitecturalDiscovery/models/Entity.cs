using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchiteturalDiscovery.dto;

public class Entity
{
    public ClassDeclarationSyntax classDeclaration;
    public int[] calledFunctions;
    public int[] accessedGlobalVariables;
    public int[] userDefinedTypes;
    public FeaturesTuple featuresTuple;
}
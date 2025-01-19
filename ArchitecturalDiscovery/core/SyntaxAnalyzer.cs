using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchiteturalDiscovery.core;

public static class SyntaxAnalyzer
{

    private static readonly HashSet<string> PrimitiveAndDefaultTypes = new HashSet<string>
    {
        "bool", "byte", "sbyte", "char", "decimal", "double", "float", "int", "uint", "long", "ulong", "object", "short", "ushort", "string", "void"
    };
   
    public static List<string> GetCalledFunctions(MethodDeclarationSyntax methodDeclaration)
    {
        var calledFunctions = new List<string>();

        var invocationExpressions = methodDeclaration.DescendantNodes()
            .OfType<InvocationExpressionSyntax>();

        foreach (var invocation in invocationExpressions)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                calledFunctions.Add(memberAccess.Name.Identifier.Text);
            } else if (invocation.Expression is IdentifierNameSyntax identifierName)
            {
                calledFunctions.Add(identifierName.Identifier.Text);
            }
        }

        return calledFunctions;
    }
    
    public static List<string> GetAccessedGlobalVariables(ClassDeclarationSyntax classDeclaration)
    {
        var accessedGlobalVariables = new List<string>();

        var fieldAccessExpressions = classDeclaration.DescendantNodes()
            .OfType<MemberAccessExpressionSyntax>();

        foreach (var fieldAccess in fieldAccessExpressions)
        {
            if (fieldAccess.Expression is IdentifierNameSyntax identifierName)
            {
                accessedGlobalVariables.Add(identifierName.Identifier.Text);
            }
        }

        return accessedGlobalVariables;
    }

    public static List<string> GetUserDefinedTypes(ClassDeclarationSyntax classDeclaration)
    {
        var userDefinedTypes = new List<string>();

        var typeSyntaxes = classDeclaration.DescendantNodes()
            .OfType<TypeSyntax>();

        foreach (var typeSyntax in typeSyntaxes)
        {
            string typeName = null;

            if (typeSyntax is IdentifierNameSyntax identifierName)
            {
                typeName = identifierName.Identifier.Text;
            }
            else if (typeSyntax is QualifiedNameSyntax qualifiedName)
            {
                typeName = qualifiedName.Right.Identifier.Text;
            }

            if (typeName != null && !PrimitiveAndDefaultTypes.Contains(typeName))
            {
                userDefinedTypes.Add(typeName);
            }
        }

        return userDefinedTypes;
    }
    
    public static IEnumerable<INamedTypeSymbol> GetExplicitlyReferencedClasses(MethodDeclarationSyntax methodDeclaration, SemanticModel semanticModel)
    {
        var referencedClasses = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

        var nodes = methodDeclaration.DescendantNodes();
        foreach (var node in nodes)
        {
            var symbolInfo = semanticModel.GetSymbolInfo(node);
            if (symbolInfo.Symbol is ITypeSymbol typeSymbol && typeSymbol.TypeKind == TypeKind.Class)
            {
                referencedClasses.Add((INamedTypeSymbol)typeSymbol);
            }
        }

        return referencedClasses;
    }
    
    public static IEnumerable<IMethodSymbol> GetCalledMethods(MethodDeclarationSyntax methodDeclaration, SemanticModel semanticModel)
    {
        var calledMethods = new List<IMethodSymbol>();

        var invocationExpressions = methodDeclaration.DescendantNodes().OfType<InvocationExpressionSyntax>();
        foreach (var invocation in invocationExpressions)
        {
            var symbolInfo = semanticModel.GetSymbolInfo(invocation);
            if (symbolInfo.Symbol is IMethodSymbol methodSymbol)
            {
                calledMethods.Add(methodSymbol);
            }
        }

        return calledMethods;
    }
    
    public static string GetPackageName(ClassDeclarationSyntax classDeclaration)
    {
        var parent = classDeclaration.Parent;
        while (parent != null && !(parent is NamespaceDeclarationSyntax))
        {
            parent = parent.Parent;
        }

        if (parent is NamespaceDeclarationSyntax namespaceDeclaration)
        {
            return namespaceDeclaration.Name.ToString();
        }

        return string.Empty;
    }
    
}
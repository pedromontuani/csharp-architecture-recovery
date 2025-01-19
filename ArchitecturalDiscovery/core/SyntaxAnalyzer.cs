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
    
}
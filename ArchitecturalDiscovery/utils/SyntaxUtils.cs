using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchiteturalDiscovery.utils;

public class MethodDeclarationSyntaxComparer : IEqualityComparer<MethodDeclarationSyntax>
{
    public bool Equals(MethodDeclarationSyntax x, MethodDeclarationSyntax y)
    {
        // Define your custom equality logic here
        return GetHashCode(x) == GetHashCode(y);
    }

    public int GetHashCode(MethodDeclarationSyntax obj)
    {
        // Define your custom hash code logic here
        return HashCode.Combine(StringUtils.GetFormattedMethodName(obj));
    }
    
}
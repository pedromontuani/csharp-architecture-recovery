using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchiteturalDiscovery.dto;

public class CsFile
{
    public Document document { get; }
    public SyntaxTree syntaxTree { get; }
    public ClassDeclarationSyntax[] classDeclarations { get; }

    public CsFile(Document document, SyntaxTree syntaxTree)
    {
        this.document = document;
        this.syntaxTree = syntaxTree;
        classDeclarations = ExtractClassDeclarations();
    }
    
    private ClassDeclarationSyntax[] ExtractClassDeclarations()
    {
        return syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().ToArray();
    }
}
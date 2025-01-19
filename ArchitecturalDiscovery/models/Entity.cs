using ArchiteturalDiscovery.utils;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchiteturalDiscovery.dto;

public class Entity
{
    public ClassDeclarationSyntax classDeclaration;
    public List<ClassDeclarationSyntax> referencesClasses = new();
    public double[] calledFunctions;
    public double[] accessedGlobalVariables;
    public double[] userDefinedTypes;
    public FeaturesTuple featuresTuple;
    public (double x, double y) position;

    public Entity(ClassDeclarationSyntax classDeclaration, double[] calledFunctions, double[] accessedGlobalVariables,
        double[] userDefinedTypes)
    {
        this.classDeclaration = classDeclaration;
        this.calledFunctions = calledFunctions;
        this.accessedGlobalVariables = accessedGlobalVariables;
        this.userDefinedTypes = userDefinedTypes;
    }
    
    public void FitFeatures(double[] functions, double[] globalVariables, double[] definedTypes, double[] weights)
    {
        this.calledFunctions = functions;
        this.accessedGlobalVariables = globalVariables;
        this.userDefinedTypes = definedTypes;
        
        double functionsAverage = (functions.Length > 0 ? functions.Sum() : 0) * weights[0];
        double globalVariablesAverage = (globalVariables.Length > 0 ? globalVariables.Sum() : 0) * weights[1];
        double definedTypesAverage = (definedTypes.Length > 0 ? definedTypes.Sum() : 0) * weights[2];

        featuresTuple = new FeaturesTuple(functionsAverage, globalVariablesAverage, definedTypesAverage);
        var reduction = PCAHelper.ReducePoint([functionsAverage, globalVariablesAverage, definedTypesAverage]);
        position = (reduction[0], reduction[1]);
    }
}
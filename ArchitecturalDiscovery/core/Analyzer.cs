using System.Collections.Concurrent;
using ArchiteturalDiscovery.dto;
using ArchiteturalDiscovery.utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchiteturalDiscovery.core;
public class Analyzer(Project msProject)
{
    private MappedFeatureDict _calledFunctions = new ConcurrentDictionary<string, double>();
    private MappedFeatureDict _accessedGlobalVariables = new ConcurrentDictionary<string, double>();
    private MappedFeatureDict _userDefinedTypes = new ConcurrentDictionary<string, double>();
    private MappedFeatureDict _packageNames = new ConcurrentDictionary<string, double>();
    private ConcurrentDictionary<string, int> _referencedClasses = new();
    private ConcurrentBag<Entity> _entities = new();

    public async Task Analyze()
    {
        var projectClasses = await GetProjectClasses();
        ExtractFeatures(projectClasses);
        await BuildReferencedClasses();
    }

    private async Task<CsFile[]> GetProjectClasses()
    {
        var tasks = msProject.Documents.Select(async d => new CsFile(d,
            (await d.GetSyntaxTreeAsync()))).ToArray();
        
        return await Task.WhenAll(tasks);
    }
    
    public List<Entity> GetResultDataSet()
    {
        var fitCalledFunctions = MinMaxScaler.Fit(_calledFunctions.Values.ToArray());
        var fitAccessedGlobalVariables = MinMaxScaler.Fit(_accessedGlobalVariables.Values.ToArray());
        var fitUserDefinedTypes = MinMaxScaler.Fit(_userDefinedTypes.Values.ToArray());
        
        Parallel.ForEach(_entities, entity =>
        {
            var scaledCalledFunctions = MinMaxScaler.Transform(entity.calledFunctions, fitCalledFunctions.min, fitCalledFunctions.max);
            var scaledAccessedGlobalVariables = MinMaxScaler.Transform(entity.accessedGlobalVariables, fitAccessedGlobalVariables.min, fitAccessedGlobalVariables.max);
            var scaledUserDefinedTypes = MinMaxScaler.Transform(entity.userDefinedTypes, fitUserDefinedTypes.min, fitUserDefinedTypes.max);
            entity.FitFeatures(scaledCalledFunctions, scaledAccessedGlobalVariables, scaledUserDefinedTypes, [1.5, 1.0, 2.0]);
        });
        
        return _entities.ToList();
    }
    
    public Dictionary<string, int> GetReferencedClasses()
    {
        return _referencedClasses.ToDictionary();
    }

    private void ExtractFeatures(CsFile[] projectClasses)
    {
        ClassDeclarationSyntax[] classDeclarationSyntaxes =
            projectClasses.SelectMany(c => c.classDeclarations).ToArray();
        
        Object[] lockObjects = { new object(),  new object(), new object(), new object()};

        Parallel.ForEach(classDeclarationSyntaxes, classDeclaration =>
        {
            MethodDeclarationSyntax[] methodDeclarationSyntaxes = classDeclaration.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .ToArray();

            var calledFunctions = methodDeclarationSyntaxes.SelectMany(SyntaxAnalyzer.GetCalledFunctions).ToList();
            var accessedGlobalVariables = SyntaxAnalyzer.GetAccessedGlobalVariables(classDeclaration);
            var userDefinedTypes = SyntaxAnalyzer.GetUserDefinedTypes(classDeclaration);
            var packageName = SyntaxAnalyzer.GetPackageName(classDeclaration);
            
            AddToMappedFeatureDict(calledFunctions, accessedGlobalVariables, userDefinedTypes, packageName, lockObjects);

            Entity entity = new Entity(classDeclaration,
                calledFunctions.Select(f => _calledFunctions[f])
                    .ToArray(),
                accessedGlobalVariables.Select(v => _accessedGlobalVariables[v])
                    .ToArray(),
                userDefinedTypes.Select(t => _userDefinedTypes[t])
                    .ToArray());;
            
            _entities.Add(entity);
        });
    }
    
    private void AddToMappedFeatureDict(List<string> functions, List<string> variables, List<string> types, string packageName, Object[] lockObj)
    {
        lock (lockObj[0])
        {
            functions.ForEach(f => _calledFunctions.TryAdd(f, _calledFunctions.Count));
        }
        
        lock (lockObj[1])
        {   
            variables.ForEach(v => _accessedGlobalVariables.TryAdd(v, _accessedGlobalVariables.Count));
        }
        
        lock (lockObj[2])
        {   
            types.ForEach(t => _userDefinedTypes.TryAdd(t, _userDefinedTypes.Count));
        }

        lock (lockObj[3])
        {
            _packageNames.TryAdd(packageName, _packageNames.Count);
        }
        
    }

    private async Task BuildReferencedClasses()
    {
        await Parallel.ForEachAsync(_entities, async (e, _) =>
        {
            var referencedClasses = await SemanticAnalyzer.GetReferencedClasses(e.classDeclaration, msProject);

            e.referencesClasses.AddRange(referencedClasses);
            referencedClasses.ForEach(r =>
            {
                _referencedClasses.AddOrUpdate(r.Identifier.Text, 1, (_, i) => i + 1);
            });
        });
    }
}

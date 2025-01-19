using System.Collections.Concurrent;
using ArchiteturalDiscovery.dto;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchiteturalDiscovery.core
{
    public class Analyzer(Project msProject)
    {
        private CsFile[] _projectClasses;
        private CommunitiesList _communities;
        private MappedFeatureDict _calledFunctions = new ConcurrentDictionary<string, int>();
        private MappedFeatureDict _accessedGlobalVariables = new ConcurrentDictionary<string, int>();
        private MappedFeatureDict _userDefinedTypes = new ConcurrentDictionary<string, int>();
        private ConcurrentBag<Entity> _entities = new();

        public async Task Analyze()
        {
            await GetProjectClasses();
            ExtractFeatures();
            System.Console.WriteLine("Analysis completed");
        }

        private async Task GetProjectClasses()
        {
            var tasks = msProject.Documents.Select(async d => new CsFile(d,
                (await d.GetSyntaxTreeAsync()))).ToArray();
            
            _projectClasses = await Task.WhenAll(tasks);
        }

        private void ExtractFeatures()
        {
            ClassDeclarationSyntax[] classDeclarationSyntaxes =
                _projectClasses.SelectMany(c => c.classDeclarations).ToArray();
            
            Object[] lockObjects = { new object(),  new object(), new object()};

            Parallel.ForEach(classDeclarationSyntaxes, classDeclaration =>
            {
                MethodDeclarationSyntax[] methodDeclarationSyntaxes = classDeclaration.DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .ToArray();

                var calledFunctions = methodDeclarationSyntaxes.SelectMany(SyntaxAnalyzer.GetCalledFunctions).ToList();
                var accessedGlobalVariables = SyntaxAnalyzer.GetAccessedGlobalVariables(classDeclaration);
                var userDefinedTypes = SyntaxAnalyzer.GetUserDefinedTypes(classDeclaration);

                AddToMappedFeatureDict(calledFunctions, accessedGlobalVariables, userDefinedTypes, lockObjects);

                Entity entity = new Entity
                {
                    classDeclaration = classDeclaration,
                    calledFunctions = calledFunctions.Select(f => _calledFunctions[f]).ToArray(),
                    accessedGlobalVariables =
                        accessedGlobalVariables.Select(v => _accessedGlobalVariables[v]).ToArray(),
                    userDefinedTypes = userDefinedTypes.Select(t => _userDefinedTypes[t]).ToArray()
                };
                
                _entities.Add(entity);
            });
        }
        
        private void AddToMappedFeatureDict(List<string> functions, List<string> variables, List<string> types, Object[] lockObj)
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
            
        }
    }
}
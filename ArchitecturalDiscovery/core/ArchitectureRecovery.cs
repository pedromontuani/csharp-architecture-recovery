using System.Collections.Concurrent;
using ArchiteturalDiscovery.dto;

namespace ArchiteturalDiscovery.core;

public class ArchitectureRecovery
{
    private ConcurrentDictionary<int, Dictionary<int, int>> _clusterRelationship = new ();
    private List<List<Entity>> _entityClusters;
    
    public ArchitectureRecovery(List<List<Entity>> entityClusters)
    {
        this._entityClusters = entityClusters;
        ExtractClusterRelationship();
    }

    private void ExtractClusterRelationship()
    {
        Parallel.ForEach(_entityClusters, cluster =>
        {
            var referencedClasses = cluster.SelectMany(c => c.referencesClasses).Select(c => c.Identifier.Text).ToList();
            
            _entityClusters.Where(c => c != cluster).ToList().ForEach(c =>
            {
                var classDeclarations = c.Select(x => x.classDeclaration.Identifier.Text).ToList();
                
                var intersections = referencedClasses.Intersect(classDeclarations).Count();
                
                if (intersections > 0)
                {
                    var a = _entityClusters.IndexOf(cluster);
                    var b = _entityClusters.IndexOf(c);

                    Dictionary<int, int> dict = new();
                    dict.Add(b, intersections);
                    
                    _clusterRelationship.AddOrUpdate(a, dict, (_, v) =>
                    {
                        v.Add(b, intersections);
                        return v;
                    });
                }
            });
        });
    }
    
    public Dictionary<int, List<int>> RecoverArchitecture()
    {
        var values = _clusterRelationship.Values.Select(v => v.Values.Any() ? v.Values.Average() : 0);
        var averageIntersections = values.Any() ? values.Average() : 1;
        Dictionary<int, List<int>> architecture = new();

        foreach (var relationship in _clusterRelationship)
        {
            foreach (var intersections in relationship.Value)
            {
                if (intersections.Value > averageIntersections)
                {
                    if (architecture.ContainsKey(relationship.Key))
                    {
                        architecture[relationship.Key].Add(intersections.Key);
                    }
                    else
                    {
                        architecture.Add(relationship.Key, new List<int> { intersections.Key });
                    }
                }
            }
        }
        return architecture;
    }

}
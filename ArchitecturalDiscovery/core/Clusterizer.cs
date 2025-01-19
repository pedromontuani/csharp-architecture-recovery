using System.Collections.Concurrent;
using ArchiteturalDiscovery.dto;

namespace ArchiteturalDiscovery.core;

public class Clusterizer(List<Entity> entities, Dictionary<string, int> referencedClasses)
{
    private List<Entity> _entities = entities;
    

    public List<List<Entity>> Clusterize3D()
    {
        var clusters = GetClusters3D();
        var omnipresentClasses = GetOmnipresentClasses();
        
        return GetFilteredEntitiesClusters(clusters, omnipresentClasses);
    }
    
    public List<List<Entity>> Clusterize2D()
    {
        var clusters = GetClusters2D();
        var omnipresentClasses = GetOmnipresentClasses();
        
        return GetFilteredEntitiesClusters(clusters, omnipresentClasses);
    }
    
   
    
    private List<int>[] GetClusters3D()
    {
         WeightedKmeans _kmeans = new();
         ElbowMethod _elbowMethod = new();
    
        double[][] data = entities.Select(e =>
        {
            var a = e.featuresTuple.fn;
            var b = e.featuresTuple.gVar;
            var c = e.featuresTuple.uTypes;

            return new double[] { a, b, c };
        }).ToArray();
        
        
        int optimalK = _elbowMethod.FindOptimalK(data, 10); // Assuming maxK is 10
        return _kmeans.Clusterize(data, Math.Max(optimalK, 3));
    }
    
    private List<int>[] GetClusters2D()
    {
        WeightedKmeans _kmeans = new();
        ElbowMethod _elbowMethod = new();
        
        double[][] data = entities.Select(e =>
        {
            var x = e.position.x;
            var y = e.position.y;

            return new double[] { x, y };
        }).ToArray();
        
        
        int optimalK = _elbowMethod.FindOptimalK(data, 10); // Assuming maxK is 10
        return _kmeans.Clusterize(data, Math.Max(optimalK, 3));
    }
    

    private List<Entity> GetOmnipresentClasses()
    {
        int classesCount = referencedClasses.Keys.Count;
        ConcurrentBag<Entity> omnipresentClasses = new();

        Parallel.ForEach(referencedClasses, kvp =>
        {
            if (kvp.Value > classesCount / 3)
            {
                var entity = _entities.Find(e => e.classDeclaration.Identifier.Text.Equals(kvp.Key));
                if (entity != null)
                {
                    omnipresentClasses.Add(entity);
                }
            }
        });
        
        return omnipresentClasses.ToList();
    }
    
    private List<List<Entity>> GetFilteredEntitiesClusters(List<int>[] clusters, List<Entity> omnipresentClasses)
    {
        ConcurrentBag<List<Entity>> filteredClusters = new();

        Parallel.ForEach(clusters, c =>
        {
            var newCluster = c.Where(index =>
            {
                var entity = _entities[index];

                return !omnipresentClasses.Contains(entity);
            }).Select(index => _entities[index]).ToList();

            if (newCluster.Count > 0)
            {
                filteredClusters.Add(newCluster);
            }
        });

        if (omnipresentClasses.Count > 0)
        {
            filteredClusters.Add(omnipresentClasses);
        }

        return filteredClusters.ToList();
    }
}
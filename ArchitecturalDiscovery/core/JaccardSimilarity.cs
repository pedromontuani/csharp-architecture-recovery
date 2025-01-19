namespace ArchiteturalDiscovery.core;

public static class JaccardSimilarity
{
    public static double Calculate(List<int>[] clusters1, List<int>[] clusters2)
    {
        var set1 = new HashSet<int>(clusters1.SelectMany(c => c));
        var set2 = new HashSet<int>(clusters2.SelectMany(c => c));

        var intersection = new HashSet<int>(set1);
        intersection.IntersectWith(set2);

        var union = new HashSet<int>(set1);
        union.UnionWith(set2);

        return (double)intersection.Count / union.Count;
    }
}
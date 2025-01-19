using Plotly.NET;
using Plotly.NET.LayoutObjects;
using ArchiteturalDiscovery.dto;
using ArchiteturalDiscovery.utils;
using Microsoft.FSharp.Core;
using QuickGraph;
using QuickGraph.Graphviz;

namespace ArchiteturalDiscovery.core;

public static class Report
{
    public static void PlotClusters3D(List<List<Entity>> clusters)
    {
        var scatterPlots = new List<GenericChart>();

        for (int i = 0; i < clusters.Count; i++)
        {
            var cluster = clusters[i];
            var x = cluster.Select(e => e.featuresTuple.fn).ToArray();
            var y = cluster.Select(e => e.featuresTuple.gVar).ToArray();
            var z = cluster.Select(e => e.featuresTuple.uTypes).ToArray();

            var scatter = Chart3D.Chart.Scatter3D<double, double, double, string>(
                x: x,
                y: y,
                z: z,
                mode: StyleParam.Mode.Markers,
                Name: $"Cluster {i + 1}"
            );

            scatterPlots.Add(scatter);
        }
        
        LinearAxis xAxis = new LinearAxis();
        xAxis.SetValue("title", "Functions called");
        xAxis.SetValue("zerolinecolor", "#ffff");
        xAxis.SetValue("gridcolor", "#ffff");
        xAxis.SetValue("showline", true);
        xAxis.SetValue("zerolinewidth",2);

        LinearAxis yAxis = new LinearAxis();
        yAxis.SetValue("title", "Global variables referenced");
        yAxis.SetValue("zerolinecolor", "#ffff");
        yAxis.SetValue("gridcolor", "#ffff");
        yAxis.SetValue("showline", true);
        yAxis.SetValue("zerolinewidth",2);
        
        LinearAxis zAxis = new LinearAxis();
        zAxis.SetValue("title", "User-defined types");
        zAxis.SetValue("zerolinecolor", "#ffff");
        zAxis.SetValue("gridcolor", "#ffff");
        zAxis.SetValue("showline", true);
        zAxis.SetValue("zerolinewidth",2);

        Layout layout = new Layout();
        layout.SetValue("xaxis", xAxis);
        layout.SetValue("yaxis", yAxis);
        layout.SetValue("title", "Architectural Clusters");
        layout.SetValue("plot_bgcolor", "#e5ecf6");
        layout.SetValue("showlegend", true);

        var chart = Chart.Combine(scatterPlots);
        chart.WithLayout(layout);

        chart.Show();
    }

    public static void PlotClusters2D(List<List<Entity>> clusters)
    {
        var scatterPlots = new List<GenericChart>();

        foreach (var cluster in clusters)
        {

            var x = cluster.Select(x => x.position.x).ToArray();
            var y = cluster.Select(x => x.position.y).ToArray();

            var scatter = Chart2D.Chart.Scatter<double, double, string>(
                x: x,
                y: y,
                mode: StyleParam.Mode.Markers,
                Name: $"Cluster {clusters.IndexOf(cluster) + 1}"
            );

            scatterPlots.Add(scatter);
        }
        
        LinearAxis xAxis = new LinearAxis();
        xAxis.SetValue("title", "Global variables referenced");
        xAxis.SetValue("zerolinecolor", "#ffff");
        xAxis.SetValue("gridcolor", "#ffff");
        xAxis.SetValue("showline", true);
        xAxis.SetValue("zerolinewidth",2);
        
        LinearAxis yAxis = new LinearAxis();
        yAxis.SetValue("title", "User-defined types");
        yAxis.SetValue("zerolinecolor", "#ffff");
        yAxis.SetValue("gridcolor", "#ffff");
        yAxis.SetValue("showline", true);
        yAxis.SetValue("zerolinewidth",2);

        var layout = new Layout();
        layout.SetValue("xaxis", xAxis);
        layout.SetValue("yaxis", yAxis);
        layout.SetValue("title", "Clusters Relationships");
        layout.SetValue("plot_bgcolor", "#e5ecf6");
        layout.SetValue("showlegend", true);

        var chart = Chart.Combine(scatterPlots);
        chart.WithLayout(layout);

        chart.Show();
    }

    public static void PlotGraph(Dictionary<int, List<int>> clustersRelationship)
    {
        var graph = new AdjacencyGraph<int, Edge<int>>();
        List<int> verticesList = new List<int>();
        verticesList.AddRange(clustersRelationship.Keys);
        verticesList.AddRange(clustersRelationship.Values.SelectMany(v => v).ToArray());
        graph.AddVertexRange(verticesList.Distinct());
        
        foreach (var relationship in clustersRelationship)
        {
            foreach (var cluster in relationship.Value)
            {
                graph.AddEdge(new Edge<int>(relationship.Key, cluster));
            }
        }
        
        var dotCode = graph.ToGraphviz();
        
        GraphvizHelper.RenderGraph(dotCode, "output/out.png");
    }

}

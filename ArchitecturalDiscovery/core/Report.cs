using Plotly.NET;
using Plotly.NET.LayoutObjects;
using ArchiteturalDiscovery.dto;
using ArchiteturalDiscovery.utils;
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
                Name: $"Cluster {i + 1}",
                MultiText: cluster.Select(x => x.classDeclaration.Identifier.Text).ToArray()
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
        
        chart.SaveHtml("output/clusters3D.html");
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
                Name: $"Cluster {clusters.IndexOf(cluster) + 1}",
                MultiText: cluster.Select(x => x.classDeclaration.Identifier.Text).ToArray()
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

        chart.SaveHtml("output/clusters2D.html");
    }
    
    public static void PlotGraph(List<List<Entity>> clusters, Dictionary<int, List<int>> clustersRelationship)
    {
        var graph = new AdjacencyGraph<string, Edge<string>>();
        List<string> verticesList = new List<string>();
        verticesList.AddRange(Enumerable.Range(0, clusters.Count).Select(e => "Cluster " + e).ToList());
        graph.AddVertexRange(verticesList.Distinct());
        
        foreach (var relationship in clustersRelationship)
        {
            foreach (var cluster in relationship.Value)
            {
                graph.AddEdge(new Edge<string>("Cluster " + relationship.Key, "Cluster " + cluster));
            }
        }
        
        GraphvizHelper.RenderGraph(graph.ToGraphviz(), "output/graph.png");
        
    }

    // public static void PlotGraph(List<List<Entity>> clusters, Dictionary<int, List<int>> clustersRelationship)
    // {
    //     
    //     var graph = new AdjacencyGraph<int, Edge<int>>();
    //     List<int> verticesList = new List<int>();
    //     verticesList.AddRange(Enumerable.Range(0, clusters.Count));
    //     graph.AddVertexRange(verticesList.Distinct());
    //     
    //     foreach (var relationship in clustersRelationship)
    //     {
    //         foreach (var cluster in relationship.Value)
    //         {
    //             graph.AddEdge(new Edge<int>(relationship.Key, cluster));
    //         }
    //     }
    //
    //
    //     var scatterPlots = new List<GenericChart>();
    //
    //     foreach (var cluster in clusters)
    //     {
    //         var center = GetClusterCenter(cluster);
    //
    //         var scatter = Chart2D.Chart.Bubble<double, double, string>(
    //             x: [center[0]],
    //             y: [center[1]],
    //             Name: $"Cluster {clusters.IndexOf(cluster) + 1}",
    //             sizes: [50]
    //         );
    //
    //         scatterPlots.Add(scatter);
    //     }
    //     
    //     var edges = graph.Edges.Select(e => new Tuple<int, int>(e.Source, e.Target)).ToList();
    //     
    //     var annotations = new List<Annotation>();
    //
    //     var xMin = clusters.SelectMany(c => c.Select(c => c.position.x)).Min();
    //     var xMax = clusters.SelectMany(c => c.Select(c => c.position.x)).Max();
    //     var yMin = clusters.SelectMany(c => c.Select(c => c.position.y)).Min();
    //     var yMax = clusters.SelectMany(c => c.Select(c => c.position.y)).Max();
    //     
    //     edges.ForEach(edge =>
    //     {
    //         var x0 = clusters[edge.Item1].Select(e => e.position.x).Average();
    //         var y0 = clusters[edge.Item1].Select(e => e.position.y).Average();
    //         var x1 = clusters[edge.Item2].Select(e => e.position.x).Average();
    //         var y1 = clusters[edge.Item2].Select(e => e.position.y).Average();
    //
    //         var arrowAnnotation = new Annotation();
    //         arrowAnnotation.SetValue("x", x1 - Math.Abs((10 + edge.Item1) / (xMax - xMin)));
    //         arrowAnnotation.SetValue("y", y1 - Math.Abs((10 + edge.Item1) / (yMax - yMin)));
    //         arrowAnnotation.SetValue("xref", "x");
    //         arrowAnnotation.SetValue("yref", "y");
    //         arrowAnnotation.SetValue("axref", "x");
    //         arrowAnnotation.SetValue("ayref", "y");
    //         arrowAnnotation.SetValue("showarrow", true);
    //         arrowAnnotation.SetValue("arrowhead", 3);
    //         arrowAnnotation.SetValue("arrowcolor", Color.fromString("black"));
    //         arrowAnnotation.SetValue("ax", x0);
    //         arrowAnnotation.SetValue("ay", y0);
    //         arrowAnnotation.SetValue("arrowsize", 2);
    //         arrowAnnotation.SetValue("borderwidth", 5);
    //         
    //         annotations.Add(arrowAnnotation);
    //     });
    //     
    //     
    //     LinearAxis xAxis = new LinearAxis();
    //     xAxis.SetValue("visible", false);
    //     
    //     LinearAxis yAxis = new LinearAxis();
    //     yAxis.SetValue("visible", false);
    //     
    //     var layout = new Layout();
    //     layout.SetValue("xaxis", xAxis);
    //     layout.SetValue("yaxis", yAxis);
    //     layout.SetValue("title", "Recovered Architecture");
    //     layout.SetValue("plot_bgcolor", "#e5ecf6");
    //     layout.SetValue("annotations", annotations);
    //     
    //     var chart = Chart.Combine(scatterPlots);
    //     chart.WithLayout(layout);
    //
    //     chart.Show();
    // }
    

}

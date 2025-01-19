using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;

namespace ArchiteturalDiscovery.utils;

public static class PCAHelper
{
    public static double[] ReducePoint(double[] point)
    {
        double[,] rotationMatrix = {
            { 0.707, -0.707, 0 },
            { 0.408, 0.408, -0.816 }
        };

        // Transform the 3D point to 2D
        double[] point2D = new double[2];
        
        for (int i = 0; i < 2; i++)
        {
            point2D[i] = 0;
            for (int j = 0; j < 3; j++)
            {
                point2D[i] += rotationMatrix[i, j] * point[j];
            }
        }

        // Return the reduced point
        return point2D;
    }
}

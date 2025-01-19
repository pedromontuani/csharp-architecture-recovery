namespace ArchiteturalDiscovery.core;

    public class ElbowMethod
    {
        public int FindOptimalK(double[][] data, int maxK)
        {
            var distortions = new List<double>();
            var kmeans = new WeightedKmeans();

            for (int k = 1; k <= maxK; k++)
            {
                var clusters = kmeans.Clusterize(data, k);
                var distortion = CalculateDistortion(data, clusters);
                distortions.Add(distortion);
            }

            return GetElbowPoint(distortions);
        }

        private double CalculateDistortion(double[][] data, List<int>[] clusters)
        {
            double distortion = 0.0;

            foreach (var cluster in clusters)
            {
                var centroid = CalculateCentroid(data, cluster);
                distortion += cluster.Sum(index => EuclideanDistance(data[index], centroid));
            }

            return distortion;
        }

        private double[] CalculateCentroid(double[][] data, List<int> cluster)
        {
            int dimensions = data[0].Length;
            var centroid = new double[dimensions];

            foreach (var index in cluster)
            {
                for (int i = 0; i < dimensions; i++)
                {
                    centroid[i] += data[index][i];
                }
            }

            for (int i = 0; i < dimensions; i++)
            {
                centroid[i] /= cluster.Count;
            }

            return centroid;
        }

        private double EuclideanDistance(double[] point1, double[] point2)
        {
            double sum = 0.0;
            for (int i = 0; i < point1.Length; i++)
            {
                sum += Math.Pow(point1[i] - point2[i], 2);
            }
            return Math.Sqrt(sum);
        }

        private int GetElbowPoint(List<double> distortions)
        {
            int elbowPoint = 1;
            double maxDiff = 0.0;

            for (int i = 1; i < distortions.Count - 1; i++)
            {
                double diff = distortions[i - 1] - distortions[i];
                if (diff > maxDiff)
                {
                    maxDiff = diff;
                    elbowPoint = i + 1;
                }
            }

            return elbowPoint;
        }
    }
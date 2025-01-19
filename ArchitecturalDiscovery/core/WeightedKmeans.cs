using System;
using System.Collections.Generic;
using System.Linq;

namespace ArchiteturalDiscovery.core;

public class WeightedKmeans
    {
        public List<int>[] Clusterize(double[][] data, int k, double[] weights)
        {
            int n = data.Length;
            int dimensions = data[0].Length;
            var random = new Random();
            var centroids = new double[k][];
            for (int i = 0; i < k; i++)
            {
                centroids[i] = data[random.Next(n)];
            }

            var clusters = new List<int>[k];
            bool changed;
            do
            {
                changed = false;
                for (int i = 0; i < k; i++)
                {
                    clusters[i] = new List<int>();
                }

                for (int i = 0; i < n; i++)
                {
                    int closest = -1;
                    double minDistance = double.MaxValue;
                    for (int j = 0; j < k; j++)
                    {
                        double distance = EuclideanDistance(data[i], centroids[j], weights);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closest = j;
                        }
                    }

                    clusters[closest].Add(i);
                }

                for (int i = 0; i < k; i++)
                {
                    var newCentroid = new double[dimensions];
                    foreach (var index in clusters[i])
                    {
                        for (int j = 0; j < dimensions; j++)
                        {
                            newCentroid[j] += data[index][j];
                        }
                    }
                    for (int j = 0; j < dimensions; j++)
                    {
                        newCentroid[j] /= clusters[i].Count;
                    }
                    if (!newCentroid.SequenceEqual(centroids[i]))
                    {
                        centroids[i] = newCentroid;
                        changed = true;
                    }
                }
            } while (changed);

            return clusters;
        }

        private double EuclideanDistance(double[] point1, double[] point2, double[] weights)
        {
            double sum = 0.0;
            for (int i = 0; i < point1.Length; i++)
            {
                sum += Math.Pow(point1[i] - point2[i], 2) * weights[i];
            }
            return Math.Sqrt(sum);
        }
    }
namespace ArchiteturalDiscovery.utils;

public static class MinMaxScaler
{
    public static (double min, double max) Fit(double[] data)
    {
        double min = data.Min();
        double max = data.Max();
        return (min, max);
    }

    public static double[] Transform(double[] data, double min, double max, double rangeMin = 0, double rangeMax = 1)
    {
        return data.Select(x => (x - min) / (max - min) * (rangeMax - rangeMin) + rangeMin).ToArray();
    }
    
    public static double[] FitTransform(int[] data, double rangeMin = 0, double rangeMax = 1)
    {
        double[] values = data.ToList().ConvertAll<double>(i => (double)i).ToArray();
        
        var (min, max) = Fit(values);
        return Transform(values, min, max, rangeMin, rangeMax);
    }

    public static double[] FitTransform(double[] data, double rangeMin = 0, double rangeMax = 1)
    {
        var (min, max) = Fit(data);
        return Transform(data, min, max, rangeMin, rangeMax);
    }

    public static double InverseTransform(double scaledValue, double min, double max, double rangeMin = 0, double rangeMax = 1)
    {
        return (scaledValue - rangeMin) / (rangeMax - rangeMin) * (max - min) + min;
    }
}
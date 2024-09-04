using System.Globalization;

namespace GenerateData;

internal class Program
{
    static void Main()
    {
        const int nSamples = 10000; // number of samples to generate
        var rnd = new Random(42);

        // Data arrays
        var solderingTemp = new double[nSamples];
        var placementSpeed = new double[nSamples];
        var ambientTemp = new double[nSamples];
        var materialQuality = new double[nSamples];
        var humidity = new double[nSamples];  
        var numberOfDefects = new double[nSamples];
        var cycleTime = new double[nSamples];

        // Generate data
        for (var i = 0; i < nSamples; i++)
        {
            solderingTemp[i] = Normal(rnd, 310, 15);  
            placementSpeed[i] = Normal(rnd, 50, 10);  
            ambientTemp[i] = Normal(rnd, 22, 5); 
            materialQuality[i] = Normal(rnd, 0.8, 0.1);  
            humidity[i] = Normal(rnd, 50, 20);  
            
            numberOfDefects[i] = Math.Max(0, 5 - 0.01 * solderingTemp[i] + 0.05 * placementSpeed[i] +
                                             0.02 * Math.Pow(ambientTemp[i] - 22, 2) - 0.5 * materialQuality[i] +
                                             0.01 * humidity[i] + Normal(rnd, 0, 0.5));
            
            cycleTime[i] = Math.Max(0, 0.4 + 0.002 * solderingTemp[i] + 0.003 * placementSpeed[i] -
                                       0.001 * ambientTemp[i] + 0.002 * (1 - materialQuality[i]) +
                                       0.001 * humidity[i] + Normal(rnd, 0, 0.01));
        }

        // Write to CSV
        using (var sw = new StreamWriter("data.csv"))
        {
            sw.WriteLine("SolderingTemperature,PlacementSpeed,AmbientTemperature,MaterialQuality,Humidity,NumberOfDefects,CycleTime");
            for (var i = 0; i < nSamples; i++)
            {
                sw.WriteLine($"{solderingTemp[i].ToString(CultureInfo.InvariantCulture)}," +
                             $"{placementSpeed[i].ToString(CultureInfo.InvariantCulture)}," +
                             $"{ambientTemp[i].ToString(CultureInfo.InvariantCulture)}," +
                             $"{materialQuality[i].ToString(CultureInfo.InvariantCulture)}," +
                             $"{humidity[i].ToString(CultureInfo.InvariantCulture)}," +
                             $"{numberOfDefects[i].ToString(CultureInfo.InvariantCulture)}," +
                             $"{cycleTime[i].ToString(CultureInfo.InvariantCulture)}");
            }
        }

        Console.WriteLine("Data generated and saved to data.csv");
    }

    // Function to generate normally distributed random numbers
    static double Normal(Random rnd, double mean, double stddev)
    {
        // Using Box-Muller transform to generate two independent standard normal distributed random numbers
        var u1 = 1.0 - rnd.NextDouble(); // uniform(0,1] random doubles
        var u2 = 1.0 - rnd.NextDouble();
        var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return mean + stddev * randStdNormal;
    }
}
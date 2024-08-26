using System.Globalization;

class Program
{
    static void Main(string[] args)
    {
        int n_samples = 10000;  // Zvýšení počtu vzorků
        Random rnd = new Random(42);

        // Data arrays
        double[] solderingTemp = new double[n_samples];
        double[] placementSpeed = new double[n_samples];
        double[] ambientTemp = new double[n_samples];
        double[] materialQuality = new double[n_samples];  // Nová proměnná pro kvalitu materiálu
        double[] humidity = new double[n_samples];  // Nová proměnná pro vlhkost
        double[] numberOfDefects = new double[n_samples];
        double[] cycleTime = new double[n_samples];

        // Generate data
        for (int i = 0; i < n_samples; i++)
        {
            solderingTemp[i] = Normal(rnd, 310, 15);  // Větší variabilita teploty pájení
            placementSpeed[i] = Normal(rnd, 50, 10);  // Větší variabilita rychlosti osazování
            ambientTemp[i] = Normal(rnd, 22, 5);  // Větší variabilita teploty prostředí
            materialQuality[i] = Normal(rnd, 0.8, 0.1);  // Přidání kvality materiálu (0.0 - 1.0)
            humidity[i] = Normal(rnd, 50, 20);  // Přidání vlhkosti prostředí (0 - 100%)

            // Nelineární a složitější vztahy pro počet vad
            numberOfDefects[i] = Math.Max(0, 5 - 0.01 * solderingTemp[i] + 0.05 * placementSpeed[i] +
                                           0.02 * Math.Pow(ambientTemp[i] - 22, 2) - 0.5 * materialQuality[i] +
                                           0.01 * humidity[i] + Normal(rnd, 0, 0.5));

            // Nelineární vztahy pro čas cyklu
            cycleTime[i] = Math.Max(0, 0.4 + 0.002 * solderingTemp[i] + 0.003 * placementSpeed[i] -
                                    0.001 * ambientTemp[i] + 0.002 * (1 - materialQuality[i]) +
                                    0.001 * humidity[i] + Normal(rnd, 0, 0.01));
        }

        // Write to CSV
        using (StreamWriter sw = new StreamWriter("data.csv"))
        {
            sw.WriteLine("SolderingTemperature,PlacementSpeed,AmbientTemperature,MaterialQuality,Humidity,NumberOfDefects,CycleTime");
            for (int i = 0; i < n_samples; i++)
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
        double u1 = 1.0 - rnd.NextDouble(); // uniform(0,1] random doubles
        double u2 = 1.0 - rnd.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return mean + stddev * randStdNormal;
    }
}

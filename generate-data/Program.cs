using System;
using System.IO;

class Program
{
    static Random random = new Random();

    // Funkce pro generování dat
    static double GenerateTemperature()
    {
        return 240 + random.NextDouble() * 20;
    }

    static double GenerateHumidity()
    {
        return 20 + random.NextDouble() * 60;
    }

    static double GenerateMaterialQuality()
    {
        return random.NextDouble();
    }

    static double GenerateAssemblySpeed()
    {
        return 30 + random.NextDouble() * 50;
    }

    static double Sigmoid(double x)
    {
        return 1 / (1 + Math.Exp(-x));
    }

    // Nová upravená funkce pro výpočet pravděpodobnosti vady
    static double CalculateDefectProbability(double temp, double speed, double quality, double humidity)
    {
        return Sigmoid(-0.08 * Math.Pow(speed, 2) - 0.1 * quality + 0.03 * Math.Pow(humidity - 50, 2) + 0.01 * Math.Pow(temp - 250, 2));
    }

    // Funkce pro určení optimálního rozsahu rychlosti montáže
    static (double, double) CalculateOptimalSpeedRange(double temp, double quality, double humidity)
    {
        double minSpeed = -1;
        double maxSpeed = -1;
        bool foundOptimalSpeed = false;

        // Iterujeme přes různé rychlosti montáže a hledáme optimální rozsah
        for (double speed = 30; speed <= 80; speed += 0.1)
        {
            double defectProbability = CalculateDefectProbability(temp, speed, quality, humidity);

            if (defectProbability < 0.1)
            {
                if (!foundOptimalSpeed)
                {
                    minSpeed = speed;  // Nastavíme první nalezenou optimální rychlost
                    foundOptimalSpeed = true;
                }
                maxSpeed = speed;  // Aktualizujeme maximální rychlost
            }
            else if (foundOptimalSpeed)
            {
                // Pokud byla nalezena optimální rychlost, ale následující už není optimální, smyčku zastavíme
                break;
            }
        }

        return (minSpeed, maxSpeed);
    }

    static void GenerateDataset(int numSamples, string filePath, char delimiter = ';', bool skipDefective = false)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine($"Temperature{delimiter}AssemblySpeed{delimiter}MaterialQuality{delimiter}Humidity{delimiter}Defective{delimiter}MinAssemblySpeed{delimiter}MaxAssemblySpeed");

            for (int i = 0; i < numSamples; i++)
            {
                double temp = GenerateTemperature();
                double speed = GenerateAssemblySpeed();
                double quality = GenerateMaterialQuality();
                double humidity = GenerateHumidity();

                double defectProbability = CalculateDefectProbability(temp, speed, quality, humidity);
                int defective = random.NextDouble() < defectProbability ? 1 : 0;

                if (skipDefective && defective == 1)
                    continue;

                (double minSpeed, double maxSpeed) = CalculateOptimalSpeedRange(temp, quality, humidity);

                // Zápis do CSV
                writer.WriteLine($"{temp:F2}{delimiter}{speed:F2}{delimiter}{quality:F2}{delimiter}{humidity:F2}{delimiter}{defective}{delimiter}{minSpeed:F2}{delimiter}{maxSpeed:F2}");
            }
        }

        Console.WriteLine($"Dataset vygenerován a uložen do souboru {filePath}");
    }

    static void Main(string[] args)
    {
        int numSamples = 10000;
        string filePath = "dataset_with_new_formula.csv";

        GenerateDataset(numSamples, filePath);
    }
}

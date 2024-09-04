using Microsoft.ML.Data;

namespace MLAI01.Models;

public class ModelInput
{
    [LoadColumn(0)]
    public float SolderingTemperature { get; set; }

    [LoadColumn(1)]
    public float PlacementSpeed { get; set; }

    [LoadColumn(2)]
    public float AmbientTemperature { get; set; }

    [LoadColumn(3)]
    public float MaterialQuality { get; set; }

    [LoadColumn(4)]
    public float Humidity { get; set; }

    [LoadColumn(5)]
    public float NumberOfDefects { get; set; }

    [LoadColumn(6)]
    public float CycleTime { get; set; }
}


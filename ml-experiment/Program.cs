using Microsoft.ML;
using Microsoft.ML.Trainers.FastTree;
using MLAI01.Models;

internal class Program
{
    static void Main()
    {
        var context = new MLContext(seed: 1);

        // Load data
        if (!File.Exists(".\\data.csv"))
            throw new FileNotFoundException("data.csv not found");

        var data = context.Data.LoadFromTextFile<ModelInput>(".\\data.csv", hasHeader: true, separatorChar: ',');

        // Split data into training and test sets
        var splitData = context.Data.TrainTestSplit(data, testFraction: 0.2);
        var trainData = splitData.TrainSet;
        var testData = splitData.TestSet;

        // Prepare pipeline with additional transformations
        var pipeline = context.Transforms.Concatenate("Features", nameof(ModelInput.SolderingTemperature), nameof(ModelInput.PlacementSpeed), nameof(ModelInput.AmbientTemperature), nameof(ModelInput.MaterialQuality), nameof(ModelInput.Humidity))
                        .Append(context.Transforms.NormalizeMinMax("Features"));

        // Train Random Forest model with more trees and optimization
        var randomForestModel = pipeline
            .Append(context.Regression.Trainers.FastForest(
                new FastForestRegressionTrainer.Options
                {
                    LabelColumnName = nameof(ModelInput.NumberOfDefects),
                    FeatureColumnName = "Features",
                    NumberOfTrees = 2000,  // Increased number of trees
                    MaximumBinCountPerFeature = 512,
                    MinimumExampleCountPerLeaf = 5
                }))
            .Fit(trainData);

        // Train Gradient Boosting model (XGBoost) with further tuning
        var xgBoostModel = pipeline
            .Append(context.Regression.Trainers.FastTree(
                new FastTreeRegressionTrainer.Options
                {
                    LabelColumnName = nameof(ModelInput.NumberOfDefects),
                    FeatureColumnName = "Features",
                    NumberOfLeaves = 200,  // Larger trees
                    NumberOfTrees = 2000,  // More boosting rounds
                    LearningRate = 0.005,
                    MinimumExampleCountPerLeaf = 5,
                    Shrinkage = 0.8,
                }))
            .Fit(trainData);

        // Train a more advanced Neural Network model
        var neuralNetworkModel = pipeline
            .Append(context.Transforms.Concatenate("Features", nameof(ModelInput.SolderingTemperature), nameof(ModelInput.PlacementSpeed), nameof(ModelInput.AmbientTemperature), nameof(ModelInput.MaterialQuality), nameof(ModelInput.Humidity)))
            .Append(context.Transforms.NormalizeMinMax("Features"))
            .Append(context.Regression.Trainers.Sdca(
                labelColumnName: nameof(ModelInput.NumberOfDefects),
                featureColumnName: "Features"))
            .Fit(trainData);

        var randomForestCycleTimeModel = pipeline
            .Append(context.Regression.Trainers.FastForest(
                new FastForestRegressionTrainer.Options
                {
                    LabelColumnName = nameof(ModelInput.CycleTime),
                    FeatureColumnName = "Features",
                    NumberOfTrees = 2000,
                    MaximumBinCountPerFeature = 512,
                    MinimumExampleCountPerLeaf = 5
                }))
            .Fit(trainData);

        var xgBoostCycleTimeModel = pipeline
            .Append(context.Regression.Trainers.FastTree(
                new FastTreeRegressionTrainer.Options
                {
                    LabelColumnName = nameof(ModelInput.CycleTime),
                    FeatureColumnName = "Features",
                    NumberOfLeaves = 200,
                    NumberOfTrees = 2000,
                    LearningRate = 0.005,
                    MinimumExampleCountPerLeaf = 5,
                    Shrinkage = 0.8,
                }))
            .Fit(trainData);

        var neuralNetworkCycleTimeModel = pipeline
            .Append(context.Regression.Trainers.Sdca(
                labelColumnName: nameof(ModelInput.CycleTime),
                featureColumnName: "Features"))
            .Fit(trainData);

        // Evaluate models
        EvaluateModel(context, randomForestModel, testData, "Random Forest");
        EvaluateModel(context, xgBoostModel, testData, "XGBoost");
        EvaluateModel(context, neuralNetworkModel, testData, "Neural Network");

        // Evaluate models for CycleTime
        EvaluateModel2(context, randomForestCycleTimeModel, testData, "Random Forest (CycleTime)");
        EvaluateModel2(context, xgBoostCycleTimeModel, testData, "XGBoost (CycleTime)");
        EvaluateModel2(context, neuralNetworkCycleTimeModel, testData, "Neural Network (CycleTime)");
    }

    static void EvaluateModel(MLContext context, ITransformer model, IDataView testData, string modelName)
    {
        var predictions = model.Transform(testData);
        var metrics = context.Regression.Evaluate(predictions, labelColumnName: nameof(ModelInput.NumberOfDefects), scoreColumnName: nameof(ModelOutput.Score));

        Console.WriteLine($"Model: {modelName}");
        Console.WriteLine($"  RMSE: {metrics.RootMeanSquaredError}");
        Console.WriteLine($"  R2: {metrics.RSquared}");
    }
    static void EvaluateModel2(MLContext context, ITransformer model, IDataView testData, string modelName)
    {
        var predictions = model.Transform(testData);
        var metrics = context.Regression.Evaluate(predictions, labelColumnName: nameof(ModelInput.CycleTime), scoreColumnName: nameof(ModelOutput.Score));

        Console.WriteLine($"Model: {modelName}");
        Console.WriteLine($"  RMSE: {metrics.RootMeanSquaredError}");
        Console.WriteLine($"  R2: {metrics.RSquared}");
    }
}

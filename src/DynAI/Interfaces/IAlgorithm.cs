using Autodesk.DesignScript.Runtime;
using System;

namespace AI
{
    [IsVisibleInDynamoLibrary(false)]
    public interface IAlgorithm
    {
        // Metadata and State
        string Name { get; set; }
        AlgorithmType Type { get; }
        bool IsTrainingDataLoaded { get; }
        bool IsTrained { get; set; }

        // dataset
        dynamic LastResult { get; }
        dynamic LastTestValue { get; }

        // Type support
        Type PredictionType { get; }
        Type ResultType { get; }

        // ML Methods
        bool Learn();
        dynamic Predict(dynamic inputData);
    }

    [IsVisibleInDynamoLibrary(false)]
    public enum AlgorithmType
    {
        Regression,
        Classifier,
        Clustering,
        DecisionTree,
        VectorMachine
    }
}

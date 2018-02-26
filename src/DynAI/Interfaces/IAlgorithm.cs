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
        bool HasTrainingDataLoaded { get; }
        bool IsTrained { get; set; }

        // dataset
        object LastResult { get; }
        object LastTestValue { get; }

        // Type support
        Type PredictionType { get; }
        Type ResultType { get; }

        // ML Methods
        void Learn();
        object Predict(object inputData);
    }

    [IsVisibleInDynamoLibrary(false)]
    public enum AlgorithmType
    {
        Supervised,
        Unsupervised
    }
}

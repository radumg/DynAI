using Autodesk.DesignScript.Runtime;

namespace AI.Algorithms
{
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

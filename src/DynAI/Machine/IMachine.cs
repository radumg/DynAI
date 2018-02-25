using System;
using Autodesk.DesignScript.Runtime;

namespace AI.Machine
{
    /// <summary>
    /// The generic capabilities of a machine, used for machine learning
    /// </summary>
    [IsVisibleInDynamoLibrary(false)]
    public interface IMachine
    {
        #region Metadata

        string Name { get; set; }
        string GUID { get; }
        string Description { get; }

        // State
        /// <summary>
        /// Indicates whether the machine has been trained already or not.
        /// Required to be TRUE for Predict() to work.
        /// </summary>
        bool IsTrained { get; }

        /// <summary>
        /// The last value that was used as input for prediction, usually known as the test value.
        /// </summary>
        object LastTestValue { get; }

        /// <summary>
        /// The result of the prediction.
        /// </summary>
        dynamic Result { get; }

        #endregion

        #region ML

        /// <summary>
        /// This object is the learning/predicting algorithm and will be used in the Learn and Predict methods.
        /// </summary>
        IAlgorithm Algorithm { get; set; }

        /// <summary>
        /// Enables a machine to learn from training data.
        /// </summary>
        /// <returns>The input machine, now trained.</returns>
        IMachine Learn();

        /// <summary>
        /// Enables a trained machine to provide a prediction from an input value.
        /// </summary>
        /// <param name="inputData">Input for the prediction</param>
        /// <returns>The predicted value</returns>
        dynamic Predict(dynamic inputData);

        #endregion
    }

    /// <summary>
    /// This provides an abstraction so that different machines can use different formats for training data.
    /// </summary>
    [IsVisibleInDynamoLibrary(false)]
    public interface ITrainingData
    {
        /// <summary>
        /// Fetches the training data.
        /// </summary>
        /// <returns>The training dataset.</returns>
        double[] GetTrainingData();

        /// <summary>
        /// Allows us to set the training data.
        /// </summary>
        /// <returns>True if the operation succeeded, false otherwise.</returns>
        bool SetTrainingData();
    }

    [IsVisibleInDynamoLibrary(false)]
    public interface IInputData<T>
    {
        /// <summary>
        /// Retrieves the input data.
        /// </summary>
        /// <returns>The data.</returns>
        T GetInputData();

        /// <summary>
        /// Sets the input data for the machine to the specified values.
        /// </summary>
        /// <returns>True if the operation succeeded, false otherwise.</returns>
        bool SetInputData();
    }
}

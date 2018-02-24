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
        bool Learned { get; set; }

        #endregion

        #region ML

        Type Algorithm { get; set; }

        /// <summary>
        /// Enables a machine to learn from training data.
        /// </summary>
        /// <param name="machine">The machine that will learn.</param>
        /// <param name="trainingData">The dataset used to train the machine.</param>
        /// <returns>The input machine, now trained.</returns>
        IMachine Learn(object trainingData);

        /// <summary>
        /// Enables a trained machine to provide a prediction from an input value.
        /// </summary>
        /// <param name="machine">The machine that will predict, needs to be trained already.</param>
        /// <param name="inputData">Input for the prediction</param>
        /// <returns>The predicted value</returns>
        object Predict(object inputData);

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

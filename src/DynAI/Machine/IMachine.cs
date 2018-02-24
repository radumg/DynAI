using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Machine
{
    /// <summary>
    /// The generic capabilities of a machine, used for machine learning
    /// </summary>
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
        IMachine Learn(IMachine machine, object trainingData);

        /// <summary>
        /// Enables a trained machine to provide a prediction from an input value.
        /// </summary>
        /// <param name="machine">The machine that will predict, needs to be trained already.</param>
        /// <param name="inputData">Input for the prediction</param>
        /// <returns>The predicted value</returns>
        object Predict(IMachine machine, object inputData);

        #endregion

        #region Serialisation

        /// <summary>
        /// Serialises a machine to JSON and saves to specified file on disk.
        /// </summary>
        /// <param name="machine">The machine to serialise.</param>
        /// <param name="filePath">The destination file on disk.</param>
        /// <returns>True if operation succeeded, false otherwise.</returns>
        bool SaveModel(IMachine machine, string filePath);

        /// <summary>
        /// Loads a trained model from a JSON file.
        /// </summary>
        /// <param name="filePath">The JSON file to load from.</param>
        /// <returns>The trained machine. Throws Exception if deserialisation did not succeed.</returns>
        IMachine LoadModel(string filePath);

        #endregion
    }

    /// <summary>
    /// This provides an abstraction so that different machines can use different formats for training data.
    /// </summary>
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

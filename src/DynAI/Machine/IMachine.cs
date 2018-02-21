using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI
{
    /// <summary>
    /// The generic capabilities of a machine, used for machine learning
    /// </summary>
    public interface IMachine
    {
        /// <summary>
        /// Enables a machine to learn from training data.
        /// </summary>
        /// <param name="machine">The machine that will learn.</param>
        /// <returns>The input machine, now trained.</returns>
        IMachine Learn(IMachine machine);

        /// <summary>
        /// Enables a machine to provide a prediction from an input value.
        /// </summary>
        /// <param name="machine">The machine that will predict.</param>
        /// <param name="inputData">Input for the prediction</param>
        /// <returns>The predicted value</returns>
        double Predict(IMachine machine, double inputData);

        IMachine SaveModel(IMachine machine);

        IMachine LoadModel(IMachine machine);
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

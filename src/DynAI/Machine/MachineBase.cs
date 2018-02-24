using System;
using AI.Utilities;
using Autodesk.DesignScript.Runtime;

namespace AI.Machine
{
    //[IsVisibleInDynamoLibrary(false)]
    public class Machine : IMachine
    {
        #region Metadata

        // metadata
        public string Name { get; set; }
        public string GUID { get; set; }
        public string Description { get; set; }
        public bool Trained { get; set; }

        // input data
        public IInputData<double> Inputs { get; set; }
        public virtual double[] Outputs { get; set; }
        public virtual double TestValue { get; set; }
        //ITrainingData TrainingData { get; set; }

        // Learning & predicting
        public object Algorithm { get; set; }

        // Result
        public double Result { get; set; }

        #endregion

        #region ML

        /// The ML Learn and Predict functions should be abstract as each algorithm needs to have its own implementation.
        /// This would however break serialisation in this base class, so they're virtual and explictly not implemented.

        /// <summary>
        /// Enables a machine to learn from training data.
        /// </summary>
        /// <param name="machine">The machine that will learn.</param>
        /// <param name="trainingData">The dataset used to train the machine.</param>
        /// <returns>The input machine, now trained.</returns>
        public virtual IMachine Learn(object trainingData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Enables a trained machine to provide a prediction from an input value.
        /// </summary>
        /// <param name="machine">The machine that will predict, needs to be trained already.</param>
        /// <param name="inputData">Input for the prediction</param>
        /// <returns>The predicted value.</returns>
        public virtual object Predict(object inputData)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IO

        /// <summary>
        /// Serialises a machine to JSON and saves to specified file on disk.
        /// </summary>
        /// <param name="machine">The machine to serialise.</param>
        /// <param name="filePath">The destination file on disk.</param>
        /// <returns>True if operation succeeded, false otherwise.</returns>
        public virtual bool SaveModel(IMachine machine, string filePath)
        {
            return Json.ToJsonFile(machine, filePath);
        }

        /// <summary>
        /// Loads a trained model from a JSON file.
        /// </summary>
        /// <param name="filePath">The JSON file to load from.</param>
        /// <returns>The trained machine. Throws Exception if deserialisation did not succeed.</returns>
        public virtual IMachine LoadModel(string filePath)
        {
            return Json.FromJsonFileTo<Machine>(filePath);
        }

        #endregion
    }
}

using System;
using AI.Utilities;
using Autodesk.DesignScript.Runtime;

namespace AI.Machine
{
    [IsVisibleInDynamoLibrary(false)]
    public class MachineBase : IMachine
    {
        #region Metadata

        // metadata
        public string Name { get; set; }
        public string GUID { get; set; }
        public string Description { get; set; }
        public bool Learned { get; set; }

        // input data
        public Type Algorithm { get; set; }
        public IInputData<double> Inputs { get; set; }
        public virtual double[] Outputs { get; set; }
        public virtual double TestValue { get; set; }

        // training data
        ITrainingData TrainingData { get; set; }

        // Result
        public double Result { get; set; }

        #endregion

        #region ML

        /// <summary>
        /// Enables a machine to learn from training data.
        /// </summary>
        /// <param name="machine">The machine that will learn.</param>
        /// <param name="trainingData">The dataset used to train the machine.</param>
        /// <returns>The input machine, now trained.</returns>
        public virtual IMachine Learn(IMachine machine, object trainingData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Enables a trained machine to provide a prediction from an input value.
        /// </summary>
        /// <param name="machine">The machine that will predict, needs to be trained already.</param>
        /// <param name="inputData">Input for the prediction</param>
        /// <returns>The predicted value.</returns>
        public virtual object Predict(IMachine machine, object inputData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serialises a machine to JSON and saves to specified file on disk.
        /// </summary>
        /// <param name="machine">The machine to serialise.</param>
        /// <param name="filePath">The destination file on disk.</param>
        /// <returns>True if operation succeeded, false otherwise.</returns>
        public bool SaveModel(IMachine machine, string filePath)
        {
            return Json.ToJsonFile(machine, filePath);
        }

        /// <summary>
        /// Loads a trained model from a JSON file.
        /// </summary>
        /// <param name="filePath">The JSON file to load from.</param>
        /// <returns>The trained machine. Throws Exception if deserialisation did not succeed.</returns>
        public IMachine LoadModel(string filePath)
        {
            return Json.FromJsonFileTo<MachineBase>(filePath);
        }

        #endregion
    }
}

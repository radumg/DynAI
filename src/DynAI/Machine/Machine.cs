using System;
using AI.Utilities;
using Autodesk.DesignScript.Runtime;

namespace AI
{
    /// <summary>
    /// Machines are entities that can leverage different algorithms to learn from training data and then predict new values.
    /// </summary>
    public class Machine : IMachine
    {
        #region Metadata

        /// <summary>
        /// The name of the machine. Useful when storing the results to disk, does not affect results in any way.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The globally unique ID of the machine. Useful when storing the results to disk, does not affect results in any way.
        /// </summary>
        public string GUID { get; set; }

        /// <summary>
        /// The description of the machine. Useful when storing the results to disk, does not affect results in any way.
        /// </summary>
        public string Description { get; set; }

        #endregion

        #region Algorithm

        /// <summary>
        /// The algorithm that will be used for learning, hidden in Dynamo due to interface issues.
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        public IAlgorithm Algorithm { get; set; }

        /// <summary>
        /// Indicates whether training data was successfully loaded into the machine and that it has learned from it.
        /// </summary>
        public bool IsTrained => Algorithm.IsTrainingDataLoaded && Algorithm.IsTrained;

        /// <summary>
        /// The result supplied by the prediction algorithm used.
        /// </summary>
        public dynamic Result => Algorithm.LastResult;

        /// <summary>
        /// The datatype of the result, useful for data validation.
        /// </summary>
        public string ResultDataType => Algorithm.ResultType.ToString();

        /// <summary>
        /// The last value used as input for Prediction.
        /// </summary>
        public object LastTestValue => Algorithm.LastTestValue;

        // Type helpers
        private Type AlgorithmClassType;

        /// <summary>
        /// Get the algorithm used by the machine for learning.
        /// </summary>
        /// <returns>The algorithm used by the machine for learning, as an object.</returns>
        public dynamic GetAlgorithm()
        {
            return Convert.ChangeType(this.Algorithm, Algorithm.GetType());
        }

        /// <summary>
        /// Sets the algorithm to the specified object, throws if argument is null or doesn't implement IAlgorithm.
        /// </summary>
        /// <param name="algorithm">The object to set algorithm to.</param>
        private void SetAlgorithm(object algorithm)
        {
            // Check algorithm implements the IAlgorithm interface and it's not null.
            // This is because the learning and prediction methods accept IAlgorithm objects only.
            if (algorithm == null) throw new ArgumentNullException("The algorithm cannot be null.");
            if (!(algorithm is IAlgorithm))
                throw new Exception(
                    "The supplied algorithm is not valid" + Environment.NewLine +
                    "Expected : something implementing IAlgorithm" + Environment.NewLine +
                    "Got : type " + algorithm.GetType().ToString() + " and interfaces " + algorithm.GetType().GetInterfaces().ToString()
                    );
            // ok to cast if we reached this point.
            var algo = (IAlgorithm)algorithm;

            // check we already have training data in the algorithm.
            if (!algo.IsTrainingDataLoaded) throw new ArgumentException("The specified algorithm has no training data.");

            // record the algorithm used as an object and its type, required in GetAlgorithm method.
            this.Algorithm = algo;
            this.AlgorithmClassType = algorithm.GetType();
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Build a new machine, to learn from training data and predict outcomes.
        /// </summary>
        /// <param name="algorithm">The algorithm to use for learning. Has to contain training data already.</param>
        /// <param name="name">(optional) Specify a name for this machine.</param>
        /// <param name="description">(optional) specify a description for this machine.</param>
        public Machine(object algorithm, string name = null, string description = null)
        {
            // record the algorithm used as an object and its type, required in GetAlgorithm method.
            SetAlgorithm(algorithm);

            // default values
            GUID = Guid.NewGuid().ToString();
            Name = string.IsNullOrWhiteSpace(name) ? GUID : name;
            Description = string.IsNullOrWhiteSpace(description) ? string.Empty : description;
        }

        /// <summary>
        /// Public parameterless constructor needed for deserialisation.
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        public Machine() { }

        #endregion

        #region ML

        /// <summary>
        /// Tells a machine to learn from its training data.
        /// </summary>
        /// <returns>The input machine, now trained.</returns>
        public Machine Learn()
        {
            this.Algorithm.Learn();
            return this;
        }

        /// <summary>
        /// Enables a trained machine to provide a prediction from an input value.
        /// Note that each algorithm expects a different data type as input.
        /// </summary>
        /// <param name="testData">The value(s) to use as input in the prediction.</param>
        /// <returns>The predicted value.</returns>
        public dynamic Predict(dynamic testData)
        {
            // don't predict if we haven't learned the model yet
            if (!this.IsTrained) throw new Exception("Cannot predict before the algorithm has learned.");

            // check we haven't already predicted for this input and use cache if so
            if (object.Equals(testData,this.LastTestValue) && this.IsTrained == true) return this.Result;

            return Algorithm.Predict(testData);
        }

        #endregion

        #region IO

        /// <summary>
        /// Serialises a machine to JSON and saves to specified file on disk.
        /// </summary>
        /// <param name="filePath">The destination file on disk.</param>
        /// <returns>True if operation succeeded, false otherwise.</returns>
        public bool Save(string filePath)
        {
            return Json.ToJsonFile(this, filePath);
        }

        /// <summary>
        /// Loads a trained model from a JSON file.
        /// </summary>
        /// <param name="filePath">The JSON file to load from.</param>
        /// <returns>The trained machine. Throws Exception if deserialisation did not succeed.</returns>
        public static Machine Load(string filePath)
        {
            return Json.FromJsonFileTo<Machine>(filePath);
        }

        #endregion
    }
}

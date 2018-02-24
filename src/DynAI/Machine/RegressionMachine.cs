using System;
using AI.Utilities;
using Autodesk.DesignScript.Runtime;
using Newtonsoft.Json;
using System.IO;
using Accord.Statistics.Models.Regression.Linear;
using System.Linq;
using Accord.Statistics.Models.Regression;

namespace AI.Machine
{
    public class RegressionMachine : Machine
    {
        #region Metadata

        // input data
        public SimpleLinearRegression Regression { get; private set; }
        public OrdinaryLeastSquares ols;

        // override Inputs
        public new double[] Inputs { get; set; }

        #endregion

        #region Constructors

        public RegressionMachine(object algorithm, string name = null, string description = null)
        {
            
            var type = algorithm.GetType();
            /*
            if (
                type != AI.MachineLearning.Regression.SimpleLinearRegression.GetType() ||
                type != AI.MachineLearning.Regression.MultipleLinearRegression.GetType() ||
                type != AI.MachineLearning.Regression.MultivariateLinearRegression.GetType() ||
                type != AI.MachineLearning.Regression.OrdinaryLeastSquares.GetType() ||
                type != AI.MachineLearning.Regression.LogisticRegression.GetType()
                ) throw new ArgumentException("The algorithm supplied is not valid"+Environment.NewLine + "Supplied type : "+type.ToString() + "Expected : " + AI.MachineLearning.Regression.SimpleLinearRegression.GetType().ToString());
                */
            GUID = Guid.NewGuid().ToString();
            Name = string.IsNullOrWhiteSpace(name) ? GUID : name;
            Description = string.IsNullOrWhiteSpace(description) ? string.Empty : description;
            Algorithm = type;
        }

        /// <summary>
        /// Public parameterless constructor used for deserialisation. Do not use for anything else.
        /// </summary>
        public RegressionMachine()
        {

        }

        #endregion

        #region ML

        /// <summary>
        /// Enables a machine to learn from training data.
        /// </summary>
        /// <param name="machine">The machine that will learn.</param>
        /// <param name="trainingData">The dataset used to train the machine.</param>
        /// <returns>The input machine, now trained.</returns>
        public IMachine Learn(object trainingData)
        {
            if (trainingData.GetType() != typeof(double[])) throw new ArgumentException("Training data needs to be an array of numbers (double)");

            // valid data, so let's save it
            Inputs = (double[]) trainingData;

            try
            {
                this.Regression = this.ols.Learn((double[])trainingData, this.Outputs);
                this.Trained = true;
            }
            catch (Exception)
            {
                throw new Exception("Learning failed.");
            }

            return this;
        }

        /// <summary>
        /// Enables a trained machine to provide a prediction from an input value.
        /// </summary>
        /// <param name="machine">The machine that will predict, needs to be trained already.</param>
        /// <param name="inputData">Input for the prediction</param>
        /// <returns>The predicted value.</returns>
        public object Predict(object inputData)
        {
            // check type first
            double input;
            try
            {
                input = (double)inputData;
            }
            catch (Exception)
            {
                throw new ArgumentException("Input data needs to be a number (double)");
            }

            // don't predict if we haven't learned the model yet
            if (this.Trained != true) throw new Exception("Cannot predict before the machine has learned.");

            // check we haven't already predicted for this input
            if (input == this.TestValue && this.Trained == true) return this.Result;

            // predict
            this.TestValue = input;
            try
            {
                this.Result = this.Regression.Transform(this.TestValue);
            }
            catch (Exception)
            {
                throw new Exception("Could not perform regression.");
            }

            return this.Result;
        }

        #endregion

        #region IO

        /// <summary>
        /// Serialises a machine to JSON and saves to specified file on disk.
        /// </summary>
        /// <param name="machine">The machine to serialise.</param>
        /// <param name="filePath">The destination file on disk.</param>
        /// <returns>True if operation succeeded, false otherwise.</returns>
        public static bool SaveModel(RegressionMachine machine, string filePath)
        {
            return Json.ToJsonFile(machine, filePath);
        }

        /// <summary>
        /// Loads a trained model from a JSON file.
        /// </summary>
        /// <param name="filePath">The JSON file to load from.</param>
        /// <returns>The trained machine. Throws Exception if deserialisation did not succeed.</returns>
        public static RegressionMachine LoadModel(string filePath)
        {
            //var json = File.ReadAllText(filePath);
            //return JsonConvert.DeserializeObject(json, this.GetType(), AI.Utilities.Json.Settings);
            return Json.FromJsonFileTo<RegressionMachine>(filePath);
        }

        #endregion
    }
}

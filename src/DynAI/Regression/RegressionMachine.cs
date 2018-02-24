using Accord.Statistics.Models.Regression.Linear;
using AI.Machine;
using System;
using System.Linq;

namespace AI.MachineLearning
{
    /// <summary>
    /// Machine for use with Regression ML algorithms
    /// </summary>
    public class RegressionMachine : MachineBase
    {
        #region public properties
        // dataset overrides
        // none needed

        // regression
        public Accord.Statistics.Models.Regression.Linear.SimpleLinearRegression Regression { get; private set; }
        public OrdinaryLeastSquares ols;

        #endregion

        #region Constructors

        public RegressionMachine(object algorithm, string name = null, string description = null)
        {
            var type = algorithm.GetType();
            if (
                !type.GetInterfaces().Contains(typeof(ILinearRegression)) ||
                type != typeof(SimpleLinearRegression) ||
                type != typeof(SimpleLinearRegression) ||
                type != typeof(SimpleLinearRegression)
                ) throw new ArgumentException("The algorithm supplied is not valid.");
            GUID = Guid.NewGuid().ToString();
            Name = string.IsNullOrWhiteSpace(name) ? GUID : name;
            Description = string.IsNullOrWhiteSpace(description) ? string.Empty : description;
        }

        #endregion

        #region IMachine methods

        /// <summary>
        /// Use the object's inputs and outputs to learn the model of the linear regression, using OrdinaryLeastSquares
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="trainingData"></param>
        /// <returns>The trained machine.</returns>
        public override IMachine Learn(IMachine machine, object trainingData)
        {
            if (trainingData.GetType() != typeof(double[])) throw new ArgumentException("Training data needs to be an array of numbers (double)");

            try
            {
                this.Regression = this.ols.Learn((double[])trainingData, this.Outputs);
                this.Learned = true;
            }
            catch (Exception)
            {
                throw new Exception("Learning failed.");
            }

            return this;
        }

        public override object Predict(IMachine machine, object inputData)
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
            if (this.Learned != true) throw new Exception("Cannot predict before the machine has learned.");

            // check we haven't already predicted for this input
            if (input == this.TestValue && this.Learned == true) return this.Result;

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
    }
}

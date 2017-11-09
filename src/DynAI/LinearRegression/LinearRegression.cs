using Accord.Statistics.Models.Regression.Linear;
using AI.Machine;
using System;

namespace AI.MachineLearning.LinearRegression
{
    /// <summary>
    /// Class providing support for linear regression ML algorithms
    /// </summary>
    public class SimpleLinearRegression : MachineBase, IMachine
    {
        #region public properties
        // dataset overrides
        // none needed

        // regression
        public Accord.Statistics.Models.Regression.Linear.SimpleLinearRegression Regression { get; private set; }
        public OrdinaryLeastSquares ols;

        #endregion

        #region IMachine methods

        /// <summary>
        /// Use the object's inputs and outputs to learn the model of the linear regression, using OrdinaryLeastSquares
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="trainingData"></param>
        /// <returns>The trained machine.</returns>
        public override IMachine Learn(IMachine machine, ITrainingData trainingData)
        {
            this.Regression = this.ols.Learn(trainingData.GetTrainingData(), this.Outputs);
            this.Learned = true;

            return this;
        }

        public override double Predict(IMachine machine, double inputData)
        {
            // don't predict if we haven't learned the model yet
            if (this.Learned != true) throw new Exception("Cannot predict before the machine has learned.");

            // check we haven't already predicted for this input
            if (inputData == this.TestValue && this.Learned == true) return this.Result;

            // predict
            this.TestValue = inputData;
            this.Result = this.Regression.Transform(this.TestValue);

            return this.Result;
        }
        #endregion
    }
}

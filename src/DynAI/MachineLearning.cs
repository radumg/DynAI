using Accord.Math.Random;
using Accord.Statistics.Models.Regression.Linear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynAI.MachineLearning
{
    #region Linear Regression

    /// <summary>
    /// Class providing support for linear regression ML algorithms
    /// </summary>
    public class LinearRegression
    {
        #region public properties
        // dataset
        public double[] inputs;
        public double[] outputs;
        public double testValue;

        // regression
        public SimpleLinearRegression regression;
        public OrdinaryLeastSquares ols;
        public bool learned = false;
        public double result;
        #endregion

        /// <summary>
        /// Constructs a new LinearRegression machine.
        /// </summary>
        public LinearRegression(List<double> inputList, List<double> outputList)
        {
            // validation
            if (inputList == null || outputList == null) throw new ArgumentNullException("Neither the input list nor the output list can be NULL");

            // initialise seed value
            Generator.Seed = new Random().Next();

            // process input and output lists into arrays
            inputs = inputList.ToArray();
            outputs = outputList.ToArray();

            // set up linear regression using OLS
            regression = new SimpleLinearRegression();
            ols = new OrdinaryLeastSquares();

            // nulls
            testValue = new double();
            result = new double();
        }

        /// <summary>
        /// Use the object's inputs and outputs to learn the model of the linear regression, using OrdinaryLeastSquares
        /// </summary>
        public LinearRegression Learn()
        {
            regression = this.ols.Learn(inputs, outputs);
            learned = true;

            return this;
        }

        /// <summary>
        /// Using the learned model, predict an output for the specified input
        /// </summary>
        /// <param name="test">The value to use as input for the prediction</param>
        /// <returns>The predicted value</returns>
        public double Predict(double test)
        {
            // don't predict if we haven't learned the model yet
            if (this.learned != true) throw new Exception("Cannot predict before the machine has learned.");

            // check we haven't already predicted for this input
            if (test == this.testValue && this.learned == true) return this.result;

            // predict
            this.testValue = test;
            this.result = this.regression.Transform(this.testValue);

            return this.result;
        }
    }
    #endregion

    #region Classifiers

    #endregion

    #region Helpers

    #endregion

}

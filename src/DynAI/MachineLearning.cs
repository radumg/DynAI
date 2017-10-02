using Accord.MachineLearning.Bayes;
using Accord.Math;
using Accord.Math.Random;
using Accord.Statistics.Filters;
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
        public double[] inputs { get; private set; }
        public double[] outputs { get; private set; }
        public double testValue { get; private set; }

        // regression
        public SimpleLinearRegression regression { get; private set; }
        public OrdinaryLeastSquares ols;

        // state & result
        public bool learned { get; private set; }
        public double result { get; private set; }
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
            this.learned = false;
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
    /// <summary>
    /// Class providing support for Naive Bayes classification machines.
    /// </summary>
    public class NaiveBayes
    {
        #region public properties
        // dataset
        public string[][] dataset { get; private set; }
        public string[] columns { get; private set; }
        public string outputColumn { get; private set; }
        public int[][] inputs;
        public int[] outputs;

        // classifier
        public Accord.MachineLearning.Bayes.NaiveBayes classifier;
        public NaiveBayesLearning learner;
        public Codification codebook { get; private set; }

        // state & result
        public bool learned { get; private set; }
        public string[] testValue { get; private set; }
        public string result { get; private set; }
        public double[] probs { get; private set; }
        #endregion

        /// <summary>
        /// Constructs a new NaiveBayes classification machine.
        /// </summary>
        public NaiveBayes(string[][] data, List<string> columnList, string outputColumn)
        {
            // validation
            if (data == null || columnList == null || outputColumn==null) throw new ArgumentNullException("Neither the input list nor the column list can be NULL");

            // initialise seed value
            Generator.Seed = new Random().Next();

            // process input and output lists into arrays
            this.dataset = data;
            this.columns = columnList.ToArray();
            this.outputColumn = outputColumn;

            // Create a new codification codebook to
            // convert strings into discrete symbols
            this.codebook = new Codification(columns, this.dataset);

            // Extract input and output pairs to train
            int[][] symbols = this.codebook.Transform(this.dataset);
            this.inputs = symbols.Get(null, 0, -1); // Gets all rows, from 0 to the last (but not the last)
            this.outputs = symbols.GetColumn(-1);     // Gets only the last column

            // Create a new Naive Bayes learning
            this.learner = new NaiveBayesLearning();

            // nulls
            testValue = null;
            result = null;
            probs = null;
            this.learned = false;
        }

        /// <summary>
        /// Use the object's inputs and outputs to learn the model of the linear regression, using OrdinaryLeastSquares
        /// </summary>
        public NaiveBayes Learn()
        {
            this.classifier = this.learner.Learn(inputs, outputs);
            this.learned = true;

            return this;
        }

        /// <summary>
        /// Using the learned model, predict an output for the specified input
        /// </summary>
        /// <param name="test">The value to use as input for the prediction</param>
        /// <returns>The predicted value</returns>
        public string Predict(string[] test)
        {
            // don't predict if we haven't learned the model yet
            if (this.learned != true) throw new Exception("Cannot predict before the machine has learned.");

            // check we haven't already predicted for this input
            if (test == this.testValue && this.learned == true) return this.result;

            // predict
            // First encode the test instance
            int[] instance = this.codebook.Transform(test);

            // Let us obtain the numeric output that represents the answer
            int codeword = this.classifier.Decide(instance);

            // Now let us convert the numeric output to an actual answer
            this.result = this.codebook.Revert(this.outputColumn, codeword);

            // We can also extract the probabilities for each possible answer
            this.probs = this.classifier.Probabilities(instance);

            return this.result;
        }
    }
    #endregion

    #region Helpers

    #endregion

}

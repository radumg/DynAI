using Accord.Math.Random;
using Accord.Statistics.Models.Regression.Linear;
using AI.Machine;
using Autodesk.DesignScript.Runtime;
using System;
using System.Collections.Generic;

namespace AI.MachineLearning.Regression
{
    public class SimpleLinearRegression : IAlgorithm
    {
        #region Interface Properties

        // Metadata
        public string Name { get; set; }
        public AlgorithmType Type { get; }
        public bool HasTrainingDataLoaded => HasTrainingData();
        public bool IsTrained { get; set; }

        // Type support
        public Type PredictionType { get; }
        public Type ResultType { get; private set; }

        // dataset
        public object LastTestValue => TestValue;
        public object LastResult => Result;

        #endregion

        #region Custom properties

        private double[] Inputs;
        private double[] Outputs;
        private double? TestValue;
        private double? Result;

        // Learner & predictor - these are not part of the interface
        private Accord.Statistics.Models.Regression.Linear.SimpleLinearRegression regression;
        private OrdinaryLeastSquares ols;

        #endregion

        #region Constructor

        /// <summary>
        /// Construct a new Simple Linear Regression algorithm, using the specified training data.
        /// </summary>
        /// <param name="inputList">Use inputList as rows with equal numbers of featurs, which used for learning.</param>
        /// <param name="outputList">Use outputList as the rows that define the result column for each</param>
        public SimpleLinearRegression(List<double> inputList, List<double> outputList)
        {
            Name = "Simple Linear Regression";
            Type = AlgorithmType.Supervised;
            IsTrained = false;
            PredictionType = typeof(double);
            ResultType = typeof(double);
            Inputs = null;
            Outputs = null;
            TestValue = null;
            Result = null;

            // initialise seed value for Accord framework
            Generator.Seed = new Random().Next();

            // Process training data
            LoadTrainingData(inputList, outputList);

            // set up linear regression using OrdinaryLeastSquares
            regression = new Accord.Statistics.Models.Regression.Linear.SimpleLinearRegression();
            ols = new OrdinaryLeastSquares();
        }

        #endregion

        #region ML

        [IsVisibleInDynamoLibrary(false)]
        public void Learn()
        {
            try
            {
                regression = this.ols.Learn(Inputs, Outputs);
                IsTrained = true;
            }
            catch (Exception e)
            {
                throw new Exception(
                    "Failed to learn using specified training data." + Environment.NewLine +
                    "Inner exception : " + e.Message
                    );
            }

           // return this as IAlgorithm;
        }

        [IsVisibleInDynamoLibrary(false)]
        public object Predict(object inputData)
        {
            // parse input to required type - throws error if not possible
            var input = ConvertToValidInputType(inputData);

            // predict & cache test value
            this.TestValue = input;
            this.Result = this.regression.Transform(input);

            return this.Result;
        }

        #endregion

        #region Utils

        private double ConvertToValidInputType(object inputData)
        {
            if (inputData.GetType() == PredictionType) return (double)inputData;

            // if not exact same type, try parsing as double
            var parsed = new double();

            if (!double.TryParse(inputData.ToString(), out parsed)){
                throw new Exception(
                    "Input data type is not valid and conversion failed." + Environment.NewLine +
                    "Supplied : " + inputData.GetType().ToString() + Environment.NewLine +
                    "Expected : " + PredictionType.ToString()); }
            else return parsed;
        }

        private void LoadTrainingData(List<double> inputList, List<double> outputList)
        {
            // validation
            if (inputList == null || outputList == null) throw new ArgumentNullException("Neither the input list nor the output list can be NULL");

            // process input and output lists into arrays
            Inputs = inputList.ToArray();
            Outputs = outputList.ToArray();
        }

        private bool HasTrainingData()
        {
            if (this.Inputs == null) return false;
            if (this.Inputs.Length == 0) return false;
            return true;
        }

        #endregion
    }
}

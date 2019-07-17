using Accord.Math.Random;
using Accord.Statistics.Models.Regression.Linear;
using Autodesk.DesignScript.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AI.Algorithms.Regression
{
    /// <summary>
    ///  In linear regression, the model specification is that the dependent variable, y is a linear combination of the parameters (but need not be linear in the independent variables). 
    /// </summary>
    public class MultivariateLinearRegression : IAlgorithm
    {
        #region Interface Properties

        // Metadata
        public string Name { get; set; }
        public AlgorithmType Type { get; }
        public bool IsTrainingDataLoaded => HasTrainingData();
        public bool IsTrained { get; set; }

        // Type support
        public Type PredictionType { get; }
        public Type ResultType { get; private set; }

        // dataset
        public object LastTestValue => TestValue;
        public object LastResult => Result;

        #endregion

        #region Custom properties

        private double[][] Inputs;
        private double[][] Outputs;
        private double[][] TestValue;
        private double[][] Result;

        // Learner & predictor - these are not part of the interface
        public Accord.Statistics.Models.Regression.Linear.MultivariateLinearRegression Regression { get; set; }
        private OrdinaryLeastSquares ols;

        #endregion

        #region Constructor

        /// <summary>
        /// Construct a new Simple Linear Regression algorithm, using the specified training data.
        /// </summary>
        /// <param name="inputList">Use inputList as rows with equal numbers of featurs, which used for learning.</param>
        /// <param name="outputList">Use outputList as the rows that define the result column for each</param>
        public MultivariateLinearRegression(List<List<double>> inputList, List<List<double>> outputList) : this()
        {
            // Process training data
            LoadTrainingData(inputList, outputList);

            // set up linear regression using OrdinaryLeastSquares
            Regression = new Accord.Statistics.Models.Regression.Linear.MultivariateLinearRegression();
            ols = new OrdinaryLeastSquares();
        }

        [IsVisibleInDynamoLibrary(false)]
        public MultivariateLinearRegression()
        {
            Name = "Multivariate Linear Regression";
            Type = AlgorithmType.Regression;
            IsTrained = false;
            PredictionType = typeof(double[][]);
            ResultType = typeof(double[][]);
            Inputs = null;
            Outputs = null;
            TestValue = null;
            Result = null;

            // initialise seed value for Accord framework
            Generator.Seed = new Random().Next();
        }
        #endregion

        #region ML

        public bool Learn()
        {
            try
            {
                Regression = this.ols.Learn(Inputs, Outputs);
                IsTrained = true;
                return true;
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
        public dynamic Predict([ArbitraryDimensionArrayImport] dynamic inputData)
        {
            // parse input to required type - throws error if not possible
            double[][] input = this.ConvertToValidInputType(inputData);

            return this.Predict(input);
        }

        private double[][] Predict(double[][] inputData)
        {
            // predict & cache test value
            this.TestValue = inputData;
            this.Result = this.Regression.Transform(inputData);

            return this.Result;
        }

        #endregion

        #region Utils

        private double[][] ConvertToValidInputType(object inputData)
        {
            if (inputData.GetType() == PredictionType) return (double[][])inputData;

            // if not exact same type, try parsing as double
            if (!AI.Utils.Types.IsList(inputData)) throw new ArgumentException("Type cannot be converted");
            var inputMatrix = (IList)inputData;
            var outputMatrix = new double[inputMatrix.Count][];

            for (int i = 0; i < inputMatrix.Count; i++)
            {
                var currentItem = inputMatrix[i];
                if (!Utils.Types.IsList(currentItem))
                {
                    throw new Exception("Expected a matrix of elements, but an inner list was a single object.");
                }
                var innerList = (IList)currentItem;
                var innerOutput = new double[innerList.Count];

                for (int j = 0; j < innerList.Count; j++)
                {
                    double parsed;
                    if (!double.TryParse(innerList[i].ToString(), out parsed))
                        throw new Exception(
                            "Input data type is not valid and conversion failed." + Environment.NewLine +
                            "Supplied : " + inputData.GetType().ToString() + Environment.NewLine +
                            "Expected : " + PredictionType.ToString());

                    innerOutput[i] = parsed;
                }
                outputMatrix[i] = innerOutput;

            }
            return outputMatrix;
        }

        private void LoadTrainingData(List<List<double>> inputList, List<List<double>> outputList)
        {
            // validation
            if (inputList == null || outputList == null) throw new ArgumentNullException("Neither the input list nor the output list can be NULL");

            // process input and output lists into multi-dimensional arrays
            Inputs = inputList.Select(x => x.ToArray()).ToArray();
            Outputs = outputList.Select(x => x.ToArray()).ToArray();
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

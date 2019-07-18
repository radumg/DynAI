using Accord.Math.Random;
using Accord.Statistics.Filters;
using Accord.Statistics.Models.Regression.Linear;
using Autodesk.DesignScript.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AI.Algorithms.Regression
{
    /// <summary>
    ///  Multiple linear regression. 
    /// </summary>
    public class MultipleLinearRegression :IAlgorithm
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
        public object LastTestValue => testValue;
        public object LastResult => result;

        #endregion

        #region Custom properties

        private double[][] inputs;
        private double[] outputs;
        private double[] testValue;
        private double? result;

        // Learner & predictor - these are not part of the interface
        public Accord.Statistics.Models.Regression.Linear.MultipleLinearRegression Regression { get; set; }
        private Codification codebook;
        private bool codify;
        private string CodifyColumn = string.Empty;
        private OrdinaryLeastSquares ols;

        #endregion

        #region Constructor

        /// <summary>
        /// Construct a new Simple Linear Regression algorithm, using the specified training data.
        /// </summary>
        /// <param name="inputList">Use inputList as rows with equal numbers of featurs, which used for learning.</param>
        /// <param name="outputList">Use outputList as the rows that define the result column for each</param>
        public static MultipleLinearRegression WithTrainingData(List<List<double>> inputList, List<double> outputList, string codifyColumn=null)
        {
            var regression = new MultipleLinearRegression();

            // use the codify column if specified
            if (!string.IsNullOrWhiteSpace(codifyColumn))
            {
                regression.codify = true;
                regression.CodifyColumn = codifyColumn;
            }

            // Process training data
            regression.LoadTrainingData(inputList, outputList);

            return regression;
        }

        [IsVisibleInDynamoLibrary(false)]
        public MultipleLinearRegression()
        {
            Name = "Multiple Linear Regression";
            Type = AlgorithmType.Regression;
            IsTrained = false;
            PredictionType = typeof(double[]);
            ResultType = typeof(double);
            inputs = null;
            outputs = null;
            testValue = null;
            result = null;

            // initialise seed value for Accord framework
            Generator.Seed = new Random().Next();

            // set up linear regression using OrdinaryLeastSquares
            Regression = new Accord.Statistics.Models.Regression.Linear.MultipleLinearRegression();
            ols = new OrdinaryLeastSquares() { UseIntercept = true };
        }
        #endregion

        #region ML

        public bool Learn()
        {
            if (codify)
            {
                // ADD CODE FOR CODIFICATION SUPPORT HERE.
            }
            try
            {
                Regression = this.ols.Learn(inputs, outputs);
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

        public dynamic Predict([ArbitraryDimensionArrayImport] dynamic inputData)
        {
            // parse input to required type - throws error if not possible
            double[] input = ConvertToValidInputType(inputData);

            return this.Predict(input);
        }

        private dynamic Predict(double[] input)
        {
            // predict & cache test value
            this.testValue = input;
            this.result = this.Regression.Transform(input);

            return this.result;
        }

        #endregion

        #region Utils

        private double[] ConvertToValidInputType(object inputData)
        {
            if (inputData.GetType() == PredictionType) return (double[])inputData;

            // if not exact same type, try parsing as double
            if (!AI.Utils.Types.IsList(inputData)) throw new ArgumentException("Type cannot be converted");
            var inputList = (IList)inputData;
            var outputList = new double[inputList.Count];

            for (int i = 0; i < inputList.Count; i++)
            {
                double parsed;
                if (!double.TryParse(inputList[i].ToString(), out parsed))
                {
                    throw new Exception(
                        "Input data type is not valid and conversion failed." + Environment.NewLine +
                        "Supplied : " + inputData.GetType().ToString() + Environment.NewLine +
                        "Expected : " + PredictionType.ToString());
                }
                else outputList[i] = parsed;
            }
            return outputList;
        }


        private void LoadTrainingData(List<List<double>> inputList, List<double> outputList)
        {
            // validation
            if (inputList == null || outputList == null) throw new ArgumentNullException("Neither the input list nor the output list can be NULL");

            // process input and output lists into arrays
            inputs = inputList.Select(x => x.ToArray()).ToArray();
            outputs = outputList.ToArray();

            if (codify)
            {
                /*
                
                ADD CODE FOR CODIFICATION SUPPORT HERE.    


                // Create a new codification codebook to convert strings into discrete symbols
                this.codebook = new Codification(CodifyColumn, codebook.Columns.);

                // Extract input and output pairs to train
                int[][] symbols = this.codebook.Transform(CodifyColumn,);
                this.codifiedDataset = symbols.Get(null, 0, -1); // Gets all rows, from 0 to the last (but not the last)
                this.codifiedOutputs = symbols.GetColumn(-1);     // Gets only the last column
                */
                
            }
        }

        private bool HasTrainingData()
        {
            if (this.inputs == null) return false;
            if (this.inputs.Length == 0) return false;
            return true;
        }

        #endregion
    }
}

using Accord.MachineLearning.Bayes;
using Accord.Math;
using Accord.Math.Random;
using Accord.Statistics.Filters;
using Accord.Statistics.Models.Regression.Linear;
using Autodesk.DesignScript.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AI.Algorithms.Classifier
{
    /// <summary>
    ///  In linear regression, the model specification is that the dependent variable, y is a linear combination of the parameters (but need not be linear in the independent variables). 
    /// </summary>
    public class NaiveBayesClassifier : IAlgorithm
    {
        #region Interface Properties

        // Metadata
        public string Name { get; set; }
        public AlgorithmType Type { get; }

        [IsVisibleInDynamoLibrary(false)]
        public bool IsTrainingDataLoaded => HasTrainingData();

        [IsVisibleInDynamoLibrary(false)]
        public bool IsTrained { get; set; }

        // Type support
        public Type PredictionType { get; }
        public Type ResultType { get; private set; }

        // dataset
        [IsVisibleInDynamoLibrary(false)]
        public object LastTestValue => TestValue;

        [IsVisibleInDynamoLibrary(false)]
        public object LastResult => Result;

        #endregion

        #region Custom properties

        public string[][] Dataset;
        public string[] Outputs;
        private string[] TestValue;
        private string Result;
        private int[][] codifiedDataset;
        private int[] codifiedOutputs;

        public string OutputColumn { get; private set; }

        // Learner & predictor - these are not part of the interface
        private NaiveBayes classifier;
        private NaiveBayesLearning learner;
        private Codification codebook { get; set; }
        public double[] Probabilities { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Construct a new Simple Linear Regression algorithm, using the specified training data.
        /// </summary>
        /// <param name="inputMatrix">The matrix of inputs to use as features.</param>
        /// <param name="columnList">The names to use for the dataset's columns.</param>
        /// <param name="outputColumn">The name of the column that codifies the result of the learning.</param>
        public NaiveBayesClassifier(string[][] inputMatrix, List<string> columnList, string outputColumn):this()
        {
            // Process training data
            LoadTrainingData(inputMatrix, columnList, outputColumn);

            // Create a new Naive Bayes learner
            this.learner = new NaiveBayesLearning();
        }

        [IsVisibleInDynamoLibrary(false)]
        public NaiveBayesClassifier()
        {
            Name = "Naive Bayes Classifier";
            Type = AlgorithmType.Classifier;
            IsTrained = false;
            PredictionType = typeof(string[]);
            ResultType = typeof(string);
            Dataset = null;
            Outputs = null;
            TestValue = null;
            Result = null;
            Probabilities = null;

            // initialise seed value for Accord framework
            Generator.Seed = new Random().Next();
        }
        #endregion

        #region ML

        public bool Learn()
        {
            try
            {
                this.classifier = this.learner.Learn(this.codifiedDataset, this.codifiedOutputs);
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
        }

        public dynamic Predict([ArbitraryDimensionArrayImport] dynamic inputData)
        {
            // parse input to required type - throws error if not possible
            string[] input = ConvertToValidInputType(inputData);

            return this.Predict(input);
        }

        private string Predict(string[] inputData)
        {
            // parse input to required type - throws error if not possible
            this.TestValue = inputData;

            // First encode the test instance
            int[] instance = this.codebook.Transform(inputData);

            // Let us obtain the numeric output that represents the answer
            int codeword = this.classifier.Decide(instance);

            // Now let us convert the numeric output to an actual answer
            this.Result = this.codebook.Revert(this.OutputColumn, codeword);

            // We can also extract the probabilities for each possible answer
            this.Probabilities = this.classifier.Probabilities(instance);

            return this.Result;
        }

        #endregion

        #region Utils

        private string[] ConvertToValidInputType(object inputData)
        {
            if (inputData.GetType() == PredictionType) return (string[])inputData;

            // if not exact same type, try parsing as double
            if (!Utils.Types.IsList(inputData)) throw new ArgumentException("Expected a list or array of objects.");

            var inputList = (IList)inputData;
            var outputList = new string[inputList.Count];

            for (int i = 0; i < inputList.Count; i++)
            {
                outputList[i] = inputList[i]?.ToString();
            }
            return outputList;
        }


        private void LoadTrainingData(string[][] dataset, List<string> outputList, string outputColumn)
        {
            // validation
            if (dataset == null || outputList == null || string.IsNullOrWhiteSpace(outputColumn)) throw new ArgumentNullException("Neither the input list, output list or output column can be NULL");

            // process input and output lists into arrays
            this.Dataset = dataset;
            this.Outputs = outputList.ToArray();
            this.OutputColumn = outputColumn;

            // Create a new codification codebook to convert strings into discrete symbols
            this.codebook = new Codification(Outputs, this.Dataset);

            // Extract input and output pairs to train
            int[][] symbols = this.codebook.Transform(this.Dataset);
            this.codifiedDataset = symbols.Get(null, 0, -1); // Gets all rows, from 0 to the last (but not the last)
            this.codifiedOutputs = symbols.GetColumn(-1);     // Gets only the last column
        }

        private bool HasTrainingData()
        {
            if (this.Dataset == null || this.Dataset.Length == 0) return false;
            if (this.Outputs == null || this.Outputs.Length == 0) return false;
            if (string.IsNullOrWhiteSpace(OutputColumn)) return false;

            return true;
        }

        #endregion
    }
}

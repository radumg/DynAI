using Accord.Math.Random;
using Accord.Statistics.Models.Regression.Linear;
using Autodesk.DesignScript.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AI.Algorithms.Clustering
{
    /// <summary>
    ///  In linear regression, the model specification is that the dependent variable, y is a linear combination of the parameters (but need not be linear in the independent variables). 
    /// </summary>
    public class KMeans : IAlgorithm
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
        private double[] TestValue;
        private int[] Result;

        // Learner & predictor - these are not part of the interface but do get serialised
        public Accord.MachineLearning.KMeans Clustering { get; set; }

        private Accord.MachineLearning.KMeansClusterCollection cluster;

        public int Clusters { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Construct a new Simple Linear Regression algorithm, using the specified training data.
        /// </summary>
        /// <param name="inputList">Use inputList as rows with equal numbers of featurs, which used for learning.</param>
        /// <param name="outputList">Use outputList as the rows that define the result column for each</param>
        public KMeans(List<List<double>> inputList, int clusters):this()
        {
            this.Clusters = clusters;

            // Process training data
            LoadTrainingData(inputList);

            // set up K-Means clustering
            Clustering = new Accord.MachineLearning.KMeans(this.Clusters);
        }

        [IsVisibleInDynamoLibrary(false)]
        public KMeans()
        {
            Name = "K-Means Clustering";
            Type = AlgorithmType.Clustering;
            IsTrained = false;
            PredictionType = typeof(double[]);
            ResultType = typeof(int[]);
            Inputs = null;
            TestValue = null;
            Result = null;

            // initialise seed value for Accord framework
            Generator.Seed = new Random().Next();
        }

        #endregion

        #region ML

        [IsVisibleInDynamoLibrary(false)]
        public bool Learn()
        {
            this.IsTrained = false;
            try
            {
                cluster = this.Clustering.Learn(Inputs);
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

        [IsVisibleInDynamoLibrary(false)]
        public dynamic Predict([ArbitraryDimensionArrayImport] dynamic ignored)
        {
            this.Result = this.cluster.Decide(Inputs);

            return this.Result;
        }

        #endregion

        #region Utils

        private void LoadTrainingData(List<List<double>> inputList)
        {
            // validation
            if (inputList == null) throw new ArgumentNullException("Neither the input list nor the output list can be NULL");

            // process input and output lists into multi-dimensional arrays
            Inputs = inputList.Select(x => x.ToArray()).ToArray();
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

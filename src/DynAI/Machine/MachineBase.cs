using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Machine
{
    public abstract class MachineBase : IMachine
    {
        #region public properties
        // input data
        public IInputData<double> Inputs { get; set; }
        public virtual double[] Outputs { get; set; }
        public virtual double TestValue { get; set; }

        // training data
        ITrainingData TrainingData { get; set; }

        // state & result
        public bool Learned { get; set; }
        public double Result { get; set; }
        #endregion

        #region IMachine methods
        public abstract IMachine Learn(IMachine machine, ITrainingData trainingData);

        public abstract double Predict(IMachine machine, double inputData);

        public IMachine Learn(IMachine machine)
        {
            throw new NotImplementedException();
        }

        public IMachine SaveModel(IMachine machine)
        {
            throw new NotImplementedException();
        }

        public IMachine LoadModel(IMachine machine)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

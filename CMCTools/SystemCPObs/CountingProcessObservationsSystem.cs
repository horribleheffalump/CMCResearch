using CMCTools;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemCPObs
{
    public class CountingProessObservationsSystem
    {
        public ControllableMarkovChain State;
        public ControllableCountingProcess Observation;
        public SuboptimalFilter Filter;

        public CountingProessObservationsSystem(int _N, double _t0, double _T, int _X0, double _h, Func<double, Matrix<double>> _A, Func<double, Vector<double>> _c, bool _saveHistory = false)
        {
            State = new ControllableMarkovChain(_N, _t0, _T, _X0, _h, (t, u) => _A(t), _saveHistory);
            Observation = new ControllableCountingProcess(_t0, _T, 0, _h, (t, u) => _c(t)[State.X] * u, _saveHistory);
            Filter = new SuboptimalFilter(_N, _t0, _T, _h, (t) => _A(t), (t) => _c(t), _saveHistory);
        }

        public double Step(double u)
        {
            State.Step(u);
            Observation.Step(u);
            Filter.Step(u, Observation.N);
            return State.t;
        }

        public void SaveAll(string MCFileName, string ObsFileName, string FilterFileName, int every = 1)
        {
            if (State.Jumps.Count > 0)
            {
                State.SaveTrajectory(MCFileName);
                Observation.SaveTrajectory(ObsFileName);
                Filter.SaveTrajectory(FilterFileName, every);
            }
        }
        //public void GenerateTrajectory(Func<double, Vector<double>> U)
        //{
        //    while (State.t < State.T)
        //    {
        //        State.Step(U(State.t)[State.X]);
        //        Observation.Step(U(State.t)[State.X]);
        //        Filter.Step(U(State.t)[State.X], Observation.N);
        //        //Crit.Step(State.t, State.X, U(State.t), Observation.N);
        //    }
        //}
    }
}

using CMCTools;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace SystemJointObs
{
    public class JointObservationsSystem
    {
        public ControllableMarkovChain State;
        public ControllableCountingProcess[] CPObservations;
        public ControllableContinuousProcess ContObservations;

        public JointObservationsSystem(int _N, double _t0, double _T, int _X0, double _h, Func<double, double, Matrix<double>> _A, Func<double, double, Vector<double>>[] _c, Func<double, double, Vector<double>> _R, Func<double, double, Vector<double>> _G, bool _saveHistory = false)
        {
            State = new ControllableMarkovChain(_N, _t0, _T, _X0, _h, (t, u) => _A(t,u), _saveHistory);
            CPObservations = new ControllableCountingProcess[_c.Length];
            for (int i = 0; i < _c.Length; i++)
            {
                CPObservations[i] = new ControllableCountingProcess(_t0, _T, 0, _h, C_i(i, _c), _saveHistory);
            }
            ContObservations = new ControllableContinuousProcess(_t0, _T, 0.0, _h, (t, u) => _R(t, u)[State.X], (t, u) => _G(t, u)[State.X], _saveHistory);
        }

        public virtual Func<double, double, double> C_i(int i, Func<double, double, Vector<double>>[] _c) // so that we use the proper i
        {
            return (t, u) => _c[i](t, u)[State.X];
        }

        public virtual double Step(double u)
        {
            State.Step(u);
            for (int i = 0; i < CPObservations.Length; i++)
            {
                CPObservations[i].Step(u);
            }
            ContObservations.Step(u);
            return State.t;
        }

        public void SaveAll(string MCFileName, string CPObsFileNameTemplate, string ContObsFileName, int every = 1)
        {
            if (State.Jumps.Count > 0)
            {
                State.SaveTrajectory(MCFileName);
                for (int i = 0; i < CPObservations.Length; i++)
                {
                    CPObservations[i].SaveTrajectory(CPObsFileNameTemplate.Replace("{num}", i.ToString()));
                }
                ContObservations.SaveTrajectory(ContObsFileName, every);
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

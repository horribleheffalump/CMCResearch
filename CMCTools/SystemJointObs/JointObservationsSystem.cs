using CMC;
using CMC.Filters;
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
        public Dictionary<String, BaseFilter> Filters;

        public JointObservationsSystem(int _N, double _t0, double _T, int _X0, double _h, Func<double, double, Matrix<double>> _A, Func<double, double, Vector<double>>[] _c, Func<double, double, Vector<double>> _R, Func<double, double, Vector<double>> _G, int _SaveEvery = 0, double _hObs = 0, bool _calculateFilters = false)
        {
            State = new ControllableMarkovChain(_N, _t0, _T, _X0, _h, (t, u) => _A(t, u), _SaveEvery > 0);
            CPObservations = new ControllableCountingProcess[_c.Length];
            for (int i = 0; i < _c.Length; i++)
            {
                CPObservations[i] = new ControllableCountingProcess(_t0, _T, 0, _h, C_i(i, _c), _SaveEvery > 0);
            }
            ContObservations = new ControllableContinuousProcess(_t0, _T, 0.0, _h, (t, u) => _R(t, u)[State.X], (t, u) => _G(t, u)[State.X], _SaveEvery, _hObs);

            if (_calculateFilters)
            {
                Filters = new Dictionary<string, BaseFilter>();
                Filters.Add("Dummy", new DummyFilter(_N, _t0, _T, _h, _A, _c, null, C_i(0, _c), C_i(1, _c), _R, _G, () => State.X, _SaveEvery));
                Filters.Add("Discrete", new FilterDiscrete(_N, _t0, _T, _h, _A, _c, null, _SaveEvery));
                Filters.Add("DiscreteContinuous", new FilterDiscreteContinuous(_N, _t0, _T, _h, _A, _c, null, _R, _G, _SaveEvery, _hObs));
                Filters.Add("DiscreteContinuousGaussian", new FilterDiscreteContinuousGaussian(_N, _t0, _T, _h, _A, _c, null, _R, _G, _SaveEvery, _hObs));
            }
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
            if (Filters != null)
            {
                foreach (var f in Filters)
                {
                    f.Value.Step(u, CPObservations.Select(co => co.dN).ToArray(), ContObservations.dx_thin);
                }
            }
            return State.t;
        }

        public void SaveAll(string MCFileName, string CPObsFileNameTemplate, string ContObsFileName, string FilterFileNameTemplate)
        {
            if (State.Jumps.Count > 0)
            {
                State.SaveTrajectory(MCFileName);
                for (int i = 0; i < CPObservations.Length; i++)
                {
                    CPObservations[i].SaveTrajectory(CPObsFileNameTemplate.Replace("{num}", i.ToString()));
                }
                ContObservations.SaveTrajectory(ContObsFileName);
                if (Filters != null)
                {
                    foreach (var f in Filters)
                    {
                        f.Value.SaveTrajectory(FilterFileNameTemplate.Replace("{name}", f.Key));
                    }
                }
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

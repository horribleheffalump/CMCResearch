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
    public class JointObservationsSystemSimultaniousJumps : JointObservationsSystem
    {
        /// <summary>
        /// This class inherits the JointObservationSystem.
        /// The difference is that here the jumps of the observed counting processes may occure at the same time as the jumps
        /// of the Markov chain.
        /// An additional parameter List<SimultaneousJumpsIntencity>[] _I is introduced to specify the intencities of the simultaneous jumps.
        /// _I[i] is the list of the intencities of the simultaneous jumps of the Markov chain and the i-th observable counting process.
        /// Each element of the list specifies the MC transision (from -> to) which may result in simultaneous CP jump and the corresponding intencity.
        /// </summary>

        public List<SimultaneousJumpsIntencity>[] SimultaneousJumpsIntencities;

        public JointObservationsSystemSimultaniousJumps(int _N, double _t0, double _T, int _X0, double _h, Func<double, double, Matrix<double>> _A, Func<double, double, Vector<double>>[] _c, List<SimultaneousJumpsIntencity>[] _I, Func<double, double, Vector<double>> _R, Func<double, double, Vector<double>> _G, int _saveEvery = 0, double _hObs = 0) :
            base(_N, _t0, _T, _X0, _h, _A, _c, _R, _G, _saveEvery, _hObs)
        {
            if (_c.Length != _I.Length)
            {
                throw (new ArgumentException("The number of Simultaneous Jump Intencities List should correspond to the number of observed counting processes"));
            }
            for (int i = 0; i < _c.Length; i++)
            {
                CPObservations[i] = new ControllableCountingProcess(_t0, _T, 0, _h, C_i(i, _c, _I), _saveEvery > 0);
            }
            SimultaneousJumpsIntencities = _I;
            Filters = new Dictionary<string, BaseFilter>();
            Filters.Add("Dummy", new DummyFilter(_N, _t0, _T, _h, _A, _c, _I, C_i(0, _c, _I), C_i(1, _c, _I), _R, _G, () => State.X, _saveEvery));
            //Filters.Add("Discrete", new FilterDiscrete(_N, _t0, _T, _h, _A, _c, _I, _saveEvery));
            ////Filters.Add("DiscreteIndependent", new FilterDiscrete(_N, _t0, _T, _h, _A, _c, null, _saveEvery));
            ////Filters.Add("DiscreteMeasureChange", new FilterDiscreteMeasureChange(_N, _t0, _T, _h, _A, _c, _saveEvery));
            //Filters.Add("DiscreteContinuous", new FilterDiscreteContinuous(_N, _t0, _T, _h, _A, _c, _I, _R, _G, _saveEvery, _hObs));
            //Filters.Add("DiscreteContinuousGaussian", new FilterDiscreteContinuousGaussian(_N, _t0, _T, _h, _A, _c, _I, _R, _G, _saveEvery, _hObs));
        }

        public Func<double, double, double> C_i(int i, Func<double, double, Vector<double>>[] _c, List<SimultaneousJumpsIntencity>[] _I) // so that we use the proper i
        {
            Func<double, double, double> SummarizedIntencity = (t, u) =>
            {
                double result = 0;
                foreach (SimultaneousJumpsIntencity _i in _I[i].Where(e => e.From == State.X))
                {
                    result += _i.Intencity(t, u);
                }
                return result;
            };
            return (t, u) => _c[i](t, u)[State.X] - SummarizedIntencity(t, u); // the intencity of the independent jumps of a counting process should be lessend by the intencities of the simultaneous jumps
        }

        public override double Step(double u)
        {
            int x_ = State.X;
            State.Step(u);
            int x = State.X;
            for (int i = 0; i < CPObservations.Length; i++)
            {
                CPObservations[i].Step(u); // the independent jumps
                if (CPObservations[i].dN == 0 && x_ != x) // if there was no independent jump, simulate the simultaneous jumps caused by Markov chain state change
                {
                    foreach (var sim in SimultaneousJumpsIntencities[i].Where(s => s.From == x_ && s.To == x))
                    {
                        double p = sim.Intencity(State.t, u) / State.TransitionRateMatrix(State.t, u)[x_, x]; // Bayes theorem: P(A|B) = P(AB)/P(B); A - MC jump, B - CPO jump. P(AB) =  SimultaneousJumpsIntencitiy(x_ -> x) * h, P(B) = TransitionRateMatrix(x_ -> x) * h
                        int nojump = FiniteDiscreteDistribution.Sample(Vector<double>.Build.DenseOfArray(new[] { p, 1 - p }));
                        if (nojump == 0)
                        {
                            CPObservations[i].Jump();
                            State.Transit(x);
                        }
                    }
                }
            }
            ContObservations.Step(u);
            foreach (var f in Filters)
            {
                f.Value.Step(u, CPObservations.Select(co => co.dN).ToArray(), ContObservations.dx_thin);
            }
            return State.t;
        }

    }
}
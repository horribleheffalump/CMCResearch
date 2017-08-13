using CMCTools;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemJointObs
{
    class SimultaneousJumpsIntencity
    {
        public int From;
        public int To;
        public Func<double, double, double> Intencity;


        SimultaneousJumpsIntencity(int _from, int _to, Func<double, double, double> _intencity)
        {
            From = _from;
            To = _to;
            Intencity = _intencity;
        }
    }

    class JointObservationsSystemSimultaniousJumps : JointObservationsSystem
    {
        /// <summary>
        /// This class inherits the JointObservationSystem.
        /// The difference is that here the jumps of the observed counting processes may occure at the same time as the jumps
        /// of the Markov chain.
        /// An additional parameter List<SimultaneousJumpsIntencity>[] _I is introduced to specify the intencities of the simultaneous jumps.
        /// _I[i] is the list of the intencities of the simultaneous jumps of the Markov chain and the i-th observable counting process.
        /// Each element of the list specifies the MC transision (from -> to) which may result in simultaneous CP jump and the corresponding intencity.
        /// </summary>

        public JointObservationsSystemSimultaniousJumps(int _N, double _t0, double _T, int _X0, double _h, Func<double, double, Matrix<double>> _A, Func<double, double, Vector<double>>[] _c, List<SimultaneousJumpsIntencity>[] _I, Func<double, double, Vector<double>> _R, Func<double, double, Vector<double>> _G, bool _saveHistory = false) :
            base(_N, _t0, _T, _X0, _h, _A, _c, _R, _G, _saveHistory)
        {
            if (_c.Length != _I.Length)
            {
                throw (new ArgumentException("The number of Simultaneous Jump Intencities List should correspond to the number of observed counting processes"));
            }
            for (int i = 0; i < _c.Length; i++)
            {
                Func<double, double, double> SummarizedIntencity = (t, u) =>
                {
                    double result = 0;
                    foreach (SimultaneousJumpsIntencity _i in _I[i])
                    {
                        result += _i.Intencity(t, u);
                    }
                    return result;
                };
                CPObservations[i].Intensity = (t, u) => C_i(i, _c)(t, u) - SummarizedIntencity(t,u); // the intencity of the independent jumps of a counting process should be lessend by the intencities of the simultaneous jumps
            }
        }

        public override double Step(double u)
        {
            State.Step(u);
            for (int i = 0; i < CPObservations.Length; i++)
            {
                CPObservations[i].Step(u); // the independent jumps
  
            }
            ContObservations.Step(u);
            return State.t;
        }

    }
}
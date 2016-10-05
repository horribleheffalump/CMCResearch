using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCTools
{
    public class SuboptimalFilter
    {
        public int N;           // state space
        Func<double, Matrix<double>> A;
        Func<double, Vector<double>> c;
        public double t;        // current time
        public double t0 = 0;   // observation start time
        public double T;        // observation end time
        Vector<double> pi0;      // initial state
        public Vector<double> pi;      // current estimate
        public double h = 1e-3; // discretization step
        public int Obs = 0;     // current observation

        public double Nu = 0;   // RHS martingale

        public bool SaveHistory;
        public List<Estimate> estimates; // estimates list

        public SuboptimalFilter(int _N, double _t0, double _T, double _h, Func<double, Matrix<double>> _A, Func<double, Vector<double>> _c, bool _SaveHistory = false)
        {
            N = _N;
            t0 = _t0;
            t = t0;
            T = _T;
            h = _h;

            A = _A;
            c = _c;

            //pi0 = Vector<double>.Build.Dense(N, 1.0 / N); //!!!!!! TODO  !!!!!!!!!!!!!!!!
            pi0 = Vector<double>.Build.Dense(N, 0.0); pi0[0] = 1.0; //!!!!!! TODO  !!!!!!!!!!!!!!!!
            pi = pi0;

            estimates = new List<Estimate>();
            SaveHistory = _SaveHistory;
            if (SaveHistory)
            {
                estimates.Add(new Estimate(t0, pi0, 0));
            }
        }

        public Vector<double> Step(double u, int _Obs)
        {
            var a = A(t);
            t += h;

            var dNu = _Obs - Obs - (c(t).ToRowMatrix() * pi)[0] * u * h;
            Obs = _Obs;

            var Gamma = (1.0 / (c(t).ToRowMatrix() * pi)[0]) * c(t).PointwiseMultiply(pi) - pi;

            pi = pi + a * pi * h + Gamma * dNu;
            pi = pi.Normalize(1.0);

            var estimate = new Estimate(t, pi, u);
            if (SaveHistory)
                estimates.Add(estimate);

            return pi;
        }

        public void SaveTrajectory(string path, int every)
        {
            using (System.IO.StreamWriter outputfile = new System.IO.StreamWriter(path))
            {
                foreach (Estimate e in estimates.Where((x, i) => i % every == 0).OrderBy(s => s.t))
                {
                    outputfile.WriteLine(e.ToString());
                }
                outputfile.Close();
            }
        }

    }
}

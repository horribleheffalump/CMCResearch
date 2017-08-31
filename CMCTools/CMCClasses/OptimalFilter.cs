using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCTools
{
    /// <summary>
    /// Optimal filter for controllable Markov chain with scalar continuous observations
    /// and multiple counting process observations with possible simultaneous jumps and MC transitions
    /// </summary>
    public class OptimalFilter
    {
        public int N;           // state space
        Func<double, double, Matrix<double>> A; // MC transition intencities matrix
        Func<double, double, Vector<double>>[] c; // CP  observation intencities
        List<SimultaneousJumpsIntencity>[] I; // Intencities of simultaneous MC transitions and CP jumps
        Func<double, double, Vector<double>> R; // Continuous observations drift matrix
        Func<double, double, Vector<double>> G; // Continous observations diffusion matrix
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

        public OptimalFilter(int _N, double _t0, double _T, double _h, Func<double, double, Matrix<double>> _A, Func<double, double, Vector<double>>[] _c, List<SimultaneousJumpsIntencity>[] _I, Func<double, double, Vector<double>> _R, Func<double, double, Vector<double>> _G, bool _SaveHistory = false)
        {
            N = _N;
            t0 = _t0;
            t = t0;
            T = _T;
            h = _h;

            A = _A;
            c = _c;
            I = _I;
            R = _R;
            G = _G;

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

        public Vector<double> Step(double u, int[] dy, double dz, bool _doCalculateFilter) 
        {
            ///
            /// u  - control
            /// y  - counting process observations; 
            /// z  - continuous proxess observations
            ///
            t += h;
            if (_doCalculateFilter)
            {
                var lambda = A(t, u);
                //var mu = Matrix<double>.Build.DenseOfColumnVectors(c.Select(v => v(t, u)));

                var k = Extensions.Diag(pi) - pi.ToColumnMatrix() * pi.ToRowMatrix();
                var x_part = pi + lambda.TransposeThisAndMultiply(pi)*h;
                var y_part = Extensions.Zero(N);
                for (int i = 0; i < dy.Length; i++)
                {
                    var IM = Matrix<double>.Build.Dense(N, N, (ii, jj) => I[i].FirstOrDefault(elem => elem.From == jj && elem.To == ii)?.Intencity(t, u) ?? 0.0);
                    var y_part_coeff = k * c[i](t, u);
                    for (int j = 0; j < N; j++)
                    {
                        y_part_coeff = y_part_coeff + pi[j] * IM.Column(j);
                    }
                    if (dy[i] > 0)
                    {
                        y_part = y_part + dy[i] * y_part_coeff / (c[i](t, u).DotProduct(pi));
                    }
                    else
                    {
                        y_part = y_part - h * y_part_coeff;
                    }
                }

                var z_part = 0.0; // !!!!!!!!!!!!!!!!! TODO !!!!!!!!!!!!!!!!!!


                if (dy.Sum() == 0)
                {
                    pi = x_part + y_part + z_part;
                }
                else
                {
                    pi = y_part;
                }

                //var y_part = k * mu.Transpose() + 

                //for (int i = 0; i < pi.Count; i++)
                //    if (pi[i] < 0) pi[i] = 0;
                //pi = pi.Normalize(1.0);
            }
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

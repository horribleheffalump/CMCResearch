using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.Distributions;

namespace CMC
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
        public double h = 1e-4; // discretization step
        public int Obs = 0;     // current observation
        public double U; // current control

        public double Nu = 0;   // RHS martingale

        //public bool SaveHistory;
        public int SaveEvery;
        public List<Estimate> estimates; // estimates list

        private int saveCounter = 0;

        public OptimalFilter(int _N, double _t0, double _T, double _h, Func<double, double, Matrix<double>> _A, Func<double, double, Vector<double>>[] _c, List<SimultaneousJumpsIntencity>[] _I, int _SaveEvery = 0)
        {
            N = _N;
            t0 = _t0;
            t = t0;
            T = _T;
            h = _h;

            A = _A;
            c = _c;
            I = _I;
            //pi0 = Vector<double>.Build.Dense(N, 1.0 / N); //!!!!!! TODO  !!!!!!!!!!!!!!!!
            pi0 = Vector<double>.Build.Dense(N, 0.0); pi0[0] = 1.0; //!!!!!! TODO  !!!!!!!!!!!!!!!!
            pi = pi0;

            estimates = new List<Estimate>();
            SaveEvery = _SaveEvery;
            if (SaveEvery > 0)
            {
                estimates.Add(new Estimate(t0, pi0, 0));
            }
        }


        public OptimalFilter(int _N, double _t0, double _T, double _h, Func<double, double, Matrix<double>> _A, Func<double, double, Vector<double>>[] _c, List<SimultaneousJumpsIntencity>[] _I, Func<double, double, Vector<double>> _R, Func<double, double, Vector<double>> _G, int _SaveEvery = 0)
            : this(_N, _t0, _T, _h, _A, _c, _I, _SaveEvery)
        {
            R = _R;
            G = _G;
        }

        public Vector<double> Step(double u, int[] dy, double dz)
        {
            ///
            /// u  - control
            /// y  - counting process observations; 
            /// z  - continuous process observations
            ///

            U = u;
            t += h;
            var lambda = A(t, u);
            //var mu = Matrix<double>.Build.DenseOfColumnVectors(c.Select(v => v(t, u)));

            var k = Extensions.Diag(pi) - pi.ToColumnMatrix() * pi.ToRowMatrix();
            var x_part = lambda.TransposeThisAndMultiply(pi) * h;
            var y_part = Extensions.Zero(N);
            for (int i = 0; i < dy.Length; i++)
            {
                var IM = Matrix<double>.Build.Dense(N, N, (ii, jj) =>
                {
                    if (ii != jj)
                        return I[i].FirstOrDefault(elem => elem.From == jj && elem.To == ii)?.Intencity(t, u) ?? 0.0;
                    else
                        return -I[i].Where(elem => elem.From == jj)?.Sum(elem => elem.Intencity(t, u)) ?? 0.0;
                });
                var y_part_coeff = k * c[i](t, u) + IM * pi;
                if (dy[i] > 0)
                {
                    y_part = y_part + dy[i] * y_part_coeff / (c[i](t, u).DotProduct(pi));
                }
                else
                {
                    y_part = y_part - h * y_part_coeff;
                }
            }
            //var z_part = k * R(t, u) / G(t, u).DotProduct(G(t, u)) * (dz - R(t, u).DotProduct(pi) * h);

            if (dy.Sum() == 0)
            {
                pi = pi + x_part + y_part;
                if (R != null && G != null)
                {
                    //pi = pi + x_part + y_part + z_part;
                    //pi = pi + x_part + y_part;
                    for (int j = 0; j < pi.Count; j++)
                    {
                        pi[j] = pi[j] * Normal.PDF(R(t, u)[j] * h, G(t, u)[j] * Math.Sqrt(h), dz);
                    }
                    pi = pi.Normalize(1.0);
                }
            }
            else
            {
                pi = pi + y_part;
            }

            //var y_part = k * mu.Transpose() + 

            //for (int i = 0; i < pi.Count; i++)
            //    if (pi[i] < 0) pi[i] = 0;
            //pi = pi.Normalize(1.0);
            Save();
            return pi;
        }

        private void Save()
        {
            saveCounter++;
            if (SaveEvery > 0 && saveCounter % SaveEvery == 0)
            {
                var estimate = new Estimate(t, pi, U);
                estimates.Add(estimate);
            }
        }

        public void SaveTrajectory(string path) //, int every)
        {
            using (System.IO.StreamWriter outputfile = new System.IO.StreamWriter(path))
            {
                //foreach (Estimate e in estimates.Where((x, i) => i % every == 0).OrderBy(s => s.t))
                foreach (Estimate e in estimates.OrderBy(s => s.t))
                {
                    outputfile.WriteLine(e.ToString());
                }
                outputfile.Close();
            }
        }

    }
}

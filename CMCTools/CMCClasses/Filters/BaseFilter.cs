using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.Distributions;

namespace CMC.Filters
{
    /// <summary>
    /// Filter base class for controllable Markov chain with scalar continuous observations
    /// and/or multiple counting process observations with possible simultaneous jumps and MC transitions
    /// </summary>
    public abstract class BaseFilter
    {
        public string Name;     // filter name 
        public int N;           // state space dimension
        public Func<double, double, Matrix<double>> A; // MC transition intencities matrix
        public double t;        // current time
        public double t0;   // observation start time
        public double T;        // observation end time
        Vector<double> pi0;      // initial state
        public Vector<double> pi;      // current estimate
        public double h; // discretization step

        public int SaveEvery;
        public List<Estimate> estimates; // estimates list

        private int saveCounter = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="N">number of states</param>
        /// <param name="t0">observation start time</param>
        /// <param name="T">observation end time</param>
        /// <param name="h">discretization step</param>
        /// <param name="A">Markov chain transition rates matrix function A(t,u)</param>
        /// <param name="_SaveEvery"></param>
        public BaseFilter(int N, double t0, double T, double h, Func<double, double, Matrix<double>> A, int SaveEvery = 1)
        {
            this.N = N;
            this.t0 = t0;
            t = t0;
            this.T = T;
            this.h = h;

            this.A = A;

            pi0 = Vector<double>.Build.Dense(N, 0.0); pi0[0] = 1.0; // initial state estimate
            pi = pi0;

            estimates = new List<Estimate>();
            this.SaveEvery = SaveEvery;
            //if (SaveEvery > 0)
            //{
            //    estimates.Add(new Estimate(t0, pi0));
            //}
        }

        /// <summary>
        /// Base filter step function to be implemented by particular filter
        /// </summary>
        /// <param name="u">scalar control</param>
        /// <param name="dy">array of counting observation increments</param>
        /// <param name="dz">continuous observation icrement</param>
        /// <returns></returns>
        public abstract Vector<double> Step(double u, int[] dy = null, double? dz = null);

        /// <summary>
        /// Save estimates to list according to thinning settings (SaveEvery)
        /// </summary>
        public void Save(params double[] p)
        {
            saveCounter++;
            if (SaveEvery > 0 && saveCounter % SaveEvery == 0)
            {
                var estimate = new Estimate(t, pi, p);
                estimates.Add(estimate);
            }
        }

        /// <summary>
        /// Saves estimates to list ignoring the thinning settngs
        /// </summary>
        /// <param name="p"></param>
        public void DoSave(params double[] p)
        {
            var estimate = new Estimate(t, pi, p);
            estimates.Add(estimate);
        }

        /// <summary>
        /// Save estimate trajectory to file
        /// </summary>
        /// <param name="path">file path</param>
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

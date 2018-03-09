using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CMC
{

    public class Criterion
    {
        Func<double, Vector<double>[], double[], int[], double> F_system;  // multiple system value function. Depends on t, X (array of all MC states), U (array of controls for each MC), Obs (array of observations for all MCs)
        Func<double, Vector<double>, Vector<double>, double, double[], double[], double> F_single;  // single system value function depends on t, X - vector state, pi - vector state estimate, u - scalar control, obs - array of observations, p - array of additional params
        //public int M;           // number of MC
        //public double t;        // current time
        //public double t0 = 0;   // observation start time
        //public double T;        // observation end time
        public double h; // discretization step
        public double J = 0;
        public List<Sample> valueFunctionSamples;
        public int SaveEvery; // trajectory storage thinning parameter to save memory
        private int saveCounter = 0;

        public Criterion(double _h, int _SaveEvery = 1)
        {
            h = _h;
            valueFunctionSamples = new List<Sample>();
            SaveEvery = _SaveEvery;
        }

        public Criterion(double _h, Func<double, Vector<double>[], double[], int[], double> _F, int _SaveEvery = 1) : this(_h, _SaveEvery)
        {
            F_system = _F;
        }

        public Criterion(double _h, Func<double, Vector<double>, Vector<double>, double, double[], double[], double> _F, int _SaveEvery = 1) : this(_h, _SaveEvery)
        {
            F_single = _F;
        }

        public double Step(double t, Vector<double>[] X, double[] U, int[] Obs) //Vectors contain values for each channel
        {
            var dJ = F_system(t, X, U, Obs);
            return Step(t, dJ, U);
        }

        public double Step(double t, Vector<double> X, Vector<double> pi, double u, double[] Obs, double[] p) 
        {
            var dJ = F_single(t, X, pi, u, Obs, p);
            return Step(t, dJ, new double[] {u});
        }


        public double Step(double t, double dJ, double[] p) //Vectors contain values for each channel
        {
            saveCounter++;
            if (SaveEvery > 0 && saveCounter % SaveEvery == 0)
            {
                valueFunctionSamples.Add(new Sample(t, dJ, p));
            }

            J += dJ * h;
            return J;
        }

        public void SaveTrajectory(string path, int every = 0) // left 'every' param for compatibility with multipleBSModel (// TODO: eliminate it!!!)
        {
            if (valueFunctionSamples.Count > 0)
            {
                using (System.IO.StreamWriter outputfile = new System.IO.StreamWriter(path))
                {
                    //foreach (Sample sample in valueFunctionSamples.Where((x, i) => i % every == 0).OrderBy(s => s.t))
                    foreach (Sample sample in valueFunctionSamples.OrderBy(s => s.t))
                    {
                        outputfile.WriteLine(sample.ToString());
                    }
                    outputfile.Close();
                }
            }
        }



    }
}

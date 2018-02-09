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
        Func<double, Vector<double>[], double[], int[], double> F;  // value function depends on t, X (array of all MC states), U (array of controls for each MC), Obs (array of observations for all MCs)
        //public int M;           // number of MC
        //public double t;        // current time
        //public double t0 = 0;   // observation start time
        //public double T;        // observation end time
        public double h = 1e-3; // discretization step
        public double J = 0;
        public List<Sample> valueFunctionSamples;
        public int SaveEvery; // trajectory storage thinning parameter to save memory
        private int saveCounter = 0;

        public Criterion(double _h, Func<double, Vector<double>[], double[], int[], double> _F, int _SaveEvery = 1)
        {
            //N = _N;
            //t0 = _t0;
            //t = t0;
            //T = _T;
            h = _h;

            F = _F;
            valueFunctionSamples = new List<Sample>();

            SaveEvery = _SaveEvery;
        }

        public double Step(double t, Vector<double>[] X, double[] U, int[] Obs) //Vectors contain values for each channel
        {
            var dJ = F(t, X, U, Obs);

            saveCounter++;
            if (SaveEvery > 0 && saveCounter % SaveEvery == 0)
            {
                valueFunctionSamples.Add(new Sample(t, dJ, U));
            }

            J += dJ * h;
            return J;
        }

        public void SaveTrajectory(string path, int every = 0) // left 'every' param for compatibility with multipleBSModel (// TODO: eliminate it!!!)
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

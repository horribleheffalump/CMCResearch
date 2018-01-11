using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CMC
{
    public class Sample
    {
        public double t;
        public double val;
        public double sumU;
        public double[] U;

        public Sample(double _t, double _val, double[] _u)
        {
            t = _t;
            val = _val;
            U = _u;
            sumU = U.Sum();
        }

        public override string ToString()
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            string result = string.Format(provider, "{0} {1} {2}", t, val, sumU);
            return result;
        }

    }
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

        public Criterion(double _h, Func<double, Vector<double>[], double[], int[], double> _F)
        {
            //N = _N;
            //t0 = _t0;
            //t = t0;
            //T = _T;
            h = _h;

            F = _F;
            valueFunctionSamples = new List<Sample>();
        }

        public double Step(double t, Vector<double>[] X, double[] U, int[] Obs) //Vectors contain values for each channel
        {
            var dJ = F(t, X, U, Obs);
            valueFunctionSamples.Add(new Sample(t, dJ, U));
            J += dJ * h;
            return J;
        }

        public void SaveTrajectory(string path, int every)
        {
            using (System.IO.StreamWriter outputfile = new System.IO.StreamWriter(path))
            {
                foreach (Sample sample in valueFunctionSamples.Where((x, i) => i % every == 0).OrderBy(s => s.t))
                {
                    outputfile.WriteLine(sample.ToString());
                }
                outputfile.Close();
            }
        }


    }
}

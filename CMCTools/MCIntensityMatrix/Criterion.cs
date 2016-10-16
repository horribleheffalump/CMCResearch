using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCTools
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

        public Criterion(double _h, Func<double, Vector<double>[], double[], int[], double> _F)
        {
            //N = _N;
            //t0 = _t0;
            //t = t0;
            //T = _T;
            h = _h;

            F = _F;
        }

        public double Step(double t, Vector<double>[] X, double[] U, int[] Obs) //Vectors contain values for each channel
        {
            var dJ = F(t, X, U, Obs);
            J += dJ * h;
            return J;
        }

    }
}

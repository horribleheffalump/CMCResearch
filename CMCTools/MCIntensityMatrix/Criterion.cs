using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCTools
{
    public class Criterion
    {
        public int N;           // state space
        Func<double, int, Vector<double>, int, double> F;  // value function depends on t, X, U, N
        public double t;        // current time
        public double t0 = 0;   // observation start time
        public double T;        // observation end time
        public double h = 1e-3; // discretization step

        public double J = 0;

        public Criterion(int _N, double _t0, double _T, double _h, Func<double, int, Vector<double>, int, double> _F)
        {
            N = _N;
            t0 = _t0;
            t = t0;
            T = _T;
            h = _h;

            F = _F;
        }

        public double Step(double t, int X, Vector<double> U, int Obs) //!!!! should take into account all states!!!!!
        {
            var dJ = F(t, X, U, N);
            J += dJ * h;
            return J;
        }

    }
}

using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMC.Filters
{
    /// <summary>
    /// Optimal filter for controllable Markov chain with 
    /// multiple counting process observations with independent jumps and MC transitions.
    /// The filter is derived with measure change techniques.
    /// </summary>
    public class FilterDiscreteMeasureChange : BaseFilter
    {
        Func<double, double, Vector<double>>[] c; // CP  observation intencities

        public FilterDiscreteMeasureChange(int N, double t0, double T, double h, Func<double, double, Matrix<double>> A, Func<double, double, Vector<double>>[] c, int SaveEvery = 1)
            : base(N, t0, T, h, A, SaveEvery)
        {
            this.c = c;
        }


        public override Vector<double> Step(double u, int[] dy, double? dz)
        {
            t += h;
            var lambda = A(t-h, u);

            var x_part = lambda.TransposeThisAndMultiply(pi) * h;

            var y_part = Extensions.Zero(N);
            for (int i = 0; i < dy.Length; i++)
            {
                var dNu  = dy[i] - (c[i](t,u).ToRowMatrix() * pi)[0] * u * h;
                var Gamma = (1.0 / (c[i](t, u).ToRowMatrix() * pi)[0]) * c[i](t, u).PointwiseMultiply(pi) - pi;
                y_part = y_part + Gamma * dNu;
            }

            pi = pi + x_part + y_part;

            for (int i = 0; i < pi.Count; i++)
                if (pi[i] < 0) pi[i] = 0;
            pi = pi.Normalize(1.0);

            Save();
            return pi;
        }
    }
}


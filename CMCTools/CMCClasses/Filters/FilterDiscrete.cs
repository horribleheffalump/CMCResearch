using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMC.Filters
{
    /// <summary>
    /// Optimal filter for controllable Markov chain with 
    /// multiple counting process observations with possible simultaneous jumps and MC transitions
    /// </summary>
    public class FilterDiscrete : Filter
    {
        Func<double, double, Vector<double>>[] c; // CP  observation intencities
        List<SimultaneousJumpsIntencity>[] I; // Intencities of simultaneous MC transitions and CP jumps


        public FilterDiscrete(int N, double t0, double T, double h, Func<double, double, Matrix<double>> A, Func<double, double, Vector<double>>[] c, List<SimultaneousJumpsIntencity>[] I, int SaveEvery = 1)
            : base(N, t0, T, h, A, SaveEvery)
        {
            this.c = c;
            this.I = I;
        }


        public override Vector<double> Step(double u, int[] dy, double? dz)
        {
            t += h;
            var lambda = A(t, u);

            var k = Extensions.Diag(pi) - pi.ToColumnMatrix() * pi.ToRowMatrix();
            //var x_part = lambda.TransposeThisAndMultiply(pi) * h;
            var x_part = lambda * pi * h;
            var y_part = Extensions.Zero(N);
            for (int i = 0; i < dy.Length; i++)
            {
                var y_part_coeff = k * c[i](t, u);
                if (I != null)
                {
                    var IM = Matrix<double>.Build.Dense(N, N, (ii, jj) =>
                    {
                        if (ii != jj)
                            return I[i].FirstOrDefault(elem => elem.From == jj && elem.To == ii)?.Intencity(t, u) ?? 0.0;
                        else
                            return -I[i].Where(elem => elem.From == jj)?.Sum(elem => elem.Intencity(t, u)) ?? 0.0;
                    });
                    y_part_coeff = y_part_coeff + IM * pi;
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

            if (dy.Sum() == 0)
            {
                pi = pi + x_part + y_part;
            }
            else
            {
                pi = pi + y_part;
            }

            for (int i = 0; i < pi.Count; i++)
                if (pi[i] < 0) pi[i] = 0;
            pi.Normalize(1.0);
            Save();
            return pi;
        }
    }
}

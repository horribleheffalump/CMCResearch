using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMC.Filters
{
    /// <summary>
    /// Optimal filter for controllable Markov chain with scalar continuous observations
    /// and multiple counting process observations with possible simultaneous jumps and MC transitions.
    /// Gaussian approximation is used to incorporate the continuous observation increments
    /// </summary>
    public class FilterDiscreteContinuousGaussian : Filter
    {
        Func<double, double, Vector<double>>[] c; // CP  observation intencities
        List<SimultaneousJumpsIntencity>[] I; // Intencities of simultaneous MC transitions and CP jumps
        Func<double, double, Vector<double>> R; // Continuous observations drift matrix
        Func<double, double, Vector<double>> G; // Continous observations diffusion matrix
        double hObs;

        public FilterDiscreteContinuousGaussian(int N, double t0, double T, double h, Func<double, double, Matrix<double>> A, Func<double, double, Vector<double>>[] c, List<SimultaneousJumpsIntencity>[] I, Func<double, double, Vector<double>> R, Func<double, double, Vector<double>> G, int SaveEvery = 1, double hObs = 0)
            : base(N, t0, T, h, A, SaveEvery)
        {
            this.c = c;
            this.I = I;
            this.R = R;
            this.G = G;

            this.hObs = h;
            if (hObs > h)
                this.hObs = hObs;
        }


        public override Vector<double> Step(double u, int[] dy, double? dz)
        {
            t += h;
            var lambda = A(t, u);

            var k = Extensions.Diag(pi) - pi.ToColumnMatrix() * pi.ToRowMatrix();
            var x_part = lambda.TransposeThisAndMultiply(pi) * h;
            var y_part = Extensions.Zero(N);
            for (int i = 0; i < dy.Length; i++)
            {
                Matrix<double> IM;
                if (I == null)
                {
                    IM = Extensions.Eye(N);
                }
                else
                {
                    IM = Matrix<double>.Build.Dense(N, N, (ii, jj) =>
                    {
                        if (ii != jj)
                            return I[i].FirstOrDefault(elem => elem.From == jj && elem.To == ii)?.Intencity(t, u) ?? 0.0;
                        else
                            return -I[i].Where(elem => elem.From == jj)?.Sum(elem => elem.Intencity(t, u)) ?? 0.0;
                    });
                }
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
                if (dz.HasValue)
                {
                    //pi = pi + x_part + y_part + z_part;
                    //pi = pi + x_part + y_part;
                    for (int j = 0; j < pi.Count; j++)
                    {
                        pi[j] = pi[j] * Normal.PDF(R(t, u)[j] * hObs, G(t, u)[j] * Math.Sqrt(hObs), dz.Value);
                    }
                    pi = pi.Normalize(1.0);
                }
            }
            else
            {
                pi = pi + y_part;
            }

            //var y_part = k * mu.Transpose() + 

            for (int i = 0; i < pi.Count; i++)
                if (pi[i] < 0) pi[i] = 0;
            pi = pi.Normalize(1.0);
            Save();
            return pi;
        }
    }
}

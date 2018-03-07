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
    public class FilterDiscreteContinuousGaussian : BaseFilter
    {
        Func<double, double, Vector<double>>[] c; // CP  observation intencities
        List<SimultaneousJumpsIntencity>[] I; // Intencities of simultaneous MC transitions and CP jumps
        Func<double, double, Vector<double>> R; // Continuous observations drift matrix
        Func<double, double, Vector<double>> G; // Continous observations diffusion matrix
        //double hObs;

        Vector<double> integral_R = null;
        Vector<double> integral_G2 = null;

        public FilterDiscreteContinuousGaussian(int N, double t0, double T, double h, Func<double, double, Matrix<double>> A, Func<double, double, Vector<double>>[] c, List<SimultaneousJumpsIntencity>[] I, Func<double, double, Vector<double>> R, Func<double, double, Vector<double>> G, int SaveEvery = 1)
            : base(N, t0, T, h, A, SaveEvery)
        {
            this.c = c;
            this.I = I;
            this.R = R;
            this.G = G;


            //this.hObs = h;
            //if (hObs > h)
            //    this.hObs = hObs;
        }


        public override Vector<double> Step(double u, int[] dy, double? dz)
        {
            t += h;

            var dR = R(t, u) * h;
            var dG2 = G(t, u).PointwisePower(2.0) * h;
            if (integral_R == null) integral_R = dR;
            else integral_R = integral_R + dR;
            if (integral_G2 == null) integral_G2 = dG2;
            else integral_G2 = integral_G2 + dG2;

            var lambda = A(t, u);

            var k = Extensions.Diag(pi) - pi.ToColumnMatrix() * pi.ToRowMatrix();
            //var x_part = lambda.TransposeThisAndMultiply(pi) * h;
            var x_part = (lambda.Transpose() * h).Exponential() * pi;
            var y_part = Extensions.Zero(N);
            for (int i = 0; i < dy.Length; i++)
            {
                Matrix<double> IM;
                if (I == null)
                {
                    IM = Matrix<double>.Build.DenseIdentity(N, N);
                }
                else
                {
                    //IM = Matrix<double>.Build.Dense(N, N, (ii, jj) =>
                    //{
                    //    if (ii != jj)
                    //        return I[i].FirstOrDefault(elem => elem.From == jj && elem.To == ii)?.Intensity(t, u) ?? 0.0;
                    //    else
                    //        return -I[i].Where(elem => elem.From == jj)?.Sum(elem => elem.Intensity(t, u)) ?? 0.0;
                    //});

                    IM = Matrix<double>.Build.Dense(N, N, 0.0);
                    foreach (var item in I[i])
                    {
                        IM[item.To, item.From] += item.Intensity(t, u);
                        IM[item.From, item.From] -= item.Intensity(t, u);
                    }
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
                //pi = pi + x_part + y_part;
                pi = x_part + y_part;
                for (int i = 0; i < pi.Count; i++)
                    if (pi[i] < 0)
                        pi[i] = 0;
                pi = pi.Normalize(1.0);
                if (dz.HasValue)
                    if (dz.Value > 0)
                    {

                        //pi = pi + x_part + y_part + z_part;
                        //pi = pi + x_part + y_part;
                        for (int j = 0; j < pi.Count; j++)
                        {
                            //var hObs = h;
                            //var temp = pi[j] * Normal.PDF(R(t, u)[j] * hObs, G(t, u)[j] * Math.Sqrt(hObs), dz.Value);
                            pi[j] = pi[j] * Normal.PDF(integral_R[j], Math.Sqrt(integral_G2[j]), dz.Value);
                            //if (pi[j] != temp)
                            //{
                            //    Console.WriteLine("omg!");
                            //}
                        }
                        //pi = pi.Normalize(1.0);
                        integral_R = null;
                        integral_G2 = null;
                    }
            }
            else
            {
                pi = pi + y_part;
            }

            //var y_part = k * mu.Transpose() + 

            for (int i = 0; i < pi.Count; i++)
            {
                if (pi[i] < 0)
                {
                    if (Math.Abs(pi[i]) > 0.001)
                    {
                        Console.WriteLine("ups");
                    }
                    pi[i] = 0;
                }
            }
            pi = pi.Normalize(1.0);
            Save();
            return pi;
        }
    }
}

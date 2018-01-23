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
    public class DummyFilter : Filter
    {
        Func<double, double, Vector<double>>[] c; // CP  observation intencities
        List<SimultaneousJumpsIntencity>[] I; // Intencities of simultaneous MC transitions and CP jumps
        Func<double, double, Vector<double>> R; // Continuous observations drift matrix
        Func<double, double, Vector<double>> G; // Continous observations diffusion matrix        
        Func<double, double, double> ci0; // loss intencities lessened by simultaneous jump intensities I
        Func<double, double, double> ci1; // timeout intencities lessened by simultaneous jump intensities I
        Func<int> X; // real state

        public DummyFilter(int N, double t0, double T, double h, Func<double, double, Matrix<double>> A, Func<double, double, Vector<double>>[] c, List<SimultaneousJumpsIntencity>[] I, Func<double, double, double> ci0, Func<double, double, double> ci1, Func<double, double, Vector<double>> R, Func<double, double, Vector<double>> G, Func<int> X, int SaveEvery = 1)
            : base(N, t0, T, h, A, SaveEvery)
        {
            this.c = c;
            this.I = I;
            this.R = R;
            this.G = G;
            this.ci0 = ci0;
            this.ci1 = ci1;
            this.X = X;
        }


        public override Vector<double> Step(double u, int[] dy, double? dz)
        {
            t += h;
            if (I != null)
                Save(Extensions.Stack(
                    Extensions.Vector(X()),
                    G(t, u), 
                    R(t, u), 
                    c[0](t,u), 
                    c[1](t,u), 
                    Extensions.Vector(ci0(t,u), ci1(t,u)),
                    Extensions.Vector(I[0].Select(e => e.Intencity(t, u)).ToArray()),
                    Extensions.Vector(I[1].Select(e => e.Intencity(t, u)).ToArray()),
                    Extensions.Vector(A(t, u)[0, 3], A(t, u)[1, 3], A(t, u)[1, 2]),
                    Extensions.Vector(A(t, u)[3, 0], A(t, u)[3, 1], A(t, u)[2, 1])
                    ).ToArray()
                    );
            else
                Save(Extensions.Stack(
                    Extensions.Vector(X()),
                    G(t, u),
                    R(t, u),
                    c[0](t, u),
                    c[1](t, u)
                    ).ToArray()
                    );

            return pi;
        }
    }
}

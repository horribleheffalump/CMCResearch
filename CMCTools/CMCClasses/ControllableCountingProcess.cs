using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMC
{
    public class ControllableCountingProcess
    {
        public Func<double, double, double> Intensity;
        public double t;        // current time
        public double t0 = 0;   // observation start time
        public double T;        // observation end time
        public int N0 = 0;      // initial state
        public int N;           // current state
        public int dN = 0;      // N - N(t-deltaT)
        public double deltaT = 0.0;   //  
        public double h = 1e-3; // discretization step
        public bool SaveHistory;

        public List<Jump> Jumps; // sequence of jumps

        public ControllableCountingProcess(double _t0, double _T, int _N0, double _h, Func<double, double, double> _Intensity, bool _SaveHistory = false)
        {
            t0 = _t0;
            t = t0;
            Jumps = new List<Jump>();
            T = _T;
            N0 = _N0;
            N = N0;
            h = _h;
            Intensity = _Intensity;
            SaveHistory = _SaveHistory;
            //if (SaveHistory)
            Jumps.Add(new Jump(t0, N0));
        }

        public int Step(double u)
        {
            double p = Intensity(t, u) * h;
            t += h;
            if (p > 0) // for the case when we got zero or less intencity due to the simultaneous jumps modelling
            {
                int nojump = FiniteDiscreteDistribution.Sample(Vector<double>.Build.DenseOfArray(new[] { p, 1 - p }));
                if (nojump == 0)
                {
                    N++;
                    var J = new Jump(t, N);
                    //if (SaveHistory)
                    Jumps.Add(J);
                }
                if (deltaT > 0.0) // if deltaT assigned, dN is equal to one if there was a jump during [t-deltaT, t]
                {
                    dN = N - Jumps.FindLast(j => j.t <= Math.Max(0, t - deltaT)).X;
                }
                else // if deltaT is not assigned, dN is equal to one if there was a jump right now
                {
                    dN = nojump == 0 ? 1 : 0;
                }
            }
            else
                dN = 0;

            return N;
        }

        public void Jump()
        {
            N++;
            var J = new Jump(t, N, true);
            //if (SaveHistory)
            Jumps.Add(J);
            if (deltaT > 0.0) // if deltaT assigned, dN is equal to one if it was a jump during [t-deltaT, t]
            {
                dN = N - Jumps.FindLast(j => j.t <= Math.Max(0, t - deltaT)).X;
            }
            else // if deltaT is not assigned, dN is equal to one if it was a jump right now
            {
                dN = 1;
            }
        }

        //public Jump GetNextState(Func<double, double> u)
        //{
        //    while (t < T)
        //    {
        //        double p = Intensity(t, u(t)) * h;
        //        t += h;
        //        int nojump = FiniteDiscreteDistribution.Sample(Vector<double>.Build.DenseOfArray(new[] { p, 1 - p }));
        //        if (nojump == 0)
        //        {
        //            N++;
        //            var J = new Jump(t, N);
        //            Jumps.Add(J);
        //            return J;
        //        }
        //    }
        //    return new Jump(T, N);
        //}

        //public void GenerateTrajectory(Func<double, double> u)
        //{
        //    while (t < T)
        //    {
        //        GetNextState(u);
        //    }
        //    Jumps.Add(new Jump(T, N));
        //}

        public void SaveTrajectory(string path)
        {
            using (System.IO.StreamWriter outputfile = new System.IO.StreamWriter(path))
            {
                foreach (Jump j in Jumps.OrderBy(s => s.t))
                {
                    outputfile.WriteLine(j.ToString());
                }
                outputfile.Close();
            }
        }

    }


}

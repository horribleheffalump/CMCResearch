using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCTools
{
    public class ControllableMarkovChain
    {
        public int N;           // state space
        public Func<double, double, Matrix<double>> TransitionRateMatrix;
        public double t;        // current time
        public double t0 = 0;   // observation start time
        public double T;        // observation end time
        public int X0 = 0;      // initial state
        public int X;           // current state
        public double h = 1e-3; // discretization step

        //public List<double> JumpTimes; // sequence of jump times
        //public List<int> States;       // sequence of states
        public List<Jump> Jumps;

        public ControllableMarkovChain(int _N, double _t0, double _T, int _X0, double _h, Func<double, double, Matrix<double>> _TransitionRateMatrix)
        {
            N = _N;
            t0 = _t0;
            t = t0;
            Jumps = new List<Jump>();
            //JumpTimes = new List<double>();
            //JumpTimes.Add(t0);
            T = _T;
            X0 = _X0;
            X = X0;
            //States = new List<int>();
            //States.Add(X0);
            h = _h;
            TransitionRateMatrix = _TransitionRateMatrix;
            Jumps.Add(new Jump(t0, X0));
        }


        //public int State(double t)
        //{
        //    int result = int.MinValue;
        //    if (t > Jumps.Max(j => j.t) || t > Jumps.Min(j => j.t))
        //    {
        //        throw new ArgumentOutOfRangeException("time instant t", "Time instant is out of generated trajectory so far");
        //    }
        //    else
        //    {
        //        result = Jumps.Where(j => j.t < t).OrderByDescending(j => j.t).FirstOrDefault().X;
        //    }
        //    return result;
        //}

        public int Step(Vector<double> U)
        {
            Matrix<double> P = TransitionRateMatrix(t, U[X]) * h + Matrix<double>.Build.DenseIdentity(N);
            t += h;
            int x = FiniteDiscreteDistribution.Sample(P.Row(X));
            if (x != X)
            {
                X = x;
                var J = new Jump(t, X);
                Jumps.Add(J);
            }
            return x;
        }

        public Jump GetNextState(Func<double, Vector<double>> U)
        {
            while (t < T)
            {
                Matrix<double> P = TransitionRateMatrix(t, U(t)[X]) * h + Matrix<double>.Build.DenseIdentity(N);
                t += h;
                int x = FiniteDiscreteDistribution.Sample(P.Row(X));
                if (x != X)
                {
                    X = x;
                    //JumpTimes.Add(t);
                    //States.Add(X);
                    var J = new Jump(t, X);
                    Jumps.Add(J);
                    return J;
                }
            }
            return new Jump(T, X);
        }

        public void GenerateTrajectory(Func<double, Vector<double>> U)
        {
            while (t < T)
            {
                GetNextState(U);
            }
            Jumps.Add(new Jump(T, X));
        }

        public void SaveTrajectory(string path)
        {
            System.IO.StreamWriter outputfile = new System.IO.StreamWriter(path);
            foreach (Jump j in Jumps.OrderBy(s => s.t))
            {
                outputfile.WriteLine(j.ToString());
            }
            outputfile.WriteLine(new Jump(T,X).ToString());
            outputfile.Close();
        }


    }
}

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
        //public List<int> States;       // sequence of 

        public bool SaveHistory;
        public List<Jump> Jumps;
        public List<Matrix<double>> TransitionMatrices;

        public ControllableMarkovChain(int _N, double _t0, double _T, int _X0, double _h, Func<double, double, Matrix<double>> _TransitionRateMatrix, bool _SaveHistory = false)
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
            SaveHistory = _SaveHistory;
            if (SaveHistory)
                Jumps.Add(new Jump(t0, X0));
            TransitionMatrices = new List<Matrix<double>>();
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

        //public int Step(Vector<double> U)

        public Vector<double> Xvec
        {
            get
            {
                Vector<double> _Xvec = Vector<double>.Build.Dense(N, 0.0);
                _Xvec[X] = 1.0;
                return _Xvec;
            }
        }    // current state vector

        public int Step(double u)
        {
            var lambda = TransitionRateMatrix(t, u);
            //TransitionMatrices.Add(lambda);
            var P = lambda * h + Matrix<double>.Build.DenseIdentity(N);
            t += h;
            int x = FiniteDiscreteDistribution.Sample(P.Row(X));
            if (x != X)
            {
                X = x;
                var J = new Jump(t, X);
                if (SaveHistory)
                    Jumps.Add(J);
            }
            return x;
        }

        public void Transit(int x)
        {
            if (x != X)
            {
                X = x;
                var J = new Jump(t, X);
                if (SaveHistory)
                    Jumps.Add(J);
            }
        }

        //public Jump GetNextState(Func<double, Vector<double>> U) // here U is vector, because it is assumed to be predefined Markov control, i.e. vector of determined functions one for each state
        //{
        //    while (t < T)
        //    {
        //        var lambda = TransitionRateMatrix(t, U(t)[X]);
        //        //TransitionMatrices.Add(lambda);
        //        var P =  lambda * h + Matrix<double>.Build.DenseIdentity(N);

        //        t += h;
        //        int x = FiniteDiscreteDistribution.Sample(P.Row(X));
        //        if (x != X)
        //        {
        //            X = x;
        //            //JumpTimes.Add(t);
        //            //States.Add(X);
        //            var J = new Jump(t, X);
        //            Jumps.Add(J);
        //            return J;
        //        }
        //    }
        //    return new Jump(T, X);
        //}

        //public void GenerateTrajectory(Func<double, Vector<double>> U) // here U is vector, because it is assumed to be predefined Markov control, i.e. vector of determined functions one for each state
        //{
        //    while (t < T)
        //    {
        //        GetNextState(U);
        //    }
        //    Jumps.Add(new Jump(T, X));
        //}

        public void SaveTrajectory(string path)
        {
            using (System.IO.StreamWriter outputfile = new System.IO.StreamWriter(path))
            {
                foreach (Jump j in Jumps.OrderBy(s => s.t))
                {
                    outputfile.WriteLine(j.ToString());
                }
                outputfile.WriteLine(new Jump(T, X).ToString());
                outputfile.Close();
            }
        }

        public void SaveTransitionMatrices(string path)
        {
            using (System.IO.StreamWriter outputfile = new System.IO.StreamWriter(path))
            {
                foreach (var m in TransitionMatrices)
                {
                    outputfile.WriteLine(m.ToString());
                }
                outputfile.Close();
            }
        }


    }
}

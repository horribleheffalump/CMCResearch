using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMC
{
    public class ControllableContinuousProcess
    {

        // X_t = \int_0^t A(s, u) ds + \int_0^t B(s, u) dW_s

        public Func<double, double, double> A;
        public Func<double, double, double> B;
        public double t;        // current time
        public double t0 = 0;   // observation start time
        public double T;        // observation end time
        public double x0 = 0;      // initial state
        public double x;           // current state
        public double dx = 0.0;    // current state increment
        public double x_thin;           // current thinned state
        public double? dx_thin = null;    // current thinned state increment
        public double h = 1e-4; // discretization step
        public double hObs = 1e-4; // observable discretization step
        public int SaveEvery; // trajectory storage thinning parameter to save memory
        private int saveCounter = 0;
        private double deltah = 0.0;

        public List<ScalarContinuousState> Trajectory; // sequence of jumps

        Normal Noise;


        public ControllableContinuousProcess(double _t0, double _T, double _x0, double _h, Func<double, double, double> _a, Func<double, double, double> _b, int _SaveEvery = 0, double _hObs = 0)
        {
            t0 = _t0;
            t = t0;
            Trajectory = new List<ScalarContinuousState>();
            T = _T;
            x0 = _x0;
            x = x0;
            x_thin = x0;
            h = _h;
            hObs = _h;
            if (_hObs > _h)
                hObs = _hObs;
            A = _a;
            B = _b;
            SaveEvery = _SaveEvery;
            if (SaveEvery > 0)
                Trajectory.Add(new ScalarContinuousState(t0, x0));
            Noise = new Normal(0, 1);
        }

        public double Step(double u)
        {
            t += h;
            deltah += h;
            dx = A(t, u) * h + B(t, u) * Noise.Sample() * Math.Sqrt(h);
            x = x + dx;
            Save();

            if (deltah >= hObs)
            {
                deltah = 0.0;
                dx_thin = x - x_thin;
                x_thin = x;
            }
            else
            {
                dx_thin = null;
            }
            return x;
        }


        private void Save()
        {
            saveCounter++;
            if (SaveEvery > 0 && saveCounter % SaveEvery == 0)
            {
                Trajectory.Add(new ScalarContinuousState(t, x));
            }
        }

        public void SaveTrajectory(string path) //, int every = 1)
        {
            using (System.IO.StreamWriter outputfile = new System.IO.StreamWriter(path))
            {
                //foreach (ScalarContinuousState j in Trajectory.Where((x, i) => i % every == 0).OrderBy(s => s.t))
                foreach (ScalarContinuousState j in Trajectory.OrderBy(s => s.t))
                {
                    outputfile.WriteLine(j.ToString());
                }
                outputfile.Close();
            }
        }
    }
}
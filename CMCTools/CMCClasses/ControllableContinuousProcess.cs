﻿using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCTools
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
        public double h = 1e-3; // discretization step
        public int SaveEvery;
        private int saveCounter = 0;

        public List<ScalarContinuousState> Trajectory; // sequence of jumps

        Normal Noise;

        public ControllableContinuousProcess(double _t0, double _T, double _x0, double _h, Func<double, double, double> _a, Func<double, double, double> _b, int _SaveEvery = 0)
        {
            t0 = _t0;
            t = t0;
            Trajectory = new List<ScalarContinuousState>();
            T = _T;
            x0 = _x0;
            x = x0;
            h = _h;
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
            dx = A(t, u) * h + B(t, u) * Noise.Sample() * Math.Sqrt(h);
            x = x + dx;
            Save();
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
﻿using CMCTools;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemCPObs
{
    public class CountingProessObservationsSystem
    {
        public ControllableMarkovChain State;
        public ControllableCountingProcess Observation;

        public CountingProessObservationsSystem(int _N, double _t0, double _T, int _X0, double _h, Func<double, Matrix<double>> _A, Func<double, Vector<double>> _c)
        {
            State = new ControllableMarkovChain(_N, _t0, _T, _X0, _h, (t, u) => _A(t));
            Observation = new ControllableCountingProcess(_t0, _T, 0, _h, (t, u) => _c(t)[State.X] * u);
        }

        public void GenerateTrajectory(Func<double, Vector<double>> U)
        {
            while (State.t < State.T)
            {
                State.Step(U(State.t));
                Observation.Step(U(State.t)[State.X]);
            }
        }
    }
}

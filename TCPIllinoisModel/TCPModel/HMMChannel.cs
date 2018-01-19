﻿using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMC;
using SystemJointObs;

namespace Channel
{
    public class HMMChannel
    {
        int N;
        int Others;
        double ContinuousObservationsDiscretizationStep;
        public bool doSimulateSimultaneousJumps;
        public double delta_p;
        public Vector<double> D;
        public Vector<double> K;
        public Vector<double> Phi;
        public Vector<double> Psi;
        public Vector<double> P;
        public Vector<double> Q;

        public JointObservationsSystem JOS;

        public HMMChannel(double _t0, double _T, double _h, int _saveEvery)
        {

            N = 4; // states count: e1 - free, e2 - moderate load, e3 - wire congestion, e4 - last mile (wireless) bad signal
            doSimulateSimultaneousJumps = true; // simulating the simultaneous jumps of the Markov chain and the observable counting processes
            Others = 6; // number of other users of the channel (affects the simultaneous jumps intensity)

            // RTT = delta_p + D X_t + w_t K X_t
            delta_p = 0.01; // delta_p - signal propagation time
            D = Extensions.Vector(0.001, 0.01, 0.05, 0.1); // mean queueing time because of the other senders transmission
            K = Extensions.Vector(0.0005, 0.005, 0.025, 0.05); // mean queueing time because of senders own transmission
            Phi = D * 0.25; // Variance of queueing time because of the other senders transmission
            Psi = K * 0.25; // Variance deviation of queueing time because of senders own transmission
            //Extensions.Vector(0.001, 0.01, 0.02, 0.04);
            //loss intensity mu_t = R_t diag(P)
            P = Extensions.Vector(0.0005, 0.0025, 0.0075, 0.05);
            P = P * 3;
            //P = Vector(0.0, 0.0, 0.0, 0.0);
            //timeout intensity nu_t = R_t diag(Q)
            Q = Extensions.Vector(0.0001, 0.0005, 0.0015, 0.01);
            Q = Q * 1;
            //Q = Vector(0.0, 0.0, 0.0, 0.0);

            //ContinuousObservationsDiscretizationStep = _h;
            ContinuousObservationsDiscretizationStep = 1e-2;

            if (doSimulateSimultaneousJumps)
            {
                // the following simultaneous jumps are simulated:
                //     Loss may occure at the same time as the Markov chain jumps e1->e4, e2->e4 of e2->e3. 
                //     The intencity of the simultaneous jump and loss is equal to 1 / ( 6.0 * Others) * min (lambda_ij, mu)
                List<SimultaneousJumpsIntencity> LossSimultaneousIntencity = new List<SimultaneousJumpsIntencity>();
                LossSimultaneousIntencity.Add(new SimultaneousJumpsIntencity(0, 3, (t, u) => SimultaneousJumpAndLossIntensity(t, u, 0, 3)));
                LossSimultaneousIntencity.Add(new SimultaneousJumpsIntencity(1, 3, (t, u) => SimultaneousJumpAndLossIntensity(t, u, 1, 3)));
                LossSimultaneousIntencity.Add(new SimultaneousJumpsIntencity(1, 2, (t, u) => SimultaneousJumpAndLossIntensity(t, u, 1, 2)));
                //     Timeout may occure at the same time as the Markov chain jumps e1->e4, e2->e4 of e2->e3. 
                //     The intencity of the simultaneous jump and timeout is equal to 1/(6*Others) min (lambda_ij, nu)
                List<SimultaneousJumpsIntencity> TimeoutSimultaneousIntencity = new List<SimultaneousJumpsIntencity>();
                TimeoutSimultaneousIntencity.Add(new SimultaneousJumpsIntencity(0, 3, (t, u) => SimultaneousJumpAndTimeoutIntensity(t, u, 0, 3)));
                TimeoutSimultaneousIntencity.Add(new SimultaneousJumpsIntencity(1, 3, (t, u) => SimultaneousJumpAndTimeoutIntensity(t, u, 1, 3)));
                TimeoutSimultaneousIntencity.Add(new SimultaneousJumpsIntencity(1, 2, (t, u) => SimultaneousJumpAndTimeoutIntensity(t, u, 1, 2)));

                JOS = new JointObservationsSystemSimultaniousJumps(N, _t0, _T, 0, _h, TransitionMatrix, new Func<double, double, Vector<double>>[] { LossIntensity, TimeoutIntensity }, new List<SimultaneousJumpsIntencity>[] { LossSimultaneousIntencity, TimeoutSimultaneousIntencity }, R, G, _saveEvery, ContinuousObservationsDiscretizationStep);

            }
            else
            {
                JOS = new JointObservationsSystem(N, _t0, _T, 0, _h, TransitionMatrix, new Func<double, double, Vector<double>>[] { LossIntensity, TimeoutIntensity }, R, G, _saveEvery, ContinuousObservationsDiscretizationStep);
            }
        }



        public Matrix<double> TransitionMatrix(double t, double w)
        {
            double lambda14 = 0.01;
            double lambda41 = 0.03;
            double lambda24 = 0.01;
            double lambda42 = 0.005;
            double lambda12 = 0.01 + 0.001 * w;
            double lambda21 = Math.Max(0.01, 0.02 - 0.0005 * w);
            double lambda23 = 0.01 + 0.001 * w;
            double lambda32 = Math.Max(0.01, 0.02 - 0.0005 * w); ;
            Matrix<double> Lambda = Matrix<double>.Build.DenseOfArray(new[,]{{-(lambda12 + lambda14), lambda12, 0, lambda14 },
                                                                             { lambda21, -(lambda21 + lambda23 + lambda24 ), lambda23, lambda24 },
                                                                             { 0, lambda32, -lambda32, 0 },
                                                                             { lambda41, lambda42, 0, -(lambda41 + lambda42) }});
            //Matrix<double> Lambda = Matrix<double>.Build.DenseOfDiagonalArray(new[]{0.0, 0.0, 0.0, 0.0});
            if (!Lambda.IsTransitionRateMatrix())
            {
                throw new ArgumentException("The result matrix is not transition rate matrix");
            }

            return Lambda;

        }

        public double SimultaneousJumpAndLossIntensity(double t, double u, int i, int j)
        {
            // return 1 / Others * Math.Min(TransitionMatrix(t, u)[i, j], TimeoutIntensity(t, u)[i]);
            if ((LossIntensity(t, u)[i] + TimeoutIntensity(t, u)[i]) / Others <= TransitionMatrix(t, u)[i, j])
                return LossIntensity(t, u)[i] / Others;
            else
                return (LossIntensity(t, u)[i] * TransitionMatrix(t, u)[i, j]) / Others / (LossIntensity(t, u)[i] + TimeoutIntensity(t, u)[i]);
        }

        public double SimultaneousJumpAndTimeoutIntensity(double t, double u, int i, int j)
        {
            // return 1 / Others * Math.Min(TransitionMatrix(t, u)[i, j], TimeoutIntensity(t, u)[i]);
            if ((LossIntensity(t, u)[i] + TimeoutIntensity(t, u)[i]) / Others <= TransitionMatrix(t, u)[i, j])
                return TimeoutIntensity(t, u)[i] / Others;
            else
                return (TimeoutIntensity(t, u)[i] * TransitionMatrix(t, u)[i, j]) / Others / (LossIntensity(t, u)[i] + TimeoutIntensity(t, u)[i]);
        }

        public Vector<double> LossIntensity(double t, double u)
        {
            return Extensions.Diag(P) * R(t, u);
        }


        public Vector<double> TimeoutIntensity(double t, double u)
        {
            return Extensions.Diag(Q) * R(t, u);
        }

        public Vector<double> R(double t, double u)
        {
            return Extensions.Vector(
                1.0 / (delta_p + D[0] + u * K[0]),
                1.0 / (delta_p + D[1] + u * K[1]),
                1.0 / (delta_p + D[2] + u * K[2]),
                1.0 / (delta_p + D[3] + u * K[3])
                );
        }
        public Vector<double> G(double t, double u)
        {
            //return Extensions.Vector(
            //    1.0 / Math.Sqrt(D[0] + u * K[0]),
            //    1.0 / Math.Sqrt(D[1] + u * K[1]),
            //    1.0 / Math.Sqrt(D[2] + u * K[2]),
            //    1.0 / Math.Sqrt(D[3] + u * K[3])
            //    );
            return Extensions.Vector(
                Math.Sqrt(Phi[0] + u * Psi[0]) / Math.Pow(delta_p + D[0] + u * K[0], 1.5),
                Math.Sqrt(Phi[1] + u * Psi[1]) / Math.Pow(delta_p + D[1] + u * K[1], 1.5),
                Math.Sqrt(Phi[2] + u * Psi[2]) / Math.Pow(delta_p + D[2] + u * K[2], 1.5),
                Math.Sqrt(Phi[3] + u * Psi[3]) / Math.Pow(delta_p + D[3] + u * K[3], 1.5)
                );

        }



    }
}

using MathNet.Numerics.LinearAlgebra;
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
        public Dictionary<String, Criterion> Criterions;
        public Dictionary<String, Func<double>> TerminalCriterions;

        public HMMChannel(double _t0, double _T, double _h, int _saveEvery)
        {

            N = 4; // states count: e1 - free, e2 - moderate load, e3 - wire congestion, e4 - last mile (wireless) bad signal
            doSimulateSimultaneousJumps = true; // simulating the simultaneous jumps of the Markov chain and the observable counting processes
            Others = 1; // number of other users of the channel (affects the simultaneous jumps intensity)

            // RTT = delta_p + D X_t + w_t K X_t
            delta_p = 0.01; // delta_p - signal propagation time
            D = Extensions.Vector(0.001, 0.01, 0.05, 0.1); // mean queueing time because of the other senders transmission
            K = Extensions.Vector(0.0005, 0.005, 0.025, 0.05); // mean queueing time because of senders own transmission
            Phi = D * 0.25; // Variance of queueing time because of the other senders transmission
            Psi = K * 0.25; // Variance deviation of queueing time because of senders own transmission
            //Extensions.Vector(0.001, 0.01, 0.02, 0.04);
            //loss intensity mu_t = R_t diag(P)
            P = Extensions.Vector(0.00015, 0.001, 0.005, 0.005);
            //P = Extensions.Vector(0.0005, 0.0025, 0.005, 0.05);
            //P = P * 0.1;
            //P = P * 3;
            //P = Vector(0.0, 0.0, 0.0, 0.0);
            //timeout intensity nu_t = R_t diag(Q)
            Q = P * 0.2;
            //Q = Extensions.Vector(0.0001, 0.0005, 0.0015, 0.01);
            //Q = Q * 0.03;
            //Q = Q * 1;
            //Q = Vector(0.0, 0.0, 0.0, 0.0);

            //ContinuousObservationsDiscretizationStep = _h;
            ContinuousObservationsDiscretizationStep = 1e-2;

            Criterions = new Dictionary<string, Criterion>();

            Criterions.Add("Throughput", new Criterion(_h, (t, X, U, Obs) =>
            {
                double MTU = 1000; // packet size = 1000 bytes
                double rtt = U[1];
                if (rtt > 0)
                {
                    double Bps = U[0] * MTU / rtt; // instant bytes per second
                    double Mbps = Bps * 8 / 1e6; // instant Mbps
                    return Mbps; 

                }
                else
                    return 0.0;
            }, _saveEvery));

            Criterions.Add("TimeInGoodState", new Criterion(_h, (t, X, U, Obs) =>
            {
                return Math.Abs(X[0][0] - 1.0) < 1e-5 ? 1.0 : 0.0;
            }, _saveEvery));
            Criterions.Add("TimeInNormState", new Criterion(_h, (t, X, U, Obs) =>
            {
                return Math.Abs(X[0][1] - 1.0) < 1e-5 ? 1.0 : 0.0;
            }, _saveEvery));
            Criterions.Add("TimeInBadState", new Criterion(_h, (t, X, U, Obs) =>
            {
                return Math.Abs(X[0][2] - 1.0) < 1e-5 ? 1.0 : 0.0;
            }, _saveEvery));

            TerminalCriterions = new Dictionary<string, Func<double>>();

            TerminalCriterions.Add("Loss", () => JOS.CPObservations[0].N);
            TerminalCriterions.Add("Timeout", () => JOS.CPObservations[1].N);
            TerminalCriterions.Add("AverageThroughput", () => Criterions["Throughput"].J / JOS.State.T);
            TerminalCriterions.Add("TotalTimeInGoodState", () => Criterions["TimeInGoodState"].J);
            TerminalCriterions.Add("TotalTimeInNormState", () => Criterions["TimeInNormState"].J);
            TerminalCriterions.Add("TotalTimeInBadState", () => Criterions["TimeInBadState"].J);

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

        public void CalculateCriterions(double u, double rtt)
        {
            foreach (var crit in Criterions)
            {
                crit.Value.Step(JOS.State.t, new Vector<double>[] { JOS.State.Xvec }, new double[] { u, rtt }, JOS.CPObservations.Select(x => x.dN).ToArray());
            }
        }

        public void SaveCriterions(string CritFileNameTemplate)
        {
            foreach (var crit in Criterions)
            {
                crit.Value.SaveTrajectory(CritFileNameTemplate.Replace("{name}", crit.Key));
            }

            foreach (var crit in TerminalCriterions)
                using (System.IO.StreamWriter outputfile = new System.IO.StreamWriter(CritFileNameTemplate.Replace("{name}", crit.Key)))
                {
                    outputfile.WriteLine(crit.Value().ToString());
                    outputfile.Close();
                }
        }

        public Matrix<double> TransitionMatrix(double t, double w)
        {
            double Wmax = 200.0;
            double lambda0 = 1e-5;
            double C = 0.01 * 10.0;

            double lambda14 = 0.01;
            double lambda41 = 0.03;
            double lambda24 = 0.01;
            double lambda42 = 0.005;
            double lambda12 = lambda0 + C / Math.Max(Wmax / 2 - w, C);
            //double lambda12 = 0.01 + 0.001 * w;
            double lambda21 = C / w;
            //double lambda21 = Math.Max(0.01, 0.03 - 0.002 * w);
            double lambda23 = lambda0 + C / Math.Max(Wmax - w, C);
            //double lambda23 = 0.01 + 0.001 * w;
            double lambda32 = C / w;
            //double lambda32 = Math.Max(0.01, 0.04 - 0.003 * w); ;
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
            return Extensions.Diag(P) * R(t, u) * u;
        }


        public Vector<double> TimeoutIntensity(double t, double u)
        {
            return Extensions.Diag(Q) * R(t, u) * u;
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

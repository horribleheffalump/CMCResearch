using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMC;
using SystemJointObs;
using MathNet.Numerics.Distributions;

namespace Channel
{
    public class HMMChannel : TCPChannel
    {
        int N;
        int Others;
        double ContinuousObservationsDiscretizationStep;
        public bool doSimulateSimultaneousJumps;

        public double delta_p; // delta_p - signal propagation time
        public double mV0;     // mean of propagation time additive noise
        public double stdV0;   // standard deviation of propagation time additive noise

        public double delta_p_bad = 0.02;
        public double mV0_bad = 0.001;
        public double stdV0_bad = 0.001;

        public double s;       // coefficient of rtt linear growth with cwnd
        public double sigma;   // coefficient of rtt jitter

        public Vector<double> D;
        public Vector<double> K;
        public Vector<double> Phi;
        public Vector<double> Psi;
        //public Vector<double> P;
        //public Vector<double> Q;


        public JointObservationsSystem JOS;
        public Dictionary<String, Criterion> Criterions;
        public Dictionary<String, Func<double>> TerminalCriterions;

        private double df = 0.0; // number of acks since last moment we've updated rtt
        private double dt = 0.0; // time spent to get at least f_step acks
        int f_step = 10; // akk discretization step


        public double bandwidth; // channel bandwidth Mbps
        public double buffersize; // channel bandwidth Mbps
        public double MTU; // packet size = 1000 bytes
        private double Ubdp; // bandwidth-delay product = maximum window size corresponding to throughput equal to bandwidth
        private double bandwidth_bps; // bandwidth bytes per second
        private double Umax; // maximum possible number of packets in flight (= Ubdp + buffersize)
        private double U1; // AQM lower bound
        private double U2; // AQL upper bound

        private double currentRTT;

        private Normal noise;

        public HMMChannel(double _t0, double _T, double _h, int _saveEvery) : base()
        {
            h = _h;

            N = 4; // states count: e1 - free, e2 - moderate load, e3 - wire congestion, e4 - last mile (wireless) bad signal
            doSimulateSimultaneousJumps = false; // simulating the simultaneous jumps of the Markov chain and the observable counting processes
            Others = 1; // number of other users of the channel (affects the simultaneous jumps intensity)

            // RTT = delta_p + D X_t + w_t K X_t
            delta_p = 0.1;
            mV0 = 0; // delta_p * 0.001;
            stdV0 = delta_p * 0.01;

            delta_p_bad = delta_p * 1.1;
            mV0_bad = 0;
            stdV0_bad = delta_p * 0.02;

            noise = new Normal(0.0, 1.0);

            // TODO : delete these vectors after observations refactoring
            D = Extensions.Vector(0.001, 0.005, 0.01, 0.01); // mean queueing time because of the other senders transmission
            K = Extensions.Vector(0.0005, 0.002, 0.005, 0.005); // mean queueing time because of senders own transmission


            //RTT0 = delta_p + D[0];

            bandwidth = 100; // Mbps
            buffersize = 100; // packets
            MTU = 1000; // bytes

            RTT0 = delta_p; // + mV0;
            currentRTT = RTT0;
            bandwidth_bps = bandwidth * 1000.0 * 1000.0 / 8.0;
            Ubdp = bandwidth_bps * RTT0 / MTU; // since throughput = W * MTU / RTT
            Umax = Ubdp + buffersize;

            U1 = Ubdp + buffersize * 0.4;
            U2 = Ubdp + buffersize * 0.8;

            s = MTU / bandwidth_bps; //0.1 * delta_p / buffersize;
            sigma = Math.Sqrt(s) / 10.0;

            Phi = D * 0.25; // Variance of queueing time because of the other senders transmission
            Psi = K * 0.25; // Variance deviation of queueing time because of senders own transmission

            
            //P = Extensions.Vector(0.00015, 0.001, 0.005, 0.005);
            //P = Extensions.Vector(0.0005, 0.0025, 0.005, 0.05);
            //P = P * 0.1;
            //P = P * 3;
            //P = Vector(0.0, 0.0, 0.0, 0.0);
            //timeout intensity nu_t = R_t diag(Q)
            //Q = P * 0.2;
            //Q = Extensions.Vector(0.0001, 0.0005, 0.0015, 0.01);
            //Q = Q * 0.03;
            //Q = Q * 1;
            //Q = Vector(0.0, 0.0, 0.0, 0.0);

            //ContinuousObservationsDiscretizationStep = _h;
            ContinuousObservationsDiscretizationStep = 1e-2;

            Criterions = new Dictionary<string, Criterion>();

            //Criterions.Add("Throughput", new Criterion(_h, (t, X, U, Obs) =>
            //{
            //    double rtt = U[1];
            //    if (rtt > 0)
            //    {
            //        double Bps = U[0] * MTU / rtt; // instant bytes per second
            //        double Mbps = Bps * 8 / 1e6; // instant Mbps
            //        return Mbps;

            //    }
            //    else
            //        return 0.0;
            //}, _saveEvery));
            //Criterions.Add("TimeInGoodState", new Criterion(_h, (t, X, U, Obs) =>
            //{
            //    return Math.Abs(X[0][0] - 1.0) < 1e-5 ? 1.0 : 0.0;
            //}, _saveEvery));
            //Criterions.Add("TimeInNormState", new Criterion(_h, (t, X, U, Obs) =>
            //{
            //    return Math.Abs(X[0][1] - 1.0) < 1e-5 ? 1.0 : 0.0;
            //}, _saveEvery));
            //Criterions.Add("TimeInBadState", new Criterion(_h, (t, X, U, Obs) =>
            //{
            //    return Math.Abs(X[0][2] - 1.0) < 1e-5 ? 1.0 : 0.0;
            //}, _saveEvery));

            TerminalCriterions = new Dictionary<string, Func<double>>();

            //TerminalCriterions.Add("Loss", () => JOS.CPObservations[0].N);
            //TerminalCriterions.Add("Timeout", () => JOS.CPObservations[1].N);
            //TerminalCriterions.Add("AverageThroughput", () => Criterions["Throughput"].J / JOS.State.T);
            //TerminalCriterions.Add("TotalTimeInGoodState", () => Criterions["TimeInGoodState"].J);
            //TerminalCriterions.Add("TotalTimeInNormState", () => Criterions["TimeInNormState"].J);
            //TerminalCriterions.Add("TotalTimeInBadState", () => Criterions["TimeInBadState"].J);

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

            dt = 0;
            df = 0;
        }

        public override (int loss, int timeout, double? rtt, double? ack_received_count, double? ack_received_time) Step(double u)
        {
            dt += h;
            df += JOS.ContObservations.dx;

            // raw rtt obtained from received acks. We calculate dt - a time spent to get df acks, where df has a minimum of f_step, then calculate rawrtt = df / dt. 
            //if (df > f_step)
            //{
            //    currentRTT = dt / df;
            //    dt = 0;
            //    df = 0;
            //}
            currentRTT = RTT(u)[JOS.State.X];
            JOS.Step(u);

            //CalculateCriterions(u, currentRTT);
            return (JOS.CPObservations[0].dN, JOS.CPObservations[1].dN, currentRTT, null, null); // TODO! FOR RTT OBSERVED
        }

        public Vector<double> RTT(double u) // real unobservable rtt 
        {
            double V0 = noise.Sample() * stdV0 + mV0;
            double V0bad = noise.Sample() * stdV0_bad + mV0_bad;
            double V1 = noise.Sample();
            double buffer_occupied = Math.Max(0.0, u - Ubdp);

            return
                Extensions.Vector(
                            delta_p + V0 + buffer_occupied * s + Math.Sqrt(buffer_occupied) * sigma * V1,
                            delta_p + V0 + buffer_occupied * s + Math.Sqrt(buffer_occupied) * sigma * V1,
                            delta_p + V0 + buffersize * s      + Math.Sqrt(buffersize) * sigma * V1,
                            delta_p_bad + V0bad
                );

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

        public override void SaveAll(string folderName)
        {
            string statepath = folderName + "\\channel_state.txt";
            string cpobspath = folderName + "\\CP_obs_{num}.txt";
            string contobspath = folderName + "\\cont_obs.txt";
            string filterpath = folderName + "\\filter_{name}.txt";
            JOS.SaveAll(statepath, cpobspath, contobspath, filterpath);

            string criterionpath = folderName + "\\crit_{name}.txt";
            SaveCriterions(criterionpath);

        }

        public Matrix<double> TransitionMatrix(double t, double w)
        {
            //double Wmax = 200.0;
            double lambda0 = 1e-5;
            double C = 0.01 * 10.0;

            double lambda14 = 0; /// DO NOT FORGET TO CHANGE IT BACK TO 0.01;
            double lambda41 = 0.03;
            double lambda24 = 0; /// DO NOT FORGET TO CHANGE IT BACK TO0.01;
            double lambda42 = 0.005;
            double lambda12 = lambda0 + (w > Ubdp ? 1e10 : C / Math.Max(Ubdp - w, C));
            //double lambda12 = 0.01 + 0.001 * w;
            double lambda21 = lambda0 + C * Math.Max(Ubdp - w, 0);
            //double lambda21 = Math.Max(0.01, 0.03 - 0.002 * w);
            double lambda23 = lambda0 + (w > U2 ? 1e10 : C / Math.Max(U2 - w, C));
            //double lambda23 = 0.01 + 0.001 * w;
            double lambda32 = lambda0 + C * Math.Max(U2 - w, 0);
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
            double mu_i = LossIntensity(t, u)[i];
            double nu_i = TimeoutIntensity(t, u)[i];
            double lambda_ij = TransitionMatrix(t, u)[i, j];

            if ((mu_i + nu_i) / Others <= lambda_ij)
                return mu_i / Others;
            else
                return (mu_i * lambda_ij) / Others / (mu_i + nu_i);
        }

        public double SimultaneousJumpAndTimeoutIntensity(double t, double u, int i, int j)
        {
            // return 1 / Others * Math.Min(TransitionMatrix(t, u)[i, j], TimeoutIntensity(t, u)[i]);
            double mu_i = LossIntensity(t, u)[i];
            double nu_i = TimeoutIntensity(t, u)[i];
            double lambda_ij = TransitionMatrix(t, u)[i, j];

            if ((mu_i + nu_i) / Others <= lambda_ij)
                return nu_i / Others;
            else
                return (nu_i * lambda_ij) / Others / (mu_i + nu_i);
        }

        public Vector<double> LossIntensity(double t, double u)
        {

            double Pmin = 0.0; //0.00005; // probability of one of unacknowledged packets to be lost on time interval equal to RTT
            double Pmax = 0.0; // 0.0005;
            double Delta = 5;

            var P = Extensions.Vector(
                            Pmin,
                            Pmin + (u > U1 ? (Pmax - Pmin) / (U2 - U1) * (u - U1) : 0.0),
                            Pmax,
                            0.005
                )
                +
                Extensions.Vector(
                            u < U2 + Delta ? Pmin / (U2 - u) : 1e10,
                            u < U2 + Delta ? Pmin / (U2 - u) : 1e10,
                            u < U2 + Delta ? Pmin / (U2 - u) : 1e10,
                            u < U2 + Delta ? Pmin / (U2 - u) : 1e10
                );
            return u * Extensions.Diag(P) * RTT(u).PointwisePower(-1);
        }


        public Vector<double> TimeoutIntensity(double t, double u)
        {
            var Q = Extensions.Vector(0.00001, 0.00001, 0.00001, 0.001)
            //+
            //Extensions.Vector(
            //            u < U2 ? 0 : 1e10,
            //            u < U2 ? 0 : 1e10,
            //            u < U2 ? 0 : 1e10,
            //            u < U2 ? 0 : 1e10
            //)
            ;

            Q = Q * 0.01;

            return u * Extensions.Diag(Q) * RTT(u).PointwisePower(-1);
            //return Extensions.Diag(Q) * R(t, u);
            //return LossIntensity(t, u) / 3.0;
        }

        public Vector<double> R(double t, double u)
        {
            return Extensions.Vector(
                u * 1.0 / (delta_p + D[0] + u * K[0]),
                u * 1.0 / (delta_p + D[1] + u * K[1]),
                u * 1.0 / (delta_p + D[2] + u * K[2]),
                u * 1.0 / (delta_p + D[3] + u * K[3])
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

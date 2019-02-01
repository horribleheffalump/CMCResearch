using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMC;
using SystemJointObs;
using MathNet.Numerics.Distributions;
using System.Globalization;
using System.IO;

namespace Channel
{
    public class CMCChannel : TCPChannel
    {
        int N;
        int Others;
        double ContinuousObservationsDiscretizationStep;
        public bool doSimulateSimultaneousJumps;

        public double delta_p; // delta_p - signal propagation time
        public Vector<double> mV0;     // mean of propagation time additive noise
        public Vector<double> stdV0;   // standard deviation of propagation time additive noise

        //public double delta_p_bad = 0.02;
        //public double mV0_bad = 0.001;
        //public double stdV0_bad = 0.001;

        public double s;       // coefficient of rtt linear growth with cwnd
        public double sigma;   // coefficient of rtt jitter

        //public Vector<double> D;
        //public Vector<double> K;
        //public Vector<double> Phi;
        //public Vector<double> Psi;
        //public Vector<double> P;
        //public Vector<double> Q;


        public JointObservationsSystem JOS;
        public Dictionary<FilterType, Criterion> FilterCriterions;
        public Dictionary<String, Criterion> Criterions;
        public Dictionary<String, Func<double>> TerminalCriterions;

        //private double df = 0.0; // number of acks since last moment we've updated rtt
        //private double dt = 0.0; // time spent to get at least f_step acks
        //int f_step = 10; // akk discretization step


        public double bandwidth; // channel bandwidth Mbps
        public double buffersize; // channel bandwidth Mbps
        public double MTU; // packet size = 1000 bytes
        private double Ubdp; // bandwidth-delay product = maximum window size corresponding to throughput equal to bandwidth
        private double bandwidth_bps; // bandwidth bytes per second
        private double Umax; // maximum possible number of packets in flight (= Ubdp + buffersize)
        private double U1; // AQM lower bound
        private double U2; // AQL upper bound

        private Tuple<double, Vector<double>> temp_possibleRTT;
        private double currentRTT;
        private RTTSimulationMode rttSimulationMode;

        private Normal noise;


        public CMCChannel(double _t0, double _T, double _h, int _saveEvery, bool _evaluatePerformance = false, FilterType[] filters = null, RTTSimulationMode _RTTSimulationMode = RTTSimulationMode.AsRenewal, bool _simultaneousJumps = true) : base()
        {
            h = _h;

            N = 4; // states count: e1 - free, e2 - moderate load, e3 - wire congestion, e4 - last mile (wireless) bad signal
            doSimulateSimultaneousJumps = _simultaneousJumps; // simulating the simultaneous jumps of the Markov chain and the observable counting processes
            Others = 10; // number of other users of the channel (affects the simultaneous jumps intensity)
            rttSimulationMode = _RTTSimulationMode;

            delta_p = 0.1;
            mV0 = delta_p * Extensions.Vector(0.0, 0.0, 0.0, 0.03);
            stdV0 = delta_p * Extensions.Vector(0.003, 0.01, 0.02, 0.02);

            noise = new Normal(0.0, 1.0);

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

            ContinuousObservationsDiscretizationStep = 0.5;
            //ContinuousObservationsDiscretizationStep = _h;

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

                JOS = new JointObservationsSystemSimultaniousJumps(N, _t0, _T, 0, _h, TransitionMatrix, new Func<double, double, Vector<double>>[] { LossIntensity, TimeoutIntensity }, new List<SimultaneousJumpsIntencity>[] { LossSimultaneousIntencity, TimeoutSimultaneousIntencity }, R, G, _saveEvery, ContinuousObservationsDiscretizationStep, filters);

            }
            else
            {
                JOS = new JointObservationsSystem(N, _t0, _T, 0, _h, TransitionMatrix, new Func<double, double, Vector<double>>[] { LossIntensity, TimeoutIntensity }, R, G, _saveEvery, ContinuousObservationsDiscretizationStep, filters);
            }

            //dt = 0;
            //df = 0;

            if (_evaluatePerformance)
            {
                FilterCriterions = new Dictionary<FilterType, Criterion>();
                Criterions = new Dictionary<string, Criterion>();

                //if (JOS.Filters != null)
                //    foreach (var f in JOS.Filters)
                //        FilterCriterions.Add(f.Key, new Criterion(_h, (t, X, pi, u, Obs, p) =>
                //        {
                //            return (X - pi).L2Norm();
                //        }, _saveEvery));

                Criterions.Add("Throughput", new Criterion(_h, (t, X, pi, u, Obs, p) =>
                {
                    double rtt = p[0];
                    if (rtt > 0)
                    {
                        double Bps = Math.Min(u, Ubdp) * MTU / rtt; // instant bytes per second
                        double Mbps = Bps * 8.0 / 1000.0 / 1000.0; // instant Mbps
                        return Mbps;

                    }
                    else
                        return 0.0;
                }, _saveEvery));
                Criterions.Add("TimeInGoodState", new Criterion(_h, (t, X, pi, u, Obs, p) =>
                {
                    return Math.Abs(X[0] - 1.0) < 1e-5 ? 1.0 : 0.0;
                }, _saveEvery));
                Criterions.Add("TimeInNormState", new Criterion(_h, (t, X, pi, u, Obs, p) =>
                {
                    return Math.Abs(X[1] - 1.0) < 1e-5 ? 1.0 : 0.0;
                }, _saveEvery));
                Criterions.Add("TimeInBadWireState", new Criterion(_h, (t, X, pi, u, Obs, p) =>
                {
                    return Math.Abs(X[2] - 1.0) < 1e-5 ? 1.0 : 0.0;
                }, _saveEvery));
                Criterions.Add("TimeInBadWirelessState", new Criterion(_h, (t, X, pi, u, Obs, p) =>
                {
                    return Math.Abs(X[3] - 1.0) < 1e-5 ? 1.0 : 0.0;
                }, _saveEvery));

                TerminalCriterions = new Dictionary<string, Func<double>>();

                foreach (var c in FilterCriterions)
                    TerminalCriterions.Add($"Mean_{c.Key.ToString()}", () => c.Value.J / JOS.State.T);

                foreach (var c in Criterions)
                    TerminalCriterions.Add($"Mean_{c.Key}", () => c.Value.J / JOS.State.T);


                TerminalCriterions.Add("Loss", () => JOS.CPObservations[0].N);
                TerminalCriterions.Add("Timeout", () => JOS.CPObservations[1].N);
                TerminalCriterions.Add("TotalTime", () => JOS.State.T);
                //TerminalCriterions.Add("AverageThroughput", () => Criterions["Throughput"].J / JOS.State.T);
                //TerminalCriterions.Add("TotalTimeInGoodState", () => Criterions["TimeInGoodState"].J / JOS.State.T);
                //TerminalCriterions.Add("TotalTimeInNormState", () => Criterions["TimeInNormState"].J / JOS.State.T);
                //TerminalCriterions.Add("TotalTimeInBadWireState", () => Criterions["TimeInBadWireState"].J / JOS.State.T);
                //TerminalCriterions.Add("TotalTimeInBadWirelessState", () => Criterions["TimeInBadWirelessState"].J / JOS.State.T);
            }

        }

        public override (int loss, int timeout, double rtt) Step(double u)
        {
            // RTTSimulationMode.Explicit  : the RTT i measured explicitly
            // RTTSimulationMode.AsRenewal : the RTT is simulated as time between full window acknowledgements (reception of all packets sent during the last RTT period)
            //                               In this case ack counting is a renewal process and RTT = time interval / counts * cwnd


            switch (rttSimulationMode)
            {
                case RTTSimulationMode.Explicit: currentRTT = PossibleRTT(JOS.State.t, u)[JOS.State.X]; break;
                case RTTSimulationMode.AsRenewal: if (JOS.ContObservations.dx > 0) currentRTT = h / JOS.ContObservations.dx * u; break;
            }



            JOS.Step(u);

            // uncomment to calculate performance criterions
            if (Criterions != null)
                CalculateCriterions(u, currentRTT);

            // if we have explicit RTT observations
            //return (JOS.CPObservations[0].dN, JOS.CPObservations[1].dN, currentRTT, null, null); 
            // if we just have acknowledgement counting process (or its approximation) and use it to estimate RTT
            return (JOS.CPObservations[0].dN, JOS.CPObservations[1].dN, currentRTT);
        }

        public Vector<double> PossibleRTT(double t, double u)
        {
            // real unobservable rtt 

            if (temp_possibleRTT == null)
                temp_possibleRTT = new Tuple<double, Vector<double>>(double.NaN, null);

            if (temp_possibleRTT.Item1 == t)
                return temp_possibleRTT.Item2;
            else
            {
                Vector<double> V0 = noise.Sample() * stdV0 + mV0;
                double V1 = noise.Sample();
                double buffer_occupied = Math.Max(0.0, u - Ubdp);

                Vector<double> rtt = Extensions.Vector(
                                delta_p + V0[0] + buffer_occupied * s + Math.Sqrt(buffer_occupied) * sigma * V1,
                                delta_p + V0[1] + buffer_occupied * s + Math.Sqrt(buffer_occupied) * sigma * V1,
                                delta_p + V0[2] + buffersize * s + Math.Sqrt(buffersize) * sigma * V1,
                                delta_p + V0[3]
                    );

                temp_possibleRTT = new Tuple<double, Vector<double>>(t, rtt);
                return rtt;
            }
        }

        public void CalculateCriterions(double u, double rtt)
        {
            if (FilterCriterions != null && JOS.Filters != null)
            {
                foreach (var fc in FilterCriterions)
                {
                    var estimate = Vector<double>.Build.Dense(N, 0.0);
                    if (JOS.Filters.ContainsKey(fc.Key))
                    {
                        estimate = JOS.Filters[fc.Key].pi;
                    }
                    fc.Value.Step(JOS.State.t, JOS.State.Xvec, estimate, u, JOS.CPObservations.Select(x => (double)x.dN).ToArray(), new double[] { rtt });
                }
            }



            foreach (var crit in Criterions)
            {
                var estimate = Vector<double>.Build.Dense(N, 0.0);
                crit.Value.Step(JOS.State.t, JOS.State.Xvec, estimate, u, JOS.CPObservations.Select(x => (double)x.dN).ToArray(), new double[] { rtt });
            }
        }

        public void SaveCriterions(string CritFileNameTemplate, string TerminalCritFileNameTemplate = null)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";

            if (string.IsNullOrEmpty(TerminalCritFileNameTemplate))
                TerminalCritFileNameTemplate = CritFileNameTemplate;
            if (Criterions != null)
            {
                foreach (var crit in Criterions)
                {
                    crit.Value.SaveTrajectory(CritFileNameTemplate.Replace("{name}", crit.Key));
                }
            }
            if (TerminalCriterions != null)
            {
                foreach (var crit in TerminalCriterions)
                    using (System.IO.StreamWriter outputfile = new System.IO.StreamWriter(TerminalCritFileNameTemplate.Replace("{name}", crit.Key)))
                    {
                        outputfile.WriteLine(crit.Value().ToString(provider));
                        outputfile.Close();
                    }
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
            string terminalcriterionpath = folderName + "\\crit_T_{name}.txt";
            SaveCriterions(criterionpath, terminalcriterionpath);

        }

        public override void SaveMain(string fileName, string[] addHead, string[] addVals)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";

            if (TerminalCriterions != null)
            {
                bool needhead = !File.Exists(fileName);
                using (System.IO.StreamWriter outputfile = new System.IO.StreamWriter(fileName, true))
                {
                    if (needhead)
                    {
                        outputfile.Write(string.Join(" ", TerminalCriterions.Select(c => c.Key)));
                        outputfile.Write(" ");
                        outputfile.Write(string.Join(" ", addHead));
                        outputfile.WriteLine();
                    }

                    outputfile.Write(string.Join(" ", TerminalCriterions.Select(c => c.Value().ToString(provider))));
                    outputfile.Write(" ");
                    outputfile.Write(string.Join(" ", addVals));
                    outputfile.WriteLine();
                    outputfile.Close();
                }
            }

        }

        public Matrix<double> TransitionMatrix(double t, double w)
        {
            //double Wmax = 200.0;
            double lambda0 = 0.01;
            double C = 0.0005;


            // if the transition intensity islambda, then probability of transit during a time interval of length deltaT is equal to p = lambda * deltaT
            // for p to be close to one during RTT0, lambda should be equal to 1/RTT0

            double lambda_guaranteed = 0.5 / RTT0;

            double lambda14 = 0.01; /// DO NOT FORGET TO CHANGE IT BACK TO 0.01;
            double lambda41 = 0.03;
            double lambda24 = 0.01; /// DO NOT FORGET TO CHANGE IT BACK TO0.01;
            double lambda42 = 0.005;
            double lambda34 = 0.01; /// DO NOT FORGET TO CHANGE IT BACK TO 0.01;
            double lambda43 = 0.0; // after wifi restored, we more likely transit to one of the good channel states, since our flow was low while wifi was bad

            double lambda12 = lambda0 + (w > Ubdp ? lambda_guaranteed : C / Math.Max(Ubdp - w, C));
            //double lambda12 = 0.01 + 0.001 * w;
            double lambda21 = lambda0 + C * Math.Max(Ubdp - w, 0);
            //double lambda21 = Math.Max(0.01, 0.03 - 0.002 * w);
            double lambda23 = lambda0 + (w > U2 ? lambda_guaranteed : C / Math.Max(U2 - w, C));
            //double lambda23 = 0.01 + 0.001 * w;
            double lambda32 = lambda0 + C * Math.Max(U2 - w, 0);
            //double lambda32 = Math.Max(0.01, 0.04 - 0.003 * w); ;
            Matrix<double> Lambda = Matrix<double>.Build.DenseOfArray(new[,]{{-(lambda12 + lambda14), lambda12, 0, lambda14 },
                                                                             { lambda21, -(lambda21 + lambda23 + lambda24 ), lambda23, lambda24 },
                                                                             { 0, lambda32, -(lambda32 + lambda34), lambda34 },
                                                                             { lambda41, lambda42, lambda43, -(lambda41 + lambda42 + lambda43) }});
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
            // TODO: Loss and timeout inensities depend on explicitly simulated RTT, even if the simulation mode is set to "Renewal". 
            // The problem is that we have to return a vector of intensities for all the states, but we can only estimate it for the
            // culrrent state if we simulate RTT as time between renewals (ACKs receptions).
            // This does not make a big difference, but looks scruffy :(
            // SOLUTION: this can be helped by substituting u * PossibleRTT(t, u).PointwisePower(-1) with R(t,u)

            double Pmin = 0.000005; // probability of one of unacknowledged packets to be lost on time interval equal to RTT
            double Pmax = 0.00001;
            double Delta = 5;

            // loss probability during h is p = intensity * h
            double p_guaranteed = 0.5; // probability of loss during RTT

            var P = Extensions.Vector(
                            Pmin,
                            Pmin + (u > U1 ? (Pmax - Pmin) / (U2 - U1) * (u - U1) : 0.0),
                            Pmax,
                            0.005
                )
                +
                Extensions.Vector(
                            u < U2 + Delta ? Pmin / (U2 - u) : p_guaranteed,
                            u < U2 + Delta ? Pmin / (U2 - u) : p_guaranteed,
                            u < U2 + Delta ? Pmin / (U2 - u) : p_guaranteed,
                            u < U2 + Delta ? Pmin / (U2 - u) : p_guaranteed
                );

            Vector<double> intensity = null;
            switch (rttSimulationMode)
            {
                case RTTSimulationMode.Explicit: intensity = u * Extensions.Diag(P) * PossibleRTT(t, u).PointwisePower(-1); break;
                case RTTSimulationMode.AsRenewal: intensity = R(t, u) * Extensions.Diag(P); break;
            }
            return intensity;
        }


        public Vector<double> TimeoutIntensity(double t, double u)
        {
            var Q = Extensions.Vector(0.000001, 0.000001, 0.000001, 0.0001)
            //+
            //Extensions.Vector(
            //            u < U2 ? 0 : 1e10,
            //            u < U2 ? 0 : 1e10,
            //            u < U2 ? 0 : 1e10,
            //            u < U2 ? 0 : 1e10
            //)
            ;

            //return u * Extensions.Diag(Q) * PossibleRTT(t, u).PointwisePower(-1);
            //return Extensions.Diag(Q) * R(t, u);
            //return LossIntensity(t, u) / 3.0;

            Vector<double> intensity = null;
            switch (rttSimulationMode)
            {
                case RTTSimulationMode.Explicit: intensity = u * Extensions.Diag(Q) * PossibleRTT(t, u).PointwisePower(-1); break;
                case RTTSimulationMode.AsRenewal: intensity = R(t, u) * Extensions.Diag(Q); break;
            }
            return intensity;

        }

        private double q(double u) // buffer occupied
        {
            return Math.Max(0.0, u - Ubdp);
        }

        public Vector<double> R(double t, double u)
        {
            return
                Extensions.Vector(
                            u * 1.0 / (delta_p + mV0[0] + q(u) * s),
                            u * 1.0 / (delta_p + mV0[1] + q(u) * s),
                            u * 1.0 / (delta_p + mV0[2] + buffersize * s),
                            u * 1.0 / (delta_p + mV0[3])
                );
        }
        public Vector<double> G(double t, double u)
        {
            //var Rt = R(t, u);
            return Extensions.Vector(
               Math.Sqrt(u) * (Math.Sqrt(stdV0[0] * stdV0[0] + q(u) * sigma * sigma) / Math.Pow(delta_p + mV0[0] + q(u) * s, 1.5)),
               Math.Sqrt(u) * (Math.Sqrt(stdV0[1] * stdV0[1] + q(u) * sigma * sigma) / Math.Pow(delta_p + mV0[1] + q(u) * s, 1.5)),
               Math.Sqrt(u) * (Math.Sqrt(stdV0[2] * stdV0[2] + buffersize * sigma * sigma) / Math.Pow(delta_p + mV0[2] + buffersize * s, 1.5)),
               Math.Sqrt(u) * (stdV0[3] / Math.Pow(delta_p + mV0[3], 1.5))
            );



        }



    }
    public enum RTTSimulationMode { Explicit, AsRenewal }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCTools;
using SystemCPObs;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;
using TransmitterModel;
using System.IO;
using System.Globalization;
using System.Threading;

namespace CMCToolsTest
{
    class Program
    {
        static void Main(string[] args)
        {

            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";

            System.IO.StreamWriter outputfile = new System.IO.StreamWriter("..\\..\\..\\output\\temp.txt");
            for (int n = 0; n < 1000; n++)
            {
                double dist = 2.0 / 1000 * n;
                var probs = Channel.Probs(dist);
                outputfile.WriteLine(string.Format(provider, "{0} {1} {2} {3}", dist, probs[0, 0], probs[0, 1], probs[0, 2]));
            }
            outputfile.Close();

            //var M = Matrix<double>.Build;
            //var m = M.DenseOfArray(new[,]{{-1.0,  0.7,  0.3 },
            //                             {  1.0, -2.0,  1.0 },
            //                             {  0.3,  0.7, -1.0 }}); // as TransitionRateMatrix;
            //var res = m.IsTransitionRateMatrix();

            //int N = 100000000;
            //double Np = 0;
            //double p = 1E-6;
            //double[] samples = SystemRandomSource.Doubles(N, RandomSeed.Robust());
            //foreach (double s in samples)
            //{
            //    if (s < p) Np += 1.0;
            //}

            //double p_est = Np / N;

            //var M = Matrix<double>.Build;
            //var m = M.DenseOfArray(new[,]{{-1.0,  0.7,  0.3 },
            //                             {  1.0, -2.0,  1.0 },
            //                             {  0.3,  0.7, -1.0 }}); // as TransitionRateMatrix;

            //var V = Vector<double>.Build;
            //var v = V.DenseOfArray(new double[] {0.00001, 0.00005, 0.99994 });

            //int N = 1000000;
            //double[] Np = { 0, 0, 0 };
            //for(int i = 0; i< N; i++)
            //{
            //    Np[FiniteDiscreteDistribution.Sample(v)] += 1.0;
            //}

            //double p0 = Np[0] / N;
            //double p1 = Np[1] / N;
            //double p2 = Np[2] / N;


            //int t = FiniteDiscreteDistribution.Sample(v);

            var M = Matrix<double>.Build;
            var m = M.DenseOfArray(new[,]{{-1.0,  0.7,  0.3 },
                                         {  1.0, -2.0,  1.0 },
                                         {  0.3,  0.7, -1.0 }}); // as TransitionRateMatrix;

            ControllableMarkovChain CMC = new ControllableMarkovChain(3, 0.0, 100.0, 0, 10E-3, (t, u) => m);
            //Vector<double> U = Vector<double>.Build.DenseOfArray(new[] { 1.0, 1.0, 1.0 });
            Vector<double> C = Vector<double>.Build.DenseOfArray(new[] { 160.0 * 0.01, 160.0 * 0.04, 160.0 * 0.1 }); // 160p/s ~ 2Mbps (MTU = 1500 bytes). Loss: 1%, 4%, 10%
                                                                                                                     //CMC.GetNextState(t => U);
                                                                                                                     //CMC.GetNextState(t => U);
                                                                                                                     //CMC.GetNextState(t => U);
                                                                                                                     //CMC.GenerateTrajectory(t => U);
                                                                                                                     //CMC.SaveTrajectory(Properties.Settings.Default.FilePath);

            //ControllableCountingProcess CP = new ControllableCountingProcess(0.0, 100.0, 0, 10E-3, (t, u) => 1);
            //CP.GenerateTrajectory(t => 0);
            //CP.SaveTrajectory(Properties.Settings.Default.FilePath);

            //CountingProessObservationsSystem CPOS = new CountingProessObservationsSystem(3, 0.0, 10.0, 0, 10e-5, (t) => m, (t) => C);
            //CPOS.GenerateTrajectory((t) => U);
            //CPOS.State.SaveTrajectory(Properties.Settings.Default.MCFilePath);
            //CPOS.Observation.SaveTrajectory(Properties.Settings.Default.CPFilePath);

            double[] U = new[] { 1.0 / 3.0, 1.0 / 3.0, 1.0 / 3.0 };
            double t0 = 0;
            double T = 10.0 * 60.0;
            Coords[] BaseStations = new[] { new Coords(0.1, 0.4), new Coords(0.4, 1.5), new Coords(0.8, 1.0) };
            Coords Pos0 = new Coords(0, 0);
            double h = 10e-2;
            Func<double, Coords> PosDynamics = (t) => new Coords(t / 600.0, 10.0 * t / 600.0 - t * t / 36000.0);

            TestEnvoronment test = new TestEnvoronment(t0, T, h, BaseStations, Pos0, PosDynamics, (t, pi) => U);
            test.UString = "[1/3, 1/3, 1/3]";
            test.Name = "uniform";

            //test.GenerateAndSaveTrajectory();
            test.GenerateSeriesAndSaveCrits(100, 5);

            U = new[] { 1.0, 0.0, 0.0 };
            test.U = (t, pi) => U;
            test.UString = "[1, 0, 0]";
            test.Name = "all_to_0";
            //test.GenerateAndSaveTrajectory();
            test.GenerateSeriesAndSaveCrits(100, 5);

            U = new[] { 0.0, 1.0, 0.0 };
            test.U = (t, pi) => U;
            test.UString = "[0, 1, 0]";
            test.Name = "all_to_1";
            test.GenerateSeriesAndSaveCrits(100, 5);

            U = new[] { 0.0, 0.0, 1.0 };
            test.U = (t, pi) => U;
            test.UString = "[0, 0, 1]";
            test.Name = "all_to_2";
            test.GenerateSeriesAndSaveCrits(100, 5);

            test.U = (t, pi) =>
            {
                double[] U0 = new[] { 0.8, 0.1, 0.1 };
                double[] U1 = new[] { 0.1, 0.8, 0.1 };
                double[] U2 = new[] { 0.1, 0.1, 0.8 };
                double kappa0 = (pi[0].ToRowMatrix() * C)[0];
                double kappa1 = (pi[1].ToRowMatrix() * C)[0];
                double kappa2 = (pi[2].ToRowMatrix() * C)[0];
                if (kappa0 < Math.Min(kappa1, kappa2)) return U0;
                else if (kappa1 < Math.Min(kappa0, kappa2)) return U1;
                else return U2;
            };
            test.UString = "[suboptimal]";
            test.Name = "suboptimal";
            //test.GenerateAndSaveTrajectory();
            test.GenerateSeriesAndSaveCrits(100, 5);



            //int samplesCount = 100;
            //int packCount = 3;

            //AsyncCalculatorPlanner acp = new AsyncCalculatorPlanner(samplesCount, packCount, CriterionsCalculator.Calculate);
            //List<double> J0 = acp.DoCalculate();
            //using (System.IO.StreamWriter critoutputfile = new System.IO.StreamWriter(string.Format(Properties.Settings.Default.CriterionsFilePath, "_uniform"), true))
            //{
            //    foreach (double j in J0)
            //    {
            //        critoutputfile.WriteLine(string.Format(provider, "{0}", j));
            //    }
            //    critoutputfile.Close();
            //}


            //int SamplesCount = 100;

            //for (int i = 0; i < SamplesCount; i++)
            //{
            //    Transmitter tr = new Transmitter(t0, T, new Coords(0, 0), 10e-4, (t) => new Coords(t / 600.0, 10.0 * t / 600.0 - t * t / 36000.0), BaseStations, (t) => U);

            //    tr.GenerateTrajectory(justPath);
            //    J0.Add(tr.Crit.J);

            //    using (System.IO.StreamWriter critoutputfile = new System.IO.StreamWriter(string.Format(Properties.Settings.Default.CriterionsFilePath, "_uniform"), true))
            //    {
            //        critoutputfile.WriteLine(string.Format(provider, "{0}", tr.Crit.J));
            //        critoutputfile.Close();
            //    }
            //    Console.WriteLine(i);
            //}

            ////U = new[] { 1.0, 0.0, 0.0 };
            //List<double> J1 = new List<double>();
            //for (int i = 0; i < SamplesCount; i++)
            //{
            //    Transmitter tr = new Transmitter(t0, T, new Coords(0, 0), 10e-4, (t) => new Coords(t / 600.0, 10.0 * t / 600.0 - t * t / 36000.0), BaseStations, (t) => U);

            //    tr.GenerateTrajectory(justPath);
            //    J1.Add(tr.Crit.J);

            //    using (System.IO.StreamWriter critoutputfile = new System.IO.StreamWriter(string.Format(Properties.Settings.Default.CriterionsFilePath, "_all_to_0"), true))
            //    {
            //        critoutputfile.WriteLine(string.Format(provider, "{0}", tr.Crit.J));
            //        critoutputfile.Close();
            //    }
            //    Console.WriteLine(i);
            //}

            //U = new[] { 0.0, 1.0, 0.0 };
            //List<double> J2 = new List<double>();
            //for (int i = 0; i < SamplesCount; i++)
            //{
            //    Transmitter tr = new Transmitter(t0, T, new Coords(0, 0), 10e-4, (t) => new Coords(t / 600.0, 10.0 * t / 600.0 - t * t / 36000.0), BaseStations, (t) => U);

            //    tr.GenerateTrajectory(justPath);
            //    J2.Add(tr.Crit.J);

            //    using (System.IO.StreamWriter critoutputfile = new System.IO.StreamWriter(string.Format(Properties.Settings.Default.CriterionsFilePath, "_all_to_1"), true))
            //    {
            //        critoutputfile.WriteLine(string.Format(provider, "{0}", tr.Crit.J));
            //        critoutputfile.Close();
            //    }
            //    Console.WriteLine(i);
            //}

            //U = new[] { 0.0, 0.0, 1.0 };
            //List<double> J3 = new List<double>();
            //for (int i = 0; i < SamplesCount; i++)
            //{
            //    Transmitter tr = new Transmitter(t0, T, new Coords(0, 0), 10e-4, (t) => new Coords(t / 600.0, 10.0 * t / 600.0 - t * t / 36000.0), BaseStations, (t) => U);

            //    tr.GenerateTrajectory(justPath);
            //    J3.Add(tr.Crit.J);

            //    using (System.IO.StreamWriter critoutputfile = new System.IO.StreamWriter(string.Format(Properties.Settings.Default.CriterionsFilePath, "_all_to_2"), true))
            //    {
            //        critoutputfile.WriteLine(string.Format(provider, "{0}", tr.Crit.J));
            //        critoutputfile.Close();
            //    }
            //    Console.WriteLine(i);
            //}

        }

    }


    //tr.SaveTrajectory(Properties.Settings.Default.TransmitterFilePath, 100);
    //tr.SaveBaseStations(Properties.Settings.Default.BaseStationsFilePath);

    //    foreach (Channel c in tr.Channels)
    //    {
    //        c.CPOS.SaveAll(
    //                        string.Format(Properties.Settings.Default.MCFilePath, tr.Channels.FindIndex(s => s.BaseStation == c.BaseStation)),
    //                        string.Format(Properties.Settings.Default.CPFilePath, tr.Channels.FindIndex(s => s.BaseStation == c.BaseStation)),
    //                        string.Format(Properties.Settings.Default.FilterFilePath, tr.Channels.FindIndex(s => s.BaseStation == c.BaseStation)),
    //                        100);
    //    }

    public class TestEnvoronment
    {
        public double t0;
        public double T;
        public Coords[] BaseStations;
        public Coords Pos0;
        public Func<double, Coords> PosDynamics;
        public Func<double, Vector<double>[], double[]> U;
        public double h;
        public string Name;
        public string UString;

        public TestEnvoronment(double _t0, double _T, double _h, Coords[] _BaseStations, Coords _Pos0, Func<double, Coords> _PosDynamics, Func<double, Vector<double>[], double[]> _U)
        {
            t0 = _t0;
            T = _T;
            h = _h;
            BaseStations = _BaseStations;
            Pos0 = _Pos0;
            PosDynamics = _PosDynamics;
            U = _U;
        }
        public double Calculate()
        {
            Transmitter tr = new Transmitter(t0, T, Pos0, h, PosDynamics, BaseStations, U, false);
            tr.GenerateTrajectory();
            return tr.Crit.J;
        }

        public void GenerateAndSaveTrajectory(int every = 1)
        {
            Transmitter tr = new Transmitter(t0, T, Pos0, h, PosDynamics, BaseStations, U, true);
            tr.GenerateTrajectory();
            tr.SaveTrajectory(Properties.Settings.Default.TransmitterFilePath, every);
            tr.SaveBaseStations(Properties.Settings.Default.BaseStationsFilePath);

            foreach (Channel c in tr.Channels)
            {
                c.CPOS.SaveAll(
                                string.Format(Properties.Settings.Default.MCFilePath, tr.Channels.FindIndex(s => s.BaseStation == c.BaseStation)),
                                string.Format(Properties.Settings.Default.CPFilePath, tr.Channels.FindIndex(s => s.BaseStation == c.BaseStation)),
                                string.Format(Properties.Settings.Default.FilterFilePath, tr.Channels.FindIndex(s => s.BaseStation == c.BaseStation)), every);
            }

        }

        public void GenerateSeriesAndSaveCrits(int samplesCount, int packCount)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            AsyncCalculatorPlanner acp = new AsyncCalculatorPlanner(samplesCount, packCount, this.Calculate);
            List<double> J = acp.DoCalculate();
            using (System.IO.StreamWriter critoutputfile = new System.IO.StreamWriter(string.Format(Properties.Settings.Default.CriterionsFilePath, "_" + Name + "_" + NowToString(), true)))
            {
                foreach (double j in J)
                {
                    critoutputfile.WriteLine(string.Format(provider, "{0}", j));
                }
                critoutputfile.Close();
            }
            double EJ = J.Average();
            double sJ = Math.Sqrt(J.Sum(j => Math.Pow(j - EJ, 2.0)) / (J.Count - 1));
            using (System.IO.StreamWriter critoutlogfile = new System.IO.StreamWriter(string.Format(Properties.Settings.Default.CriterionsCountLog), true))
            {
                critoutlogfile.WriteLine(string.Format(provider, "{0} {1} {2} {3} {4} {5}", DateTime.Now, Name, UString, EJ, sJ, samplesCount));
                critoutlogfile.Close();
            }

        }

        public static string NowToString()
        {
            string result = string.Format("{0}{1}{2}-{3}{4}{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            return result;
        }
    }
}

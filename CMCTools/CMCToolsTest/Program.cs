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
using Cureos.Numerics.Optimizers;

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

            //var M = Matrix<double>.Build;
            //var m = M.DenseOfArray(new[,]{{-1.0,  0.7,  0.3 },
            //                             {  1.0, -2.0,  1.0 },
            //                             {  0.3,  0.7, -1.0 }}); // as TransitionRateMatrix;

            //ControllableMarkovChain CMC = new ControllableMarkovChain(3, 0.0, 100.0, 0, 10E-3, (t, u) => m);
            //Vector<double> U = Vector<double>.Build.DenseOfArray(new[] { 1.0, 1.0, 1.0 });
            //Vector<double> C = Vector<double>.Build.DenseOfArray(new[] { 160.0 * 0.01, 160.0 * 0.04, 160.0 * 0.1 }); // 160p/s ~ 2Mbps (MTU = 1500 bytes). Loss: 1%, 4%, 10%
            // Vector<double> C = Vector<double>.Build.DenseOfArray(new[] { 160.0 * 0.03, 160.0 * 0.07, 160.0 * 0.15 }); // 160p/s ~ 2Mbps (MTU = 1500 bytes). Loss: 1%, 4%, 10%
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


            //<CSNumerics test>

            //LincoaObjectiveFunctionDelegate F = new LincoaObjectiveFunctionDelegate((n, x, flag) => -(-1 / (x[0] + x[1] + x[2]) - 0.1 * x[0] - 0.01*x[1] - 0.001*x[2]));
            //double[,] a = new double[,] { { 1.0, 1.0, 1.0 }, { -1.0, 0.0, 0.0 }, { 0.0, -1.0, 0.0 }, { 0.0, 0.0, -1.0 } };
            //double[] b = new double[] { 1.0, 0.0, 0.0, 0.0 };

            //Lincoa optimizer = new Lincoa(F, a, b);
            //var result = optimizer.FindMinimum(new[] { 1.0 / 3.0, 1.0 / 3.0, 1.0 / 3.0 });
            //if (result.Status == OptimizationStatus.Normal)
            //{
            //    var res = result.X;
            //}

            // </CSNumerics test>


            // <criterion series calcultion>

            double t0 = 0;
            double T = 10.0 * 60.0;
            //Coords[] BaseStations = new[] { new Coords(0.1, 0.5), new Coords(0.35, 1.7), new Coords(0.8, 1.3) };
            //Coords[] BaseStations = new[] { new Coords(0.1, 0.4), new Coords(0.3, 0.9), new Coords(0.6, 0.6) };
            Coords[] BaseStations = new[] { new Coords(0.1, 0.4), new Coords(0.3, 0.7), new Coords(0.8, 0.2) };
            Coords Pos0 = new Coords(0, 0);
            double h = 10e-3;
            Func<double, Coords> PosDynamics = (t) => new Coords(t / 600.0, 10.0 * t / 600.0 - t * t / 36000.0);
            //Func<double, Vector<double>> Costs = (t) => Vector<double>.Build.DenseOfArray(new[] { 160.0 * 0.01, 160.0 * 0.04, 160.0 * 0.1 }); // costs for transmission in correponding states. Equal for all channels
            //Func<double, Vector<double>> Costs = (t) => Vector<double>.Build.DenseOfArray(new[] { 1 + 0.01, 1 + 0.04, 1 + 0.1 }); // costs for transmission in correponding states. Equal for all channels

            Func<double, Vector<double>> LossPercentage = (t) => Vector<double>.Build.DenseOfArray(new[] { 0.02, 0.05, 0.12 }); // Loss in percents for each state. Equal for all channels
            Func<double, Vector<double>> C = (t) => 160.0 * LossPercentage(t); // Loss intensity for each state. Equal for all channels
            Func<double, Vector<double>> Costs = (t) => LossPercentage(t); // costs for transmission in correponding states. Equal for all channels

            Func<double, Vector<double>[], double[], int[], double> ValueFunction = (t, X, U, Obs) =>
            {
                //double pow = 0.5;
                //double J = Math.Pow(U.Sum(), pow) / (1 - pow);
                //for (int i = 0; i < X.Length; i++)
                //{
                //    J -= (1+Costs(t)).ConjugateDotProduct(X[i]) * U[i] / (1 + Costs(t).Min()) * pow / (1 - pow);
                //}

                //f =@(v)v.^ a / (1 - a); % функция полезности
                //h =@(v, g) f(v) - (a / (1 - a)) * (g / g_min) * v; % целевая функция
                //v_opt =@(g)(g_min./ g).^ (1 / (1 - a)); % оптимальная скорость
                // h_opt =@(g)(g_min./ g).^ (a / (1 - a)); % максимум целевой функции


                //double J = -1.0 / U.Sum();
                //for (int i = 0; i < X.Length; i++)
                //{
                //    J -= Costs(t).ConjugateDotProduct(X[i]) * U[i];
                //}

                double denominator = 0;
                double energylosses = 0;
                for (int i = 0; i < X.Length; i++)
                {
                    denominator += (1 - Costs(t)).ConjugateDotProduct(X[i]) * U[i];
                    energylosses += (1 + Costs(t)).ConjugateDotProduct(X[i]) * U[i];
                }

                double J = 4.0 * (1 - 1 / (1 + denominator)) - energylosses / (1 + Costs(t).Min());


                return J;

            };


            List<Func<double, Vector<double>[], double[], int[], double>> ValueFunctions = new List<Func<double, Vector<double>[], double[], int[], double>>();
            ValueFunctions.Add(ValueFunction);
            ValueFunctions.Add(
                 (t, X, U, Obs) =>
                 {
                     return 160.0 * U.Sum();
                 }
            );

            TestEnvoronment test = new TestEnvoronment(t0, T, h, BaseStations, C, Pos0, PosDynamics, (t, pi, X, dists, dN) => new[] { 1.0 / 3.0, 1.0 / 3.0, 1.0 / 3.0 }, ValueFunctions, false);
            test.GenerateAndSaveValueFunction(1000, 0, 1);

            test.UString = "[1/3; 1/3; 1/3]";
            test.Name = "uniform";
            //test.GenerateAndSaveTrajectory();
            //test.GenerateSeriesAndSaveCrits(20, 5);

            test.U = (t, pi, X, dists, dN) => new[] { 1.0, 0.0, 0.0 };
            test.UString = "[1; 0; 0]";
            test.Name = "all_to_0";
            //test.GenerateAndSaveTrajectory();
            //test.GenerateSeriesAndSaveCrits(20, 5);

            test.U = (t, pi, X, dists, dN) => new[] { 0.0, 1.0, 0.0 };
            test.UString = "[0; 1; 0]";
            test.Name = "all_to_1";
            //test.GenerateAndSaveTrajectory();
            //test.GenerateSeriesAndSaveCrits(20, 5);

            test.U = (t, pi, X, dists, dN) => new[] { 0.0, 0.0, 1.0 };
            test.UString = "[0; 0; 1]";
            test.Name = "all_to_2";
            //test.GenerateAndSaveTrajectory();
            //test.GenerateSeriesAndSaveCrits(20, 5);

            //
            test.U = (t, pi, X, dists, dN) =>
            {
                double[] U = new[] { 0.0, 0.0, 0.0 };
                double sum = dists.Sum(x => 1 / Math.Pow(x + 1.0, 2));
                for (int i = 0; i < dists.Count(); i++)
                {
                    U[i] = 1 / Math.Pow(dists[i] + 1.0, 2) / sum;
                }
                return U;
            };
            test.UString = "[apr prop]";
            test.Name = "a_priori_proportional";
            //test.GenerateAndSaveTrajectory();
            //test.GenerateSeriesAndSaveCrits(20, 5);



            test.U = (t, pi, X, dists, dN) =>
            {
                double[] U0 = new[] { 1.0, 0.0, 0.0 };
                double[] U1 = new[] { 0.0, 1.0, 0.0 };
                double[] U2 = new[] { 0.0, 0.0, 1.0 };
                if (dists[0] < Math.Min(dists[1], dists[2])) return U0;
                else if (dists[1] < Math.Min(dists[0], dists[2])) return U1;
                else return U2;
            };
            test.UString = "[apr conc]";
            test.Name = "a_priori_concentrated";
            //test.GenerateAndSaveTrajectory();
            //test.GenerateSeriesAndSaveCrits(20, 5);

            test.U = (t, pi, X, dists, dN) =>
            {
                double[] U = new[] { 1.0 / 3.0, 1.0 / 3.0, 1.0 / 3.0 };
                if (dN.Sum() > 0)
                {
                    for (int i = 0; i < dN.Count(); i++)
                    {
                        U[i] = (double)dN[i] / (double)dN.Sum();
                    }
                }
                return U;
            };
            test.UString = "[fb prop]";
            test.Name = "feedback_proportional";
            //test.GenerateAndSaveTrajectory();
            //test.GenerateSeriesAndSaveCrits(20, 5);

            test.U = (t, pi, X, dists, dN) =>
            {
                double[] U0 = new[] { 1.0, 0.0, 0.0 };
                double[] U1 = new[] { 0.0, 1.0, 0.0 };
                double[] U2 = new[] { 0.0, 0.0, 1.0 };
                if (dN[0] < Math.Min(dN[1], dN[2])) return U0;
                else if (dN[1] < Math.Min(dN[0], dN[2])) return U1;
                else return U2;
            };
            test.UString = "[fb conc]";
            test.Name = "feedback_concentrated";
            //test.GenerateAndSaveTrajectory();
            //test.GenerateSeriesAndSaveCrits(20, 5);

            ////Vector<double> C = Vector<double>.Build.DenseOfArray(new[] { 160.0 * 0.03, 160.0 * 0.07, 160.0 * 0.15 }); // 160p/s ~ 2Mbps (MTU = 1500 bytes). Loss: 1%, 4%, 10%
            double[,] a = new double[,] { { 1.0, 1.0, 1.0 }, { -1.0, 0.0, 0.0 }, { 0.0, -1.0, 0.0 }, { 0.0, 0.0, -1.0 } };
            //double[] b = new double[] { 1.0, -0.05, -0.05, -0.05 };
            double[] b = new double[] { 1.0, -0.01, -0.01, -0.01 };

            test.doCalculateFilter = true;
            test.h = 10e-4;
            test.U = (t, pi, X, dists, dN) =>
            {
                LincoaObjectiveFunctionDelegate F = new LincoaObjectiveFunctionDelegate((n, u, flag) => -ValueFunction(t, pi, (u as double[]), null));
                Lincoa optimizer = new Lincoa(F, a, b);

                double[] U0 = new[] { 0.90, 0.05, 0.05 };
                double[] U1 = new[] { 0.05, 0.90, 0.05 };
                double[] U2 = new[] { 0.05, 0.05, 0.90 };
                double[] U_start;
                if (dists[0] < Math.Min(dists[1], dists[2])) U_start = U0;
                else if (dists[1] < Math.Min(dists[0], dists[2])) U_start = U1;
                else U_start = U2;

                //var result = optimizer.FindMinimum(new[] { 1.0 / 3.0, 1.0 / 3.0, 1.0 / 3.0 });
                var result = optimizer.FindMinimum(U_start);
                double[] res;// = new[] { 1.0 / 3.0, 1.0 / 3.0, 1.0 / 3.0 };
                if (result.Status == OptimizationStatus.Normal)
                {
                    res = result.X;
                }
                else
                {
                    throw new Exception();
                }
                //double[] res2;
                //double[] res3;

                //double[] U0 = new[] { 0.9, 0.05, 0.05 };
                //double[] U1 = new[] { 0.05, 0.9, 0.05 };
                //double[] U2 = new[] { 0.05, 0.05, 0.9 };
                //double[] U01 = new[] { 1.0, 0.0, 0.0 };
                //double[] U11 = new[] { 0.0, 1.0, 0.0 };
                //double[] U21 = new[] { 0.0, 0.0, 1.0 };
                //double kappa0 = Costs(t).ConjugateDotProduct(pi[0]);// pi[0].ConjugateDotProduct(Costs(t));
                //double kappa1 = Costs(t).ConjugateDotProduct(pi[1]);// pi[1].ConjugateDotProduct(Costs(t));
                //double kappa2 = Costs(t).ConjugateDotProduct(pi[2]);// pi[2].ConjugateDotProduct(Costs(t));
                //if (kappa0 < Math.Min(kappa1, kappa2)) { res2 = U0; res3 = U01; }
                //else if (kappa1 < Math.Min(kappa0, kappa2)) { res2 = U1; res3 = U11; }
                //else { res2 = U2; res3 = U21; }

                //if (res != res2)
                //{
                //    double v1 = ValueFuncion(t, pi, res, null);
                //    double v2 = ValueFuncion(t, pi, res2, null);
                //    double v3 = ValueFuncion(t, pi, res3, null);

                //    return res;
                //}
                return res;
            };
            test.UString = "[suboptimal]";
            test.Name = "suboptimal";
            test.GenerateAndSaveTrajectory(1000);
            //test.GenerateSeriesAndSaveCrits(20, 5);

            test.doCalculateFilter = false;
            b = new double[] { 1.0, 0.0, 0.0, 0.0 };
            test.h = 10e-3;
            test.U = (t, pi, X, dists, dN) =>
            {
                LincoaObjectiveFunctionDelegate F = new LincoaObjectiveFunctionDelegate((n, u, flag) => -ValueFunction(t, X, (u as double[]), null));
                Lincoa optimizer = new Lincoa(F, a, b);

                double[] U0 = new[] { 0.90, 0.05, 0.05 };
                double[] U1 = new[] { 0.05, 0.90, 0.05 };
                double[] U2 = new[] { 0.05, 0.05, 0.90 };
                double[] U_start;
                if (dists[0] < Math.Min(dists[1], dists[2])) U_start = U0;
                else if (dists[1] < Math.Min(dists[0], dists[2])) U_start = U1;
                else U_start = U2;

                //var result = optimizer.FindMinimum(new[] { 1.0 / 3.0, 1.0 / 3.0, 1.0 / 3.0 });
                var result = optimizer.FindMinimum(U_start);
                double[] res;// = new[] { 1.0 / 3.0, 1.0 / 3.0, 1.0 / 3.0 };
                if (result.Status == OptimizationStatus.Normal)
                {
                    res = result.X;
                    for (int i = 0; i < res.Length; i++)
                    {
                        res[i] = Math.Abs(res[i]);
                    }
                }
                else
                {
                    throw new Exception();
                }

                return res;
            };
            test.UString = "[best]";
            test.Name = "best";
            //test.GenerateAndSaveTrajectory();
            //test.GenerateSeriesAndSaveCrits(20, 5);




            // </criterion series calculation>


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
        Func<double, Vector<double>> C; // loss intensities
        public Coords Pos0;
        public Func<double, Coords> PosDynamics;
        public Func<double, Vector<double>[], Vector<double>[], double[], int[], double[]> U;
        public List<Func<double, Vector<double>[], double[], int[], double>> ValueFunctions;
        public double h;
        public string Name;
        public string UString;
        public bool doCalculateFilter;

        public TestEnvoronment(double _t0, double _T, double _h, Coords[] _BaseStations, Func<double, Vector<double>> _C, Coords _Pos0, Func<double, Coords> _PosDynamics, Func<double, Vector<double>[], Vector<double>[], double[], int[], double[]> _U, List<Func<double, Vector<double>[], double[], int[], double>> _ValueFunctions, bool _doCalculateFilter)
        {
            t0 = _t0;
            T = _T;
            h = _h;
            BaseStations = _BaseStations;
            Pos0 = _Pos0;
            PosDynamics = _PosDynamics;
            U = _U;
            ValueFunctions = _ValueFunctions;
            C = _C;
            doCalculateFilter = _doCalculateFilter;
        }
        public double[] Calculate()
        {
            Transmitter tr = new Transmitter(t0, T, Pos0, h, PosDynamics, BaseStations, U, ValueFunctions, C, false);
            tr.GenerateTrajectory(doCalculateFilter);
            List<double> result = tr.Crits.Select(c => c.J).ToList();
            result.Add(tr.Channels.Sum(c => c.CPOS.Observation.N));
            result.Add(tr.Crits.Last().J - 2 * tr.Channels.Sum(c => c.CPOS.Observation.N));

            return result.ToArray();
        }

        public void GenerateAndSaveTrajectory(int every = 1)
        {
            Transmitter tr = new Transmitter(t0, T, Pos0, h, PosDynamics, BaseStations, U, ValueFunctions, C, true);
            tr.GenerateTrajectory(doCalculateFilter);
            tr.SaveTrajectory(Properties.Settings.Default.TransmitterFilePath, every);
            tr.SaveBaseStations(Properties.Settings.Default.BaseStationsFilePath);

            foreach (Channel c in tr.Channels)
            {
                c.CPOS.SaveAll(
                                string.Format(Properties.Settings.Default.MCFilePath, tr.Channels.FindIndex(s => s.BaseStation == c.BaseStation)),
                                string.Format(Properties.Settings.Default.CPFilePath, tr.Channels.FindIndex(s => s.BaseStation == c.BaseStation)),
                                string.Format(Properties.Settings.Default.FilterFilePath, tr.Channels.FindIndex(s => s.BaseStation == c.BaseStation)), every
                                );
            }
            foreach (Criterion c in tr.Crits)
            {
                c.SaveTrajectory(
                                                    string.Format(Properties.Settings.Default.ValueFunctionTrajectory, tr.Crits.FindIndex(s => s == c)), every
                    );
            }
        }

        public void GenerateSeriesAndSaveCrits(int samplesCount, int packCount)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            AsyncCalculatorPlanner acp = new AsyncCalculatorPlanner(samplesCount, packCount, this.Calculate);
            List<double[]> J = acp.DoCalculate();
            using (System.IO.StreamWriter critoutputfile = new System.IO.StreamWriter(string.Format(Properties.Settings.Default.CriterionsFilePath, "_" + Name + "_" + NowToString(), true)))
            {
                foreach (double[] j in J)
                {
                    critoutputfile.WriteLine(string.Join(", ", j.Select(e => string.Format(provider, "{0}", e))));
                }
                critoutputfile.Close();
            }
            double[] EJ = new double[J[0].Length];
            double[] sJ = new double[J[0].Length];
            for (int i = 0; i < J[0].Length; i++)
            {
                EJ[i] = J.Average(j => j[i]);
                sJ[i] = Math.Sqrt(J.Sum(j => Math.Pow(j[i] - EJ[i], 2.0)) / (J.Count - 1));
            }
            using (System.IO.StreamWriter critoutlogfile = new System.IO.StreamWriter(string.Format(Properties.Settings.Default.CriterionsCountLog), true))
            {
                critoutlogfile.WriteLine(string.Format(provider, "{0}, {1}, {2}, {3}, {4}, {5}", DateTime.Now, Name, UString,
                                                            string.Join(", ", EJ.Select(e => string.Format(provider, "{0}", e))),
                                                            string.Join(", ", sJ.Select(e => string.Format(provider, "{0}", e))), samplesCount));
                critoutlogfile.Close();
            }

        }

        public void GenerateAndSaveValueFunction(int samplesCount, double start = 0, double finish = 1)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";

            double step = (finish - start) / (double)samplesCount;
            double[] X1 = new[] { 1.0, 0.0, 0.0 };
            double[] X2 = new[] { 0.0, 1.0, 0.0 };
            double[] X3 = new[] { 0.0, 0.0, 1.0 };
            double[] Z = new[] { 0.0, 0.0, 0.0 };


            using (System.IO.StreamWriter critoutlogfile = new System.IO.StreamWriter(string.Format(Properties.Settings.Default.ValueFunctionForm), false))
            {

                for (int i = 0; i <= samplesCount; i++)
                {
                    double u = start + i * step;
                    critoutlogfile.WriteLine(string.Format(provider, "{0}, {1}, {2}, {3}", u,
                        string.Join(", ", ValueFunctions.Select(vf => string.Format(provider, "{0}", vf(0, new Vector<double>[] { Vector<double>.Build.DenseOfArray(X1), Vector<double>.Build.DenseOfArray(Z), Vector<double>.Build.DenseOfArray(Z) }, new[] { u, 0.0, 0.0 }, null)))),
                        string.Join(", ", ValueFunctions.Select(vf => string.Format(provider, "{0}", vf(0, new Vector<double>[] { Vector<double>.Build.DenseOfArray(X2), Vector<double>.Build.DenseOfArray(Z), Vector<double>.Build.DenseOfArray(Z) }, new[] { u, 0.0, 0.0 }, null)))),
                        string.Join(", ", ValueFunctions.Select(vf => string.Format(provider, "{0}", vf(0, new Vector<double>[] { Vector<double>.Build.DenseOfArray(X3), Vector<double>.Build.DenseOfArray(Z), Vector<double>.Build.DenseOfArray(Z) }, new[] { u, 0.0, 0.0 }, null))))
                        ));
                }
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

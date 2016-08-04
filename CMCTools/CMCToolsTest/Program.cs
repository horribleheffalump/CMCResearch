using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCTools;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;

namespace CMCToolsTest
{
    class Program
    {
        static void Main(string[] args)
        {
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
            Vector<double> U = Vector<double>.Build.DenseOfArray(new[] { 0.0, 0.0, 0.0 });
            //CMC.GetNextState(t => U);
            //CMC.GetNextState(t => U);
            //CMC.GetNextState(t => U);
            CMC.GenerateTrajectory(t => U);
            //CMC.SaveTrajectory(Properties.Settings.Default.FilePath);

            ControllableCountingProcess CP = new ControllableCountingProcess(0.0, 100.0, 0, 10E-3, (t, u) => 1);
            CP.GenerateTrajectory(t => 0);
            CP.SaveTrajectory(Properties.Settings.Default.FilePath);
        }
    }
}

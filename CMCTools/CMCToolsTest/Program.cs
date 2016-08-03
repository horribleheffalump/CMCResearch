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
            var M = Matrix<double>.Build;
            var m = M.DenseOfArray(new[,]{{-1.0,  0.7,  0.3 },
                                         {  1.0, -2.0,  1.0 },
                                         {  0.3,  0.7, -1.0 }}); // as TransitionRateMatrix;
            var res = m.IsTransitionRateMatrix();

            int N = 100000000;
            double Np = 0;
            double p = 1E-6;
            double[] samples = SystemRandomSource.Doubles(N, RandomSeed.Robust());
            foreach (double s in samples)
            {
                if (s < p) Np += 1.0;
            }

            double p_est = Np / N;
        }
    }
}

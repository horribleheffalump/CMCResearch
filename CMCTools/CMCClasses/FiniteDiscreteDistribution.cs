using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Random;

namespace CMCTools
{
    public static class FiniteDiscreteDistribution
    {
        private static double _tolerance = 1E-6;
        public static int Sample(Vector<double> measure)
        {
            if (Math.Abs(measure.Sum() - 1.0) > _tolerance)
            {
                throw new ArgumentException("Sum of probabilities should be equal to 1");
            }
            Vector<double> intervals = Vector<double>.Build.Dense(measure.ToArray());
            //measure.CopyTo(intervals);
            for (int i = 1; i < intervals.Count; i++)
            {
                intervals[i] += intervals[i - 1];
            }

            Random random = new SystemRandomSource(RandomSeed.Robust());
            double sample = random.NextDouble();
            int result = int.MinValue;
            for (int i = 0; i < intervals.Count; i++)
            {
                if (measure[i] < 0)
                    throw new ArgumentException("Probabilities should be positive");
                if (sample < intervals[i])
                {
                    result = i;
                    break;
                }
            }
            return result;
        }
    }
}

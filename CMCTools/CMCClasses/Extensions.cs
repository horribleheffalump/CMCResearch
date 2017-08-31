using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCTools
{
    public static class Extensions
    {
        public static Vector<double> Vector(params double[] val)
        {
            return Vector<double>.Build.Dense(val);
        }

        public static Matrix<double> Diag(params double[] val)
        {
            return Matrix<double>.Build.DenseDiagonal(val.Length, val.Length, (i) => val[i]);
        }

        public static Matrix<double> Diag(Vector<double> val)
        {
            return Matrix<double>.Build.DenseDiagonal(val.Count, val.Count, (i) => val[i]);
        }

        public static Vector<double> Zero(int N)
        {
            return Vector<double>.Build.Dense(N, 0.0);
        }
    }
}

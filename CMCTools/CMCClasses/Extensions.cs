using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMC
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

        public static Matrix<double> Eye(int N)
        {
            return Matrix<double>.Build.Dense(N, N, (ii, jj) => ii == jj ? 1.0 : 0.0);
        }


        public static Vector<double> Stack(this Vector<double> v1, Vector<double> v2)
        {
            Vector<double> result = Vector<double>.Build.Dense(v1.Count + v2.Count);
            for (int i = 0; i < v1.Count; i++)
            {
                result[i] = v1[i];
            }
            for (int i = 0; i < v2.Count; i++)
            {
                result[i + v1.Count] = v2[i];
            }
            return result;
        }

        public static Vector<double> Stack(params Vector<double>[] v)
        {
            Vector<double> result = v[0];
            for (int i = 1; i < v.Length; i++)
            {
                result = result.Stack(v[i]);
            }
            return result;
        }

    }
}

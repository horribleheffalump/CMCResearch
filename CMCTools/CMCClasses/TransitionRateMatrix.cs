using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra.Storage;

namespace CMC
{
    public static class TransitionRateMatrix
    {
        private static double _tolerance = 1E-6;
        public static bool IsTransitionRateMatrix(this Matrix<double> self)
        {
            try
            {
                if (self.RowCount != self.ColumnCount)
                    return false;
                for (int i = 0; i < self.RowCount; i++)
                {
                    if (self[i, i] > 0) return false;
                    double _sumNonDiag = 0;
                    for (int j = 0; j < self.ColumnCount; j++)
                    {
                        if (i != j)
                        {
                            if (self[i, j] < 0) return false;
                            _sumNonDiag += self[i, j];
                        }
                    }
                    if (Math.Abs(self[i, i] + _sumNonDiag) > _tolerance) return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

    }
}


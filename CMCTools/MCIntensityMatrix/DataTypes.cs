using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CMCTools
{
    public class Jump
    {
        public double t;
        public int X;

        public Jump(double _t, int _X)
        {
            t = _t;
            X = _X;
        }

        public override string ToString()
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            string result = string.Format(provider, "{0} {1}", t, X);
            return result;
        }

    }

    public class Estimate
    {
        public double t;
        public Vector<double> pi;

        public Estimate(double _t, Vector<double> _pi)
        {
            t = _t;
            pi = _pi;
        }

        public override string ToString()
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            string piString = pi.ToRowMatrix().ToMatrixString(null, provider).Trim().Replace("  "," ");
            string result = string.Format(provider, "{0} {1} {2}", t, piString, pi.Sum());
            return result;
        }

    }

}

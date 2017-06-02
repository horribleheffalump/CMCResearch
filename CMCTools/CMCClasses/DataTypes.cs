﻿using MathNet.Numerics.LinearAlgebra;
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
        public double u;

        public Estimate(double _t, Vector<double> _pi, double _u)
        {
            t = _t;
            pi = _pi;
            u = _u;

        }

        public override string ToString()
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            string piString = pi.ToRowMatrix().ToMatrixString(null, provider).Trim().Replace("  "," ");
            string result = string.Format(provider, "{0} {1} {2}", t, piString, u);
            return result;
        }

    }

    public class ScalarContinuousState
    {
        public double t;
        public double x;
        public ScalarContinuousState(double _t, double _x)
        {
            t = _t;
            x = _x;
        }

        public override string ToString()
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            string result = string.Format(provider, "{0} {1}", t, x);
            return result;
        }

    }

}

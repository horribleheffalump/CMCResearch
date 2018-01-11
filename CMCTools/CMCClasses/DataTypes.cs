using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CMC
{
    public class Jump
    {
        public double t;
        public int X;
        public bool isSimultaneous;

        public Jump(double _t, int _X, bool _isSimultaneous = false)
        {
            t = _t;
            X = _X;
            isSimultaneous = _isSimultaneous;
        }

        public override string ToString()
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            string result = string.Format(provider, "{0} {1} {2}", t, X, isSimultaneous ? "sim" : "");
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

    public class Control
    {
        public double t;
        public double u;
        public double[] p;

        public Control(double t, double u, params double[] p)
        {
            this.t = t;
            this.u = u;
            this.p = p;
        }

        public override string ToString()
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            string result = string.Format(provider, "{0} {1} {2}", t, u, string.Join(" ", p.Select(e => e.ToString(provider))));
            return result;
        }

    }


    public class SimultaneousJumpsIntencity
    {
        public int From;
        public int To;
        public Func<double, double, double> Intencity;


        public SimultaneousJumpsIntencity(int _from, int _to, Func<double, double, double> _intencity)
        {
            From = _from;
            To = _to;
            Intencity = _intencity;
        }
    }

}

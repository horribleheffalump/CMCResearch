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
        public double[] p;

        public Jump(double t, int X, bool isSimultaneous = false, params double[] p)
        {
            this.t = t;
            this.X = X;
            this.isSimultaneous = isSimultaneous;
            this.p = p;
        }

        public override string ToString()
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            string result = string.Format(provider, "{0} {1} {3} {2}", t, X, isSimultaneous ? "sim" : "", string.Join(" ", p.Select(e => e.ToString(provider))));
            return result;
        }

    }


    public class Estimate
    {
        public double t;
        public Vector<double> pi;
        public double[] p;

        public Estimate(double t, Vector<double> pi, params double[] p)
        {
            this.t = t;
            this.pi = pi;
            this.p = p;
        }

        public override string ToString()
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            string piString = pi.ToRowMatrix().ToMatrixString(null, provider).Trim().Replace("  "," ");
            string result = string.Format(provider, "{0} {1} {2}", t, piString, string.Join(" ", p.Select(e => e.ToString(provider))));
            return result;
        }

    }

    public class ScalarContinuousState
    {
        public double t;
        public double x;
        public double[] p;

        public ScalarContinuousState(double t, double x, params double[] p)
        {
            this.t = t;
            this.x = x;
            this.p = p;
        }

        public override string ToString()
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            string result = string.Format(provider, "{0} {1} {2}", t, x, string.Join(" ", p.Select(e => e.ToString(provider))));
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

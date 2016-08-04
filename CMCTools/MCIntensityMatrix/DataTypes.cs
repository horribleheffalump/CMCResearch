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
        public double X;

        public Jump(double _t, double _X)
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
}

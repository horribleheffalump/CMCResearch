using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using TCPIllinoisAgent;
using MathNet.Numerics.Distributions;

namespace TCPIllinoisTest
{
    class Program
    {
        static void Main(string[] args)
        {
            double h = 1e-3; 
            StringBuilder res = new StringBuilder();
            Sender s = new Sender(h);
            //for (double d = 0; d <= 11; d += 0.01)
            //{
            //    NumberFormatInfo provider = new NumberFormatInfo();
            //    provider.NumberDecimalSeparator = ".";
            //    res.AppendLine(string.Format(provider, "{0} {1} {2}", d, s.alpha(d), s.beta(d)));
            //}

            //File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\alpha_beta.txt"), res.ToString());

            for (double t = 0; t <= 1; t += h)
            {
                double rtt = (100.0 + Normal.Sample(0, 5.0)) / 1000.0;
                double u = s.step(rtt, 0, 0);
                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";
                res.AppendLine(string.Format(provider, "{0} {1} {2} {3} {4} {5}", t, u, rtt, s.T_min, s.T_max, s.d_m));
            }
            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\control.txt"), res.ToString());


        }
    }
}

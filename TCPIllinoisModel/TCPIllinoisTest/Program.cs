using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using TCPIllinoisAgent;
using MathNet.Numerics.Distributions;
using Channel;

namespace TCPIllinoisTest
{
    class Program
    {
        static void Main(string[] args)
        {
            double h = 1e-4;
            double t0 = 0.0;
            double T = 100.0;
            StringBuilder res = new StringBuilder();
            Sender s = new Sender();

            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";

            //for (double d = 0; d <= 11; d += 0.01)
            //{
            //    NumberFormatInfo provider = new NumberFormatInfo();
            //    provider.NumberDecimalSeparator = ".";
            //    res.AppendLine(string.Format(provider, "{0} {1} {2}", d, s.alpha(d), s.beta(d)));
            //}

            //File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\alpha_beta.txt"), res.ToString());

            //for (double t = t0; t <= T; t += h)
            //{
            //    double p_loss = 0.00025;
            //    double p_to = 0.0005;
            //    int dh = ContinuousUniform.Sample(0.0, 1.0) < p_loss * h ? 1 : 0;
            //    int dl = ContinuousUniform.Sample(0.0, 1.0) < p_to * h ? 1 : 0;
            //    double rtt = 0.01;
            //    double u = s.step(h, rtt, dh, dl);
            //    NumberFormatInfo provider = new NumberFormatInfo();
            //    provider.NumberDecimalSeparator = ".";
            //    res.AppendLine(string.Format(provider, "{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}", t, u, rtt, s.T_min, s.T_max, s.d_m, dh, dl, s.SSIndicator, s.W_1));
            //}
            //File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\control.txt"), res.ToString());

            HMMChannel channel = new HMMChannel(t0, T, h, true);
            double rtt = 0.01;
            int dh = 0; int _h = 0;
            int dl = 0; int _l = 0;
            double m = channel.delta_p + channel.D[0];
            rtt = m;
            double f = 0.0;
            double df = 0.0;
            double gamma = 0.1; // exponential smoothing parameter
            int f_step = 10; // akk discretization step
            double dt = 0.0; // time to get f_step acks
            for (double t = t0; t <= T; t += h)
            {
                double u = s.step(h, rtt, dh, dl);
                channel.JOS.Step(u);
                dh = channel.JOS.CPObservations[0].N - _h;
                _h = channel.JOS.CPObservations[0].N;

                dl = channel.JOS.CPObservations[1].N - _l;
                _l = channel.JOS.CPObservations[1].N;

                df = df + (channel.JOS.ContObservations.x - f);
                f = channel.JOS.ContObservations.x;
                dt += h;
                if (df > f_step)
                {
                    m = dt/df;
                    rtt = m + (gamma) * rtt; // exponential smoothing
                    df = 0;
                    dt = 0;
                }
                //m = m - gamma * m * h + gamma * df; // exponential smoothing
                
                res.AppendLine(string.Format(provider, "{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}", t, channel.JOS.State.X, u, dh, dl, df, rtt, f, s.SSIndicator, s.W_1));
            }
            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\control.txt"), res.ToString());


            string statepath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\channel_state.txt");
            string cpobspath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\CP_obs_{num}.txt");
            string contobspath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\cont_obs.txt");
            channel.JOS.SaveAll(statepath, cpobspath, contobspath, 100);

            //File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\control.txt"), res.ToString());


        }
    }
}

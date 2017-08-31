﻿using System;
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
            double T = 50.0;
            //StringBuilder res = new StringBuilder();
            List<string> res = new List<string>();

            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";

            HMMChannel channel = new HMMChannel(t0, T, h, true);
            Sender sender = new Sender(channel.delta_p + channel.D[0]);

            for (double t = t0; t <= T; t += h)
            {
                double rtt = sender.estimateRTT(h, channel.JOS.ContObservations.dx);
                double u = sender.step(h, channel.JOS.CPObservations[0].dN, channel.JOS.CPObservations[1].dN);
                channel.JOS.Step(u);

                res.Add(string.Format(provider, "{0} {1} {2} {3} {4} {5}", t, u, sender.SSIndicator, sender.W_1, sender.rawrtt, sender.rtt));
            }
            //File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\control.txt"), res.ToString());

            int every = 100;
            string statepath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\channel_state.txt");
            string cpobspath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\CP_obs_{num}.txt");
            string contobspath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\cont_obs.txt");
            string filterpath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\filter.txt");
            channel.JOS.SaveAll(statepath, cpobspath, contobspath, filterpath, every);

            string controlpath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\control.txt");
            using (System.IO.StreamWriter outputfile = new System.IO.StreamWriter(controlpath))
            {
                foreach (string str in res.Where((x, i) => i % every == 0))
                {
                    outputfile.WriteLine(str.ToString());
                }
                outputfile.Close();
            }


            //File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\control.txt"), res.ToString());


        }
    }
}

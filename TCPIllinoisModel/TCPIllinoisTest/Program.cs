using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using MathNet.Numerics.Distributions;
using Channel;
using TCPAgent;

namespace TCPIllinoisTest
{
    class Program
    {
        static void Main(string[] args)
        {
            double h = 1e-4;
            double h_write = 1e-1;
            double t0 = 0.0;
            double T = 10.0;
            int saveEvery = 100;
            //StringBuilder res = new StringBuilder();
            List<string> res = new List<string>();

            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";

            HMMChannel channel = new HMMChannel(t0, T, h, saveEvery);
            TCPSender sender;

            try { double t_args = double.Parse(args[1]); T = t_args; }
            catch { }


            string protocol = args[0];
            if (protocol == "ILLINOIS")
                sender = new TCPIllinoisSender(channel.delta_p + channel.D[0], saveEvery);
            else if (protocol == "NEWRENO")
                sender = new TCPNewRenoSender(channel.delta_p + channel.D[0], saveEvery);
            else
                sender = null;


            for (double t = t0; t <= T; t += h)
            {
                double rtt = sender.estimateRTT(h, channel.JOS.ContObservations.dx);
                // to estimate RTT we use continuous observations with the same discretization step as defined for all the system. 
                // but for continuous filters we use thinned continuous observations
                double u = sender.step(h, channel.JOS.CPObservations[0].dN, channel.JOS.CPObservations[1].dN, rtt);
                channel.CalculateCriterions(u, rtt);
                channel.JOS.Step(u);

                //res.Add(string.Format(provider, "{0} {1} {2} {3} {4} {5}", t, u, sender.SSIndicator, sender.W_1, sender.rawrtt, sender.rtt));
                if (t / h_write - Math.Truncate(t / h_write) < h / h_write)
                {
                    Console.WriteLine(t);
                }
            }
            //File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\control.txt"), res.ToString());

            string statepath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\" + protocol + "\\channel_state.txt");
            string cpobspath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\" + protocol + "\\CP_obs_{num}.txt");
            string contobspath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\" + protocol + "\\cont_obs.txt");
            string filterpath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\" + protocol + "\\filter_{name}.txt");
            channel.JOS.SaveAll(statepath, cpobspath, contobspath, filterpath);

            string controlpath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\" + protocol + "\\control.txt");
            sender.SaveTrajectory(controlpath);

            string criterionpath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\" + protocol + "\\crit_{name}.txt");
            channel.SaveCriterions(criterionpath);

            //using (System.IO.StreamWriter outputfile = new System.IO.StreamWriter(controlpath))
            //{
            //    foreach (string str in res.Where((x, i) => i % every == 0))
            //    {
            //        outputfile.WriteLine(str.ToString());
            //    }
            //    outputfile.Close();
            //}


            //File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\control.txt"), res.ToString());


        }
    }
}

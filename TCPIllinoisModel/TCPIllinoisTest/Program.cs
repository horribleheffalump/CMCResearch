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
            double h_write = 1e-0;
            double t0 = 0.0;
            double T = 500.0;
            int saveEvery = 100;


            SimpleChannel channel_i = new SimpleChannel(h, 0.1, 100, 1000, 100); // simple channel RTT = 100ms, Bandwidth = 100Mbps, Packet size = 1000bytes, buffer size = 100
            SimpleChannel channel_nr = new SimpleChannel(h, 0.1, 100, 1000, 100); // simple channel RTT = 100ms, Bandwidth = 100Mbps, Packet size = 1000bytes, buffer size = 100
            TCPSender sender_i = new TCPIllinoisSender(0.1, saveEvery);
            TCPSender sender_nr = new TCPNewRenoSender(0.1, saveEvery);

            for (double t = t0; t <= T; t += h)
            {
                channel_i.Step(sender_i.W);
                sender_i.rtt = channel_i.RTT(sender_i.W);
                sender_i.Step(h, channel_i.LossIndicator(sender_i.W), 0);

                channel_nr.Step(sender_nr.W);
                sender_nr.rtt = channel_nr.RTT(sender_nr.W);
                sender_nr.Step(h, channel_nr.LossIndicator(sender_nr.W), 0);
                if (t / h_write - Math.Truncate(t / h_write) < h / h_write)
                {
                    Console.WriteLine($"{t}");
                }
            }

            string controlpath_i = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\simple_illinois_control.txt");
            sender_i.SaveTrajectory(controlpath_i);

            string controlpath_nr = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\simple_newreno_control.txt");
            sender_nr.SaveTrajectory(controlpath_nr);

            //HMMChannel channel = new HMMChannel(t0, T, h, saveEvery);
            //TCPSender sender;

            //try { double t_args = double.Parse(args[1]); T = t_args; }
            //catch { }


            //string protocol = args[0];
            //if (protocol == "ILLINOIS")
            //    sender = new TCPIllinoisSender(channel.delta_p + channel.D[0], saveEvery);
            //else if (protocol == "NEWRENO")
            //    sender = new TCPNewRenoSender(channel.delta_p + channel.D[0], saveEvery);
            //else
            //    sender = null;


            //for (double t = t0; t <= T; t += h)
            //{
            //    double rtt = sender.estimateRTT(h, channel.JOS.ContObservations.dx);
            //    // to estimate RTT we use continuous observations with the same discretization step as defined for all the system. 
            //    // but for continuous filters we use thinned continuous observations
            //    double u = sender.step(h, channel.JOS.CPObservations[0].dN, channel.JOS.CPObservations[1].dN, rtt);
            //    channel.CalculateCriterions(u, rtt);
            //    channel.JOS.Step(u);

            //    if (t / h_write - Math.Truncate(t / h_write) < h / h_write)
            //    {
            //        Console.WriteLine(t);
            //    }
            //}

            //string statepath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\" + protocol + "\\channel_state.txt");
            //string cpobspath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\" + protocol + "\\CP_obs_{num}.txt");
            //string contobspath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\" + protocol + "\\cont_obs.txt");
            //string filterpath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\" + protocol + "\\filter_{name}.txt");
            //channel.JOS.SaveAll(statepath, cpobspath, contobspath, filterpath);

            //string controlpath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\" + protocol + "\\control.txt");
            //sender.SaveTrajectory(controlpath);

            //string criterionpath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\" + protocol + "\\crit_{name}.txt");
            //channel.SaveCriterions(criterionpath);
        }
    }
}

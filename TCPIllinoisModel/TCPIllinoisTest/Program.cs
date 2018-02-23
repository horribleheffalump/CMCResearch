using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using MathNet.Numerics.Distributions;
using Channel;
using TCPAgent;
using MathNet.Numerics.LinearAlgebra;
using CMC;

namespace TCPIllinoisTest
{
    class Program
    {
        static void Main(string[] args)
        {
            double h = 1e-4;
            double h_write = 1e-1;
            double t0 = 0.0;
             double T = 200.0;
            int saveEvery = 100;

            //TCPChannel channel_i = new SimpleChannel(h, 0.1, 100, 1000, 100); // simple channel RTT = 100ms, Bandwidth = 100Mbps, Packet size = 1000bytes, buffer size = 100
            //TCPChannel channel_nr = new SimpleChannel(h, 0.1, 100, 1000, 100); // simple channel RTT = 100ms, Bandwidth = 100Mbps, Packet size = 1000bytes, buffer size = 100
            //TCPSender sender_i =llinoisSender(0.1, saveEvery);
            //TCPSender sender_nr = new NewRenoSender(0.1, saveEvery);

            //for (double t = t0; t <= T; t += h)
            //{
            //    (int loss_i, int timeout_i, double? rtt_i, double? ack_received_count_i, double? ack_received_time_i) = channel_i.Step(sender_i.W);
            //    sender_i.Step(h, loss_i, timeout_i, rtt_i.Value);

            //    (int loss_nr, int timeout_nr, double? rtt_nr, double? ack_received_count_nr, double? ack_received_time_nr) = channel_nr.Step(sender_nr.W);
            //    sender_nr.Step(h, loss_nr, timeout_nr, rtt_nr.Value);
            //    if (t / h_write - Math.Truncate(t / h_write) < h / h_write)
            //    {
            //        Console.WriteLine($"{t}");
            //    }
            //}

            //string controlpath_i = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\simple_illinois_control.txt");
            //sender_i.SaveTrajectory(controlpath_i);

            //string controlpath_nr = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\simple_newreno_control.txt");
            //sender_nr.SaveTrajectory(controlpath_nr);

            TCPChannel channel = new HMMChannel(t0, T, h, saveEvery, true);
            TCPSender sender;

            string protocol = "ILLINOIS";
            try
            {
                protocol = args[0];
                try
                {

                    double t_args = double.Parse(args[1]); T = t_args;
                }
                catch { }
            }
            catch { }


            if (protocol == "ILLINOIS")
                sender = new IllinoisSender(channel.RTT0, saveEvery);
            else if (protocol == "NEWRENO")
                sender = new NewRenoSender(channel.RTT0, saveEvery);
            else
                sender = null;

            sender.W = 1300;
            for (double t = t0; t <= T; t += h)
            {
                (int loss, int timeout, double rtt) = channel.Step(sender.W);
                sender.Step(h, loss, timeout, rtt);

                if (t / h_write - Math.Truncate(t / h_write) < h / h_write)
                {
                    Console.WriteLine(t);
                }
            }


            string folderName = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\" + protocol);
            channel.SaveAll(folderName);
            //string statepath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\" + protocol + "\\channel_state.txt");
            //string cpobspath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\" + protocol + "\\CP_obs_{num}.txt");
            //string contobspath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\" + protocol + "\\cont_obs.txt");
            //string filterpath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\" + protocol + "\\filter_{name}.txt");
            //channel.JOS.SaveAll(statepath, cpobspath, contobspath, filterpath);
            //string criterionpath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\" + protocol + "\\crit_{name}.txt");
            //channel.SaveCriterions(criterionpath);

            string controlpath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\" + protocol + "\\control.txt");
            sender.SaveTrajectory(controlpath);

        }
    }
}

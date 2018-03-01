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
using PythonInteract;
using CMC.Filters;
using SystemJointObs;

namespace TCPIllinoisTest
{
    class Program
    {
        static void Main(string[] args)
        {
            double h = 1e-4;
            double h_write = 1e-0;
            double t0 = 0.0;
            double T = 200.0;
            int saveEvery = 0;

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


            string protocol = "CUBIC";
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

            if (protocol == "stats")
            {
                string response = Python.RunScript(
                                    Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\OutputScripts\\", "performance.py"),
                                    new string[] { Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\"), "ILLINOIS", "NEWRENO", "STATEBASED", "CUBIC" }
                                );
                Console.WriteLine(response);
            }
            else
            {

                FilterType[] filters = null;
                if (protocol == "STATEBASED")
                    filters = new FilterType[] { FilterType.DiscreteContinuousGaussian };

                TCPChannel channel = new HMMChannel(t0, T, h, saveEvery, true, filters);
                TCPSender sender;

                double exponential_smooth = 0.99999;
                switch (protocol)
                {
                    case "ILLINOIS": sender = new IllinoisSender(channel.RTT0, exponential_smooth, saveEvery); break;
                    case "NEWRENO": sender = new NewRenoSender(channel.RTT0, exponential_smooth, saveEvery); break;
                    case "STATEBASED": sender = new StateBasedTCPSender(channel.RTT0, exponential_smooth, saveEvery); break;
                    case "CUBIC": sender = new CUBICTCPSender(channel.RTT0, exponential_smooth, saveEvery); break;
                    default: sender = null; break;

                }

                sender.W = 1300;
                //sender.W = 10;
                for (double t = t0; t <= T; t += h)
                {
                    (int loss, int timeout, double rtt) = channel.Step(sender.W);
                    if (protocol == "STATEBASED")
                    {
                        (sender as StateBasedTCPSender).Step(h, loss, timeout, rtt, (channel as HMMChannel).JOS.Filters[FilterType.DiscreteContinuousGaussian].pi);
                    }
                    else
                        sender.Step(h, loss, timeout, rtt);

                    if (t / h_write - Math.Truncate(t / h_write) < h / h_write)
                    {
                        Console.WriteLine(t);
                    }
                }


                string folderName = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\" + protocol);
                channel.SaveAll(folderName);

                string controlpath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\" + protocol + "\\control.txt");
                sender.SaveTrajectory(controlpath);
            }

        }
    }
}

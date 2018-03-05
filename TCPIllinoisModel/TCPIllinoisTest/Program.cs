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
            double T = 600.0;
            int saveEvery = 0;
            double exponential_smooth = 0.99999;

            //// simple channel 
            //exponential_smooth = 0; // rtt is known exactly for simple channel, so there is no need to smooth
            //double RTT0 = 0.1;  // RTT = 100ms
            //double bandwidth = 100.0; // Bandwidth = 100Mbps
            //double MTU = 1000.0; // Packet size = 1000bytes
            //int buffersize = 100; // buffer size = 100
            //TCPChannel channel_i = new SimpleChannel(h, RTT0, bandwidth, MTU, buffersize);
            //TCPChannel channel_nr = new SimpleChannel(h, RTT0, bandwidth, MTU, buffersize);
            //TCPChannel channel_sb = new SimpleChannel(h, RTT0, bandwidth, MTU, buffersize);
            //TCPSender sender_i = new IllinoisSender(RTT0, exponential_smooth, saveEvery);
            //TCPSender sender_nr = new NewRenoSender(RTT0, exponential_smooth, saveEvery);
            //TCPSender sender_sb = new StateBasedSender(RTT0, exponential_smooth, saveEvery);

            //double bandwidth_bps = bandwidth * 1000.0 * 1000.0 / 8.0;
            //double Ubdp = bandwidth_bps * RTT0 / MTU; // since throughput = W * MTU / RTT
            //double U2 = Ubdp + buffersize * 0.8;

            //for (double t = t0; t <= T; t += h)
            //{
            //    (int loss_i, int timeout_i, double? rtt_i) = channel_i.Step(sender_i.W);
            //    sender_i.Step(h, loss_i, timeout_i, rtt_i.Value);

            //    (int loss_nr, int timeout_nr, double? rtt_nr) = channel_nr.Step(sender_nr.W);
            //    sender_nr.Step(h, loss_nr, timeout_nr, rtt_nr.Value);

            //    (int loss_sb, int timeout_sb, double? rtt_sb) = channel_sb.Step(sender_sb.W);
            //    Vector<double> pi = Extensions.Zero(4);
            //    if (sender_sb.W <= Ubdp) pi[0] = 1.0;
            //    else if (sender_sb.W <= U2) pi[1] = 1.0;
            //    else pi[2] = 1.0;
            //    (sender_sb as StateBasedSender).Step(h, loss_sb, timeout_sb, rtt_sb.Value, pi);

            //    if (t / h_write - Math.Truncate(t / h_write) < h / h_write)
            //    {
            //        Console.WriteLine($"{t}");
            //    }
            //}

            //string controlpath_i = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\simple_illinois_control.txt");
            //sender_i.SaveTrajectory(controlpath_i);

            //string controlpath_nr = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\simple_newreno_control.txt");
            //sender_nr.SaveTrajectory(controlpath_nr);

            //string controlpath_sb = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\out\\simple_statebased_control.txt");
            //sender_sb.SaveTrajectory(controlpath_sb);


            string protocol = "CUBIC";
            try
            {
                protocol = args[0];
                try
                {
                    double t_args = double.Parse(args[1]); T = t_args;
                }
                catch { }
                try
                {
                    int saveevery_args = int.Parse(args[2]); saveEvery = saveevery_args;
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
                //filters = new FilterType[] { FilterType.Discrete, FilterType.DiscreteContinuousGaussian };

                if (protocol == "STATEBASED")
                    filters = new FilterType[] { FilterType.DiscreteContinuousGaussian };

                TCPChannel channel = new HMMChannel(t0, T, h, saveEvery, true, filters);
                TCPSender sender;


                switch (protocol)
                {
                    case "ILLINOIS": sender = new IllinoisSender(channel.RTT0, exponential_smooth, saveEvery); break;
                    case "NEWRENO": sender = new NewRenoSender(channel.RTT0, exponential_smooth, saveEvery); break;
                    case "STATEBASED": sender = new StateBasedSender(channel.RTT0, exponential_smooth, saveEvery); break;
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
                        (sender as StateBasedSender).Step(h, loss, timeout, rtt, (channel as HMMChannel).JOS.Filters[FilterType.DiscreteContinuousGaussian].pi);
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

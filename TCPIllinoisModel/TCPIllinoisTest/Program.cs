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
using MathNet.Numerics.Distributions;
using CommandLine;
using System.Text.RegularExpressions;

namespace TCPIllinoisTest
{
    class Program
    {
        public class Options
        {
            [Option('p', "protocol", Required = true, HelpText = "Protocol name, one of ILLINOIS, NEWRENO, STATEBASED, STATEBASED_RAND, CUBIC")]
            public string Protocol { get; set; }

            [Option('T', "upper-bound", Required = true, HelpText = "The upper bound of the observation interval")]
            public double T { get; set; }

            [Option('h', "step", Required = false, Default =1e-3, HelpText = "Discretization step")]
            public double h { get; set; }

            [Option("inform-period", Required = false, Default= 1e0, HelpText = "Write to console period")]
            public double h_write { get; set; }

            [Option("save-every", Required = false, Default = 1000, HelpText = "How often the sample path should be saved")]
            public int saveEvery { get; set; }

            [Option("amax", Required = false, HelpText = "For STATEBASED protocol defines the value or the distribution of the upper bound of the additive increase parameter, e.g. 10.0, N(10,1), U(9.0, 10.0)")]
            public string amax { get; set; }

            [Option("amin", Required = false, HelpText = "For STATEBASED protocol defines the value or the distribution of the lower bound of the additive increase parameter, e.g. 0.3, N(0.3,0.1), U(0, .5)")]
            public string amin { get; set; }

            [Option("bmax", Required = false, HelpText = "For STATEBASED protocol defines the value or the distribution of the upper bound of the multiplicative decrease parameter, e.g. 0.5, N(0.5,0.1), U(.4, .6)")]
            public string bmax { get; set; }

            [Option("bmin", Required = false, HelpText = "For STATEBASED protocol defines the value or the distribution of the lower bound of the multiplicative decrease parameter, e.g. 0.125, N(0.125,0.025), U(.0, .25)")]
            public string bmin { get; set; }

            [Option('o', "output-folder", Required = false, HelpText = "Folder to store the results")]
            public string OutputFolder { get; set; }

            [Option('f', "output-file", Required = false, HelpText = "File to store the main results. If set, it is the only output.")]
            public string OutputFile { get; set; }


        }

        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(opts => Run(opts, args));
        }
        static void Run(Options o, string[] args)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";

            double h = o.h;
            double h_write = o.h_write;
            double t0 = 0.0;

            double exponential_smooth = 0.9999;
            //double exponential_smooth = 0.99;

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



            if (o.Protocol == "stats")
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
                //filters = new FilterType[] { FilterType.DiscreteContinuousGaussian, FilterType.Discrete };

                if (o.Protocol == "STATEBASED" || o.Protocol == "STATEBASED_RAND")
                    filters = new FilterType[] { FilterType.DiscreteContinuousGaussian }; // , FilterType.DiscreteIndependent, FilterType.Discrete, FilterType.Dummy 

                TCPChannel channel = new CMCChannel(0, o.T, h, o.saveEvery, true, filters, RTTSimulationMode.AsRenewal, false); // set last param to false for no simult jumps
                TCPSender sender;


                //double alpha_max = Math.Abs(new Normal(10.0, 1.0).Sample());
                //double alpha_min = Math.Abs(new Normal(0.1, 0.1).Sample());
                //double beta_max = Math.Abs(new Normal(0.5, 0.1).Sample());
                //double beta_min = Math.Abs(new Normal(0.125, 0.1).Sample());

                //double alpha_max = new ContinuousUniform(0.0, 10.0).Sample();
                //double alpha_min = new ContinuousUniform(0.0, alpha_max).Sample();
                //double beta_max = new ContinuousUniform(0.0, 1.0).Sample();
                //double beta_min = new ContinuousUniform(0.0, beta_max).Sample();

                double alpha_max = new ScalarDistribution(o.amax).Sample();
                double alpha_min = new ScalarDistribution(o.amin).Sample();
                double beta_max = new ScalarDistribution(o.bmax).Sample();
                double beta_min = new ScalarDistribution(o.bmin).Sample();

                if ((alpha_max < 0) || (alpha_min < 0) || (beta_max < 0) || (beta_min < 0) || (alpha_max < alpha_min) || (beta_max < beta_min))
                {
                    throw new ArgumentException($"Bad STATEBASED parameters: alpha_max = {alpha_max}, alpha_max = {alpha_min}, beta_max = {beta_max}, beta_min = {beta_min}");
                }

                switch (o.Protocol)
                {
                    case "ILLINOIS": sender = new IllinoisSender(channel.RTT0, exponential_smooth, o.saveEvery); break;
                    case "NEWRENO": sender = new NewRenoSender(channel.RTT0, exponential_smooth, o.saveEvery); break;
                    case "STATEBASED": sender = new StateBasedSender(channel.RTT0, exponential_smooth, o.saveEvery); break;
                    case "STATEBASED_RAND": sender = new StateBasedSender(channel.RTT0, exponential_smooth, alpha_min, alpha_max, beta_min, beta_max, o.saveEvery); break;
                    case "CUBIC": sender = new CUBICTCPSender(channel.RTT0, exponential_smooth, o.saveEvery); break;
                    default: sender = null; break;

                }


                DateTime start = DateTime.Now;
                sender.W = 1300;
                //sender.W = 10;
                for (double t = t0; t <= o.T; t += h)
                {
                    (int loss, int timeout, double rtt) = channel.Step(sender.W);
                    if (o.Protocol == "STATEBASED" || o.Protocol == "STATEBASED_RAND")
                    {
                        (sender as StateBasedSender).Step(h, loss, timeout, rtt, (channel as CMCChannel).JOS.Filters[FilterType.DiscreteContinuousGaussian].pi);
                    }
                    else
                        sender.Step(h, loss, timeout, rtt);

                    if (t / h_write - Math.Truncate(t / h_write) < h / h_write)
                    {
                        if (t > 0)
                        {
                            var elapsed = DateTime.Now - start;
                            Console.WriteLine($"{t.ToString("F3")}\t elapsed: {elapsed}\t estimated finish time: {DateTime.Now + TimeSpan.FromTicks((long)(elapsed.Ticks * (o.T - t) / t))}");
                        }
                    }
                }



                if (string.IsNullOrEmpty(o.OutputFile))
                {
                    string folderName = Path.Combine(Environment.CurrentDirectory, o.OutputFolder + "\\" + o.Protocol);
                    if (o.Protocol == "STATEBASED_RAND")
                    {
                        folderName = folderName + $"_{alpha_min}_{alpha_max}_{beta_min}_{beta_max}";
                    }
                    folderName = folderName + $"_{Guid.NewGuid()}";

                    string controlpath = Path.Combine(Environment.CurrentDirectory, folderName + "\\control.txt");

                    if (!Directory.Exists(folderName))
                    {
                        Directory.CreateDirectory(folderName);
                    }

                    channel.SaveAll(folderName);
                    sender.SaveTrajectory(controlpath);
                }
                else
                {
                    channel.SaveMain(o.OutputFile, new string[] { "protocol", "alpha_min", "alpha_max", "beta_min", "beta_max" }, new string[] { o.Protocol, alpha_min.ToString(provider), alpha_max.ToString(provider), beta_min.ToString(provider), beta_max.ToString(provider) });
                }
            }

        }
    }

    public class ScalarDistribution
    {
        private string pattern = "[NU][(](([0-9]*[.])?[0-9]+[, ]*){2,2}[)]";
        private string floatpattern = "([0-9]*[.])?[0-9]+";
        private IContinuousDistribution distr;
        private double value;
        private string error = "Input string should match one of the patterns: float, N(float, float) or U(float, float)";
        bool isStochastic = true;
        public ScalarDistribution(string str)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";

            if (double.TryParse(str, NumberStyles.AllowDecimalPoint, provider, out value))
            {
                isStochastic = false;
            }
            else
            {
                Regex regexFull = new Regex(pattern);
                if (regexFull.IsMatch(str))
                {
                    Regex regexFloat = new Regex(floatpattern);
                    var floats = regexFloat.Matches(str);
                    var a = double.Parse(floats[0].Value, NumberStyles.AllowDecimalPoint, provider);
                    var b = double.Parse(floats[1].Value, NumberStyles.AllowDecimalPoint, provider);
                    switch (str[0])                        
                    {
                        case 'N': distr = new Normal(a, b); break;
                        case 'U': distr = new ContinuousUniform(a, b); break;
                        default: throw new ArgumentException(error);
                    }
                }
                else
                {
                    throw new ArgumentException(error);
                }
            }
        }
        public double Sample()
        {
            if (isStochastic)
            {
                return distr.Sample();
            }
            else
            {
                return value;
            }
        }
    }
}

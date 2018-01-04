using CMCTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPIllinoisAgent
{
    public class Sender
    {
        double W_0 = 1;                     // min windiw size
        public double W_1 = 0;                    // slow start -> congestion avoidance threshold
        double W_max = 1000;              // maximum window size (bottleneck threshhold)
        double W;                          // ccurrent window size
        double alpha_ss = 1;            // growth coefficient in slow start
        double beta_ss = 0.5;           // denominator in slow start

        public double rawrtt;   // row rtt obtained from received acks. We calculate dt - a time spent to get df acks, where df has a minimum of f_step, then calculate rawrtt = df / dt. 
        public double rtt;      // rtt estimate obtained from rawrtt data by means of ecpontntial smoothing
        double gamma = 0.9; // exponential smoothing parameter
        int f_step = 10; // akk discretization step
        double df = 0.0; // number of acks since last moment we've updated rtt
        double dt = 0.0; // time spent to get at least f_step acks
        double t = 0.0; // current time
        public int SaveEvery;
        private int saveCounter = 0;

        public double T_min { get; set; } // min RTT in session 
        public double T_max { get; set; } // max RTT in session 

        public List<Control> controls;
        //double h; //discretization step

        public Sender(double _rawrtt, int _saveEvery = 0) // parameters: start point for RTT estimation
        {
            //h = _h;
            W = W_0;
            T_min = double.NaN;
            T_max = double.NaN;
            rawrtt = _rawrtt;
            rtt = rawrtt;
            SaveEvery = _saveEvery;

            controls = new List<Control>();
        }

        public int SSIndicator
        {
            get { return W < W_1 ? 1 : 0; }
        }


        public double estimateRTT(double h, double _df) //parameters: time increment, acks received increment. Returns exponential smooth estimate of RTT
        {
            dt += h;
            df += _df;
            if (df > f_step)
            {
                rawrtt = dt / df;
                rtt = (1 - gamma) * rawrtt + (gamma) * rtt; // exponential smoothing
                df = 0;
                dt = 0;
            }
            return rtt;
        }

        public double step(double h, int dh, int dl, double Rtt = double.NaN) //parameters: time increment, RTT, loss increment, timeout increment; returns: current control (window size)
        {
            t += h;
            if (double.IsNaN(Rtt)) Rtt = rtt;

            if (double.IsNaN(T_min) || double.IsNaN(T_max))
            {
                T_min = Rtt;
                T_max = Rtt;
            }
            T_min = Math.Min(T_min, Rtt);
            T_max = Math.Max(T_max, Rtt);

            double d = Rtt - T_min;

            W = W
                + SSIndicator * alpha_ss * W * h //slow start additive increase 
                + (1 - SSIndicator) * alpha(d) / W * h //congestion avoidance additive increase
                - SSIndicator * beta_ss * W * dh //multiple decrease when loss occurs in slow start
                - (1 - SSIndicator) * beta(d) * W * dh; //multiple decrease when loss occurs in congavoid
            W = Math.Min(Math.Max(W, W_0), W_max); // make W in [W_0, W_max]

            if (dl > 0)
            {
                W_1 = W / 2;
                W = W_0; // setting W_1 equal to W/2 and entering the SS phase when timout occurs
            }

            Save();
            return W;
        }

        private void Save()
        {
            saveCounter++;
            if (SaveEvery > 0 && saveCounter % SaveEvery == 0)
            {
                var control = new Control(t, W, SSIndicator, W_1, rawrtt, rtt);
                controls.Add(control);
            }
        }

        public void SaveTrajectory(string path) //, int every)
        {
            using (System.IO.StreamWriter outputfile = new System.IO.StreamWriter(path))
            {
                //foreach (Estimate e in estimates.Where((x, i) => i % every == 0).OrderBy(s => s.t))
                foreach (Control e in controls.OrderBy(s => s.t))
                {
                    outputfile.WriteLine(e.ToString());
                }
                outputfile.Close();
            }
        }




        public double d_m   // maximum average queueing delay = T_max-T_min
        {
            get
            {
                return T_max - T_min;
            }
        }


        // coefficients for alpha(d), beta(d)
        double alpha_max = 10;
        double alpha_min = 0.1;
        double beta_max = 0.5;
        double beta_min = 0.125;
        double eta_1 = 0.01;
        double eta_2 = 0.1;
        double eta_3 = 0.8;

        double d_1 { get { return d_m * eta_1; } }
        double d_2 { get { return d_m * eta_2; } }
        double d_3 { get { return d_m * eta_3; } }

        double kappa_1
        {
            get { return (d_m - d_1) * alpha_min * alpha_max / (alpha_max - alpha_min); }
        }
        double kappa_2
        {
            get { return (d_m - d_1) * alpha_min / (alpha_max - alpha_min) - d_1; }
        }
        double kappa_3
        {
            get { return (beta_min * d_3 - beta_max * d_2) / (d_3 - d_2); }
        }
        double kappa_4
        {
            get { return (beta_max - beta_min) / (d_3 - d_2); }
        }

        public double alpha(double d)
        {
            if (d <= d_1)
                return alpha_max;
            else
                return kappa_1 / (kappa_2 + d);
        }

        public double beta(double d)
        {
            if (d <= d_2)
                return beta_min;
            else if (d_2 < d && d <= d_3)
                return kappa_3 + kappa_4 * d;
            else
                return beta_max;
        }

    }
}

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
        public double W_1 = 10;                    // slow start -> congestion avoidance threshold
        double W_max = 1000;              // maximum window size (bottleneck threshhold)
        double W;                          // ccurrent window size
        double alpha_ss = 1;            // growth coefficient in slow start
        double beta_ss = 0.5;           // denominator in slow start

        public double T_min { get; set; } // min RTT in session 
        public double T_max { get; set; } // max RTT in session 

        //double h; //discretization step

        public Sender()//(double _h)
        {
            //h = _h;
            W = W_0;
            T_min = double.NaN;
            T_max = double.NaN;
        }

        public int SSIndicator
        {
            get { return W < W_1 ? 1 : 0; }
        }
        public double step(double h, double Rtt, int dh, int dl) //parameters: time increment, RTT, loss increment, timeout increment; returns: current control (window size)
        {
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
            return W;
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

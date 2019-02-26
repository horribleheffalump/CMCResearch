using CMC;
using CMC.Filters;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TCPAgent
{
    public class StateBasedSender : TCPSender
    {
        double alpha_max = 10;
        double alpha_min = 0.1; // from Illinois: 0.1 in Basar original paper, 0.3 in Linux kernel implimentation
        double alpha_cons = 0.3; // conservative value for normal state
        double beta_max = 0.5;
        double beta_min = 0.125;

        private Vector<double> alphas;
        private Vector<double> betas;
        private double alpha;
        private double beta;

        double alpha_ss = 1;            // growth coefficient in slow start
        double beta_ss = 0.5;           // denominator in slow start


        public double T_min { get; set; } // min RTT in session 
        public double T_max { get; set; } // max RTT in session 

        public double[] RTTs;
        private int c = 0;
        private bool full = false;
        //double h; //discretization step

        public StateBasedSender(double _rawrtt, double _gamma, int _saveEvery = 0) : base(_rawrtt, _gamma, _saveEvery) // parameters: start point for RTT estimation
        {
            //h = _h;
            alphas = Extensions.Vector(alpha_max, alpha_min, alpha_min, alpha_min);
            betas = Extensions.Vector(beta_min, beta_min, beta_max, beta_max);

            T_min = double.NaN;
            T_max = double.NaN;

            RTTs = new double[100]; // TODO: calculate for different discretization steps and RTTs
        }
        public StateBasedSender(double _rawrtt, double _gamma, double alpha_min, double alpha_max, double beta_min, double beta_max, int _saveEvery = 0) : base(_rawrtt, _gamma, _saveEvery) // parameters: start point for RTT estimation
        {
            //h = _h;
            this.alpha_max = alpha_max;
            this.alpha_min = alpha_min;
            this.beta_max = beta_max;
            this.beta_min = beta_min;

            alphas = Extensions.Vector(alpha_max, alpha_min, alpha_min, alpha_min);
            betas = Extensions.Vector(beta_min, beta_min, beta_max, beta_max);

            T_min = double.NaN;
            T_max = double.NaN;

            RTTs = new double[100]; // TODO: calculate for different discretization steps and RTTs
        }


        public double Step(double h, int dh, int dl, double rawrtt, Vector<double> p)
        {
            alpha = p.DotProduct(alphas);
            beta = p.DotProduct(betas);
            return Step(h, dh, dl, rawrtt);
        }

        public override double Step(double h, int dh, int dl, double rawrtt) //parameters: time increment,loss increment, timeout increment, RTT; returns: current control (window size)
        {
            t += h;

            //this.rawrtt = estimateRTT(rawrtt);
            //this.rtt = estimateRTT(rawrtt);
            RTTs[c] = rawrtt;
            double sumRTT = 0;
            if (full)
            {
                for (int i = 0; i < RTTs.Length; i++)
                {
                    sumRTT += RTTs[i];
                }
                rtt = sumRTT / RTTs.Length;
            }
            else
            {
                for (int i = 0; i <= c; i++)
                {
                    sumRTT += RTTs[i];
                }
                rtt = sumRTT / (c + 1);
            }
            // moving average with window equal to RTT
            c++;
            if (c == RTTs.Length)
            {
                full = true;
                c = 0;
            }
            //this.rawrtt = rawrtt;
            //this.rawrtt = estimateRTT(rawrtt);
            //this.rtt = estimateRTT(rawrtt);
            this.rawrtt = rtt;

            //if (double.IsNaN(T_min) || double.IsNaN(T_max))
            //{
            //    T_min = this.rawrtt;
            //    T_max = this.rawrtt;
            //}
            //T_min = Math.Min(T_min, this.rawrtt);
            //T_max = Math.Max(T_max, this.rawrtt);


            //double d = rtt - T_min;



            W = W
                + SSIndicator * alpha_ss * W * h / rtt //slow start additive increase 
                + (1 - SSIndicator) * alpha * h / rtt //congestion avoidance additive increase
                - SSIndicator * beta_ss * W * dh //multiple decrease when loss occurs in slow start
                - (1 - SSIndicator) * beta * W * dh; //multiple decrease when loss occurs in congavoid
            W = Math.Min(Math.Max(W, W_0), W_max); // make W in [W_0, W_max]

            if (dl > 0)
            {
                W_1 = W / 2;
                W = W_0; // setting W_1 equal to W/2 and entering the SS phase when timout occurs
            }

            Save();

            return W;
        }
    }
}

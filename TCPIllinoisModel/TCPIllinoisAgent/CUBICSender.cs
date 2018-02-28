using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPAgent
{
    public class CUBICTCPSender : TCPSender
    {
        double beta_cubic = 0.7;
        double C = 0.4; // C is a constant fixed to determine the aggressiveness of growth in high BDP networks, with higher C values (for example C = 4.0), CIBIC is more aggressive
        double beta_aimd = 0.5;
        double alpha_aimd = 1.0;
        double t_sincelastloss;
        double W_atpreviousloss;
        double W_atlastloss;

        public CUBICTCPSender(double _rawrtt, double _gamma, int _saveEvery = 0) : base(_rawrtt, _gamma, _saveEvery) // parameters: start point for RTT estimation
        {
            t_sincelastloss = 0.0;
            W_atpreviousloss = W_0;
            W_atlastloss = W_0;
        }


        public override double Step(double h, int dh, int dl, double rawrtt) //parameters: time increment,loss increment, timeout increment, RTT; returns: current control (window size)
        {
            t += h;
            t_sincelastloss += h;

            this.rawrtt = estimateRTT(rawrtt);
            this.rtt = estimateRTT(rawrtt);

            //this.rawrtt = rawrtt;
            double K = Math.Pow(W_atlastloss * (1 - beta_cubic) / C, 1 / 3.0);

            double W_cubic = C * Math.Sign(t_sincelastloss - K) * Math.Pow(Math.Abs(t_sincelastloss - K), 3.0) + W_atlastloss;

            // TCP friendly region as defined in RFC with reference to  
            // Floyd, S., Handley, M., and J. Padhye, "A Comparison of Equation - Based and AIMD Congestion Control", May 2000.
            double W_aimd = W_atlastloss * beta_aimd + (3 * (1 - beta_aimd) / (1 + beta_aimd)) * (t_sincelastloss / rtt);

            // If W_cubic(t) is less than W_aimd(t), then the protocol is in the TCP
            // friendly region and cwnd SHOULD be set to W_aimd(t) 
            W = Math.Max(W_cubic, W_aimd);

            W = Math.Min(Math.Max(W, W_0), W_max); // keep W in [W_0, W_max]
            if (dh > 0)
            {
                // When a packet loss(detected by duplicate ACKs) occurs, CUBIC updates
                // its W_atlastloss, cwnd, and ssthresh(slow start threshold) as follows.
                // Parameter beta_cubic SHOULD be set to 0.7.


                W_atlastloss = W;                 // save window size before reduction

                // Fast convergence heuristics
                if (W_atlastloss < W_atpreviousloss)
                {
                    // check downward trend
                    W_atpreviousloss = W_atlastloss;      // remember the last W_atlastloss
                    W_atlastloss = W_atlastloss * (1.0 + beta_cubic) / 2.0; // further reduce W_atlastloss
                }
                else
                {
                    // check upward trend
                    W_atpreviousloss = W_atlastloss;              // remember the last W_atlastloss
                }


                W_1 = W * beta_cubic; // new slow start threshold
                W = W * beta_cubic;     // window reduction
                t_sincelastloss = 0.0;
            }



            if (dl > 0)
            {
                //W_atlastloss = W;                 // save window size before reduction
                W_1 = W / 2;
                W = W_0; // setting W_1 equal to W/2 and entering the SS phase when timout occurs
                t_sincelastloss = 0.0;
            }

            Save();

            return W;
        }

    }
}

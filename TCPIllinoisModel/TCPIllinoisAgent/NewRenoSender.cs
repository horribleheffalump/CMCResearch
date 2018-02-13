using CMC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPAgent
{
    public class NewRenoSender : TCPSender
    {
        double alpha_ss = 1;            // growth coefficient in slow start
        double beta_ss = 0.5;           // denominator in slow start

        double alpha_ca = 1;            // growth coefficient in congestion avoidance
        double beta_ca = 0.5;           // denominator in congestion avoidance


        public NewRenoSender(double _rawrtt, int _saveEvery = 0) : base(_rawrtt, _saveEvery) // parameters: start point for RTT estimation
        { }

        public override double Step(double h, int dh, int dl, double rawrtt) //parameters: time increment, loss increment, timeout increment, RTT; returns: current control (window size)
        {
            this.rawrtt = rtt;
            this.rtt = rawrtt;

            t += h;

            W = W
                + SSIndicator * alpha_ss * W * h / rtt //slow start additive increase 
                + (1 - SSIndicator) * alpha_ca * h / rtt //congestion avoidance additive increase
                - SSIndicator * beta_ss * W * dh //multiple decrease when loss occurs in slow start
                - (1 - SSIndicator) * beta_ca * W * dh; //multiple decrease when loss occurs in congavoid
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

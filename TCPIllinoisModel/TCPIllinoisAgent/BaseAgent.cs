using CMC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPAgent
{
    public abstract class TCPSender
    {
        protected double W_0 = 1;                     // min windiw size
        public double W_1 = 10;                    // slow start -> congestion avoidance threshold
        protected double W_max = 10000;              // maximum window size (bottleneck threshhold)
        public double W;                          // ccurrent window size
        public int SaveEvery;
        private int saveCounter = 0;

        public List<Control> controls;


        public double rawrtt;   // row rtt obtained from received acks. We calculate dt - a time spent to get df acks, where df has a minimum of f_step, then calculate rawrtt = df / dt. 
        public double rtt;      // rtt estimate obtained from rawrtt data by means of ecpontntial smoothing
        protected double gamma = 0.99; // exponential smoothing parameter
        int f_step = 10; // akk discretization step
        protected double df = 0.0; // number of acks since last moment we've updated rtt
        protected double dt = 0.0; // time spent to get at least f_step acks
        protected double t = 0.0; // current time

        public TCPSender(double _rawrtt, int _saveEvery = 0)
        {
            //h = _h;
            rawrtt = _rawrtt;
            rtt = rawrtt;

            W = W_0;
            SaveEvery = _saveEvery;

            controls = new List<Control>();
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
        public int SSIndicator
        {
            get { return W < W_1 ? 1 : 0; }
        }

        public abstract double Step(double h, int dh, int dl, double Rtt = double.NaN); //parameters: time increment, RTT, loss increment, timeout increment; returns: current control (window size)

        protected void Save(params double[] p)
        {
            saveCounter++;
            if (SaveEvery > 0 && saveCounter % SaveEvery == 0)
            {
                double[] all_params = new double[] { SSIndicator, W_1, rawrtt, rtt };
                if (p != null)
                {
                    all_params = all_params.Concat(p).ToArray();
                }
                var control = new Control(t, W, all_params);
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



    }
}

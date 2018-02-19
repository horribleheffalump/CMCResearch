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
        public double W_1 = 100;                    // slow start -> congestion avoidance threshold. Initial value is high for efficeint bandwidth probing
        protected double W_max = 10000;              // maximum window size (bottleneck threshhold)
        public double W;                          // ccurrent window size
        public int SaveEvery;
        private int saveCounter = 0;

        public List<Control> controls;

        public double rawrtt; // raw rtt observations
        public double rtt;      // rtt smoothed (if required)
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


        public int SSIndicator
        {
            get { return W < W_1 ? 1 : 0; }
        }

        public abstract double Step(double h, int dh, int dl, double rtt); //parameters: time increment, loss increment, timeout increment, RTT; returns: current control (window size)

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

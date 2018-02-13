using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMC;
using SystemJointObs;

namespace Channel
{
    public abstract class TCPChannel
    {
        protected double h; // discretization step
        public double RTT0; // propagation delay in seconds

        public TCPChannel()
        { }

        public abstract (double rtt, int loss, int timeout) Step(double u);

        public abstract void SaveAll(string folderName);
    }
}

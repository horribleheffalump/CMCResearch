using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Channel
{
    public class SimpleChannel
    {
        private double h; // discretization step
        private double RTT0; // propagation delay in seconds
        public double bandwidth; // channel bandwidth Mbps
        public double MTU; // packet size in bytes
        public int buffersize; // size of link buffer
        public double current_buffersize; // current queue length in queue

        private double maxU; // maximum window size corresponding to throughput = bandwidth
        private double bandwidth_bps; // bandwidth bytes per second


        public SimpleChannel(double h, double RTT0, double bandwidth, double MTU, int buffersize)
        {
            this.h = h;
            this.RTT0 = RTT0;
            this.bandwidth = bandwidth;
            this.MTU = MTU;
            this.buffersize = buffersize;

            bandwidth_bps = bandwidth * 1000.0 * 1000.0 / 8.0; 
            maxU = bandwidth_bps * RTT0 / MTU; // since throughput = W * MTU / RTT
        }

        public void Step(double u)
        {
            current_buffersize += (u / RTT(u) - bandwidth_bps / MTU) * h;
            if (current_buffersize < 0) current_buffersize = 0;
        }

        public double RTT(double u)
        {
            //double current_buffersize = u / maxU * buffersize; // the part of buffer occupied is proportional to the window size. The buffer is full, when throughput = bandwidth
            return RTT0 + current_buffersize * MTU / bandwidth_bps;
        }
        public int LossIndicator(double u) //indicates loss in channel. A loss occures when throughput acheives its maximum
        {
            //return u > maxU + buffersize ? 1 : 0;
            return current_buffersize > buffersize ? 1 : 0;
        }
    }
}

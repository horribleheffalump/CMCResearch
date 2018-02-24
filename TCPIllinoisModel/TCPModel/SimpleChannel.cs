using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Channel
{
    public class SimpleChannel : TCPChannel
    {

        public double bandwidth; // channel bandwidth Mbps
        public double MTU; // packet size in bytes
        public int buffersize; // size of link buffer
        public double current_buffersize; // current queue length in queue

        private double maxU; // maximum window size corresponding to throughput = bandwidth
        private double bandwidth_bps; // bandwidth bytes per second


        public SimpleChannel(double h, double RTT0, double bandwidth, double MTU, int buffersize): base()
        {
            this.h = h;
            this.RTT0 = RTT0;
            this.bandwidth = bandwidth;
            this.MTU = MTU;
            this.buffersize = buffersize;

            bandwidth_bps = bandwidth * 1000.0 * 1000.0 / 8.0; 
            maxU = bandwidth_bps * RTT0 / MTU; // since throughput = W * MTU / RTT
        }

        public override (int loss, int timeout, double rtt) Step(double u)
        {
            current_buffersize += (u / RTT - bandwidth_bps / MTU) * h;
            if (current_buffersize < 0) current_buffersize = 0;
            if (current_buffersize > buffersize) current_buffersize = buffersize;

            return (LossIndicator, TimeoutIndicator, RTT); 
        }

        public double RTT // rtt depends on queue size: rtt = propagation delay (constant RTT0) + queueing delay (queue size * packet size / bandwidth)
        {
            get
            {
                return RTT0 + current_buffersize * MTU / bandwidth_bps;
            }
        }
        public int LossIndicator //indicates loss in channel. A loss occures when throughput acheives its maximum
        {
            get
            {
                //return u > maxU + buffersize ? 1 : 0;
                return current_buffersize >= buffersize ? 1 : 0;
            }
        }
   
        public int TimeoutIndicator //indicates timeout in channel. There is no timeout in simple channel :)
        {
            get
            {
                return 0;
            }
        }

        public override void SaveAll(string folderName)
        {
            // TODO: save all for SimpleChannel
        }
    }
}

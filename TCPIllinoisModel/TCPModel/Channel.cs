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

        /// <summary>
        /// Abstract method for channel state reaclculaton
        /// </summary>
        /// <param name="u">cwnd size in packets</param>
        /// <returns>
        /// loss - number of losses occured since last recalculation
        /// timeout - number of timeouts occured since last recalculation
        /// rtt - current rtt (if calculated)
        /// ack_received_count - number of acks received during ack_received_time period
        /// ack_received_time - period of time for ack accounting (may not depend on h - discretization step)
        /// </returns>
        public abstract (int loss, int timeout, double? rtt, double? ack_received_count, double? ack_received_time) Step(double u);

        public abstract void SaveAll(string folderName);
    }
}

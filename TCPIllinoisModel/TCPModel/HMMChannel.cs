using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMCTools;
using SystemJointObs;

namespace Channel
{
    public class HMMChannel
    {
        int N;
        public double delta_p;
        public Vector<double> D;
        Vector<double> K;
        Vector<double> P;
        Vector<double> Q;

        public JointObservationsSystem JOS;

        public HMMChannel(double _t0, double _T, double _h, bool _saveHistory)
        {

            N = 4; // states count: e1 - free, e2 - moderate load, e3 - wire congestion, e4 - last mile (wireless) bad signal
            // RTT = delta_p + D X_t + w_t K X_t
            delta_p = 0.01; // delta_p - signal propagation time
            D = Vector(0.001, 0.01, 0.02, 0.02); // D X_t - queueing time because of the other senders transmission
            K = Vector(0.001, 0.01, 0.02, 0.02); // w_t K X_t - queueing time because of senders own transmission

            //loss intensity mu_t = R_t diag(P)
            P = Vector(0.01, 0.05, 0.15, 0.15);
            //timeout intensity nu_t = R_t diag(Q)
            Q = Vector(0.001, 0.005, 0.015, 0.015);

            JOS = new JointObservationsSystem(N, _t0, _T, 0, _h, TransitionMatrix, new Func<double, double, Vector<double>>[] { LossIntensity, TimeoutIntensity }, R, G, _saveHistory);

        }



        public Matrix<double> TransitionMatrix(double t, double w)
        {
            double lambda14 = 0.1;
            double lambda41 = 0.1;
            double lambda24 = 0.1;
            double lambda42 = 0.1;
            double lambda12 = 0.1 + 1 * w;
            double lambda21 = Math.Max(0.0, 1.0 - 0.1 * w);
            double lambda23 = 0.1 + 0.1 * w;
            double lambda32 = Math.Max(0.0, 1.0 - 0.1 * w); ;
            Matrix<double> Lambda = Matrix<double>.Build.DenseOfArray(new[,]{{-(lambda12 + lambda14), lambda12, 0, lambda14 },
                                                                             { lambda21, -(lambda21 + lambda23 + lambda24 ), lambda23, lambda24 },
                                                                             { 0, lambda32, -lambda32, 0 },
                                                                             { lambda41, lambda42, 0, -(lambda41 + lambda42) }});
            if (!Lambda.IsTransitionRateMatrix())
            {
                throw new ArgumentException("The result matrix is not transition rate matrix");
            }

            return Lambda;

        }


        public Vector<double> LossIntensity(double t, double u)
        {
            return Diag(P) * R(t,u);
        }

        public Vector<double> TimeoutIntensity(double t, double u)
        {
            return  Diag(Q) * R(t, u);
        }

        public Vector<double> R(double t, double u)
        {
            return Vector(
                1.0 / (delta_p + D[0] + u * K[0]),
                1.0 / (delta_p + D[1] + u * K[1]),
                1.0 / (delta_p + D[2] + u * K[2]),
                1.0 / (delta_p + D[3] + u * K[3])
                );
        }
        public Vector<double> G(double t, double u)
        {
            return Vector(
                1.0 / Math.Sqrt(D[0] + u * K[0]),
                1.0 / Math.Sqrt(D[1] + u * K[1]),
                1.0 / Math.Sqrt(D[2] + u * K[2]),
                1.0 / Math.Sqrt(D[3] + u * K[3])
                );
        }

        static Vector<double> Vector(params double[] val)
        {
            return Vector<double>.Build.Dense(val);
        }

        static Matrix<double> Diag(params double[] val)
        {
            return Matrix<double>.Build.DenseDiagonal(val.Length, val.Length, (i) => val[i]);
        }

        static Matrix<double> Diag(Vector<double> val)
        {
            return Matrix<double>.Build.DenseDiagonal(val.Count, val.Count, (i) => val[i]);
        }


    }
}

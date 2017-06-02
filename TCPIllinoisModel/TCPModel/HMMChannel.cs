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
        int N = 4; // states count: e1 - free, e2 - moderate load, e3 - wire congestion, e4 - last mile (wireless) bad signal
        JointObservationsSystem JOS;

        public HMMChannel(int _N, double _t0, double _T, double _h, bool _saveHistory)
        {
            JOS = new JointObservationsSystem(_N, _t0, _T, 0, _h, TransitionMatrix, new Func<double, double, Vector<double>>[] { LossIntensity, TimeoutIntensity }, R, Q, _saveHistory);
        }



        public Matrix<double> TransitionMatrix(double t, double w)
        {
            double lambda14 = 0.1;
            double lambda41 = 0.1;
            double lambda24 = 0.1;
            double lambda42 = 0.1;
            double lambda12 = 0.1 + 0.1 * w;
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
            return Vector<double>.Build.DenseOfArray(new double[] { 0, 0, 0, 0 });
        }
        public Vector<double> TimeoutIntensity(double t, double u)
        {
            return Vector<double>.Build.DenseOfArray(new double[] { 0, 0, 0, 0 });
        }
        public Vector<double> R(double t, double u)
        {
            return Vector<double>.Build.DenseOfArray(new double[] { 0, 0, 0, 0 });
        }
        public Vector<double> Q(double t, double u)
        {
            return Vector<double>.Build.DenseOfArray(new double[] { 0, 0, 0, 0 });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCTools;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using SystemCPObs;

namespace TransmitterModel
{
    public class Channel
    {
        public Coords BaseStation;
        double t0;
        double T;
        double h;
        Func<double, Coords> TransmitterPosition;
        const int N = 3;
        const double minDist = 0;
        const double maxDist = 10;
        public CountingProessObservationsSystem CPOS;

        //CPOS.GenerateTrajectory((t) => U);
        //CPOS.State.SaveTrajectory(Properties.Settings.Default.MCFilePath);
        //CPOS.Observation.SaveTrajectory(Properties.Settings.Default.CPFilePath);


        public Channel(Coords _BaseStation, double _t0, double _T, double _h, Func<double, Coords> _TransmitterPosition)
        {
            BaseStation = _BaseStation;
            t0 = _t0;
            T = _T;
            h = _h;
            TransmitterPosition = _TransmitterPosition;
            //Vector<double> C = Vector<double>.Build.DenseOfArray(new[] { 1.0, 50.0, 150.0 });
            Vector<double> C = Vector<double>.Build.DenseOfArray(new[] { 160.0 * 0.01, 160.0 * 0.04, 160.0 * 0.1 }); // 160p/s ~ 2Mbps (MTU = 1500 bytes). Loss: 1%, 4%, 10%
            CPOS = new CountingProessObservationsSystem(N, t0, T, 0, h, (t) => TransitionRate(Distance(t)), (t) => C);

        }

        public double Distance(double t)
        {
            return Coords.Distance(BaseStation, TransmitterPosition(t));
        }

        public static Matrix<double> TransitionRate(double dist)
        {
            var e = Matrix<double>.Build.DenseOfArray(new[,] { { 1.0, 1.0, 1.0 } });
            var l = (e.Transpose() * Probs(dist)) - Matrix<double>.Build.DenseIdentity(N);
            //var l = Matrix<double>.Build.DenseOfArray(new[,]{{p0, p1, p2 },
            //                             {  p0, p1, p2 },
            //                             {  p0, p1, p2 }}) - Matrix<double>.Build.DenseIdentity(N);
            l = l * 0.1;
            if (!l.IsTransitionRateMatrix())
            {
                throw new ArgumentException("The result matrix is not transition rate matrix");
            }
            return l;
            //            plt.plot(x, exp(-10 * x / 2), color = 'black')
            //plt.plot(x, maxval(exp(-5 + 10 * x / 2)), color = 'blue')
            //plt.plot(x, minval(1.0 - exp(-10 * x / 2) - maxval(exp(-5 + 10 * x / 2))), color = 'red')
        }

        public static Matrix<double> Probs(double dist)
        {
            dist = Math.Min(maxDist, dist);
            dist = Math.Max(minDist, dist);
            dist = dist * 2.0;
            double p0 = Math.Max(Math.Min(1 / Math.Pow(dist + 1.0, 2), 0.95), 0.05);
            double p2 = Math.Max(Math.Min(1 / Math.Pow(5.0-dist, 2), 0.95), 0.05);
            //double p0 = Math.Max(Math.Min(Math.Exp(-dist / 2.0), 0.95), 0.05);
            //double p2 = Math.Max(Math.Min(Math.Exp(-5 + dist / 2.0), 0.95), 0.05);
            double p1 = 1.0 - p0 - p2;
            return Matrix<double>.Build.DenseOfArray(new[,] { { p0, p1, p2 } });
        }
    }
}

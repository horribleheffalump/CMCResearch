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
        int N = 3;
        double minDist = 0;
        double maxDist = 10;
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
            Vector<double> C = Vector<double>.Build.DenseOfArray(new[] { 1.0, 50.0, 150.0 });
            CPOS = new CountingProessObservationsSystem(N, t0, T, 0, h, (t) => TransitionRate(Distance(t)), (t) => C);

        }

        public double Distance(double t)
        {
            return Coords.Distance(BaseStation, TransmitterPosition(t));
        }

        public Matrix<double> TransitionRate(double dist)
        {
            dist = Math.Min(maxDist, dist);
            dist = Math.Max(minDist, dist);
            dist = dist / 10.0;
            double p0 = Math.Max(Math.Min(Math.Exp(-dist / 2.0), 0.95), 0.05);
            double p2 = Math.Max(Math.Min(Math.Exp(-5 + dist / 2.0), 0.95), 0.05);
            double p1 = 1.0 - p0 - p2;
            var l = Matrix<double>.Build.DenseOfArray(new[,]{{p0, p1, p2 },
                                         {  p0, p1, p2 },
                                         {  p0, p1, p2 }}) - Matrix<double>.Build.DenseIdentity(N);
            if (!l.IsTransitionRateMatrix())
            {
                throw new ArgumentException("The result matrix is not transition rate matrix");
            }
            return l;

        }
    }
}

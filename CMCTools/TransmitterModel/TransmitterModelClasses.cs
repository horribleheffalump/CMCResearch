using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransmitterModel
{
    public class Coords
    {
        public double x;
        public double y;

        public Coords(double _x, double _y)
        {
            x = _x;
            y = _y;
        }

        public static Coords operator +(Coords c1, Coords c2)
        {
            return new Coords(c1.x + c2.x, c1.y + c2.y);
        }

        public static Coords operator -(Coords c1, Coords c2)
        {
            return new Coords(c1.x - c2.x, c1.y - c2.y);
        }

        public static double Distance(Coords c1, Coords c2)
        {
            return Math.Sqrt(Math.Pow(c1.x - c2.x, 2) + Math.Pow(c1.y - c2.y, 2));
        }

        public override string ToString()
        {
            //return "(" + x + ", " + y + ")";
            return x + " " + y;
        }
    }

    public class Transmitter
    {
        public double t0 = 0;   // observation start time
        public double T;        // observation end time
        public double h = 1e-3; // discretization step
        public Coords Pos0;     // start position
        public Func<double, Coords> PosDynamics; // function to calculate position in time
        public List<Coords> Trajectory;

        public Transmitter(double _t0, double _T, Coords _Pos0, double _h,  Func<double, Coords> _PosDynamics)
        {
            t0 = _t0;
            T = _T;
            h = _h;
            Pos0 = _Pos0;
            PosDynamics = _PosDynamics;
        }

        public Coords Pos(double t)
        {
            return Pos0 + PosDynamics(t);
        }

        public void GenerateTrajectory()
        {
            Trajectory = new List<Coords>();
            double t = t0;
            while (t < T)
            {
                Trajectory.Add(Pos(t));
                t += h;
            }
        }

        public void SaveTrajectory(string path)
        {
            System.IO.StreamWriter outputfile = new System.IO.StreamWriter(path);
            foreach (Coords c in Trajectory)
            {
                outputfile.WriteLine(c.ToString());
            }
            outputfile.Close();
        }

    }
}

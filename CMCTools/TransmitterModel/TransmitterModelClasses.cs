using System;
using System.Collections.Generic;
using System.Globalization;
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

        public override bool Equals(object c)
        {
            return x == (c as Coords).x && y == (c as Coords).y;
        }

        public override Int32 GetHashCode()
        {
            return (x+y).GetHashCode();
        }

        public static bool operator ==(Coords c1, Coords c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }
        public static bool operator !=(Coords c1, Coords c2)
        {
            return !(c1.x == c2.x && c1.y == c2.y);
        }



        public static double Distance(Coords c1, Coords c2)
        {
            return Math.Sqrt(Math.Pow(c1.x - c2.x, 2) + Math.Pow(c1.y - c2.y, 2));
        }

        public override string ToString()
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            string result = string.Format(provider, "{0} {1}", x, y);
            return result;
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
        public List<Channel> Channels;
        public double PathLength; // длина пути

        public Transmitter(double _t0, double _T, Coords _Pos0, double _h,  Func<double, Coords> _PosDynamics)
        {
            t0 = _t0;
            T = _T;
            h = _h;
            Pos0 = _Pos0;
            PosDynamics = _PosDynamics;
            Channels = new List<Channel>();
            Channels.Add(new Channel(new Coords(0.1, 0.4), t0, T, h, (t) => _Pos0 + Pos(t)));
            Channels.Add(new Channel(new Coords(0.4, 2.0), t0, T, h, (t) => Pos(t)));
            Channels.Add(new Channel(new Coords(0.8, 1.0), t0, T, h, (t) => Pos(t)));

        }

        public Coords Pos(double t)
        {
            return Pos0 + PosDynamics(t);
        }

        public void GenerateTrajectory()
        {
            Trajectory = new List<Coords>();
            double t = t0;
            Coords CurrentPos = Pos(t);
            while (t < T)
            {
                Trajectory.Add(CurrentPos);
                t += h;
                Coords NextPos = Pos(t);
                PathLength += Coords.Distance(CurrentPos, NextPos);
                CurrentPos = NextPos;
            }
        }

        public void SaveTrajectory(string path)
        {
            System.IO.StreamWriter outputfile = new System.IO.StreamWriter(path);
            foreach (Coords c in Trajectory.Where((x, i) => i % 100 == 0))
            {
                outputfile.WriteLine(c.ToString());
            }
            outputfile.Close();
        }

        public void SaveBaseStations(string path)
        {
            System.IO.StreamWriter outputfile = new System.IO.StreamWriter(path);
            foreach (Channel c in Channels)
            {
                outputfile.WriteLine(c.BaseStation.ToString());
            }
            outputfile.Close();
        }

    }
}

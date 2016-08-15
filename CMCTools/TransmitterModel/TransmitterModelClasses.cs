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

    public class Point
    {
        public double t;
        public Coords coords;
    }

    public class Transmitter
    {
        public double t0 = 0;   // observation start time
        public double T;        // observation end time
        public double h = 1e-3; // discretization step
        public Coords Pos0;     // start position
        public Func<double, Coords> PosDynamics; // function to calculate position in time
        public List<Point> Trajectory;
        public List<Channel> Channels;
        public double PathLength; // длина пути

        public Transmitter(double _t0, double _T, Coords _Pos0, double _h,  Func<double, Coords> _PosDynamics, Coords[] _BaseStations)
        {
            t0 = _t0;
            T = _T;
            h = _h;
            Pos0 = _Pos0;
            PosDynamics = _PosDynamics;
            Channels = new List<Channel>();
            for (int i = 0; i < _BaseStations.Length; i++)
            {
                Channels.Add(new Channel(_BaseStations[i], t0, T, h, (t) => Pos(t)));
            }
        }

        public Coords Pos(double t)
        {
            return Pos0 + PosDynamics(t);
        }

        public void GenerateTrajectory()
        {
            Trajectory = new List<Point>();
            double t = t0;
            Coords CurrentPos = Pos(t);
            while (t < T)
            {
                Trajectory.Add(new Point { t = t, coords = CurrentPos });
                t += h;
                Coords NextPos = Pos(t);
                PathLength += Coords.Distance(CurrentPos, NextPos);
                CurrentPos = NextPos;
            }
        }

        public void SaveTrajectory(string path, int every = 1)
        {
            using (System.IO.StreamWriter outputfile = new System.IO.StreamWriter(path))
            {
                foreach (Point p in Trajectory.Where((x, i) => i % 100 == 0))
                {
                    NumberFormatInfo provider = new NumberFormatInfo();
                    provider.NumberDecimalSeparator = ".";
                    string line = string.Format(provider, "{0} {1}", p.t, p.coords.ToString());
                    foreach (var ch in Channels)
                    {
                        line = string.Format(provider, "{0} {1}", line, Coords.Distance(ch.BaseStation, p.coords));
                    }
                    outputfile.WriteLine(line);
                }
                outputfile.Close();
            }
        }

        public void SaveBaseStations(string path)
        {
            using (System.IO.StreamWriter outputfile = new System.IO.StreamWriter(path))
            {
                foreach (Channel c in Channels)
                {
                    outputfile.WriteLine(c.BaseStation.ToString());
                }
                outputfile.Close();
            }
        }

    }
}

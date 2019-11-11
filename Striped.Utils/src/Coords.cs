using System;
using System.Collections.Generic;

namespace Striped.Utils
{
    public class Coords
    {
        public static Coords Zero { get => new Coords(0, 0); }

        public int X { get; set; }
        public int Y { get; set; }

        public Coords() { }

        public Coords(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public Coords(Coords coords)
        {
            X = coords.X;
            Y = coords.Y;
        }

        public void Set(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public bool SameAs(Coords coords)
        {
            return X == coords.X && Y == coords.Y;
        }

        public static int HorisontalDistance(Coords pos1, Coords pos2)
        {
            void swap(ref int i1, ref int i2)
            {
                var tmp = i1;
                i1 = i2;
                i2 = tmp;
            }

            var x1 = pos1.X;
            var x2 = pos2.X;

            if (x1 > x2)
            {
                swap(ref x1, ref x2);
            }

            if (x1 < 0)
            {
                return (x1 * -1) + x2;
            }

            if (x1 > 0)
            {
                return x2 - x1;
            }

            return x2;
        }

        public static int VerticalDistance(Coords pos1, Coords pos2)
        {
            void swap(ref int i1, ref int i2)
            {
                var tmp = i1;
                i1 = i2;
                i2 = tmp;
            }

            var y1 = pos1.Y;
            var y2 = pos2.Y;

            if (y1 > y2)
            {
                swap(ref y1, ref y2);
            }

            if (y1 < 0)
            {
                return (y1 * -1) + y2;
            }

            if (y1 > 0)
            {
                return y2 - y1;
            }

            return y2;
        }

        public static List<Coords> GetPath(Coords position1, Coords position2)
        {
            var traectoryPoints = new List<Coords>();

            void Swap(ref int a, ref int b)
            {
                int tmp = a;
                a = b;
                b = tmp;
            }

            int x0 = position1.X;
            int x1 = position2.X;
            int y0 = position1.Y;
            int y1 = position2.Y;

            var steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);

            if (steep)
            {
                Swap(ref x0, ref y0);
                Swap(ref x1, ref y1);
            }

            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }
            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                {
                    traectoryPoints.Add(new Coords(steep ? y : x, steep ? x : y));
                }
                error -= dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }

            if (!traectoryPoints[0].SameAs(position1))
            {
                traectoryPoints.Reverse();
            }

            return traectoryPoints;
        }

        public bool IsInRectangle(Coords coords1, Coords coords2)
        {
            void swap<T>(ref T a, ref T b)
            {
                var tmp = a;
                a = b;
                b = tmp;
            }

            if (coords1.X > coords2.X)
            {
                swap(ref coords1, ref coords2);
            }

            var x1 = coords1.X;
            var x2 = coords2.X;

            var y1 = coords1.Y;
            var y2 = coords2.Y;

            return (X >= x1 && X <= x2) && (Y >= y1 && Y <= y2);
        }
    }
}

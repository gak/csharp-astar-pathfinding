using System;
using System.Collections.Generic;

namespace Example
{
    internal class Pos
    {
        public readonly int X;
        public readonly int Y;

        public Pos(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(Object otherObject)
        {
            var other = (Pos) otherObject;

            return other != null && (X == other.X && Y == other.Y);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public IEnumerable<Pos> Neighbours(int bounds)
        {
            var n = new List<Pos>();
            if (X > 0)
            {
                n.Add(new Pos(X - 1, Y));
            }

            if (Y > 0)
            {
                n.Add(new Pos(X, Y - 1));
            }

            if (X < bounds - 1)
            {
                n.Add(new Pos(X + 1, Y));
            }

            if (Y < bounds - 1)
            {
                n.Add(new Pos(X, Y + 1));
            }

            return n;
        }

        public float ApproxDistance(Pos dst)
        {
            return (float) (
                Math.Pow(Math.Abs(dst.X - X), 2) +
                Math.Pow(Math.Abs(dst.Y - Y), 2)
            );
        }
    }
}
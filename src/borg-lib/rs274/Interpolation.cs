using System;
using System.Runtime.Serialization;
using Yuni.CoordinateSpace;

namespace Borg.Machine
{
    public static class Interpolation
    {
        public static Coordinate Intersect(int direction, Coordinate start,
                                                    Coordinate offset,
                                                    Coordinate end,
                                                    CoordinateBounds workEnvelop)
        {
            var intersectionBuilder = new CoordinateBuilder();
            var center = new CoordinateBuilder()
                                .WithX(start.X.Value + offset.X.Value)
                                .WithY(start.Y.Value + offset.Y.Value).Build();
            var radius = Hypotenuse(offset.X.Value, offset.Y.Value);
            var opposite = workEnvelop.MinX - center.X.Value;
            var angle = FindAngleAFromOppositeAndHypotenuse(opposite, radius);
            var xcross = (Math.Sin(AngleToRadians(angle)) * radius) + center.X.Value;
            var ycross = (Math.Cos(AngleToRadians(angle)) * radius) + center.Y.Value;

            return intersectionBuilder.WithX(xcross).WithY(ycross).Build();
        }

        public static double Hypotenuse(double opposite, double adjacent)
        {
            return Math.Sqrt((Math.Pow(opposite, 2.0)) + Math.Pow(adjacent, 2.0));
        }

        public static double FindAngleAFromOppositeAndAdjacent(double opposite, double adjacent)
        {
            return RadiansToAngle(Math.Atan(opposite / adjacent));
        }

        public static double FindAngleAFromOppositeAndHypotenuse(double opposite, double hypotenuse)
        {
            return RadiansToAngle(Math.Asin(opposite / hypotenuse));
        }

        public static double FindAngleAFromAdjacentAndHypotenuse(double adjacent, double hypotenuse)
        {
            return RadiansToAngle(Math.Acos(adjacent / hypotenuse));
        }

        public static (double, double) FindCenter(Coordinate start, Coordinate offset)
        {
            return FindCenter(start.X.Value, start.Y.Value, offset.X.Value, offset.Y.Value);
        }

        public static (double, double) FindCenter(double xstart, double ystart, double xoffset, double yoffset)
        {
            return (xstart + xoffset, ystart + yoffset);
        }

        public static double AngleToRadians(double angle)
        {
            return angle * (Math.PI / 180.0);
        }

        public static double RadiansToAngle(double radians)
        {
            return radians * (180.0 / Math.PI);
        }

        public static bool Intersect(double ax, double ay, double bx, double by,
                                    double cx, double cy, double r)
        {

            ax -= cx;
            ay -= cy;
            bx -= cx;
            by -= cy;

            var c = Math.Pow(ax, 2) + Math.Pow(ay, 2) - Math.Pow(r, 2);
            var b = 2 * (ax * (bx - ax) + ay * (by - ay));
            var a = Math.Pow(bx - ax, 2) + Math.Pow(by - ay, 2);

            //        var a = Math.Pow(ax, 2) + Math.Pow(ay, 2) - Math.Pow(r, 2);
            //       var b = 2 * (ax * (bx - ax) + ay * (by - ay));
            //      var c = Math.Pow(bx - ax, 2) + Math.Pow(by - ay, 2);
            var disc = Math.Pow(b, 2) - 4 * a * c;
            if (disc <= 0) return false;
            var sqrtdisc = Math.Sqrt(disc);
            var t1 = (-b + sqrtdisc) / (2 * a);
            var t2 = (-b - sqrtdisc) / (2 * a);
            if ((0 < t1 && t1 < 1) || (0 < t2 && t2 < 1)) return true;

            return false;
        }

        public static Quadrant QuadrantFromOffset(double offsetX, double offsetY)
        {
            if (offsetX <= 0 && offsetY <= 0)
                return Quadrant.Q1;
            else if (offsetX > 0 && offsetY <= 0)
                return Quadrant.Q2;
            else if (offsetX > 0 && offsetY > 0)
                return Quadrant.Q3;
            else
                return Quadrant.Q4;
        }

        public static (double, double) GetOppositeAndAdjacent(double x, double y) {
            var opposite = Math.Abs(y);
            var adjacent = Math.Abs(x);

            if (adjacent < opposite)
            {
                opposite = Math.Abs(x);
                adjacent = Math.Abs(y);
            }
            return (opposite, adjacent);
        }

        public static double AbsAngle(double offsetX, double offsetY)
        {
            var oppositeAndAdjacent = GetOppositeAndAdjacent(offsetX, offsetY);
            return Interpolation.FindAngleAFromOppositeAndAdjacent(oppositeAndAdjacent.Item1, oppositeAndAdjacent.Item2);
        }

        public static double PolarAngle(double x1, double y1, double offsetX, double offsetY)
        {
            var quadrant = Interpolation.QuadrantFromOffset(offsetX, offsetY);
            var absAngle = Interpolation.AbsAngle(offsetX, offsetY);
            var absXOffset = Math.Abs(offsetX);
            var absYOffset = Math.Abs(offsetY);

            switch (quadrant)
            {
                case Quadrant.Q4:
                    {
                        if (absYOffset <= absXOffset)
                            return 360 - absAngle;

                        return 270 + absAngle;
                    }
                case Quadrant.Q3:
                    {
                        if (absYOffset <= absXOffset)
                            return 180.0 + absAngle;

                        return 270 - absAngle;
                    }
                case Quadrant.Q2:
                    {
                        if (absYOffset <= absXOffset)
                            return 180 - absAngle;

                        return 90 + absAngle;
                    }
                case Quadrant.Q1:
                    {
                        if (absYOffset <= absXOffset)
                            return absAngle;

                        return 90 - absAngle;
                    }
                default:
                    throw new InvalidQuadrantException("quadrant is invalid");

            }
        }

       public static (double, double) GetXYOffsetFromAngleAndRadius(double angle, double radius) {
           var xOffset = Math.Cos(AngleToRadians(angle)) * radius;
           var yOffset = Math.Sin(AngleToRadians(angle)) * radius;
           return (xOffset, yOffset);
       }
    }

    public enum Quadrant
    {
        Q1 = 1, Q2 = 2, Q3 = 3, Q4 = 4
    }

    [Serializable]
    internal class InvalidQuadrantException : Exception
    {
        public InvalidQuadrantException()
        {
        }

        public InvalidQuadrantException(string message) : base(message)
        {
        }

        public InvalidQuadrantException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidQuadrantException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace Yuni.CoordinateSpace
{
    public static class LineUtilities
    {
        public static (LineDescriptor, Coordinate[]) DescribeLine(Line line, CoordinateBounds bounds)
        {
            var intersections = FindAllIntersections(line, bounds);
            var descriptor = LineDescriptor.Unknown;

            if (!CoordinateUtilities.IsInBounds(bounds, line.Start) && CoordinateUtilities.IsInBounds(bounds, line.End))
                descriptor = LineDescriptor.Enters;
            else if (CoordinateUtilities.IsInBounds(bounds, line.Start) && !CoordinateUtilities.IsInBounds(bounds, line.End))
                descriptor = LineDescriptor.Leaves;
            else if (intersections.Length == 0)
                descriptor = IsLineContained(line, bounds) ? LineDescriptor.Contained : LineDescriptor.Excluded;
            else if (intersections.Length == 1 
                    && IsCornerPoint(intersections[0], bounds)
                    && !CoordinateUtilities.IsInBounds(bounds, line.Start)
                    && !CoordinateUtilities.IsInBounds(bounds, line.End)
                    )
                descriptor = LineDescriptor.Excluded;
            else if (intersections.Length == 1)
                descriptor = IsLineEntering(line, bounds) ? LineDescriptor.Enters : LineDescriptor.Leaves;
            else if (intersections.Length == 2)
                descriptor = IsLineContained(line, bounds) ? LineDescriptor.Contained : LineDescriptor.Crosses;
            else if (intersections.Length == 4) // Has to be corner to corner, contained
                descriptor = LineDescriptor.Contained;

            return (descriptor, intersections);
        }

        private static bool IsCornerPoint(Coordinate coordinate, CoordinateBounds bounds)
        {
            var c0 = new Coordinate(bounds.MinX, bounds.MaxY);
            var c1 = new Coordinate(bounds.MaxX, bounds.MaxY);
            var c2 = new Coordinate(bounds.MaxX, bounds.MinY);
            var c3 = new Coordinate(bounds.MinX, bounds.MinY);

            return c0 == coordinate || c1 == coordinate || c2 == coordinate || c3 == coordinate;
        }

        public static bool Between(double val, double a, double b)
        {
            var min = Math.Min(a, b);
            var max = Math.Max(a, b);
            return val >= min && val <= max;
        }

        public static bool IsLineEntering(Line line, CoordinateBounds bounds)
        {
            var startX = line.Start.X.Value;
            var startY = line.Start.Y.Value;
            var endX = line.End.X.Value;
            var endY = line.End.Y.Value;

            if (startX <= bounds.MinX && endX >= bounds.MinX && Between(startY, bounds.MinY, bounds.MaxY))
                return true;
            if (startY >= bounds.MaxY && endY <= bounds.MaxY && Between(startX, bounds.MinX, bounds.MaxX))
                return true;
            if (startX >= bounds.MaxX && endX <= bounds.MaxX && Between(startY, bounds.MinY, bounds.MaxY))
                return true;
            if (startY <= bounds.MinY && endY >= bounds.MinY && Between(startX, bounds.MinX, bounds.MaxX))
                return true;

            return false;
        }

        public static bool IsLineContained(Line line, CoordinateBounds bounds)
        {
            return CoordinateUtilities.IsInBounds(bounds, line.Start) && CoordinateUtilities.IsInBounds(bounds, line.End);
        }

        public static Coordinate FindIntersection(this Line a, Line b)
        {
            double x1 = a.Start.X.Value;
            double y1 = a.Start.Y.Value;
            double x2 = a.End.X.Value;
            double y2 = a.End.Y.Value;

            double x3 = b.Start.X.Value;
            double y3 = b.Start.Y.Value;
            double x4 = b.End.X.Value;
            double y4 = b.End.Y.Value;

            double denominator = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (denominator == 0)
                return null;

            double xNominator = (x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4);
            double yNominator = (x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4);

            double px = xNominator / denominator;
            double py = yNominator / denominator;

            if (!Between(px, x1, x2) || !Between(py, y1, y2) || !Between(px, x3, x4) || !Between(py, y3, y4))
                return null;

            return new Coordinate(px, py);
        }

        public static Coordinate[] FindAllIntersections(Line a, CoordinateBounds bounds)
        {
            var c0 = new Coordinate(bounds.MinX, bounds.MaxY);
            var c1 = new Coordinate(bounds.MaxX, bounds.MaxY);
            var c2 = new Coordinate(bounds.MaxX, bounds.MinY);
            var c3 = new Coordinate(bounds.MinX, bounds.MinY);

            var lines = new[] { new Line(c0, c1), new Line(c1, c2), new Line(c2, c3), new Line(c3, c0) };
            var intersections = new List<Coordinate>();

            foreach (var l in lines)
            {
                var intersection = FindIntersection(a, l);
                if (intersection != null && !intersections.Any(i => i == intersection))
                    intersections.Add(intersection);
            }

            return intersections.ToArray();
        }

        public static Line NormalizeLine(this Line line)
        {
            if (line.End.X.Value < line.Start.X.Value)
                return new Line(line.End.X.Value, line.End.Y.Value, line.Start.X.Value, line.Start.Y.Value);

            return new Line(line.Start.X.Value, line.Start.Y.Value, line.End.X.Value, line.End.Y.Value);
        }
    }
}
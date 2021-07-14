using System;
using System.Collections.Generic;
using Yuni.CoordinateSpace;

namespace Borg.Machine
{
    public static class RewriterUtilities
    {
        public static bool IsLeavingWorkEnvelope(Coordinate current, Coordinate coordinate, CoordinateBounds bounds)
        {
            var currentIsInBounds = CoordinateUtilities.IsInBounds(bounds, current);
            var targetIsInBounds = CoordinateUtilities.IsInBounds(bounds, coordinate);
            return currentIsInBounds && !targetIsInBounds;
        }

        public static bool IsEnteringWorkEnvelope(Coordinate current, Coordinate coordinate, CoordinateBounds bounds)
        {
            var currentIsInBounds = CoordinateUtilities.IsInBounds(bounds, current);
            var targetIsInBounds = CoordinateUtilities.IsInBounds(bounds, coordinate);

            return !currentIsInBounds && targetIsInBounds;
        }

        public static double? GetValue(double? target, double min, double max)
        {
            if (target.HasValue && target < min)
                return min;
            else if (target.HasValue && target > max)
                return max;

            return target;
        }

        public static (double, double, double)[] StepCircle(InterpolationDirection direction, Coordinate start, double xOffset, double yOffset)
        {
            var stepIncrement = 1;
            var stepDirection = (int)direction;
            var results = new List<(double, double, double)>();

            var oppositeAndAdjacent = Interpolation.GetOppositeAndAdjacent(xOffset, yOffset);
            var radius2 = Interpolation.Hypotenuse(oppositeAndAdjacent.Item1, oppositeAndAdjacent.Item2);

            var polarAngle = Interpolation.PolarAngle(start.X.Value, start.Y.Value, xOffset, yOffset);
            var x = start.X.Value;
            var y = start.Y.Value;
            results.Add((polarAngle, x, y));

            var totalSteps = 1;
            while (totalSteps < (360 / stepIncrement))
            {
                polarAngle += (stepDirection * stepIncrement);

                if (polarAngle < 0)
                    polarAngle += 360;
                else if (polarAngle > 360)
                    polarAngle -= 360;

                totalSteps++;

                var coords = Interpolation.GetXYOffsetFromAngleAndRadius(polarAngle, radius2);
                results.Add((polarAngle, coords.Item1, coords.Item2));
            }

            return results.ToArray();
        }

        public static bool CrossesLine(Coordinate start, Coordinate end, Line line)
        {
            var crossingLine = new Line(start, end);
            var x1 = crossingLine.Start.X.Value;
            var x2 = crossingLine.End.X.Value;
            var x3 = line.Start.X.Value;
            var x4 = line.End.X.Value;
            var y1 = crossingLine.Start.Y.Value;
            var y2 = crossingLine.End.Y.Value;
            var y3 = line.Start.Y.Value;
            var y4 = line.End.Y.Value;

            if (Math.Max(x1, x2) < Math.Min(x3, x4))
                return false;

            var a1 = (y1 - y2) / (x1 - x2);
            var a2 = (y3 - y4) / (x3 - x4);
            var b1 = y1 - a1 * x1;
            var b2 = y3 - a2 * x3;

            var Xa = (b2 - b1) / (a1 - a2);
            if ((Xa < Math.Max(Math.Min(x1, x2), Math.Min(x3, x4))) ||
                 (Xa > Math.Min(Math.Max(x1, x2), Math.Max(x3, x4))))
                return false;
            else
                return true;

        }
    }
}
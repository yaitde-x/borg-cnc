using System.Collections.Generic;
using Yuni.CoordinateSpace;

namespace Borg.Machine
{

    public class InterpolationBlockRewriter : IBlockRewriter
    {
        private readonly InterpolationDirection _direction;

        public InterpolationBlockRewriter(InterpolationDirection direction)
        {
            this._direction = direction;
        }

        public RS274Instruction[] Rewrite(CoordinateBounds workEnvelope, RS274State rewriterState, RS274State machineState, RS274RewriterContext rewriterContext, RS274Instruction originalBlock)
        {
            var block = originalBlock;
            var start = new Coordinate(machineState.X, machineState.Y);
            var xStart = machineState.X;
            var yStart = machineState.Y;
            var offsetX = originalBlock.Parameters["I"];
            var offsetY = originalBlock.Parameters["J"];

            var leftLine = new Line(workEnvelope.MinX, workEnvelope.MinY, workEnvelope.MinX, workEnvelope.MaxY);
            var rightLine = new Line(workEnvelope.MaxX, workEnvelope.MinY, workEnvelope.MaxX, workEnvelope.MaxY);
            var topLine = new Line(workEnvelope.MinX, workEnvelope.MinY, workEnvelope.MaxX, workEnvelope.MinY);
            var bottomLine = new Line(workEnvelope.MinX, workEnvelope.MaxY, workEnvelope.MaxX, workEnvelope.MaxY);

            // var oppositeAndAdjacent = Interpolation.GetOppositeAndAdjacent(offsetX, offsetY);
            // var radius = Interpolation.Hypotenuse(oppositeAndAdjacent.Item1, oppositeAndAdjacent.Item2);
            // var angle = Interpolation.AbsAngle(offsetX, offsetY);
            var oneDegreePoints = RewriterUtilities.StepCircle(_direction, start, offsetX, offsetY);
            var crossingSections = new List<(double, double, double)>();
            var s = start;
            foreach (var point in oneDegreePoints)
            {
                var e = new Coordinate(point.Item2, point.Item3);
                if (RewriterUtilities.CrossesLine(s, e, leftLine))
                    crossingSections.Add(point);
                if (RewriterUtilities.CrossesLine(s, e, topLine))
                    crossingSections.Add(point);
                if (RewriterUtilities.CrossesLine(s, e, rightLine))
                    crossingSections.Add(point);
                if (RewriterUtilities.CrossesLine(s, e, bottomLine))
                    crossingSections.Add(point);

                s = e;
            }
            return new[] { block };
        }

    }

}
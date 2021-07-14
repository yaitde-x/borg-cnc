using Yuni.CoordinateSpace;

namespace Yuni.CoordinateSpace
{
    public enum LineDescriptor
    {
        Unknown = 0, Excluded = 1, Enters = 2, Leaves = 3, Contained = 4, Crosses = 5
    }

    public class Line
    {
        public Line(double x1, double y1, double x2, double y2)
        {
            Start = new Coordinate(x1, y1);
            End = new Coordinate(x2, y2);
        }

        public Line(Coordinate start, Coordinate end)
        {
            Start = start;
            End = end;
        }

        public Coordinate Start { get; set; }
        public Coordinate End { get; set; }
    }
}
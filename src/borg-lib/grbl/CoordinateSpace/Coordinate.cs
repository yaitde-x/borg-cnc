namespace Yuni.CoordinateSpace
{
    public class Scale
    {
        public Scale(decimal? xScale = null, decimal? yScale = null, decimal? zScale = null)
        {
            X = xScale; Y = yScale; Z = zScale;
        }

        public decimal? X { get; private set; }
        public decimal? Y { get; private set; }
        public decimal? Z { get; private set; }
    }

    public static class OriginUtilities
    {
        public static Origin Inverse(this Origin original)
        {
            var x = original.Offset.X.HasValue ? original.Offset.X.Value * -1 : default(double?);
            var y = original.Offset.Y.HasValue ? original.Offset.Y.Value * -1 : default(double?);
            var z = original.Offset.Z.HasValue ? original.Offset.Z.Value * -1 : default(double?);
            return new Origin(new Coordinate(x, y, z), original.Scale);
        }
    }
    
    public class Origin
    {
        public static Origin Default { get; private set; } = new Origin(new Coordinate(0, 0, 0), new Scale(1, 1, 1));

        public Origin(Coordinate offset, Scale scale)
        {
            Offset = offset; Scale = scale;
        }

        public Coordinate Offset { get; private set; }
        public Scale Scale { get; private set; }
    }

    public class Coordinate
    {
        public Coordinate()
        {

        }
        public Coordinate(double? x, double? y)
        {
            X = x;
            Y = y;
        }

        public Coordinate(double? x, double? y, double? z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double? X { get; internal set; }
        public double? Y { get; internal set; }
        public double? Z { get; internal set; }
        public double? A { get; internal set; }
        public double? B { get; internal set; }
        public double? E { get; internal set; }

        public override bool Equals(object obj)
        {
            return this == (obj as Coordinate);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public static Coordinate operator +(Coordinate a, Coordinate b) => CoordinateMath.Add(a, b);
        public static Coordinate operator -(Coordinate a, Coordinate b) => CoordinateMath.Subtract(a, b);
        public static bool operator ==(Coordinate a, Coordinate b)
        {

            var equals = true;

            if (object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null))
                return true;
            if (!object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null))
                return false;
            if (object.ReferenceEquals(a, null) && !object.ReferenceEquals(b, null))
                return false;
            if (a.X.HasValue && !b.X.HasValue || !a.X.HasValue && b.X.HasValue)
                return false;
            if (a.Y.HasValue && !b.Y.HasValue || !a.Y.HasValue && b.Y.HasValue)
                return false;
            if (a.Z.HasValue && !b.Z.HasValue || !a.Z.HasValue && b.Z.HasValue)
                return false;

            if (a.X.HasValue && b.X.HasValue)
                equals = equals && a.X == b.X;

            if (a.Y.HasValue && b.Y.HasValue)
                equals = equals && a.Y == b.Y;

            if (a.Z.HasValue && b.Z.HasValue)
                equals = equals && a.Z == b.Z;

            return equals;
        }

        public static bool operator !=(Coordinate a, Coordinate b)
        {
            var equals = true;

            if (object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null))
                return false;
            if (!object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null))
                return true;
            if (object.ReferenceEquals(a, null) && !object.ReferenceEquals(b, null))
                return true;
            if (a.X.HasValue && !b.X.HasValue || !a.X.HasValue && b.X.HasValue)
                return true;
            if (a.Y.HasValue && !b.Y.HasValue || !a.Y.HasValue && b.Y.HasValue)
                return true;
            if (a.Z.HasValue && !b.Z.HasValue || !a.Z.HasValue && b.Z.HasValue)
                return true;

            if (a.X.HasValue && b.X.HasValue)
                equals = equals && a.X == b.X;

            if (a.Y.HasValue && b.Y.HasValue)
                equals = equals && a.Y == b.Y;

            if (a.Z.HasValue && b.Z.HasValue)
                equals = equals && a.Z == b.Z;

            return !equals;
        }
    }

    public static class CoordinateMath
    {
        public static Coordinate Copy(this Coordinate coordinate)
        {
            return new Coordinate()
            {
                X = coordinate.X,
                Y = coordinate.Y,
                Z = coordinate.Z,
                A = coordinate.A,
                B = coordinate.B
            };
        }
        public static Coordinate Add(this Coordinate value, Coordinate offset)
        {
            return new Coordinate()
            {
                X = value.X.Value + offset.X.Value,
                Y = value.Y.Value + offset.Y.Value,
                Z = value.Z.Value + offset.Z.Value
            };
        }

        public static Coordinate Subtract(this Coordinate value, Coordinate offset)
        {
            return new Coordinate()
            {
                X = value.X.Value - offset.X.Value,
                Y = value.Y.Value - offset.Y.Value,
                Z = value.Z.Value - offset.Z.Value
            };
        }
    }
}
using System;
using System.Collections.Generic;

namespace Yuni.CoordinateSpace
{
    public static class CoordinateUtilities
    {

        public static Coordinate CloneAndOffset(this Coordinate original, Origin offset)
        {
            var x = original.X.HasValue ? original.X.Value + offset.Offset.X : null;
            var y = original.Y.HasValue ? original.Y.Value + offset.Offset.Y : null;
            return new Coordinate(x, y, original.Z);
        }

        public static CoordinateBounds CloneAndOffset(this CoordinateBounds original, Origin offset)
        {
            return new CoordinateBounds()
            {
                MinX = original.MinX + (offset.Offset.X.HasValue ? offset.Offset.X.Value : 0),
                MaxX = original.MaxX + (offset.Offset.X.HasValue ? offset.Offset.X.Value : 0),
                MinY = original.MinY + (offset.Offset.Y.HasValue ? offset.Offset.Y.Value : 0),
                MaxY = original.MaxY + (offset.Offset.Y.HasValue ? offset.Offset.Y.Value : 0),
                MinZ = original.MinZ + (offset.Offset.Z.HasValue ? offset.Offset.Z.Value : 0),
                MaxZ = original.MaxZ + (offset.Offset.Z.HasValue ? offset.Offset.Z.Value : 0)
            };
        }

        public static Coordinate FindClosestCoordinate(Coordinate p1, IEnumerable<Coordinate> coords)
        {
            var dist = decimal.MaxValue;
            var selectedCoord = default(Coordinate);

            foreach (var c in coords)
            {
                var d = Distance(p1, c);

                if (d < dist)
                {
                    selectedCoord = c;
                    dist = d;
                }
            }

            return selectedCoord;

        }

        public static decimal Distance(Coordinate p1, Coordinate p2)
        {
            return Convert.ToDecimal(Math.Sqrt(Math.Pow(p2.X.Value - p1.X.Value, 2) + Math.Pow(p2.Y.Value - p1.Y.Value, 2)));
        }

        public static bool IsInBounds(this CoordinateBounds bounds, Coordinate coordinate)
        {
            return (
                        !coordinate.X.HasValue ||
                        (coordinate.X.HasValue && ((coordinate.X.Value >= bounds.MinX) && (coordinate.X.Value <= bounds.MaxX)))
                    )
                    &&
                    (
                        !coordinate.Y.HasValue ||
                        (coordinate.Y.HasValue && ((coordinate.Y.Value >= bounds.MinY) && (coordinate.Y.Value <= bounds.MaxY)))
                    )
                    &&
                    (
                        !coordinate.Z.HasValue ||
                        (coordinate.Z.HasValue && ((coordinate.Z.Value >= bounds.MinZ) && (coordinate.Z.Value <= bounds.MaxZ)))
                    );
        }
    }
}
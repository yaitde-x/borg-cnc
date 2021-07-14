namespace Yuni.CoordinateSpace
{
    public static class CoordinateTranslation
    {

        public static class Direction {
            public const int ToWorld = 1;
            public const int ToCell = -1;
        }

        public static Origin CreateOrigin(CoordinateBounds cellBounds, int row, int column)
        {
            var cellWidth = cellBounds.MaxX - cellBounds.MinX;
            var cellHeight = cellBounds.MaxY - cellBounds.MinY;

            var xOffset = (cellWidth * column);
            var yOffset = (cellHeight * row);

            return new Origin(new Coordinate(xOffset, yOffset), new Scale(1));
        }
        public static Coordinate Translate(Coordinate cellCoord, Origin offset, int direction)
        {
            return new Coordinate(HandleNullAdd(cellCoord.X, offset.Offset.X.Value * direction), HandleNullAdd(cellCoord.Y, offset.Offset.Y.Value * direction), cellCoord.Z);
        }

        public static Coordinate Translate(CoordinateBounds worldBounds, CoordinateBounds cellBounds, int row, int column, int direction, Coordinate cellCoord)
        {
            var origin = CreateOrigin(cellBounds, row, column);
            return Translate(cellCoord, origin, direction);
        }

        public static Coordinate ToCell(CoordinateBounds worldBounds, CoordinateBounds cellBounds, int row, int column, Coordinate worldCoord)
        {

            var cellWidth = cellBounds.MinX - cellBounds.MaxX;
            var cellHeight = cellBounds.MinY - cellBounds.MaxY;

            var xOffset = cellWidth * column;
            var yOffset = cellHeight * row;

            return new Coordinate(HandleNullAdd(worldCoord.X, -xOffset), HandleNullAdd(worldCoord.Y, -yOffset), worldCoord.Z);
        }

        private static double? HandleNullAdd(double? value, double valueToAdd)
        {
            return value.HasValue ? value.Value + valueToAdd : valueToAdd;
        }
    }
}
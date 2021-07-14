using System;
using System.Collections.Generic;
using Xunit;
using Yuni.CoordinateSpace;

namespace Yuni.Grbl.CoordinateSpace.Tests
{

    public class CoordinateTests
    {

        public class CoordinateTestData : TheoryData<Coordinate, Coordinate, bool>
        {
            public CoordinateTestData()
            {
                Add(new Coordinate(0, 0, 0), new Coordinate(0, 0, 0), true);
                Add(new Coordinate(0, 0), new Coordinate(0, 0, 0), false);
                Add(new Coordinate(0, 0, 0), new Coordinate(1, 0, 0), false);
                Add(new Coordinate(0, 0), new Coordinate(0, 0), true);
                Add(new Coordinate(0, 0), null, false);
                Add(null, new Coordinate(0, 0, 1), false);
                Add(new Coordinate(1, 1, 1), new Coordinate(1, 1, 1), true);
            }
        }

        [Theory]
        [ClassData(typeof(CoordinateTestData))]
        public void EqualsTest(Coordinate a, Coordinate b, bool expected)
        {
            Assert.Equal(expected, a == b);
        }

        [Theory]
        [ClassData(typeof(CoordinateTestData))]
        public void NotEqualsTest(Coordinate a, Coordinate b, bool expected)
        {
            Assert.Equal(!expected, a != b);
        }
    }

    public class CoordinateUtilitiesTests
    {
        public class CoordinateTestData : TheoryData<CoordinateBounds, Coordinate, bool>
        {
            public CoordinateTestData()
            {
                Add(new CoordinateBounds()
                {
                    MinX = 10,
                    MaxX = 20,
                    MinY = 10,
                    MaxY = 20,
                    MinZ = -5,
                    MaxZ = 10
                }, new Coordinate(15, 15, 5), true);

                Add(new CoordinateBounds()
                {
                    MinX = 0,
                    MaxX = 5,
                    MinY = 0,
                    MaxY = 5,
                    MinZ = -5,
                    MaxZ = 5
                }, new Coordinate(5, 5), true);

                Add(new CoordinateBounds()
                {
                    MinX = 10,
                    MaxX = 20,
                    MinY = 10,
                    MaxY = 20,
                    MinZ = -5,
                    MaxZ = 10
                }, new Coordinate(25, 15, 5), false);

                Add(new CoordinateBounds()
                {
                    MinX = 10,
                    MaxX = 20,
                    MinY = 10,
                    MaxY = 20,
                    MinZ = -5,
                    MaxZ = 10
                }, new Coordinate(15, 5, 5), false);

                Add(new CoordinateBounds()
                {
                    MinX = 10,
                    MaxX = 20,
                    MinY = 10,
                    MaxY = 20,
                    MinZ = -5,
                    MaxZ = 10
                }, new Coordinate(15, 15, 15), false);
            }
        }

        [Theory]
        [ClassData(typeof(CoordinateTestData))]
        public void IsInBounds_InBounds_Success(CoordinateBounds bounds, Coordinate coordinate, bool expected)
        {
            var result = CoordinateUtilities.IsInBounds(bounds, coordinate);
            Assert.Equal(expected, result);
        }

        public class ClosestPointTestData : TheoryData<Coordinate, Coordinate[], Coordinate>
        {
            public ClosestPointTestData()
            {
                Add(new Coordinate(10, 10),
                    new[] {
                        new Coordinate(12, 12),
                        new Coordinate(15, 15),
                        new Coordinate(11, 11)
                        },
                    new Coordinate(11, 11));

                Add(new Coordinate(35, 15),
                new[] {
                        new Coordinate(45, 30),
                        new Coordinate(35, 10),
                        new Coordinate(40, 20),
                        new Coordinate(25, 20),
                        new Coordinate(25, 40),
                        new Coordinate(65, 5)
                    },
                new Coordinate(35, 10));

                Add(new Coordinate(65, 5),
                new[] {
                        new Coordinate(45, 30),
                        new Coordinate(35, 10),
                        new Coordinate(40, 20),
                        new Coordinate(25, 20),
                        new Coordinate(25, 40),
                        new Coordinate(35, 15)
                    },
                new Coordinate(40, 20));
            }
        }

        [Theory]
        [ClassData(typeof(ClosestPointTestData))]
        public void ClosestPoint(Coordinate currentPoint, Coordinate[] points, Coordinate expected)
        {
            var result = CoordinateUtilities.FindClosestCoordinate(currentPoint, points);
            Assert.Equal(expected, result);
        }

        public class DistanceTestData : TheoryData<Coordinate, Coordinate, decimal>
        {
            public DistanceTestData()
            {
                Add(new Coordinate(35, 15), new Coordinate(65, 5), 31.623m);
                Add(new Coordinate(65, 5), new Coordinate(40, 20), 29.155m);
            }
        }

        [Theory]
        [ClassData(typeof(DistanceTestData))]
        public void Distance(Coordinate point1, Coordinate point2, decimal expected)
        {
            var result = CoordinateUtilities.Distance(point1, point2);
            Assert.Equal(expected, Math.Round(result, 3, MidpointRounding.AwayFromZero));
        }
    }
}
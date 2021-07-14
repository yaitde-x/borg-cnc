using Xunit;
using Yuni.CoordinateSpace;

namespace Yuni.Grbl.CoordinateSpace.Tests
{
    public class LineUtilitiesTests
    {
        public class DescribeLineTestData : TheoryData<Line, CoordinateBounds, LineDescriptor>
        {
            public DescribeLineTestData()
            {
                Add(new Line(-7, 0, 7, 0), new CoordinateBounds(-5, 5, 5, -5), LineDescriptor.Crosses);
                Add(new Line(-5, 5, 10, 5), new CoordinateBounds(0, 0, 15, 15), LineDescriptor.Enters);
                Add(new Line(5, 5, 20, 5), new CoordinateBounds(0, 0, 15, 15), LineDescriptor.Leaves);
                Add(new Line(5, 5, 10, 5), new CoordinateBounds(0, 0, 15, 15), LineDescriptor.Contained);
                Add(new Line(5, 25, 10, 25), new CoordinateBounds(0, 0, 15, 15), LineDescriptor.Excluded);
                Add(new Line(0, 0, 15, 15), new CoordinateBounds(0, 0, 15, 15), LineDescriptor.Contained);
                Add(new Line(-5, -5, 10, 10), new CoordinateBounds(0, 0, 5, 5), LineDescriptor.Crosses);
                Add(new Line(0, 24, 24, 0), new CoordinateBounds(0, 0, 12, 12), LineDescriptor.Excluded);
                Add(new Line(24, 24, 0, 0), new CoordinateBounds(0, 0, 12, 12), LineDescriptor.Enters);
                Add(new Line(0, 24, 0, 0), new CoordinateBounds(0, 0, 12, 12), LineDescriptor.Enters);
                Add(new Line(0, 0, 24, 0), new CoordinateBounds(0, 0, 12, 12), LineDescriptor.Leaves);
            }
        }

        [Theory]
        [ClassData(typeof(DescribeLineTestData))]
        public void DescribeLineTest(Line line, CoordinateBounds bounds, LineDescriptor expectedResult)
        {
            var result = LineUtilities.DescribeLine(line, bounds);
            Assert.Equal(expectedResult, result.Item1);
        }

        public class IntersectTestData : TheoryData<Line, Line, Coordinate>
        {
            public IntersectTestData()
            {
                Add(new Line(-5, 30, 40, -5), new Line(15, 35, 15, -10), new Coordinate(15, 14.444444444444445));
                Add(new Line(10, 25, 25, 25), new Line(15, 35, 15, -10), new Coordinate(15, 25));
                Add(new Line(20.0000002980232239, 30.0000004470348358, 20.0000002980232239, 15.0000002235174179), new Line(15, 35, 15, -10), null);
                Add(new Line(5, 65, 50, 0), new Line(35, 40, 85, 70), null);
                Add(new Line(0, 50, 0, 0), new Line(5, 25, 50, 25), null);
                Add(new Line(0, 50, 0, 0), new Line(0, 25, 50, 25), new Coordinate(0, 25));
                Add(new Line(0, 50, 0, 0), new Line(0, 25, 50, 25), new Coordinate(0, 25));
            }
        }

        [Theory]
        [ClassData(typeof(IntersectTestData))]
        public void FindIntersectionTest(Line line1, Line line2, Coordinate expectedResult)
        {
            var result = LineUtilities.FindIntersection(line1, line2);
            Assert.Equal(expectedResult, result);
        }

        public class IsLineEnteringTestData : TheoryData<Line, CoordinateBounds, bool>
        {
            public IsLineEnteringTestData()
            {
                Add(new Line(-5, 5, 10, 5), new CoordinateBounds(0, 0, 15, 15), true);
                Add(new Line(5, 25, 5, 10), new CoordinateBounds(0, 0, 15, 15), true);
                Add(new Line(25, 10, 5, 10), new CoordinateBounds(0, 0, 15, 15), true);
                Add(new Line(10, -5, 10, 5), new CoordinateBounds(0, 0, 15, 15), true);
                Add(new Line(-5, -5, -5, 5), new CoordinateBounds(0, 0, 15, 15), false);
                Add(new Line(25, -5, 25, 5), new CoordinateBounds(0, 0, 15, 15), false);
                Add(new Line(5, -5, 25, -5), new CoordinateBounds(0, 0, 15, 15), false);
            }
        }

        [Theory]
        [ClassData(typeof(IsLineEnteringTestData))]
        public void IsLineEnteringTests(Line line, CoordinateBounds bounds, bool expectedResult)
        {
            var result = LineUtilities.IsLineEntering(line, bounds);

            Assert.Equal(expectedResult, result);
        }

    }
}
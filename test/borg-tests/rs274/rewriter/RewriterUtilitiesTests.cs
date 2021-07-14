using Borg.Machine;
using Xunit;
using Yuni.CoordinateSpace;

namespace borg_tests
{
    public class RewriterUtilitiesTests
    {
        [Fact]
        public void WalkCircleTestCW()
        {
            var start = new Coordinate(8.569591054, 0.7497420683);
            RewriterUtilities.StepCircle(InterpolationDirection.Clockwise, start, -8.569591054, -0.7497420683);
        }

        [Fact]
        public void WalkCircleTestCCW()
        {
            var start = new Coordinate(8.569591054, 0.7497420683);
            RewriterUtilities.StepCircle(InterpolationDirection.CounterClockwise, start, -8.569591054, -0.7497420683);
        }

        [Theory]
        [InlineData(10.0000001490116119, 30.0000004470348358, 25.0000003725290298, 44.0000006556510925,
                    15.0000002235174179, 21.0000003129243851, 17.0000002533197403, 34.0000005066394806, false)]
        [InlineData(10.0000001490116119, 30.0000004470348358, 25.0000003725290298, 44.0000006556510925,
                    9.0000001341104507, 35.0000005215406418, 19.0000002831220627, 44.0000006556510925, false)]
        [InlineData(10.0000001490116119, 30.0000004470348358, 25.0000003725290298, 44.0000006556510925,
                    5.4728067820121105, 31.9219932966094682, 11.0000001639127731, 26.000000387430191, false)]
        [InlineData(10.0000001490116119, 30.0000004470348358, 25.0000003725290298, 44.0000006556510925,
                    27.0000004023313522, 40.0000005960464478, 3.0000000447034836, 37.0000005513429642, true)]
        [InlineData(10.0000001490116119, 30.0000004470348358, 25.0000003725290298, 44.0000006556510925,
                    7.0000001043081284, 38.0000005662441254, 25.0000003725290298, 44.0000006556510925, true)]
        [InlineData(10.0000001490116119, 30.0000004470348358, 25.0000003725290298, 44.0000006556510925,
                    16.4886557663167288, 36.0560790231862782, 10.0000001490116119, 30.0000004470348358, true)]
        public void CrossesLineTest(double x1, double y1, double x2, double y2,
                                    double x3, double y3, double x4, double y4, bool expected)
        {

            var crosses = RewriterUtilities.CrossesLine(new Coordinate(x1, y1), new Coordinate(x2, y2),
                                                        new Line(new Coordinate(x3, y3), new Coordinate(x4, y4)));
            Assert.Equal(expected, crosses);

        }

        [Fact]
        public void IsLeavingWorkEnvelope_Leaving_Success()
        {
            var currentCoordinate = new Coordinate(5, 5, 1);
            var targetCoordinate = new Coordinate(20, 20, 1);
            var bounds = new CoordinateBounds() { MinX = 0, MaxX = 10, MinY = 0, MaxY = 10, MinZ = -1, MaxZ = 10 };

            var result = RewriterUtilities.IsLeavingWorkEnvelope(currentCoordinate, targetCoordinate, bounds);
            Assert.True(result);
        }

        [Fact]
        public void IsLeavingWorkEnvelope_NotLeaving_Success()
        {
            var currentCoordinate = new Coordinate(5, 5, 1);
            var targetCoordinate = new Coordinate(5, 7, 1);
            var bounds = new CoordinateBounds() { MinX = 0, MaxX = 10, MinY = 0, MaxY = 10, MinZ = -1, MaxZ = 10 };

            var result = RewriterUtilities.IsLeavingWorkEnvelope(currentCoordinate, targetCoordinate, bounds);
            Assert.False(result);
        }

        [Fact]
        public void IsEnteringWorkEnvelope_Entering_Success()
        {
            var currentCoordinate = new Coordinate(-5, -5, 1);
            var targetCoordinate = new Coordinate(5, 5, 1);
            var bounds = new CoordinateBounds() { MinX = 0, MaxX = 10, MinY = 0, MaxY = 10, MinZ = -1, MaxZ = 10 };

            var result = RewriterUtilities.IsEnteringWorkEnvelope(currentCoordinate, targetCoordinate, bounds);
            Assert.True(result);
        }

        [Fact]
        public void IsEnteringWorkEnvelope_NotEntering_Success()
        {
            var currentCoordinate = new Coordinate(-5, -5, 1);
            var targetCoordinate = new Coordinate(-5, -7, 1);
            var bounds = new CoordinateBounds() { MinX = 0, MaxX = 10, MinY = 0, MaxY = 10, MinZ = -1, MaxZ = 10 };

            var result = RewriterUtilities.IsEnteringWorkEnvelope(currentCoordinate, targetCoordinate, bounds);
            Assert.False(result);
        }

    }
}
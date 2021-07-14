using System;
using Borg.Machine;
using Xunit;
using Yuni.CoordinateSpace;

namespace borg_tests
{

    public class InterpolationTests
    {
        [Fact]
        public void BasicClockwiseTest()
        {

            var start = new CoordinateBuilder().WithX(41.7460041354522104).WithY(62.069031631983357).Build();
            var offset = new CoordinateBuilder().WithX(3.2539965351000433).WithY(-12.0690308869253027).Build();
            var end = new CoordinateBuilder().WithX(56.3272899204930653).WithY(55.2860690616971056).Build();
            var envelope = new CoordinateBounds()
            {
                MinX = 50.0000007450580597,
                MaxX = 70.0000010430812836,
                MinY = 37.0000005513429642,
                MaxY = 63.0000009387731552
            };

            var intersect = Interpolation.Intersect(1, start, offset, end, envelope);

            Assert.Equal(Math.Round(50.0000007450580597, MachineConstants.Precision),
                        Math.Round(intersect.X.Value, MachineConstants.Precision));
            Assert.Equal(Math.Round(61.4564399499306617, MachineConstants.Precision),
                        Math.Round(intersect.Y.Value, MachineConstants.Precision));

        }

        [Theory]
        [InlineData(3.7500, 6.495191, 7.500)]
        [InlineData(5.286068, 11.327289, 12.500)]
        [InlineData(5.988673, 10.972046, 12.500)]
        public void HypotenuseTest(double a, double b, double expected)
        {
            var result = Interpolation.Hypotenuse(a, b);
            Assert.Equal(Math.Round(expected, 6), Math.Round(result, 6));
        }


        [Theory]
        [InlineData(3.7500, 6.4951905283832874, 30.00)]
        [InlineData(5.2860683166390476, 11.3272892499408151, 25.016893)]
        [InlineData(5.9886731493729384, 10.9720460220498364, 28.626237)]
        public void AngleAFromOppositeAndAdjacentTest(double opposite, double adjacent, double expected)
        {
            var result = Interpolation.FindAngleAFromOppositeAndAdjacent(opposite, adjacent);
            Assert.Equal(Math.Round(expected, 6), Math.Round(result, 6));
        }

        [Theory]
        [InlineData(3.7500, 7.500, 30.00)]
        [InlineData(5.2860683166390476, 12.5, 25.016893)]
        [InlineData(5.9886731493729384, 12.5, 28.626237)]
        public void AngleAFromOppositeAndHypotenuseTest(double opposite, double hypotenuse, double expected)
        {
            var result = Interpolation.FindAngleAFromOppositeAndHypotenuse(opposite, hypotenuse);
            Assert.Equal(Math.Round(expected, 6), Math.Round(result, 6));
        }

        [Theory]
        [InlineData(6.4951905283832874, 7.5, 30.00)]
        [InlineData(11.3272892499408151, 12.5, 25.016893)]
        [InlineData(10.9720460220498364, 12.5, 28.626237)]
        public void AngleAFromAdjacentAndHypotenuseTest(double adjacent, double hypotenuse, double expected)
        {
            var result = Interpolation.FindAngleAFromAdjacentAndHypotenuse(adjacent, hypotenuse);
            Assert.Equal(Math.Round(expected, 6), Math.Round(result, 6));
        }

        [Theory]
        [InlineData(41.7460041354522104, 62.069031631983357, 3.2539965351000433, -12.0690308869253027, 45.0000006705522537, 50.0000007450580597)]
        public void FindCenterTest(double xstart, double ystart, double xoffset, double yoffset, double expectedX, double expectedY)
        {
            var result = Interpolation.FindCenter(xstart, ystart, xoffset, yoffset);
            Assert.Equal(Math.Round(expectedX, 6), Math.Round(result.Item1, 6));
            Assert.Equal(Math.Round(expectedY, 6), Math.Round(result.Item2, 6));
        }

        [Theory]
        [InlineData(22.6941539628377775, 73.8470779127414545, -7.6941537393203596, -3.8470768696601798, 26.565051177077994)]
        [InlineData(8.9172376025789735, 76.0827636640197227, 6.0827626209384444, -6.0827626209384444, 45.0)]
        [InlineData(11.0173333127947259, 62.3751491439552055, 3.982666910722692, 7.6248518991260728, 27.579285967125877)]
        [InlineData(23.09528514016759, 67.090299284545182, -8.0952849166501739, 2.9097017585360962, 19.770044081707141)]
        [InlineData(16.5539657076710611, 61.5391976840951429, -1.5539654841536432, 8.4608033589861353, 10.407320729918665)]
        [InlineData(12.8786013038779359, 78.3366471810149534, 2.1213989196394811, -8.3366461379336698, 14.276858496841065)]
        [InlineData(16.5799827921708385, 78.4559845629400598, 1.5799825686534219, -8.4559835198587763, 10.583556083886263)]
        public void FindAngle(double x1, double y1, double offsetX, double offsetY, double expected)
        {
            var angle = Interpolation.AbsAngle(offsetX, offsetY);
            Assert.Equal(expected, angle);
        }

        [Theory]
        // xcenter = 15.0000002235174179, ycenter = 70.0000010430812836
        [InlineData(22.6941539628377775, 73.8470779127414545, -7.6941537393203596, -3.8470768696601798, 26.565051177077994)]
        [InlineData(8.9172376025789735, 76.0827636640197227, 6.0827626209384444, -6.0827626209384444, 135.0)]
        [InlineData(11.0173333127947259, 62.3751491439552055, 3.982666910722692, 7.6248518991260728, 242.42071403287412)]
        [InlineData(23.09528514016759, 67.090299284545182, -8.0952849166501739, 2.9097017585360962, 340.22995591829283)]
        [InlineData(16.5539657076710611, 61.5391976840951429, -1.5539654841536432, 8.4608033589861353, 280.40732072991864)]
        [InlineData(12.8786013038779359, 78.3366471810149534, 2.1213989196394811, -8.3366461379336698, 104.27685849684107)]
        [InlineData(16.5799827921708385, 78.4559845629400598, -1.5799825686534219, -8.4559835198587763, 79.416443916113735)]
        public void PolarAngleTest(double x1, double y1, double offsetX, double offsetY, double expected)
        {
            var polarAngle = Interpolation.PolarAngle(x1, y1, offsetX, offsetY);
            Assert.Equal(expected, polarAngle);
        }

        [Theory]
        [InlineData(38.8429631178061783, 64.0347915468439055, 64.0000009536743164, 47.0000007003545761, 45.0000006705522537, 50.0000007450580597, 12.5, true)]
        [InlineData(37.6949999045645328, 63.5211125251555231, 41.0000006109476089, 65.0000009685754776, 45.0000006705522537, 50.0000007450580597, 12.5, false)]
        public void IntersectTest(double x1, double y1, double x2, double y2, double cx, double cy, double r, bool expected)
        {
            var intersect = Interpolation.Intersect(x1, y1, x2, y2, cx, cy, r);
            Assert.Equal(expected, intersect);
        }

        [Theory]
        [InlineData(-7.6941537393203596, -3.8470768696601798, Quadrant.Q1)]
        [InlineData(6.0827626209384444, -6.0827626209384444, Quadrant.Q2)]
        [InlineData(6.2887752993203101, 5.8695236126989592, Quadrant.Q3)]
        [InlineData(-6.0827626209384444, 6.0827626209384444, Quadrant.Q4)]
        public void QuadrantFromCenterOffsetTest(double offsetX, double offsetY, Quadrant expected)
        {
            var quadrant = Interpolation.QuadrantFromOffset(offsetX, offsetY);
            Assert.Equal(expected, quadrant);
        }

        [Theory]
        [InlineData(79.416444, 8.6023255, 1.5799825755166186,8.4559836251623679)]
        [InlineData(352.538146, 8.6023255,8.52947707201328, -1.117150341695323)]
        [InlineData(169.4, 8.6023255,-8.4555298188406116, 1.5824093941352444)]
        [InlineData(199.804693, 8.6023255,-8.0935239287112744, -2.9145967857198114)]
        [InlineData(257.975746, 8.6023255,-1.7920857734447673, -8.413586190713632)]
        [InlineData(290.477347, 8.6023255,3.0094119381432396, -8.0587495180400772)]
        public void GetXYOffsetFromAngleAndRadius(double angle, double radius, double expectedX, double expectedY) {
            var offsets = Interpolation.GetXYOffsetFromAngleAndRadius(angle, radius);
            Assert.Equal(expectedX, offsets.Item1);
            Assert.Equal(expectedY, offsets.Item2);
        }
    }
}
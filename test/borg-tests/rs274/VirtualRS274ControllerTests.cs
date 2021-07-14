using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Borg.Machine;
using Xunit;
using Yuni.CoordinateSpace;

namespace borg_tests
{
    public class VirtualRS274ControllerTests
    {
        private VirtualRS274Controller GetTestController()
        {
            var parser = new RS274Parser();
            var bounds = new CoordinateBounds(0, 0, -10, 20, 20, 10);
            var controller = new VirtualRS274Controller(parser, bounds);
            return controller;
        }

        [Fact]
        public async Task Stream_RapidMoveOnly_Success()
        {
            var controller = GetTestController();

            using (var stream = await GetTestFile1())
            {
                var result = await controller.Stream(stream);

                Assert.Equal(5, result.X);
                Assert.Equal(10, result.Y);
                Assert.Equal(15, result.Z);
            }
        }

        [Fact]
        public async Task Stream_RapidThenMove_Success()
        {
            var controller = GetTestController();

            using (var stream = await GetTestFile2())
            {
                var result = await controller.Stream(stream);

                Assert.Equal(5, result.X);
                Assert.Equal(5, result.Y);
                Assert.Equal(0, result.Z);
                Assert.Equal(100, result.Feed);
            }
        }

        private Task<Stream> GetTestFile1()
        {
            return GetTestStream(new List<string>() { "G0 X5 Y10 Z15" });
        }

        private Task<Stream> GetTestFile2()
        {
            return GetTestStream(new List<string>() { "G0 X0 Y0 Z15", "G1 X5 Y5 Z0 F100" });
        }

        private async Task<Stream> GetTestStream(List<string> lines)
        {
            var ms = new MemoryStream();

            using (var writer = new StreamWriter(ms, Encoding.Default, 2048, true))
            {
                lines.ForEach(async l => await writer.WriteLineAsync(l));
            }

            await ms.FlushAsync();
            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }

    }
}
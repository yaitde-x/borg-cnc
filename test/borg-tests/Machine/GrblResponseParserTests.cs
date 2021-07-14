using Xunit;

namespace Borg.Machine.Tests
{

    public class GrblResponseParserTests
    {

        [Fact]
        public void ParseStatus_Success()
        {
            var resp = "<Idle|MPos:5.000,10.000,20.000|FS:10,1100>";
            var testObject = new GrblResponseParser();
            var result = testObject.ParseStatus(resp);

            Assert.Equal(5, result.X);
            Assert.Equal(10, result.Y);
            Assert.Equal(20, result.Z);
            Assert.Equal(10, result.Feed);
            Assert.Equal(1100, result.Speed);
            Assert.Equal(RunState.Idle, result.State);

        }

    }
}
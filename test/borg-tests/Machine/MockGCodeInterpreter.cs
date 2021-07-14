using Moq;
using Yuni.CoordinateSpace;

namespace Borg.Machine.Tests
{

    public class MockGCodeInterpreter : IRS274Interpreter
    {
        private readonly Mock<IRS274Interpreter> _mock;

        public MockGCodeInterpreter()
        {
            _mock = new Mock<IRS274Interpreter>();
        }
        public void SetupFeed(string gCode)
        {
            _mock.Setup(m => m.Feed(It.IsAny<Coordinate>(), It.IsAny<decimal>())).Returns(gCode);
        }

        public void SetupRapid(string gCode)
        {
            _mock.Setup(m => m.Rapid(It.IsAny<Coordinate>()));
        }

        public void SetupHome(string gCode)
        {
            _mock.Setup(m => m.Home()).Returns(gCode);
        }

        public string Feed(Coordinate pos, decimal feedRate)
        {
            return _mock.Object.Feed(pos, feedRate);
        }

        public string Home()
        {
            return _mock.Object.Home();
        }

        public string Rapid(Coordinate pos)
        {
            return _mock.Object.Rapid(pos);
        }

        public void VerifyRapidWasCalled() {
            _mock.Verify(m => m.Rapid(It.IsAny<Coordinate>()), Times.Once);
        }
    }
}
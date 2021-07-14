
using Xunit;

namespace Borg.Machine.Tests {
    public class RunStateCoversionTests {
        [Fact]
        public void TryParseIdle_Success() {
            Assert.True(RunStateConversion.TryParse("Idle", out var state));
            Assert.Equal(RunState.Idle, state);
        }
        [Fact]
        public void TryParseRun_Success() {
            Assert.True(RunStateConversion.TryParse("Run", out var state));
            Assert.Equal(RunState.Running, state);
        }

    }
}
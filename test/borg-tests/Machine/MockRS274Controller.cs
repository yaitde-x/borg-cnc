
using System;
using System.Threading.Tasks;
using Moq;

namespace Borg.Machine.Tests
{
    public class MockRS274Controller : IRS274Controller
    {
        private readonly Mock<IRS274Controller> _mock;

        public MockRS274Controller() : this(RS274State.Default)
        {

        }
        public MockRS274Controller(RS274State initialState)
        {
            _mock = new Mock<IRS274Controller>();
            _mock.Setup(m => m.GetState()).ReturnsAsync(initialState);
        }

        public void SetStateAfterRunningBlock(string block, RS274State state)
        {
            _mock.Setup(m => m.GetState()).ReturnsAsync(state);
        }

        public Task<RS274State> GetState()
        {
            return _mock.Object.GetState();
        }

        public Task<GrblSettings> GetSettings()
        {
            return _mock.Object.GetSettings();
        }

        public Task<RS274EngineStatus> GetEngineStatus()
        {
            return _mock.Object.GetEngineStatus();
        }

        public Task<RS274Job> GetJob(string jobId)
        {
            return _mock.Object.GetJob(jobId);
        }

        public Task<RS274Job> Run(string gCodeBlock)
        {
            return _mock.Object.Run(gCodeBlock);
        }

        public Task<RS274Job> Run(RS274Instruction instruction)
        {
            return _mock.Object.Run(instruction);
        }

        public Task<RS274State> File(string fileName)
        {
            return _mock.Object.File(fileName);
        }

        internal void VerifyRunWasCalledWith(string gBlock)
        {
            _mock.Verify(m => m.Run(gBlock));
        }
    }
}
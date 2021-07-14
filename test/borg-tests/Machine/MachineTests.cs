
using System.Threading.Tasks;
using Moq;
using Xunit;
using Yuni.Library;

namespace Borg.Machine.Tests
{

    public class MachineTests
    {
        private IMachine CreateMachine()
        {
            var controller = new MockRS274Controller();
            return CreateMachine(controller);
        }

        private IMachine CreateMachine(IRS274Controller controller)
        {
            var mockRepo = new Mock<ILibraryRepo>();
            return CreateMachine(controller, new MockGCodeInterpreter());
        }

        private IMachine CreateMachine(IRS274Controller controller, IRS274Interpreter interpreter)
        {
            var mockRepo = new Mock<ILibraryRepo>();
            return new Machine(controller, interpreter, mockRepo.Object);
        }

        [Fact]
        public async Task Construct_StartsLockedAndHomed_Success()
        {
            var machine = CreateMachine();
            var state = await machine.GetState();

            Assert.Equal(0, state.X);
            Assert.Equal(0, state.Y);
            Assert.Equal(0, state.Z);
            Assert.Equal(RunState.Locked, state.State);
        }

        [Fact]
        public async Task Unlock_TransistionsTheMachineToIdle_Success()
        {
            var machine = CreateMachine();
            var state = await machine.UnLock();

            Assert.Equal(RunState.Idle, state.State);
        }

        [Fact]
        public async Task Run_UsesTheMachineController_Success()
        {
            var gBlock = "G0X10Y11Z12";
            var controller = new MockRS274Controller();

            controller.SetStateAfterRunningBlock(gBlock, new RS274StateBuilder()
                                                            .WithX(10)
                                                            .WithY(11)
                                                            .WithZ(12)
                                                            .WithFeed(0)
                                                            .WithSpeed(0)
                                                            .WithRunState(RunState.Idle)
                                                            .Build());

            var machine = CreateMachine(controller);

            await machine.UnLock();

            var job = await machine.ExecuteBlock(gBlock);

            controller.VerifyRunWasCalledWith(gBlock);
            var state = await machine.GetState();
            Assert.Equal(10, state.X);
            Assert.Equal(11, state.Y);
            Assert.Equal(12, state.Z);
        }

        [Fact]
        public async Task Run_UsesInterpreterToGetHome_Success()
        {
            var gBlock = "G0X0Y0Z0";
            var controller = new MockRS274Controller();
            controller.SetStateAfterRunningBlock(gBlock, new RS274StateBuilder()
                                                            .WithX(0)
                                                            .WithY(0)
                                                            .WithZ(0)
                                                            .WithFeed(0)
                                                            .WithSpeed(0)
                                                            .WithRunState(RunState.Idle)
                                                            .Build());

            var interpreter = new MockGCodeInterpreter();
            interpreter.SetupHome(gBlock);

            var machine = CreateMachine(controller, interpreter);

            await machine.UnLock();
            var status = await machine.Home();
            var state = await machine.GetState();

            Assert.Equal(RunState.Idle, state.State);
            controller.VerifyRunWasCalledWith(gBlock);
            Assert.Equal(0, state.X);
            Assert.Equal(0, state.Y);
            Assert.Equal(0, state.Z);
        }
    }
}
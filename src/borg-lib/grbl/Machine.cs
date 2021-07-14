using System;
using System.Threading.Tasks;
using Yuni.CoordinateSpace;
using Yuni.Library;

namespace Borg.Machine
{

    public class Machine : IMachine
    {
        private readonly IRS274Controller _machineController;
        private readonly IRS274Interpreter _interpreter;
        private readonly ILibraryRepo _library;

        private RS274State State { get; set; } = RS274State.Default;

        public Machine(IRS274Controller machineController, IRS274Interpreter interpreter, ILibraryRepo library)
        {
            _machineController = machineController;
            _interpreter = interpreter;
            _library = library;
        }

        public Task<CoordinateBounds> Bounds()
        {
            return Task.FromResult(new CoordinateBounds(0, 0, -10, 200, 300, 20));
        }

        private RS274State LockingOp(Func<RS274State> op)
        {
            lock (State)
            {
                return op();
            }
        }

        private Task<RS274State> LockingAsyncOp(Func<Task<RS274State>> op)
        {
            lock (State)
            {
                return op();
            }
        }

        public Task<RS274State> UnLock()
        {
            return Task.Run(() => LockingOp(() =>
            {
                if (State.State == RunState.Locked)
                    State.State = RunState.Idle;

                return State;
            }));
        }

        public Task<RS274State> Lock()
        {
            return Task.Run(() => LockingOp(() =>
            {
                if (State.State != RunState.Error)
                    State.State = RunState.Locked;

                return State;
            }));
        }

        public Task<RS274Job> Home()
        {
            var gCode = _interpreter.Home();
            return ExecuteBlock(gCode);
        }

        public Task<RS274State> GetState()
        {
            return _machineController.GetState();
        }

        public Task<GrblSettings> GetSettings()
        {
            return _machineController.GetSettings();
        }
        
        public Task<RS274Job> GetJob(string jobId)
        {
            return _machineController.GetJob(jobId);
        }

         public Task<RS274EngineStatus> GetEngineStatus()
        {
            return _machineController.GetEngineStatus();
        }
        public async Task<RS274Job> ExecuteBlock(string gCodeBlock)
        {

            // if (State.State != RunState.Idle)
            //     throw new MachineOperationException($"Can't run when unit is in state: {State.State}");

            var status = await _machineController.Run(gCodeBlock);
            // var state = await _machineController.GetState();
            // State.X = state.X;
            // State.Y = state.Y;
            // State.Z = state.Z;

            return status;

        }

        public async Task<RS274State> ExecuteFile(string fileName)
        {

            // if (State.State != RunState.Idle)
            //     throw new MachineOperationException($"Can't run when unit is in state: {State.State}");


            var state = await _machineController.File(fileName);
            State.X = state.X;
            State.Y = state.Y;
            State.Z = state.Z;

            return State;

        }

        public Task<RS274State> Stop()
        {
            return Task.Run(() => LockingOp(() =>
            {
                if (State.State == RunState.Running)
                    State.State = RunState.Idle;

                return State;
            }));
        }
    }
}
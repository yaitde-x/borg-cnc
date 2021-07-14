
using System;
using System.IO;
using System.Threading.Tasks;
using Yuni.CoordinateSpace;

namespace Borg.Machine
{

    public class VirtualRS274Controller : IRS274Controller
    {
        private readonly RS274Parser _parser;
        private RS274State _machineState = RS274State.Default;
        private RS274EngineStatus _engineStatus = new RS274EngineStatus();
        private object _mutex = new object();
        private CoordinateBounds _machineBounds;

        public VirtualRS274Controller(RS274Parser parser, CoordinateBounds machineBounds)
            : this(parser, machineBounds, RS274State.Default)
        {
        }

        public VirtualRS274Controller(RS274Parser parser, CoordinateBounds machineBounds, RS274State machineState)
        {
            _parser = parser;
            _machineState = machineState;
            _machineBounds = machineBounds;
        }

        public Task<RS274State> File(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                return Stream(stream);
            }
        }

        public async Task<RS274State> Stream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var block = await reader.ReadLineAsync();

                    if (!string.IsNullOrWhiteSpace(block))
                    {
                        var instruction = _parser.ParseLine(block);
                        await Run(instruction);
                    }
                }
            }

            return _machineState;
        }

        public Task<RS274EngineStatus> GetEngineStatus()
        {
            return Task.FromResult(new RS274EngineStatus());
        }

        public Task<RS274Job> GetJob(string jobId)
        {
            return Task.FromResult(new RS274Job() { Id = jobId });
        }

        public Task<RS274State> GetState()
        {
            return Task.FromResult(_machineState);
        }

        public Task<GrblSettings> GetSettings()
        {
            var settingsBundle = "$0=10$1=25$2=0$3=4$4=0$5=0$6=0$10=1$11=0.010$12=0.002$13=0$20=0$21=0$22=0$23=0$24=25.000$25=500.000$26=250$27=1.000$30=1000$31=0$32=0$100=410.000$101=405.000$102=405.000$110=1500.000$111=1500.000$112=1500.000$120=25.000$121=20.000$122=10.000$130=200.000$131=300.000$132=20.000";
            return Task.FromResult(new GrblSettings(GrblSettingsParser.Parse(settingsBundle)));
        }

        public Task<RS274Job> Run(string gCodeBlock)
        {
            var instruction = _parser.ParseLine(gCodeBlock);
            return Run(instruction);
        }

        public Task<RS274Job> Run(RS274Instruction instruction)
        {
            lock (_mutex)
            {
                _machineState = MutateState(_machineState, instruction);
            }

            return Task.FromResult(new RS274Job() { Id = Environment.TickCount.ToString() });
        }

        private RS274State MutateState(RS274State oldState, RS274Instruction instruction)
        {

            var builder = new RS274StateBuilder(oldState);
            var vector = instruction.TargetVector;

            if (vector.X.HasValue)
                builder.WithX(vector.X.Value);

            if (vector.Y.HasValue)
                builder.WithY(vector.Y.Value);

            if (vector.Z.HasValue)
                builder.WithZ(vector.Z.Value);

            builder.WithRunState(RunState.Idle);

            foreach (var m in instruction.Modals)
            {
                var function = RS274Functions.FunctionIndex()[m];
                builder.WithModal(function.Group, m);
            }

            var parameters = instruction.Parameters;
            if (parameters.ContainsKey("F"))
                builder.WithFeed(parameters["F"]);

            return builder.Build();
        }
    }
}
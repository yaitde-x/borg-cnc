using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Borg.Machine;
using Microsoft.AspNetCore.Mvc;
using Yuni.Library;

namespace borg_api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MachineController : ControllerBase
    {
        private readonly IMachine _machine;
        private readonly ILibraryRepo _libraryRepo;

        public class ExternalState
        {
            public decimal X { get; set; }
            public decimal Y { get; set; }
            public decimal Z { get; set; }
            public decimal Feed { get; set; }
            public decimal Speed { get; set; }
            public string State { get; set; }
            public string StatusMessage { get; set; }
        }

        public MachineController(IMachine machine, ILibraryRepo libraryRepo)
        {
            _machine = machine;
            _libraryRepo = libraryRepo;
        }

        [HttpPost("unlock")]
        public async Task<ActionResult<ExternalState>> UnlockMachine()
        {
            try
            {
                return ToExternalState(await _machine.UnLock());
            }
            catch (MachineOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("settings")]
        public async Task<ActionResult<ImmutableDictionary<string, decimal>>> GetGettings()
        {
            try
            {
                var settings = await _machine.GetSettings();
                return settings.AllSettings();
            }
            catch (MachineOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("state")]
        public async Task<ActionResult<ExternalState>> GetState()
        {
            try
            {
                return ToExternalState(await _machine.GetState());
            }
            catch (MachineOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("home")]
        public async Task<ActionResult<RS274Job>> HomeMachine()
        {
            try
            {
                return await _machine.Home();

            }
            catch (MachineOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class RunRequest
        {
            public string Block { get; set; }
        }

        [HttpGet("engine")]
        public async Task<ActionResult<RS274EngineStatus>> GetEngineStatus()
        {
            try
            {
                return await _machine.GetEngineStatus();

            }
            catch (MachineOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("job/{jobId}")]
        public async Task<ActionResult<RS274Job>> GetJob(string jobId)
        {
            try
            {
                return await _machine.GetJob(jobId);

            }
            catch (MachineOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("run")]
        public async Task<ActionResult<RS274Job>> Run(RunRequest request)
        {
            try
            {
                return await _machine.ExecuteBlock(request.Block);

            }
            catch (MachineOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class FileRequest
        {
            public string Id { get; set; }
            public string FileName { get; set; }
            public int Row { get; set; }
            public int Column { get; set; }
        }

        [HttpPost("file")]
        public async Task<ActionResult<ExternalState>> File(FileRequest request)
        {
            try
            {
                var sliceFileName = await _libraryRepo.GetSlicePath(request.Id, request.Row, request.Column);
                return ToExternalState(await _machine.ExecuteFile(sliceFileName));

            }
            catch (MachineOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private ExternalState ToExternalState(RS274State internalState)
        {
            var result = new ExternalState()
            {
                X = Convert.ToDecimal(Math.Round(internalState.X, MachineConstants.Precision)),
                Y = Convert.ToDecimal(Math.Round(internalState.Y, MachineConstants.Precision)),
                Z = Convert.ToDecimal(Math.Round(internalState.Z, MachineConstants.Precision)),
                Feed = Convert.ToDecimal(Math.Round(internalState.Feed, MachineConstants.Precision)),
                Speed = Convert.ToDecimal(Math.Round(internalState.Speed, MachineConstants.Precision)),
                State = Convert.ToString(internalState.State)
            };
            return result;
        }
    }
}

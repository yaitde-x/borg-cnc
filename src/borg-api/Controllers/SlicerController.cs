using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Borg.Machine;
using Microsoft.AspNetCore.Mvc;
using Utilities;
using Yuni.CoordinateSpace;
using Yuni.Grbl;
using Yuni.Library;
using Yuni.Rewriter;

namespace borg_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlicerController : ControllerBase
    {
        private readonly ILibraryRepo _libraryRepo;
        private readonly IMachine _machine;

        public SlicerController(ILibraryRepo libraryRepo, IMachine machine)
        {
            _libraryRepo = libraryRepo;
            _machine = machine;
        }

        // [HttpPost("slice/{id}")]
        // public async Task<ActionResult<string[]>> Slice(string id)
        // {
        //     try
        //     {

        //         var analysis = await _libraryRepo.GetAnalysis(id);

        //         var cellBounds = await _machine.Bounds();
        //         var machineState = RS274State.Default;
        //         var worldBounds = analysis.Bounds;

        //         var worldController = new VirtualRS274Controller(new RS274Parser(), worldBounds, machineState);

        //         var rewriterState = new RS274StateBuilder().WithExistingState(machineState).Build();
        //         var rewriterController = new VirtualRS274Controller(new RS274Parser(), cellBounds, rewriterState);
        //         var row = 1;
        //         var column = 1;

        //         var origin = CoordinateTranslation.CreateOrigin(cellBounds, row, column);

        //         var rewriter = new RS274Rewriter(analysis, origin, worldController, rewriterController);

        //         using (var stream = await _libraryRepo.Read(id))
        //         {
        //             var view = await rewriter.Rewrite(cellBounds, stream);

        //             using (var fs = await _libraryRepo.WriteSlice(id, 1, 1))
        //             {
        //                 var writer = new GrblRS274Writer();
        //                 await writer.Write(view, fs);
        //             }
        //         }

        //         return new string[0];
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(ex.Message);
        //     }
        // }

        [HttpPost("slice/{id}")]
        public async Task<ActionResult<SliceData>> Slice2(string id)
        {
            try
            {

                var meta = await _libraryRepo.GetMeta(id);
                var slicer = new RS274Slicer(new RS274Parser(), _libraryRepo, new CoordinateTranslatorFactory());
                var analysis = await _libraryRepo.GetAnalysis(id);
                var slicerData = default(SliceData);

                using (var stream = await _libraryRepo.Read(id))
                {

                    var worldBounds = analysis.Bounds;
                    var cellBounds = await _machine.Bounds();
                    var machineState = RS274State.Default;
                   
                    var slicerResults = await slicer.Slice(stream, id, analysis, worldBounds, cellBounds, machineState);
                    slicerData = new SliceData(slicerResults.rows, slicerResults.columns, slicerResults.cells);

                    meta.Profiles[0].Slices = slicerData;
                }

                await _libraryRepo.WriteMeta(id, meta);
                return Ok(slicerData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

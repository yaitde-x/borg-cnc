using System;
using System.Threading.Tasks;
using Borg.Machine;
using Microsoft.AspNetCore.Mvc;
using Yuni.Library;
using Yuni.Query;

namespace borg_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalysisController : ControllerBase
    {
        private readonly ILibraryRepo _libraryRepo;

        public AnalysisController(ILibraryRepo libraryRepo)
        {
            _libraryRepo = libraryRepo;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RS274Analysis>> GetAnalysis([FromRoute]string id)
        {
            try
            {
                return await _libraryRepo.GetAnalysis(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("analyze/{id}")]
        public async Task<ActionResult<RS274Analysis>> Analyze([FromRoute]string id)
        {
            try
            {
                using (var stream = await _libraryRepo.Read(id))
                {
                    var analyzer = new RS274Analyzer(new RS274Parser());
                    var analysis = await analyzer.Scan(stream);
                    await _libraryRepo.WriteAnalysis(id, analysis);
                    return analysis;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

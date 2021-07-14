using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Borg.Machine;
using Microsoft.AspNetCore.Mvc;
using Yuni.Library;

namespace borg_api.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class LibraryController : ControllerBase
    {
        private readonly ILibraryRepo _libraryRepo;
        private readonly RS274MetaBuilder _metaBuilder;

        public LibraryController(ILibraryRepo libraryRepo, RS274MetaBuilder metaBuilder)
        {
            _libraryRepo = libraryRepo;
            _metaBuilder = metaBuilder;
        }

        [HttpGet("all")]
        public async Task<ActionResult<RS274Meta[]>> AllItems()
        {
            try
            {
                var items = await _libraryRepo.AllItems();
                return items.ToArray();
            }
            catch (LibraryException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("item")]
        public async Task<ActionResult<Stream>> GetItem(string id)
        {
            try
            {
                var meta = await _libraryRepo.GetMeta(id);
                var stream = await _libraryRepo.Read(id);
                return stream;
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
            catch (LibraryException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("item/{id}")]
        public async Task<ActionResult> CreateItem(string id, [FromQuery] string name, [FromQuery] string fileName)
        {
            try
            {
                var meta = _metaBuilder.Build(id, name, string.Empty, fileName);
                await _libraryRepo.WriteMeta(id, meta);
                using var fileStream = await _libraryRepo.Write(id);
                await Request.Body.CopyToAsync(fileStream);
                return Ok();
            }
            catch (LibraryException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("slice")]
        public async Task<ActionResult<Stream>> Slice(string id, int row, int column)
        {
            try
            {
                var stream = await _libraryRepo.Slice(id, row, column);
                return stream;
            }
            catch (LibraryException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

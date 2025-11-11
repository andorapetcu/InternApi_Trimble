using Microsoft.AspNetCore.Mvc;
using InternApi.ModelEntity;
using InternApi.ModelDTO;
using InternApi.Services;

namespace InternApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InternController : ControllerBase
    {

        InternService _internService;

        public InternController(InternService internService)
        {
            _internService = internService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _internService.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetIntern(Guid id)
        {
            var intern = await _internService.GetById(id);
            if (intern == null)
                return NotFound();
            else
                return Ok(intern);
        }

        [HttpGet("sortByName")]
        public async Task<IActionResult> SortByAscName()
        {
            return Ok(await _internService.SortAscByName());
        }

        [HttpGet("sortDescByName")]
        public async Task<IActionResult> SortDescByName()
        {
            return Ok(await _internService.SortDescByName());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InternDTO internDTO)
        {
            var result = await _internService.Create(internDTO);
            if (!result)
                return BadRequest("intern cannot be created");
            else
                return Ok(internDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] InternDTO internDTO, Guid id)
        {
            var result = await _internService.Update(id, internDTO);
            if (!result)
                return NotFound();
            else
                return Ok(internDTO);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIntern(Guid id)
        {
            var result = await _internService.Delete(id);
            if (!result)
                return NotFound();
            else
                return NoContent();
        }

    }
}

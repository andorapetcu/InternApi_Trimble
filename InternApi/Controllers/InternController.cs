using Microsoft.AspNetCore.Mvc;
using InternApi.ModelEntity;
using InternApi.ModelDTO;
using InternApi.Services;
using AutoMapper;

namespace InternApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InternController : ControllerBase
    {

        private readonly IInternService _internService;

        public InternController(IInternService internService)
        {
            _internService = internService ?? throw new ArgumentNullException(nameof(internService));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _internService.GetAll());
        }

        [HttpGet("{id}", Name = "GetInternById")]
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
            if (internDTO == null)
                return BadRequest("Intern data is missing");

            if (internDTO.Id == Guid.Empty)
                internDTO.Id = Guid.NewGuid();

            if (string.IsNullOrWhiteSpace(internDTO.Name))
                return BadRequest("Intern name cannot be empty");

            if (internDTO.Age < 18)
                return BadRequest("Intern age must be at least 18");

            if (internDTO.DateOfBirth > DateTime.UtcNow)
                return BadRequest("Date of birth cannot be in the future");

            if (internDTO.DateOfBirth.AddYears(internDTO.Age) > DateTime.UtcNow)
                return BadRequest("Age does not match date of birth");

            var result = await _internService.Create(internDTO);
            return CreatedAtRoute("GetInternById", new { id = internDTO.Id }, internDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] InternDTO internDTO, Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid intern ID");

            if (internDTO == null)
                return BadRequest("Intern new data is missing");

            if (internDTO.Id == Guid.Empty)
                internDTO.Id = id;

            if (string.IsNullOrWhiteSpace(internDTO.Name))
                return BadRequest("Intern name cannot be empty");

            if (internDTO.Age < 18)
                return BadRequest("Intern age must be at least 18");

            if (internDTO.DateOfBirth > DateTime.UtcNow)
                return BadRequest("Date of birth cannot be in the future");

            if (internDTO.DateOfBirth.AddYears(internDTO.Age) > DateTime.UtcNow)
                return BadRequest("Age does not match date of birth");

            if (await _internService.GetById(id) == null)
                return NotFound();

            var result = await _internService.Update(id, internDTO);
            return Ok(internDTO);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIntern(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid intern ID");

            if (await _internService.GetById(id) == null)
                return NotFound();

            var result = await _internService.Delete(id);
            return NoContent();
        }
    }
}

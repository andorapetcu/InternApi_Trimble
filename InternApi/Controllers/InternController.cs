using Microsoft.AspNetCore.Mvc;
using InternApi.ModelEntity;
using InternApi.ModelDTO;
using InternApi.Services;
using AutoMapper;

namespace InternApi.Controllers
{
    /// <summary>
    /// 
    /// Handles endpoint requests.
    /// Determines thecorrect HTTP status code to return based on the outcome of the operations.
    /// 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class InternController : ControllerBase
    {

        private readonly IInternService _internService;

        /// <summary>
        /// 
        /// InternContrller constructor that initializes the necessary variables.
        /// 
        /// </summary>
        /// <param name="internService"> Instance of the InternService that provides access to the intern business operations. </param>
        /// <exception cref="ArgumentNullException"> Thrown if the parameter passed to the constructor in null. </exception>
        public InternController(IInternService internService)
        {
            _internService = internService ?? throw new ArgumentNullException(nameof(internService));
        }

        /// <summary>
        /// 
        /// Retrieves a list of all Interns.
        /// This endpoint calls the asynchronous GetAll method in the InternService layer to fetch all intern records from the database.
        /// 
        /// </summary>
        /// <returns>
        /// A "Task{TResult}" representing the asynchronous operation.
        /// The task result is an "IActionResult" containing a list of InternDTOs>.
        /// </returns>
        /// <response code="200"> Returns the complete list of Interns (may be empty if no records are found).</response>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _internService.GetAll());
        }

        /// <summary>
        /// 
        /// Retrieves the intern that matches the id sent by the client.
        /// This endpoint calls the asynchronous GetById method in the InternService layer to fetch the requested intern.
        /// 
        /// </summary>
        /// <param name="id"> The id of the intern that should be returned. </param>
        /// <returns>
        /// A "Task{TResult}" representing the asynchronous operation.
        /// The task result is an "IActionResult" containing the requested intern>.
        /// </returns>
        /// <response code="200"> Returns the internmatching the id. </response>
        /// <response code="404"> Id does not match any interns from the database. </response>
        /// <response code="400"> The response body will contain a message explaining the validation error. </response>
        [HttpGet("{id}", Name = "GetInternById")]
        public async Task<IActionResult> GetIntern(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid intern ID");
            var intern = await _internService.GetById(id);
            if (intern == null)
                return NotFound();
            else
                return Ok(intern);
        }

        /// <summary>
        /// 
        /// Retrieves a list of sorted Interns by name (A-Z).
        /// This endpoint calls the asynchronous SortAscByName method in the InternService layer to fetch and sort all intern records from the database.
        /// 
        /// </summary>
        /// <returns>
        /// A "Task{TResult}" representing the asynchronous operation.
        /// The task result is an "IActionResult" containing a list of InternDTOs sorted by name>.
        /// </returns>
        /// <response code="200"> Returns the list of sorted Interns.</response>
        [HttpGet("sortByName")]
        public async Task<IActionResult> SortByAscName()
        {
            return Ok(await _internService.SortAscByName());
        }

        /// <summary>
        /// 
        /// Retrieves a list of sorted Interns by name in descnding order (Z-A).
        /// This endpoint calls the asynchronous SortDescByName method in the InternService layer to fetch and sort all intern records from the database.
        /// 
        /// </summary>
        /// <returns>
        /// A "Task{TResult}" representing the asynchronous operation.
        /// The task result is an "IActionResult" containing a list of InternDTOs sorted by name in descending order. >.
        /// </returns>
        /// <response code="200"> Returns the list of sorted Interns.</response>
        [HttpGet("sortDescByName")]
        public async Task<IActionResult> SortDescByName()
        {
            return Ok(await _internService.SortDescByName());
        }

        /// <summary>
        /// 
        /// Creates a new intern.
        /// This endpoint calls the asynchrounous Create method in the InternService layer to create a new intern in the database with the recieved data.
        /// 
        /// </summary>
        /// <param name="internDTO"> The internDTO containing the data about the intern that should be created. </param>
        /// <returns>
        /// A "Task{TResult}" representing the asynchronous operation.
        /// The task result is an "IActionResult" indicating the outcome of the creation attempt. >.
        /// </returns>
        /// <response code="201"> Returns the newly created internDTO. 
        /// The response includes a 'Location' header pointing to the new resource's path (using GetInternById). </response>
        /// <response code="400"> The response body will contain a message explaining the validation error. </response>
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

        /// <summary>
        /// 
        /// Updates an intern Entity with the new internDTO sent by the client.
        /// This endpoint calls the asynchrounous Update method in the InternService layer to replace the old intern data with the one sent by the client.
        /// 
        /// </summary>
        /// <param name="internDTO"> The internDTO containing the data that will replace the current intern data. </param>
        /// <param name="id">The id of the intern that should be updated. </param>
        /// <returns>
        /// A "Task{TResult}" representing the asynchronous operation.
        /// The task result is an "IActionResult" indicating the outcome of the update attempt. >.
        /// </returns>
        /// <response code="200"> Returns the newly updated internDTO. 
        /// <response code="404"> No user with the same id was found. </response>
        /// <response code="400"> The response body will contain a message explaining the validation error. </response>
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

        /// <summary>
        /// 
        /// Deletes the intern that matches the id sent by the client.
        /// This endpoint calls the asynchrounous Delee method in the InternService layer to delete an intern from the database.
        /// 
        /// </summary>
        /// <param name="id">The id of the intern that should be deleted. </param>
        /// <returns>
        /// A "Task{TResult}" representing the asynchronous operation.
        /// The task result is an "IActionResult" indicating the outcome of the delete attempt.
        /// </returns>
        /// <response code="204"> Indicates successful deletion with no content returned in the response body. </response>  
        /// <response code="404"> No user with the same id was found. </response>
        /// <response code="400"> The response body will contain a message explaining the validation error. </response>
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

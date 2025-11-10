//using Microsoft.AspNetCore.Mvc;
//using InternApi.ModelEntity;
//using InternApi.ModelDTO;

//namespace InternApi.Controllers
//{
//    [ApiController]
//    [Route("[controller]")]
//    public class InternController : ControllerBase
//    {
//       private static readonly Dictionary<Guid, Intern> Interns = new();

//        static InternController()
//        {
//            var intern1 = new Intern
//            {
//                Name = "Alice Johnson",
//                Age = 22,
//                Date = new DateTime(2003, 1, 15)
//            };
//            var intern2 = new Intern
//            {
//                Name = "Bob Smith",
//                Age = 24,
//                Date = new DateTime(2005, 7, 23)
//            };
//            Interns[intern1.Id] = intern1;
//            Interns[intern2.Id] = intern2;
//        }


//        [HttpGet("{id}")]
//        public ActionResult<InternDTO> GetInternById(Guid id)
//        {
//            if (Interns.TryGetValue(id, out var intern))
//            {
//                var internDTO = new InternDTO
//                {
//                    Id = intern.Id,
//                    Name = intern.Name,
//                    Age = intern.Age,
//                    date = intern.Date
//                };
//                return Ok(internDTO);
//            }
//            return NotFound();
//        }
//        [HttpPost]
//        public ActionResult<InternDTO> CreateIntern(InternDTO internDTO)
//        {
//            var intern = new Intern
//            {
//                Id = Guid.NewGuid(),
//                Name = internDTO.Name,
//                Age = internDTO.Age,
//                Date = internDTO.date
//            };
//            Interns[intern.Id] = intern;
//            internDTO.Id = intern.Id;
//            return CreatedAtAction(nameof(GetInternById), new { id = intern.Id }, internDTO);
//        }
//    }
//}

using AutoMapper;
using InternApi.ModelDTO;
using InternApi.ModelEntity;
using MongoDB.Driver;

namespace InternApi.Services
{
    /// <summary>
    /// 
    /// Implementation of the IInternService interface. 
    /// The methods in InternService are asynchronous because they rely on communication with the external MongoDB database. 
    /// The keyword "await" pauses execution until the result is available.
    /// Executes validations and handles the business logic.
    /// 
    /// </summary>
    public class InternService : IInternService
    {
        private readonly IMongoCollection<Intern> _interns;
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// InternSevice constructor that initializes the necessary variables.
        /// 
        /// </summary>
        /// <param name="internsCollection"> Intern collection from the MongoDB database. </param>
        /// <param name="mapper"> Instance of the Mapper used to convert an input object of one type into an input object of another type. </param>
        public InternService(IMongoCollection<Intern> internsCollection, IMapper mapper)
        {
            _interns = internsCollection;
            _mapper = mapper;
        }

        /// <summary>
        /// 
        /// Retrieves all the interns from the database.
        /// FindAsync asynchronously finds the entries from the intern collection that match the condition "true".
        /// 
        /// </summary>
        /// <returns> List of internDTOs </returns>
        public async Task<List<InternDTO>> GetAll()
        {
            var result = await _interns.FindAsync(intern => true);
            var interns = await result.ToListAsync();
            return _mapper.Map<List<InternDTO>>(interns);
        }

        /// <summary>
        /// 
        /// Retrieves an intern from the database that matches the id sent by the client.
        /// 
        /// </summary>
        /// <param name="id"> The id of the intern that should be returned. </param>
        /// <returns> The requested InternDTO </returns>
        public async Task<InternDTO?> GetById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }
            var cursor = await _interns.FindAsync(intern => intern.Id == id);
            var intern = await cursor.FirstOrDefaultAsync();
            if (intern == null)
            {
                return null;
            }
            return _mapper.Map<InternDTO>(intern);
        }

        /// <summary>
        /// 
        /// Retrieves all the interns from the database and then orders them by their names (A-Z).
        /// 
        /// </summary>
        /// <returns> The list of sorted InternDTOs </returns>
        public async Task<List<InternDTO>> SortAscByName()
        {
            var interns = await GetAll();
            var sorted = interns.OrderBy(intern => intern.Name).ToList();
            return _mapper.Map<List<InternDTO>>(sorted);
        }

        /// <summary>
        /// 
        /// Retrieves all the interns from the database and then orders them by their names in descending order (Z-A).
        /// 
        /// </summary>
        /// <returns> The list of sorted InternDTOs </returns>
        public async Task<List<InternDTO>> SortDescByName()
        {
            var interns = await GetAll();
            var sorted = interns.OrderByDescending(intern => intern.Name).ToList();
            return _mapper.Map<List<InternDTO>>(sorted);
        }

        /// <summary>
        /// 
        /// Validates the input sent by the client.
        /// verifies if a duplicate user exists.
        /// Creates a new intern with the recieved data.
        /// 
        /// </summary>
        /// <param name="internDTO"> The data of the intern that should be created. </param>
        /// <returns> True or False depending on the operation success. </returns>
        public async Task<bool> Create(InternDTO internDTO)
        {
            if (internDTO == null)
                return false;

            if (await GetById(internDTO.Id) != null)
                return false;

            if (string.IsNullOrWhiteSpace(internDTO.Name))
                return false;

            var intern = _mapper.Map<Intern>(internDTO);
            await _interns.InsertOneAsync(intern);
            return true;
        }

        /// <summary>
        /// 
        /// Validates the input sent by the client.
        /// Validates if the user that should be updated exists in the database.
        /// Replaces the old intern data with the one sent by the client.
        /// 
        /// </summary>
        /// <param name="id"> The id of the intern that should pe updated. </param>
        /// <param name="internDTO"> The data of the intern that should replace the old intern. </param>
        /// <returns> True or false depending on the operation success. </returns>
        public async Task<bool> Update(Guid id, InternDTO internDTO)
        {
            if (internDTO == null)
                return false;

            if (id == Guid.Empty)
                return false;

            if (await GetById(id) == null)
                return false;

            if (string.IsNullOrWhiteSpace(internDTO.Name))
                return false;

            var intern = _mapper.Map<Intern>(internDTO);
            var result = await _interns.ReplaceOneAsync(i => i.Id == id, intern);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// 
        /// Validates the input sent by the client.
        /// Validates if the user that should be deleted exists in the database.
        /// Deletes the intern from the database.
        /// 
        /// </summary>
        /// <param name="id"> The id of the intern that should be deleted. </param>
        /// <returns> True or False depending on the operation success. </returns>
        public async Task<bool> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return false;

            if (await GetById(id) == null)
                return false;

            var result = await _interns.DeleteOneAsync(intern => intern.Id == id);
            return result.DeletedCount > 0;
        }
    }
}

using AutoMapper;
using InternApi.ModelEntity;
using InternApi.ModelDTO;
using MongoDB.Driver;
using InternApi.Settings;

namespace InternApi.Services
{
    public class InternService
    {

        private readonly IMongoCollection<Intern> _interns;
        private readonly IMapper _mapper;

        public InternService(IMongoDBSettings settings, IMapper mapper)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _interns = database.GetCollection<Intern>("Interns");
            _mapper = mapper;
        }

        public async Task<List<InternDTO>> GetAll()
        {
            var result = await _interns.FindAsync(intern => true);
            var interns = await result.ToListAsync();
            return _mapper.Map<List<InternDTO>>(interns);
        }

        public async Task<InternDTO> GetById(Guid id)
        {
            var intern = await _interns.Find(intern => intern.Id == id).FirstOrDefaultAsync();
            if (intern == null)
            {
                return null;
            }
            return _mapper.Map<InternDTO>(intern);
        }


        public async Task<bool> Create(InternDTO internDTO)
        {
            var intern = _mapper.Map<Intern>(internDTO);
            if (intern == null)
            {
                return false;
            }
            if (intern.Id == Guid.Empty)
            {
                intern.Id = Guid.NewGuid();
            }
            await _interns.InsertOneAsync(intern);
            return true;
        }

        public async Task<bool> Update(Guid id, InternDTO internDTO)
        {
            var intern = _mapper.Map<Intern>(internDTO);
            if (intern == null)
            {
                return false;
            }
            var result = await _interns.ReplaceOneAsync(i => i.Id == id, intern);
            return true;
        }

        public async Task<bool> Delete(Guid id)
        {
            var result = await _interns.DeleteOneAsync(intern => intern.Id == id);
            return result.DeletedCount > 0;
        }
    }
}

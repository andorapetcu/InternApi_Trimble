using AutoMapper;
using InternApi.ModelDTO;
using InternApi.ModelEntity;
using InternApi.Settings;
using MongoDB.Driver;
using System.Globalization;

namespace InternApi.Services
{
    public class InternService : IInternService
    {
        private readonly IMongoCollection<Intern> _interns;
        private readonly IMapper _mapper;

        public InternService(IMongoCollection<Intern> internsCollection, IMapper mapper)
        {
            _interns = internsCollection;
            _mapper = mapper;
        }

        public async Task<List<InternDTO>> GetAll()
        {
            var result = await _interns.FindAsync(intern => true);
            var interns = await result.ToListAsync();
            return _mapper.Map<List<InternDTO>>(interns);
        }

        public async Task<InternDTO?> GetById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }
            var intern = await _interns.Find(intern => intern.Id == id).FirstOrDefaultAsync();
            if (intern == null)
            {
                return null;
            }
            return _mapper.Map<InternDTO>(intern);
        }

        public async Task<List<InternDTO>> SortAscByName()
        {
            var interns = await GetAll();
            var sorted = interns.OrderBy(intern => intern.Name).ToList();
            return _mapper.Map<List<InternDTO>>(sorted);
        }

        public async Task<List<InternDTO>> SortDescByName()
        {
            var interns = await GetAll();
            var sorted = interns.OrderByDescending(intern => intern.Name).ToList();
            return _mapper.Map<List<InternDTO>>(sorted);
        }

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

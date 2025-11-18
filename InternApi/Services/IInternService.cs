using InternApi.ModelDTO;

namespace InternApi.Services
{
    /// <summary>
    /// 
    /// Interface fixing the methods that InternService will implement.
    /// The methods return a Task because all database operations take a longer time on MongoDB.
    /// 
    /// </summary>
    public interface IInternService
    {
        Task<List<InternDTO>> GetAll();
        Task<InternDTO?> GetById(Guid id);
        Task<bool> Create(InternDTO intern);
        Task<bool> Update(Guid id, InternDTO intern);
        Task<bool> Delete(Guid id);
        Task<List<InternDTO>> SortAscByName();
        Task<List<InternDTO>> SortDescByName();
    }
}

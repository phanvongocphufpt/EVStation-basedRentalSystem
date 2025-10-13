using Repository.Entities;

namespace Repository.IRepositories
{
    public interface ICitizenIdRepository
    {
        Task<IEnumerable<CitizenId>> GetAllAsync();
        Task<CitizenId?> GetByIdAsync(int id);
        Task<CitizenId> CreateAsync(CitizenId citizenId);
        Task<CitizenId> UpdateAsync(CitizenId citizenId);
        Task<bool> DeleteAsync(int id);
    }
}



using Repository.Entities;

namespace Service.IServices
{
    public interface ICitizenIdService
    {
        Task<IEnumerable<CitizenId>> GetAllAsync();
        Task<CitizenId?> GetByIdAsync(int id);
        Task<CitizenId> CreateAsync(CitizenId citizenId);
        Task<CitizenId> UpdateAsync(CitizenId citizenId);
        Task<bool> DeleteAsync(int id);
    }
}



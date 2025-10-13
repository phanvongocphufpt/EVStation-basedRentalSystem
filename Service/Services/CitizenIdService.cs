using Repository.Entities;
using Repository.IRepositories;
using Service.IServices;

namespace Service.Services
{
    public class CitizenIdService : ICitizenIdService
    {
        private readonly ICitizenIdRepository _citizenIdRepository;

        public CitizenIdService(ICitizenIdRepository citizenIdRepository)
        {
            _citizenIdRepository = citizenIdRepository;
        }

        public async Task<IEnumerable<CitizenId>> GetAllAsync()
        {
            return await _citizenIdRepository.GetAllAsync();
        }

        public async Task<CitizenId?> GetByIdAsync(int id)
        {
            return await _citizenIdRepository.GetByIdAsync(id);
        }

        public async Task<CitizenId> CreateAsync(CitizenId citizenId)
        {
            return await _citizenIdRepository.CreateAsync(citizenId);
        }

        public async Task<CitizenId> UpdateAsync(CitizenId citizenId)
        {
            return await _citizenIdRepository.UpdateAsync(citizenId);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _citizenIdRepository.DeleteAsync(id);
        }
    }
}



using Repository.Entities;
using Repository.IRepositories;
using Service.IServices;

namespace Service.Services
{
    public class DriverLicenseService : IDriverLicenseService
    {
        private readonly IDriverLicenseRepository _driverLicenseRepository;

        public DriverLicenseService(IDriverLicenseRepository driverLicenseRepository)
        {
            _driverLicenseRepository = driverLicenseRepository;
        }

        public async Task<IEnumerable<DriverLicense>> GetAllAsync()
        {
            return await _driverLicenseRepository.GetAllAsync();
        }

        public async Task<DriverLicense?> GetByIdAsync(int id)
        {
            return await _driverLicenseRepository.GetByIdAsync(id);
        }

        public async Task<DriverLicense> CreateAsync(DriverLicense driverLicense)
        {
            return await _driverLicenseRepository.CreateAsync(driverLicense);
        }

        public async Task<DriverLicense> UpdateAsync(DriverLicense driverLicense)
        {
            return await _driverLicenseRepository.UpdateAsync(driverLicense);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _driverLicenseRepository.DeleteAsync(id);
        }
    }
}



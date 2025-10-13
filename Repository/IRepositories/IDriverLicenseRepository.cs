using Repository.Entities;

namespace Repository.IRepositories
{
    public interface IDriverLicenseRepository
    {
        Task<IEnumerable<DriverLicense>> GetAllAsync();
        Task<DriverLicense?> GetByIdAsync(int id);
        Task<DriverLicense> CreateAsync(DriverLicense driverLicense);
        Task<DriverLicense> UpdateAsync(DriverLicense driverLicense);
        Task<bool> DeleteAsync(int id);
    }
}



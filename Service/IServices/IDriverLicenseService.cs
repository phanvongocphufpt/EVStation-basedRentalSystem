using Repository.Entities;

namespace Service.IServices
{
    public interface IDriverLicenseService
    {
        Task<IEnumerable<DriverLicense>> GetAllAsync();
        Task<DriverLicense?> GetByIdAsync(int id);
        Task<DriverLicense> CreateAsync(DriverLicense driverLicense);
        Task<DriverLicense> UpdateAsync(DriverLicense driverLicense);
        Task<bool> DeleteAsync(int id);
    }
}



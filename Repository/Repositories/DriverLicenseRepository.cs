using Microsoft.EntityFrameworkCore;
using Repository.Context;
using Repository.Entities;
using Repository.IRepositories;

namespace Repository.Repositories
{
    public class DriverLicenseRepository : IDriverLicenseRepository
    {
        private readonly EVSDbContext _context;

        public DriverLicenseRepository(EVSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DriverLicense>> GetAllAsync()
        {
            return await _context.DriverLicenses
                .Include(d => d.User)
                .ToListAsync();
        }

        public async Task<DriverLicense?> GetByIdAsync(int id)
        {
            return await _context.DriverLicenses
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<DriverLicense> CreateAsync(DriverLicense driverLicense)
        {
            driverLicense.CreatedAt = DateTime.UtcNow;
            _context.DriverLicenses.Add(driverLicense);
            await _context.SaveChangesAsync();
            return driverLicense;
        }

        public async Task<DriverLicense> UpdateAsync(DriverLicense driverLicense)
        {
            driverLicense.UpdatedAt = DateTime.UtcNow;
            _context.DriverLicenses.Update(driverLicense);
            await _context.SaveChangesAsync();
            return driverLicense;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.DriverLicenses.FindAsync(id);
            if (item == null) return false;
            _context.DriverLicenses.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}



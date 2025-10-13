using Microsoft.EntityFrameworkCore;
using Repository.Context;
using Repository.Entities;
using Repository.IRepositories;

namespace Repository.Repositories
{
    public class CitizenIdRepository : ICitizenIdRepository
    {
        private readonly EVSDbContext _context;

        public CitizenIdRepository(EVSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CitizenId>> GetAllAsync()
        {
            return await _context.CitizenIds
                .Include(c => c.User)
                .ToListAsync();
        }

        public async Task<CitizenId?> GetByIdAsync(int id)
        {
            return await _context.CitizenIds
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CitizenId> CreateAsync(CitizenId citizenId)
        {
            citizenId.CreatedAt = DateTime.UtcNow;
            _context.CitizenIds.Add(citizenId);
            await _context.SaveChangesAsync();
            return citizenId;
        }

        public async Task<CitizenId> UpdateAsync(CitizenId citizenId)
        {
            citizenId.UpdatedAt = DateTime.UtcNow;
            _context.CitizenIds.Update(citizenId);
            await _context.SaveChangesAsync();
            return citizenId;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.CitizenIds.FindAsync(id);
            if (item == null) return false;
            _context.CitizenIds.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}



using Microsoft.EntityFrameworkCore;
using Repository.Context;
using Repository.Entities;
using Repository.IRepositories;


namespace Repository.Repositories
{
    public class CarDeliveryHistoryRepository : ICarDeliveryHistoryRepository
    {
        private readonly EVSDbContext _context;

        public CarDeliveryHistoryRepository(EVSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CarDeliveryHistory>> GetAllAsync()
        {
            return await _context.CarDeliveryHistories.ToListAsync();
        }

        public async Task<CarDeliveryHistory?> GetByIdAsync(int id)
        {
            return await _context.CarDeliveryHistories.FindAsync(id);
        }

        public async Task AddAsync(CarDeliveryHistory entity)
        {
            await _context.CarDeliveryHistories.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CarDeliveryHistory entity)
        {
            _context.CarDeliveryHistories.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(CarDeliveryHistory entity)
        {
            _context.CarDeliveryHistories.Remove(entity);
            await _context.SaveChangesAsync();
        }

     


      
    }
}

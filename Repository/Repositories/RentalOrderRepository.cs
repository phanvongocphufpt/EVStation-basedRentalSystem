using Microsoft.EntityFrameworkCore;
using Repository.Context;
using Repository.Entities;
using Repository.IRepositories;

namespace Repository.Repositories
{
    public class RentalOrderRepository : IRentalOrderRepository
    {
        private readonly EVSDbContext _context;

        public RentalOrderRepository(EVSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RentalOrder>> GetAllAsync()
        {
            return await _context.RentalOrders
                .Include(r => r.User)
                .Include(r => r.Car)
                .Include(r => r.RentalContact)
                .ToListAsync();
        }

        public async Task<RentalOrder?> GetByIdAsync(int id)
        {
            return await _context.RentalOrders
                .Include(r => r.User)
                .Include(r => r.Car)
                .Include(r => r.RentalContact)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<RentalOrder> CreateAsync(RentalOrder rentalOrder)
        {
            rentalOrder.CreatedAt = DateTime.UtcNow;
            _context.RentalOrders.Add(rentalOrder);
            await _context.SaveChangesAsync();
            return rentalOrder;
        }

        public async Task<RentalOrder> UpdateAsync(RentalOrder rentalOrder)
        {
            rentalOrder.UpdatedAt = DateTime.UtcNow;
            _context.RentalOrders.Update(rentalOrder);
            await _context.SaveChangesAsync();
            return rentalOrder;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rentalOrder = await _context.RentalOrders.FindAsync(id);
            if (rentalOrder == null)
                return false;

            _context.RentalOrders.Remove(rentalOrder);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}



using Microsoft.EntityFrameworkCore;
using Repository.Context;

using Repository.Entities;
using Repository.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                .Include(x => x.Payments)
                .Include(x => x.RentalLocation)
                .ToListAsync();
        }
        public async Task<IEnumerable<RentalOrder>> GetOrderByLocationAsync(int locationId)
        {
            return await _context.RentalOrders.Where(x => x.RentalLocationId == locationId)
                .Include(x => x.Payments)
                .Include(x => x.Car)
                .Include(x => x.RentalLocation)
                .ToListAsync();
        }

        public async Task<RentalOrder?> GetByIdAsync(int id)
        {
            return await _context.RentalOrders
                .Include(x => x.Payments)
                .Include(x => x.Car)
                .Include(x => x.RentalLocation)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<RentalOrder>> GetByUserIdAsync(int userId)
        {
            return await _context.RentalOrders
                .Include(x => x.Payments)
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }
        public async Task<IEnumerable<RentalOrder>> GetOrdersByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.RentalOrders
                .Include(x => x.Payments)
                .Include(x => x.Car)
                .Include(x => x.User)
                .Where(x => x.User.PhoneNumber == phoneNumber)
                .ToListAsync();
        }
        public async Task AddAsync(RentalOrder order)
        {
            await _context.RentalOrders.AddAsync(order);
        }

        public async Task UpdateAsync(RentalOrder order)
        {
            _context.RentalOrders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

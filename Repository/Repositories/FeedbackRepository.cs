using Microsoft.EntityFrameworkCore;
using Repository.Context;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly EVSDbContext _context;

        public FeedbackRepository(EVSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Feedback>> GetAllAsync()
        {
            return await _context.Feedbacks
                .Where(f => !f.IsDeleted)
                .Include(f => f.User)
                .Include(f => f.RentalOrder)
                    .ThenInclude(r => r.Car) // 🔍 lấy luôn thông tin xe
                .ToListAsync();
        }

        public async Task<Feedback> GetByCarName(string carName)
        {
            return await _context.Feedbacks
                .Include(f => f.RentalOrder)
                .ThenInclude(o => o.Car)
                .Where(f => !f.IsDeleted &&
                            f.RentalOrder.Car.Name.ToLower().Contains(carName.ToLower()))
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Feedback>> GetByUserIdAsync(int userId)
        {
            return await _context.Feedbacks
                .Where(f => f.UserId == userId && !f.IsDeleted)
                .Include(f => f.User)
                .Include(f => f.RentalOrder)
                    .ThenInclude(r => r.Car)
                .ToListAsync();
        }

        public async Task AddAsync(Feedback feedback)
        {
            await _context.Feedbacks.AddAsync(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Feedback feedback)
        {
            _context.Feedbacks.Update(feedback);
            await _context.SaveChangesAsync();
        }

        // ✅ XÓA MỀM (IsDeleted = true)
        public async Task DeleteAsync(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null && !feedback.IsDeleted)
            {
                feedback.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }

       
      
    }
}

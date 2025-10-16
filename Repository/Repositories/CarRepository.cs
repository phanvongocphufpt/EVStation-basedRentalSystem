using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Repository.Context;
using Repository.Entities;
using Repository.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Repository.Repositories
{
    public class CarRepository : ICarRepository
    {
        private readonly EVSDbContext _context;

        public CarRepository(EVSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Car>> GetAllAsync()
        {
            return await _context.Cars.Where(c => !c.IsDeleted).ToListAsync();
        }

        public async Task<Car> GetByNameAsync(string name)
        {
            return await _context.Cars
               .Where(c => !c.IsDeleted && c.Name.ToLower().Contains(name.ToLower()))
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(Car car)
        {
            await _context.Cars.AddAsync(car);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Car car)
        {
            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                car.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}

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
        public async Task<Car?> GetByIdAsync(int id)
        {
            return await _context.Cars
                .Where(c => !c.IsDeleted && c.Id == id)
                .FirstOrDefaultAsync();
        }
        public async Task<Car?> GetByNameAsync(string name)
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
            var tracked = _context.Cars.Local.FirstOrDefault(c => c.Id == car.Id);
            
            if (tracked != null)
            {
                await _context.SaveChangesAsync();
            }
            else
            {
                var existing = await _context.Cars.FindAsync(car.Id);
                if (existing != null)
                {
                    existing.Name = car.Name;
                    existing.Model = car.Model;
                    existing.Seats = car.Seats;
                    existing.SizeType = car.SizeType;
                    existing.TrunkCapacity = car.TrunkCapacity;
                    existing.BatteryType = car.BatteryType;
                    existing.DepositOrderAmount = car.DepositOrderAmount;
                    existing.DepositCarAmount = car.DepositCarAmount;
                    existing.BatteryDuration = car.BatteryDuration;
                    existing.RentPricePerDay = car.RentPricePerDay;
                    existing.RentPricePer4Hour = car.RentPricePer4Hour;
                    existing.RentPricePer8Hour = car.RentPricePer8Hour;
                    existing.RentPricePerDayWithDriver = car.RentPricePerDayWithDriver;
                    existing.RentPricePer4HourWithDriver = car.RentPricePer4HourWithDriver;
                    existing.RentPricePer8HourWithDriver = car.RentPricePer8HourWithDriver;
                    existing.ImageUrl = car.ImageUrl;
                    existing.ImageUrl2 = car.ImageUrl2;
                    existing.ImageUrl3 = car.ImageUrl3;
                    existing.IsActive = car.IsActive;
                    existing.ReportNote = car.ReportNote;
                    existing.UpdatedAt = car.UpdatedAt ?? DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
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

        // ✅ Hàm mới: lấy top xe thuê nhiều nhất
        public async Task<IEnumerable<Car>> GetTopRentedAsync(int topCount)
        {
            var query = _context.RentalOrders
                .Include(r => r.Car)
                .GroupBy(r => r.CarId)
                .Select(g => new
                {
                    Car = g.First().Car,
                    RentalCount = g.Count()
                })
                .OrderByDescending(x => x.RentalCount)
                .Take(topCount)
                .Select(x => x.Car);

            return await query.ToListAsync();
        }
        public async Task<IEnumerable<Car>> GetCarsByLocationAsync(int locationId)
        {
            return await _context.Cars
                .Where(c => !c.IsDeleted && c.RentalLocationId == locationId)
                .ToListAsync();
        }
    }
}
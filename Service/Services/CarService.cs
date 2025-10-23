using Repository.Entities;
using Repository.IRepositories;
using Service.Common.Service.Common;
using Service.DTOs;
using Service.IServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;

        public CarService(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        public async Task<IEnumerable<Car>> GetAllAsync()
        {
            return await _carRepository.GetAllAsync();
        }

        public async Task<Car> GetByNameAsync(string name)
        {
            return await _carRepository.GetByNameAsync(name);
        }

        public async Task<Pagination<Car>> GetPagedAsync(int pageIndex, int pageSize, string? keyword = null)
        {
            var cars = await _carRepository.GetAllAsync();
            var query = cars.Where(c => !c.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(keyword) || c.Model.ToLower().Contains(keyword));
            }

            var totalCount = query.Count();

            var pagedCars = query
                .OrderByDescending(c => c.CreatedAt)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            return new Pagination<Car>
            {
                TotalItemsCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Items = pagedCars
            };
        }

        public async Task AddAsync(Car car)
        {
            await _carRepository.AddAsync(car);
        }

        public async Task UpdateAsync(Car car)
        {
            await _carRepository.UpdateAsync(car);
        }

        public async Task DeleteAsync(int id)
        {
            await _carRepository.DeleteAsync(id);
        }

        // ✅ mới thêm
        public async Task<IEnumerable<TopRentCarDto>> GetTopRentedAsync(int topCount)
        {
            var cars = await _carRepository.GetTopRentedAsync(topCount);

            // Map entity -> DTO
            return cars.Select(c => new TopRentCarDto
            {
                CarId = c.Id,
                CarName = c.Name,
                Model = c.Model,
                ImageUrl = c.ImageUrl,
                Seats = c.Seats,
                SizeType = c.SizeType,
                IsActive = c.IsActive,
                RentalCount = c.RentalOrders?.Count ?? 0
            });
        }
    }
}

using Repository.Entities;
using Repository.IRepositories;
using Service.Common;
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

        public async Task<Result<IEnumerable<Car>>> GetAllAsync()
        {
            var cars = await _carRepository.GetAllAsync();
            var activeCars = cars.Where(c => !c.IsDeleted);

            if (!activeCars.Any())
                return Result<IEnumerable<Car>>.Failure("Không tìm thấy xe nào trong hệ thống.");

            return Result<IEnumerable<Car>>.Success(activeCars, "Lấy danh sách xe thành công.");
        }

        public async Task<Result<Car>> GetByNameAsync(string name)
        {
            var car = await _carRepository.GetByNameAsync(name);
            if (car == null || car.IsDeleted)
                return Result<Car>.Failure($"Không tìm thấy xe có tên '{name}'.");

            return Result<Car>.Success(car, "Lấy thông tin xe thành công.");
        }

        public async Task<Result<(IEnumerable<Car> Data, int Total)>> GetPagedAsync(int pageIndex, int pageSize, string? keyword = null)
        {
            var cars = await _carRepository.GetAllAsync();
            var query = cars.Where(c => !c.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(c =>
                    c.Name.ToLower().Contains(keyword) ||
                    c.Model.ToLower().Contains(keyword));
            }

            var totalCount = query.Count();
            var pagedCars = query
                .OrderByDescending(c => c.CreatedAt)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            var result = (Data: (IEnumerable<Car>)pagedCars, Total: totalCount);
            return Result<(IEnumerable<Car> Data, int Total)>.Success(result, "Lấy danh sách xe phân trang thành công.");
        }

        public async Task<Result<Car>> AddAsync(Car car)
        {
            if (string.IsNullOrWhiteSpace(car.Name))
                return Result<Car>.Failure("Tên xe không được để trống.");

            await _carRepository.AddAsync(car);
            return Result<Car>.Success(car, "Thêm xe mới thành công.");
        }

        public async Task<Result<Car>> UpdateAsync(Car car)
        {
            var existing = await _carRepository.GetByNameAsync(car.Name);
            if (existing == null)
                return Result<Car>.Failure("Xe cần cập nhật không tồn tại.");

            await _carRepository.UpdateAsync(car);
            return Result<Car>.Success(car, "Cập nhật thông tin xe thành công.");
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var cars = await _carRepository.GetAllAsync();
            var car = cars.FirstOrDefault(c => c.Id == id);

            if (car == null)
                return Result<bool>.Failure("Xe cần xóa không tồn tại.");

            await _carRepository.DeleteAsync(id);
            return Result<bool>.Success(true, "Đã xóa xe thành công.");
        }

        public async Task<Result<IEnumerable<TopRentCarDto>>> GetTopRentedAsync(int topCount)
        {
            var cars = await _carRepository.GetTopRentedAsync(topCount);

            if (cars == null || !cars.Any())
                return Result<IEnumerable<TopRentCarDto>>.Failure("Không có xe nào được thuê.");

            var dtoList = cars.Select(c => new TopRentCarDto
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

            return Result<IEnumerable<TopRentCarDto>>.Success(dtoList, "Lấy danh sách xe được thuê nhiều nhất thành công.");
        }
    }
}

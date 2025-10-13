using Repository.Entities;
using Repository.IRepositories;
using Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public Task<IEnumerable<Car>> GetAllAsync()
        {
            return _carRepository.GetAllAsync();
        }

        public Task<Car> SearchByNameAsync(string Name)
        {
            return _carRepository.SearchByNameAsync(Name);
        }

        public Task AddAsync(Car car)
        {
            return _carRepository.AddAsync(car);
        }

        public Task UpdateAsync(Car car)
        {
            return _carRepository.UpdateAsync(car);
        }

        public Task DeleteAsync(int id)
        {
            return _carRepository.DeleteAsync(id);
        }
    }
}

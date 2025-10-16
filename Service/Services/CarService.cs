using Repository.Entities;
using Repository.IRepositories;
using Repository.Repositories;
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

        public async Task<IEnumerable<Car>> GetAllAsync()
        {
            return await _carRepository.GetAllAsync();
        }

        public async Task<Car> GetByNameAsync(string name)
        {
            return await _carRepository.GetByNameAsync(name);
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
    }
}

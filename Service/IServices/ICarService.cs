using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface ICarService
    {
        Task<IEnumerable<Car>> GetAllAsync();
        Task<Car> GetByNameAsync(string name);   
        Task AddAsync(Car car);
        Task UpdateAsync(Car car);
        Task DeleteAsync(int id);
    }
}

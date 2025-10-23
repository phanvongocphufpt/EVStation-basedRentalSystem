using Repository.Entities;
using Service.DTOs;
using Service.Common.Service.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface ICarService
    {
        Task<IEnumerable<Car>> GetAllAsync();
        Task<Car> GetByNameAsync(string name);
        Task<Pagination<Car>> GetPagedAsync(int pageIndex, int pageSize, string? keyword = null);
        Task AddAsync(Car car);
        Task UpdateAsync(Car car);
        Task DeleteAsync(int id);

        // ✅ mới thêm
        Task<IEnumerable<TopRentCarDto>> GetTopRentedAsync(int topCount);
    }
}

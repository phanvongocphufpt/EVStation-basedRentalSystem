using Repository.Entities;
using Service.Common.Service.Common;
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

        // 🔹 Lấy danh sách xe có phân trang (ví dụ pageIndex = 0, pageSize = 10)
        Task<Pagination<Car>> GetPagedAsync(int pageIndex, int pageSize, string? keyword = null);
        Task AddAsync(Car car);
        Task UpdateAsync(Car car);
        Task DeleteAsync(int id);
    }
}

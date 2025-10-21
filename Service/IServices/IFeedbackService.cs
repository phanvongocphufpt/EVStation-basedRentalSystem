using Repository.Entities;
using Service.Common.Service.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IFeedbackService
    {
        // Lấy tất cả feedback
        Task<IEnumerable<Feedback>> GetAllAsync();
        Task<Feedback> GetByCarName(string carName); // 🔍 tìm feedback theo xe
        //Phân trang
        Task<Pagination<Feedback>> GetPagedAsync(int pageIndex, int pageSize, string? keyword = null);
        Task AddAsync(Feedback feedback);
        Task UpdateAsync(Feedback feedback);
        Task DeleteAsync(int id);
    }
}

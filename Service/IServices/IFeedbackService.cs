using Repository.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IFeedbackService
    {
        Task<IEnumerable<Feedback>> GetAllAsync();
        Task<Feedback> GetByCarName(string carName); // 🔍 tìm feedback theo xe
        Task<IEnumerable<Feedback>> GetByUserIdAsync(int userId);
        Task AddAsync(Feedback feedback);
        Task UpdateAsync(Feedback feedback);
        Task DeleteAsync(int id);
      
    }
}

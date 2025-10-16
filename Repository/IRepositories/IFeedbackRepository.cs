using Repository.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public interface IFeedbackRepository
    {
        Task<IEnumerable<Feedback>> GetAllAsync();
        Task<Feedback> GetByCarName(string carName); // 🔍 tìm feedback theo xe (RentalOrderId)
        Task<IEnumerable<Feedback>> GetByUserIdAsync(int userId);
        Task AddAsync(Feedback feedback);
        Task UpdateAsync(Feedback feedback);
        Task DeleteAsync(int id);
       
    }
}

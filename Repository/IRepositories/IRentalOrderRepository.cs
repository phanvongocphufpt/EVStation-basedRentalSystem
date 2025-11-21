using Repository.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IRentalOrderRepository
    {
        Task<IEnumerable<RentalOrder>> GetAllAsync();
        Task<RentalOrder?> GetByIdAsync(int id);
        Task<IEnumerable<RentalOrder>> GetOrderByLocationAsync(int locationId);
        Task<IEnumerable<RentalOrder>> GetByUserIdAsync(int userId);
        Task AddAsync(RentalOrder order);
        Task UpdateAsync(RentalOrder order);
        Task SaveChangesAsync();
    }
}

using Repository.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IRentalContactService
    {
        Task<IEnumerable<RentalContact>> GetAllAsync();
        Task<RentalContact?> GetByRentalOrderIdAsync(int rentalOrderId); // 🔍 get by RentalOrderId
        Task AddAsync(RentalContact contact);
        Task UpdateAsync(RentalContact contact);
        Task DeleteAsync(int id);
    }
}

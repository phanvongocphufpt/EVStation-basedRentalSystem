using Repository.Entities;

using System.Collections.Generic;

using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IRentalContactRepository
    {
        Task<IEnumerable<RentalContact>> GetAllAsync();
        Task<RentalContact> GetByIdAsync(int id);
        Task AddAsync(RentalContact rentalContact);
        Task UpdateAsync(RentalContact rentalContact);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}

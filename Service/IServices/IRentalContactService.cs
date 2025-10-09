using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IRentalContactService
    {
        Task<IEnumerable<RentalContact>> GetAllAsync();
        Task<RentalContact> GetByIdAsync(int id);
        Task AddAsync(RentalContact rentalContact);
        Task UpdateAsync(RentalContact rentalContact);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}

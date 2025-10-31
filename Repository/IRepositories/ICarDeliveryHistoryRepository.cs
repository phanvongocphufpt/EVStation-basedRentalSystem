using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repository.IRepositories
{
    public interface ICarDeliveryHistoryRepository
    {
        Task<IEnumerable<RentalLocation>> GetAllAsync();
        Task<RentalLocation?> GetByIdAsync(int id);
        Task AddAsync(RentalLocation rentalLocation);
        Task UpdateAsync(RentalLocation rentalLocation);
        Task DeleteAsync(int id);
    
}
}   

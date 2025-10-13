using Repository.Entities;
using Service.Common;

namespace Repository.IRepositories
{
    public interface IRentalOrderRepository
    {
        Task<IEnumerable<RentalOrder>> GetAllAsync();
        Task<RentalOrder?> GetByIdAsync(int id);
        Task<RentalOrder> CreateAsync(RentalOrder rentalOrder);
        Task<RentalOrder> UpdateAsync(RentalOrder rentalOrder);
        Task<bool> DeleteAsync(int id);
        Task<Pagination<RentalOrder>> GetPagedAsync(int pageIndex, int pageSize);
        Task<IEnumerable<RentalOrder>> GetByUserIdAsync(int userId);
        Task<IEnumerable<RentalOrder>> GetByCarIdAsync(int carId);
    }
}



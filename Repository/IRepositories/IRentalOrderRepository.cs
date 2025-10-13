using Repository.Entities;

namespace Repository.IRepositories
{
    public interface IRentalOrderRepository
    {
        Task<IEnumerable<RentalOrder>> GetAllAsync();
        Task<RentalOrder?> GetByIdAsync(int id);
        Task<RentalOrder> CreateAsync(RentalOrder rentalOrder);
        Task<RentalOrder> UpdateAsync(RentalOrder rentalOrder);
        Task<bool> DeleteAsync(int id);
    }
}



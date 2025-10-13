using Repository.Entities;

namespace Service.IServices
{
    public interface IRentalOrderService
    {
        Task<IEnumerable<RentalOrder>> GetAllAsync();
        Task<RentalOrder?> GetByIdAsync(int id);
        Task<RentalOrder> CreateAsync(RentalOrder rentalOrder);
        Task<RentalOrder> UpdateAsync(RentalOrder rentalOrder);
        Task<bool> DeleteAsync(int id);
    }
}



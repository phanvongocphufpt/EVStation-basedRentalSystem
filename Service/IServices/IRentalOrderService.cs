using Repository.Entities;
using Service.Common;

namespace Service.IServices
{
    public interface IRentalOrderService
    {
        Task<Result<IEnumerable<RentalOrder>>> GetAllAsync();
        Task<Result<RentalOrder>> GetByIdAsync(int id);
        Task<Result<RentalOrder>> CreateAsync(RentalOrder rentalOrder);
        Task<Result<RentalOrder>> UpdateAsync(RentalOrder rentalOrder);
        Task<Result<bool>> DeleteAsync(int id);
        Task<Result<Pagination<RentalOrder>>> GetPagedAsync(int pageIndex, int pageSize);
        Task<Result<IEnumerable<RentalOrder>>> GetByUserIdAsync(int userId);
        Task<Result<IEnumerable<RentalOrder>>> GetByCarIdAsync(int carId);
    }
}



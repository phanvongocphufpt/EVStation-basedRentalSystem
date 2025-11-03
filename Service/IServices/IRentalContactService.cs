using Repository.Entities;
using Service.Common;
using Service.Common.Service.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IRentalContactService
    {
        // ✅ Lấy tất cả liên hệ thuê xe
        Task<Result<IEnumerable<RentalContact>>> GetAllAsync();

        // ✅ Lấy liên hệ theo RentalOrderId
        Task<Result<RentalContact>> GetByRentalOrderIdAsync(int rentalOrderId);

        // ✅ Thêm mới
        Task<Result<RentalContact>> AddAsync(RentalContact contact);

        // ✅ Cập nhật
        Task<Result<RentalContact>> UpdateAsync(RentalContact contact);

        // ✅ Xóa
        Task<Result<bool>> DeleteAsync(int id);
    }
}

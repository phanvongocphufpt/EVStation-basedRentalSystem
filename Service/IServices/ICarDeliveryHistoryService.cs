using Service.Common;
using Service.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface ICarDeliveryHistoryService
    {
        // 🔹 Lấy danh sách phân trang
        Task<Result<(IEnumerable<CarDeliveryHistoryDTO> Data, int Total)>> GetAllAsync(int pageIndex, int pageSize);

        // 🔹 Lấy theo ID
        Task<Result<CarDeliveryHistoryDTO?>> GetByIdAsync(int id);

        // 🔹 Thêm lịch sử giao xe
        Task<Result<string>> AddAsync(CarDeliveryHistoryCreateDTO dto);

        // 🔹 Cập nhật lịch sử giao xe
        Task<Result<string>> UpdateAsync(CarDeliveryHistoryUpdateDTO dto);

        // 🔹 Xóa lịch sử giao xe
        Task<Result<string>> DeleteAsync(int id);
    }
}

using Service.Common;
using Service.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IRentalOrderService
    {
        // Lấy tất cả đơn thuê
        Task<Result<IEnumerable<RentalOrderDTO>>> GetAllAsync();

        // Lấy đơn thuê theo Id
        Task<Result<RentalOrderDTO>> GetByIdAsync(int id);

        // Lấy tất cả đơn thuê của user
        Task<Result<IEnumerable<RentalOrderDTO>>> GetByUserIdAsync(int id);

        // Tạo đơn thuê
        Task<Result<CreateRentalOrderDTO>> CreateAsync(CreateRentalOrderDTO createRentalOrderDTO);

        // Cập nhật trạng thái đơn thuê (cho phép chọn trạng thái mới)
        Task<Result<UpdateRentalOrderStatusDTO>> UpdateStatusAsync(UpdateRentalOrderStatusDTO updateRentalOrderStatusDTO);

        // Cập nhật tổng tiền đơn thuê
        Task<Result<UpdateRentalOrderTotalDTO>> UpdateTotalAsync(UpdateRentalOrderTotalDTO updateRentalOrderTotalDTO);

        // Xác nhận thanh toán cho đơn thuê
        Task<Result<bool>> ConfirmPaymentAsync(int orderId);
<<<<<<< Updated upstream
=======

        // Xác nhận tổng tiền cho đơn thuê
        Task<Result<bool>> ConfirmTotalAsync(int orderId);
>>>>>>> Stashed changes
    }
}

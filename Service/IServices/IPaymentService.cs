using Service.Common;
using Service.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IPaymentService
    {
        // ===== Payment thông thường =====
        Task<Result<IEnumerable<PaymentDTO>>> GetAllAsync();
        Task<Result<IEnumerable<PaymentDTO>>> GetAllByUserIdAsync(int id);
        Task<Result<PaymentDTO>> GetByIdAsync(int id);
        Task<Result<CreatePaymentDTO>> AddAsync(CreatePaymentDTO createPaymentDTO);
        Task<Result<UpdatePaymentStatusDTO>> UpdatePaymentStatusAsync(UpdatePaymentStatusDTO updatePaymentDTO);
        Task<Result<bool>> ConfirmDepositPaymentAsync(int orderId);
        Task<Result<IEnumerable<RevenueByLocationDTO>>> GetRevenueByLocationAsync();

        // ===== MoMo Integration =====

        /// <summary>
        /// Tạo thanh toán MoMo (trả về payUrl để redirect user)
        /// </summary>
        Task<Result<CreateMomoPaymentResponseDTO>> CreateMomoPaymentAsync(int rentalOrderId, int userId, double amount);

        /// <summary>
        /// Xử lý IPN từ MoMo (server-to-server)
        /// </summary>
        Task<Result<bool>> ProcessMomoIpnAsync(object payload); // payload có thể là JObject hoặc Dictionary

        /// <summary>
        /// Lấy payment theo MomoOrderId (dùng khi nhận IPN)
        /// </summary>
        Task<Result<PaymentDetailDTO>> GetPaymentByMomoOrderIdAsync(string momoOrderId);
    }
}

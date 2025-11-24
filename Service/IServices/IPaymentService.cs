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

        Task<Result<CreateMomoPaymentResponseDTO>> CreateMomoPaymentAsync(int rentalOrderId, int userId, double amount);
        Task<Result<bool>> ProcessMomoIpnAsync(object payload); // payload có thể là JObject hoặc Dictionary
        Task<Result<PaymentDetailDTO>> GetPaymentByMomoOrderIdAsync(string momoOrderId);

        // ===== PayOS Integration =====

        Task<Result<CreatePayOSPaymentResponseDTO>> CreatePayOSPaymentAsync(int rentalOrderId, int userId, double amount);
        Task<Result<bool>> ProcessPayOSIpnAsync(object payload);
        Task<Result<PaymentDetailDTO>> GetPaymentByPayOSOrderCodeAsync(int orderCode);
        
        // ===== Payment Status Check =====
        
        /// <summary>
        /// Kiểm tra và cập nhật payment status (dùng khi callback chưa được gọi)
        /// </summary>
        Task<Result<bool>> CheckAndUpdatePaymentStatusAsync(int paymentId);

        // ===== Unified Payment Gateway =====

        /// <summary>
        /// Tạo payment với gateway được chọn (MoMo, PayOS, Cash, BankTransfer)
        /// </summary>
        Task<Result<CreatePaymentResponseDTO>> CreatePaymentAsync(CreatePaymentRequestDTO request);

        /// <summary>
        /// Đổi phương thức thanh toán cho payment đã tạo
        /// </summary>
        Task<Result<CreatePaymentResponseDTO>> ChangePaymentGatewayAsync(ChangePaymentGatewayRequestDTO request);
    }
}
